using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static ArtifactDissimilarity.Defs;

namespace ArtifactDissimilarity
{
    public class SimuWave
    {

        public static void MakeSimuWaves()
        {
            #region Simu Waves
            #region Augment of Brigade
            GameObject InfiniteTowerWaveArtifactSingleEliteType = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/ITAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactSingleEliteType", true);
            GameObject InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/ITAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacSingleEliteTypeDisabledPrerequisite = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();

            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<ArtifactEnabler>().artifactDef = Brigade_Def;
            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI;
            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<CombatDirector>().eliteBias = 0.25f;

            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Brigade_Def.smallIconSelectedSprite;
            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "ITWAVE_ARTIFACT_SINGLE_ELITE_NAME";
            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "ITWAVE_ARTIFACT_SINGLE_ELITE_DESC";

            ArtifacSingleEliteTypeDisabledPrerequisite.bannedArtifact = Brigade_Def;
            ArtifacSingleEliteTypeDisabledPrerequisite.name = "ArtifacSingleEliteTypeDisabledPrerequisite";
            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactSingleEliteType = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifactSingleEliteType, weight = 2, prerequisites = ArtifacSingleEliteTypeDisabledPrerequisite };
            #endregion
            #region Augment of Spiriting
            GameObject InfiniteTowerWaveArtifactStatsOnLowHealth = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/ITAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactStatsOnLowHealth", true);
            GameObject InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/ITAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacStatsOnLowHealthDisabledPrerequisite = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();

            InfiniteTowerWaveArtifactStatsOnLowHealth.GetComponent<ArtifactEnabler>().artifactDef = Spiriting_Def;
            InfiniteTowerWaveArtifactStatsOnLowHealth.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI;

            InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Spiriting_Def.smallIconSelectedSprite;
            InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "ITWAVE_ARTIFACT_SPEED_ONLOWHEALTH_NAME";
            InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "ITWAVE_ARTIFACT_SPEED_ONLOWHEALTH_DESC";

            ArtifacStatsOnLowHealthDisabledPrerequisite.bannedArtifact = Spiriting_Def;
            ArtifacStatsOnLowHealthDisabledPrerequisite.name = "ArtifacStatsOnLowHealthDisabledPrerequisite";

            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactStatsOnLowHealth = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifactStatsOnLowHealth, weight = 1f, prerequisites = ArtifacStatsOnLowHealthDisabledPrerequisite };

            #endregion
            #region Augment of Tranpose
            GameObject InfiniteTowerWaveArtifactRandomLoadout = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/ITAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactRandomLoadout", true);
            GameObject InfiniteTowerCurrentArtifactRandomLoadoutWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/ITAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactRandomLoadoutWaveUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacRandomLoadoutDisabledPrerequisite = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();

            InfiniteTowerWaveArtifactRandomLoadout.GetComponent<ArtifactEnabler>().artifactDef = Transpose_Def;
            InfiniteTowerWaveArtifactRandomLoadout.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerCurrentArtifactRandomLoadoutWaveUI;

            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Transpose_Def.smallIconSelectedSprite;
            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "ITWAVE_ARTIFACT_REROLL_SKILLS_NAME";
            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "ITWAVE_ARTIFACT_REROLL_SKILLS_DESC";

            ArtifacRandomLoadoutDisabledPrerequisite.bannedArtifact = Transpose_Def;
            ArtifacRandomLoadoutDisabledPrerequisite.name = "ArtifacRandomLoadoutDisabledPrerequisite";
            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactRandomLoadout = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifactRandomLoadout, weight = 1f, prerequisites = ArtifacRandomLoadoutDisabledPrerequisite };

            #endregion
            RoR2.InfiniteTowerWaveCategory ITBasicWaves = Addressables.LoadAssetAsync<RoR2.InfiniteTowerWaveCategory>(key: "4e63333e89a09f64680d3475ba1b5903").WaitForCompletion();
            ITBasicWaves.wavePrefabs = ITBasicWaves.wavePrefabs.Add(ITBasicArtifactSingleEliteType, ITBasicArtifactRandomLoadout, ITBasicArtifactStatsOnLowHealth);
            #endregion
        }


    }
}