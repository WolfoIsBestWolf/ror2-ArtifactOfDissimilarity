using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;


namespace ArtifactDissimilarity
{
    public class Remodeling
    {
        public static readonly System.Random random = new System.Random();

        public static BasicPickupDropTable dtMonsterTeamTier2Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier2Item.asset").WaitForCompletion();


        public static List<EquipmentIndex> EliteEquipmentList = new List<EquipmentIndex>();
        static List<EquipmentIndex> BlacklistedEquipment = new List<EquipmentIndex>();

        public static void RandomizeMain(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Main.Unison_Def))
                {
                    Run.instance.BuildDropTable();
                }
                RandomizeItems();
                RandomizeEquipment();
                if (WConfig.RemodelRerollEquipmentDrones.Value == true)
                {
                    RandomizeEquipmentDrone();
                }
                if (WConfig.RemodelRerollMonsterItems.Value == true)
                {
                    RandomizeItemsMonster();
                }
            }
            System.GC.Collect();
        }



        public static void RandomizeEquipment()
        {
            foreach (var playerController in PlayerCharacterMasterController.instances)
            {
                Inventory inv = playerController.master.inventory;
                if (inv == null) { return; }

                List<PickupIndex> TempTrimOrangeEquipmentList = new List<PickupIndex>();
                List<PickupIndex> TempTrimBlueEquipmentList = new List<PickupIndex>();
                List<EquipmentIndex> TempTrimEliteEquipmentList = new List<EquipmentIndex>();
                TempTrimOrangeEquipmentList.AddRange(Run.instance.availableEquipmentDropList);
                TempTrimBlueEquipmentList.AddRange(Run.instance.availableLunarEquipmentDropList);
                //TempTrimYellowEquipmentList.AddRange(YellowEquipmentList);
                TempTrimEliteEquipmentList.AddRange(EliteEquipmentList);

                for (byte k = 0; k < inv.GetEquipmentSlotCount(); k++)
                {

                    playerController.master.inventory.SetActiveEquipmentSlot(k);

                    if (inv.GetEquipmentIndex() != EquipmentIndex.None)
                    {
                        int tagcheckertemp = 0;

                        if (BlacklistedEquipment.Contains(inv.currentEquipmentIndex))
                        {
                            tagcheckertemp = 1;
                        }

                        if (tagcheckertemp == 0)
                        {
                            //string tempname2 = EquipmentCatalog.GetEquipmentDef(inv.currentEquipmentIndex).name;
                            if (EliteEquipmentList.Contains(inv.currentEquipmentIndex))
                            {
                                TempTrimEliteEquipmentList.Remove(inv.currentEquipmentIndex);
                                if (TempTrimEliteEquipmentList.Count == 0)
                                {
                                    TempTrimEliteEquipmentList.AddRange(EliteEquipmentList);
                                }
                                int T1Eindex = random.Next(0, TempTrimEliteEquipmentList.Count);
                                inv.SetEquipment(new EquipmentState(TempTrimEliteEquipmentList[T1Eindex], Run.FixedTimeStamp.negativeInfinity, 0), k);

                            }
                            else if (EquipmentCatalog.GetEquipmentDef(inv.currentEquipmentIndex).isLunar == true)
                            {
                                TempTrimBlueEquipmentList.Remove(PickupCatalog.FindPickupIndex(inv.currentEquipmentIndex));
                                if (TempTrimBlueEquipmentList.Count == 0)
                                {
                                    TempTrimBlueEquipmentList.AddRange(Run.instance.availableLunarEquipmentDropList);
                                }
                                int BEindex = random.Next(0, TempTrimBlueEquipmentList.Count);
                                inv.SetEquipment(new EquipmentState(TempTrimBlueEquipmentList[BEindex].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 0), k);
                            }
                            else
                            {
                                TempTrimOrangeEquipmentList.Remove(PickupCatalog.FindPickupIndex(inv.currentEquipmentIndex));
                                if (TempTrimOrangeEquipmentList.Count == 0)
                                {
                                    TempTrimOrangeEquipmentList.AddRange(Run.instance.availableEquipmentDropList);
                                }
                                int OEindex = random.Next(0, TempTrimOrangeEquipmentList.Count);
                                inv.SetEquipment(new EquipmentState(TempTrimOrangeEquipmentList[OEindex].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 0), k);
                            }
                        }
                        Debug.Log($"Rerolled {playerController.GetDisplayName()}'s equipment");
                    }
                }
            }
        }

        public static void RandomizeEquipmentDrone()
        {
            foreach (var gameObj in UnityEngine.Object.FindObjectsOfType(typeof(CharacterMaster)) as CharacterMaster[])
            {
                if (gameObj.name.Contains("Drone") || gameObj.name.Contains("Turret"))
                {
                    Inventory invDrone = gameObj.GetComponent<Inventory>();

                    if (invDrone.GetEquipmentIndex() != EquipmentIndex.None)
                    {
                        int tagcheckertemp = 0;
                        if (BlacklistedEquipment.Contains(invDrone.currentEquipmentIndex))
                        {
                            tagcheckertemp = 1;
                        }

                        if (tagcheckertemp == 0)
                        {
                            string tempname2 = EquipmentCatalog.GetEquipmentDef(invDrone.currentEquipmentIndex).name;
                            if (EliteEquipmentList.Contains(invDrone.currentEquipmentIndex))
                            {
                                if (tempname2.Contains("Gold") || tempname2.Contains("Echo") || tempname2.Contains("Yellow") || tempname2.Contains("SecretSpeed"))
                                {
                                }
                                else
                                {
                                    int T1Eindex = random.Next(0, EliteEquipmentList.Count);
                                    invDrone.SetEquipment(new EquipmentState(EliteEquipmentList[T1Eindex], Run.FixedTimeStamp.negativeInfinity, 0), 0);
                                }
                            }
                            else if (EquipmentCatalog.GetEquipmentDef(invDrone.currentEquipmentIndex).isLunar == true)
                            {
                                int BEindex = random.Next(0, Run.instance.availableLunarEquipmentDropList.Count);
                                invDrone.SetEquipment(new EquipmentState(Run.instance.availableLunarEquipmentDropList[BEindex].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 0), 0);
                            }
                            else
                            {
                                int OEindex = random.Next(0, Run.instance.availableEquipmentDropList.Count);
                                invDrone.SetEquipment(new EquipmentState(Run.instance.availableEquipmentDropList[OEindex].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 0), 0);
                            }
                            Debug.Log($"Rerolled {gameObj.name}'s equipment");
                        }
                    }
                }


            }
        }



        //This hell of a method could be redone more cleanly
        public static void RandomizeItems()
        {
            foreach (var playerController in PlayerCharacterMasterController.instances)
            {
                Inventory inv = playerController.master.inventory;
                //int[] invoutput = new int[TotalItemCount];
                //inv.WriteItemStacks(invoutput);
                int[] invoutput = new int[inv.itemStacks.Length];
                inv.itemStacks.CopyTo(invoutput, 0);

                int WhiteCount = 0;
                int GreenCount = 0;
                int RedCount = 0;
                int YellowCount = 0;
                int BlueCount = 0;
                int PinkT1Count = 0;
                int PinkT2Count = 0;
                int PinkT3Count = 0;
                int PinkBossCount = 0;

                List<PickupIndex> TempTrimWhiteItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimGreenItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimRedItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimYellowItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimBlueItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimPinkT1ItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimPinkT2ItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimPinkT3ItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimPinkBossItemList = new List<PickupIndex>();

                TempTrimWhiteItemList.AddRange(Run.instance.availableTier1DropList);
                TempTrimGreenItemList.AddRange(Run.instance.availableTier2DropList);
                TempTrimRedItemList.AddRange(Run.instance.availableTier3DropList);
                TempTrimYellowItemList.AddRange(Run.instance.availableBossDropList);
                TempTrimYellowItemList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex));
                TempTrimYellowItemList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex));
                //TempTrimYellowItemList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.TitanGoldDuringTP.itemIndex));

                TempTrimBlueItemList.AddRange(Run.instance.availableLunarItemDropList);
                TempTrimBlueItemList.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarPrimaryReplacement.itemIndex));
                TempTrimBlueItemList.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarSecondaryReplacement.itemIndex));
                TempTrimBlueItemList.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarUtilityReplacement.itemIndex));
                TempTrimBlueItemList.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarSpecialReplacement.itemIndex));
                TempTrimBlueItemList.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarTrinket.itemIndex));

                TempTrimPinkT1ItemList.AddRange(Run.instance.availableVoidTier1DropList);
                TempTrimPinkT2ItemList.AddRange(Run.instance.availableVoidTier2DropList);
                TempTrimPinkT3ItemList.AddRange(Run.instance.availableVoidTier3DropList);
                TempTrimPinkBossItemList.AddRange(Run.instance.availableVoidBossDropList);

                List<int> WhiteItemCountList = new List<int>();
                List<int> GreenItemCountList = new List<int>();
                List<int> RedItemCountList = new List<int>();
                List<int> YellowItemCountList = new List<int>();
                List<int> BlueItemCountList = new List<int>();
                List<int> PinkT1ItemCountList = new List<int>();
                List<int> PinkT2ItemCountList = new List<int>();
                List<int> PinkT3ItemCountList = new List<int>();
                List<int> PinkBossItemCountList = new List<int>();

                for (var i = 0; i < invoutput.Length; i++)
                {
                    if (invoutput[i] > 0)
                    {
                        ItemDef tempitemdef = ItemCatalog.GetItemDef((ItemIndex)i);
                        //Debug.LogWarning(tempitemdef);
                        if (tempitemdef.name.Equals("LunarTrinket") || tempitemdef.name.EndsWith("Replacement"))
                        { }
                        else if (tempitemdef.ContainsTag(ItemTag.Scrap) || tempitemdef.name.EndsWith("Pearl") || tempitemdef.DoesNotContainTag(ItemTag.WorldUnique))
                        {
                            switch (tempitemdef.tier)
                            {
                                case ItemTier.Tier1:
                                    WhiteCount++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    WhiteItemCountList.Add(invoutput[i]);
                                    break;
                                case ItemTier.Tier2:
                                    GreenCount++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    GreenItemCountList.Add(invoutput[i]);
                                    break;
                                case ItemTier.Tier3:
                                    RedCount++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    RedItemCountList.Add(invoutput[i]);
                                    break;
                                case ItemTier.Boss:
                                    YellowCount++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    YellowItemCountList.Add(invoutput[i]);
                                    break;
                                case ItemTier.Lunar:
                                    BlueCount++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    BlueItemCountList.Add(invoutput[i]);
                                    break;
                                case ItemTier.VoidTier1:
                                    PinkT1Count++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    PinkT1ItemCountList.Add(invoutput[i]);
                                    break;
                                case ItemTier.VoidTier2:
                                    PinkT2Count++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    PinkT2ItemCountList.Add(invoutput[i]);
                                    break;
                                case ItemTier.VoidTier3:
                                    PinkT3Count++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    PinkT3ItemCountList.Add(invoutput[i]);
                                    break;
                                case ItemTier.VoidBoss:
                                    PinkBossCount++;
                                    inv.RemoveItem((ItemIndex)i, invoutput[i]);
                                    PinkBossItemCountList.Add(invoutput[i]);
                                    break;


                            }
                        }
                    }
                }

                for (var w = 0; w < PinkT1Count; w++)
                {
                    int Windex = random.Next(0, TempTrimPinkT1ItemList.Count);
                    int WCount = random.Next(0, PinkT1ItemCountList.Count);
                    if (TempTrimPinkT1ItemList.Count == 0)
                    {
                        TempTrimPinkT1ItemList.AddRange(Run.instance.availableVoidTier1DropList);
                    }
                    inv.GiveItem(TempTrimPinkT1ItemList[Windex].itemIndex, PinkT1ItemCountList[WCount]);
                    TempTrimPinkT1ItemList.Remove(TempTrimPinkT1ItemList[Windex]);
                    PinkT1ItemCountList.Remove(PinkT1ItemCountList[WCount]);
                }
                for (var w = 0; w < PinkT2Count; w++)
                {
                    int Windex = random.Next(0, TempTrimPinkT2ItemList.Count);
                    int WCount = random.Next(0, PinkT2ItemCountList.Count);
                    if (TempTrimPinkT2ItemList.Count == 0)
                    {
                        TempTrimPinkT2ItemList.AddRange(Run.instance.availableVoidTier2DropList);
                    }
                    inv.GiveItem(TempTrimPinkT2ItemList[Windex].itemIndex, PinkT2ItemCountList[WCount]);
                    TempTrimPinkT2ItemList.Remove(TempTrimPinkT2ItemList[Windex]);
                    PinkT2ItemCountList.Remove(PinkT2ItemCountList[WCount]);
                }
                for (var w = 0; w < PinkT3Count; w++)
                {
                    int Windex = random.Next(0, TempTrimPinkT3ItemList.Count);
                    int WCount = random.Next(0, PinkT3ItemCountList.Count);
                    if (TempTrimPinkT3ItemList.Count == 0)
                    {
                        TempTrimPinkT3ItemList.AddRange(Run.instance.availableVoidTier3DropList);
                    }
                    inv.GiveItem(TempTrimPinkT3ItemList[Windex].itemIndex, PinkT3ItemCountList[WCount]);
                    TempTrimPinkT3ItemList.Remove(TempTrimPinkT3ItemList[Windex]);
                    PinkT3ItemCountList.Remove(PinkT3ItemCountList[WCount]);
                }
                for (var w = 0; w < PinkBossCount; w++)
                {
                    int Windex = random.Next(0, TempTrimPinkBossItemList.Count);
                    int WCount = random.Next(0, PinkBossItemCountList.Count);
                    if (TempTrimPinkBossItemList.Count == 0)
                    {
                        TempTrimPinkBossItemList.AddRange(Run.instance.availableVoidBossDropList);
                    }
                    inv.GiveItem(TempTrimPinkBossItemList[Windex].itemIndex, PinkBossItemCountList[WCount]);
                    TempTrimPinkBossItemList.Remove(TempTrimPinkBossItemList[Windex]);
                    PinkBossItemCountList.Remove(PinkBossItemCountList[WCount]);
                }
                //
                for (var w = 0; w < WhiteCount; w++)
                {
                    int Windex = random.Next(0, TempTrimWhiteItemList.Count);
                    int WCount = random.Next(0, WhiteItemCountList.Count);
                    if (TempTrimWhiteItemList.Count == 0)
                    {
                        TempTrimWhiteItemList.AddRange(Run.instance.availableTier1DropList);
                    }
                    inv.GiveItem(TempTrimWhiteItemList[Windex].itemIndex, WhiteItemCountList[WCount]);
                    TempTrimWhiteItemList.Remove(TempTrimWhiteItemList[Windex]);
                    WhiteItemCountList.Remove(WhiteItemCountList[WCount]);

                }
                for (var g = 0; g < GreenCount; g++)
                {
                    int Gindex = random.Next(0, TempTrimGreenItemList.Count);
                    int GCount = random.Next(0, GreenItemCountList.Count);
                    if (TempTrimGreenItemList.Count == 0)
                    {
                        TempTrimGreenItemList.AddRange(Run.instance.availableTier2DropList);
                    }
                    inv.GiveItem(TempTrimGreenItemList[Gindex].itemIndex, GreenItemCountList[GCount]);
                    TempTrimGreenItemList.Remove(TempTrimGreenItemList[Gindex]);
                    GreenItemCountList.Remove(GreenItemCountList[GCount]);
                }
                for (var r = 0; r < RedCount; r++)
                {
                    int Rindex = random.Next(0, TempTrimRedItemList.Count);
                    int RCount = random.Next(0, RedItemCountList.Count);
                    if (TempTrimRedItemList.Count == 0)
                    {
                        TempTrimRedItemList.AddRange(Run.instance.availableTier3DropList);
                    }
                    inv.GiveItem(TempTrimRedItemList[Rindex].itemIndex, RedItemCountList[RCount]);
                    TempTrimRedItemList.Remove(TempTrimRedItemList[Rindex]);
                    RedItemCountList.Remove(RedItemCountList[RCount]);
                }
                for (var y = 0; y < YellowCount; y++)
                {
                    int Yindex = random.Next(0, TempTrimYellowItemList.Count);
                    int YCount = random.Next(0, YellowItemCountList.Count);
                    if (TempTrimYellowItemList.Count == 0)
                    {
                        TempTrimYellowItemList.AddRange(Run.instance.availableBossDropList);
                    }
                    inv.GiveItem(TempTrimYellowItemList[Yindex].itemIndex, YellowItemCountList[YCount]);
                    TempTrimYellowItemList.Remove(TempTrimYellowItemList[Yindex]);
                    YellowItemCountList.Remove(YellowItemCountList[YCount]);
                }
                for (var b = 0; b < BlueCount; b++)
                {
                    int Bindex = random.Next(0, TempTrimBlueItemList.Count);
                    int BCount = random.Next(0, BlueItemCountList.Count);
                    if (TempTrimBlueItemList.Count == 0)
                    {
                        TempTrimBlueItemList.AddRange(Run.instance.availableLunarItemDropList);
                    }
                    inv.GiveItem(TempTrimBlueItemList[Bindex].itemIndex, BlueItemCountList[BCount]);
                    TempTrimBlueItemList.Remove(TempTrimBlueItemList[Bindex]);
                    BlueItemCountList.Remove(BlueItemCountList[BCount]);
                }
                Debug.Log($"Rerolled {playerController.GetDisplayName()}'s inventory");
            }
        }

        public static void RandomizeItemsMonster()
        {
            if (Run.instance && Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
            {
                Inventory SimulacrumInventory = Run.instance.GetComponent<Inventory>();
                InfiniteTowerRun SimulacrumRun = Run.instance.GetComponent<InfiniteTowerRun>();
                int[] invoutput = new int[SimulacrumInventory.itemStacks.Length];
                SimulacrumInventory.itemStacks.CopyTo(invoutput, 0);

                int WhiteCount = 0;
                int GreenCount = 0;
                int RedCount = 0;
                int BlueCount = 0;

                //BasicPickupDropTable

                BasicPickupDropTable SimulacrumDropTable = (BasicPickupDropTable)SimulacrumRun.enemyItemPattern[0].dropTable;
                List<ItemTag> BannedItemTagsSimu = SimulacrumDropTable.bannedItemTags.ToList();

                List<PickupIndex> SimulacrumWhiteItemList = new List<PickupIndex>();
                List<PickupIndex> SimulacrumGreenItemList = new List<PickupIndex>();
                List<PickupIndex> SimulacrumRedItemList = new List<PickupIndex>();
                List<PickupIndex> SimulacrumBlueItemList = new List<PickupIndex>();
                SimulacrumWhiteItemList.AddRange(Run.instance.availableTier1DropList);
                SimulacrumGreenItemList.AddRange(Run.instance.availableTier2DropList);
                SimulacrumRedItemList.AddRange(Run.instance.availableTier3DropList);
                SimulacrumBlueItemList.AddRange(Run.instance.availableLunarItemDropList);

                for (int i = SimulacrumWhiteItemList.Count - 1; i >= 0; i--)
                {
                    ItemDef temp = ItemCatalog.GetItemDef(SimulacrumWhiteItemList[i].itemIndex);
                    for (int tags = 0; tags < temp.tags.Length; tags++)
                    {
                        if (BannedItemTagsSimu.Contains(temp.tags[tags]))
                        {
                            SimulacrumWhiteItemList.Remove(SimulacrumWhiteItemList[i]);
                            tags = 100;
                        }
                    }
                }
                for (int i = SimulacrumGreenItemList.Count - 1; i >= 0; i--)
                {
                    ItemDef temp = ItemCatalog.GetItemDef(SimulacrumGreenItemList[i].itemIndex);
                    for (int tags = 0; tags < temp.tags.Length; tags++)
                    {
                        if (BannedItemTagsSimu.Contains(temp.tags[tags]))
                        {
                            SimulacrumGreenItemList.Remove(SimulacrumGreenItemList[i]);
                            tags = 100;
                        }
                    }
                }
                for (int i = SimulacrumRedItemList.Count - 1; i >= 0; i--)
                {
                    ItemDef temp = ItemCatalog.GetItemDef(SimulacrumRedItemList[i].itemIndex);
                    for (int tags = 0; tags < temp.tags.Length; tags++)
                    {
                        if (BannedItemTagsSimu.Contains(temp.tags[tags]))
                        {
                            SimulacrumRedItemList.Remove(SimulacrumRedItemList[i]);
                            tags = 100;
                        }
                    }
                }
                for (int i = SimulacrumBlueItemList.Count - 1; i >= 0; i--)
                {
                    ItemDef temp = ItemCatalog.GetItemDef(SimulacrumBlueItemList[i].itemIndex);
                    for (int tags = 0; tags < temp.tags.Length; tags++)
                    {
                        if (BannedItemTagsSimu.Contains(temp.tags[tags]))
                        {
                            SimulacrumBlueItemList.Remove(SimulacrumBlueItemList[i]);
                            tags = 100;
                        }
                    }
                }

                List<PickupIndex> TempTrimWhiteItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimGreenItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimRedItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimBlueItemList = new List<PickupIndex>();

                TempTrimWhiteItemList.AddRange(SimulacrumWhiteItemList);
                TempTrimGreenItemList.AddRange(SimulacrumGreenItemList);
                TempTrimRedItemList.AddRange(SimulacrumRedItemList);
                TempTrimBlueItemList.AddRange(SimulacrumBlueItemList);

                List<int> WhiteItemCountList = new List<int>();
                List<int> GreenItemCountList = new List<int>();
                List<int> RedItemCountList = new List<int>();
                List<int> BlueItemCountList = new List<int>();

                for (var i = 0; i < invoutput.Length; i++)
                {

                    if (invoutput[i] > 0)
                    {
                        ItemDef tempitemdef = ItemCatalog.GetItemDef((ItemIndex)i);
                        switch (tempitemdef.tier)
                        {
                            case ItemTier.Tier1:
                                WhiteCount++;
                                SimulacrumInventory.RemoveItem((ItemIndex)i, invoutput[i]);
                                WhiteItemCountList.Add(invoutput[i]);
                                break;
                            case ItemTier.Tier2:
                                GreenCount++;
                                SimulacrumInventory.RemoveItem((ItemIndex)i, invoutput[i]);
                                GreenItemCountList.Add(invoutput[i]);
                                break;
                            case ItemTier.Tier3:
                                RedCount++;
                                SimulacrumInventory.RemoveItem((ItemIndex)i, invoutput[i]);
                                RedItemCountList.Add(invoutput[i]);
                                break;
                            case ItemTier.Lunar:
                                BlueCount++;
                                SimulacrumInventory.RemoveItem((ItemIndex)i, invoutput[i]);
                                BlueItemCountList.Add(invoutput[i]);
                                break;
                        }
                    }
                }


                for (var w = 0; w < WhiteCount; w++)
                {
                    int Windex = random.Next(0, TempTrimWhiteItemList.Count);
                    int WCount = random.Next(0, WhiteItemCountList.Count);
                    if (TempTrimWhiteItemList.Count == 0)
                    {
                        TempTrimWhiteItemList.AddRange(SimulacrumWhiteItemList);
                    }
                    SimulacrumInventory.GiveItem(TempTrimWhiteItemList[Windex].itemIndex, WhiteItemCountList[WCount]);
                    TempTrimWhiteItemList.Remove(TempTrimWhiteItemList[Windex]);
                    WhiteItemCountList.Remove(WhiteItemCountList[WCount]);

                }
                for (var g = 0; g < GreenCount; g++)
                {
                    int Gindex = random.Next(0, TempTrimGreenItemList.Count);
                    int GCount = random.Next(0, GreenItemCountList.Count);
                    if (TempTrimGreenItemList.Count == 0)
                    {
                        TempTrimGreenItemList.AddRange(SimulacrumGreenItemList);
                    }
                    SimulacrumInventory.GiveItem(TempTrimGreenItemList[Gindex].itemIndex, GreenItemCountList[GCount]);
                    TempTrimGreenItemList.Remove(TempTrimGreenItemList[Gindex]);
                    GreenItemCountList.Remove(GreenItemCountList[GCount]);
                }
                for (var r = 0; r < RedCount; r++)
                {
                    int Rindex = random.Next(0, TempTrimRedItemList.Count);
                    int RCount = random.Next(0, RedItemCountList.Count);
                    if (TempTrimRedItemList.Count == 0)
                    {
                        TempTrimRedItemList.AddRange(SimulacrumRedItemList);
                    }
                    SimulacrumInventory.GiveItem(TempTrimRedItemList[Rindex].itemIndex, RedItemCountList[RCount]);
                    TempTrimRedItemList.Remove(TempTrimRedItemList[Rindex]);
                    RedItemCountList.Remove(RedItemCountList[RCount]);
                }
                for (var r = 0; r < BlueCount; r++)
                {
                    int Rindex = random.Next(0, TempTrimBlueItemList.Count);
                    int RCount = random.Next(0, BlueItemCountList.Count);
                    if (TempTrimBlueItemList.Count == 0)
                    {
                        TempTrimBlueItemList.AddRange(SimulacrumBlueItemList);
                    }
                    SimulacrumInventory.GiveItem(TempTrimBlueItemList[Rindex].itemIndex, BlueItemCountList[RCount]);
                    TempTrimBlueItemList.Remove(TempTrimBlueItemList[Rindex]);
                    BlueItemCountList.Remove(BlueItemCountList[RCount]);
                }

                Debug.Log($"Rerolled Simulacrum's inventory");
            }
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.MonsterTeamGainsItems))
            {
                Inventory MonsterTeamGainItemInventory = RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.monsterTeamInventory;
                int[] invoutput = new int[MonsterTeamGainItemInventory.itemStacks.Length];
                MonsterTeamGainItemInventory.itemStacks.CopyTo(invoutput, 0);


                List<ItemTag> BannedItemTagsSimu = dtMonsterTeamTier2Item.bannedItemTags.ToList();

                List<PickupIndex> MonsterWhiteItemList = new List<PickupIndex>();
                List<PickupIndex> MonsterGreenItemList = new List<PickupIndex>();
                List<PickupIndex> MonsterRedItemList = new List<PickupIndex>();
                List<PickupIndex> MonsterBlueItemList = new List<PickupIndex>();
                MonsterWhiteItemList.AddRange(Run.instance.availableTier1DropList);
                MonsterGreenItemList.AddRange(Run.instance.availableTier2DropList);
                MonsterRedItemList.AddRange(Run.instance.availableTier3DropList);
                MonsterBlueItemList.AddRange(Run.instance.availableLunarItemDropList);

                int WhiteCount = 0;
                int GreenCount = 0;
                int RedCount = 0;
                int BlueCount = 0;

                for (int i = MonsterWhiteItemList.Count - 1; i >= 0; i--)
                {
                    ItemDef temp = ItemCatalog.GetItemDef(MonsterWhiteItemList[i].itemIndex);
                    for (int tags = 0; tags < temp.tags.Length; tags++)
                    {
                        //Debug.LogWarning(temp + " " + temp.tags[tags]);
                        if (BannedItemTagsSimu.Contains(temp.tags[tags]))
                        {
                            //Debug.LogWarning(temp + " " + temp.tags[tags] + " "+ MonsterWhiteItemList.Contains(MonsterWhiteItemList[i]));
                            MonsterWhiteItemList.Remove(MonsterWhiteItemList[i]);
                            tags = 100;
                        }
                    }
                }
                for (int i = MonsterGreenItemList.Count - 1; i >= 0; i--)
                {
                    ItemDef temp = ItemCatalog.GetItemDef(MonsterGreenItemList[i].itemIndex);
                    for (int tags = 0; tags < temp.tags.Length; tags++)
                    {
                        if (BannedItemTagsSimu.Contains(temp.tags[tags]))
                        {
                            MonsterGreenItemList.Remove(MonsterGreenItemList[i]);
                            tags = 100;
                        }
                    }
                }
                for (int i = MonsterRedItemList.Count - 1; i >= 0; i--)
                {
                    ItemDef temp = ItemCatalog.GetItemDef(MonsterRedItemList[i].itemIndex);
                    for (int tags = 0; tags < temp.tags.Length; tags++)
                    {
                        if (BannedItemTagsSimu.Contains(temp.tags[tags]))
                        {
                            MonsterRedItemList.Remove(MonsterRedItemList[i]);
                            tags = 100;
                        }
                    }
                }
                for (int i = MonsterBlueItemList.Count - 1; i >= 0; i--)
                {
                    ItemDef temp = ItemCatalog.GetItemDef(MonsterBlueItemList[i].itemIndex);
                    for (int tags = 0; tags < temp.tags.Length; tags++)
                    {
                        if (BannedItemTagsSimu.Contains(temp.tags[tags]))
                        {
                            MonsterBlueItemList.Remove(MonsterBlueItemList[i]);
                            tags = 100;
                        }
                    }
                }

                List<PickupIndex> TempTrimWhiteItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimGreenItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimRedItemList = new List<PickupIndex>();
                List<PickupIndex> TempTrimBlueItemList = new List<PickupIndex>();

                TempTrimWhiteItemList.AddRange(MonsterWhiteItemList);
                TempTrimGreenItemList.AddRange(MonsterGreenItemList);
                TempTrimRedItemList.AddRange(MonsterRedItemList);
                TempTrimBlueItemList.AddRange(MonsterBlueItemList);

                List<int> WhiteItemCountList = new List<int>();
                List<int> GreenItemCountList = new List<int>();
                List<int> RedItemCountList = new List<int>();
                List<int> BlueItemCountList = new List<int>();

                for (var i = 0; i < invoutput.Length; i++)
                {
                    if (invoutput[i] > 0)
                    {
                        ItemDef tempitemdef = ItemCatalog.GetItemDef((ItemIndex)i);
                        switch (tempitemdef.tier)
                        {
                            case ItemTier.Tier1:
                                WhiteCount++;
                                MonsterTeamGainItemInventory.RemoveItem((ItemIndex)i, invoutput[i]);
                                WhiteItemCountList.Add(invoutput[i]);
                                break;
                            case ItemTier.Tier2:
                                GreenCount++;
                                MonsterTeamGainItemInventory.RemoveItem((ItemIndex)i, invoutput[i]);
                                GreenItemCountList.Add(invoutput[i]);
                                break;
                            case ItemTier.Tier3:
                                RedCount++;
                                MonsterTeamGainItemInventory.RemoveItem((ItemIndex)i, invoutput[i]);
                                RedItemCountList.Add(invoutput[i]);
                                break;
                            case ItemTier.Lunar:
                                BlueCount++;
                                MonsterTeamGainItemInventory.RemoveItem((ItemIndex)i, invoutput[i]);
                                BlueItemCountList.Add(invoutput[i]);
                                break;
                        }

                    }
                }


                for (var w = 0; w < WhiteCount; w++)
                {
                    int Windex = random.Next(0, TempTrimWhiteItemList.Count);
                    int WCount = random.Next(0, WhiteItemCountList.Count);
                    if (TempTrimWhiteItemList.Count == 0)
                    {
                        TempTrimWhiteItemList.AddRange(MonsterWhiteItemList);
                    }
                    MonsterTeamGainItemInventory.GiveItem(TempTrimWhiteItemList[Windex].itemIndex, WhiteItemCountList[WCount]);
                    TempTrimWhiteItemList.Remove(TempTrimWhiteItemList[Windex]);
                    WhiteItemCountList.Remove(WhiteItemCountList[WCount]);

                }
                for (var g = 0; g < GreenCount; g++)
                {
                    int Gindex = random.Next(0, TempTrimGreenItemList.Count);
                    int GCount = random.Next(0, GreenItemCountList.Count);
                    if (TempTrimGreenItemList.Count == 0)
                    {
                        TempTrimGreenItemList.AddRange(MonsterGreenItemList);
                    }
                    MonsterTeamGainItemInventory.GiveItem(TempTrimGreenItemList[Gindex].itemIndex, GreenItemCountList[GCount]);
                    TempTrimGreenItemList.Remove(TempTrimGreenItemList[Gindex]);
                    GreenItemCountList.Remove(GreenItemCountList[GCount]);
                }
                for (var r = 0; r < RedCount; r++)
                {
                    int Rindex = random.Next(0, TempTrimRedItemList.Count);
                    int RCount = random.Next(0, RedItemCountList.Count);
                    if (TempTrimRedItemList.Count == 0)
                    {
                        TempTrimRedItemList.AddRange(MonsterRedItemList);
                    }
                    MonsterTeamGainItemInventory.GiveItem(TempTrimRedItemList[Rindex].itemIndex, RedItemCountList[RCount]);
                    TempTrimRedItemList.Remove(TempTrimRedItemList[Rindex]);
                    RedItemCountList.Remove(RedItemCountList[RCount]);
                }
                for (var r = 0; r < BlueCount; r++)
                {
                    int Rindex = random.Next(0, TempTrimBlueItemList.Count);
                    int RCount = random.Next(0, BlueItemCountList.Count);
                    if (TempTrimBlueItemList.Count == 0)
                    {
                        TempTrimBlueItemList.AddRange(MonsterBlueItemList);
                    }
                    MonsterTeamGainItemInventory.GiveItem(TempTrimBlueItemList[Rindex].itemIndex, BlueItemCountList[RCount]);
                    TempTrimBlueItemList.Remove(TempTrimBlueItemList[Rindex]);
                    BlueItemCountList.Remove(BlueItemCountList[RCount]);
                }

                Debug.Log($"Rerolled Monster Team's inventory");
            }
        }

    }
}