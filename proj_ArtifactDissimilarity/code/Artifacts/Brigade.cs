using R2API.Utils;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;

namespace ArtifactDissimilarity.Aritfacts
{
    public class Brigade
    {
        private static List<EliteDef> Tier1EliteDefs = new List<EliteDef>();
        private static List<EliteDef> Tier2EliteDefs = new List<EliteDef>();
        public static List<EliteDef> forRandomization = new List<EliteDef>();
        public static EliteDef chosenEliteDef;
        //private static EliteDef NoRepeatForUsageEliteDef;
        // public static EquipmentIndex[] brigadedAffixes = Array.Empty<EquipmentIndex>();

        //public static bool DidBrigadeHappen;
        public static CombatDirector.EliteTierDef[] backupNormalEliteTiers;

        static int howManyTimesAddedTier2 = 1;

        public static void OnArtifactEnable()
        {
            SetupBrigade();
            backupNormalEliteTiers = CombatDirector.eliteTiers;
            if (Brigade.forRandomization.Count > 0 && SceneInfo.instance && Run.instance)
            {
                Brigade.EliteKinAsMethod();
            }
            Debug.Log("Added Brigade");
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
        }


        public static void OnArtifactDisable()
        {
            CombatDirector.eliteTiers = backupNormalEliteTiers;
            SceneDirector.onPrePopulateSceneServer -= SceneDirector_onPrePopulateSceneServer;
            Debug.Log("UnBrigading");
            Tier1EliteDefs.Clear();
            Tier2EliteDefs.Clear();
            forRandomization.Clear();
        }

        private static void SceneDirector_onPrePopulateSceneServer(SceneDirector obj)
        {
            Brigade.EliteKinAsMethod();
        }

        public static void SetupBrigade()
        {
            Tier1EliteDefs = new List<EliteDef>();
            Tier2EliteDefs = new List<EliteDef>();
            forRandomization = new List<EliteDef>();

            Debug.Log("EliteTiers : " + CombatDirector.eliteTiers.Length);
            for (int i = 0; i < CombatDirector.eliteTiers.Length; i++)
            {
                var tier = CombatDirector.eliteTiers[i];

                if (tier.eliteTypes.Length == 0 || tier.eliteTypes[0] == null || tier.eliteTypes[0].name.EndsWith("Honor"))
                {
                    continue;
                }
                foreach (EliteDef eliteDef in tier.eliteTypes)
                {
                    if (eliteDef.IsAvailable())
                    {
                        if (eliteDef.healthBoostCoefficient > 8f)
                        {
                            if (!Tier2EliteDefs.Contains(eliteDef))
                            {
                                Tier2EliteDefs.Add(eliteDef);
                            }
                        }
                        else
                        {
                            if (!Tier1EliteDefs.Contains(eliteDef))
                            {
                                Tier1EliteDefs.Add(eliteDef);
                            }
                        }
                    }
                }
            }

            Tier1EliteDefs.Remove(RoR2Content.Elites.Lunar);
            if (DLC1Content.Elites.Void.IsAvailable())
            {
                //Tier2EliteDefs.Add(DLC1Content.Elites.Void);
            }
            howManyTimesAddedTier2 = 1;
            forRandomization.AddRange(Tier1EliteDefs);
        }



        public class BrigadeMessage : RoR2.ChatMessageBase
        {
            public override string ConstructChatString()
            {
                string token = Language.GetString("ANNOUNCE_BRIGADE_ELITE");
                string token2 = string.Format(Language.GetString(eliteNameToken), "");

                return string.Format(token, token2);
            }

            public string eliteNameToken;

            public override void Serialize(NetworkWriter writer)
            {
                base.Serialize(writer);
                writer.Write(eliteNameToken);
            }

            public override void Deserialize(NetworkReader reader)
            {
                base.Deserialize(reader);
                eliteNameToken = reader.ReadString();
            }
        }

