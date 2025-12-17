using RoR2;
//using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ArtifactDissimilarity
{
    public class DissimWander_LunarSeer
    {


        public static List<SceneDef> scenesSeerDestinations;

        public static void PopulateSeerList()
        {
            if (!Run.instance)
            {
                return;
            }
            if (scenesSeerDestinations.Count > 13)
            {
                //Already did list;
                return;
            }
            //Remove expansion locked Hidden Realms
            for (int i = 0; i < scenesSeerDestinations.Count; i++)
            {
                if (Run.instance.CanPickStage(scenesSeerDestinations[i]))
                {
                    scenesSeerDestinations.Remove(scenesSeerDestinations[i]);
                }
            }
            for (int i = 0; i < SceneCatalog.allStageSceneDefs.Length; i++)
            {
                if (ValidStageForSeer(SceneCatalog.allStageSceneDefs[i]))
                {
                    scenesSeerDestinations.Add(SceneCatalog.allStageSceneDefs[i]);
                }
            }
        }


        public static bool ValidStageForSeer(SceneDef sceneDef)
        {
            if (!Run.instance.CanPickStage(sceneDef))
            {
                return false;
            }
            return sceneDef.hasAnyDestinations && sceneDef.validForRandomSelection;
        }

        public static void Start()
        {
            MakeSeerMaterials();
            On.RoR2.SeerStationController.OnStartClient += SeerDestinationRandomizerDissimWander;

            On.RoR2.SeerStationController.SetRunNextStageToTarget += SeerStationController_SetRunNextStageToTarget;

        }

        private static void SeerStationController_SetRunNextStageToTarget(On.RoR2.SeerStationController.orig_SetRunNextStageToTarget orig, SeerStationController self)
        {
            orig(self);

            if (self.explicitTargetSceneExitController && self.explicitTargetSceneExitController.name.StartsWith("LunarTeleporter"))
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "LUNAR_TELEPORTER_ALIGN_DREAM"
                });
            }
        }

        public static void MakeSeerMaterials()
        {
            //SceneDef Roost = LegacyResourcesAPI.Load<SceneDef>("scenedefs/blackbeach");
            SceneDef Plains = LegacyResourcesAPI.Load<SceneDef>("scenedefs/golemplains");
            SceneDef Sand = LegacyResourcesAPI.Load<SceneDef>("scenedefs/goolake");
            //SceneDef Swamp = LegacyResourcesAPI.Load<SceneDef>("scenedefs/foggyswamp");
            //SceneDef Acres = LegacyResourcesAPI.Load<SceneDef>("scenedefs/wispgraveyard");
            SceneDef Snow = LegacyResourcesAPI.Load<SceneDef>("scenedefs/frozenwall");
            SceneDef Depths = LegacyResourcesAPI.Load<SceneDef>("scenedefs/dampcavesimple");
            //SceneDef Grove = LegacyResourcesAPI.Load<SceneDef>("scenedefs/rootjungle");
            //SceneDef Siren = LegacyResourcesAPI.Load<SceneDef>("scenedefs/shipgraveyard");
            SceneDef SkyMeadow = LegacyResourcesAPI.Load<SceneDef>("scenedefs/skymeadow");
            SceneDef Moon2 = LegacyResourcesAPI.Load<SceneDef>("scenedefs/moon2");
            SceneDef Gold = LegacyResourcesAPI.Load<SceneDef>("scenedefs/goldshores");

            SceneDef Arena = LegacyResourcesAPI.Load<SceneDef>("scenedefs/arena");
            SceneDef ArtifactWorld = LegacyResourcesAPI.Load<SceneDef>("scenedefs/artifactworld");
            SceneDef Bazaar = LegacyResourcesAPI.Load<SceneDef>("scenedefs/bazaar");
            SceneDef Limbo = LegacyResourcesAPI.Load<SceneDef>("scenedefs/limbo");
            SceneDef MysterySpace = LegacyResourcesAPI.Load<SceneDef>("scenedefs/mysteryspace");
           
            //SceneDef SnowyForest = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/snowyforest/snowyforest.asset").WaitForCompletion();
            SceneDef AncientLoft = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/ancientloft/ancientloft.asset").WaitForCompletion();
            //SceneDef SulfurPools = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/sulfurpools/sulfurpools.asset").WaitForCompletion();
            SceneDef VoidStage = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/voidstage/voidstage.asset").WaitForCompletion();
            SceneDef VoidRaid = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC1/voidraid/voidraid.asset").WaitForCompletion();

            SceneDef meridian = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC2/meridian/meridian.asset").WaitForCompletion();
            SceneDef artifactworld01 = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC2/artifactworld01/artifactworld01.asset").WaitForCompletion();
            SceneDef artifactworld02 = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC2/artifactworld02/artifactworld02.asset").WaitForCompletion();
            SceneDef artifactworld03 = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC2/artifactworld03/artifactworld03.asset").WaitForCompletion();

            scenesSeerDestinations = new List<SceneDef> { artifactworld01, artifactworld02, artifactworld03, Moon2, Gold, Arena, MysterySpace, ArtifactWorld, VoidStage, meridian };
 
            Material PortalMaterialArena = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialArtifactWorld = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialBazaar = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialLimbo = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialMysterySpace = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialMenuLobby = Object.Instantiate(Plains.portalMaterial);
            Material PortalMaterialVoidRaid = Object.Instantiate(Plains.portalMaterial);
 

            Moon2.portalMaterial.SetFloat("_Boost", 1);
            Moon2.portalMaterial.mainTextureScale = new Vector2(1f, 0.5f);
            meridian.portalMaterial.SetFloat("_Boost", 1);

 
            PortalMaterialArena.mainTexture = Arena.previewTexture;
            Arena.portalMaterial = PortalMaterialArena;
            Arena.portalSelectionMessageString = "BAZAAR_SEER_ARENA";

            PortalMaterialArtifactWorld.mainTexture = ArtifactWorld.previewTexture;
            ArtifactWorld.portalMaterial = PortalMaterialArtifactWorld;
            artifactworld01.portalSelectionMessageString = "BAZAAR_SEER_ARTIFACTWORLD";
            artifactworld02.portalSelectionMessageString = "BAZAAR_SEER_ARTIFACTWORLD";
            artifactworld03.portalSelectionMessageString = "BAZAAR_SEER_ARTIFACTWORLD";



            PortalMaterialBazaar.mainTexture = Bazaar.previewTexture;
            Bazaar.portalMaterial = PortalMaterialBazaar;
            Bazaar.portalSelectionMessageString = "BAZAAR_SEER_SHOP";

            PortalMaterialLimbo.mainTexture = Limbo.previewTexture;
            Limbo.portalMaterial = PortalMaterialLimbo;
            Limbo.portalSelectionMessageString = "BAZAAR_SEER_LIMBO";

            PortalMaterialMysterySpace.mainTexture = MysterySpace.previewTexture;
            MysterySpace.portalMaterial = PortalMaterialMysterySpace;
            MysterySpace.portalSelectionMessageString = "BAZAAR_SEER_MS";

            PortalMaterialVoidRaid.mainTexture = VoidRaid.previewTexture;
            VoidRaid.portalMaterial = PortalMaterialVoidRaid;
            VoidRaid.portalSelectionMessageString = "BAZAAR_SEER_VOIDLING";


 
        }

        public static void SeerDestinationRandomizerDissimWander(On.RoR2.SeerStationController.orig_OnStartClient orig, SeerStationController self)
        {
            orig(self);
            //Randomizes Seers when Wander or Dissim 
            if (!NetworkServer.active)
            {
                return;
            }

            if (RunArtifactManager.instance.IsArtifactEnabled(Main.Wander_Def) || RunArtifactManager.instance.IsArtifactEnabled(Main.Dissimilarity_Def) && BazaarController.instance == null)
            {
                //There's probably a way to get the Teleporter Instance in a better way
                if (TeleporterInteraction.instance)
                {
                    self.explicitTargetSceneExitController = TeleporterInteraction.instance.sceneExitController;
                }
                int index = Run.instance.nextStageRng.RangeInt(0, scenesSeerDestinations.Count);
                self.SetTargetScene(scenesSeerDestinations[index]);
                if (WConfig.DebugPrint.Value == true)
                {
                    Debug.Log("Lunar Seer going to " + scenesSeerDestinations[index] + " spawned");
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

    }
}