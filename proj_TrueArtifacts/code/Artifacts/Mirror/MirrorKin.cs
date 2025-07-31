using RoR2;
using RoR2.ExpansionManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;


namespace TrueArtifacts.Aritfacts
{
    public class MirrorKin
    {
        public static DccsPool dpAllFamilies;
        public static FamilyDirectorCardCategorySelection dccsFamilyTrueDisso;

        public static void MakeDCCSPool()
        {
            Debug.Log("ALL FAMILY");

            ExpansionDef DLC1E = Addressables.LoadAssetAsync<ExpansionDef>(key: "RoR2/DLC1/Common/DLC1.asset").WaitForCompletion();
            ExpansionDef DLC2E = Addressables.LoadAssetAsync<ExpansionDef>(key: "RoR2/DLC2/Common/DLC2.asset").WaitForCompletion();
            ExpansionDef[] DLC1 = new ExpansionDef[] { DLC1E };
            ExpansionDef[] DLC2 = new ExpansionDef[] { DLC2E };


            DccsPool.PoolEntry Clay = null;
            DccsPool.PoolEntry Robo = null;
            DccsPool.ConditionalPoolEntry Vermin = null;
            DccsPool.ConditionalPoolEntry Worm = null;



            FamilyDirectorCardCategorySelection[] FamilyDCCSs = Object.FindObjectsOfType(typeof(RoR2.FamilyDirectorCardCategorySelection)) as RoR2.FamilyDirectorCardCategorySelection[];
            for (var i = 0; i < FamilyDCCSs.Length; i++)
            {
                Debug.Log(FamilyDCCSs[i].name);
                switch (FamilyDCCSs[i].name)
                {
                    case "dccsClayFamily":
                        Clay = new DccsPool.PoolEntry
                        {
                            dccs = FamilyDCCSs[i],
                            weight = 1,
                        };
                        break;
                    case "dccsRoboBallFamily":
                        Robo = new DccsPool.PoolEntry
                        {
                            dccs = FamilyDCCSs[i],
                            weight = 1,
                        };
                        break;
                    case "dccsVerminFamily":
                        Vermin = new DccsPool.ConditionalPoolEntry
                        {
                            requiredExpansions = DLC1,
                            dccs = FamilyDCCSs[i],
                            weight = 1,
                        };
                        break;
                    case "dccsWormsFamily":
                        Worm = new DccsPool.ConditionalPoolEntry
                        {
                            requiredExpansions = DLC2,
                            dccs = FamilyDCCSs[i],
                            weight = 1,
                        };
                        break;
                }
            }

            dpAllFamilies = ScriptableObject.CreateInstance<DccsPool>();
            dpAllFamilies.name = "dpAllFamilies";
            dpAllFamilies.poolCategories = new DccsPool.Category[]
            {
                new DccsPool.Category
                {
                    name = "Families",
                    categoryWeight = 1,
                    alwaysIncluded = new DccsPool.PoolEntry[]
                    {
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsBeetleFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsGolemFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsJellyfishFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsWispFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsLemurianFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsImpFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsParentFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsMushroomFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                        new DccsPool.PoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsLunarFamily.asset").WaitForCompletion(),
                            weight = 1,
                        },
                    },
                    includedIfConditionsMet = new DccsPool.ConditionalPoolEntry[]
                    {
                        new DccsPool.ConditionalPoolEntry
                        {
                             dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/Base/Common/dccsGupFamily.asset").WaitForCompletion(),
                             weight = 1,
                             requiredExpansions = DLC1,
                        },
                        new DccsPool.ConditionalPoolEntry
                        {
                             dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/DLC1/Common/dccsConstructFamily.asset").WaitForCompletion(),
                             weight = 1,
                             requiredExpansions = DLC1,
                        },
                        new DccsPool.ConditionalPoolEntry
                        {
                             dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/DLC1/Common/dccsAcidLarvaFamily.asset").WaitForCompletion(),
                             weight = 1,
                             requiredExpansions = DLC1,
                        },
                        new DccsPool.ConditionalPoolEntry
                        {
                             dccs = Addressables.LoadAssetAsync<FamilyDirectorCardCategorySelection>(key: "RoR2/DLC1/Common/dccsVoidFamily.asset").WaitForCompletion(),
                             weight = 1,
                             requiredExpansions = DLC1,
                        },
                    },
                    includedIfNoConditionsMet = new DccsPool.PoolEntry[0]
                }
            };

