using RoR2;
//using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtifactDissimilarity.Aritfacts
{
    public class Wander
    {

        public static List<SceneDef> previouslyVisitedSceneDef = new List<SceneDef>();

        public static void OnArtifactDisable()
        {
            On.RoR2.Run.PickNextStageScene -= Wander_PickStage;
            On.RoR2.TeleporterInteraction.Start -= MoreMysterySpacePortal;
            SceneDirector.onPrePopulateSceneServer -= MoreLunarTeleporter;

            previouslyVisitedSceneDef.Clear();
        }

        public static void OnArtifactEnable()
        {
            On.RoR2.Run.PickNextStageScene += Wander_PickStage;
            On.RoR2.TeleporterInteraction.Start += MoreMysterySpacePortal;
            SceneDirector.onPrePopulateSceneServer += MoreLunarTeleporter;

            DissimWander_LunarSeer.PopulateSeerList();
            Debug.Log("Added Wander");
        }

        private static void MoreLunarTeleporter(SceneDirector obj)
        {
            bool stage5 = (Run.instance.NetworkstageClearCount+1) % 5 == 0;
            bool randomChanceStarting4 = Run.instance.NetworkstageClearCount >= 3 && Random.RandomRangeInt(0, 5) == 0;
            if (stage5 || randomChanceStarting4)
            {
                obj.teleporterSpawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarTeleporter");
            }
        }
        private static void MoreMysterySpacePortal(On.RoR2.TeleporterInteraction.orig_Start orig, TeleporterInteraction self)
        {
            orig(self);
            if (Run.instance.NetworkstageClearCount >= 3) //Stage 4
            {
                self.shouldAttemptToSpawnMSPortal = Random.RandomRangeInt(0, 5) == 0; //20% chance to appear any time
                var portalSpawners = self.GetComponents<PortalSpawner>();
                foreach (var portalSpawner in portalSpawners)
                {
                    if (portalSpawner.previewChildName == "VoidPortalIndicator")
                    {
                        portalSpawner.minStagesCleared = 0;
                        if (portalSpawner.spawnChance < 0.2f)
                        {
                            portalSpawner.spawnChance = 0.2f;
                        }
                    }
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
            Debug.Log("Wander : Pick next Stage");
            if (self.stageClearCount % 5 == 0)
            {
                previouslyVisitedSceneDef.Clear();
            }
            SceneDef[] array = SceneCatalog.allStageSceneDefs.Where(new System.Func<SceneDef, bool>(ValidForWander)).ToArray<SceneDef>();
            self.nextStageScene = self.nextStageRng.NextElementUniform<SceneDef>(array);
            previouslyVisitedSceneDef.Add(self.nextStageScene);

            self.Network_loopClearCount = self.stageClearCount / 5;
        }
 


    }
}