        public static void EliteKinAsMethod()
        {
            Debug.Log("COMBATDIRECTORS:" + CombatDirector.instancesList.Count);

            //CombatDirector.instancesList.Count = 0;
            if (!NetworkServer.active)
            {
                return;
            }
            chosenEliteDef = null;
            if (howManyTimesAddedTier2 <= Run.instance.NetworkstageClearCount / 5)
            {
                howManyTimesAddedTier2++;
                Debug.Log("Add Tier 2 to Brigade");
                forRandomization.AddRange(Tier2EliteDefs);
                forRandomization.AddRange(Tier2EliteDefs);
            }



            string baseSceneName = SceneInfo.instance.sceneDef.baseSceneName;
            switch (baseSceneName)
            {
                case "moon":
                case "moon2":
                    chosenEliteDef = RoR2Content.Elites.Lunar;
                    break;
                case "itmoon":
                    if (Run.instance.treasureRng.RangeInt(0, 5) == 0)
                    {
                        chosenEliteDef = RoR2Content.Elites.Lunar;
                    }
                    break;
                case "voidraid":
                    if (Run.instance.treasureRng.RangeInt(0, 4) == 0)
                    {
                        chosenEliteDef = DLC1Content.Elites.Void;
                    }
                    break;
                case "conduitcanyon":
                    chosenEliteDef = WolfoLibrary.MissedContent.Elites.CollectiveWeak;
                    break;
                default:
                    if (Run.instance.treasureRng.RangeInt(0, 200) == 0 && DLC1Content.Elites.Void.IsAvailable())
                    {
                        chosenEliteDef = DLC1Content.Elites.Void;
                    }
                    else
                    {
                        chosenEliteDef = forRandomization[Run.instance.treasureRng.RangeInt(0, forRandomization.Count)];
                    }
                    break;
            }

            if (chosenEliteDef == null)
            {
                Debug.LogError("NO ELITE DEF FOUND");
            }

            //Eulogy Lunar
            int itemCountGlobal = Util.GetItemCountGlobal(DLC1Content.Items.RandomlyLunar.itemIndex, false, false);
            if (itemCountGlobal > 0)
            {
                itemCountGlobal++;
                //itemCountGlobal = Math.Min(itemCountGlobal, 10);
                if (Run.instance.spawnRng.nextNormalizedFloat < 0.05f * (float)itemCountGlobal)
                {
                    chosenEliteDef = RoR2Content.Elites.Lunar;
                }
            }

            if (Run.instance is WeeklyRun)
            {
                (Run.instance as WeeklyRun).bossAffixes = new EquipmentIndex[]
                {
                        chosenEliteDef.eliteEquipmentDef.equipmentIndex
                };
            }

            Debug.Log("Artifact of Brigade: This stages only Elite " + chosenEliteDef.name);
 
            float CostMultiplier = 6;
            if (chosenEliteDef.healthBoostCoefficient > 8)
            {
                CostMultiplier = 30;
            }


            CombatDirector.EliteTierDef[] array = new CombatDirector.EliteTierDef[3];
            array[0] = new CombatDirector.EliteTierDef
            {
                costMultiplier = 1,
                eliteTypes = new EliteDef[1],
                canSelectWithoutAvailableEliteDef = true,
                isAvailable = ((SpawnCard.EliteRules rules) => !CombatDirector.IsEliteOnlyArtifactActive()),
            };
            array[1] = new CombatDirector.EliteTierDef
            {
                costMultiplier = CostMultiplier * 0.8f,
                eliteTypes = new EliteDef[]
                {
                        chosenEliteDef
                },
                isAvailable = ((SpawnCard.EliteRules rules) => !CombatDirector.IsEliteOnlyArtifactActive()),
                canSelectWithoutAvailableEliteDef = false,
            };
            array[2] = new CombatDirector.EliteTierDef
            {
                costMultiplier = CostMultiplier,
                eliteTypes = new EliteDef[]
                {
                       chosenEliteDef
                },
                isAvailable = ((SpawnCard.EliteRules rules) => CombatDirector.IsEliteOnlyArtifactActive()),
                canSelectWithoutAvailableEliteDef = false,
            };
            CombatDirector.eliteTiers = array;

            if (SceneInfo.instance && ClassicStageInfo.instance)
            {
                SceneInfo.instance.StartCoroutine(DelayedBrigadeMessage(Brigade.chosenEliteDef.modifierToken, 1.2f));
            }
            else
            {
                Chat.SendBroadcastChat(new Brigade.BrigadeMessage
                {
                    eliteNameToken = Brigade.chosenEliteDef.modifierToken
                });
            }

        }

        public static IEnumerator DelayedBrigadeMessage(string elite, float delay)
        {
            yield return new WaitForSeconds(delay);
            Chat.SendBroadcastChat(new Brigade.BrigadeMessage
            {
                eliteNameToken = elite
            });
            yield break;
        }
    }
}