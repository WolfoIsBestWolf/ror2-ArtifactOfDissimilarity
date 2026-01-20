using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace ArtifactDissimilarity.Aritfacts
{
    public partial class Dissimilarity
    {
        public static DirectorCardCategorySelection mixInteractablesCards = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static DirectorCardCategorySelection TrimmedmixInteractablesCards = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();


        public static void Start()
        {
            mixInteractablesCards.name = "dccsMixInteractableMaster";
            TrimmedmixInteractablesCards.name = "dccsMixInteractableTrimmed";
            DCCSmaker();
        }

        public static void OnArtifactDisable()
        {
            On.RoR2.ClassicStageInfo.RebuildCards -= ForceMixInteractables_ClassicStageInfo_RebuildCards;
            SceneDirector.onPrePopulateSceneServer -= LunarTeleporterEvery5Stages;
        }
        public static void OnArtifactEnable()
        {
            On.RoR2.ClassicStageInfo.RebuildCards += ForceMixInteractables_ClassicStageInfo_RebuildCards;
            SceneDirector.onPrePopulateSceneServer += LunarTeleporterEvery5Stages;
            DissimWander_LunarSeer.PopulateSeerList();
        }

        private static void LunarTeleporterEvery5Stages(SceneDirector obj)
        {
            //if (Run.instance.NetworkstageClearCount % Run.stagesPerLoop == Run.stagesPerLoop - 1)
            if (Run.instance.NetworkstageClearCount % 5 == 5 - 1)
            {
                obj.teleporterSpawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarTeleporter");
            }
        }

        private static void ForceMixInteractables_ClassicStageInfo_RebuildCards(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, ClassicStageInfo self, DirectorCardCategorySelection forcedMonsterCategory, DirectorCardCategorySelection forcedInteractableCategory)
        {
            MakeTrimmerMixInteractablesDCCS();
            
            orig(self, forcedInteractableCategory, TrimmedmixInteractablesCards);
        }





        public static void ModSupport()
        {
            //0 Chests
            //1 Barrels
            //2 Shrines
            //3 Drones
            //4 Misc
            //5 Rare
            //6 Duplicator


            InteractableSpawnCard[] ISCList = UnityEngine.Object.FindObjectsOfType(typeof(InteractableSpawnCard)) as InteractableSpawnCard[];
            for (var i = 0; i < ISCList.Length; i++)
            {
                //Debug.LogWarning(ISCList[i]);
                switch (ISCList[i].name)
                {
                    case "MysticsItems_iscShrineLegendary":
                        DirectorCard AD_MysticsItems_ShrineLegendary = new DirectorCard
                        {
                            spawnCard = ISCList[i],
                            selectionWeight = 50,
                        };
                        break;
                    case "msidcCloneDrone":
                        DirectorCard msidcCloneDrone = new DirectorCard
                        {
                            spawnCard = ISCList[i],
                            selectionWeight = 7,
                        };
                        mixInteractablesCards.AddCard(3, msidcCloneDrone);  //30
                        break;
                    case "msidcShockDrone":
                        DirectorCard msidcShockDrone = new DirectorCard
                        {
                            spawnCard = ISCList[i],
                            selectionWeight = 14,
                        };
                        mixInteractablesCards.AddCard(3, msidcShockDrone);  //30
                        break;
                    case "iscCloakedShrine":
                        DirectorCard iscCloakedShrine = new DirectorCard
                        {
                            spawnCard = ISCList[i],
                            selectionWeight = 8,
                        };
                        mixInteractablesCards.AddCard(5, iscCloakedShrine);  //30
                        break;
                    case "iscAegisShrine":
                        DirectorCard iscAegisShrine = new DirectorCard
                        {
                            spawnCard = ISCList[i],
                            selectionWeight = 8,
                        };
                        mixInteractablesCards.AddCard(5, iscAegisShrine);  //30
                        break;
                }
            }


        }


        public static void DCCSmaker()
        {


            //Spawn Cards
            InteractableSpawnCard SoupWhiteGreenISC = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            SoupWhiteGreenISC.name = "iscSoupWhiteGreen";
            SoupWhiteGreenISC.prefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, WhiteToGreen");
            SoupWhiteGreenISC.sendOverNetwork = true;
            SoupWhiteGreenISC.hullSize = HullClassification.Golem;
            SoupWhiteGreenISC.nodeGraphType = MapNodeGroup.GraphType.Ground;
            SoupWhiteGreenISC.requiredFlags = NodeFlags.None;
            SoupWhiteGreenISC.forbiddenFlags = NodeFlags.NoChestSpawn;
            SoupWhiteGreenISC.directorCreditCost = 3;
            SoupWhiteGreenISC.occupyPosition = true;
            SoupWhiteGreenISC.eliteRules = SpawnCard.EliteRules.Default;
            SoupWhiteGreenISC.orientToFloor = true;
            SoupWhiteGreenISC.slightlyRandomizeOrientation = false;
            SoupWhiteGreenISC.skipSpawnWhenSacrificeArtifactEnabled = false;

            InteractableSpawnCard SoupGreenRedISC = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            SoupGreenRedISC.name = "iscSoupGreenRed";
            SoupGreenRedISC.prefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, GreenToRed Variant");
            SoupGreenRedISC.sendOverNetwork = true;
            SoupGreenRedISC.hullSize = HullClassification.Golem;
            SoupGreenRedISC.nodeGraphType = MapNodeGroup.GraphType.Ground;
            SoupGreenRedISC.requiredFlags = NodeFlags.None;
            SoupGreenRedISC.forbiddenFlags = NodeFlags.NoChestSpawn;
            SoupGreenRedISC.directorCreditCost = 2;
            SoupGreenRedISC.occupyPosition = true;
            SoupGreenRedISC.orientToFloor = true;


            InteractableSpawnCard SoupRedWhiteISC = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            SoupRedWhiteISC.name = "iscSoupRedWhite";
            SoupRedWhiteISC.prefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, RedToWhite Variant");
            SoupRedWhiteISC.sendOverNetwork = true;
            SoupRedWhiteISC.hullSize = HullClassification.Golem;
            SoupRedWhiteISC.nodeGraphType = MapNodeGroup.GraphType.Ground;
            SoupRedWhiteISC.requiredFlags = NodeFlags.None;
            SoupRedWhiteISC.forbiddenFlags = NodeFlags.NoChestSpawn;
            SoupRedWhiteISC.directorCreditCost = 1;
            SoupRedWhiteISC.occupyPosition = true;
            SoupRedWhiteISC.orientToFloor = true;


            GameObject LunarSeerObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/SeerStation");
            SeerStationController LunarSeerTele1 = LunarSeerObject.GetComponent<RoR2.SeerStationController>();
            LunarSeerTele1.fallBackToFirstActiveExitController = true;


            InteractableSpawnCard LunarSeerISC = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            LunarSeerISC.name = "iscLunarSeer";
            LunarSeerISC.prefab = LunarSeerObject;
            LunarSeerISC.sendOverNetwork = true;
            LunarSeerISC.hullSize = HullClassification.Human;
            LunarSeerISC.nodeGraphType = MapNodeGroup.GraphType.Ground;
            LunarSeerISC.requiredFlags = NodeFlags.None;
            LunarSeerISC.forbiddenFlags = NodeFlags.NoChestSpawn;
            LunarSeerISC.directorCreditCost = 1;
            LunarSeerISC.occupyPosition = true;
            LunarSeerISC.orientToFloor = true;



            #region CHEST
            DirectorCard ADChest1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscChest1"),
                selectionWeight = 200,
            };
            DirectorCard ADChest2 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscChest2"),
                selectionWeight = 40,
            };
            DirectorCard ADEquipmentBarrel = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscEquipmentBarrel"),
                selectionWeight = 20,
            };
            DirectorCard ADTripleShop = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscTripleShop"),
                selectionWeight = 80,
            };
            DirectorCard ADLunarChest = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarChest"),
                selectionWeight = 20,
            };
            DirectorCard ADCategoryChestDamage = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestDamage"),
                selectionWeight = 40,
            };
            DirectorCard ADCategoryChestHealing = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestHealing"),
                selectionWeight = 40,
            };
            DirectorCard ADCategoryChestUtility = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestUtility"),
                selectionWeight = 40,
            };
            DirectorCard iscTripleShopLarge = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscTripleShopLarge"),
                selectionWeight = 35, //40 late, 10 early
            };
            DirectorCard iscCasinoChest = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCasinoChest"),
                selectionWeight = 30,
            };
            DirectorCard iscTripleShopEquipment = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscTripleShopEquipment"),
                selectionWeight = 10,
            };

            //DLC Chests
            DirectorCard ADCategoryChest2Damage = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/CategoryChest2/iscCategoryChest2Damage.asset").WaitForCompletion(),
                selectionWeight = 20,
            };
            DirectorCard ADCategoryChest2Healing = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/CategoryChest2/iscCategoryChest2Healing.asset").WaitForCompletion(),
                selectionWeight = 20,
            };
            DirectorCard ADCategoryChest2Utility = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/CategoryChest2/iscCategoryChest2Utility.asset").WaitForCompletion(),
                selectionWeight = 20,
            };
            DirectorCard ADVoidChest = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidChest/iscVoidChest.asset").WaitForCompletion(),
                selectionWeight = 30,
            };
            DirectorCard ADVoidTriple = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidTriple/iscVoidTriple.asset").WaitForCompletion(),
                selectionWeight = 15,
            };
            DirectorCard iscTemporaryItemsShop = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("6df786822d3105e4e820c69e1ef94d16"),
                selectionWeight = 15, //5 to 20
            };
            #endregion
            #region Barrel
            //ChestsEnd
            //Barrel
            DirectorCard ADBarrel1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBarrel1"),
                selectionWeight = 100,
            };
            InteractableSpawnCard iscVoidCoinBarrel = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<InteractableSpawnCard>(key: "RoR2/DLC1/VoidCoinBarrel/iscVoidCoinBarrel.asset").WaitForCompletion());
            iscVoidCoinBarrel.name = "iscVoidCoinBarrelLowPrice";
            iscVoidCoinBarrel.directorCreditCost = 2;
            DirectorCard ADBarrelVoidCoin = new DirectorCard
            {
                spawnCard = iscVoidCoinBarrel,
                selectionWeight = 30,
            };

            #endregion
            #region Shrines
            DirectorCard iscShrineCombat = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineCombat"),
                selectionWeight = 250,
            };
            DirectorCard iscShrineBoss = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineBoss"),
                selectionWeight = 200,
            };
            DirectorCard iscShrineChance = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineChance"),
                selectionWeight = 400,
            };
            DirectorCard iscShrineBlood = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineBlood"),
                selectionWeight = 200,
            };
            DirectorCard iscShrineHealing = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineHealing"),
                selectionWeight = 200,
            };
            DirectorCard iscShrineRestack = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineRestack"),
                selectionWeight = 50,
            };
            DirectorCard iscShrineCleanse = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineCleanse"),
                selectionWeight = 100,
                minimumStageCompletions = 1
            };
            DirectorCard iscShrineColossusAccess = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/iscShrineColossusAccess.asset").WaitForCompletion(),
                selectionWeight = 50,
                minimumStageCompletions = 1
            };
            #endregion
            #region Drones
            DirectorCard iscBrokenDroneGunner = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone1"),
                selectionWeight = 70,
            };
            DirectorCard iscBrokenDroneHealing = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone2"),
                selectionWeight = 70,
            };
            DirectorCard iscBrokenTurret1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenTurret1"),
                selectionWeight = 35,
            };
            DirectorCard iscBrokenEmergencyDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenEmergencyDrone"),
                selectionWeight = 25,
            };
            DirectorCard iscBrokenEquipmentDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenEquipmentDrone"),
                selectionWeight = 25,
            };
            DirectorCard iscBrokenFlameDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenFlameDrone"),
                selectionWeight = 25,
            };
            DirectorCard iscBrokenMissileDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMissileDrone"),
                selectionWeight = 60,
            };
            DirectorCard ADBrokenMegaDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMegaDrone"),
                selectionWeight = 10,
                minimumStageCompletions = 1
            };


            DirectorCard iscBrokenHaulerDrone = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("d304fff1f19d4184bb1f9444df3c0837"),
                selectionWeight = 40,
            };
            DirectorCard iscBrokenJunkDrone = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("d8aad1d9c0616c644869900039f7e3f3"),
                selectionWeight = 25,
            };

            DirectorCard iscBrokenJailerDrone = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("4e0d52fe3545f474b9076987b6ac92ec"),
                selectionWeight = 25,
            };
            DirectorCard iscBrokenRechargeDrone = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("5b6dafbe8f6447e49a151e62961f9f77"),
                selectionWeight = 30,
            };
            DirectorCard iscBrokenCleanupDrone = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("f92a522e57b907d4b8585b495e706636"),
                selectionWeight = 30,
            };
            DirectorCard iscBrokenCopycatDrone = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("f6970c1e4b273ea4ca332eb714b8801d"),
                selectionWeight = 15,
                minimumStageCompletions = 1
            };
            DirectorCard iscBrokenBombardmentDrone = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("384bcd7226702fd45a431a2795ff3d01"),
                selectionWeight = 15,
                minimumStageCompletions = 0
            };

            DirectorCard iscTripleDroneShop = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("5a86990b032424e48b4b8456f7d684c9"),
                selectionWeight = 50,
            };

            #endregion
            #region MISC

            DirectorCard iscShrineHalcyonite = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/iscShrineHalcyonite.asset").WaitForCompletion(),
                selectionWeight = 23,
                minimumStageCompletions = 1
            };
            DirectorCard iscVoidCamp = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidCamp/iscVoidCamp.asset").WaitForCompletion(),
                selectionWeight = 25,
                minimumStageCompletions = 1
            };
            #endregion
            #region Rare
            DirectorCard ADGoldChest = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscGoldChest"),
                selectionWeight = 6,
                minimumStageCompletions = 2
            };
            DirectorCard ADShrineGoldshoresAccess = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineGoldshoresAccess"),
                selectionWeight = 5,
                minimumStageCompletions = 1
            };
            DirectorCard ADLunarSeer = new DirectorCard
            {
                spawnCard = LunarSeerISC,
                selectionWeight = 17,
            };
            DirectorCard ADChest1Stealthed = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscChest1Stealthed"),
                selectionWeight = 13,
            };
            DirectorCard ADVoidSuppressor = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidSuppressor/iscVoidSuppressor.asset").WaitForCompletion(),
                selectionWeight = 23,
            };
            #endregion
            #region Duplicators
            DirectorCard ADDuplicator = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscDuplicator"),
                selectionWeight = 300,
            };
            DirectorCard ADDuplicatorLarge = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscDuplicatorLarge"),
                selectionWeight = 60,
            };
            DirectorCard ADDuplicatorMilitary = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscDuplicatorMilitary"),
                selectionWeight = 20,
                minimumStageCompletions = 2
            };
            DirectorCard iscDuplicatorWild = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscDuplicatorWild"),
                selectionWeight = 20,
                minimumStageCompletions = 1
            };
            DirectorCard iscScrapper = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscScrapper"),
                selectionWeight = 120,
            };
            DirectorCard ADSoupWhiteGreen = new DirectorCard
            {
                spawnCard = SoupWhiteGreenISC,
                selectionWeight = 70,
            };
            DirectorCard ADSoupGreenRed = new DirectorCard
            {
                spawnCard = SoupGreenRedISC,
                selectionWeight = 50,
                minimumStageCompletions = 2
            };
            DirectorCard ADSoupRedWhite = new DirectorCard
            {
                spawnCard = SoupRedWhiteISC,
                selectionWeight = 30,
                minimumStageCompletions = 2
            };
            DirectorCard iscDroneCombinerStation = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("2eaec01927ea16245822dcb50080cba3"),
                selectionWeight = 40,
                minimumStageCompletions = 1
            };
            DirectorCard iscDroneScrapper = new DirectorCard
            {
                spawnCardReference = new AssetReferenceT<SpawnCard>("d7e78d150bd132744934165e6471f5f6"),
                selectionWeight = 80,
            };
            #endregion


            //0 Chest
            //1 Barrel
            //2 Shrine
            //3 Drones
            //4 MiscDLC
            //5 Rare
            //6 Duplicate

            mixInteractablesCards.AddCategory("Chests", 45); //0
            mixInteractablesCards.AddCategory("Barrels", 10); //1
            mixInteractablesCards.AddCategory("Shrines", 10); //2
            mixInteractablesCards.AddCategory("Drones", 14); //3
            mixInteractablesCards.AddCategory("Misc", 1.5f); //4
            mixInteractablesCards.AddCategory("Rare", 0.5f); //5
            mixInteractablesCards.AddCategory("Duplicator", 8f); //6

            //Cut to 6?
            mixInteractablesCards.AddCard(0, ADChest1);  //15
            mixInteractablesCards.AddCard(0, ADChest1);  //15
            mixInteractablesCards.AddCard(0, ADChest2);  //30
            mixInteractablesCards.AddCard(0, ADChest2);  //30
            mixInteractablesCards.AddCard(0, ADCategoryChestDamage);  //15
            mixInteractablesCards.AddCard(0, ADCategoryChestHealing);  //15
            mixInteractablesCards.AddCard(0, ADCategoryChestUtility);  //15
            mixInteractablesCards.AddCard(0, ADEquipmentBarrel);  //1
            mixInteractablesCards.AddCard(0, ADLunarChest);  //25
            mixInteractablesCards.AddCard(0, ADTripleShop);  //20
            mixInteractablesCards.AddCard(0, iscTripleShopLarge);  //40
            mixInteractablesCards.AddCard(0, iscTripleShopEquipment);  //2
            mixInteractablesCards.AddCard(0, iscCasinoChest);  //20
            //mixInteractablesCards.AddCard(0, ADChest1Stealthed);  //10

            mixInteractablesCards.AddCard(0, ADCategoryChest2Damage);  //20
            mixInteractablesCards.AddCard(0, ADCategoryChest2Healing);  //20
            mixInteractablesCards.AddCard(0, ADCategoryChest2Utility);  //20
            mixInteractablesCards.AddCard(0, ADVoidChest);  //20
            mixInteractablesCards.AddCard(0, ADVoidTriple);  //20
            mixInteractablesCards.AddCard(0, iscTemporaryItemsShop);  //20

            //Singular Barrel
            mixInteractablesCards.AddCard(1, ADBarrel1);  //1     
            mixInteractablesCards.AddCard(1, ADBarrel1);  //1     
            mixInteractablesCards.AddCard(1, ADBarrel1);  //1     
            mixInteractablesCards.AddCard(1, ADBarrelVoidCoin);  //1   

            //Cut to 3?
            mixInteractablesCards.AddCard(2, iscShrineBlood);  //20
            mixInteractablesCards.AddCard(2, iscShrineBoss);  //20
            mixInteractablesCards.AddCard(2, iscShrineBoss);  //20
            mixInteractablesCards.AddCard(2, iscShrineChance);  //20
            mixInteractablesCards.AddCard(2, iscShrineCleanse);  //5
            mixInteractablesCards.AddCard(2, iscShrineCombat);  //20
            mixInteractablesCards.AddCard(2, iscShrineHealing);  //15
            mixInteractablesCards.AddCard(2, iscShrineRestack);  //30
            mixInteractablesCards.AddCard(2, iscShrineColossusAccess);  //30

            //Cut to 3?
            mixInteractablesCards.AddCard(3, iscBrokenDroneGunner);  //15
            mixInteractablesCards.AddCard(3, iscBrokenDroneHealing);  //15
            mixInteractablesCards.AddCard(3, iscBrokenEmergencyDrone);  //30
            mixInteractablesCards.AddCard(3, iscBrokenEquipmentDrone);  //15
            mixInteractablesCards.AddCard(3, iscBrokenFlameDrone);  //30
            mixInteractablesCards.AddCard(3, ADBrokenMegaDrone);  //40
            mixInteractablesCards.AddCard(3, iscBrokenMissileDrone);  //20
            mixInteractablesCards.AddCard(3, iscBrokenTurret1);  //10

            mixInteractablesCards.AddCard(3, iscBrokenJunkDrone);
            mixInteractablesCards.AddCard(3, iscBrokenHaulerDrone);
            mixInteractablesCards.AddCard(3, iscBrokenJailerDrone);
            mixInteractablesCards.AddCard(3, iscBrokenCleanupDrone);
            mixInteractablesCards.AddCard(3, iscBrokenRechargeDrone);
            mixInteractablesCards.AddCard(3, iscBrokenBombardmentDrone);
            mixInteractablesCards.AddCard(3, iscBrokenCopycatDrone);
            mixInteractablesCards.AddCard(3, iscTripleDroneShop);


            //Cut to 1?
            mixInteractablesCards.AddCard(4, iscVoidCamp);  //10
            mixInteractablesCards.AddCard(4, iscShrineHalcyonite);  //10
            mixInteractablesCards.AddCard(4, iscDroneCombinerStation);  //10
            //
            mixInteractablesCards.AddCard(5, ADChest1Stealthed);  //10
            mixInteractablesCards.AddCard(5, ADGoldChest);  //50
            mixInteractablesCards.AddCard(5, ADShrineGoldshoresAccess);  //1
            //mixInteractablesCards.AddCard(5, ADVoidSuppressor);  //1
            mixInteractablesCards.AddCard(5, ADLunarSeer);  //1


            //Cut to 4?
            mixInteractablesCards.AddCard(6, iscScrapper);  //5
            mixInteractablesCards.AddCard(6, iscScrapper);  //5
            mixInteractablesCards.AddCard(6, iscDroneScrapper);  //5
            mixInteractablesCards.AddCard(6, ADDuplicator);  //5
            mixInteractablesCards.AddCard(6, ADDuplicatorLarge);  //10
            mixInteractablesCards.AddCard(6, ADDuplicatorMilitary);  //15
            mixInteractablesCards.AddCard(6, iscDuplicatorWild);  //10
            mixInteractablesCards.AddCard(6, ADSoupWhiteGreen);  //10
            mixInteractablesCards.AddCard(6, ADSoupGreenRed);  //10
            mixInteractablesCards.AddCard(6, ADSoupRedWhite);  //5

            //


        }



        public static void MakeTrimmerMixInteractablesDCCS()
        {
            if (!Run.instance)
            {
                return;
            }



            TrimmedmixInteractablesCards.Clear();
            TrimmedmixInteractablesCards.CopyFrom(mixInteractablesCards);

            int drone = Random.RandomRangeInt(1, 14);//Drones range from 3.5 to 18
            if (drone <= 5)
            {
                TrimmedmixInteractablesCards.categories[3].selectionWeight = 7f;
            }
            else if (drone <= 10)
            {
                TrimmedmixInteractablesCards.categories[3].selectionWeight = 14f;
            }
            else if (drone == 11)
            {
                TrimmedmixInteractablesCards.categories[3].selectionWeight = 18f;
            }
            else
            {
                TrimmedmixInteractablesCards.categories[3].selectionWeight = 3.5f;
            }

            //Chests
            //Barrels + Void Camp
            TrimmedmixInteractablesCards.categories[2].selectionWeight = Random.RandomRangeInt(7, 12); //Shrines range from 7 to 10 in vanilla
                                                                                             //Misc Nothing ig
                                                                                             //Rare
                                                                                             //TrimmedmixInteractablesCards.categories[6].selectionWeight = random.Next(7, 10); //Duplicator is 8 in vanilla

            Filters.Mix_ApplyCardRemovingFilters(TrimmedmixInteractablesCards);
            Filters.MixInteractables_Trimmer_Direct(TrimmedmixInteractablesCards);
            Filters.ApplySandSnow(TrimmedmixInteractablesCards);


            if (Run.instance && Run.instance.stageClearCount == 0)
            {
                TrimmedmixInteractablesCards.AddCard(4, new DirectorCard
                {
                    spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/iscShrineHalcyoniteTier1.asset").WaitForCompletion(),
                    selectionWeight = 200,
                });
            }

        }


    }
}