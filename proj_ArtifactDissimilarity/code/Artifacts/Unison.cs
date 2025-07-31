using HG;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


namespace ArtifactDissimilarity
{
    public class Unison
    {
        public static Xoroshiro128Plus unisonRNG = new Xoroshiro128Plus(1U);

        public static PickupIndex[] pickupsArrayOverride = new PickupIndex[0];
        public static PickupIndex[] pickupsArrayNoRepeats = new PickupIndex[0];
        public static PickupIndex[] pickupsArrayNoRepeats2 = new PickupIndex[0];

        public static void OnArtifactDisable()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            On.RoR2.PickupDropTable.GenerateDrop -= OverrideDrop_GenerateDrop;
            On.RoR2.PickupDropTable.GenerateUniqueDrops -= OverrideDropsArray_GenerateUniqueDrops;
            On.RoR2.ChestBehavior.ItemDrop -= ChestBehavior_ItemDrop; //??
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact -= Unison_Command;
            Run.onRunStartGlobal -= Run_onRunStartGlobal;
            Stage.onServerStageComplete -= Stage_onServerStageComplete;
            SceneDirector.onGenerateInteractableCardSelection -= MorePrintersAndCredits;
            On.EntityStates.InfiniteTowerSafeWard.Travelling.OnExit -= NewItemOnCrabTravel;
        }

