using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.ScriptableObjects;
using R2API.Utils;
using RoR2;
using RoR2.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ArtifactDissimilarity
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Wolfo.ArtifactOfDissimilarity", "ArtifactOfDissimilarity", "2.0.3")]
    [R2APISubmoduleDependency(nameof(ContentAddition), nameof(PrefabAPI), nameof(LanguageAPI), nameof(ArtifactCodeAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class ArtifactDissimilarity : BaseUnityPlugin
    {

        public static ItemDef ScrapWhiteSuppressed = Addressables.LoadAssetAsync<ItemDef>(key: "RoR2/DLC1/ScrapVoid/ScrapWhiteSuppressed.asset").WaitForCompletion();
        public static ItemDef ScrapGreenSuppressed = Addressables.LoadAssetAsync<ItemDef>(key: "RoR2/DLC1/ScrapVoid/ScrapGreenSuppressed.asset").WaitForCompletion();
        public static ItemDef ScrapRedSuppressed = Addressables.LoadAssetAsync<ItemDef>(key: "RoR2/DLC1/ScrapVoid/ScrapRedSuppressed.asset").WaitForCompletion();

        public static SceneDef WanderPreviousSceneDef = null;

        public static SpawnCard iscShrineBlood = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBlood.asset").WaitForCompletion();
        public static SpawnCard iscShrineBloodSnowy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBloodSnowy.asset").WaitForCompletion();
        public static SpawnCard iscShrineBloodSandy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBloodSandy.asset").WaitForCompletion();

        public static SpawnCard iscShrineBoss = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion();
        public static SpawnCard iscShrineBossSnowy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBossSnowy.asset").WaitForCompletion();
        public static SpawnCard iscShrineBossSandy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBossSandy.asset").WaitForCompletion();

        public static SpawnCard iscShrineChance = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChance.asset").WaitForCompletion();
        public static SpawnCard iscShrineChanceSnowy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChanceSnowy.asset").WaitForCompletion();
        public static SpawnCard iscShrineChanceSandy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChanceSandy.asset").WaitForCompletion();

        public static SpawnCard iscShrineCleanse = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanse.asset").WaitForCompletion();
        public static SpawnCard iscShrineCleanseSnowy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanseSnowy.asset").WaitForCompletion();
        public static SpawnCard iscShrineCleanseSandy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanseSandy.asset").WaitForCompletion();

        public static SpawnCard iscShrineCombat = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombat.asset").WaitForCompletion();
        public static SpawnCard iscShrineCombatSnowy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombatSnowy.asset").WaitForCompletion();
        public static SpawnCard iscShrineCombatSandy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombatSandy.asset").WaitForCompletion();

        public static SpawnCard iscShrineRestack = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestack.asset").WaitForCompletion();
        public static SpawnCard iscShrineRestackSnowy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestackSnowy.asset").WaitForCompletion();
        public static SpawnCard iscShrineRestackSandy = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestackSandy.asset").WaitForCompletion();


        public static ArtifactDef Dissimilarity = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Kith = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Wander = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Remodeling = ScriptableObject.CreateInstance<ArtifactDef>();
        //public static ArtifactDef Retooling = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Spiriting = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Brigade = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Transpose = ScriptableObject.CreateInstance<ArtifactDef>();


        public static ArtifactDef Command = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Command");
        public static ArtifactDef MonsterTeamGain = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MonsterTeamGainsItems");
        public static ArtifactDef ZetDropifact;
        public static ArtifactDef RiskyConformity;

        public static ArtifactDef tempartifact = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/FriendlyFire");

        public static ArtifactCode DissimCode = ScriptableObject.CreateInstance<ArtifactCode>();
        public static ArtifactCode KithCode = ScriptableObject.CreateInstance<ArtifactCode>();
        public static ArtifactCode WanderCode = ScriptableObject.CreateInstance<ArtifactCode>();
        public static ArtifactCode RemodelCode = ScriptableObject.CreateInstance<ArtifactCode>();
        //public static ArtifactCode RetoolingCode = ScriptableObject.CreateInstance<ArtifactCode>();
        public static ArtifactCode SpiritingCode = ScriptableObject.CreateInstance<ArtifactCode>();
        public static ArtifactCode BrigadeCode = ScriptableObject.CreateInstance<ArtifactCode>();
        public static ArtifactCode TransposeCode = ScriptableObject.CreateInstance<ArtifactCode>();

        public static EquipmentIndex[] bossAffixes = Array.Empty<EquipmentIndex>();
        public static EquipmentIndex[] brigadedAffixes = Array.Empty<EquipmentIndex>();

        public static UnlockableDef NoMoreArtifact = ScriptableObject.CreateInstance<UnlockableDef>();
        public static DirectorCardCategorySelection HelperSingleMixInteractable = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static DirectorCardCategorySelection mixInteractablesCards = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static DirectorCardCategorySelection TrimmedmixInteractablesCards = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static DirectorCardCategorySelection TrimmedSingleInteractableType = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();


        public static BasicPickupDropTable dtMonsterTeamTier2Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier2Item.asset").WaitForCompletion();
        /*
        public static BasicPickupDropTable dtMonsterTeamTier1Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier1Item.asset").WaitForCompletion();
        public static BasicPickupDropTable dtMonsterTeamTier3Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/MonsterTeamGainsItems/dtMonsterTeamTier3Item.asset").WaitForCompletion();

        public static BasicPickupDropTable dtAISafeTier1Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/Common/dtAISafeTier1Item.asset").WaitForCompletion();
        public static BasicPickupDropTable dtAISafeTier2Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/Common/dtAISafeTier2Item.asset").WaitForCompletion();
        public static BasicPickupDropTable dtAISafeTier3Item = Addressables.LoadAssetAsync<BasicPickupDropTable>(key: "RoR2/Base/Common/dtAISafeTier3Item.asset").WaitForCompletion();
        */

        public static CharacterMaster[] EquipmentDroneList;

        public static CombatDirector.EliteTierDef[] normalelitetierdefs;
        public static bool DidBrigadeHappen;
        private static Inventory ArenaInventory = new Inventory();


        public static ConfigEntry<bool> EnableDissim;
        public static ConfigEntry<bool> EnableKith;
        public static ConfigEntry<bool> EnableWanderArtifact;
        public static ConfigEntry<bool> EnableRemodelArtifact;
        //public static ConfigEntry<bool> EnableRetoolingArtifact;
        public static ConfigEntry<bool> EnableSpiritualArtifact;
        public static ConfigEntry<bool> EnableBrigadeArtifact;
        public static ConfigEntry<bool> EnableTransposeArtifact;

        /*
        public static ConfigEntry<bool> DisableEnigmaArtifact;
        public static ConfigEntry<bool> DisableDeathArtifact;
        public static ConfigEntry<bool> DisableWeakAssKneeArtifact;
        public static ConfigEntry<bool> DisableSoulArtifact;
        public static ConfigEntry<bool> DisableSwarmArtifact;
        public static ConfigEntry<bool> DisableGlassArtifact;
        public static ConfigEntry<bool> DisableChaosArtifact;
        */

        public static ConfigEntry<bool> EnableCustomIneractables;
        //public static ConfigEntry<bool> EnableLunarSeers;
        public static ConfigEntry<bool> ChangeBazaarSeer;
        public static ConfigEntry<bool> RemoveMinimumStageCompletion;
        public static ConfigEntry<bool> AncestralIncubatorBool;
        public static ConfigEntry<bool> RerollOutOfScrap;
        public static ConfigEntry<bool> RerollMonsterItems;
        public static ConfigEntry<bool> RerollEquipmentDrones;

        public static ConfigEntry<bool> DebugPrintDissimilarity;
        public static ConfigEntry<bool> DebugPrintKith;
        public static ConfigEntry<bool> DebugPrintLunarSeer;

        public static ConfigEntry<bool> SpiritStun;
        public static ConfigEntry<float> SpiritMovement;
        public static ConfigEntry<float> SpiritAccel;
        public static ConfigEntry<float> SpiritAttackSpeed;
        public static ConfigEntry<float> SpiritAttackSpeedPlayer;
        public static ConfigEntry<float> SpiritJump;
        public static ConfigEntry<float> SpiritJumpPlayer;
        public static ConfigEntry<float> SpiritCooldown;
        public static ConfigEntry<float> SpiritDamage;
        public static ConfigEntry<float> SpiritDamagePlayer;
        public static ConfigEntry<float> SpiritProjectileSpeed;

        public static int ChestDissimTrimCount = 7;
        public static int RareDissimTrimCount = 2;

        public static float SpiritSpeedVal;
        public static float SpiritSpeedHalfVal;
        public static float SpiritAttackSpeedVal;
        public static float SpiritAttackSpeedPlayerVal;
        public static float SpiritJumpVal;
        public static float SpiritJumpPlayerVal;
        public static float SpiritCooldownVal;
        public static float SpiritDamageVal;
        public static float SpiritDamagePlayerVal;
        public static float SpiritProjectileSpeedVal;
        public static float SpiritProjectileSpeedPlusVal;

        public static float temphealthfrac;

        public static bool RedSoupBought = false;

        private static ulong ADseedServer;
        private static Xoroshiro128Plus ADrng;

        private static InteractableSpawnCard LunarTP = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarTeleporter");
        private static SceneExitController tempexitcontroller;

        private static readonly SceneDef Roost = LegacyResourcesAPI.Load<SceneDef>("scenedefs/blackbeach");
        private static readonly SceneDef Plains = LegacyResourcesAPI.Load<SceneDef>("scenedefs/golemplains");
        private static readonly SceneDef Sand = LegacyResourcesAPI.Load<SceneDef>("scenedefs/goolake");
        private static readonly SceneDef Swamp = LegacyResourcesAPI.Load<SceneDef>("scenedefs/foggyswamp");
        private static readonly SceneDef Acres = LegacyResourcesAPI.Load<SceneDef>("scenedefs/wispgraveyard");
        private static readonly SceneDef Snow = LegacyResourcesAPI.Load<SceneDef>("scenedefs/frozenwall");
        private static readonly SceneDef Depths = LegacyResourcesAPI.Load<SceneDef>("scenedefs/dampcavesimple");
        private static readonly SceneDef Grove = LegacyResourcesAPI.Load<SceneDef>("scenedefs/rootjungle");
        private static readonly SceneDef Siren = LegacyResourcesAPI.Load<SceneDef>("scenedefs/shipgraveyard");
        private static readonly SceneDef SkyMeadow = LegacyResourcesAPI.Load<SceneDef>("scenedefs/skymeadow");
        private static readonly SceneDef Moon2 = LegacyResourcesAPI.Load<SceneDef>("scenedefs/moon2");
        private static readonly SceneDef Gold = LegacyResourcesAPI.Load<SceneDef>("scenedefs/goldshores");

        private static readonly SceneDef Arena = LegacyResourcesAPI.Load<SceneDef>("scenedefs/arena");
        private static readonly SceneDef ArtifactWorld = LegacyResourcesAPI.Load<SceneDef>("scenedefs/artifactworld");
        private static readonly SceneDef Bazaar = LegacyResourcesAPI.Load<SceneDef>("scenedefs/bazaar");
        private static readonly SceneDef Limbo = LegacyResourcesAPI.Load<SceneDef>("scenedefs/limbo");
        private static readonly SceneDef MysterySpace = LegacyResourcesAPI.Load<SceneDef>("scenedefs/mysteryspace");
        private static readonly SceneDef MenuLobby = LegacyResourcesAPI.Load<SceneDef>("scenedefs/lobby");


        private static readonly SceneDef SnowyForest = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/snowyforest/snowyforest.asset").WaitForCompletion();
        private static readonly SceneDef AncientLoft = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/ancientloft/ancientloft.asset").WaitForCompletion();
        private static readonly SceneDef SulfurPools = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/sulfurpools/sulfurpools.asset").WaitForCompletion();
        private static readonly SceneDef VoidStage = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/voidstage/voidstage.asset").WaitForCompletion();
        private static readonly SceneDef VoidRaid = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/voidraid/voidraid.asset").WaitForCompletion();


        private static readonly InfiniteTowerRun InfiniteTowerRunBase = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerRun.prefab").WaitForCompletion().GetComponent<InfiniteTowerRun>();
        private static readonly SceneDef itgolemplains = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itgolemplains/itgolemplains.asset").WaitForCompletion();
        private static readonly SceneDef itgoolake = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itgoolake/itgoolake.asset").WaitForCompletion();
        private static readonly SceneDef itancientloft = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itancientloft/itancientloft.asset").WaitForCompletion();
        private static readonly SceneDef itfrozenwall = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itfrozenwall/itfrozenwall.asset").WaitForCompletion();
        private static readonly SceneDef itdampcave = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itdampcave/itdampcave.asset").WaitForCompletion();
        private static readonly SceneDef itskymeadow = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itskymeadow/itskymeadow.asset").WaitForCompletion();
        private static readonly SceneDef itmoon = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itmoon/itmoon.asset").WaitForCompletion();
        private static readonly SceneDef[] InfiniteTowerSceneDefs = { itgolemplains, itgoolake, itfrozenwall, itfrozenwall, itdampcave, itskymeadow, itmoon };


        private static readonly SceneCollection sgInfiniteTowerStage1 = InfiniteTowerRunBase.startingSceneGroup;
        private static readonly SceneCollection sgInfiniteTowerStageXGolemPlains = itgolemplains.destinationsGroup;
        private static readonly SceneCollection sgInfiniteTowerStageXGooLake = itgoolake.destinationsGroup;
        private static readonly SceneCollection sgInfiniteTowerStageXAncientLoft = itancientloft.destinationsGroup;
        private static readonly SceneCollection sgInfiniteTowerStageXFrozenwall = itfrozenwall.destinationsGroup;
        private static readonly SceneCollection sgInfiniteTowerStageXDampCave = itdampcave.destinationsGroup;
        private static readonly SceneCollection sgInfiniteTowerStageXSkyMeadow = itskymeadow.destinationsGroup;
        private static readonly SceneCollection sgInfiniteTowerStageXMoon = itmoon.destinationsGroup;

        private static readonly SceneCollection sgInfiniteTowerStage1Wander = ScriptableObject.CreateInstance<SceneCollection>();
        private static readonly SceneCollection sgInfiniteTowerStageXGolemPlainsWander = ScriptableObject.CreateInstance<SceneCollection>();
        private static readonly SceneCollection sgInfiniteTowerStageXGooLakeWander = ScriptableObject.CreateInstance<SceneCollection>();
        private static readonly SceneCollection sgInfiniteTowerStageXAncientLoftWander = ScriptableObject.CreateInstance<SceneCollection>();
        private static readonly SceneCollection sgInfiniteTowerStageXFrozenwallWander = ScriptableObject.CreateInstance<SceneCollection>();
        private static readonly SceneCollection sgInfiniteTowerStageXDampCaveWander = ScriptableObject.CreateInstance<SceneCollection>();
        private static readonly SceneCollection sgInfiniteTowerStageXSkyMeadowWander = ScriptableObject.CreateInstance<SceneCollection>();
        private static readonly SceneCollection sgInfiniteTowerStageXMoonWander = ScriptableObject.CreateInstance<SceneCollection>();





        private static readonly Material PortalMaterialArena = Instantiate(Plains.portalMaterial);
        private static readonly Material PortalMaterialArtifactWorld = Instantiate(Plains.portalMaterial);
        private static readonly Material PortalMaterialBazaar = Instantiate(Plains.portalMaterial);
        private static readonly Material PortalMaterialLimbo = Instantiate(Plains.portalMaterial);
        private static readonly Material PortalMaterialMysterySpace = Instantiate(Plains.portalMaterial);
        private static readonly Material PortalMaterialMenuLobby = Instantiate(Plains.portalMaterial);
        private static readonly Material PortalMaterialVoidRaid = Instantiate(Plains.portalMaterial);

        private static readonly Material PortalMaterialITGolemPlains = Instantiate(Plains.portalMaterial);
        private static readonly Material PortalMaterialITGooLake = Instantiate(Sand.portalMaterial);
        private static readonly Material PortalMaterialITAncientLoft = Instantiate(AncientLoft.portalMaterial);
        private static readonly Material PortalMaterialITFrozenWall = Instantiate(Snow.portalMaterial);
        private static readonly Material PortalMaterialITDampCave = Instantiate(Depths.portalMaterial);
        private static readonly Material PortalMaterialITSkyMeadow = Instantiate(SkyMeadow.portalMaterial);
        private static readonly Material PortalMaterialITMoon = Instantiate(Moon2.portalMaterial);




        static readonly System.Random random = new System.Random();
        static readonly List<SceneDef> sceneNames = new List<SceneDef> { Roost, Plains, Sand, Swamp, Acres, Snow, Depths, Grove, Siren, SkyMeadow, Moon2, Gold, Arena, MysterySpace, ArtifactWorld, SnowyForest, AncientLoft, SulfurPools, VoidStage };
        static List<SceneDef> tempsceneNames = new List<SceneDef>(sceneNames);
        static readonly List<SceneDef> sceneNamesForWanderList = new List<SceneDef> { Roost, Plains, Sand, Swamp, Acres, Snow, Depths, Grove, Siren, SkyMeadow, SnowyForest, AncientLoft, SulfurPools };
        private static SceneDef[] sceneNamesForWander = new SceneDef[10];

        private static List<EliteDef> EliteDefsTier1 = new List<EliteDef>();
        private static List<EliteDef> EliteDefsTier2 = new List<EliteDef>();
        private static List<EliteDef> ForUsageEliteDefList = new List<EliteDef>();
        private static EliteDef TempForUsageEliteDef;
        private static EliteDef NoRepeatForUsageEliteDef;

        /*
        static List<ItemIndex> WhiteItemList = new List<ItemIndex>();
        static List<ItemIndex> GreenItemList = new List<ItemIndex>();
        static List<ItemIndex> RedItemList = new List<ItemIndex>();
        static List<ItemIndex> YellowItemList = new List<ItemIndex>();
        static List<ItemIndex> BlueItemList = new List<ItemIndex>();
        static List<ItemIndex> PinkT1ItemList = new List<ItemIndex>();
        static List<ItemIndex> PinkT2ItemList = new List<ItemIndex>();
        static List<ItemIndex> PinkT3ItemList = new List<ItemIndex>();
        static List<ItemIndex> PinkBossItemList = new List<ItemIndex>();
        */

        private static List<PickupIndex> MonsterWhiteItemList = new List<PickupIndex>();
        private static List<PickupIndex> MonsterGreenItemList = new List<PickupIndex>();
        private static List<PickupIndex> MonsterRedItemList = new List<PickupIndex>();
        private static List<PickupIndex> MonsterBlueItemList = new List<PickupIndex>();


        private static List<PickupIndex> SimulacrumWhiteItemList = new List<PickupIndex>();
        private static List<PickupIndex> SimulacrumGreenItemList = new List<PickupIndex>();
        private static List<PickupIndex> SimulacrumRedItemList = new List<PickupIndex>();
        private static List<PickupIndex> SimulacrumBlueItemList = new List<PickupIndex>();


        //static List<ItemIndex> BlacklistedItems = new List<ItemIndex>();
        //static List<ItemIndex> MonsterBlacklistedItems = new List<ItemIndex>();

        static List<PickupIndex> TempTrimAllItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimWhiteItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimGreenItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimRedItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimYellowItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimBlueItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimPinkT1ItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimPinkT2ItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimPinkT3ItemList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimPinkBossItemList = new List<PickupIndex>();

        //static List<EquipmentIndex> OrangeEquipmentList = new List<EquipmentIndex>();
        //static List<EquipmentIndex> BlueEquipmentList = new List<EquipmentIndex>();
        //tatic List<PickupIndex> YellowEquipmentList = new List<EquipmentIndex>();
        static List<EquipmentIndex> EliteEquipmentList = new List<EquipmentIndex>();
        //static List<EquipmentIndex> T2EliteEquipmentList = new List<EquipmentIndex>();

        static List<EquipmentIndex> FullEquipmentList = new List<EquipmentIndex>();
        static List<EquipmentIndex> BlacklistedEquipment = new List<EquipmentIndex>();

        static List<PickupIndex> TempTrimOrangeEquipmentList = new List<PickupIndex>();
        static List<PickupIndex> TempTrimBlueEquipmentList = new List<PickupIndex>();
        //static List<PickupIndex> TempTrimYellowEquipmentList = new List<PickupIndex>();
        static List<EquipmentIndex> TempTrimEliteEquipmentList = new List<EquipmentIndex>();
        //static List<EquipmentIndex> TempTrimT2EliteEquipmentList = new List<EquipmentIndex>();

        static List<int> WhiteItemCountList = new List<int>();
        static List<int> GreenItemCountList = new List<int>();
        static List<int> RedItemCountList = new List<int>();
        static List<int> YellowItemCountList = new List<int>();
        static List<int> BlueItemCountList = new List<int>();
        static List<int> PinkT1ItemCountList = new List<int>();
        static List<int> PinkT2ItemCountList = new List<int>();
        static List<int> PinkT3ItemCountList = new List<int>();
        static List<int> PinkBossItemCountList = new List<int>();


        static RoR2.Inventory MonsterTeamGainItemRandom;

        static int TotalItemCount = 132;
        static int TotalEquipmentCount = 40;


        static bool DumbItemThing = false;
        static bool DumbEquipmentThing = false;
        static bool DumbItemThingMonster = false;
        static bool DumbEliteKinThing = false;
        //static bool DumbT2EliteKinThing = false;
        static int DumbT2EliteKinInt = 1;
        static bool SpiritAdded = false;
        static bool DumbShieldThing = false;
        public static SpawnCard KithNoRepeat;

        /*
        //private static int[] WhiteItemList;
        private static int[] GreenItemList;
        private static int[] RedItemList;
        private static int[] YellowItemList;
        private static int[] BlueItemList;
        */

        private void InitConfig()
        {
            ConfigFile configFile = new ConfigFile(Paths.ConfigPath + "\\com.Wolfo.ArtifactOfDissimilarity.cfg", true);

            EnableDissim = configFile.Bind(
                "0 - Artifacts",
                "Enable/Disable Artifact of Dissimilarity",
                true,
                "Artifact of Dissonance for Interactables"
            );
            EnableKith = configFile.Bind(
                "0 - Artifacts",
                "Enable/Disable Artifact of Kith",
                true,
                "Artifact of Kin for Interactables. One per interactable category"
            );
            EnableWanderArtifact = configFile.Bind(
                "0 - Artifacts",
                "Enable/Disable Artifact of Wander",
                true,
                "Just another mod to add a way to enable the Gamerule Meander used in Prsimatic Trials which makes stages progress in a random order.\nOption to disable it for compatibility or disinterest reasons"
            );
            EnableRemodelArtifact = configFile.Bind(
                "0 - Artifacts",
                "Enable/Disable Artifact of Remodel",
                true,
                "Reroll all items and equipment on stage change"
            );
            EnableSpiritualArtifact = configFile.Bind(
                "0 - Artifacts",
                "Enable/Disable Artifact of Spiriting",
                true,
                "RoR1 Spirit inspired, affects more stats and is more extreme"
            );
            EnableBrigadeArtifact = configFile.Bind(
                "0 - Artifacts",
                "Enable/Disable Artifact of Brigade",
                true,
                "Artifact of Kin but for Elite Types"
            );
            EnableTransposeArtifact = configFile.Bind(
                "0 - Artifacts",
                "Enable/Disable Artifact of Transpose",
                true,
                "Get a random skill loadout every stage"
            );



            ChangeBazaarSeer = configFile.Bind(
                "1a - Dissimilarity",
                "Should Bazaar Seers be randomized too",
                false,
                "If turned on, Lunar Seers in the Bazaar will go to random locations."
            );
            RemoveMinimumStageCompletion = configFile.Bind(
                "1b - Kith",
                "Remove the minimum stage completion of vanilla interactables",
                true,
                "Normally certain interactables can only spawn after a certain stage, this doesn't prevent them from being chosen with Kith. Making it so they can always spawn might make the early game more varied.\n Default minium stage completions;\n Stage 2 onwards; Gold Shrine, Green Printer, Yellow Printer\nStage 3 onwards; Emergency Drone\nStage 4 onwards; Legendary Chest\nStage 5 onwards; Red Printer"
            );
            AncestralIncubatorBool = configFile.Bind(
                "1c - Remodeling",
                "Turn the unreleased Ancestral Incubator into a Yellow item",
                false,
                "If enabled you'll be able to reroll into it with Remodeling or get it from Yellow Printers.\nWhy would you want this"
            );
            RerollOutOfScrap = configFile.Bind(
                "1c - Remodeling",
                "Reroll out of Scrap",
                true,
                "If enabled, Scrap will be rerolled into a regular items\nThis allows you to quickly get a high stack of something (ie Scrap all Reds if you want 1 high stack instead of multiple small stacks).\nBut makes it so you can't take Scrap to Bazaar/Moon Cauldrons."
            );
            RerollMonsterItems = configFile.Bind(
                "1c - Remodeling",
                "Reroll Monster Teams Inventory",
                true,
                "Reroll the items that Artifact of Evolution gives."
            );

            RerollEquipmentDrones = configFile.Bind(
                "1c - Remodeling",
                "Reroll Equipment Drone Equipments",
                true,
                "Whether or not their Equipment should also be rerolled"
            );

            SpiritMovement = configFile.Bind(
                "1e - Spiriting",
                "Maximum Movement Speed Bonus",
                2.1f,
                "Bonus multiplier for Movement Speed. Setting to 0 will disable it."
            );
            SpiritJump = configFile.Bind(
                "1e - Spiriting",
                "Maximum Jump Bonus (Non Player)",
                0.35f,
                "Bonus multiplier for Jump height. Setting to 0 will disable it."
            );
            SpiritJumpPlayer = configFile.Bind(
                "1e - Spiriting",
                "Maximum Jump Bonus (Player)",
                0.15f,
                "Bonus multiplier for Jump height. Setting to 0 will disable it."
            );
            SpiritAttackSpeed = configFile.Bind(
                "1e - Spiriting",
                "Maximum Attack Speed Bonus (Non Player)",
                1.35f,
                "Bonus multiplier for Attack Speed. Setting to 0 will disable it."
            );
            SpiritAttackSpeedPlayer = configFile.Bind(
                "1e - Spiriting",
                "Maximum Attack Speed Bonus (Player)",
                1.05f,
                "Bonus multiplier for Attack Speed. Setting to 0 will disable it."
            );
            SpiritProjectileSpeed = configFile.Bind(
                "1e - Spiriting",
                "Maximum Projectile Speed Bonus",
                1f,
                "Bonus multiplier for Projectile Speed. Setting to 0 will disable it."
            );

            SpiritCooldown = configFile.Bind(
                "1e - Spiriting",
                "Maximum Cooldown Reduction",
                1f,
                "Maximum Cooldown Reduction in percent (0.0 - 1.0). 1 means 100% reduction at 0 health and 0 means no Reduction. Do not put over 1\nAWU, Mithrix attack where they gain health or turn invincible aren't affected."
            );
            SpiritDamage = configFile.Bind(
                "1e - Spiriting",
                "Maximum Damage Reduction (Non Player)",
                0.25f,
                "Maximum Damage Reduction in percent (0.0 - 1.0). 1 means 100% reduction at 0 health and 0 means no Reduction. Do not put over 1"
            );
            SpiritDamagePlayer = configFile.Bind(
                "1e - Spiriting",
                "Maximum Damage Reduction (Player)",
                0.25f,
                "Maximum Damage Reduction in percent (0.0 - 1.0). 1 means 100% reduction at 0 health and 0 means no Reduction. Do not put over 1"
            );

            /*
            DisableEnigmaArtifact = configFile.Bind(
                "6 - Disable Vanilla Artifact",
                "Disable Artifact of Enigma",
                false,
                "Make Artifact of Enigma not show up in the menu."
            );

            DisableDeathArtifact = configFile.Bind(
                "6 - Disable Vanilla Artifact",
                "Disable Artifact of Death",
                false,
                "Make Artifact of Death not show up in the menu\nIn case you hate this artifact and want it gone for symmetry."
            );
            DisableWeakAssKneeArtifact = configFile.Bind(
                "6 - Disable Vanilla Artifact",
                "Disable Artifact of Frailty",
                false,
                "Make Artifact of Frailty not show up in the menu\nIn case you just play EclipseMultiplayer and this artifact becomes redundant or you dislike this artifact and want it gone for symmetry."
            );
            DisableSoulArtifact = configFile.Bind(
                "6 - Disable Vanilla Artifact",
                "Disable Artifact of Soul",
                false,
                "Make Artifact of Soul not show up in the menu\nIn case you dislike this artifact or think it just feels awkward and want it gone for symmetry."
            );


            DisableGlassArtifact = configFile.Bind(
                "6 - Disable Vanilla Artifact",
                "Disable Artifact of Glass",
                false,
                "Make Artifact of Glass not show up in the menu\nIn case you just wanna stack glass or dislike this artifact and want it gone for symmetry."
            );
            DisableSwarmArtifact = configFile.Bind(
                "6 - Disable Vanilla Artifact",
                "Disable Artifact of Swarms",
                false,
                "Make Artifact of Swarms not show up in the menu\nIn case you dislike this artifact and want it gone for symmetry."
            );
            DisableChaosArtifact = configFile.Bind(
                "6 - Disable Vanilla Artifact",
                "Disable Artifact of Soul",
                false,
                "Make Artifact of Soul not show up in the menu\nIn case you dislike this artifact and want it gone for symmetry."
            );
            */


            DebugPrintDissimilarity = configFile.Bind(
                "Print Info",
                "Print Dissimilarity results in console",
                false,
                "Full list of interactables possible on current stage"
            );
            DebugPrintKith = configFile.Bind(
                "Print Info",
                "Print Kith results in console",
                false,
                "Full list of interactables possible on current stage"
            );
            DebugPrintLunarSeer = configFile.Bind(
                "Print Info",
                "Print when a LunarSeer spawns and its destination",
                false,
                "Mild spoiler for people looking at BepInEx Output log"
            );
        }


        internal static void NewTrimmer1(ref DirectorCard[] cards, int requiredCount, ref ClassicStageInfo self)
        {
            if (cards.Length <= requiredCount)
            {
                return;
            }
            DirectorCard[] array = HG.ArrayUtils.Clone<DirectorCard>(cards);
            RoR2.Util.ShuffleArray(array, ADrng);
            if (array.Length > requiredCount)
            {
                Array.Resize<DirectorCard>(ref array, requiredCount);
            }
            cards = array;
        }

        internal static void NewTrimmer2(string categoryName, int requiredCount, ref ClassicStageInfo self)
        {
            DirectorCardCategorySelection.Category[] categories = self.interactableCategories.categories;
            for (int i = 0; i < categories.Length; i++)
            {
                if (string.CompareOrdinal(categoryName, categories[i].name) == 0)
                {
                    NewTrimmer1(ref categories[i].cards, requiredCount, ref self);
                }
            }
        }

        public static void NewTrimmer3Mix(DirectorCardCategorySelection interactableCategories)
        {
            ADseedServer = Run.instance.stageRng.nextUlong;
            ADrng = new Xoroshiro128Plus(ADseedServer);
            //Debug.Log(ADseedServer);

            ClassicStageInfo CSI_Dummy = new ClassicStageInfo();
            CSI_Dummy.interactableCategories = interactableCategories;
            CSI_Dummy.interactableCategories.CopyFrom(TrimmedmixInteractablesCards);
            NewTrimmer2("Chests", 7, ref CSI_Dummy);
            if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
            {
                NewTrimmer2("Barrels", 0, ref CSI_Dummy);
                NewTrimmer2("Shrines", 2, ref CSI_Dummy);
                NewTrimmer2("Drones", 0, ref CSI_Dummy);
                TrimmedmixInteractablesCards.categories[1].selectionWeight = 0;
                TrimmedmixInteractablesCards.categories[2].selectionWeight = random.Next(1, 6);
                TrimmedmixInteractablesCards.categories[3].selectionWeight = 0;
            }
            else
            {
                NewTrimmer2("Barrels", 2, ref CSI_Dummy);
                NewTrimmer2("Shrines", 3, ref CSI_Dummy);
                NewTrimmer2("Drones", 3, ref CSI_Dummy);

            }
            NewTrimmer2("Rare", 2, ref CSI_Dummy);
            if (RunArtifactManager.instance.IsArtifactEnabled(Remodeling))
            {
                NewTrimmer2("Duplicator", 2, ref CSI_Dummy);
            }
            else
            {
                NewTrimmer2("Duplicator", 4, ref CSI_Dummy);
            }
        }

        public static void NewTrimmer3Single(DirectorCardCategorySelection interactableCategories)
        {
            ADseedServer = Run.instance.stageRng.nextUlong;
            ADrng = new Xoroshiro128Plus(ADseedServer);
            //Debug.Log(ADseedServer);

            ClassicStageInfo CSI_Dummy = new ClassicStageInfo();
            CSI_Dummy.interactableCategories = interactableCategories;
            CSI_Dummy.interactableCategories.CopyFrom(TrimmedSingleInteractableType);
            NewTrimmer2("Chests", 1, ref CSI_Dummy);
            if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
            {
                NewTrimmer2("Barrels", 0, ref CSI_Dummy);
                NewTrimmer2("Drones", 0, ref CSI_Dummy);
                NewTrimmer2("Misc", 0, ref CSI_Dummy);
            }
            else
            {
                NewTrimmer2("Barrels", 1, ref CSI_Dummy);
                NewTrimmer2("Drones", 1, ref CSI_Dummy);
                NewTrimmer2("Misc", 1, ref CSI_Dummy);
            }
            NewTrimmer2("Shrines", 1, ref CSI_Dummy);
            NewTrimmer2("Rare", 1, ref CSI_Dummy);
            NewTrimmer2("Duplicator", 1, ref CSI_Dummy);
            NewTrimmer2("Void Stuff", 1, ref CSI_Dummy);
        }

        public static bool NoMoreRadioTower(DirectorCard card)
        {
            card.selectionWeight = 1;
            GameObject prefab = card.spawnCard.prefab;
            return !prefab.GetComponent<RoR2.RadiotowerTerminal>();
        }


        public static bool CommandArtifactTrimmer(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.GetComponent<RoR2.ShopTerminalBehavior>() | prefab.GetComponent<RoR2.MultiShopController>() | prefab.GetComponent<RoR2.ScrapperController>() | prefab.GetComponent<RoR2.RouletteChestController>());
        }

        public static bool SacrificeArtifactTrimmer(DirectorCard card)
        {
            InteractableSpawnCard prefab = (InteractableSpawnCard)card.spawnCard;
            return !(prefab.skipSpawnWhenSacrificeArtifactEnabled);
        }

        public static bool SimulacrumTrimmer(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.GetComponent<RoR2.ShrineCombatBehavior>() | prefab.GetComponent<RoR2.OutsideInteractableLocker>() | prefab.GetComponent<RoR2.ShrineBossBehavior>() | prefab.GetComponent<RoR2.SeerStationController>() | prefab.GetComponent<RoR2.PortalStatueBehavior>());
        }

        public static bool TestingPrintCardResults(DirectorCard card)
        {
            Debug.Log(card.spawnCard);
            return true;
        }

        public static bool RemoveMinimumStageCompletionTrimmer(DirectorCard card)
        {
            //Debug.LogWarning(card.minimumStageCompletions);
            if (card.minimumStageCompletions <= 3)
            {
                card.minimumStageCompletions = 0;
            }
            else if (card.minimumStageCompletions >= 4)
            {
                card.minimumStageCompletions = 1;
            }
            return true;
        }

        public static bool KithNoRepeatPredicate(DirectorCard card)
        {

            if (KithNoRepeat.name.Contains("LunarChest"))
            {
                return !(card.spawnCard.name.Contains("LunarChest"));
            }
            else if (KithNoRepeat.name.Contains("Category"))
            {
                string temp = KithNoRepeat.name;
                //temp = temp.Replace("isc", "");
                //temp = temp.Replace("Large", "");
                return !(card.spawnCard.name.Contains(temp));
            }
            else if (KithNoRepeat.name.Contains("Equipment"))
            {

                return !(card.spawnCard.name.Contains("Equipment") && !card.spawnCard.name.Contains("Drone"));
            }


            return !((card.spawnCard.name.Contains("Equipment") && !card.spawnCard.name.Contains("Drone")) | card.spawnCard.name.Contains("LunarChest"));
        }




        public static WeightedSelection<DirectorCard> MixInteractableApplier(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, RoR2.SceneDirector self)
        {
            //Debug.Log("Artifact of Dissimilarity: MixInteractableApplier");

            WeightedSelection<DirectorCard> DissimilarityDirectorCards = new WeightedSelection<DirectorCard>();
            TrimmedmixInteractablesCards.Clear();

            TrimmedmixInteractablesCards.CopyFrom(mixInteractablesCards);

            TrimmedmixInteractablesCards.categories[1].selectionWeight = random.Next(9, 12);
            TrimmedmixInteractablesCards.categories[2].selectionWeight = random.Next(9, 13);
            TrimmedmixInteractablesCards.categories[3].selectionWeight = random.Next(11, 15);
            TrimmedmixInteractablesCards.categories[6].selectionWeight = random.Next(8, 11);




            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Dissimilarity))
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
                {
                    TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(CommandArtifactTrimmer));
                    //Debug.Log("Artifact of Dissimilarity + Command");
                }
                else
                {
                    if (RunArtifactManager.instance.IsArtifactEnabled(ZetDropifact))
                    {
                        TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(NoMoreScrapper));
                        //Debug.Log("Artifact of Dissimilarity + Tossing");
                    }
                    if (RunArtifactManager.instance.IsArtifactEnabled(RiskyConformity))
                    {
                        TrimmedmixInteractablesCards.categories[6].selectionWeight = 0;
                        //Debug.Log("Artifact of Dissimilarity + RiskyConformity");
                    }
                    if (RunArtifactManager.instance.IsArtifactEnabled(Remodeling))
                    {
                        TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(RemodelingPredicate));
                        mixInteractablesCards.categories[6].selectionWeight = 4;
                        //Debug.Log("Artifact of Dissimilarity + Tossing");
                    }
                }
                if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
                {
                    TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(SimulacrumTrimmer));
                    //Debug.Log("Artifact of Kith + Command");
                }
                else if (SceneInfo.instance.sceneDef.baseSceneName == "artifactworld")
                {
                    TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(ArtifactWorldPredicate));
                }
                else if (SceneInfo.instance.sceneDef.baseSceneName == "voidstage")
                {
                    TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(VoidStagePredicate));
                }
                if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Sacrifice))
                {
                    TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(SacrificeArtifactTrimmer));
                    //Debug.Log("Artifact of Kith + Command");
                }

                if (SceneInfo.instance.sceneDef.baseSceneName == "snowyforest" || SceneInfo.instance.sceneDef.baseSceneName == "frozenwall" || SceneInfo.instance.sceneDef.baseSceneName == "itfrozenwall")
                {
                    foreach (DirectorCard directorCard in TrimmedmixInteractablesCards.categories[2].cards)
                    {
                        switch (directorCard.spawnCard.name)
                        {
                            case "iscShrineBlood":
                            case "iscShrineBloodSnowy":
                            case "iscShrineBloodSandy":
                                directorCard.spawnCard = iscShrineBloodSnowy;
                                break;
                            case "iscShrineBoss":
                            case "iscShrineBossSnowy":
                            case "iscShrineBossSandy":
                                directorCard.spawnCard = iscShrineBossSnowy;
                                break;
                            case "iscShrineChance":
                            case "iscShrineChanceSnowy":
                            case "iscShrineChanceSandy":
                                directorCard.spawnCard = iscShrineChanceSnowy;
                                break;
                            case "iscShrineCleanse":
                            case "iscShrineCleanseSnowy":
                            case "iscShrineCleanseSandy":
                                directorCard.spawnCard = iscShrineCleanseSnowy;
                                break;
                            case "iscShrineCombat":
                            case "iscShrineCombatSnowy":
                            case "iscShrineCombatSandy":
                                directorCard.spawnCard = iscShrineCombatSnowy;
                                break;
                            case "iscShrineRestack":
                            case "iscShrineRestackSnowy":
                            case "iscShrineRestackSandy":
                                directorCard.spawnCard = iscShrineRestackSnowy;
                                break;
                        }
                    }
                }
                else if (SceneInfo.instance.sceneDef.baseSceneName == "goolake")
                {
                    foreach (DirectorCard directorCard in TrimmedmixInteractablesCards.categories[2].cards)
                    {
                        switch (directorCard.spawnCard.name)
                        {
                            case "iscShrineBlood":
                            case "iscShrineBloodSnowy":
                            case "iscShrineBloodSandy":
                                directorCard.spawnCard = iscShrineBloodSandy;
                                break;
                            case "iscShrineBoss":
                            case "iscShrineBossSnowy":
                            case "iscShrineBossSandy":
                                directorCard.spawnCard = iscShrineBossSandy;
                                break;
                            case "iscShrineChance":
                            case "iscShrineChanceSnowy":
                            case "iscShrineChanceSandy":
                                directorCard.spawnCard = iscShrineChanceSandy;
                                break;
                            case "iscShrineCleanse":
                            case "iscShrineCleanseSnowy":
                            case "iscShrineCleanseSandy":
                                directorCard.spawnCard = iscShrineCleanseSandy;
                                break;
                            case "iscShrineCombat":
                            case "iscShrineCombatSnowy":
                            case "iscShrineCombatSandy":
                                directorCard.spawnCard = iscShrineCombatSandy;
                                break;
                            case "iscShrineRestack":
                            case "iscShrineRestackSnowy":
                            case "iscShrineRestackSandy":
                                directorCard.spawnCard = iscShrineRestackSandy;
                                break;
                        }
                    }
                }
                else
                {
                    foreach (DirectorCard directorCard in TrimmedmixInteractablesCards.categories[2].cards)
                    {
                        switch (directorCard.spawnCard.name)
                        {
                            case "iscShrineBlood":
                            case "iscShrineBloodSnowy":
                            case "iscShrineBloodSandy":
                                directorCard.spawnCard = iscShrineBlood;
                                break;
                            case "iscShrineBoss":
                            case "iscShrineBossSnowy":
                            case "iscShrineBossSandy":
                                directorCard.spawnCard = iscShrineBoss;
                                break;
                            case "iscShrineChance":
                            case "iscShrineChanceSnowy":
                            case "iscShrineChanceSandy":
                                directorCard.spawnCard = iscShrineChance;
                                break;
                            case "iscShrineCleanse":
                            case "iscShrineCleanseSnowy":
                            case "iscShrineCleanseSandy":
                                directorCard.spawnCard = iscShrineCleanse;
                                break;
                            case "iscShrineCombat":
                            case "iscShrineCombatSnowy":
                            case "iscShrineCombatSandy":
                                directorCard.spawnCard = iscShrineCombat;
                                break;
                            case "iscShrineRestack":
                            case "iscShrineRestackSnowy":
                            case "iscShrineRestackSandy":
                                directorCard.spawnCard = iscShrineRestack;
                                break;
                        }
                    }
                }



                NewTrimmer3Mix(TrimmedmixInteractablesCards);



                DissimilarityDirectorCards = TrimmedmixInteractablesCards.GenerateDirectorCardWeightedSelection();
                Debug.Log("Artifact of Dissimilarity: Generated Trimmed mixInteractables selection");

                if (DebugPrintDissimilarity.Value == true)
                {
                    Debug.Log("__________________________________________________");
                    Debug.Log("Artifact of Dissimilarity: Trimmed Interactable List");
                    Debug.Log("");
                    TrimmedmixInteractablesCards.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(TestingPrintCardResults));
                    Debug.Log("__________________________________________________");
                };

            }
            else
            {
                DissimilarityDirectorCards = ClassicStageInfo.instance.interactableCategories.GenerateDirectorCardWeightedSelection(); //fallback
                Debug.LogWarning("Artifact of Dissimilarity: Failed to generate normal Interactable Categories, using fallback");
            };
            return DissimilarityDirectorCards;
        }

        public static WeightedSelection<DirectorCard> SingleInteractableApplier(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, RoR2.SceneDirector self)
        {
            WeightedSelection<DirectorCard> KithDirectorCards = new WeightedSelection<DirectorCard>();
            if (ClassicStageInfo.instance != null)
            {
                //Debug.Log("Artifact of Kith: SingleInteractableApplier");

                TrimmedSingleInteractableType.Clear();
                TrimmedSingleInteractableType.CopyFrom(ClassicStageInfo.instance.interactableCategories);
                TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(NoMoreRadioTower));
                if (RemoveMinimumStageCompletion.Value == true)
                {
                    TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(RemoveMinimumStageCompletionTrimmer));
                }

                if (RunArtifactManager.instance.IsArtifactEnabled(Kith))
                {
                    if (RunArtifactManager.instance.IsArtifactEnabled(Dissimilarity))
                    {
                        TrimmedSingleInteractableType.Clear();
                        TrimmedSingleInteractableType.CopyFrom(HelperSingleMixInteractable);

                        if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
                        {
                            TrimmedSingleInteractableType.categories[1].selectionWeight = 0;
                            TrimmedSingleInteractableType.categories[2].selectionWeight = random.Next(1, 5);
                            TrimmedSingleInteractableType.categories[3].selectionWeight = 0;
                        }
                        else
                        {
                            TrimmedSingleInteractableType.categories[1].selectionWeight = random.Next(9, 11);
                            TrimmedSingleInteractableType.categories[2].selectionWeight = random.Next(8, 13);
                            TrimmedSingleInteractableType.categories[3].selectionWeight = random.Next(11, 15);
                            TrimmedSingleInteractableType.categories[6].selectionWeight = random.Next(6, 10);
                        }


                        if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
                        {
                            TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(CommandArtifactTrimmer));
                            //Debug.Log("Artifact of Kith + Command");
                        }
                        else
                        {
                            if (RunArtifactManager.instance.IsArtifactEnabled(ZetDropifact))
                            {
                                TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(NoMoreScrapper));
                                //Debug.Log("Artifact of Dissimilarity + Tossing");
                            }
                            if (RunArtifactManager.instance.IsArtifactEnabled(RiskyConformity))
                            {


                                TrimmedmixInteractablesCards.categories[6].selectionWeight = 0;
                                //Debug.Log("Artifact of Dissimilarity + RiskyConformity");
                            }
                            if (RunArtifactManager.instance.IsArtifactEnabled(Remodeling))
                            {
                                TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(RemodelingPredicate));
                                //Debug.Log("Artifact of Dissimilarity + Tossing");
                            }
                        }
                        if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
                        {
                            TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(SimulacrumTrimmer));
                            //Debug.Log("Artifact of Kith + Command");
                        }
                        else if (SceneInfo.instance.sceneDef.baseSceneName == "artifactworld")
                        {
                            TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(ArtifactWorldPredicate));
                        }
                        else if (SceneInfo.instance.sceneDef.baseSceneName == "voidstage")
                        {
                            TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(ArtifactWorldPredicate));
                        }
                        if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Sacrifice))
                        {
                            TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(SacrificeArtifactTrimmer));
                            //Debug.Log("Artifact of Kith + Command");
                        }


                        if (SceneInfo.instance.sceneDef.baseSceneName == "snowyforest" || SceneInfo.instance.sceneDef.baseSceneName == "frozenwall" || SceneInfo.instance.sceneDef.baseSceneName == "itfrozenwall")
                        {
                            foreach (DirectorCard directorCard in TrimmedSingleInteractableType.categories[2].cards)
                            {
                                switch (directorCard.spawnCard.name)
                                {
                                    case "iscShrineBlood":
                                    case "iscShrineBloodSnowy":
                                    case "iscShrineBloodSandy":
                                        directorCard.spawnCard = iscShrineBloodSnowy;
                                        break;
                                    case "iscShrineBoss":
                                    case "iscShrineBossSnowy":
                                    case "iscShrineBossSandy":
                                        directorCard.spawnCard = iscShrineBossSnowy;
                                        break;
                                    case "iscShrineChance":
                                    case "iscShrineChanceSnowy":
                                    case "iscShrineChanceSandy":
                                        directorCard.spawnCard = iscShrineChanceSnowy;
                                        break;
                                    case "iscShrineCleanse":
                                    case "iscShrineCleanseSnowy":
                                    case "iscShrineCleanseSandy":
                                        directorCard.spawnCard = iscShrineCleanseSnowy;
                                        break;
                                    case "iscShrineCombat":
                                    case "iscShrineCombatSnowy":
                                    case "iscShrineCombatSandy":
                                        directorCard.spawnCard = iscShrineCombatSnowy;
                                        break;
                                    case "iscShrineRestack":
                                    case "iscShrineRestackSnowy":
                                    case "iscShrineRestackSandy":
                                        directorCard.spawnCard = iscShrineRestackSnowy;
                                        break;
                                }
                            }
                        }
                        else if (SceneInfo.instance.sceneDef.baseSceneName == "goolake")
                        {
                            foreach (DirectorCard directorCard in TrimmedSingleInteractableType.categories[2].cards)
                            {
                                switch (directorCard.spawnCard.name)
                                {
                                    case "iscShrineBlood":
                                    case "iscShrineBloodSnowy":
                                    case "iscShrineBloodSandy":
                                        directorCard.spawnCard = iscShrineBloodSandy;
                                        break;
                                    case "iscShrineBoss":
                                    case "iscShrineBossSnowy":
                                    case "iscShrineBossSandy":
                                        directorCard.spawnCard = iscShrineBossSandy;
                                        break;
                                    case "iscShrineChance":
                                    case "iscShrineChanceSnowy":
                                    case "iscShrineChanceSandy":
                                        directorCard.spawnCard = iscShrineChanceSandy;
                                        break;
                                    case "iscShrineCleanse":
                                    case "iscShrineCleanseSnowy":
                                    case "iscShrineCleanseSandy":
                                        directorCard.spawnCard = iscShrineCleanseSandy;
                                        break;
                                    case "iscShrineCombat":
                                    case "iscShrineCombatSnowy":
                                    case "iscShrineCombatSandy":
                                        directorCard.spawnCard = iscShrineCombatSandy;
                                        break;
                                    case "iscShrineRestack":
                                    case "iscShrineRestackSnowy":
                                    case "iscShrineRestackSandy":
                                        directorCard.spawnCard = iscShrineRestackSandy;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            foreach (DirectorCard directorCard in TrimmedSingleInteractableType.categories[2].cards)
                            {
                                switch (directorCard.spawnCard.name)
                                {
                                    case "iscShrineBlood":
                                    case "iscShrineBloodSnowy":
                                    case "iscShrineBloodSandy":
                                        directorCard.spawnCard = iscShrineBlood;
                                        break;
                                    case "iscShrineBoss":
                                    case "iscShrineBossSnowy":
                                    case "iscShrineBossSandy":
                                        directorCard.spawnCard = iscShrineBoss;
                                        break;
                                    case "iscShrineChance":
                                    case "iscShrineChanceSnowy":
                                    case "iscShrineChanceSandy":
                                        directorCard.spawnCard = iscShrineChance;
                                        break;
                                    case "iscShrineCleanse":
                                    case "iscShrineCleanseSnowy":
                                    case "iscShrineCleanseSandy":
                                        directorCard.spawnCard = iscShrineCleanse;
                                        break;
                                    case "iscShrineCombat":
                                    case "iscShrineCombatSnowy":
                                    case "iscShrineCombatSandy":
                                        directorCard.spawnCard = iscShrineCombat;
                                        break;
                                    case "iscShrineRestack":
                                    case "iscShrineRestackSnowy":
                                    case "iscShrineRestackSandy":
                                        directorCard.spawnCard = iscShrineRestack;
                                        break;
                                }
                            }
                        }

                        Debug.Log("Artifact of Kith: SingleInteractable using mixInteractables");
                    }



                    if (KithNoRepeat == null)
                    {
                        //Debug.LogWarning("Kith limiter null");
                        KithNoRepeat = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscSquidTurret");
                    }
                    if (KithNoRepeat.name.Contains("Equipment") || KithNoRepeat.name.Contains("LunarChest") || KithNoRepeat.name.Contains("Category") || KithNoRepeat.name.Equals("iscSquidTurret"))
                    {
                        //Debug.Log("Kith limiter " + KithNoRepeat.name);
                        TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(KithNoRepeatPredicate));
                    }

                    NewTrimmer3Single(TrimmedSingleInteractableType);


                    KithDirectorCards = TrimmedSingleInteractableType.GenerateDirectorCardWeightedSelection();
                    Debug.Log("Artifact of Kith: Generated Trimmed SingleInteractable selection");

                    KithNoRepeat = KithDirectorCards.choices[0].value.spawnCard;

                    if (DebugPrintKith.Value == true)
                    {
                        Debug.Log("__________________________________________________");
                        Debug.Log("Artifact of Kith: Trimmed Interactable List");
                        Debug.Log("");
                        TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(TestingPrintCardResults));
                        Debug.Log("__________________________________________________");
                    };

                }
                else
                {
                    KithDirectorCards = ClassicStageInfo.instance.interactableCategories.GenerateDirectorCardWeightedSelection(); //fallback
                    Debug.LogWarning("Artifact of Kith: Failed to generate normal Interactable Categories, using fallback");
                };
            }
            return KithDirectorCards;

        }





        public static void ArtifactCheckerOnStageAwake(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {

            if (NetworkServer.active)
            {
                On.RoR2.SceneDirector.GenerateInteractableCardSelection -= SingleInteractableApplier;
                On.RoR2.SceneDirector.GenerateInteractableCardSelection -= MixInteractableApplier;

                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Dissimilarity) && !RunArtifactManager.instance.IsArtifactEnabled(Kith))
                {
                    On.RoR2.SceneDirector.GenerateInteractableCardSelection += MixInteractableApplier;
                    //Debug.Log("Artifact of Dissimilarity: mixInteractables");
                }
                else if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Kith))
                {
                    On.RoR2.SceneDirector.GenerateInteractableCardSelection += SingleInteractableApplier;
                    //Debug.Log("Artifact of Kith: SingleInteractablePerCategory");
                }





                if (Run.instance.name.Equals("WeeklyRun(Clone)"))
                {
                    if (bossAffixes.Length == 0)
                    {
                        bossAffixes = Run.instance.GetFieldValue<EquipmentIndex[]>("bossAffixes");
                    }

                    Run.instance.SetFieldValue<EquipmentIndex[]>("bossAffixes", bossAffixes);
                }

                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Brigade))
                {
                    EliteKinAsMethod();
                }

            }




            orig(self);
        }




        public static void WanderChecker(On.RoR2.Run.orig_Start orig, Run self)
        {
            //Debug.LogWarning(self);
            //Debug.LogWarning(Run.instance);
            if (NetworkServer.active)
            {
                //Debug.LogWarning(self);
                //Debug.Log("Artifact of Wander Check");
                if (EnableWanderArtifact.Value == true)
                {
                    RuleDef StageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");
                    if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Wander))
                    {
                        if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
                        {
                            Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Normal;
                            Run.instance.startingSceneGroup = sgInfiniteTowerStage1Wander;
                            itgolemplains.destinationsGroup = sgInfiniteTowerStageXGolemPlainsWander;
                            itgoolake.destinationsGroup = sgInfiniteTowerStageXGooLakeWander;
                            itancientloft.destinationsGroup = sgInfiniteTowerStageXAncientLoftWander;
                            itfrozenwall.destinationsGroup = sgInfiniteTowerStageXFrozenwallWander;
                            itdampcave.destinationsGroup = sgInfiniteTowerStageXDampCaveWander;
                            itskymeadow.destinationsGroup = sgInfiniteTowerStageXSkyMeadowWander;
                            itmoon.destinationsGroup = sgInfiniteTowerStageXMoonWander;
                        }
                        else if (Run.instance.name.Equals("WeeklyRun(Clone)"))
                        {
                            Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Normal;
                        }
                        else
                        {
                            Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Random;
                        }
                    }
                }
            }

            orig(self);

            if (NetworkServer.active)
            {

                GameObject tempobj = GameObject.Find("MonsterTeamGainsItemsArtifactInventory(Clone)");
                ArenaInventory = new Inventory();
                if (tempobj)
                {
                    MonsterTeamGainItemRandom = tempobj.GetComponent<RoR2.Inventory>();
                }
                KithNoRepeat = null;


                if (DumbEliteKinThing == false)
                {
                    for (int i = 0; i < EliteCatalog.eliteList.Count; i++)
                    {
                        var tempdef = EliteCatalog.GetEliteDef(EliteCatalog.eliteList[i]);
                        string tempname = tempdef.name;
                        //Debug.LogWarning(tempdef);
                        if (tempname.Contains("Gold") || tempname.Contains("Honor") || tempname.Contains("SecretSpeed") || tempname.Contains("Echo") || tempname.Contains("Yellow") || tempname.Contains("Lunar"))
                        {
                        }
                        else if (tempname.Contains("Poison") || tempname.Contains("Haunted") || tempname.Contains("ImpPlane") || tempname.Contains("Blighted") || tempname.Contains("Void"))
                        {
                            EliteDefsTier2.Add(tempdef);
                        }
                        else
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


            //DumbItemThing = 1;
            //DumbItemThingMonster = 1;
            //DumbEquipmentThing = 1;

        }


        public static IEnumerator DelayedRespawn(PlayerCharacterMasterController playerCharacterMasterController, float delay)
        {
            yield return new WaitForSeconds(delay);
            CharacterBody temp = playerCharacterMasterController.master.GetBody();
            if (temp)
            {
                playerCharacterMasterController.master.Respawn(temp.footPosition, temp.transform.rotation);
            }
            yield break;
        }

        public static void WanderEnder(On.RoR2.Run.orig_OnDisable orig, Run self)
        {
            //Debug.LogWarning("Run end");
            RuleDef StageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Wander))
            {
                if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
                {
                    InfiniteTowerRunBase.startingSceneGroup = sgInfiniteTowerStage1;
                    itgolemplains.destinationsGroup = sgInfiniteTowerStageXGolemPlains;
                    itgoolake.destinationsGroup = sgInfiniteTowerStageXGooLake;
                    itancientloft.destinationsGroup = sgInfiniteTowerStageXAncientLoft;
                    itfrozenwall.destinationsGroup = sgInfiniteTowerStageXFrozenwall;
                    itdampcave.destinationsGroup = sgInfiniteTowerStageXDampCave;
                    itskymeadow.destinationsGroup = sgInfiniteTowerStageXSkyMeadow;
                    itmoon.destinationsGroup = sgInfiniteTowerStageXMoon;
                }

                else if (Run.instance.name.Equals("WeeklyRun(Clone)"))
                {
                    Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Random;
                }
                else
                {
                    Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Normal;
                }
            }
            orig(self);
        }

        public static void WanderCheckerMidRun(On.RoR2.Stage.orig_Start orig, RoR2.Stage self)
        {

            if (NetworkServer.active)
            {
                if (EnableWanderArtifact.Value == true)
                {
                    if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Wander))
                    {

                        if (Run.instance.name.Equals("ClassicRun(Clone)"))
                        {
                            if (SceneInfo.instance.countsAsStage == false)
                            {
                                //Debug.LogWarning("Hidden Realm - Manual Random Stage");


                                //Run.instance.nextStageScene = sceneNamesForWanderList[random.Next(0, sceneNamesForWanderList.Count)];
                            }
                            //Debug.Log("Next Stage will be random");
                        }
                    }
                }
            }


            orig(self);
            //Debug.LogWarning("Artifact of Wander Check");

            if (NetworkServer.active)
            {

                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Brigade))
                {
                    if (!SceneInfo.instance) { return; }
                  
                    string token = "<style=cWorldEvent>All elite combatants will be ";
                    string token2 = Language.GetString(TempForUsageEliteDef.modifierToken);
                    token2 = token2.Replace("{0}", "");
                    token += token2 + "</style>";
                    self.StartCoroutine(DelayedChatMessage(token, 1.5f));
                }

                do
                {
                    tempartifact = ArtifactCatalog.GetArtifactDef((ArtifactIndex)random.Next(ArtifactCatalog.artifactCount));
                    //Debug.LogWarning(temp);
                    RoR2.ArtifactTrialMissionController.trialArtifact = tempartifact;
                } while (tempartifact.unlockableDef == NoMoreArtifact);


                //Debug.LogWarning(Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData);

            }

        }

        public static void OneTimeSceneDic(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            for (int i = 0; i < ArtifactCatalog.artifactCount; i++)
            {
                ArtifactDef temp = ArtifactCatalog.GetArtifactDef((ArtifactIndex)i);
                if (temp.pickupModelPrefab == null)
                {
                    temp.pickupModelPrefab = RoR2Content.Artifacts.EliteOnly.pickupModelPrefab;
                }
                PickupDef temppickup = PickupCatalog.FindPickupIndex((ArtifactIndex)i).pickupDef;
                if (temppickup.displayPrefab == null)
                {
                    temppickup.displayPrefab = RoR2Content.Artifacts.EliteOnly.pickupModelPrefab;
                }

            }


            On.RoR2.SceneDirector.Start -= OneTimeSceneDic;
        }



        private static void RandomizeEquipment(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (NetworkServer.active)
            {

                if (DumbEquipmentThing == false)
                {
                    //Debug.Log("Equipment Lister: " + EquipmentCatalog.equipmentCount + " Items");


                    DumbEquipmentThing = true;

                    //Inventory inv = new RoR2.Inventory();
                    //inv.WriteItemStacks(invoutput);
                    TotalEquipmentCount = EquipmentCatalog.equipmentCount;
                    int[] invoutput = new int[TotalEquipmentCount];

                    FullEquipmentList.Clear();
                    FullEquipmentList.AddRange(EquipmentCatalog.equipmentList);

                    for (var i = 0; i < invoutput.Length; i++)
                    {

                        string tempname = EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i).name;


                        if (tempname.StartsWith("Elite"))
                        {
                            if (tempname.Contains("Gold") || tempname.Contains("Echo") || tempname.Contains("Yellow") || tempname.Contains("SecretSpeed"))
                            {
                            }
                            else
                            {

                                EliteEquipmentList.Add((EquipmentIndex)i);
                            }
                        }


                    }

                }


                if (RerollEquipmentDrones.Value == true)
                {
                    foreach (var gameObj in FindObjectsOfType(typeof(CharacterMaster)) as CharacterMaster[])
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

                foreach (var playerController in PlayerCharacterMasterController.instances)
                {
                    Inventory inv = playerController.master.inventory;

                    TempTrimOrangeEquipmentList.AddRange(Run.instance.availableEquipmentDropList);
                    TempTrimBlueEquipmentList.AddRange(Run.instance.availableLunarEquipmentDropList);
                    //TempTrimYellowEquipmentList.AddRange(YellowEquipmentList);
                    TempTrimEliteEquipmentList.AddRange(EliteEquipmentList);



                    if (inv == null) { return; }

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
                                string tempname2 = EquipmentCatalog.GetEquipmentDef(inv.currentEquipmentIndex).name;
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

                    TempTrimOrangeEquipmentList.Clear();
                    TempTrimBlueEquipmentList.Clear();
                    TempTrimEliteEquipmentList.Clear();

                }

            }
        }


        private static void RandomizeItems(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
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

                //inv.RemoveItem(RoR2Content.Items.TonicAffliction, inv.GetItemCount(RoR2Content.Items.TonicAffliction));


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

                TempTrimWhiteItemList.Clear();
                TempTrimGreenItemList.Clear();
                TempTrimRedItemList.Clear();
                TempTrimYellowItemList.Clear();
                TempTrimBlueItemList.Clear();
                TempTrimPinkT1ItemList.Clear();
                TempTrimPinkT2ItemList.Clear();
                TempTrimPinkT3ItemList.Clear();
                TempTrimPinkBossItemList.Clear();


            }

        }


        private static void RandomizeHeresyItems(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            foreach (var playerController in PlayerCharacterMasterController.instances)
            {
                Inventory inv = playerController.master.inventory;

                List<ItemDef> HeresyItemList = new List<ItemDef>() { 
                    RoR2Content.Items.LunarPrimaryReplacement,
                    RoR2Content.Items.LunarSecondaryReplacement,
                    RoR2Content.Items.LunarUtilityReplacement,
                    RoR2Content.Items.LunarSpecialReplacement
                };
                List<int> HeresyItemCounts = new List<int>();

                for (int i = 0; i < HeresyItemList.Count; i++)
                {
                    int count = inv.GetItemCount(HeresyItemList[i]);
                    HeresyItemCounts.Add(count);
                    inv.RemoveItem(HeresyItemList[i], count);
                }

                for (int i = 0; i < 4; i++)
                {
                    int randomItem = random.Next(HeresyItemList.Count);
                    int randomCount = random.Next(HeresyItemCounts.Count);

                    inv.GiveItem(HeresyItemList[randomItem], HeresyItemCounts[randomCount]);
                    HeresyItemList.Remove(HeresyItemList[randomItem]);
                    HeresyItemCounts.Remove(HeresyItemCounts[randomCount]);
                }
            }

        }



        private static void RandomizeItemsMonster(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {

            if (!NetworkServer.active) { return; }
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


                SimulacrumWhiteItemList.Clear();
                SimulacrumGreenItemList.Clear();
                SimulacrumRedItemList.Clear();
                SimulacrumBlueItemList.Clear();
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

                TempTrimWhiteItemList.AddRange(SimulacrumWhiteItemList);
                TempTrimGreenItemList.AddRange(SimulacrumGreenItemList);
                TempTrimRedItemList.AddRange(SimulacrumRedItemList);
                TempTrimBlueItemList.AddRange(SimulacrumBlueItemList);


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

                TempTrimWhiteItemList.Clear();
                TempTrimGreenItemList.Clear();
                TempTrimRedItemList.Clear();
                TempTrimBlueItemList.Clear();
                WhiteItemCountList.Clear();
                GreenItemCountList.Clear();
                RedItemCountList.Clear();
                BlueItemCountList.Clear();
            }
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(MonsterTeamGain))
            {
                Inventory MonsterTeamGainItemInventory = RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.monsterTeamInventory;
                int[] invoutput = new int[MonsterTeamGainItemInventory.itemStacks.Length];
                MonsterTeamGainItemInventory.itemStacks.CopyTo(invoutput, 0);



                int WhiteCount = 0;
                int GreenCount = 0;
                int RedCount = 0;
                int BlueCount = 0;

                //BasicPickupDropTable


                List<ItemTag> BannedItemTagsSimu = dtMonsterTeamTier2Item.bannedItemTags.ToList();
                //Debug.LogWarning(BannedItemTagsSimu.Count);

                MonsterWhiteItemList.Clear();
                MonsterGreenItemList.Clear();
                MonsterRedItemList.Clear();
                MonsterBlueItemList.Clear();
                MonsterWhiteItemList.AddRange(Run.instance.availableTier1DropList);
                MonsterGreenItemList.AddRange(Run.instance.availableTier2DropList);
                MonsterRedItemList.AddRange(Run.instance.availableTier3DropList);
                MonsterBlueItemList.AddRange(Run.instance.availableLunarItemDropList);


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

                TempTrimWhiteItemList.AddRange(MonsterWhiteItemList);
                TempTrimGreenItemList.AddRange(MonsterGreenItemList);
                TempTrimRedItemList.AddRange(MonsterRedItemList);
                TempTrimBlueItemList.AddRange(MonsterBlueItemList);


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

                TempTrimWhiteItemList.Clear();
                TempTrimGreenItemList.Clear();
                TempTrimRedItemList.Clear();
                TempTrimBlueItemList.Clear();

                WhiteItemCountList.Clear();
                GreenItemCountList.Clear();
                RedItemCountList.Clear();
                BlueItemCountList.Clear();



            }
            orig(self);
        }


        public static void RecalcOnDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, global::RoR2.HealthComponent self, global::RoR2.DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            if (!NetworkServer.active) { return; }
            //self.body.RecalculateStats();
            self.body.MarkAllStatsDirty();
            //Debug.LogWarning(self);
        }

        public static void RecalcOnShield(On.RoR2.HealthComponent.orig_ServerFixedUpdate orig, global::RoR2.HealthComponent self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            if (self.body.outOfDanger == true && self.shield < self.fullShield)
            {
                self.body.RecalculateStats();
            }
        }

        public static void RecalcOnLand(On.RoR2.CharacterMotor.orig_OnLanded orig, global::RoR2.CharacterMotor self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            //self.gameObject.GetComponent<CharacterBody>().RecalculateStats();
            self.gameObject.GetComponent<CharacterBody>().MarkAllStatsDirty();
            //Debug.LogWarning(self);
        }

        public static void RecalcOnSkill(On.RoR2.GenericSkill.orig_OnExecute orig, global::RoR2.GenericSkill self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            self.characterBody.RecalculateStats();
        }

        public static void RecalcOnHeal(On.RoR2.HealthComponent.orig_SendHeal orig, GameObject target, float amount, bool isCrit)
        {
            orig(target, amount, isCrit);
            if (!NetworkServer.active) { return; }
            //target.GetComponent<CharacterBody>().RecalculateStats();
            target.GetComponent<CharacterBody>().MarkAllStatsDirty();
            //Debug.LogWarning(target);
        }

        public static void RecalcStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, global::RoR2.CharacterBody self)
        {


            if (!self.master | !self.healthComponent | !self.skillLocator) { orig(self); return; }
            float tempfrac = self.healthComponent.combinedHealthFraction;
            if (tempfrac > 1 || tempfrac < 0) { tempfrac = 1; }

            CharacterBody tempmaster = self.master.bodyPrefab.GetComponent<CharacterBody>();


            if (self.isPlayerControlled == true)
            {
                self.baseMoveSpeed = ((1 - tempfrac) * SpiritSpeedVal + 1) * tempmaster.baseMoveSpeed;
                self.baseAcceleration = ((1 - tempfrac) * SpiritSpeedVal + 1) * tempmaster.baseAcceleration;
                self.baseJumpPower = ((1 - tempfrac) * SpiritJumpPlayerVal + 1) * tempmaster.baseJumpPower;

                self.baseDamage = (tempfrac * SpiritDamagePlayerVal + 1 - SpiritDamagePlayerVal) * tempmaster.baseDamage;
                self.levelDamage = (tempfrac * SpiritDamagePlayerVal + 1 - SpiritDamagePlayerVal) * tempmaster.levelDamage;
                self.baseAttackSpeed = ((1 - tempfrac) * SpiritAttackSpeedPlayerVal + 1) * tempmaster.baseAttackSpeed;
            }
            else if (self.name.Equals("BrotherBody(Clone)"))
            {
                self.baseMoveSpeed = ((1 - tempfrac) * SpiritSpeedHalfVal + 1) * tempmaster.baseMoveSpeed;
                self.baseAcceleration = ((1 - tempfrac) * SpiritSpeedHalfVal + 1) * tempmaster.baseAcceleration;
                self.baseDamage = (tempfrac * SpiritDamagePlayerVal + 1 - SpiritDamagePlayerVal) * tempmaster.baseDamage;
                self.baseAttackSpeed = ((1 - tempfrac) * SpiritAttackSpeedVal + 1) * tempmaster.baseAttackSpeed;
                self.levelDamage = (tempfrac * SpiritDamagePlayerVal + 1 - SpiritDamagePlayerVal) * tempmaster.levelDamage;
            }
            else
            {
                self.baseMoveSpeed = ((1 - tempfrac) * SpiritSpeedVal + 1) * tempmaster.baseMoveSpeed;
                self.baseAcceleration = ((1 - tempfrac) * SpiritSpeedVal + 1) * tempmaster.baseAcceleration;
                self.baseJumpPower = ((1 - tempfrac) * SpiritJumpVal + 1) * tempmaster.baseJumpPower;

                self.baseDamage = (tempfrac * SpiritDamageVal + 1 - SpiritDamageVal) * tempmaster.baseDamage;
                self.levelDamage = (tempfrac * SpiritDamageVal + 1 - SpiritDamageVal) * tempmaster.levelDamage;
                self.baseAttackSpeed = ((1 - tempfrac) * SpiritAttackSpeedVal + 1) * tempmaster.baseAttackSpeed;
            }

            //self.PerformAutoCalculateLevelStats();
            orig(self);

            if (self.skillLocator.primary)
            {
                self.skillLocator.primary.cooldownScale *= (tempfrac * SpiritCooldownVal + 1 - SpiritCooldownVal);
            }
            if (self.skillLocator.secondary)
            {
                self.skillLocator.secondary.cooldownScale *= (tempfrac * SpiritCooldownVal + 1 - SpiritCooldownVal);
            }
            if (self.skillLocator.utility)
            {
                if (self.name.Equals("VagrantBody(Clone)") || self.name.Equals("GrandParentBody(Clone)")) { tempfrac *= 0.5f; }
                self.skillLocator.utility.cooldownScale *= (tempfrac * SpiritCooldownVal + 1 - SpiritCooldownVal);
            }
            if (self.skillLocator.special)
            {
                if (self.name.Equals("BrotherBody(Clone)") || self.name.Equals("SuperRoboBallBossBody(Clone)")) { return; }
                self.skillLocator.special.cooldownScale *= (tempfrac * SpiritCooldownVal + 1 - SpiritCooldownVal);
            }



        }

        public static void RecalcStun(On.EntityStates.StunState.orig_OnEnter orig, global::EntityStates.StunState self)
        {
            self.stunDuration = 0f;
            orig(self);
        }
        public static void RecalcProjectileSpeed(On.RoR2.Projectile.ProjectileController.orig_Start orig, global::RoR2.Projectile.ProjectileController self)
        {
            orig(self);
            if (self.owner == null) { return; }

            var temp = self.gameObject.GetComponent<RoR2.Projectile.ProjectileSimple>();
            if (temp == null) { return; }
            var temphp = self.owner.GetComponent<CharacterBody>().healthComponent.combinedHealthFraction;
            if (temphp > 1 || temphp < 0) { temphp = 1; }
            temp.desiredForwardSpeed *= (SpiritProjectileSpeedPlusVal - temphp * SpiritProjectileSpeedVal);
            //Debug.LogWarning(self);
        }

        public static void ArtifactAdded()
        {


            //Texture garbage
            Texture2D DissArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D DissArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D KithArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D KithArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D WanderArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D WanderArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D RemodelingArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D RemodelingArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            //Texture2D RetoolingArtifactOn = new Texture2D(64, 64, TextureFormat.DXT5, false);
            //Texture2D RetoolingArtifactOff = new Texture2D(64, 64, TextureFormat.DXT5, false);
            Texture2D SpiritingArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D SpiritingArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D BrigadeArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D BrigadeArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D TransposeArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D TransposeArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);

            DissArtifactOn.filterMode = FilterMode.Trilinear;
            DissArtifactOff.filterMode = FilterMode.Trilinear;
            KithArtifactOn.filterMode = FilterMode.Trilinear;
            KithArtifactOff.filterMode = FilterMode.Trilinear;
            WanderArtifactOn.filterMode = FilterMode.Trilinear;
            WanderArtifactOff.filterMode = FilterMode.Trilinear;
            RemodelingArtifactOn.filterMode = FilterMode.Trilinear;
            RemodelingArtifactOff.filterMode = FilterMode.Trilinear;
            //RetoolingArtifactOn.filterMode = FilterMode.Trilinear;
            //RetoolingArtifactOff.filterMode = FilterMode.Trilinear;
            SpiritingArtifactOn.filterMode = FilterMode.Trilinear;
            SpiritingArtifactOff.filterMode = FilterMode.Trilinear;
            BrigadeArtifactOn.filterMode = FilterMode.Trilinear;
            BrigadeArtifactOff.filterMode = FilterMode.Trilinear;
            TransposeArtifactOn.filterMode = FilterMode.Trilinear;
            TransposeArtifactOff.filterMode = FilterMode.Trilinear;


            DissArtifactOn.LoadImage(Properties.Resources.Dissimilarity_on, false);
            DissArtifactOff.LoadImage(Properties.Resources.Dissimilarity_off, false);
            KithArtifactOn.LoadImage(Properties.Resources.Kith_on, false);
            KithArtifactOff.LoadImage(Properties.Resources.Kith_off, false);
            WanderArtifactOn.LoadImage(Properties.Resources.Wander_on, false);
            WanderArtifactOff.LoadImage(Properties.Resources.Wander_off, false);
            RemodelingArtifactOn.LoadImage(Properties.Resources.Remodeling_on, false);
            RemodelingArtifactOff.LoadImage(Properties.Resources.Remodeling_off, false);
            //RetoolingArtifactOn.LoadImage(Properties.Resources.Retooling_on, false);
            //RetoolingArtifactOff.LoadImage(Properties.Resources.Retooling_off, false);
            SpiritingArtifactOn.LoadImage(Properties.Resources.Spiriting_on, false);
            SpiritingArtifactOff.LoadImage(Properties.Resources.Spiriting_off, false);
            BrigadeArtifactOn.LoadImage(Properties.Resources.Briaged_on, false);
            BrigadeArtifactOff.LoadImage(Properties.Resources.Brigade_off, false);
            TransposeArtifactOn.LoadImage(Properties.Resources.Transpose_on, false);
            TransposeArtifactOff.LoadImage(Properties.Resources.Transpose_off, false);

            Rect rec = new Rect(0, 0, DissArtifactOn.width, DissArtifactOn.height);
            Sprite DisimOn = Sprite.Create(DissArtifactOn, rec, new Vector2(0, 0));
            Sprite DisimOff = Sprite.Create(DissArtifactOff, rec, new Vector2(0, 0));
            Sprite KithOn = Sprite.Create(KithArtifactOn, rec, new Vector2(0, 0));
            Sprite KithOff = Sprite.Create(KithArtifactOff, rec, new Vector2(0, 0));
            Sprite WanderOn = Sprite.Create(WanderArtifactOn, rec, new Vector2(0, 0));
            Sprite WanderOff = Sprite.Create(WanderArtifactOff, rec, new Vector2(0, 0));
            Sprite RemodelingOn = Sprite.Create(RemodelingArtifactOn, rec, new Vector2(0, 0));
            Sprite RemodelingOff = Sprite.Create(RemodelingArtifactOff, rec, new Vector2(0, 0));
            //Sprite RetoolingOn = Sprite.Create(RetoolingArtifactOn, rec, new Vector2(0, 0));
            //Sprite RetoolingOff = Sprite.Create(RetoolingArtifactOff, rec, new Vector2(0, 0));
            Sprite SpiritingOn = Sprite.Create(SpiritingArtifactOn, rec, new Vector2(0, 0));
            Sprite SpiritingOff = Sprite.Create(SpiritingArtifactOff, rec, new Vector2(0, 0));
            Sprite BrigadeOn = Sprite.Create(BrigadeArtifactOn, rec, new Vector2(0, 0));
            Sprite BrigadeOff = Sprite.Create(BrigadeArtifactOff, rec, new Vector2(0, 0));
            Sprite TransposeOn = Sprite.Create(TransposeArtifactOn, rec, new Vector2(0, 0));
            Sprite TransposeOff = Sprite.Create(TransposeArtifactOff, rec, new Vector2(0, 0));



            //
            /*
            Empty = 11;
            Square = 7;
            Circle = 1;
            Triangle = 3;
            Diamond = 5;
            
            0 1 2
            3 4 5
            6 7 8
            */
            //

            //Debug.Log("Loading Artifact of Dissimilarity");
            Dissimilarity.nameToken = "Artifact of Dissimilarity";
            Dissimilarity.descriptionToken = "Interactables can appear outside their usual environments.";
            Dissimilarity.smallIconSelectedSprite = DisimOn;
            Dissimilarity.smallIconDeselectedSprite = DisimOff;
            Dissimilarity.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MixEnemy").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Dissimilarity);
            if (EnableDissim.Value == false)
            {
                Dissimilarity.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //DissimCode.ArtifactCompounds = new List<int> { 1, 1, 5, 1, 1, 1, 5, 1, 1 };
                DissimCode.topRow = new Vector3Int(1, 1, 5);
                DissimCode.topRow = new Vector3Int(1, 1, 1);
                DissimCode.topRow = new Vector3Int(5, 1, 1);
                ArtifactCodeAPI.AddCode(Dissimilarity, DissimCode);
            }

            //Debug.Log("Loading Artifact of Kith");
            Kith.nameToken = "Artifact of Kith";
            Kith.descriptionToken = "Each Interactable category will only contain one entry per stage.";
            Kith.smallIconSelectedSprite = KithOn;
            Kith.smallIconDeselectedSprite = KithOff;
            Kith.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/SingleMonsterType").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Kith);
            if (EnableKith.Value == false)
            {
                Kith.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //KithCode.ArtifactCompounds = new List<int> { 3, 5, 3, 3, 5, 3, 1, 1, 1 };
                KithCode.topRow = new Vector3Int(3, 5, 3);
                KithCode.topRow = new Vector3Int(3, 5, 3);
                KithCode.topRow = new Vector3Int(1, 1, 1);
                ArtifactCodeAPI.AddCode(Kith, KithCode);
            }

            //Debug.Log("Loading Artifact of Wander");
            Wander.nameToken = "Artifact of Wander";
            //Wander.descriptionToken = "Stages progress in a random order.";
            //Wander.descriptionToken = "Stages progress in a random order.\nSimulacrum/Prismatic: Start in a random stage, normal stage order.";
            //Wander.descriptionToken = "Stages progress in a random order.\nAlt Gamemodes: normal stage order.";
            Wander.descriptionToken = "Stages progress in a random order.\nSimulacrum: stages are in order.";
            Wander.smallIconSelectedSprite = WanderOn;
            Wander.smallIconDeselectedSprite = WanderOff;
            Wander.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Wander);
            if (EnableWanderArtifact.Value == false)
            {
                Wander.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //WanderCode.ArtifactCompounds = new List<int> { 3, 7, 3, 7, 7, 7, 3, 7, 3 };
                WanderCode.topRow = new Vector3Int(3, 7, 3);
                WanderCode.topRow = new Vector3Int(7, 7, 7);
                WanderCode.topRow = new Vector3Int(3, 7, 3);
                ArtifactCodeAPI.AddCode(Wander, WanderCode);
            }


            On.RoR2.Stage.Start += WanderCheckerMidRun;
            On.RoR2.Run.Start += WanderChecker;
            On.RoR2.Run.OnDisable += WanderEnder;
            On.RoR2.SceneDirector.PlaceTeleporter += WanderLunarTeleporter;

            //Debug.Log("Loading Artifact of Remodeling");
            Remodeling.nameToken = "Artifact of Remodeling";
            Remodeling.descriptionToken = "Reroll all passive items and equipments each stage.";
            Remodeling.smallIconSelectedSprite = RemodelingOn;
            Remodeling.smallIconDeselectedSprite = RemodelingOff;
            Remodeling.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/RandomSurvivorOnRespawn").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Remodeling);



            if (EnableRemodelArtifact.Value == false)
            {
                Remodeling.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //RemodelCode.ArtifactCompounds = new List<int> { 1, 7, 1, 1, 5, 1, 1, 7, 1 };
                RemodelCode.topRow = new Vector3Int(1, 7, 1);
                RemodelCode.topRow = new Vector3Int(1, 5, 1);
                RemodelCode.topRow = new Vector3Int(1, 7, 1);
                ArtifactCodeAPI.AddCode(Remodeling, RemodelCode);
            }

            /*
            Debug.Log("Loading Artifact of Retooling");
            Retooling.nameToken = "Artifact of Retooling";
            Retooling.descriptionToken = "Reroll your equipments each stage.";
            Retooling.smallIconSelectedSprite = RetoolingOn;
            Retooling.smallIconDeselectedSprite = RetoolingOff;
            Retooling.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Retooling);


            if (EnableRetoolingArtifact.Value == false)
            {
                Retooling.unlockableDef = NoMoreArtifact;
            }
            else
            {
                RetoolingCode.ArtifactCompounds = new List<int> { 7, 1, 1, 5, 1, 5, 3, 7, 7 };
                ArtifactCodeAPI.AddCode(Retooling, RetoolingCode);
            }
            */



            //Debug.Log("Loading Artifact of Spiriting");
            Spiriting.nameToken = "Artifact of Spiriting";
            Spiriting.descriptionToken = "All characters move and attack faster the lower their health gets.";
            Spiriting.smallIconSelectedSprite = SpiritingOn;
            Spiriting.smallIconDeselectedSprite = SpiritingOff;
            Spiriting.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/TeamDeath").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Spiriting);

            SpiritSpeedVal = SpiritMovement.Value;
            SpiritSpeedHalfVal = SpiritMovement.Value * 0.75f;
            SpiritAttackSpeedVal = SpiritAttackSpeed.Value;
            SpiritAttackSpeedPlayerVal = SpiritAttackSpeedPlayer.Value;
            SpiritJumpVal = SpiritJump.Value;
            SpiritJumpPlayerVal = SpiritJumpPlayer.Value;
            SpiritCooldownVal = SpiritCooldown.Value;
            SpiritDamageVal = SpiritDamage.Value;
            SpiritDamagePlayerVal = SpiritDamagePlayer.Value;
            SpiritProjectileSpeedVal = SpiritProjectileSpeed.Value;
            SpiritProjectileSpeedPlusVal = SpiritProjectileSpeed.Value + 1;
            if (EnableSpiritualArtifact.Value == false)
            {
                Spiriting.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //SpiritingCode.ArtifactCompounds = new List<int> { 5, 3, 5, 1, 3, 1, 5, 3, 5 };
                SpiritingCode.topRow = new Vector3Int(5, 3, 5);
                SpiritingCode.topRow = new Vector3Int(1, 3, 1);
                SpiritingCode.topRow = new Vector3Int(5, 3, 5);
                ArtifactCodeAPI.AddCode(Spiriting, SpiritingCode);
            }


            //Debug.Log("Loading Artifact of Brigade");
            Brigade.nameToken = "Artifact of Brigade";
            Brigade.descriptionToken = "All elites will be the same type per stage.";
            Brigade.smallIconSelectedSprite = BrigadeOn;
            Brigade.smallIconDeselectedSprite = BrigadeOff;
            Brigade.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MonsterTeamGainsItems").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Brigade);
            if (EnableBrigadeArtifact.Value == false)
            {
                Brigade.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //BrigadeCode.ArtifactCompounds = new List<int> { 7, 1, 1, 5, 1, 5, 3, 7, 7 };
                //ArtifactCodeAPI.AddCode(Brigade, BrigadeCode);

            }

            //Debug.Log("Loading Artifact of Transpose");
            Transpose.nameToken = "Artifact of Transpose";
            Transpose.descriptionToken = "Get a randomized skill loadout every stage.";
            Transpose.smallIconSelectedSprite = TransposeOn;
            Transpose.smallIconDeselectedSprite = TransposeOff;
            Transpose.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/RandomSurvivorOnRespawn").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Transpose);
            if (EnableTransposeArtifact.Value == false)
            {
                Transpose.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //TransposeCode.ArtifactCompounds = new List<int> { 7, 1, 1, 5, 1, 5, 3, 7, 7 };
                //ArtifactCodeAPI.AddCode(Transpose, TransposeCode);

            }






            //Simu Stuff
            GameObject InfiniteTowerWaveArtifactSingleEliteType = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactSingleEliteType", true);
            GameObject InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacSingleEliteTypeDisabledPrerequisite = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();
            //ArtifactDef ArtifactDefMonsterTeamGainsItems = LegacyResourcesAPI.Load<ArtifactDef>("ArtifactDefs/MonsterTeamGainsItems");

            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<ArtifactEnabler>().artifactDef = Brigade;
            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI;
            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<CombatDirector>().eliteBias *= 2;

            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Brigade.smallIconSelectedSprite;
            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "Wave {0} - Augment of Brigade";
            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "All elites will be of the same type.";

            ArtifacSingleEliteTypeDisabledPrerequisite.bannedArtifact = Brigade;
            ArtifacSingleEliteTypeDisabledPrerequisite.name = "ArtifacSingleEliteTypeDisabledPrerequisite";

            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactSingleEliteType = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifactSingleEliteType, weight = 2, prerequisites = ArtifacSingleEliteTypeDisabledPrerequisite };



            GameObject InfiniteTowerWaveArtifactRandomLoadout = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactRandomLoadout", true);
            GameObject InfiniteTowerCurrentArtifactRandomLoadoutWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactRandomLoadoutWaveUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacRandomLoadoutDisabledPrerequisite = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();
            //ArtifactDef ArtifactDefMonsterTeamGainsItems = LegacyResourcesAPI.Load<ArtifactDef>("ArtifactDefs/MonsterTeamGainsItems");

            InfiniteTowerWaveArtifactRandomLoadout.GetComponent<ArtifactEnabler>().artifactDef = Transpose;
            InfiniteTowerWaveArtifactRandomLoadout.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerCurrentArtifactRandomLoadoutWaveUI;
            InfiniteTowerWaveArtifactRandomLoadout.GetComponent<InfiniteTowerWaveController>().baseCredits = 230;

            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Transpose.smallIconSelectedSprite;
            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "Wave {0} - Augment of Transpose";
            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "Use a random loadout for this wave.";

            ArtifacRandomLoadoutDisabledPrerequisite.bannedArtifact = Transpose;
            ArtifacRandomLoadoutDisabledPrerequisite.name = "ArtifacRandomLoadoutDisabledPrerequisite";

            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactRandomLoadout = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifactRandomLoadout, weight = 0.5f, prerequisites = ArtifacRandomLoadoutDisabledPrerequisite };



            RoR2.InfiniteTowerWaveCategory ITBasicWaves = Addressables.LoadAssetAsync<RoR2.InfiniteTowerWaveCategory>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveCategories/CommonWaveCategory.asset").WaitForCompletion();
            ITBasicWaves.wavePrefabs = ITBasicWaves.wavePrefabs.Add(ITBasicArtifactSingleEliteType, ITBasicArtifactRandomLoadout);




        }


        public static void RandomizeLoadoutRespawn(On.RoR2.Stage.orig_RespawnCharacter orig, global::RoR2.Stage self, global::RoR2.CharacterMaster characterMaster)
        {
            orig(self, characterMaster);

            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Transpose))
                {
                    var tempbod = characterMaster.GetBody();
                    Loadout newloadout = new Loadout();
                    newloadout.Copy(characterMaster.loadout);
                    int globalrepeat = 0;
                    int repeat = 0;
                    do
                    {
                        if (repeat == tempbod.skillLocator.skillSlotCount)
                        {
                            globalrepeat++;
                            Debug.Log("Repeat Loadout " + globalrepeat + ", rerolling again");
                        }
                        repeat = 0;
                        for (int i = 0; i < tempbod.skillLocator.skillSlotCount; i++)
                        {
                            var tempgenericskill = tempbod.skillLocator.GetSkillAtIndex(i);
                            uint skillVariant = (uint)UnityEngine.Random.Range(0, tempgenericskill.skillFamily.variants.Length);
                            newloadout.bodyLoadoutManager.SetSkillVariant(tempbod.bodyIndex, i, skillVariant);

                            string tempSkillOldName = tempgenericskill.skillDef.ToString();
                            string tempSkillOldType = tempgenericskill.skillDef.GetType().ToString();
                            tempSkillOldName = tempSkillOldName.Replace(" (" + tempSkillOldType + ")", "");
                            var tempSkillindexOld = tempgenericskill.skillFamily.GetVariantIndex(tempSkillOldName);
                            var tempSkillindexNew = newloadout.bodyLoadoutManager.GetSkillVariant(tempbod.bodyIndex, i);

                            if (tempSkillindexNew == tempSkillindexOld)
                            {
                                repeat++;
                            }
                            characterMaster.loadout.bodyLoadoutManager.SetSkillVariant(tempbod.bodyIndex, i, skillVariant);


                            //Debug.LogWarning(tempgenericskill.skillFamily + " " + tempgenericskill.skillFamily.variants.Length + " different variants");
                            //Debug.LogWarning("slot " + i + ": variant " + skillVariant);
                            //Debug.LogWarning("Old Index " + tempSkillindexOld);
                            //Debug.LogWarning("New Index " + tempSkillindexNew);
                            //Debug.LogWarning("Repeats "+ repeat);
                            //Debug.LogWarning(tempbod.skillLocator.skillSlotCount);
                        }

                    } while (repeat == tempbod.skillLocator.skillSlotCount && globalrepeat < 2);

                    tempbod.SetLoadoutServer(newloadout);

                    characterMaster.loadout.bodyLoadoutManager.SetSkinIndex(tempbod.bodyIndex, (uint)UnityEngine.Random.Range(0, RoR2.SkinCatalog.GetBodySkinCount(tempbod.bodyIndex)));
                    Debug.Log("Rerolled " + tempbod.name + "'s Loadout ");
                }
            }

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
                }


                TempForUsageEliteDef = ForUsageEliteDefList[random.Next(ForUsageEliteDefList.Count)];

                if (SceneInfo.instance.sceneDef.baseSceneName == "moon2" || SceneInfo.instance.sceneDef.baseSceneName == "limbo")
                {
                    TempForUsageEliteDef = RoR2Content.Elites.Lunar;
                }
                else if (SceneInfo.instance.sceneDef.baseSceneName == "voidstage" || SceneInfo.instance.sceneDef.baseSceneName == "voidraid")
                {
                    TempForUsageEliteDef = DLC1Content.Elites.Void;
                }

                int itemCountGlobal = Util.GetItemCountGlobal(DLC1Content.Items.RandomlyLunar.itemIndex, false, false);
                if (itemCountGlobal > 0)
                {
                    itemCountGlobal = Math.Min(itemCountGlobal, 10);
                    if (Run.instance.spawnRng.nextNormalizedFloat < 0.05f * (float)itemCountGlobal)
                    {
                        TempForUsageEliteDef = RoR2Content.Elites.Lunar;
                    }
                }

                brigadedAffixes = Array.Empty<EquipmentIndex>();
                brigadedAffixes = brigadedAffixes.Add(TempForUsageEliteDef.eliteEquipmentDef.equipmentIndex);

                if (Run.instance.name.Equals("WeeklyRun(Clone)"))
                {
                    Run.instance.SetFieldValue<EquipmentIndex[]>("bossAffixes", brigadedAffixes);
                }

                Debug.LogWarning("Artifact of Brigade: This stages only Elite " + TempForUsageEliteDef.name);

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



                CombatDirector.EliteTierDef[] array = new CombatDirector.EliteTierDef[5];
                int num = 0;
                CombatDirector.EliteTierDef eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = 1f;
                eliteTierDef.eliteTypes = new EliteDef[1];
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => CombatDirector.NotEliteOnlyArtifactActive());
                eliteTierDef.canSelectWithoutAvailableEliteDef = true;
                array[num] = eliteTierDef;
                int num2 = 1;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = CostMultiplier;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => CombatDirector.NotEliteOnlyArtifactActive() && rules == SpawnCard.EliteRules.Default);
                eliteTierDef.canSelectWithoutAvailableEliteDef = false;
                array[num2] = eliteTierDef;
                int num3 = 2;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = CostMultiplier;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => CombatDirector.IsEliteOnlyArtifactActive());
                eliteTierDef.canSelectWithoutAvailableEliteDef = false;
                array[num3] = eliteTierDef;
                int num4 = 3;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = CostMultiplier;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => Run.instance.loopClearCount > 0 && rules == SpawnCard.EliteRules.Default);
                eliteTierDef.canSelectWithoutAvailableEliteDef = false;
                array[num4] = eliteTierDef;
                int num5 = 4;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = CostMultiplier;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => rules == SpawnCard.EliteRules.Lunar);
                eliteTierDef.canSelectWithoutAvailableEliteDef = false;
                array[num5] = eliteTierDef;


                //EliteAPI.OverrideCombatDirectorEliteTiers(array);
                CombatDirector.eliteTiers = array;

                //self.SetFieldValue<CombatDirector.EliteTierDef[]>("eliteTiers", array);

            }
        }




        public static CharacterBody RandomizeLoadoutRespawnMethod(On.RoR2.CharacterMaster.orig_Respawn orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation)
        {

            if (self.playerCharacterMasterController)
            {
                //self.playerCharacterMasterController.networkUser.CopyLoadoutToMaster();
                var tempbod = orig(self, footPosition, rotation);



                Loadout newloadout = new Loadout();
                newloadout.Copy(self.loadout);
                int globalrepeat = 0;
                int repeat = 0;
                do
                {
                    if (repeat == tempbod.skillLocator.skillSlotCount)
                    {
                        globalrepeat++;
                        Debug.Log("Repeat Loadout " + globalrepeat + ", rerolling again");
                    }
                    repeat = 0;
                    for (int i = 0; i < tempbod.skillLocator.skillSlotCount; i++)
                    {
                        var tempgenericskill = tempbod.skillLocator.GetSkillAtIndex(i);
                        uint skillVariant = (uint)UnityEngine.Random.Range(0, tempgenericskill.skillFamily.variants.Length);
                        newloadout.bodyLoadoutManager.SetSkillVariant(tempbod.bodyIndex, i, skillVariant);

                        string tempSkillOldName = tempgenericskill.skillDef.ToString();
                        string tempSkillOldType = tempgenericskill.skillDef.GetType().ToString();
                        tempSkillOldName = tempSkillOldName.Replace(" (" + tempSkillOldType + ")", "");
                        var tempSkillindexOld = tempgenericskill.skillFamily.GetVariantIndex(tempSkillOldName);
                        var tempSkillindexNew = newloadout.bodyLoadoutManager.GetSkillVariant(tempbod.bodyIndex, i);

                        if (tempSkillindexNew == tempSkillindexOld)
                        {
                            repeat++;
                        }
                        self.loadout.bodyLoadoutManager.SetSkillVariant(tempbod.bodyIndex, i, skillVariant);


                        //Debug.LogWarning(tempgenericskill.skillFamily + " " + tempgenericskill.skillFamily.variants.Length + " different variants");
                        //Debug.LogWarning("slot " + i + ": variant " + skillVariant);
                        //Debug.LogWarning("Old Index " + tempSkillindexOld);
                        //Debug.LogWarning("New Index " + tempSkillindexNew);
                        //Debug.LogWarning("Repeats "+ repeat);
                        //Debug.LogWarning(tempbod.skillLocator.skillSlotCount);
                    }

                } while (repeat == tempbod.skillLocator.skillSlotCount && globalrepeat < 2);
                self.loadout.bodyLoadoutManager.SetSkinIndex(tempbod.bodyIndex, (uint)random.Next(0, RoR2.SkinCatalog.GetBodySkinCount(tempbod.bodyIndex)));
                tempbod.SetLoadoutServer(newloadout);
                Debug.Log("Rerolled " + tempbod.name + "'s Loadout ");





                return tempbod;
            }


            return orig(self, footPosition, rotation);
        }

        public static CharacterBody UnRandomizeLoadoutRespawnMethod(On.RoR2.CharacterMaster.orig_Respawn orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation)
        {
            if (self.playerCharacterMasterController)
            {
                self.playerCharacterMasterController.networkUser.CopyLoadoutToMaster();
            }
            return orig(self, footPosition, rotation);
        }

        public static IEnumerator DelayedChatMessage(string chatMessage, float delay)
        {
            yield return new WaitForSeconds(delay);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = chatMessage
            });
            yield break;
        }
        public static IEnumerator DelayedChatMessageNonGlobal(string chatMessage, NetworkConnection networkConnection)
        {
            yield return new WaitForSeconds(1f);
            Chat.SimpleChatMessage simpleChatMessage = new Chat.SimpleChatMessage
            {
                baseToken = chatMessage
            };

            NetworkWriter networkWriter = new NetworkWriter();
            networkWriter.StartMessage(59);
            networkWriter.Write(simpleChatMessage.GetTypeIndex());
            networkWriter.Write(simpleChatMessage);
            networkWriter.FinishMessage();
            if (networkConnection == null)
            {
                yield break;
            }
            networkConnection.SendWriter(networkWriter, RoR2.Networking.QosChannelIndex.chat.intVal);
            yield break;
        }



        public void Awake()
        {
            InitConfig();

            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;

           


            GameObject VoidSuppressorPrefab = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidSuppressor/VoidSuppressor.prefab").WaitForCompletion();

            VoidSuppressorPrefab.GetComponent<PurchaseInteraction>().isShrine = true;
            VoidSuppressorPrefab.GetComponent<VoidSuppressorBehavior>().effectColor.a = 0.85f;
            VoidSuppressorPrefab.transform.GetChild(0).GetChild(7).GetChild(1).GetChild(1).gameObject.SetActive(true);


            ScrapWhiteSuppressed.tier = ItemTier.Tier1;
            ScrapGreenSuppressed.tier = ItemTier.Tier2;
            ScrapRedSuppressed.tier = ItemTier.Tier3;

            ScrapWhiteSuppressed.pickupToken = "ITEM_SCRAPWHITE_PICKUP";
            ScrapGreenSuppressed.pickupToken = "ITEM_SCRAPGREEN_PICKUP";
            ScrapRedSuppressed.pickupToken = "ITEM_SCRAPRED_PICKUP";

            ScrapWhiteSuppressed.descriptionToken = "ITEM_SCRAPWHITE_DESC";
            ScrapGreenSuppressed.descriptionToken = "ITEM_SCRAPGREEN_DESC";
            ScrapRedSuppressed.descriptionToken = "ITEM_SCRAPRED_DESC";

            On.RoR2.UI.LogBook.LogBookController.BuildStaticData += (orig) =>
            {
                ScrapWhiteSuppressed.tier = ItemTier.NoTier;
                ScrapGreenSuppressed.tier = ItemTier.NoTier;
                ScrapRedSuppressed.tier = ItemTier.NoTier;
                orig();
                ScrapWhiteSuppressed.tier = ItemTier.Tier1;
                ScrapGreenSuppressed.tier = ItemTier.Tier2;
                ScrapRedSuppressed.tier = ItemTier.Tier3;
            };




            sgInfiniteTowerStage1Wander._sceneEntries = sgInfiniteTowerStage1Wander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[3]);
            sgInfiniteTowerStageXGolemPlainsWander._sceneEntries = sgInfiniteTowerStageXGolemPlainsWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[4]);
            sgInfiniteTowerStageXGooLakeWander._sceneEntries = sgInfiniteTowerStageXGooLakeWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[0]);
            sgInfiniteTowerStageXAncientLoftWander._sceneEntries = sgInfiniteTowerStageXAncientLoftWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[2]);
            sgInfiniteTowerStageXFrozenwallWander._sceneEntries = sgInfiniteTowerStageXFrozenwallWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[1]);
            sgInfiniteTowerStageXDampCaveWander._sceneEntries = sgInfiniteTowerStageXDampCaveWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[5]);
            sgInfiniteTowerStageXSkyMeadowWander._sceneEntries = sgInfiniteTowerStageXSkyMeadowWander._sceneEntries.Add(sgInfiniteTowerStageXGolemPlains._sceneEntries[5]);
            sgInfiniteTowerStageXMoonWander._sceneEntries = sgInfiniteTowerStageXMoonWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[3]);




            //On.RoR2.ClassicStageInfo.Start += ClassicStageStartChecker;
            On.RoR2.SceneDirector.Start += OneTimeSceneDic;



            ArtifactAdded();
            DCCSmaker();



            GameModeCatalog.availability.CallWhenAvailable(ModSupport);

            //DirectorCardCategoryCopy
            HelperSingleMixInteractable.name = "dcccHelperSingleMixInteractable";
            mixInteractablesCards.name = "dccsMixInteractableMaster";
            TrimmedmixInteractablesCards.name = "dccsMixInteractableTrimmed";
            TrimmedSingleInteractableType.name = "dccsSingleInteractable";

            Dissimilarity.cachedName = "MixInteractable";
            Kith.cachedName = "SingleInteractablePerCategory";
            Wander.cachedName = "MeanderStageOrder";
            Remodeling.cachedName = "RerollItemsAndEquipments";
            //Retooling.cachedName = "RerollEquipmentsOnStageChange";
            Spiriting.cachedName = "StatsOnLowHealth";
            Transpose.cachedName = "RandomLoadoutOnRespawn";
            Brigade.cachedName = "SingleEliteType";



            NoMoreArtifact.cachedName = "NoMoreArtifact";
            sceneNamesForWander = sceneNamesForWanderList.ToArray();

            RoR2.SceneDirector.onGenerateInteractableCardSelection += SceneDirector_onGenerateInteractableCardSelection;

            LanguageAPI.Add("BAZAAR_SEER_SHIPGRAVEYARD", "<style=cWorldEvent>You dream of windy cliffs.</style>", "en");
            LanguageAPI.Add("BAZAAR_SEER_DAMPCAVESIMPLE", "<style=cWorldEvent>You dream of fiery caverns.</style>", "en");

            LegacyResourcesAPI.Load<SceneDef>("scenedefs/moon").portalSelectionMessageString = "<style=cWorldEvent>You're having a nightmare about Commencement 1.0</style>";

            PortalMaterialArena.mainTexture = Arena.previewTexture;
            Arena.portalMaterial = PortalMaterialArena;
            Arena.portalSelectionMessageString = "<style=cWorldEvent>You dream of imprisonment.</style>";

            PortalMaterialArtifactWorld.mainTexture = ArtifactWorld.previewTexture;
            ArtifactWorld.portalMaterial = PortalMaterialArtifactWorld;
            ArtifactWorld.portalSelectionMessageString = "<style=cWorldEvent>You dream of sacred treasures.</style>";

            PortalMaterialBazaar.mainTexture = Bazaar.previewTexture;
            Bazaar.portalMaterial = PortalMaterialBazaar;
            Bazaar.portalSelectionMessageString = "<style=cWorldEvent>You dream of spending.</style>";

            PortalMaterialLimbo.mainTexture = Limbo.previewTexture;
            Limbo.portalMaterial = PortalMaterialLimbo;
            Limbo.portalSelectionMessageString = "<style=cWorldEvent>You dream of peace.</style>";

            PortalMaterialMysterySpace.mainTexture = MysterySpace.previewTexture;
            MysterySpace.portalMaterial = PortalMaterialMysterySpace;
            MysterySpace.portalSelectionMessageString = "<style=cWorldEvent>You dream of lost respite.</style>";

            PortalMaterialMenuLobby.mainTexture = MenuLobby.previewTexture;
            MenuLobby.portalMaterial = PortalMaterialMenuLobby;
            MenuLobby.portalSelectionMessageString = "<style=cWorldEvent>You're having a nightmare about Commandos disapproval</style>";

            PortalMaterialVoidRaid.mainTexture = VoidRaid.previewTexture;
            VoidRaid.portalMaterial = PortalMaterialVoidRaid;
            VoidRaid.portalSelectionMessageString = "<style=cWorldEvent>You dream of unfathomable depths.</style>";

            Color ITBazaarPreview = new Color(0.15f, 0.15f, 1f, 1f);
            Color ITBazaarPreviewBlue = new Color(0.5f, 0.5f, 1f, 1f);
            PortalMaterialITGolemPlains.SetColor("_TintColor", new Color(0.8f, 0.2f, 1f, 1f));
            itgolemplains.portalMaterial = PortalMaterialITGolemPlains;
            itgolemplains.portalSelectionMessageString = "BAZAAR_SEER_GOLEMPLAINS";

            PortalMaterialITGooLake.SetColor("_TintColor", ITBazaarPreview); //
            itgoolake.portalMaterial = PortalMaterialITGooLake;
            itgoolake.portalSelectionMessageString = "BAZAAR_SEER_GOOLAKE";

            PortalMaterialITAncientLoft.SetColor("_TintColor", ITBazaarPreview); //
            itancientloft.portalMaterial = PortalMaterialITAncientLoft;
            itancientloft.portalSelectionMessageString = "BAZAAR_SEER_ANCIENTLOFT";

            PortalMaterialITFrozenWall.SetColor("_TintColor", new Color(0.5f, 0.5f, 1f, 1f));
            itfrozenwall.portalMaterial = PortalMaterialITFrozenWall;
            itfrozenwall.portalSelectionMessageString = "BAZAAR_SEER_FROZENWALL";

            PortalMaterialITDampCave.SetColor("_TintColor", new Color(0f, 1.5f, 1.5f, 1f));
            itdampcave.portalMaterial = PortalMaterialITDampCave;
            itdampcave.portalSelectionMessageString = "BAZAAR_SEER_DAMPCAVESIMPLE";

            PortalMaterialITSkyMeadow.SetColor("_TintColor", new Color(0.6f, 0.6f, 0.1f, 1));
            itskymeadow.portalMaterial = PortalMaterialITSkyMeadow;
            itskymeadow.portalSelectionMessageString = "BAZAAR_SEER_SKYMEADOW";

            PortalMaterialITMoon.SetColor("_TintColor", new Color(0.5f, 0.2f, 0.7f, 1));
            itmoon.portalMaterial = PortalMaterialITMoon;
            itmoon.portalSelectionMessageString = "BAZAAR_SEER_MOON";

            /*
            On.RoR2.ArtifactTrialMissionController.Awake += (orig, self) =>
            {


                if (self.NetworkcurrentArtifactIndex == -1)
                {
                    //int newArtifactIndex = random.Next(ArtifactCatalog.artifactCount);
                    var temp = ArtifactCatalog.GetArtifactDef((ArtifactIndex)random.Next(ArtifactCatalog.artifactCount));
                    Debug.LogWarning(temp);

                    self.SetFieldValue<ArtifactDef>("trialArtifact", temp);


                }
                orig(self);

            };
            */
            /*
            if (DisableEnigmaArtifact.Value == true)
            {
                LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma").unlockableDef = NoMoreArtifact;
            }
            if (DisableDeathArtifact.Value == true)
            {
                LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/TeamDeath").unlockableDef = NoMoreArtifact;
            }
            if (DisableWeakAssKneeArtifact.Value == true)
            {
                LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/WeakAssKnees").unlockableDef = NoMoreArtifact;
            }
            if (DisableSoulArtifact.Value == true)
            {
                LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/WispOnDeath").unlockableDef = NoMoreArtifact;
            }
            if (DisableSoulArtifact.Value == true)
            {
                LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Swarms").unlockableDef = NoMoreArtifact;
            }
            if (DisableSoulArtifact.Value == true)
            {
                LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Glass").unlockableDef = NoMoreArtifact;
            }
            if (DisableSoulArtifact.Value == true)
            {
                LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/FriendlyFire").unlockableDef = NoMoreArtifact;
            }
            */

            On.RoR2.SceneDirector.Start += ArtifactCheckerOnStageAwake;

            if (AncestralIncubatorBool.Value == true)
            {
                //Debug.LogWarning("Incubation");
                LegacyResourcesAPI.Load<ItemDef>("itemdefs/Incubator").deprecatedTier = ItemTier.Boss;
            };



            On.RoR2.ArenaMissionController.Awake += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && ArenaInventory)
                {
                    self.gameObject.GetComponent<Inventory>().CopyItemsFrom(ArenaInventory);
                    //Debug.LogWarning(ArenaInventory.itemAcquisitionOrder.Count);
                }
            };
            On.RoR2.ArenaMissionController.OnDisable += (orig, self) =>
            {
                if (NetworkServer.active && ArenaInventory)
                {
                    if (self.gameObject && self.gameObject.GetComponent<Inventory>())
                    {
                        ArenaInventory.CopyItemsFrom(self.gameObject.GetComponent<Inventory>());
                    }
                    //Debug.LogWarning(ArenaInventory.itemAcquisitionOrder.Count);
                }
                orig(self);
            };





        }

        private void WanderNoRepeatStages(On.RoR2.Run.orig_PickNextStageScene orig, Run self, WeightedSelection<SceneDef> choices)
        {
            Debug.LogWarning("Wander : Pick next Stage");
            orig(self, choices);
            
            if (Stage.instance && self.nextStageScene == Stage.instance.sceneDef)
            {
                Debug.Log("Wander : Preventing same stage twice in a row");
                //int preventInfiniteLoop = 0;
                do
                {
                    orig(self, choices);
                }
                while (self.nextStageScene == Stage.instance.sceneDef);
            }
            if (self.nextStageScene.requiredExpansion && !self.IsExpansionEnabled(self.nextStageScene.requiredExpansion))
            {
                Debug.Log("Wander : Preventing dlc stage without required dlc");
                do
                {
                    orig(self, choices);
                }
                while (self.IsExpansionEnabled(self.nextStageScene.requiredExpansion));
            }


        }

        public void RunArtifactManager_onArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            //Remodeling, Retool and Transpose depending on what Metamorphosis does
            //Dissim and Kith don't work
            //Wander, Spiriting, Brigade make work.
            if (NetworkServer.active)
            {
                if (artifactDef == Spiriting)
                {

                    On.RoR2.HealthComponent.TakeDamage += RecalcOnDamage;
                    On.RoR2.HealthComponent.SendHeal += RecalcOnHeal;
                    //On.RoR2.HealthComponent.ServerFixedUpdate += RecalcOnShield;
                    On.RoR2.CharacterMotor.OnLanded += RecalcOnLand;
                    //On.RoR2.GenericSkill.OnExecute += RecalcOnSkill;
                    On.RoR2.CharacterBody.RecalculateStats += RecalcStats;
                    //On.EntityStates.StunState.OnEnter += RecalcStun;
                    On.RoR2.Projectile.ProjectileController.Start += RecalcProjectileSpeed;
                    Debug.Log("Added Spirit");



                }
                else if (artifactDef == Brigade)
                {
                    if (ForUsageEliteDefList.Count > 0 && SceneInfo.instance && Run.instance)
                    {
                        EliteKinAsMethod();
                        string token = "<style=cWorldEvent>All elite combatants will be ";
                        string token2 = Language.GetString(TempForUsageEliteDef.modifierToken);
                        token2 = token2.Replace("{0}", "");
                        token += token2 + "</style>";
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                        {
                            baseToken = token

                        });
                    }

                    Debug.Log("Added Brigade");

                    /*
                    On.RoR2.CombatDirector.Awake += EliteKinArtifact;
                    CombatDirector[] combatDirectorList = FindObjectsOfType(typeof(CombatDirector)) as CombatDirector[];
                    for (var i = 0; i < combatDirectorList.Length; i++)
                    {
                        EliteKinAsMethod(combatDirectorList[i]);
                    }
                    */
                }
                else if (artifactDef == Wander)
                {
                    RuleDef StageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");
                   
                    if (Run.instance)
                    {
                        On.RoR2.Run.PickNextStageScene += WanderNoRepeatStages;
                        if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
                        {

                            InfiniteTowerRunBase.startingSceneGroup = sgInfiniteTowerStage1Wander;
                            itgolemplains.destinationsGroup = sgInfiniteTowerStageXGolemPlainsWander;
                            itgoolake.destinationsGroup = sgInfiniteTowerStageXGooLakeWander;
                            itancientloft.destinationsGroup = sgInfiniteTowerStageXAncientLoftWander;
                            itfrozenwall.destinationsGroup = sgInfiniteTowerStageXFrozenwallWander;
                            itdampcave.destinationsGroup = sgInfiniteTowerStageXDampCaveWander;
                            itskymeadow.destinationsGroup = sgInfiniteTowerStageXSkyMeadowWander;
                            itmoon.destinationsGroup = sgInfiniteTowerStageXMoonWander;

                        }
                        else
                        {
                            if (Run.instance.name.Equals("WeeklyRun(Clone)"))
                            {
                                Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Normal;
                            }
                            else
                            {
                                Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Random;

                            }
                        }


                        Debug.Log("Added Wander");
                    }
                    //Debug.LogWarning(Run.instance);






                }
                else if (artifactDef == Transpose)
                {
                    //On.RoR2.Stage.RespawnCharacter += RandomizeLoadoutRespawn;
                    On.RoR2.CharacterMaster.Respawn += RandomizeLoadoutRespawnMethod;

                    On.RoR2.SceneDirector.Start -= RandomizeHeresyItems;
                    On.RoR2.SceneDirector.Start += RandomizeHeresyItems;

                    foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                    {
                        playerCharacterMasterController.StartCoroutine(DelayedRespawn(playerCharacterMasterController, 0.25f));
                    }
                    Debug.Log("Added Transpose");
                }
                else if (artifactDef == Remodeling)
                {
                    On.RoR2.SceneDirector.Start -= RandomizeItems;
                    On.RoR2.SceneDirector.Start -= RandomizeEquipment;
                    On.RoR2.SceneDirector.Start -= RandomizeItemsMonster;

                    On.RoR2.SceneDirector.Start += RandomizeItems;
                    On.RoR2.SceneDirector.Start += RandomizeEquipment;
                    if (RerollMonsterItems.Value == true)
                    {
                        On.RoR2.SceneDirector.Start += RandomizeItemsMonster;

                    }
                    Debug.Log("Added Remodeling");
                }


            }

        }

        public void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            //Debug.LogWarning(runArtifactManager + " " + artifactDef);

            if (artifactDef == Spiriting)
            {
                On.RoR2.HealthComponent.TakeDamage -= RecalcOnDamage;
                On.RoR2.HealthComponent.SendHeal -= RecalcOnHeal;
                //On.RoR2.HealthComponent.ServerFixedUpdate -= RecalcOnShield;
                On.RoR2.CharacterMotor.OnLanded -= RecalcOnLand;
                //On.RoR2.GenericSkill.OnExecute -= RecalcOnSkill;
                On.RoR2.CharacterBody.RecalculateStats -= RecalcStats;
                //On.EntityStates.StunState.OnEnter -= RecalcStun;
                On.RoR2.Projectile.ProjectileController.Start -= RecalcProjectileSpeed;
                Debug.Log("Removed Spirit");

            }
            else if (artifactDef == Brigade)
            {
                //On.RoR2.CombatDirector.Awake -= EliteKinArtifact;
                if (DidBrigadeHappen == true)
                {
                    //EliteAPI.OverrideCombatDirectorEliteTiers(normalelitetierdefs);
                    CombatDirector.eliteTiers = normalelitetierdefs;
                    DidBrigadeHappen = false;
                    Debug.Log("UnBrigading");
                }
            }
            else if (artifactDef == Wander)
            {
                RuleDef StageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");
                if (Run.instance)
                {
                    On.RoR2.Run.PickNextStageScene -= WanderNoRepeatStages;
                    if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
                    {
                        InfiniteTowerRunBase.startingSceneGroup = sgInfiniteTowerStage1;
                        itgolemplains.destinationsGroup = sgInfiniteTowerStageXGolemPlains;
                        itgoolake.destinationsGroup = sgInfiniteTowerStageXGooLake;
                        itancientloft.destinationsGroup = sgInfiniteTowerStageXAncientLoft;
                        itfrozenwall.destinationsGroup = sgInfiniteTowerStageXFrozenwall;
                        itdampcave.destinationsGroup = sgInfiniteTowerStageXDampCave;
                        itskymeadow.destinationsGroup = sgInfiniteTowerStageXSkyMeadow;
                        itmoon.destinationsGroup = sgInfiniteTowerStageXMoon;
                    }
                    else
                    {
                        if (Run.instance.name.Equals("WeeklyRun(Clone)"))
                        {
                            Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Random;
                        }
                        else
                        {
                            Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Normal;
                        }
                    }

                }
                //Debug.LogWarning(Run.instance);



            }
            else if (artifactDef == Transpose)
            {
                //On.RoR2.Stage.RespawnCharacter -= RandomizeLoadoutRespawn;


                On.RoR2.CharacterMaster.Respawn -= RandomizeLoadoutRespawnMethod;
                On.RoR2.CharacterMaster.Respawn += UnRandomizeLoadoutRespawnMethod;
                foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                {
                    CharacterBody temp = playerCharacterMasterController.master.GetBody();
                    if (temp)
                    {
                        playerCharacterMasterController.master.Respawn(temp.footPosition, temp.transform.rotation);
                    }
                };
                On.RoR2.CharacterMaster.Respawn -= UnRandomizeLoadoutRespawnMethod;
                On.RoR2.SceneDirector.Start -= RandomizeHeresyItems;

            }
            else if (artifactDef == Remodeling)
            {
                On.RoR2.SceneDirector.Start -= RandomizeItems;
                On.RoR2.SceneDirector.Start -= RandomizeEquipment;
                if (RerollMonsterItems.Value == true)
                {
                    On.RoR2.SceneDirector.Start -= RandomizeItemsMonster;
                }
            }


        }



        public static void WanderLunarTeleporter(On.RoR2.SceneDirector.orig_PlaceTeleporter orig, global::RoR2.SceneDirector self)
        {
            if (self.teleporterSpawnCard != null)
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(Wander) || RunArtifactManager.instance.IsArtifactEnabled(Dissimilarity))
                {
                    if (Run.instance.NetworkstageClearCount % Run.stagesPerLoop == Run.stagesPerLoop - 1)
                    {
                        self.teleporterSpawnCard = LunarTP;
                        //Debug.LogWarning("End of Loop Primordial");
                    }
                    else if (SceneInfo.instance.sceneDef.baseSceneName == "skymeadow")
                    {
                        self.teleporterSpawnCard = LunarTP;
                        //Debug.LogWarning("SkyMeadow Primordial");
                    }
                }
            }
            orig(self);
            On.RoR2.Language.GetString_string -= DreamPrimordial;


            if (self.teleporterInstance)
            {
                tempexitcontroller = self.teleporterInstance.GetComponent<SceneExitController>();
            }
            else
            {
                tempexitcontroller = null;
            }
            //Debug.LogWarning(tempexitcontroller);
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
            SoupWhiteGreenISC.directorCreditCost = 9;
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
            SoupGreenRedISC.directorCreditCost = 6;
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
            SoupRedWhiteISC.directorCreditCost = 3;
            SoupRedWhiteISC.occupyPosition = true;
            SoupRedWhiteISC.eliteRules = SpawnCard.EliteRules.Default;
            SoupRedWhiteISC.orientToFloor = true;
            SoupRedWhiteISC.slightlyRandomizeOrientation = false;
            SoupRedWhiteISC.skipSpawnWhenSacrificeArtifactEnabled = false;

            GameObject LunarSeerObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/SeerStation");
            SeerStationController LunarSeerTele1 = LunarSeerObject.GetComponent<RoR2.SeerStationController>();
            LunarSeerTele1.fallBackToFirstActiveExitController = true;


            On.RoR2.SeerStationController.OnStartClient += (orig, self) =>
            {
                orig(self);



                if (RunArtifactManager.instance.IsArtifactEnabled(Dissimilarity) || RunArtifactManager.instance.IsArtifactEnabled(Wander))
                {

                    if (RunArtifactManager.instance.IsArtifactEnabled(Wander) || SceneInfo.instance.sceneDef.baseSceneName != "bazaar" || ChangeBazaarSeer.Value == true)
                    {
                        if (tempexitcontroller)
                        {
                            self.explicitTargetSceneExitController = tempexitcontroller;
                        }

                        if (Run.instance.GetComponent<InfiniteTowerRun>())
                        {
                            int index = random.Next(InfiniteTowerSceneDefs.Length);
                            self.SetTargetScene(InfiniteTowerSceneDefs[index]);
                        }
                        else
                        {
                            int index = random.Next(sceneNames.Count);
                            self.SetTargetScene(sceneNames[index]);
                            if (DebugPrintLunarSeer.Value == true)
                            {
                                Debug.Log("Lunar Seer going to " + sceneNames[index] + " spawned");
                            }
                        }
                        self.gameObject.GetComponent<PurchaseInteraction>().SetAvailable(true);
                    }
                }

                if (self.NetworktargetSceneDefIndex != -1)
                {
                    string temp = Language.GetString(SceneCatalog.GetSceneDef((SceneIndex)self.NetworktargetSceneDefIndex).nameToken);
                    temp = temp.Replace("Hidden Realm: ", "");
                    self.gameObject.GetComponent<PurchaseInteraction>().contextToken = (Language.GetString("BAZAAR_SEER_CONTEXT") + " of " + temp);
                    self.gameObject.GetComponent<PurchaseInteraction>().displayNameToken = (Language.GetString("BAZAAR_SEER_NAME") + " (" + temp + ")");
                }


            };

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

                if (self.explicitTargetSceneExitController && self.explicitTargetSceneExitController.name.Equals("LunarTeleporter Variant(Clone)"))
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = "<style=cWorldEvent>The Primordial Teleporter aligns with your new dream..</style>"
                    });

                    On.RoR2.Language.GetString_string -= DreamPrimordial;
                    On.RoR2.Language.GetString_string += DreamPrimordial;
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
                selectionWeight = 480,
            };
            DirectorCard ADChest2 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscChest2"),
                selectionWeight = 80,
            };
            DirectorCard ADEquipmentBarrel = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscEquipmentBarrel"),
                selectionWeight = 50,
            };
            DirectorCard ADTripleShop = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscTripleShop"),
                selectionWeight = 160,
            };
            DirectorCard ADLunarChest = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarChest"),
                selectionWeight = 40,
            };
            DirectorCard ADCategoryChestDamage = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestDamage"),
                selectionWeight = 70,
            };
            DirectorCard ADCategoryChestHealing = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestHealing"),
                selectionWeight = 70,
            };
            DirectorCard ADCategoryChestUtility = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscCategoryChestUtility"),
                selectionWeight = 70,
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
                selectionWeight = 40,
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
                selectionWeight = 80,
            };
            DirectorCard ADVoidTriple = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidTriple/iscVoidTriple.asset").WaitForCompletion(),
                selectionWeight = 50,
            };

            //ChestsEnd
            //Barrel
            DirectorCard ADBarrel1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBarrel1"),
                selectionWeight = 820,
            };
            InteractableSpawnCard iscVoidCoinBarrel = Instantiate(Addressables.LoadAssetAsync<InteractableSpawnCard>(key: "RoR2/DLC1/VoidCoinBarrel/iscVoidCoinBarrel.asset").WaitForCompletion());
            iscVoidCoinBarrel.name = "iscVoidCoinBarrelLowPrice";
            iscVoidCoinBarrel.directorCreditCost = 2;
            DirectorCard ADBarrelVoidCoin = new DirectorCard
            {
                spawnCard = iscVoidCoinBarrel,
                selectionWeight = 600,
            };
            ADBarrelVoidCoin.spawnCard = iscVoidCoinBarrel;
            DirectorCard ADScrapperB = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscScrapper"),
                selectionWeight = 160,
            };
            DirectorCard ADVoidCamp = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidCamp/iscVoidCamp.asset").WaitForCompletion(),
                selectionWeight = 160,
            };


            DirectorCard ADDummyRareBarrel = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBarrel1"),
                selectionWeight = 8,
            };
            //BarrelEnd
            //Shrines
            DirectorCard ADShrineCombat = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineCombat"),
                selectionWeight = 270,
            };
            DirectorCard ADShrineBoss = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineBoss"),
                selectionWeight = 110,
            };
            DirectorCard ADShrineChance = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineChance"),
                selectionWeight = 400,
            };
            DirectorCard ADShrineBlood = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineBlood"),
                selectionWeight = 300,
            };
            DirectorCard ADShrineHealing = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineHealing"),
                selectionWeight = 110,
            };
            DirectorCard ADShrineRestack = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineRestack"),
                selectionWeight = 60,
            };
            DirectorCard ADShrineCleanse = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscShrineCleanse"),
                selectionWeight = 120,
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
                selectionWeight = 11,
            };
            DirectorCard ADBrokenFlameDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenFlameDrone"),
                selectionWeight = 15,
            };
            DirectorCard ADBrokenMegaDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMegaDrone"),
                selectionWeight = 2,
            };
            DirectorCard ADBrokenMissileDrone = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenMissileDrone"),
                selectionWeight = 15,
            };
            DirectorCard ADBrokenTurret1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenTurret1"),
                selectionWeight = 21,
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
                selectionWeight = 100,
            };
            DirectorCard ADSoupGreenRed = new DirectorCard
            {
                spawnCard = SoupGreenRedISC,
                selectionWeight = 50,
            };
            DirectorCard ADSoupRedWhite = new DirectorCard
            {
                spawnCard = SoupRedWhiteISC,
                selectionWeight = 35,
            };
            //DuplicatorsEnd


            mixInteractablesCards.AddCategory("Chests", 45); //0
            mixInteractablesCards.AddCategory("Barrels", 9); //1
            mixInteractablesCards.AddCategory("Shrines", 11); //2
            mixInteractablesCards.AddCategory("Drones", 12); //3
            mixInteractablesCards.AddCategory("Misc", 0); //4
            mixInteractablesCards.AddCategory("Rare", 0.5f); //5
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

            //Cut to 4?
            //mixInteractablesCards.AddCard(6, ADScrapper);  //5
            mixInteractablesCards.AddCard(6, ADDuplicator);  //5
            mixInteractablesCards.AddCard(6, ADDuplicatorLarge);  //10
            mixInteractablesCards.AddCard(6, ADDuplicatorMilitary);  //15
            mixInteractablesCards.AddCard(6, ADDuplicatorWild);  //10
            mixInteractablesCards.AddCard(6, ADSoupWhiteGreen);  //10
            mixInteractablesCards.AddCard(6, ADSoupGreenRed);  //10
            mixInteractablesCards.AddCard(6, ADSoupRedWhite);  //5

            //


        }




        internal static void ModSupport()
        {

            //Debug.LogWarning(GameModeCatalog.FindGameModePrefabComponent("ClassicRun"));

            GameModeCatalog.FindGameModePrefabComponent("WeeklyRun").startingScenes = GameModeCatalog.FindGameModePrefabComponent("ClassicRun").startingScenes;



            //Debug.LogWarning(ArtifactCatalog.FindArtifactDef("ARTIFACT_ZETDROPIFACT"));
            ZetDropifact = ArtifactCatalog.FindArtifactDef("ARTIFACT_ZETDROPIFACT");
            RiskyConformity = ArtifactCatalog.FindArtifactDef("RiskyArtifactOfConformity");
            //Debug.LogWarning(ZetDropifact);

            if (ZetDropifact == null)
            {
                ZetDropifact = Command;
            }
            if (RiskyConformity == null)
            {
                RiskyConformity = Command;
            }

            for (int i = 0; i < ArtifactCatalog.artifactCount; i++)
            {
                var temp = ArtifactCatalog.GetArtifactDef((ArtifactIndex)i);
                if (temp.pickupModelPrefab == null)
                {
                    temp.pickupModelPrefab = RoR2Content.Artifacts.EliteOnly.pickupModelPrefab;
                }
            }


            InteractableSpawnCard[] ISCList = FindObjectsOfType(typeof(InteractableSpawnCard)) as InteractableSpawnCard[];
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
                        mixInteractablesCards.AddCard(2, AD_MysticsItems_ShrineLegendary);  //
                        break;
                    case "iscTripleShopRed":
                        DirectorCard ADRedMultiShop = new DirectorCard
                        {
                            spawnCard = ISCList[i],
                            selectionWeight = 2,
                        };
                        mixInteractablesCards.AddCard(5, ADRedMultiShop);  //30
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

            HelperSingleMixInteractable.CopyFrom(mixInteractablesCards);

            mixInteractablesCards.AddCard(6, ADScrapper);  //5
            HelperSingleMixInteractable.AddCard(1, ADScrapperB);  //1   

            /*
            RoR2Content.Items.Bear.tags = RoR2Content.Items.Bear.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.Bear.tags = RoR2Content.Items.Bear.tags.Add(ItemTag.Healing);
            RoR2Content.Items.ArmorPlate.tags = RoR2Content.Items.ArmorPlate.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.ArmorPlate.tags = RoR2Content.Items.ArmorPlate.tags.Add(ItemTag.Healing);
            RoR2Content.Items.PersonalShield.tags = RoR2Content.Items.PersonalShield.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.PersonalShield.tags = RoR2Content.Items.PersonalShield.tags.Add(ItemTag.Healing);
            RoR2Content.Items.ShieldOnly.tags = RoR2Content.Items.ShieldOnly.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.ShieldOnly.tags = RoR2Content.Items.ShieldOnly.tags.Add(ItemTag.Healing);
            RoR2Content.Items.ParentEgg.tags = RoR2Content.Items.ParentEgg.tags.Remove(ItemTag.Damage);
            RoR2Content.Items.ParentEgg.tags = RoR2Content.Items.ParentEgg.tags.Add(ItemTag.Healing);

            RoR2Content.Items.ExtraLife.tags = RoR2Content.Items.ExtraLife.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.ExtraLife.tags = RoR2Content.Items.ExtraLife.tags.Add(ItemTag.Healing);

            RoR2Content.Items.SprintArmor.tags = RoR2Content.Items.SprintArmor.tags.Remove(ItemTag.Utility);
            RoR2Content.Items.SprintArmor.tags = RoR2Content.Items.SprintArmor.tags.Add(ItemTag.Healing);

            RoR2Content.Items.Squid.tags = RoR2Content.Items.Squid.tags.Remove(ItemTag.Damage);
            RoR2Content.Items.Squid.tags = RoR2Content.Items.Squid.tags.Add(ItemTag.Utility);
            RoR2Content.Items.GhostOnKill.tags = RoR2Content.Items.GhostOnKill.tags.Remove(ItemTag.Damage);
            RoR2Content.Items.GhostOnKill.tags = RoR2Content.Items.GhostOnKill.tags.Add(ItemTag.Utility);
            */




        }



        private void SceneDirector_onGenerateInteractableCardSelection(SceneDirector sceneDirector, DirectorCardCategorySelection dccs)
        {
            if (RunArtifactManager.instance)
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(ZetDropifact))
                {
                    dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(NoMoreScrapper));
                }
                if (RunArtifactManager.instance.IsArtifactEnabled(Remodeling))
                {
                    dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(RemodelingPredicate));
                    //Debug.Log("Artifact of Dissimilarity + Tossing");
                }
            }
        }


        private static bool NoMoreScrapper(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !prefab.GetComponent<ScrapperController>();
        }

        private static bool RemodelingPredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.name.Contains("Duplicator"));
        }

        private static bool ArtifactWorldPredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.name.Contains("DuplicatorWild") | prefab.GetComponent<RoR2.OutsideInteractableLocker>() | prefab.GetComponent<RoR2.ShrineBossBehavior>() | prefab.GetComponent<RoR2.ShrineCombatBehavior>() | prefab.GetComponent<RoR2.PortalStatueBehavior>() | prefab.GetComponent<RoR2.ScrapperController>() | prefab.GetComponent<RoR2.SeerStationController>());
        }

        private static bool VoidStagePredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.GetComponent<RoR2.ShrineBossBehavior>() | prefab.GetComponent<RoR2.PortalStatueBehavior>() | prefab.GetComponent<RoR2.SeerStationController>());
        }

        private static bool SimulacrumPredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return false;
        }


        public static string DreamPrimordial(On.RoR2.Language.orig_GetString_string orig, string token)
        {
            if (token == "LUNAR_TELEPORTER_ACTIVE")
            {
                return "<style=cWorldEvent>The Primordial Teleporter aligns with your dream..</style>";
            }
            else if (token == "LUNAR_TELEPORTER_IDLE")
            {
                return "<style=cWorldEvent>The Primordial Teleporter no longer aligns with your dream..</style>";
            }
            return orig(token);
        }

    }

}

