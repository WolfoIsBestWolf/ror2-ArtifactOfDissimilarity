using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactDissimilarity
{
    public class Brigade
    {
        private static List<EliteDef> EliteDefsTier1 = new List<EliteDef>();
        private static List<EliteDef> EliteDefsTier2 = new List<EliteDef>();
        public static List<EliteDef> ForUsageEliteDefList = new List<EliteDef>();
        private static EliteDef TempForUsageEliteDef;
        //private static EliteDef NoRepeatForUsageEliteDef;
        // public static EquipmentIndex[] brigadedAffixes = Array.Empty<EquipmentIndex>();

        public static bool DidBrigadeHappen;
        public static CombatDirector.EliteTierDef[] normalelitetierdefs;

        static bool DumbEliteKinThing = false;
        //static bool DumbT2EliteKinThing = false;
        static int DumbT2EliteKinInt = 1;


 
        public static void SetupBrigade()
        {
            if (DumbEliteKinThing == false)
            {
                for (int i = 0; i < EliteCatalog.eliteList.Count; i++)
                {
                    EliteDef tempdef = EliteCatalog.GetEliteDef(EliteCatalog.eliteList[i]);
                    string tempname = tempdef.name;
                    //Debug.LogWarning(tempdef);
                    if (tempname.EndsWith("Gold") || tempname.EndsWith("Honor") || tempname.EndsWith("SecretSpeed") || tempname.EndsWith("Echo") || tempname.Contains("Lunar"))
                    {
                    }
                    else if (tempdef.healthBoostCoefficient > 10 && !EliteDefsTier2.Contains(tempdef) || tempname.EndsWith("Void"))
                    {
                        EliteDefsTier2.Add(tempdef);
                    }
                    else if (!EliteDefsTier1.Contains(tempdef))
                    {
                        EliteDefsTier1.Add(tempdef);
                    }
                }

                if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.arimah.PerfectedLoop"))
                {
                    EliteDefsTier2.Add(RoR2Content.Elites.Lunar);
                }

                DumbEliteKinThing = true;
            }
            DumbT2EliteKinInt = 1;
            ForUsageEliteDefList.Clear();
            ForUsageEliteDefList.AddRange(EliteDefsTier1);
        }

        public static string SendBrigadeMessage()
        {
            string token = "<style=cWorldEvent>All elite combatants will be ";
            string token2 = Language.GetString(TempForUsageEliteDef.modifierToken);
            token2 = token2.Replace("{0}", "");
            token += token2 + "</style>";
            return token;
        }

        public static void EliteKinAsMethod()
        {
            if (NetworkServer.active)
            {
                if (DumbT2EliteKinInt <= Run.instance.NetworkstageClearCount / 5)
                {
                    DumbT2EliteKinInt++;
                    Debug.Log("Add Tier 2 to Brigade");
                    ForUsageEliteDefList.AddRange(EliteDefsTier2);
                    ForUsageEliteDefList.AddRange(EliteDefsTier2);
                }

                /*if (TempForUsageEliteDef && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.EliteOnly))
                {
                    TempForUsageEliteDef.healthBoostCoefficient /= 2;
                    TempForUsageEliteDef.damageBoostCoefficient /= 2;
                }*/

                if (SceneInfo.instance.sceneDef.baseSceneName == "moon2" || SceneInfo.instance.sceneDef.baseSceneName == "limbo")
                {
                    TempForUsageEliteDef = RoR2Content.Elites.Lunar;
                }
                else if (SceneInfo.instance.sceneDef.baseSceneName == "voidstage" || SceneInfo.instance.sceneDef.baseSceneName == "voidraid")
                {
                    TempForUsageEliteDef = DLC1Content.Elites.Void;
                }
                else
                {
                    TempForUsageEliteDef = ForUsageEliteDefList[Main.random.Next(ForUsageEliteDefList.Count)];
                }

                //Eulogy Lunar
                int itemCountGlobal = Util.GetItemCountGlobal(DLC1Content.Items.RandomlyLunar.itemIndex, false, false);
                if (itemCountGlobal > 0)
                {
                    itemCountGlobal++;
                    //itemCountGlobal = Math.Min(itemCountGlobal, 10);
                    if (Run.instance.spawnRng.nextNormalizedFloat < 0.05f * (float)itemCountGlobal)
                    {
                        TempForUsageEliteDef = RoR2Content.Elites.Lunar;
                    }
                }

                if (Run.instance.name.StartsWith("WeeklyRun"))
                {
                    //WeeklyRun.bossAffixes
                    EquipmentIndex[] brigadedAffixes = Array.Empty<EquipmentIndex>();
                    brigadedAffixes = brigadedAffixes.Add(TempForUsageEliteDef.eliteEquipmentDef.equipmentIndex);
                    Run.instance.SetFieldValue<EquipmentIndex[]>("bossAffixes", brigadedAffixes);
                }

                Debug.Log("Artifact of Brigade: This stages only Elite " + TempForUsageEliteDef.name);

                if (DidBrigadeHappen == false)
                {
                    DidBrigadeHappen = true;
                    normalelitetierdefs = CombatDirector.eliteTiers;
                }

                EliteDef tempelitedef = TempForUsageEliteDef;
                float CostMultiplier = 6;
                if (TempForUsageEliteDef.healthBoostCoefficient > 10)
                {
                    CostMultiplier = 36;
                }
 

                CombatDirector.EliteTierDef[] array = new CombatDirector.EliteTierDef[3];
                array[0] = new CombatDirector.EliteTierDef
                {
                    costMultiplier = 1,
                    eliteTypes = new EliteDef[1],
                    isAvailable = ((SpawnCard.EliteRules rules) => CombatDirector.NotEliteOnlyArtifactActive()),
                    canSelectWithoutAvailableEliteDef = true,
                };
                array[1] = new CombatDirector.EliteTierDef
                {
                    costMultiplier = CostMultiplier,
                    eliteTypes = new EliteDef[]
                    {
                        tempelitedef,
                    },
                    isAvailable = ((SpawnCard.EliteRules rules) => CombatDirector.NotEliteOnlyArtifactActive()),
                    canSelectWithoutAvailableEliteDef = false,
                };
                array[2] = new CombatDirector.EliteTierDef
                {
                    costMultiplier = CostMultiplier*1.25f,
                    eliteTypes = new EliteDef[]
                    {
                        tempelitedef,
                    },
                    isAvailable = ((SpawnCard.EliteRules rules) => CombatDirector.IsEliteOnlyArtifactActive()),
                    canSelectWithoutAvailableEliteDef = false,
                };
                CombatDirector.eliteTiers = array;
            }
        }

    }
}