using ArtifactDissimilarity.Aritfacts;
using BepInEx;
using R2API;
using R2API.ScriptableObjects;
using R2API.Utils;
using RoR2;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ArtifactDissimilarity
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Wolfo.WolfoArtifacts", "WolfoArtifacts", "3.5.3")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class Main : BaseUnityPlugin
    {
        public void Awake()
        {
            Assets.Init(Info);
            WConfig.InitConfig();
           
            Defs.MakeArtifacts();
            SimuWave.MakeSimuWaves();
 
            Spiriting.Start();
            Dissimilarity.Start();
            DissimWander_LunarSeer.Start();
            Kith.Start();
            Doubles.Start();
            Flamboyance.Start();




            GameModeCatalog.availability.CallWhenAvailable(ModSupport);


            ChatMessageBase.chatMessageTypeToIndex.Add(typeof(Brigade.BrigadeMessage), (byte)ChatMessageBase.chatMessageIndexToType.Count);
            ChatMessageBase.chatMessageIndexToType.Add(typeof(Brigade.BrigadeMessage));

 
            On.RoR2.ArtifactTrialMissionController.Awake += SetArtifactWorldArtifactToRandom;

            On.RoR2.InfiniteTowerRun.OverrideRuleChoices += RemoveArtifactsFromSimu;
        }

        private void SetArtifactWorldArtifactToRandom(On.RoR2.ArtifactTrialMissionController.orig_Awake orig, ArtifactTrialMissionController self)
        {
			if (NetworkServer.active && ArtifactTrialMissionController.trialArtifact == null)
			{
				ArtifactTrialMissionController.trialArtifact = ArtifactCatalog.GetArtifactDef((ArtifactIndex)Random.RandomRangeInt(0, ArtifactCatalog.artifactCount));
			}
            orig(self);
		}

        private void RemoveArtifactsFromSimu(On.RoR2.InfiniteTowerRun.orig_OverrideRuleChoices orig, InfiniteTowerRun self, RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, ulong runSeed)
        {
            orig(self, mustInclude, mustExclude, runSeed);

            self.ForceChoice(mustInclude, mustExclude, RuleCatalog.FindRuleDef("Artifacts." + Defs.Wander_Def.cachedName).FindChoice("Off"));
        }
 
        public static IEnumerator DelayedRespawn(PlayerCharacterMasterController playerCharacterMasterController, float delay)
        {
            yield return new WaitForSeconds(delay);
            CharacterBody temp = playerCharacterMasterController.master.GetBody();
            if (temp)
            {
                Vector3 vector = temp.footPosition;
                playerCharacterMasterController.master.Respawn(vector, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));
            }
            yield break;
        }
 


        internal static void ModSupport()
        {
            Dissimilarity.ModSupport();


            for (int i = 0; i < ArtifactCatalog.artifactCount; i++)
            {
                ArtifactDef temp = ArtifactCatalog.GetArtifactDef((ArtifactIndex)i);
                if (temp.pickupModelPrefab && temp.pickupModelReference == null)
                {
                    temp.pickupModelReference = RoR2Content.Artifacts.EliteOnly.pickupModelReference;
                }
            }
        }



    }

}