        public static void OnArtifactEnable()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            //Overrides
            On.RoR2.PickupDropTable.GenerateDrop += OverrideDrop_GenerateDrop;
            On.RoR2.PickupDropTable.GenerateUniqueDrops += OverrideDropsArray_GenerateUniqueDrops;
            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop; //??
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact += Unison_Command;
            //Generate new
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Stage.onServerStageComplete += Stage_onServerStageComplete;
            //Extras
            SceneDirector.onGenerateInteractableCardSelection += MorePrintersAndCredits;
            On.EntityStates.InfiniteTowerSafeWard.Travelling.OnExit += NewItemOnCrabTravel;
            //Activated mid Run
            if (Stage.instance && Run.instance)
            {
                Run_onRunStartGlobal(Run.instance);
            }
        }

        private static void Unison_Command(On.RoR2.PickupPickerController.orig_SetOptionsFromPickupForCommandArtifact orig, PickupPickerController self, PickupIndex pickupIndex)
        {
            bool dontOverride = false;
            PickupDef pickupDef = pickupIndex.pickupDef;
            if (pickupDef.itemIndex == JunkContent.Items.AACannon.itemIndex || pickupDef.itemIndex == JunkContent.Items.SkullCounter.itemIndex)
            {
                PickupPickerController.Option[] commandOptions = new PickupPickerController.Option[11];
                commandOptions[0].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier1];
                commandOptions[1].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                commandOptions[2].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier3];
                commandOptions[3].pickupIndex = pickupsArrayOverride[(int)ItemTier.Boss];
                commandOptions[4].pickupIndex = pickupsArrayOverride[(int)ItemTier.Lunar];
                commandOptions[5].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier1];
                commandOptions[6].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier2];
                commandOptions[7].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier3];
                commandOptions[8].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidBoss];
                commandOptions[9].pickupIndex = pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 0];
                commandOptions[10].pickupIndex = pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1];

                for (int i = 0; i < commandOptions.Length; i++)
                {
                    commandOptions[i].available = true;
                }
                self.SetOptionsServer(commandOptions);
                return;
            }
            else
            {            
                PickupPickerController.Option[] commandOptions = new PickupPickerController.Option[4];
                if (pickupDef.itemIndex != ItemIndex.None)
                {
                    int oCount = 0;
                    ItemTier itemTier = ItemCatalog.GetItemDef(pickupDef.itemIndex).tier;
                    switch (itemTier)
                    {
                        //80 20 1
                        case ItemTier.Tier1:
                            if (unisonRNG.RangeInt(0, 100) < 1)
                            {    
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier3];
                                oCount++;
                            }
                            if (unisonRNG.RangeInt(0, 100) < 20)
                            {                      
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                                oCount++;
                            }
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier1];
                            break;
                        case ItemTier.Tier2:
                            //80 20 / 85 15
                            if (unisonRNG.RangeInt(0, 100) < 15)
                            {
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Boss];
                                oCount++;
                            }
                            if (unisonRNG.RangeInt(0, 100) < 20)
                            {
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier3];
                                oCount++;
                            }
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                            oCount++;
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier1];
                            break;
                        case ItemTier.Tier3:
                            if (unisonRNG.RangeInt(0, 100) < 20)
                            {
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Boss];
                                oCount++;
                            }
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier3];
                            oCount++;
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                            oCount++;
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier1];
                            break;
                        case ItemTier.Boss:
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Boss];
                            oCount++;
                            if (unisonRNG.RangeInt(0, 100) < 20)
                            {
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier3];
                                oCount++;
                            }
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                            oCount++;
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier1];
                            break;
                        case ItemTier.Lunar:
                            commandOptions[0].pickupIndex = pickupsArrayOverride[(int)ItemTier.Lunar];
                            commandOptions[1].pickupIndex = pickupsArrayOverride[(int)ItemTierCatalog.itemTierDefs.Length + 1];
                            break;
                        case ItemTier.VoidTier1:
                            //6 3 1
                            if (unisonRNG.RangeInt(0, 10) < 1)
                            {
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier3];
                                oCount++;
                            }        
                            if (unisonRNG.RangeInt(0, 10) < 3)
                            {
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier2];
                                oCount++;
                            }
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier1];
                            break;
                        case ItemTier.VoidTier2:
                            //5 5 2
                            if (unisonRNG.RangeInt(0, 12) < 2)
                            {
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier3];
                                oCount++;
                            }
                            commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier2];
                            if (unisonRNG.RangeInt(0, 12) < 5)
                            {
                                oCount++;
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier1];
                            }                 
                            break;
                        case ItemTier.VoidTier3:
                            commandOptions[0].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier3];
                            commandOptions[1].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier2];
                            commandOptions[2].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier1];
                            break;
                        case ItemTier.VoidBoss:
                            commandOptions[0].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidBoss];
                            commandOptions[1].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier3];
                            commandOptions[2].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier2];
                            commandOptions[3].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier1];
                            break;
                        default:
                            dontOverride = true;
                            break;
                    }
                }
                if (pickupDef.equipmentIndex != EquipmentIndex.None)
                {
                    EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(pickupDef.equipmentIndex);
                    if (equipmentDef.isBoss)
                    {
                        commandOptions[0].pickupIndex = pickupIndex;
                        commandOptions[1].pickupIndex = pickupsArrayOverride[(int)ItemTier.Boss];
                    }
                    else if (equipmentDef.isLunar)
                    {
                        commandOptions[0].pickupIndex = pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1];
                        commandOptions[1].pickupIndex = pickupsArrayOverride[(int)ItemTier.Lunar];
                    }
                    else
                    {
                        commandOptions[0].pickupIndex = pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 0];
                        commandOptions[1].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier1];
                    }
                }
                if (!dontOverride)
                {
                    for (int i = 0; i < commandOptions.Length; i++)
                    {
                        commandOptions[i].available = true;
                        if (commandOptions[i].pickupIndex.value == 0)
                        {
                            commandOptions = commandOptions.Remove(commandOptions[i]);
                            i--;
                        }
                    }
                    self.SetOptionsServer(commandOptions);
                    return;
                }
            }
            orig(self, pickupIndex);
        }

        private static void Stage_onServerStageComplete(Stage obj)
        {
            SetupItemOverrides();
        }

        public static bool IsDropTableEligibleForReplacing(PickupDropTable dt)
        {
            if (dt is DoppelgangerDropTable || dt is ArenaMonsterItemDropTable)
            {
                return false;
            }
            else if (dt is BasicPickupDropTable) //Needs to be the lowest
            {
                if ((dt as BasicPickupDropTable).bannedItemTags.Contains(ItemTag.AIBlacklist))
                {
                    return false;
                }
                else if (dt.name.StartsWith("dtDupli"))
                {
                    return false;
                }
                else if (Stage.instance && Stage.instance.spawnedAnyPlayer && SceneInfo.instance.sceneDef.baseSceneName == "bazaar")
                {
                    return false;
                }
            }
            return true;
        }

        public static PickupIndex OverrideDrop_GenerateDrop(On.RoR2.PickupDropTable.orig_GenerateDrop orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            if (IsDropTableEligibleForReplacing(self))
            {
                return GetNewPickupIndex(orig(self, rng));
            }
            return orig(self, rng);
        }

        private static PickupIndex[] OverrideDropsArray_GenerateUniqueDrops(On.RoR2.PickupDropTable.orig_GenerateUniqueDrops orig, PickupDropTable self, int maxDrops, Xoroshiro128Plus rng)
        {
            if (IsDropTableEligibleForReplacing(self))
            {
                PickupIndex[] temp = orig(self, maxDrops, rng);
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = GetNewPickupIndex(temp[i]);
                }
                return temp;
            }
            return orig(self, maxDrops, rng);
        }



        public static void RemoveCategoryChests(DirectorCardCategorySelection dccs)
        {
            if (dccs && dccs.categories.Length > 0)
            {
                ref DirectorCardCategorySelection.Category ptr = ref dccs.categories[0];
                for (int j = ptr.cards.Length - 1; j >= 0; j--)
                {
                    DirectorCard obj = ptr.cards[j];
                    if (obj.spawnCard.name.StartsWith("iscCate"))
                    {
                        ArrayUtils.ArrayRemoveAtAndResize<DirectorCard>(ref ptr.cards, j, 1);
                    }
                }
            }

        }


        private static void MorePrintersAndCredits(SceneDirector self, DirectorCardCategorySelection dccs)
        {
            if (self.interactableCredit > 0)
            {
                int bonusCredit = 20 + 10 * Run.instance.participatingPlayerCount;
                self.interactableCredit += bonusCredit;
            }
            int Index = dccs.FindCategoryIndexByName("Duplicator");
            if (Index != -1)
            {
                dccs.categories[Index].selectionWeight *= 1.5f;
            }
            RemoveCategoryChests(dccs);
        }

        private static void Run_onRunStartGlobal(Run obj)
        {
            pickupsArrayOverride = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 4];
            pickupsArrayNoRepeats = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 4];
            pickupsArrayNoRepeats2 = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 4];
            SetupItemOverrides();
        }




        public static void NewItemOnCrabTravel(On.EntityStates.InfiniteTowerSafeWard.Travelling.orig_OnExit orig, EntityStates.InfiniteTowerSafeWard.Travelling self)
        {
            //Helps with IT or mid stage changes
            orig(self);
            SetupNew1TierOnly(0, Run.instance.availableTier1DropList, LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier1"));
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = "ITREROLL_UNISON_WHITE",
            });

            //If the player for some reason gets like 5 greens in 5 waves they should probably get a different green
            int greenCount = Util.GetItemCountForTeam(TeamIndex.Player, pickupsArrayOverride[1].pickupDef.itemIndex, false, true);
            if (greenCount > (Run.instance.participatingPlayerCount * 4))
            {
                SetupNew1TierOnly(1, Run.instance.availableTier2DropList, LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier2"));
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "ITREROLL_UNISON_GREEN",
                });
            }
        }

        public static void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            //Helps with IT or mid stage changes
            self.dropPickup = GetNewPickupIndex(self.dropPickup);
            orig(self);
        }



        public static void SetupItemOverrides()
        {
            Debug.Log(Run.instance.treasureRng.nextUlong);
            unisonRNG = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
            Debug.Log("Artifact of Unison : New item set");
            if (pickupsArrayOverride.Length < 3)
            {
                pickupsArrayOverride = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 4];
                pickupsArrayNoRepeats = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 4];
                pickupsArrayNoRepeats2 = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 4];
                Debug.LogWarning("does this code even still run");
            }


            //Run.instance.availableBossDropList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex));

            pickupsArrayOverride[0] = NoRepeatsItems(0, Run.instance.availableTier1DropList);
            pickupsArrayOverride[1] = NoRepeatsItems(1, Run.instance.availableTier2DropList);
            pickupsArrayOverride[2] = NoRepeatsItems(2, Run.instance.availableTier3DropList);
            pickupsArrayOverride[3] = NoRepeatsItems(3, Run.instance.availableLunarItemDropList);
            pickupsArrayOverride[4] = NoRepeatsItems(4, Run.instance.availableBossDropList);
            //pickupsArrayOverride[5] = NoTier
            pickupsArrayOverride[6] = NoRepeatsItems(6, Run.instance.availableVoidTier1DropList);
            pickupsArrayOverride[7] = NoRepeatsItems(7, Run.instance.availableVoidTier2DropList);
            if (Run.instance.availableVoidTier3DropList.Count > 0)
            {
                pickupsArrayOverride[8] = Run.instance.availableVoidTier3DropList[unisonRNG.RangeInt(0, Run.instance.availableVoidTier3DropList.Count)];
            }
            if (Run.instance.availableVoidBossDropList.Count > 0)
            {
                pickupsArrayOverride[9] = Run.instance.availableVoidBossDropList[unisonRNG.RangeInt(0, Run.instance.availableVoidBossDropList.Count)];
            }

            //Equipment
            pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length] = NoRepeatsEquipment(ItemTierCatalog.itemTierDefs.Length, Run.instance.availableEquipmentDropList);
            pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1] = Run.instance.availableLunarEquipmentDropList[unisonRNG.RangeInt(0, Run.instance.availableLunarEquipmentDropList.Count)];


            if (WConfig.DebugPrint.Value)
            {
                Debug.Log("___");
                Debug.Log("Print Artifact of Unison results");
                string results = "\n";
                for (int i = 0; i < pickupsArrayOverride.Length; i++)
                {
                    results += pickupsArrayOverride[i].ToString() + "\n";
                }
                Debug.Log(results);
            }
        }

        public static void SetupNew1TierOnly(int Tier, List<PickupIndex> runList, BasicPickupDropTable dropTable)
        {
            Debug.Log("Artifact of Unison : New " + (ItemTier)Tier + " item");
            pickupsArrayOverride[Tier] = NoRepeatsItems(Tier, runList);
            if (WConfig.DebugPrint.Value)
            {
                Debug.Log(pickupsArrayOverride[Tier].ToString());
            }
        }


        public static PickupIndex GetNewPickupIndex(PickupIndex original)
        {
            if (original == PickupIndex.none)
            {
                return original;
            }
            if (original.pickupDef == null)
            {
                return original;
            }
            PickupDef pickupDef = original.pickupDef;
            if (pickupDef.itemIndex != ItemIndex.None && pickupDef.itemIndex != RoR2Content.Items.ArtifactKey.itemIndex)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
                if (!itemDef.tags.Contains(ItemTag.Scrap))
                {
                    return pickupsArrayOverride[(int)itemDef.tier];
                }
            }
            else if (pickupDef.equipmentIndex != EquipmentIndex.None)
            {
                if (pickupDef.isLunar)
                {
                    return pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1];
                }
                else if (!pickupDef.isBoss)
                {
                    return pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 0];
                }
            }

            return original;
        }

        public static PickupIndex NoRepeatsItems(int Tier, List<PickupIndex> pickupList)
        {
            if (pickupList.Count == 0)
            {
                return PickupIndex.none;
            }
            else if (pickupList.Count == 1)
            {
                return pickupList[0];
            }
            if (Tier == 4)
            {
                pickupList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex));
            }

            ItemTag bannedTag = ItemTag.Any;
            ItemDef tempDef = ItemCatalog.GetItemDef(pickupsArrayOverride[Tier].pickupDef.itemIndex);
            if (tempDef && tempDef.tags.Length > 0)
            {
                bannedTag = tempDef.tags[0];
            }
            //First stage, forces White and Green to be of different categories.
            if (bannedTag == ItemTag.Any && Tier == 1)
            {
                tempDef = ItemCatalog.GetItemDef(pickupsArrayOverride[Tier - 1].pickupDef.itemIndex);
                if (tempDef && tempDef.tags.Length > 0)
                {
                    bannedTag = tempDef.tags[0];
                }
            }
            if (WConfig.DebugPrint.Value)
            {
                string debug = "\n" + (ItemTier)Tier + "  BannedTag: " + bannedTag.ToString() + "\n";
                debug += (ItemTier)Tier + "  BannedItem2: " + pickupsArrayNoRepeats[Tier].ToString() + "\n";
                debug += (ItemTier)Tier + "  BannedItem3: " + pickupsArrayNoRepeats2[Tier].ToString() + "\n";
                Debug.Log(debug);
            }
            PickupIndex tempDex;
            do
            {
                tempDex = pickupList[unisonRNG.RangeInt(0, pickupList.Count)];
                tempDef = ItemCatalog.GetItemDef(tempDex.pickupDef.itemIndex);
            }
            while (tempDex == pickupsArrayOverride[Tier] || tempDex == pickupsArrayNoRepeats[Tier] || tempDex == pickupsArrayNoRepeats2[Tier] || tempDef && tempDef.tags.Contains(bannedTag));
            //2 Layers of repeat protection + Tag Repeat
            pickupsArrayNoRepeats2[Tier] = pickupsArrayNoRepeats[Tier];
            pickupsArrayNoRepeats[Tier] = pickupsArrayOverride[Tier];
            return tempDex;
        }

        public static PickupIndex NoRepeatsEquipment(int Tier, List<PickupIndex> pickupList)
        {
            if (pickupList.Count == 0)
            {
                return PickupIndex.none;
            }
            else if (pickupList.Count == 1)
            {
                return pickupList[0];
            }

            if (WConfig.DebugPrint.Value)
            {
                string debug = "BannedEquip1: " + pickupsArrayOverride[Tier].ToString() +
                "\nBannedEquip2: " + pickupsArrayNoRepeats[Tier].ToString() +
                "\nBannedEquip3: " + pickupsArrayNoRepeats2[Tier].ToString();
                Debug.Log(debug);
            }

            PickupIndex tempDex;
            do
            {
                //tempDex = pickupList[random.Next(pickupList.Count)];
                tempDex = pickupList[Run.instance.treasureRng.RangeInt(0, pickupList.Count)];
            }
            while (tempDex == pickupsArrayOverride[Tier] || tempDex == pickupsArrayNoRepeats[Tier] || tempDex == pickupsArrayNoRepeats2[Tier]);
            //2 Repeats
            pickupsArrayNoRepeats2[Tier] = pickupsArrayNoRepeats[Tier];
            pickupsArrayNoRepeats[Tier] = pickupsArrayOverride[Tier];
            return tempDex;
        }

    }
}