            if (Clay != null)
            {
                var A = dpAllFamilies.poolCategories[0].alwaysIncluded;
                var B = dpAllFamilies.poolCategories[0].includedIfConditionsMet;
                dpAllFamilies.poolCategories[0].alwaysIncluded = A.Add(Clay, Robo);
                dpAllFamilies.poolCategories[0].includedIfConditionsMet = B.Add(Vermin, Worm);
            }

            //Normal Appropriate
            //Disso Any 
            //True Disso 3 random?
            On.RoR2.ClassicStageInfo.HandleMixEnemyArtifact += RandomFamilyOnDissonance;

            dccsFamilyTrueDisso = ScriptableObject.CreateInstance<FamilyDirectorCardCategorySelection>();
            dccsFamilyTrueDisso.name = "dccsFamilyTrueDissonance";
            dccsFamilyTrueDisso.AddCategory("Champion", 3);
            dccsFamilyTrueDisso.AddCategory("Miniboss", 3);
            dccsFamilyTrueDisso.AddCategory("Basic Monsters", 4);
            dccsFamilyTrueDisso.AddCategory("UniqueBosses", 0.3f);


            DirectorCard directorCard = new DirectorCard();
            dccsFamilyTrueDisso.AddCard(0, directorCard);
            dccsFamilyTrueDisso.AddCard(1, directorCard);
            dccsFamilyTrueDisso.AddCard(2, directorCard);
            dccsFamilyTrueDisso.AddCard(3, directorCard);

        }

