using RoR2;
//using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<SceneDef> previouslyVisitedSceneDef = new List<SceneDef>();


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

            SceneDef scene = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC2/habitat/habitat.asset").WaitForCompletion();
            scene.validForRandomSelection = true;
            scene = Addressables.LoadAssetAsync<SceneDef>(key: "RoR2/DLC2/helminthroost/helminthroost.asset").WaitForCompletion();
            scene.validForRandomSelection = true;
        }

        public static void OnArtifactDisable()
        {
            On.RoR2.Run.PickNextStageScene -= Wander_PickStage;
            On.RoR2.TeleporterInteraction.Start -= MoreMysterySpacePortal;
            SceneDirector.onPrePopulateSceneServer -= MoreLunarTeleporter;
            if (Run.instance)
            {
                Wander.WanderSetup(false);
            }
            previouslyVisitedSceneDef.Clear();
        }

        public static void OnArtifactEnable()
        {
            On.RoR2.Run.PickNextStageScene += Wander_PickStage;
            On.RoR2.TeleporterInteraction.Start += MoreMysterySpacePortal;
            SceneDirector.onPrePopulateSceneServer += MoreLunarTeleporter;
            if (Run.instance)
            {
                Wander.WanderSetup(true);
            }
            WanderDissim_LunarSeer.PopulateSeerList();
            Debug.Log("Added Wander");       
        }

        private static void MoreLunarTeleporter(SceneDirector obj)
        {
            if (Run.instance.NetworkstageClearCount >= 4 && Run.instance.NetworkstageClearCount % 2 == 0)
            {
                obj.teleporterSpawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarTeleporter");
            }
        }
        private static void MoreMysterySpacePortal(On.RoR2.TeleporterInteraction.orig_Start orig, TeleporterInteraction self)
        {
            orig(self);
            if (Run.instance.NetworkstageClearCount >= 4 && Run.instance.NetworkstageClearCount % 2 == 0)
            {
                self.shouldAttemptToSpawnMSPortal = true;
                var portalSpawners = self.GetComponents<PortalSpawner>();
                foreach (var portalSpawner in portalSpawners)
                {
                    portalSpawner.minStagesCleared = 0;
                    portalSpawner.spawnChance = 1;
                    portalSpawner.validStages = new string[0];
                }
            }
        }


        public static bool ValidForWander(SceneDef sceneDef)
        {
            if (previouslyVisitedSceneDef.Contains(sceneDef))
            {
                return false;
            }
            if (!Run.instance.CanPickStage(sceneDef))
            {
                return false;
            }
            return sceneDef.validForRandomSelection && sceneDef.hasAnyDestinations;
        }

        public static void Wander_PickStage(On.RoR2.Run.orig_PickNextStageScene orig, Run self, WeightedSelection<SceneDef> choices)
        {
            if (self.ruleBook.stageOrder == StageOrder.Random)
            {
                Debug.Log("Wander : Pick next Stage");
                if (self.stageClearCount % 5 == 0)
                {
                    previouslyVisitedSceneDef.Clear();
                }
                SceneDef[] array = SceneCatalog.allStageSceneDefs.Where(new System.Func<SceneDef, bool>(ValidForWander)).ToArray<SceneDef>();
                self.nextStageScene = self.nextStageRng.NextElementUniform<SceneDef>(array);
                previouslyVisitedSceneDef.Add(self.nextStageScene);
            }
            else
            {
                orig(self,choices);
            }
        }


        public static void WanderSetup(bool isEnable)
        {
         
            if (isEnable)
            {
                if (Run.instance is InfiniteTowerRun)
                {
                    Run.instance.startingSceneGroup = sgInfiniteTowerStage1Wander;
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
                    Run.instance.ruleBook.GetRuleChoice(RuleBook.stageOrderRule).extraData = StageOrder.Random;
                }
            }
            else
            {
                if (Run.instance is InfiniteTowerRun)
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
                    Run.instance.ruleBook.GetRuleChoice(RuleBook.stageOrderRule).extraData = StageOrder.Normal;
                }
            }
            
        }
 

    }
}