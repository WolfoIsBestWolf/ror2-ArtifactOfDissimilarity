using RoR2;
//using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ArtifactDissimilarity
{
    public class Wander
    {

        private static readonly InfiniteTowerRun InfiniteTowerRunBase = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerRun.prefab").WaitForCompletion().GetComponent<InfiniteTowerRun>();
        private static readonly SceneDef itgolemplains = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itgolemplains/itgolemplains.asset").WaitForCompletion();
        private static readonly SceneDef itgoolake = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itgoolake/itgoolake.asset").WaitForCompletion();
        private static readonly SceneDef itancientloft = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itancientloft/itancientloft.asset").WaitForCompletion();
        private static readonly SceneDef itfrozenwall = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itfrozenwall/itfrozenwall.asset").WaitForCompletion();
        private static readonly SceneDef itdampcave = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itdampcave/itdampcave.asset").WaitForCompletion();
        private static readonly SceneDef itskymeadow = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itskymeadow/itskymeadow.asset").WaitForCompletion();
        private static readonly SceneDef itmoon = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/itmoon/itmoon.asset").WaitForCompletion();
        public static readonly SceneDef[] InfiniteTowerSceneDefs = { itgolemplains, itgoolake, itfrozenwall, itfrozenwall, itdampcave, itskymeadow, itmoon };


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

        /*private static readonly Material PortalMaterialArena = Instantiate(Plains.portalMaterial);
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
        private static readonly Material PortalMaterialITMoon = Instantiate(Moon2.portalMaterial);*/

        public static readonly List<SceneDef> scenesSeerDestinations = new List<SceneDef> { Roost, Plains, Sand, Swamp, Acres, Snow, Depths, Grove, Siren, SkyMeadow, Moon2, Gold, Arena, MysterySpace, ArtifactWorld, SnowyForest, AncientLoft, SulfurPools, VoidStage };
        //static List<SceneDef> tempsceneNames = new List<SceneDef>(sceneNames);
        //static readonly List<SceneDef> sceneNamesForWanderList = new List<SceneDef> { Roost, Plains, Sand, Swamp, Acres, Snow, Depths, Grove, Siren, SkyMeadow, SnowyForest, AncientLoft, SulfurPools };
        //private static SceneDef[] sceneNamesForWander = new SceneDef[10];



        public static void Start()
        {
            sgInfiniteTowerStage1Wander._sceneEntries = sgInfiniteTowerStage1Wander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[3]);
            sgInfiniteTowerStageXGolemPlainsWander._sceneEntries = sgInfiniteTowerStageXGolemPlainsWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[4]);
            sgInfiniteTowerStageXGooLakeWander._sceneEntries = sgInfiniteTowerStageXGooLakeWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[0]);
            sgInfiniteTowerStageXAncientLoftWander._sceneEntries = sgInfiniteTowerStageXAncientLoftWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[2]);
            sgInfiniteTowerStageXFrozenwallWander._sceneEntries = sgInfiniteTowerStageXFrozenwallWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[1]);
            sgInfiniteTowerStageXDampCaveWander._sceneEntries = sgInfiniteTowerStageXDampCaveWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[5]);
            sgInfiniteTowerStageXSkyMeadowWander._sceneEntries = sgInfiniteTowerStageXSkyMeadowWander._sceneEntries.Add(sgInfiniteTowerStageXGolemPlains._sceneEntries[5]);
            sgInfiniteTowerStageXMoonWander._sceneEntries = sgInfiniteTowerStageXMoonWander._sceneEntries.Add(sgInfiniteTowerStage1._sceneEntries[3]);

            MakeSeerMaterials();
        }


        public static void MakeSeerMaterials()
        {

            //LegacyResourcesAPI.Load<SceneDef>("scenedefs/moon").portalSelectionMessageString = "<style=cWorldEvent>You are having a nightmare</style>";

            Material PortalMaterialArena = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialArtifactWorld = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialBazaar = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialLimbo = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialMysterySpace = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialMenuLobby = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialVoidRaid = Object.Instantiate(Plains.portalMaterial);

            Material PortalMaterialITGolemPlains = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialITGooLake = Object.Instantiate(Sand.portalMaterial);
            Material PortalMaterialITAncientLoft = Object.Instantiate(AncientLoft.portalMaterial);
            Material PortalMaterialITFrozenWall = Object.Instantiate(Snow.portalMaterial);
            Material PortalMaterialITDampCave = Object.Instantiate(Depths.portalMaterial);
            Material PortalMaterialITSkyMeadow = Object.Instantiate(SkyMeadow.portalMaterial);
            Material PortalMaterialITMoon = Object.Instantiate(Moon2.portalMaterial);


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
        }

        public static void SeerDestinationRandomizerDissimWander(On.RoR2.SeerStationController.orig_OnStartClient orig, SeerStationController self)
        {
            orig(self);
            //Randomizes Seers when Wander or Dissim 
            if (RunArtifactManager.instance.IsArtifactEnabled(Main.Dissimilarity_Def) && SceneInfo.instance.sceneDef.baseSceneName != "bazaar" || RunArtifactManager.instance.IsArtifactEnabled(Main.Wander_Def))
            {
                //There's probably a way to get the Teleporter Instance in a better way
                if (Main.tempexitcontroller)
                {
                    self.explicitTargetSceneExitController = Main.tempexitcontroller;
                }

                if (Run.instance.GetComponent<InfiniteTowerRun>())
                {
                    int index = Main.random.Next(Wander.InfiniteTowerSceneDefs.Length);
                    self.SetTargetScene(Wander.InfiniteTowerSceneDefs[index]);
                }
                else
                {
                    int index = Main.random.Next(Wander.scenesSeerDestinations.Count);
                    self.SetTargetScene(Wander.scenesSeerDestinations[index]);
                    if (WConfig.DebugPrint.Value == true)
                    {
                        Debug.Log("Lunar Seer going to " + Wander.scenesSeerDestinations[index] + " spawned");
                    }
                }
                self.gameObject.GetComponent<PurchaseInteraction>().SetAvailable(true);
            }
        }


        public static void WanderNoRepeatStages(On.RoR2.Run.orig_PickNextStageScene orig, Run self, WeightedSelection<SceneDef> choices)
        {
            orig(self, choices);
            Debug.Log("Wander : Pick next Stage");
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


        public static void WanderSetup()
        {
            RuleDef StageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");
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
                //Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Normal;
            }
            else
            {
                Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Random;
            }
        }

        public static void WanderUnSet()
        {
            RuleDef StageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");
            if (Run.instance.name.StartsWith("InfiniteTowerRun"))
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
            else if (Run.instance.name.StartsWith("WeeklyRun"))
            {
                //Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Random;
            }
            else
            {
                Run.instance.ruleBook.GetRuleChoice(StageOrderRule).extraData = StageOrder.Normal;
            }

        }


        public static void WanderStageStart()
        {
            //Is this no longer needed or smth
            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Main.Wander_Def))
                {
                    if (Run.instance.name.StartsWith("ClassicRun"))
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

    }
}