        private static void RandomFamilyOnDissonance(On.RoR2.ClassicStageInfo.orig_HandleMixEnemyArtifact orig, DirectorCardCategorySelection monsterCategories, Xoroshiro128Plus rng)
        {
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Main.Mirror_Kin))
            {
                DirectorCardCategorySelection dccs = FindFamily(true);
                if (dccs == null)
                {

                }
                if (dccs is FamilyDirectorCardCategorySelection)
                {
                    ClassicStageInfo.instance.StartCoroutine(ClassicStageInfo.instance.BroadcastFamilySelection((dccs as FamilyDirectorCardCategorySelection).selectionChatString));
                }
                monsterCategories.CopyFrom(dccs);
            }
            else
            {
                orig(monsterCategories, rng);
            }
        }


        //Maybe don't rely purely on vanilla family selections since they suck ass
        //50/50 vanilla or random adequete family 
        //If Dissonance, I guess like any family event but how do we do that.
        public static void On_Artifact_Enable()
        {
            On.RoR2.FamilyDirectorCardCategorySelection.IsAvailable += AllowFamilyEventsEarlier;
            On.RoR2.ClassicStageInfo.RebuildCards += Force_Family_Event;
            if (ClassicStageInfo.instance && Run.instance)
            {
                ClassicStageInfo.instance.RebuildCards(null, null);
            }
            On.RoR2.UI.EnemyInfoPanel.SetDisplayDataForViewer += AddMirrorKinDisplay;
            RoR2.UI.EnemyInfoPanel.MarkDirty();
        }

        private static void AddMirrorKinDisplay(On.RoR2.UI.EnemyInfoPanel.orig_SetDisplayDataForViewer orig, RoR2.UI.HUD hud, System.Collections.Generic.List<BodyIndex> bodyIndices, ItemIndex[] itemAcquisitionOrderBuffer, int itemAcquisitonOrderLength, int[] itemStacks)
        {
            if (ClassicStageInfo.instance)
            {
                MirrorKinTracker mirror = ClassicStageInfo.instance.GetComponent<MirrorKinTracker>();
                if (mirror)
                {
                    if (mirror.Enemy1 != BodyIndex.None)
                    {
                        bodyIndices.Add(mirror.Enemy1);
                    }
                    if (mirror.Enemy2 != BodyIndex.None)
                    {
                        bodyIndices.Add(mirror.Enemy2);
                    }
                    if (mirror.Enemy3 != BodyIndex.None)
                    {
                        bodyIndices.Add(mirror.Enemy3);
                    }
                    if (mirror.Enemy4 != BodyIndex.None)
                    {
                        bodyIndices.Add(mirror.Enemy4);
                    }
                }
            }
            orig(hud, bodyIndices, itemAcquisitionOrderBuffer, itemAcquisitonOrderLength, itemStacks);
        }

        private static bool AllowFamilyEventsEarlier(On.RoR2.FamilyDirectorCardCategorySelection.orig_IsAvailable orig, FamilyDirectorCardCategorySelection self)
        {
            if (Run.instance && Run.instance.stageClearCount >= 3)
            {
                return true;
            }
            else
            {
                return Run.instance && self.minimumStageCompletion <= Run.instance.stageClearCount;
            }
        }

        public static void On_Artifact_Disable()
        {
            On.RoR2.FamilyDirectorCardCategorySelection.IsAvailable -= AllowFamilyEventsEarlier;
            On.RoR2.ClassicStageInfo.RebuildCards -= Force_Family_Event;
            if (ClassicStageInfo.instance && Run.instance)
            {
                ClassicStageInfo.instance.RebuildCards(null, null);
            }
            On.RoR2.UI.EnemyInfoPanel.SetDisplayDataForViewer -= AddMirrorKinDisplay;
            RoR2.UI.EnemyInfoPanel.MarkDirty();
        }


        public static List<DirectorCard> GenerateCandiateList(DirectorCardCategorySelection dccs)
        {
            List<DirectorCard> listCards = new List<DirectorCard>();
            for (int i = 0; i < dccs.categories.Length; i++)
            {
                ref DirectorCardCategorySelection.Category ptr = ref dccs.categories[i];
                foreach (DirectorCard directorCard in ptr.cards)
                {
                    listCards.Add(directorCard);
                }
            }
            return listCards;
        }
        public static List<DirectorCardCategorySelection> GenerateCandiateListDCCS(DccsPool dccsPool)
        {
            List<DirectorCardCategorySelection> listDCCS = new List<DirectorCardCategorySelection>();
            foreach (var A in dccsPool.poolCategories[0].alwaysIncluded)
            {
                listDCCS.Add(A.dccs);
            }
            foreach (var A in dccsPool.poolCategories[0].includedIfConditionsMet)
            {
                if (A.requiredExpansions.Length > 0)
                {
                    if (Run.instance.IsExpansionEnabled(A.requiredExpansions[0]))
                    {
                        listDCCS.Add(A.dccs);
                    }
                }              
            }
            return listDCCS;
        }

        public static DirectorCardCategorySelection FindFamily(bool Dissonance)
        {
            Debug.Log("FindFamily");
            ClassicStageInfo csi = ClassicStageInfo.instance;
            List<DirectorCard> candidates = null;
            Xoroshiro128Plus RNG = new Xoroshiro128Plus(csi.seedServer);

            if (Dissonance)
            {
                candidates = GenerateCandiateList(RoR2Content.mixEnemyMonsterCards);
            }
            else if (csi.monsterDccsPool)
            {
                DirectorCardCategorySelection directorCardCategorySelection2 = csi.monsterDccsPool.GenerateWeightedSelection().Evaluate(RNG.nextNormalizedFloat);
                if (directorCardCategorySelection2 != null)
                {
                    candidates = GenerateCandiateList(directorCardCategorySelection2);
                }
            }
            else
            {
                return null;
            }
            int V0 = Random.RandomRangeInt(0, 3);
            if (V0 == 2)
            {
                if (SceneCatalog.mostRecentSceneDef.cachedName == "voidstage")
                {
                    return null;
                }
            }


            DirectorCard candiateCard = candidates[RNG.RangeInt(0, candidates.Count)];
            Debug.Log("Family for " + candiateCard.spawnCard);


            List<DirectorCardCategorySelection> allFamilies = GenerateCandiateListDCCS(dpAllFamilies);

            for (int I1 = 0; I1 < allFamilies.Count; I1++)
            {
                var DCCS = allFamilies[I1];
                //Debug.Log(DCCS);
                for (int I2 = 0; I2 < allFamilies[I1].categories.Length; I2++)
                {
                    var CAT = DCCS.categories[I2];            
                    foreach (DirectorCard card in CAT.cards)
                    {
                        //Debug.Log(card.spawnCard);
                        if (card.spawnCard == candiateCard.spawnCard)
                        {
                            //Debug.Log(DCCS + " matching family for " + candiateCard.spawnCard);

                            if (Dissonance || DCCS.IsAvailable())
                            {
                                return DCCS;
                            }
                            else
                            {
                                Debug.Log(DCCS + " is not available yet.");
                                return null;
                            }
                        }
                    }
                }
            }
            Debug.Log("Could not find matching family for " + candiateCard.spawnCard);
            if (Dissonance)
            {
                int I0 = RNG.RangeInt(0, allFamilies.Count);
                return allFamilies[I0];
            }
            return null;
        }

        public static DirectorCardCategorySelection MakeTrueDissoFamily()
        {
            Debug.Log("Making TrueDisso Family");
            if (TrueDissonance.dccsMixEnemyTRUE == null)
            {
                TrueDissonance.MakeDCCS();
            }

 
            Xoroshiro128Plus RNG = new Xoroshiro128Plus(ClassicStageInfo.instance.seedServer);        
            int I0 = RNG.RangeInt(0, TrueDissonance.dccsMixEnemyTRUE.categories[0].cards.Length);
            int I1 = RNG.RangeInt(0, TrueDissonance.dccsMixEnemyTRUE.categories[1].cards.Length);
            int I2 = RNG.RangeInt(0, TrueDissonance.dccsMixEnemyTRUE.categories[2].cards.Length);
            dccsFamilyTrueDisso.categories[0].cards[0] = TrueDissonance.dccsMixEnemyTRUE.categories[0].cards[I0];
            dccsFamilyTrueDisso.categories[1].cards[0] = TrueDissonance.dccsMixEnemyTRUE.categories[1].cards[I1];
            dccsFamilyTrueDisso.categories[2].cards[0] = TrueDissonance.dccsMixEnemyTRUE.categories[2].cards[I2];
            int newCategory = TrueDissonance.dccsMixEnemyTRUE.FindCategoryIndexByName("UniqueBosses");
            if (newCategory > -1)
            {
                int I3 = RNG.RangeInt(0, TrueDissonance.dccsMixEnemyTRUE.categories[newCategory].cards.Length);
                dccsFamilyTrueDisso.categories[3].cards[0] = TrueDissonance.dccsMixEnemyTRUE.categories[newCategory].cards[I3];
            }

            int D0 = RNG.RangeInt(1, 20);
            int D1 = RNG.RangeInt(1, 21);
            dccsFamilyTrueDisso.selectionChatString = string.Format(Language.GetString("FAMILY_DISSONANT"), Language.GetString("FAMILY_DISSONANT_" + D0.ToString()), Language.GetString("FAMILY_DISSONANT_0" + D1));
            return dccsFamilyTrueDisso;
        }


        private static void Force_Family_Event(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, RoR2.ClassicStageInfo self, RoR2.DirectorCardCategorySelection forcedMonsterCategory, RoR2.DirectorCardCategorySelection forcedInteractableCategory)
        {
            float storage = -1;
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Main.True_Dissonance))
            {
                //Select family that could normally occur
                forcedMonsterCategory = MakeTrueDissoFamily();
            }
            else
            {
                //Disso is taken care of in a different place
                bool Dissonance = RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.MixEnemy);         
                if (!Dissonance)
                {
                    if (forcedMonsterCategory == null)
                    {
                        try
                        {
                            forcedMonsterCategory = FindFamily(false);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                            forcedMonsterCategory = null;
                        }
                    }
                    if (forcedMonsterCategory == null && self.monsterDccsPool && self.monsterDccsPool.poolCategories.Length > 1)
                    {
                        storage = self.monsterDccsPool.poolCategories[0].categoryWeight;
                        self.monsterDccsPool.poolCategories[0].categoryWeight = 0;
                    }
                    else
                    {
                        storage = ClassicStageInfo.monsterFamilyChance;
                        ClassicStageInfo.monsterFamilyChance = 10;
                    }
                }
            }
            orig(self, forcedMonsterCategory, forcedInteractableCategory);
            if (storage > -1)
            {
                if (self.monsterDccsPool && self.monsterDccsPool.poolCategories.Length > 0)
                {
                    self.monsterDccsPool.poolCategories[0].categoryWeight = storage;
                }
                else
                {
                    ClassicStageInfo.monsterFamilyChance = storage;
                }
            }
            
 
            if (WConfig.MirrorKinDisplay.Value)
            {            
                if (RunArtifactManager.instance && !RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.singleMonsterTypeArtifactDef))
                {
                    var chatMessage = new SendMirrorKinTracker();
                    for (int i = 0; i < 4; i++)
                    {
                        BodyIndex index = BodyIndex.None;
                        if (i < self.monsterSelection.Count)
                        {
                            try
                            {
                                index = self.monsterSelection.choices[i].value.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().bodyIndex;
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                        switch (i)
                        {
                            case 0:
                                chatMessage.Enemy1 = (int)index;
                                break;
                            case 1:
                                chatMessage.Enemy2 = (int)index;
                                break;
                            case 2:
                                chatMessage.Enemy3 = (int)index;
                                break;
                            case 3:
                                chatMessage.Enemy4 = (int)index;
                                break;
                        }
                    }
                    Chat.SendBroadcastChat(chatMessage);

                }
            }
        }
    }

    public class MirrorKinTracker : MonoBehaviour
    {
        public BodyIndex Enemy1 = BodyIndex.None;
        public BodyIndex Enemy2 = BodyIndex.None;
        public BodyIndex Enemy3 = BodyIndex.None;
        public BodyIndex Enemy4 = BodyIndex.None;

    }


    public class SendMirrorKinTracker : RoR2.ChatMessageBase
    {
        public int Enemy1 = -1;
        public int Enemy2 = -1;
        public int Enemy3 = -1;
        public int Enemy4 = -1;
        public override string ConstructChatString()
        {
            if (ClassicStageInfo.instance)
            {
                MirrorKinTracker tracker = ClassicStageInfo.instance.gameObject.GetComponent<MirrorKinTracker>();
                if (tracker == null)
                {
                    tracker = ClassicStageInfo.instance.gameObject.AddComponent<MirrorKinTracker>();
                }
                tracker.Enemy1 = (BodyIndex)Enemy1;
                tracker.Enemy2 = (BodyIndex)Enemy2;
                tracker.Enemy3 = (BodyIndex)Enemy3;
                tracker.Enemy4 = (BodyIndex)Enemy4;
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Main.True_Dissonance))
                {
                    if (Run.instance && Run.instance.stageClearCount > 9)
                    {
                        tracker.Enemy2 = (BodyIndex)Enemy1;
                        tracker.Enemy3 = (BodyIndex)Enemy2;
                        tracker.Enemy4 = (BodyIndex)Enemy3;
                        tracker.Enemy1 = (BodyIndex)Enemy4;
                    }
                    else
                    {
                        tracker.Enemy4 = BodyIndex.None;
                    }            
                }
              
            }
            RoR2.UI.EnemyInfoPanel.MarkDirty();
            return null;
        }
 
        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Enemy1);
            writer.Write(Enemy2);
            writer.Write(Enemy3);
            writer.Write(Enemy4);

        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            Enemy1 = reader.ReadInt32();
            Enemy2 = reader.ReadInt32();
            Enemy3 = reader.ReadInt32();
            Enemy4 = reader.ReadInt32();

        }

    }

}



