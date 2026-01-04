using RoR2;
using RoR2.Artifacts;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace ArtifactDissimilarity.Aritfacts
{
    public class Remodeling
    {
        public static readonly System.Random random = new System.Random();

        //public static BasicPickupDropTable dtMonsterTeamTier2Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier2Item.asset").WaitForCompletion();
        //public static List<EquipmentIndex> EliteEquipmentList = new List<EquipmentIndex>();
        //public static List<EquipmentIndex> BlacklistedEquipment = new List<EquipmentIndex>();
        public static List<List<PickupIndex>> itemPickupLists;
        public static List<List<PickupIndex>> equipmentPickupLists;


        public static void OnArtifactEnable()
        {
            MakePickupIndexLists();
            SceneDirector.onPrePopulateSceneServer += RandomizeOnStageStart;
            SceneDirector.onGenerateInteractableCardSelection += RemoveInteractables_Remodeling;
            Debug.Log("Added Remodeling");
        }

        public static void OnArtifactDisable()
        {
            SceneDirector.onPrePopulateSceneServer -= RandomizeOnStageStart;
            SceneDirector.onGenerateInteractableCardSelection -= RemoveInteractables_Remodeling;
            itemPickupLists = null;
            equipmentPickupLists = null;
            Debug.Log("Removed Remodeling");
        }


        private static void RemoveInteractables_Remodeling(SceneDirector sceneDirector, DirectorCardCategorySelection dccs)
        {
            dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.NoMorePrinters));
        }



        public static void MakePickupIndexLists()
        {
            if (!Run.instance)
            {
                return;
            }
            Run.instance.BuildDropTable();

            int tierSize = ItemTierCatalog.itemTierDefs.Length + 1;
            itemPickupLists = new List<List<PickupIndex>>(tierSize);
            equipmentPickupLists = new List<List<PickupIndex>>(3);
            for (int i = 0; i < tierSize; i++)
            {
                itemPickupLists.Add(new List<PickupIndex>());
            }
            for (int i = 0; i < 3; i++)
            {
                equipmentPickupLists.Add(new List<PickupIndex>());
            }


            itemPickupLists[(int)ItemTier.Tier1].AddRange(Run.instance.availableTier1DropList);
            itemPickupLists[(int)ItemTier.Tier2].AddRange(Run.instance.availableTier2DropList);
            itemPickupLists[(int)ItemTier.Tier3].AddRange(Run.instance.availableTier3DropList);
            itemPickupLists[(int)ItemTier.Lunar].AddRange(Run.instance.availableLunarItemDropList);
            itemPickupLists[(int)ItemTier.Boss].AddRange(Run.instance.availableBossDropList);
            //NoTier
            itemPickupLists[(int)ItemTier.VoidTier1].AddRange(Run.instance.availableVoidTier1DropList);
            itemPickupLists[(int)ItemTier.VoidTier2].AddRange(Run.instance.availableVoidTier2DropList);
            itemPickupLists[(int)ItemTier.VoidTier3].AddRange(Run.instance.availableVoidTier3DropList);
            itemPickupLists[(int)ItemTier.VoidBoss].AddRange(Run.instance.availableVoidBossDropList);

            itemPickupLists[(int)ItemTier.Lunar].Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarPrimaryReplacement.itemIndex));
            itemPickupLists[(int)ItemTier.Lunar].Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarSecondaryReplacement.itemIndex));
            itemPickupLists[(int)ItemTier.Lunar].Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarUtilityReplacement.itemIndex));
            itemPickupLists[(int)ItemTier.Lunar].Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarSpecialReplacement.itemIndex));
            itemPickupLists[(int)ItemTier.Lunar].Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarTrinket.itemIndex));
            itemPickupLists[(int)ItemTier.Boss].Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex));
            itemPickupLists[(int)ItemTier.Boss].Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex));


            equipmentPickupLists[0].AddRange(Run.instance.availableEquipmentDropList);
            equipmentPickupLists[1].AddRange(Run.instance.availableLunarEquipmentDropList);
            equipmentPickupLists[2].AddRange(PickupTransmutationManager.equipmentBossGroup);
        }





        public static void RandomizeOnStageStart(SceneDirector self)
        {
            if (!Run.instance)
            {
                return;
            }
            else if (Run.instance && Run.instance.stageClearCount == 0)
            {
                return;
            }
            if (itemPickupLists == null)
            {
                MakePickupIndexLists();
            }
            foreach (var playerController in PlayerCharacterMasterController.instances)
            {
                Inventory inv = playerController.master.inventory;
                if (inv)
                {
                    RandomizeItemsNew(inv, false, false);
                    RandomizeEquipmentNew(inv);
                }
            }
            if (WConfig.RemodelRerollEquipmentDrones.Value == true)
            {
                foreach (CharacterMaster master in CharacterMaster.instancesList)
                {
                    if (master && master.inventory)
                    {
                        if (master.name.EndsWith("DroneMaster") || master.name.StartsWith("Turret"))
                        {
                            RandomizeEquipmentNew(master.inventory);
                        }
                    }
                }
            }
            if (WConfig.RemodelRerollMonsterItems.Value == true)
            {
                if (Run.instance is InfiniteTowerRun)
                {
                    RandomizeItemsNew((Run.instance as InfiniteTowerRun).enemyInventory, true, false);
                }
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.MonsterTeamGainsItems))
                {
                    RandomizeItemsNew(MonsterTeamGainsItemsArtifactManager.monsterTeamInventory, true, false);
                }
            }
            if (WConfig.RemodelRerollDevotion.Value)
            {
                foreach (DevotionInventoryController devotionInventory in DevotionInventoryController.InstanceList)
                {
                    RandomizeItemsNew(devotionInventory._devotionMinionInventory, false, true);
                }
            }

            System.GC.Collect();
        }



        public static void RandomizeEquipmentNew(Inventory inv)
        {
            if (!inv)
            {
                Debug.Log("Attempted to reroll equipment of Null inventory");
                return;
            }
            //Slot is MulT
            //Set is FunctionalCoupler

            for (uint slot = 0; slot < inv._equipmentStateSlots.Length; slot++)
            {
                for (uint set = 0; set < inv._equipmentStateSlots[slot].Length; set++)
                {
                    var state = inv._equipmentStateSlots[slot][set];
                    if (state.equipmentIndex != EquipmentIndex.None)
                    {
                        EquipmentDef equipDef = EquipmentCatalog.GetEquipmentDef(inv.currentEquipmentIndex);
                        if (equipDef.isBoss)
                        {
                            int randomIndex = random.Next(0, equipmentPickupLists[2].Count);
                            inv.SetEquipmentIndexForSlot(equipmentPickupLists[2][randomIndex].equipmentIndex, slot, set);
                        }
                        else if (equipDef.isLunar)
                        {
                            int randomIndex = random.Next(0, equipmentPickupLists[1].Count);
                            inv.SetEquipmentIndexForSlot(equipmentPickupLists[1][randomIndex].equipmentIndex, slot, set);
                        }
                        else if (equipDef.isConsumed || equipDef.canDrop)
                        {
                            int randomIndex = random.Next(0, equipmentPickupLists[0].Count);
                            inv.SetEquipmentIndexForSlot(equipmentPickupLists[0][randomIndex].equipmentIndex, slot, set);
                        }
                        Debug.Log($"Rerolled {inv}'s equipment");
                    }
                }
            }
        }


        public enum InventoryOwner
        {
            Player,
            Evolution,
            Devotion
        }
        public static void RandomizeItemsNew(Inventory inv, bool isEnemyTeam, bool isDevotion)
        {
            if (!inv)
            {
                Debug.Log("Attempted to reroll items of Null inventory");
                return;
            }
            int LengthLists = ItemTierCatalog.itemTierDefs.Length + 1;
            int LengthDo = (int)ItemTier.AssignedAtRuntime - 1;



            int[] srcItemStacks = ItemCatalog.RequestItemStackArray();
            inv.WriteAllPermanentItemStacks(srcItemStacks);

            int[] amountPerTier = new int[LengthLists];
            List<List<int>> itemStacksList = new List<List<int>>(LengthLists);
            for (int i = 0; i < LengthLists; i++)
            {
                itemStacksList.Add(new List<int>());
            }

            //While it'd be fun to work on stuff like Essences and Highlighlander, I don't think the game stores anything like that as a group
            //Could do it myself but eeeehhh, how'd we know if it's like a stupid leftover or an item that can actually drop.
            //Better to use Run because i don't think pickuptransmanager knows if a item is disabled

            #region Compile Permament Item Stack Sizes
            for (var i = 0; i < srcItemStacks.Length; i++)
            {
                if (srcItemStacks[i] > 0)
                {
                    ItemDef itemDef = ItemCatalog.GetItemDef((ItemIndex)i);
                    if (itemDef.tier == ItemTier.NoTier || itemDef.tier > ItemTier.AssignedAtRuntime)
                    {
                        //No untiered or modded tiers
                        continue;
                    }
                    if (itemDef.name.EndsWith("Replacement"))
                    {
                        //Important to run too shouldn't reroll
                        continue;
                    }
                    if (itemDef.ContainsTag(ItemTag.ObjectiveRelated) || itemDef.ContainsTag(ItemTag.ObliterationRelated) || itemDef.ContainsTag(ItemTag.WorldUnique) && !(itemDef.ContainsTag(ItemTag.Scrap) || itemDef.name.EndsWith("Pearl")))
                    {
                        //Filter out world unique items like Captain. But we keep pearls in rotation
                        continue;
                    }
                    amountPerTier[(int)itemDef.tier]++;
                    itemStacksList[(int)itemDef.tier].Add(srcItemStacks[i]);
                    inv.RemoveItemPermanent((ItemIndex)i, srcItemStacks[i]);
                }
            }
            #endregion

            ItemTag[] banndItemTags = new ItemTag[0];
            if (isEnemyTeam)
            {
                banndItemTags = new ItemTag[]
                {
                    ItemTag.AIBlacklist,
                    ItemTag.CannotCopy,
                    ItemTag.OnKillEffect,
                    ItemTag.EquipmentRelated,
                    ItemTag.SprintRelated,
                    ItemTag.HoldoutZoneRelated,
                    ItemTag.OnStageBeginEffect,
                    ItemTag.InteractableRelated,
                };
            }
            if (isDevotion)
            {
                banndItemTags = new ItemTag[]
                {
                    ItemTag.CannotCopy,
                    ItemTag.EquipmentRelated,
                    ItemTag.SprintRelated,
                    ItemTag.InteractableRelated,
                    ItemTag.DevotionBlacklist,
                };
            }

            for (var tier = 0; tier < LengthDo; tier++)
            {
                if (tier == (int)ItemTier.NoTier)
                {
                    continue;
                }
                if (itemPickupLists[tier].Count <= 1)
                {
                    //Skip item tiers with only 1 (Void Boss)
                    continue;
                }
                if (amountPerTier[tier] == 0)
                {
                    //Dont reroll tiers that you didnt have
                    continue;
                }
                List<PickupIndex> forUseList = new List<PickupIndex>();
                forUseList.AddRange(itemPickupLists[tier]);

                if (isEnemyTeam || isDevotion)
                {
                    for (var i = 0; i < forUseList.Count; i++)
                    {
                        ItemDef temp = ItemCatalog.GetItemDef(forUseList[i].itemIndex);
                        if (temp.tags.Length == 0)
                        {
                            continue;
                        }
                        foreach (ItemTag itemTag in banndItemTags)
                        {
                            if (temp.ContainsTag(itemTag))
                            {
                                forUseList.Remove(forUseList[i]);
                                i--;
                                break;
                            }
                        }
                    }
                }

                for (var i = 0; i < amountPerTier[tier]; i++)
                {
                    if (forUseList.Count == 0)
                    {
                        forUseList.AddRange(itemPickupLists[tier]);
                    }
                    int Windex = random.Next(0, forUseList.Count);
                    int WCount = random.Next(0, itemStacksList[tier].Count);
                    inv.GiveItemPermanent(forUseList[Windex].itemIndex, itemStacksList[tier][WCount]);
                    forUseList.Remove(forUseList[Windex]);
                    itemStacksList[tier].Remove(itemStacksList[tier][WCount]);
                }
            }


            #region Temporary 
            if (!isEnemyTeam || isDevotion)
            {

                float[] srcItemStacksTEMP = new float[srcItemStacks.Length];
                inv.WriteAllTempItemRawValues(srcItemStacksTEMP);

                int[] amountPerTierTEMP = new int[LengthLists];
                List<List<float>> itemStacksListTEMP = new List<List<float>>(LengthLists);
                for (int i = 0; i < LengthLists; i++)
                {
                    itemStacksListTEMP.Add(new List<float>());
                }

                //While it'd be fun to work on stuff like Essences and Highlighlander, I don't think the game stores anything like that as a group
                //Could do it myself but eeeehhh, how'd we know if it's like a stupid leftover or an item that can actually drop.
                //Better to use Run because i don't think pickuptransmanager knows if a item is disabled

                #region Compile Temp Item Stack Sizes
                for (var i = 0; i < srcItemStacksTEMP.Length; i++)
                {
                    //Debug.Log($"{i} | {srcItemStacksTEMP[i]}");
                    if (srcItemStacksTEMP[i] > 0)
                    {
                        ItemDef itemDef = ItemCatalog.GetItemDef((ItemIndex)i);
                        if (itemDef.tier == ItemTier.NoTier || itemDef.tier > ItemTier.AssignedAtRuntime)
                        {
                            //No untiered or modded tiers
                            continue;
                        }
                        if (itemDef.ContainsTag(ItemTag.ObjectiveRelated) || itemDef.ContainsTag(ItemTag.ObliterationRelated) || itemDef.ContainsTag(ItemTag.WorldUnique) && !(itemDef.ContainsTag(ItemTag.Scrap) || itemDef.name.EndsWith("Pearl")))
                        {
                            //Filter out world unique items like Captain. But we keep pearls in rotation
                            continue;
                        }
                        amountPerTierTEMP[(int)itemDef.tier]++;
                        itemStacksListTEMP[(int)itemDef.tier].Add(srcItemStacksTEMP[i]);
                        inv.RemoveItemTemp((ItemIndex)i, srcItemStacksTEMP[i]);
                    }
                }
                #endregion


                for (var tier = 0; tier < LengthDo; tier++)
                {
                    if (tier == (int)ItemTier.NoTier)
                    {
                        continue;
                    }
                    if (itemPickupLists[tier].Count <= 1)
                    {
                        //Skip item tiers with only 1 (Void Boss)
                        continue;
                    }
                    if (amountPerTierTEMP[tier] == 0)
                    {
                        //Dont reroll tiers that you didnt have
                        continue;
                    }
                    List<PickupIndex> forUseList = new List<PickupIndex>();
                    forUseList.AddRange(itemPickupLists[tier]);

                    for (var i = 0; i < forUseList.Count; i++)
                    {
                        ItemDef temp = ItemCatalog.GetItemDef(forUseList[i].itemIndex);
                        if (!temp.ContainsTag(ItemTag.CanBeTemporary))
                        {
                            forUseList.Remove(forUseList[i]);
                            i--;
                            break;
                        }
                    }
                    for (var i = 0; i < amountPerTierTEMP[tier]; i++)
                    {
                        if (forUseList.Count == 0)
                        {
                            forUseList.AddRange(itemPickupLists[tier]);
                        }
                        int Windex = random.Next(0, forUseList.Count);
                        int WCount = random.Next(0, itemStacksListTEMP[tier].Count);
                        inv.GiveItemTemp(forUseList[Windex].itemIndex, itemStacksListTEMP[tier][WCount]);
                        forUseList.Remove(forUseList[Windex]);
                        itemStacksListTEMP[tier].Remove(itemStacksListTEMP[tier][WCount]);
                    }
                }

            }
            #endregion

            Debug.Log($"Rerolled {inv}'s Items");
        }

    }
}