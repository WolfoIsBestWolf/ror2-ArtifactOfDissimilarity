using HG;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;


namespace ArtifactDissimilarity.Aritfacts
{
    public class Unison
    {
        public static Xoroshiro128Plus unisonRNG = new Xoroshiro128Plus(1U);

        public static bool[] isTempBlacklistedPickup;
        public static PickupIndex[] pickupsArrayOverride = new PickupIndex[0];
        public static PickupIndex[] pickupsArrayNoRepeats = new PickupIndex[0];
        public static PickupIndex[] pickupsArrayNoRepeats2 = new PickupIndex[0];

        public static void OnArtifactDisable()
        {
           
            On.RoR2.PickupDropTable.GenerateDrop -= LEGACY_OverrideDrop_GenerateDrop;
            On.RoR2.PickupDropTable.GeneratePickup -= PickupDropTable_GeneratePickup;
            On.RoR2.PickupDropTable.GenerateUniqueDrops -= LEGACY_OverrideDropsArray_GenerateUniqueDrops;
            On.RoR2.PickupDropTable.GenerateDistinctPickups -= PickupDropTable_GenerateDistinctPickups;
            On.RoR2.ChestBehavior.ItemDrop -= ChestBehavior_ItemDrop; //??
 
            Run.onRunStartGlobal -= Run_onRunStartGlobal;
            Stage.onServerStageComplete -= Stage_onServerStageComplete;
            SceneDirector.onGenerateInteractableCardSelection -= MorePrintersAndCredits;
            On.EntityStates.InfiniteTowerSafeWard.Travelling.OnExit -= NewItemOnCrabTravel;
            Debug.Log("Removed Unison");
        }

        public static void OnArtifactEnable()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            //Overrides
            On.RoR2.PickupDropTable.GenerateDrop += LEGACY_OverrideDrop_GenerateDrop;
            On.RoR2.PickupDropTable.GeneratePickup += PickupDropTable_GeneratePickup;
            On.RoR2.PickupDropTable.GenerateUniqueDrops += LEGACY_OverrideDropsArray_GenerateUniqueDrops;
            On.RoR2.PickupDropTable.GenerateDistinctPickups += PickupDropTable_GenerateDistinctPickups;
            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop; //??
 
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
            Debug.Log("Added Unison");
        }

        private static void PickupDropTable_GenerateDistinctPickups(On.RoR2.PickupDropTable.orig_GenerateDistinctPickups orig, PickupDropTable self, List<UniquePickup> dest, int desiredCount, Xoroshiro128Plus rng, bool allowLoop)
        {
            orig(self, dest, desiredCount, rng, allowLoop);
            if (IsDropTableEligibleForReplacing(self, out bool temp))
            {
                for (int i = 0; i < dest.Count; i++)
                {
                    dest[i] = new UniquePickup(GetNewPickupIndex(dest[i].pickupIndex, temp));
                }
            }
        }

        private static UniquePickup PickupDropTable_GeneratePickup(On.RoR2.PickupDropTable.orig_GeneratePickup orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            if (IsDropTableEligibleForReplacing(self, out bool temp))
            {
                return new UniquePickup(GetNewPickupIndex(orig(self, rng).pickupIndex, temp));
            }
            return orig(self, rng);
        }

        /*
        private static void Unison_Command(On.RoR2.PickupPickerController.orig_SetOptionsFromPickupForCommandArtifact_UniquePickup orig, PickupPickerController self, UniquePickup pickupIndex)
        {
            bool dontOverride = false;
            PickupDef pickupDef = pickupIndex.pickupIndex.pickupDef;
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
        */
        private static void Stage_onServerStageComplete(Stage obj)
        {
            SetupItemOverrides();
        }

        public static bool IsDropTableEligibleForReplacing(PickupDropTable dt, out bool isTemp)
        {
            isTemp = false;
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
                if ((dt as BasicPickupDropTable).requiredItemTags.Contains(ItemTag.CanBeTemporary))
                {
                    isTemp = true;
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

        public static PickupIndex LEGACY_OverrideDrop_GenerateDrop(On.RoR2.PickupDropTable.orig_GenerateDrop orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            if (IsDropTableEligibleForReplacing(self, out _))
            {
                return GetNewPickupIndex(orig(self, rng), false);
            }
            return orig(self, rng);
        }

        private static PickupIndex[] LEGACY_OverrideDropsArray_GenerateUniqueDrops(On.RoR2.PickupDropTable.orig_GenerateUniqueDrops orig, PickupDropTable self, int maxDrops, Xoroshiro128Plus rng)
        {
            if (IsDropTableEligibleForReplacing(self, out _))
            {
                PickupIndex[] temp = orig(self, maxDrops, rng);
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = GetNewPickupIndex(temp[i], false);
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
                    if (obj.GetSpawnCard().name.StartsWith("iscCate"))
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
            isTempBlacklistedPickup = new bool[ItemTierCatalog.itemTierDefs.Length + 4];
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
            self.currentPickup = new UniquePickup
            {
                pickupIndex = GetNewPickupIndex(self.currentPickup.pickupIndex, self.currentPickup.isTempItem),
                decayValue = self.currentPickup.decayValue,
            };
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
                isTempBlacklistedPickup = new bool[ItemTierCatalog.itemTierDefs.Length + 4];
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
            if (Run.instance.availableFoodTierDropList.Count > 0)
            {
                pickupsArrayOverride[(int)ItemTier.FoodTier] = Run.instance.availableFoodTierDropList[unisonRNG.RangeInt(0, Run.instance.availableFoodTierDropList.Count)];
            }
            //Equipment
            pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length] = NoRepeatsEquipment(ItemTierCatalog.itemTierDefs.Length, Run.instance.availableEquipmentDropList);
            pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1] = Run.instance.availableLunarEquipmentDropList[unisonRNG.RangeInt(0, Run.instance.availableLunarEquipmentDropList.Count)];
            if (PickupTransmutationManager.equipmentBossGroup.Length > 0)
            {
                pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 2] = PickupTransmutationManager.equipmentBossGroup[unisonRNG.RangeInt(0, PickupTransmutationManager.equipmentBossGroup.Length)];
            }

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


        public static PickupIndex GetNewPickupIndex(PickupIndex original, bool temp)
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
            if (pickupDef.itemIndex != ItemIndex.None)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
                if (!(itemDef.ContainsTag(ItemTag.Scrap) || itemDef.ContainsTag(ItemTag.ObjectiveRelated) || itemDef.ContainsTag(ItemTag.ObliterationRelated)))
                {
                    if (temp && isTempBlacklistedPickup[(int)itemDef.tier])
                    {
                        return original;
                    }
                    if (pickupsArrayOverride[(int)itemDef.tier].value != 0)
                    {
                        return pickupsArrayOverride[(int)itemDef.tier];
                    }
                }
            }
            else if (pickupDef.equipmentIndex != EquipmentIndex.None)
            {              
                if (pickupDef.isLunar)
                {
                    return pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1];
                }
                if (pickupDef.isBoss)
                {
                    return pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 2];
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

