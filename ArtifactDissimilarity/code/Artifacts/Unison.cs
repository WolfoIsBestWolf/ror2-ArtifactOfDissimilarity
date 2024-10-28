using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ArtifactDissimilarity
{
    public class Unison
    {
        public static readonly System.Random random = new System.Random();
        public static PickupIndex[] pickupsArrayOverride = new PickupIndex[0];
        public static PickupIndex[] pickupsArrayNoRepeats = new PickupIndex[0];
        public static PickupIndex[] pickupsArrayNoRepeats2 = new PickupIndex[0];

        public static void OnArtifactDisable()
        {
            On.RoR2.PickupDropTable.GenerateDrop -= Unison.PickupDropTable_GenerateDrop;
            On.RoR2.PickupPickerController.SetOptionsServer -= Unison.PickupPickerController_SetOptionsServer;
            On.RoR2.ChestBehavior.ItemDrop -= Unison.ChestBehavior_ItemDrop;
            //
            On.RoR2.Run.Start -= Unison.Run_Start;
            On.RoR2.Run.AdvanceStage -= Unison.Run_AdvanceStage;
            //
            On.RoR2.SceneDirector.GenerateInteractableCardSelection -= MoreCreditsPrinters;
            On.EntityStates.InfiniteTowerSafeWard.Travelling.OnExit -= Unison.NewItemOnCrabTravel;
            On.EntityStates.ScavMonster.FindItem.OnEnter -= FixScavs;
            On.EntityStates.ScavMonster.FindItem.PickupIsNonBlacklistedItem -= FixScavs2;

            if (Run.instance && Run.instance.availableTier1DropList.Count == 1)
            {
                Run.instance.BuildDropTable();
            }

            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicator").maxSpawnsPerStage = -1;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicatorLarge").maxSpawnsPerStage = -1;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicatorWild").maxSpawnsPerStage = -1;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicatorMilitary").maxSpawnsPerStage = -1;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineCleanse").maxSpawnsPerStage = -1;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscScrapper").maxSpawnsPerStage = -1;
        }

        public static void OnArtifactEnable()
        {
            On.RoR2.PickupDropTable.GenerateDrop += Unison.PickupDropTable_GenerateDrop;
            On.RoR2.PickupPickerController.SetOptionsServer += Unison.PickupPickerController_SetOptionsServer;
            On.RoR2.ChestBehavior.ItemDrop += Unison.ChestBehavior_ItemDrop;
            //Generate new
            On.RoR2.Run.Start += Unison.Run_Start;
            On.RoR2.Run.AdvanceStage += Unison.Run_AdvanceStage;
            //Extras
            On.RoR2.SceneDirector.GenerateInteractableCardSelection += MoreCreditsPrinters;
            On.EntityStates.InfiniteTowerSafeWard.Travelling.OnExit += Unison.NewItemOnCrabTravel;
            On.EntityStates.ScavMonster.FindItem.OnEnter += FixScavs;
            On.EntityStates.ScavMonster.FindItem.PickupIsNonBlacklistedItem += FixScavs2;
            //
            if (Run.instance && Run.instance.availableTier1DropList.Count > 1)
            {
                Unison.SetupItemOverrides();
            }
            int add = 1;
            if (Run.instance is InfiniteTowerRun)
            {
                add = 2;
            }
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicator").maxSpawnsPerStage = 2 *add;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicatorLarge").maxSpawnsPerStage = (int)(add*1.5f);
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicatorWild").maxSpawnsPerStage = 1;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscDuplicatorMilitary").maxSpawnsPerStage = 1;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscShrineCleanse").maxSpawnsPerStage = 2 * add;
            LegacyResourcesAPI.Load<InteractableSpawnCard>("SpawnCards/InteractableSpawnCard/iscScrapper").maxSpawnsPerStage = 2 * add;
        }

        private static WeightedSelection<DirectorCard> MoreCreditsPrinters(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, SceneDirector self)
        {
            if (ClassicStageInfo.instance && ClassicStageInfo.instance.interactableCategories)
            {
                if(self.interactableCredit > 0)
                {
                    self.interactableCredit += 25; //5+10+15+15
                }
                int Index = ClassicStageInfo.instance.interactableCategories.FindCategoryIndexByName("Duplicator");
                if (Index != -1)
                {
                    ClassicStageInfo.instance.interactableCategories.categories[Index].selectionWeight *= 2;
                }
                WeightedSelection<DirectorCard> temp = orig(self);
                if (Index != -1)
                {
                    ClassicStageInfo.instance.interactableCategories.categories[Index].selectionWeight /= 2;
                }
                return temp;
            }
            return orig(self);
        }


        //Tier1Chance 70 * 5
        //Tier2Chance 30 * 2
        //Tier3Chance 10 * 1
        private static void FixScavs(On.EntityStates.ScavMonster.FindItem.orig_OnEnter orig, EntityStates.ScavMonster.FindItem self)
        {
            orig(self);
            PickupIndex newPickup;
            int chance = random.Next(110);

            if (chance < 10)
            {
                self.itemsToGrant = 1;
                newPickup = self.characterBody.masterObject.GetComponent<ScavengerItemGranter>().stackRollDataList[2].dropTable.GenerateDrop(Run.instance.runRNG);
            }
            else if (chance < 40)
            {
                self.itemsToGrant = 2;
                newPickup = self.characterBody.masterObject.GetComponent<ScavengerItemGranter>().stackRollDataList[1].dropTable.GenerateDrop(Run.instance.runRNG);
            }
            else
            {
                self.itemsToGrant = 5;
                newPickup = self.characterBody.masterObject.GetComponent<ScavengerItemGranter>().stackRollDataList[0].dropTable.GenerateDrop(Run.instance.runRNG);
            }
            self.dropPickup = newPickup;
            Transform transform = self.FindModelChild("PickupDisplay");
            PickupDisplay pickupDisplay = transform.GetComponent<PickupDisplay>();
            pickupDisplay.SetPickupIndex(newPickup, false);
        }

        public static void NewItemOnCrabTravel(On.EntityStates.InfiniteTowerSafeWard.Travelling.orig_OnExit orig, EntityStates.InfiniteTowerSafeWard.Travelling self)
        {
            //Helps with IT or mid stage changes
            orig(self);
            //SetupItemOverrides();
            SetupNew1TierOnly(0, Run.instance.availableTier1DropList, LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier1"));
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = "ITREROLL_UNISON_WHITE",
            });

            //If the player for some reason gets like 5 greens in 5 waves they should probably get a different green
            int greenCount = RoR2.Util.GetItemCountForTeam(TeamIndex.Player, pickupsArrayOverride[1].pickupDef.itemIndex, false, true);
            if (greenCount > (Run.instance.participatingPlayerCount * 4))
            {
                SetupNew1TierOnly(1, Run.instance.availableTier2DropList, LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier2"));
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    //baseToken = "<style=cWorldEvent>[WARNING] Item amount exceeded, simulating new Unison <color=#77FF17>uncommon</color> item.</style>",
                    baseToken = "ITREROLL_UNISON_GREEN",
                });
            }
            //<color=#FF8000> 
        }

        public static void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            //Helps with IT or mid stage changes
            self.dropPickup = GetNewPickupIndex(self.dropPickup);
            orig(self);
        }

        public static void PickupPickerController_SetOptionsServer(On.RoR2.PickupPickerController.orig_SetOptionsServer orig, PickupPickerController self, PickupPickerController.Option[] newOptions)
        {
            if (!self.name.StartsWith("Scrapper"))
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Command))
                {
                    PickupPickerController.Option[] commandOptions = new PickupPickerController.Option[4];
                    int oCount = 0;
                    if (newOptions[0].pickupIndex.pickupDef.itemIndex != ItemIndex.None)
                    {
                        ItemTier itemTier = ItemCatalog.GetItemDef(newOptions[0].pickupIndex.pickupDef.itemIndex).tier;
                        //newOptions[i].pickupIndex = pickupsArrayOverride[((int)itemDef.tier)];
                        switch (itemTier)
                        {
                            case ItemTier.Tier1:
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier1];
                                if (random.Next(100) < 25)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                                }
                                if (random.Next(100) < 2)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier3];
                                }
                                break;
                            case ItemTier.Tier2:
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                                if (random.Next(100) < 25)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier3];
                                }
                                if (random.Next(100) < 18)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Boss];
                                }
                                break;
                            case ItemTier.Tier3:
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier3];
                                if (random.Next(100) < 100)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                                }
                                if (random.Next(100) < 25)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.Boss];
                                }
                                break;
                            case ItemTier.Boss:
                                commandOptions[0].pickupIndex = pickupsArrayOverride[(int)ItemTier.Boss];
                                commandOptions[1].pickupIndex = pickupsArrayOverride[(int)ItemTier.Tier2];
                                break;
                            case ItemTier.Lunar:
                                commandOptions[0].pickupIndex = pickupsArrayOverride[(int)ItemTier.Lunar];
                                if (random.Next(100) < 15)
                                {
                                    if (random.Next(100) < 2)
                                    {
                                        commandOptions[1].pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex);
                                    }
                                    else
                                    {
                                        commandOptions[1].pickupIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex);
                                    }
                                }
                                break;
                            case ItemTier.VoidTier1:
                                //6, 3, 1
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier1];
                                if (random.Next(100) < 40)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier2];
                                }
                                if (random.Next(100) < 12)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier3];
                                }
                                break;
                            case ItemTier.VoidTier2:
                                commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier2];
                                if (random.Next(100) < 80)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier1];
                                }
                                if (random.Next(100) < 30)
                                {
                                    oCount++;
                                    commandOptions[oCount].pickupIndex = pickupsArrayOverride[(int)ItemTier.VoidTier3];
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
                        }
                    }
                    if (newOptions[0].pickupIndex.pickupDef.equipmentIndex != EquipmentIndex.None)
                    {
                        if (newOptions[0].pickupIndex.pickupDef.isLunar)
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
                    for (int i = 0; i < commandOptions.Length; i++)
                    {
                        commandOptions[i].available = true;
                        if (commandOptions[i].pickupIndex.pickupDef.itemIndex == ItemIndex.None && commandOptions[i].pickupIndex.pickupDef.equipmentIndex == EquipmentIndex.None)
                        {
                            commandOptions = commandOptions.Remove(commandOptions[i]);
                            i--;
                        }
                    }
                    orig(self, commandOptions);
                    return;
                }
                else if (newOptions.Length > 1)
                {
                    for (int i = 0; i < newOptions.Length; i++)
                    {
                        newOptions[i].pickupIndex = GetNewPickupIndex(newOptions[i].pickupIndex);
                    }
                }

            }
            orig(self, newOptions);
        }

        public static PickupIndex PickupDropTable_GenerateDrop(On.RoR2.PickupDropTable.orig_GenerateDrop orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            PickupIndex original = orig(self, rng);

            if (original.pickupDef == null)
            {
                return original;
            }
            else if (self is DoppelgangerDropTable || self is ArenaMonsterItemDropTable)
            {
                //Explicit Drop Tables will stay overriden to overwrite boss drops.
                return original;
            }
            else if (self is BasicPickupDropTable) //Needs to be the lowest
            {
                if ((self as BasicPickupDropTable).bannedItemTags.Contains(ItemTag.AIBlacklist))
                {
                    return original;
                }
                else if (self.name.StartsWith("dtDupli"))
                {
                    return original;
                }
                else if (Stage.instance && Stage.instance.spawnedAnyPlayer && SceneInfo.instance.sceneDef.baseSceneName == "bazaar")
                {
                    return original;
                }
            }
            return GetNewPickupIndex(original);
        }
        //
        //
        public static void SetupItemOverrides()
        {
            if (pickupsArrayOverride.Length < 3)
            {
                pickupsArrayOverride = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 2];
                pickupsArrayNoRepeats = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 2];
                pickupsArrayNoRepeats2 = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 2];
                //Do not blacklist Bison Steak on stage 1 you know you want to
                Debug.LogWarning("does this code even still run");
            }

            Debug.Log("Artifact of Unison : New item set");

            Run.instance.BuildDropTable();

            Run.instance.availableBossDropList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex));
 
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
                pickupsArrayOverride[8] = Run.instance.availableVoidTier3DropList[random.Next(Run.instance.availableVoidTier3DropList.Count)];
            }
            if (Run.instance.availableVoidBossDropList.Count > 0)
            {
                pickupsArrayOverride[9] = Run.instance.availableVoidBossDropList[random.Next(Run.instance.availableVoidBossDropList.Count)];
            }

            //Equipment
            pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length] = NoRepeatsEquipment(ItemTierCatalog.itemTierDefs.Length, Run.instance.availableEquipmentDropList);
            pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1] = Run.instance.availableLunarEquipmentDropList[random.Next(Run.instance.availableLunarEquipmentDropList.Count)];

            ReplaceRunLists();

            if (WConfig.DebugPrint.Value)
            {
                Debug.Log("___");
                Debug.Log("Print Artifact of Unison results");
                for (int i = 0; i < pickupsArrayOverride.Length; i++)
                {
                    Debug.Log(pickupsArrayOverride[i].ToString());
                }
            }
        }

        public static void SetupNew1TierOnly(int Tier, List<PickupIndex> runList, BasicPickupDropTable dropTable)
        {
            Debug.Log("Artifact of Unison : New "+ (ItemTier)Tier+" item");
            for (int i = 0;i < dropTable.selector.Count; i++)
            {
                runList.Add(dropTable.selector.choices[i].value);
            }
            pickupsArrayOverride[Tier] = NoRepeatsItems(Tier, runList);
            runList.Clear();
            runList.Add(pickupsArrayOverride[Tier]);
            if (WConfig.DebugPrint.Value)
            {
                Debug.Log(pickupsArrayOverride[Tier].ToString());
            }
        }

 
        public static void ReplaceRunLists()
        {
            Run.instance.availableTier1DropList.Clear();
            Run.instance.availableTier1DropList.Add(pickupsArrayOverride[0]);

            Run.instance.availableTier2DropList.Clear();
            Run.instance.availableTier2DropList.Add(pickupsArrayOverride[1]);

            Run.instance.availableTier3DropList.Clear();
            Run.instance.availableTier3DropList.Add(pickupsArrayOverride[2]);

            Run.instance.availableLunarItemDropList.Clear();
            Run.instance.availableLunarItemDropList.Add(pickupsArrayOverride[3]);

            Run.instance.availableBossDropList.Clear();
            Run.instance.availableBossDropList.Add(pickupsArrayOverride[4]);

            Run.instance.availableVoidTier1DropList.Clear();
            Run.instance.availableVoidTier1DropList.Add(pickupsArrayOverride[6]);

            Run.instance.availableVoidTier2DropList.Clear();
            Run.instance.availableVoidTier2DropList.Add(pickupsArrayOverride[7]);

            Run.instance.availableVoidTier3DropList.Clear();
            Run.instance.availableVoidTier3DropList.Add(pickupsArrayOverride[8]);

            Run.instance.availableVoidBossDropList.Clear();
            Run.instance.availableVoidBossDropList.Add(pickupsArrayOverride[9]);

            Run.instance.availableEquipmentDropList.Clear();
            Run.instance.availableEquipmentDropList.Add(pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length]);

            Run.instance.availableLunarEquipmentDropList.Clear();
            Run.instance.availableLunarEquipmentDropList.Add(pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1]);

            //Run.instance.RefreshLunarCombinedDropList();
        }

        //
        //
        public static PickupIndex GetNewPickupIndex(PickupIndex original)
        {
            //Debug.Log(original.ToString());
            if (original == PickupIndex.none)
            {
                return original;
            }

            if (original.pickupDef.itemIndex != ItemIndex.None && original.pickupDef.itemIndex != RoR2Content.Items.ArtifactKey.itemIndex)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(original.pickupDef.itemIndex);
                if (!itemDef.tags.Contains(ItemTag.Scrap))
                {
                    original = pickupsArrayOverride[(int)itemDef.tier];
                }
            }
            else if (original.pickupDef.equipmentIndex != EquipmentIndex.None)
            {
                if (original.pickupDef.isLunar)
                {
                    original = pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 1];
                }
                else if (!original.pickupDef.isBoss)
                {
                    original = pickupsArrayOverride[ItemTierCatalog.itemTierDefs.Length + 0];
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
            else if(pickupList.Count == 1)
            {
                return pickupList[0];
            }

            ItemTag bannedTag = ItemTag.Any;
            ItemDef tempDef = ItemCatalog.GetItemDef(pickupsArrayOverride[Tier].pickupDef.itemIndex);
            if (tempDef && tempDef.tags.Length > 0)
            {
                bannedTag = tempDef.tags[0];
            }
            if (bannedTag == ItemTag.Any && Tier == 1)
            {
                tempDef = ItemCatalog.GetItemDef(pickupsArrayOverride[Tier-1].pickupDef.itemIndex);
                if (tempDef && tempDef.tags.Length > 0)
                {
                    bannedTag = tempDef.tags[0];
                }
            }
            if (WConfig.DebugPrint.Value)
            {
                Debug.Log((ItemTier)Tier+  "  BannedTag: " + bannedTag.ToString());
                Debug.Log((ItemTier)Tier + "  BannedItem2: " + pickupsArrayNoRepeats[Tier].ToString());
                Debug.Log((ItemTier)Tier + "  BannedItem3: " + pickupsArrayNoRepeats2[Tier].ToString());
            }
            PickupIndex tempDex;
            do
            {
                tempDex = pickupList[Run.instance.treasureRng.RangeInt(0, pickupList.Count)];
                tempDef = ItemCatalog.GetItemDef(tempDex.pickupDef.itemIndex);
                //Debug.Log(tempDef);
            }
            while (tempDef && tempDef.tags.Contains(bannedTag) || tempDex == pickupsArrayNoRepeats[Tier] || tempDex == pickupsArrayNoRepeats2[Tier]);
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
                Debug.Log("BannedEquip1: " + pickupsArrayOverride[Tier].ToString());
                Debug.Log("BannedEquip2: " + pickupsArrayNoRepeats[Tier].ToString());
                Debug.Log("BannedEquip3: " + pickupsArrayNoRepeats2[Tier].ToString());
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


        public static void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            pickupsArrayOverride = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 2];
            pickupsArrayNoRepeats = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 2];
            pickupsArrayNoRepeats2 = new PickupIndex[ItemTierCatalog.itemTierDefs.Length + 2];
            SetupItemOverrides();
        }

        public static void Run_AdvanceStage(On.RoR2.Run.orig_AdvanceStage orig, Run self, SceneDef nextScene)
        {
            orig(self, nextScene);
            SetupItemOverrides();
            //Debug.LogWarning("Run_AdvanceStage");
        }

        private static bool FixScavs2(On.EntityStates.ScavMonster.FindItem.orig_PickupIsNonBlacklistedItem orig, EntityStates.ScavMonster.FindItem self, PickupIndex pickupIndex)
        {
            //We could do some run build and then unbuild afterwards but idk that'd be dumb
            return true;
        }

    }
}