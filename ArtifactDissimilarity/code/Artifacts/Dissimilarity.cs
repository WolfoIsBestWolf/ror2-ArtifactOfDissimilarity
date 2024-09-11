using RoR2;
using RoR2.Navigation;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace ArtifactDissimilarity
{
    public class Dissimilarity
    {
        public static readonly System.Random random = new System.Random();

        public static bool DissimAdded = false;

        public static DirectorCardCategorySelection mixInteractablesCards = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static DirectorCardCategorySelection TrimmedmixInteractablesCards = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();


        public static void Start()
        {
            mixInteractablesCards.name = "dccsMixInteractableMaster";
            TrimmedmixInteractablesCards.name = "dccsMixInteractableTrimmed";
            DCCSmaker();
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
                        //mixInteractablesCards.AddCard(2, AD_MysticsItems_ShrineLegendary);  //
                        break;
                    case "iscTripleShopRed":
                        DirectorCard ADRedMultiShop = new DirectorCard
                        {
                            spawnCard = ISCList[i],
                            selectionWeight = 11,
                        };
                        mixInteractablesCards.AddCard(5, ADRedMultiShop);  //30
                        break;
                    case "midcDroneTable":
                        DirectorCard midcDroneTable = new DirectorCard
                        {
                            spawnCard = ISCList[i],
                            selectionWeight = 100,
                        };
                        mixInteractablesCards.AddCard(6, midcDroneTable);  //30
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

            DirectorCard ADScrapper = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscScrapper"),
                selectionWeight = 120,
            };
            DirectorCard ADScrapperB = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscScrapper"),
                selectionWeight = 180,
            };

            //Smth about not having 2 scrappers or some shit
            Kith.HelperSingleMixInteractable.CopyFrom(mixInteractablesCards);
            mixInteractablesCards.AddCard(6, ADScrapper);  //5
            Kith.HelperSingleMixInteractable.AddCard(1, ADScrapperB);  //1   
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
            SoupGreenRedISC.eliteRules = SpawnCard.EliteRules.Default;
            SoupGreenRedISC.orientToFloor = true;
            SoupGreenRedISC.slightlyRandomizeOrientation = false;
            SoupGreenRedISC.skipSpawnWhenSacrificeArtifactEnabled = false;

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
            SoupRedWhiteISC.eliteRules = SpawnCard.EliteRules.Default;
            SoupRedWhiteISC.orientToFloor = true;
            SoupRedWhiteISC.slightlyRandomizeOrientation = false;
            SoupRedWhiteISC.skipSpawnWhenSacrificeArtifactEnabled = false;

            GameObject LunarSeerObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/SeerStation");
            SeerStationController LunarSeerTele1 = LunarSeerObject.GetComponent<RoR2.SeerStationController>();
            LunarSeerTele1.fallBackToFirstActiveExitController = true;

            On.RoR2.SeerStationController.OnStartClient += Wander.SeerDestinationRandomizerDissimWander;

            On.RoR2.SeerStationController.OnTargetSceneChanged += (orig, self, sceneDef) =>
            {
                orig(self, sceneDef);

                //Debug.LogWarning(sceneDef);
                if (sceneDef != null)
                {
                    string temp = Language.GetString(SceneCatalog.GetSceneDef((SceneIndex)self.NetworktargetSceneDefIndex).nameToken);
                    temp = temp.Replace("Hidden Realm: ", "");
                    self.gameObject.GetComponent<PurchaseInteraction>().contextToken = (Language.GetString("BAZAAR_SEER_CONTEXT") + " of " + temp);
                    self.gameObject.GetComponent<PurchaseInteraction>().displayNameToken = (Language.GetString("BAZAAR_SEER_NAME") + " (" + temp + ")");
                }
            };

            On.RoR2.SeerStationController.SetRunNextStageToTarget += (orig, self) =>
            {
                orig(self);

                if (self.explicitTargetSceneExitController && self.explicitTargetSceneExitController.name.StartsWith("LunarTeleporter"))
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = "<style=cWorldEvent>The Primordial Teleporter aligns with your new dream..</style>"
                    });
                    if (Main.DreamPrimordialBool == false)
                    {
                        Main.DreamPrimordialBool = true;
                        On.RoR2.Language.GetString_string += Main.DreamPrimordial;
                    }
                }
            };


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
            LunarSeerISC.eliteRules = SpawnCard.EliteRules.Default;
            LunarSeerISC.orientToFloor = true;
            LunarSeerISC.slightlyRandomizeOrientation = false;
            LunarSeerISC.skipSpawnWhenSacrificeArtifactEnabled = false;


            //Chests
            DirectorCard ADChest1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscChest1"),
                selectionWeight = 240,
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
                selectionWeight = 40,
            };
            DirectorCard ADLunarChest = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarChest"),
                selectionWeight = 15,
            };
            DirectorCard ADCategoryChestDamage = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestDamage"),
                selectionWeight = 25,
            };
            DirectorCard ADCategoryChestHealing = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestHealing"),
                selectionWeight = 25,
            };
            DirectorCard ADCategoryChestUtility = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestUtility"),
                selectionWeight = 25,
            };
            DirectorCard ADTripleShopLarge = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscTripleShopLarge"),
                selectionWeight = 40,
            };
            DirectorCard ADCasinoChest = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCasinoChest"),
                selectionWeight = 40,
            };
            DirectorCard ADTripleShopEquipment = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscTripleShopEquipment"),
                selectionWeight = 20,
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
                selectionWeight = 40,
            };
            DirectorCard ADVoidTriple = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidTriple/iscVoidTriple.asset").WaitForCompletion(),
                selectionWeight = 20,
            };

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
                selectionWeight = 50,
            };
            ADBarrelVoidCoin.spawnCard = iscVoidCoinBarrel;
            DirectorCard ADScrapperB = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscScrapper"),
                selectionWeight = 10,
            };
            DirectorCard ADVoidCamp = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidCamp/iscVoidCamp.asset").WaitForCompletion(),
                selectionWeight = 10,
            };


            DirectorCard ADDummyRareBarrel = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBarrel1"),
                selectionWeight = 1,
            };
            //BarrelEnd
            //Shrines
            DirectorCard ADShrineCombat = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineCombat"),
                selectionWeight = 250,
            };
            DirectorCard ADShrineBoss = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineBoss"),
                selectionWeight = 250,
            };
            DirectorCard ADShrineChance = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineChance"),
                selectionWeight = 400,
            };
            DirectorCard ADShrineBlood = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineBlood"),
                selectionWeight = 200,
            };
            DirectorCard ADShrineHealing = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineHealing"),
                selectionWeight = 200,
            };
            DirectorCard ADShrineRestack = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineRestack"),
                selectionWeight = 50,
            };
            DirectorCard ADShrineCleanse = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineCleanse"),
                selectionWeight = 100,
            };
            DirectorCard ADShrineShaping = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/iscShrineColossusAccess.asset").WaitForCompletion(),
                selectionWeight = 50,
            };
            //ShrinesEnd
            //Drones
            DirectorCard ADBrokenDrone1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone1"),
                selectionWeight = 21,
            };
            DirectorCard ADBrokenDrone2 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone2"),
                selectionWeight = 21,
            };
            DirectorCard ADBrokenEmergencyDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenEmergencyDrone"),
                selectionWeight = 14,
            };
            DirectorCard ADBrokenEquipmentDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenEquipmentDrone"),
                selectionWeight = 14,
            };
            DirectorCard ADBrokenFlameDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenFlameDrone"),
                selectionWeight = 14,
            };
            DirectorCard ADBrokenMegaDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMegaDrone"),
                selectionWeight = 3,
            };
            DirectorCard ADBrokenMissileDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMissileDrone"),
                selectionWeight = 14,
            };
            DirectorCard ADBrokenTurret1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenTurret1"),
                selectionWeight = 14,
            };
            //DronesEnd
            //Rare
            DirectorCard ADGoldChest = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscGoldChest"),
                selectionWeight = 5,
            };
            DirectorCard ADShrineGoldshoresAccess = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineGoldshoresAccess"),
                selectionWeight = 5,
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
                selectionWeight = 20,
            };
            DirectorCard AdShrineHalcyonite = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/iscShrineHalcyonite.asset").WaitForCompletion(),
                selectionWeight = 20,
                minimumStageCompletions = 1
            };

            //RareEnd
            //Duplicators
                DirectorCard ADDuplicator = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscDuplicator"),
                selectionWeight = 300,
            };
            DirectorCard ADDuplicatorLarge = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscDuplicatorLarge"),
                selectionWeight = 80,
            };
            DirectorCard ADDuplicatorMilitary = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscDuplicatorMilitary"),
                selectionWeight = 30,
            };
            DirectorCard ADDuplicatorWild = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscDuplicatorWild"),
                selectionWeight = 30,
            };
            DirectorCard ADScrapper = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscScrapper"),
                selectionWeight = 120,
            };
            DirectorCard ADSoupWhiteGreen = new DirectorCard
            {
                spawnCard = SoupWhiteGreenISC,
                selectionWeight = 75,
            };
            DirectorCard ADSoupGreenRed = new DirectorCard
            {
                spawnCard = SoupGreenRedISC,
                selectionWeight = 45,
            };
            DirectorCard ADSoupRedWhite = new DirectorCard
            {
                spawnCard = SoupRedWhiteISC,
                selectionWeight = 25,
            };
            //DuplicatorsEnd


            mixInteractablesCards.AddCategory("Chests", 48); //0
            mixInteractablesCards.AddCategory("Barrels", 10); //1
            mixInteractablesCards.AddCategory("Shrines", 11); //2
            mixInteractablesCards.AddCategory("Drones", 11); //3
            mixInteractablesCards.AddCategory("Misc", 0); //4
            mixInteractablesCards.AddCategory("Rare", 0.7f); //5
            mixInteractablesCards.AddCategory("Duplicator", 9f); //6


            //Cut to 6?
            mixInteractablesCards.AddCard(0, ADChest1);  //15
            mixInteractablesCards.AddCard(0, ADChest2);  //30
            mixInteractablesCards.AddCard(0, ADCategoryChestDamage);  //15
            mixInteractablesCards.AddCard(0, ADCategoryChestHealing);  //15
            mixInteractablesCards.AddCard(0, ADCategoryChestUtility);  //15
            mixInteractablesCards.AddCard(0, ADEquipmentBarrel);  //1
            mixInteractablesCards.AddCard(0, ADLunarChest);  //25
            mixInteractablesCards.AddCard(0, ADTripleShop);  //20
            mixInteractablesCards.AddCard(0, ADTripleShopLarge);  //40
            mixInteractablesCards.AddCard(0, ADTripleShopEquipment);  //2
            mixInteractablesCards.AddCard(0, ADCasinoChest);  //20
            //mixInteractablesCards.AddCard(0, ADChest1Stealthed);  //10

            mixInteractablesCards.AddCard(0, ADCategoryChest2Damage);  //20
            mixInteractablesCards.AddCard(0, ADCategoryChest2Healing);  //20
            mixInteractablesCards.AddCard(0, ADCategoryChest2Utility);  //20
            mixInteractablesCards.AddCard(0, ADVoidChest);  //20
            mixInteractablesCards.AddCard(0, ADVoidTriple);  //20

            //Singular Barrel
            mixInteractablesCards.AddCard(1, ADBarrel1);  //1     
            mixInteractablesCards.AddCard(1, ADBarrelVoidCoin);  //1   
            mixInteractablesCards.AddCard(1, ADScrapperB);  //1     
            mixInteractablesCards.AddCard(1, ADVoidCamp);  //1     
            //Cut to 3?
            mixInteractablesCards.AddCard(2, ADShrineBlood);  //20
            mixInteractablesCards.AddCard(2, ADShrineBoss);  //20
            mixInteractablesCards.AddCard(2, ADShrineBoss);  //20
            mixInteractablesCards.AddCard(2, ADShrineChance);  //20
            mixInteractablesCards.AddCard(2, ADShrineCleanse);  //5
            mixInteractablesCards.AddCard(2, ADShrineCombat);  //20
            mixInteractablesCards.AddCard(2, ADShrineHealing);  //15
            mixInteractablesCards.AddCard(2, ADShrineRestack);  //30
            mixInteractablesCards.AddCard(2, ADShrineShaping);  //30
            
            //Cut to 3?
            mixInteractablesCards.AddCard(3, ADBrokenDrone1);  //15
            mixInteractablesCards.AddCard(3, ADBrokenDrone2);  //15
            mixInteractablesCards.AddCard(3, ADBrokenEmergencyDrone);  //30
            mixInteractablesCards.AddCard(3, ADBrokenEquipmentDrone);  //15
            mixInteractablesCards.AddCard(3, ADBrokenFlameDrone);  //30
            mixInteractablesCards.AddCard(3, ADBrokenMegaDrone);  //40
            mixInteractablesCards.AddCard(3, ADBrokenMissileDrone);  //20
            mixInteractablesCards.AddCard(3, ADBrokenTurret1);  //10
            //Unused
            //Misc
            //Cut to 1?

            mixInteractablesCards.AddCard(5, ADChest1Stealthed);  //10
            mixInteractablesCards.AddCard(5, ADGoldChest);  //50
            mixInteractablesCards.AddCard(5, ADShrineGoldshoresAccess);  //1
            mixInteractablesCards.AddCard(5, ADVoidSuppressor);  //1
            mixInteractablesCards.AddCard(5, ADLunarSeer);  //1
            mixInteractablesCards.AddCard(5, AdShrineHalcyonite);  //1

            //Cut to 4?
            mixInteractablesCards.AddCard(6, ADScrapper);  //5
            mixInteractablesCards.AddCard(6, ADDuplicator);  //5
            mixInteractablesCards.AddCard(6, ADDuplicatorLarge);  //10
            mixInteractablesCards.AddCard(6, ADDuplicatorMilitary);  //15
            mixInteractablesCards.AddCard(6, ADDuplicatorWild);  //10
            mixInteractablesCards.AddCard(6, ADSoupWhiteGreen);  //10
            mixInteractablesCards.AddCard(6, ADSoupGreenRed);  //10
            mixInteractablesCards.AddCard(6, ADSoupRedWhite);  //5

            //


        }


        public static WeightedSelection<DirectorCard> MixInteractableApplier(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, RoR2.SceneDirector self)
        {
            //Debug.Log("Artifact of Dissimilarity: MixInteractableApplier");
            if (self.interactableCredit != 0)
            {
                self.interactableCredit += 24;
            }
            WeightedSelection<DirectorCard> DissimilarityDirectorCards = new WeightedSelection<DirectorCard>();
            TrimmedmixInteractablesCards.Clear();
            TrimmedmixInteractablesCards.CopyFrom(mixInteractablesCards);

            //Chests
            //Barrels + Void Camp
            TrimmedmixInteractablesCards.categories[2].selectionWeight = random.Next(7, 13); //Shrines range from 7 to 10 in vanilla
            TrimmedmixInteractablesCards.categories[3].selectionWeight = random.Next(3, 15); //Drones either 3,4.5, 11 or 14
            //Misc Nothing ig
            //Rare
            //TrimmedmixInteractablesCards.categories[6].selectionWeight = random.Next(7, 10); //Duplicator is 8 in vanilla

            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Main.Dissimilarity_Def))
            {
                Filters.ApplyCardRemovingFilters(TrimmedmixInteractablesCards);
                Filters.MixInteractableTrimmer3(TrimmedmixInteractablesCards);
                Filters.ApplySandSnow(TrimmedmixInteractablesCards);


                if (Run.instance && Run.instance.stageClearCount == 0)
                {
                    DirectorCard AltPathAccess = new DirectorCard
                    {
                        spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC2/iscShrineHalcyoniteTier1.asset").WaitForCompletion(),
                        selectionWeight = 200,
                    };
                    TrimmedmixInteractablesCards.AddCard(2, AltPathAccess);
                }

                DissimilarityDirectorCards = TrimmedmixInteractablesCards.GenerateDirectorCardWeightedSelection();
                Debug.Log("Artifact of Dissimilarity: Generated Trimmed mixInteractables selection");

                if (WConfig.DebugPrint.Value == true)
                {
                    Debug.Log("__________________________________________________");
                    Debug.Log("Artifact of Dissimilarity: Trimmed Interactable List");
                    Debug.Log("");
                    TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.TestingPrintCardResults));
                    Debug.Log("__________________________________________________");
                };

            }
            else
            {
                //DissimilarityDirectorCards = ClassicStageInfo.instance.interactableCategories.GenerateDirectorCardWeightedSelection(); //fallback
                Debug.LogWarning("Artifact of Dissimilarity: Failed to generate normal Interactable Categories, using fallback");
                return orig(self);
            };
            return DissimilarityDirectorCards;
        }


    }
}