            if (tempDef.ContainsTag(ItemTag.CanBeTemporary))
            {
                isTempBlacklistedPickup[Tier] = false;
            }
            else
            {
                isTempBlacklistedPickup[Tier] = true;
            }

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

    /*public class UnisonBackupStorage : MonoBehaviour
    {
        public void Set()
        {
            if (!Run.instance)
            {
                return;
            }
            availableTier1DropList.CopyFrom(Run.instance.availableTier1DropList);
            availableTier2DropList.CopyFrom(Run.instance.availableTier2DropList);
            availableTier3DropList.CopyFrom(Run.instance.availableTier3DropList);
            availableBossDropList.CopyFrom(Run.instance.availableBossDropList);

            availableVoidTier1DropList.CopyFrom(Run.instance.availableVoidTier1DropList);
            availableVoidTier2DropList.CopyFrom(Run.instance.availableVoidTier2DropList);
            availableVoidTier3DropList.CopyFrom(Run.instance.availableVoidTier3DropList);
            availableVoidBossDropList.CopyFrom(Run.instance.availableVoidBossDropList);

            availableLunarItemDropList.CopyFrom(Run.instance.availableLunarItemDropList);
            availableFoodTierDropList.CopyFrom(Run.instance.availableFoodTierDropList);

            availableEquipmentDropList.CopyFrom(Run.instance.availableEquipmentDropList);
            availableLunarEquipmentDropList.CopyFrom(Run.instance.availableLunarEquipmentDropList);
            availableLunarCombinedDropList.CopyFrom(Run.instance.availableLunarCombinedDropList);

        }
        public void UnSet()
        {
            if (!Run.instance)
            {
                return;
            }
            Run.instance.BuildDropTable();
            //Probably unneeded, just do the vanilla regenerate

            Run.instance.availableTier1DropList.CopyFrom(availableTier1DropList);
            Run.instance.availableTier2DropList.CopyFrom(availableTier2DropList);
            Run.instance.availableTier3DropList.CopyFrom(availableTier3DropList);
            Run.instance.availableBossDropList.CopyFrom(availableBossDropList);

            Run.instance.availableVoidTier1DropList.CopyFrom(availableVoidTier1DropList);
            Run.instance.availableVoidTier2DropList.CopyFrom(availableVoidTier2DropList);
            Run.instance.availableVoidTier3DropList.CopyFrom(availableVoidTier3DropList);
            Run.instance.availableVoidBossDropList.CopyFrom(availableVoidBossDropList);

            Run.instance.availableLunarItemDropList.CopyFrom(availableLunarItemDropList);
            Run.instance.availableFoodTierDropList.CopyFrom(availableFoodTierDropList);

            Run.instance.availableEquipmentDropList.CopyFrom(availableEquipmentDropList);
            Run.instance.availableLunarEquipmentDropList.CopyFrom(availableLunarEquipmentDropList);
            Run.instance.availableLunarCombinedDropList.CopyFrom(availableLunarCombinedDropList);
        }

        public List<PickupIndex> availableTier1DropList = new List<PickupIndex>();
        public List<PickupIndex> availableTier2DropList = new List<PickupIndex>();
        public List<PickupIndex> availableTier3DropList = new List<PickupIndex>();

        public List<PickupIndex> availableLunarItemDropList = new List<PickupIndex>();
        public List<PickupIndex> availableBossDropList = new List<PickupIndex>();

        public List<PickupIndex> availableEquipmentDropList = new List<PickupIndex>();
        public List<PickupIndex> availableLunarEquipmentDropList = new List<PickupIndex>();

        public List<PickupIndex> availableLunarCombinedDropList = new List<PickupIndex>();

        public List<PickupIndex> availableVoidTier1DropList = new List<PickupIndex>();
        public List<PickupIndex> availableVoidTier2DropList = new List<PickupIndex>();
        public List<PickupIndex> availableVoidTier3DropList = new List<PickupIndex>();
        public List<PickupIndex> availableVoidBossDropList = new List<PickupIndex>();
        public List<PickupIndex> availableFoodTierDropList = new List<PickupIndex>();

    }
    */
}