using BepInEx;
using R2API;
using R2API.ScriptableObjects;
using R2API.Utils;
using RoR2;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using ArtifactDissimilarity.Aritfacts;

namespace ArtifactDissimilarity
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Wolfo.WolfoArtifacts", "WolfoArtifacts", "3.5.0")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class Main : BaseUnityPlugin
    {
        public static readonly System.Random random = new System.Random();


        public static ArtifactDef Dissimilarity_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Kith_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Wander_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Remodeling_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Spiriting_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Brigade_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Transpose_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Unison_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Obscurity_Def = ScriptableObject.CreateInstance<ArtifactDef>();

        public static ArtifactDef Flamboyance_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Doubles_Def = ScriptableObject.CreateInstance<ArtifactDef>();

        public void Awake()
        {
            Assets.Init(Info);
            WConfig.InitConfig();
            ArtifactAdded();
            Spiriting.Start();
            //Wander.Start();
            Dissimilarity.Start();
            DissimWander_LunarSeer.Start();
            Kith.Start();

            Doubles.Start();
            Flamboyance.Start();

            GameModeCatalog.availability.CallWhenAvailable(ModSupport);

            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onEnabledArtifactGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onDisableArtifactGlobal;


            ChatMessageBase.chatMessageTypeToIndex.Add(typeof(Brigade.BrigadeMessage), (byte)ChatMessageBase.chatMessageIndexToType.Count);
            ChatMessageBase.chatMessageIndexToType.Add(typeof(Brigade.BrigadeMessage));


            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, WhiteToGreen").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier2");
            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, GreenToRed Variant").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier3");
            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, RedToWhite Variant").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier1");


            On.RoR2.Stage.Start += StageStartMethod;
            On.RoR2.Run.Start += RunStartHook;
            On.RoR2.Run.OnDisable += WanderEnder;

            //Obscurity.Start();

            On.RoR2.InfiniteTowerRun.OverrideRuleChoices += InfiniteTowerRun_OverrideRuleChoices;
        }

        private void InfiniteTowerRun_OverrideRuleChoices(On.RoR2.InfiniteTowerRun.orig_OverrideRuleChoices orig, InfiniteTowerRun self, RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, ulong runSeed)
        {
            orig(self, mustInclude, mustExclude, runSeed);

            self.ForceChoice(mustInclude, mustExclude, RuleCatalog.FindRuleDef("Artifacts." + Wander_Def.cachedName).FindChoice("Off"));
        }

        public static void RunStartHook(On.RoR2.Run.orig_Start orig, Run self)
        {
            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Wander_Def))
                {
                    Wander.WanderSetup(true);
                }
            }
            orig(self);


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

        public static void WanderEnder(On.RoR2.Run.orig_OnDisable orig, Run self)
        {
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Wander_Def))
            {
                Wander.WanderSetup(false);
            }
            orig(self);
        }

        public static IEnumerator StageStartMethod(On.RoR2.Stage.orig_Start orig, RoR2.Stage self)
        {
            if (NetworkServer.active)
            {
                ArtifactTrialMissionController.trialArtifact = ArtifactCatalog.GetArtifactDef((ArtifactIndex)random.Next(ArtifactCatalog.artifactCount));
            }
            return orig(self);
        }



        public static void ArtifactAdded()
        {
            UnlockableDef AlwaysLocked = ScriptableObject.CreateInstance<UnlockableDef>();
            AlwaysLocked.cachedName = "NoMoreArtifact";
  
            #region Dissimilarity (Dissonance Interactable)
            Dissimilarity_Def.cachedName = "MixInteractable";
            Dissimilarity_Def.nameToken = "ARTIFACT_MIX_INTERACTABLE_NAME";
            Dissimilarity_Def.descriptionToken = "ARTIFACT_MIX_INTERACTABLE_DESC";
            Dissimilarity_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Dissimilarity_On.png"); ;
            Dissimilarity_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Dissimilarity_Off.png"); ;
            Dissimilarity_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MixEnemy").pickupModelReference;
            ContentAddition.AddArtifactDef(Dissimilarity_Def);
            if (WConfig.EnableDissim.Value == false)
            {
                Dissimilarity_Def.unlockableDef = AlwaysLocked;
            }
            else
            {
                ArtifactCode DissimCode = ScriptableObject.CreateInstance<ArtifactCode>();
                DissimCode.topRow = new Vector3Int(1, 1, 5);
                DissimCode.topRow = new Vector3Int(1, 1, 1);
                DissimCode.topRow = new Vector3Int(5, 1, 1);
                ArtifactCodeAPI.AddCode(Dissimilarity_Def, DissimCode);
            }
            #endregion
            #region Kith (Kin Interactables)
            Kith_Def.cachedName = "SingleInteractablePerCategory";
            Kith_Def.nameToken = "ARTIFACT_SINGLE_INTERACTABLE_NAME";
            Kith_Def.descriptionToken = "ARTIFACT_SINGLE_INTERACTABLE_DESC";
            Kith_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Kith_On.png");
            Kith_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Kith_Off.png");
            Kith_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/SingleMonsterType").pickupModelReference;
            ContentAddition.AddArtifactDef(Kith_Def);
            if (WConfig.EnableKith.Value == false)
            {
                Kith_Def.unlockableDef = AlwaysLocked;
            }
            else
            {
                ArtifactCode KithCode = ScriptableObject.CreateInstance<ArtifactCode>();
                KithCode.topRow = new Vector3Int(3, 5, 3);
                KithCode.topRow = new Vector3Int(3, 5, 3);
                KithCode.topRow = new Vector3Int(1, 1, 1);
                ArtifactCodeAPI.AddCode(Kith_Def, KithCode);
            }
            #endregion
            #region Wander (Random Stage Order)
            Wander_Def.cachedName = "MeanderStageOrder";
            Wander_Def.nameToken = "ARTIFACT_RANDOM_STAGEORDER_NAME";
            Wander_Def.descriptionToken = "ARTIFACT_RANDOM_STAGEORDER_DESC";
            Wander_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Wander_On.png"); ;
            Wander_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Wander_Off.png"); ;
            Wander_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma").pickupModelReference;
            ContentAddition.AddArtifactDef(Wander_Def);
            if (WConfig.EnableWanderArtifact.Value == false)
            {
                Wander_Def.unlockableDef = AlwaysLocked;
            }
            else
            {
                ArtifactCode WanderCode = ScriptableObject.CreateInstance<ArtifactCode>();
                WanderCode.topRow = new Vector3Int(3, 7, 3);
                WanderCode.topRow = new Vector3Int(7, 7, 7);
                WanderCode.topRow = new Vector3Int(3, 7, 3);
                ArtifactCodeAPI.AddCode(Wander_Def, WanderCode);
            }
            #endregion
            #region Remodeling (Reroll Items)
            Remodeling_Def.cachedName = "RerollItemsAndEquipments";
            Remodeling_Def.nameToken = "ARTIFACT_REROLL_ITEMS_NAME";
            Remodeling_Def.descriptionToken = "ARTIFACT_REROLL_ITEMS_DESC";
            Remodeling_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Remodeling_On.png"); ;
            Remodeling_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Remodeling_Off.png"); ;
            Remodeling_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/RandomSurvivorOnRespawn").pickupModelReference;
            ContentAddition.AddArtifactDef(Remodeling_Def);
            if (WConfig.EnableRemodelArtifact.Value == false)
            {
                Remodeling_Def.unlockableDef = AlwaysLocked;
            }
            else
            {
                ArtifactCode RemodelCode = ScriptableObject.CreateInstance<ArtifactCode>();
                RemodelCode.topRow = new Vector3Int(1, 7, 1);
                RemodelCode.topRow = new Vector3Int(1, 5, 1);
                RemodelCode.topRow = new Vector3Int(1, 7, 1);
                ArtifactCodeAPI.AddCode(Remodeling_Def, RemodelCode);
            }
            #endregion
            #region Spiriting (High Speed on Low Health)
            Spiriting_Def.cachedName = "StatsOnLowHealth";
            Spiriting_Def.nameToken = "ARTIFACT_SPEED_ONLOWHEALTH_NAME";
            Spiriting_Def.descriptionToken = "ARTIFACT_SPEED_ONLOWHEALTH_DESC";
            Spiriting_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Spiriting_On.png"); ;
            Spiriting_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Spiriting_Off.png"); ;
            Spiriting_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/TeamDeath").pickupModelReference;
            ContentAddition.AddArtifactDef(Spiriting_Def);
            if (WConfig.EnableSpiritualArtifact.Value == false)
            {
                Spiriting_Def.unlockableDef = AlwaysLocked;
            }
            else
            {
                ArtifactCode SpiritingCode = ScriptableObject.CreateInstance<ArtifactCode>();
                SpiritingCode.topRow = new Vector3Int(5, 3, 5);
                SpiritingCode.topRow = new Vector3Int(1, 3, 1);
                SpiritingCode.topRow = new Vector3Int(5, 3, 5);
                ArtifactCodeAPI.AddCode(Spiriting_Def, SpiritingCode);
            }
            #endregion
            #region Brigade (One Elite Type)
            Brigade_Def.cachedName = "SingleEliteType";
            Brigade_Def.nameToken = "ARTIFACT_SINGLE_ELITE_NAME";
            Brigade_Def.descriptionToken = "ARTIFACT_SINGLE_ELITE_DESC";
            Brigade_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Briaged_On.png");
            Brigade_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Briaged_Off.png");
            Brigade_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MonsterTeamGainsItems").pickupModelReference;
            ContentAddition.AddArtifactDef(Brigade_Def);
            if (WConfig.EnableBrigadeArtifact.Value == false)
            {
                Brigade_Def.unlockableDef = AlwaysLocked;
            }
            #endregion
            #region Tranpose (Metamorphosis for Loadout)
            Transpose_Def.cachedName = "RandomLoadoutOnRespawn";
            Transpose_Def.nameToken = "ARTIFACT_REROLL_SKILLS_NAME";
            Transpose_Def.descriptionToken = "ARTIFACT_REROLL_SKILLS_DESC";
            Transpose_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Transpose_On.png"); ;
            Transpose_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Transpose_Off.png"); ;
            Transpose_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/RandomSurvivorOnRespawn").pickupModelReference;
            ContentAddition.AddArtifactDef(Transpose_Def);
            if (WConfig.EnableTransposeArtifact.Value == false)
            {
                Transpose_Def.unlockableDef = AlwaysLocked;
            }
            #endregion
            #region Unison (Single Item Per Tier Per Stage)
            Unison_Def.cachedName = "SingleItemPerTier";
            Unison_Def.nameToken = "ARTIFACT_SINGLE_ITEM_NAME";
            Unison_Def.descriptionToken = "ARTIFACT_SINGLE_ITEM_DESC";
            Unison_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Unison_On.png"); ;
            Unison_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Unison_Off.png"); ;
            Unison_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/SingleMonsterType").pickupModelReference;
            ContentAddition.AddArtifactDef(Unison_Def);
            if (WConfig.EnableUnisonArtifact.Value == false)
            {
                Unison_Def.unlockableDef = AlwaysLocked;
            }
            #endregion
            #region Obscurity (Curse of the Blind)
            Obscurity_Def.cachedName = "ItemsBlind";
            Obscurity_Def.nameToken = "ARTIFACT_BLIND_ITEMS_NAME";
            Obscurity_Def.descriptionToken = "ARTIFACT_BLIND_ITEMS_DESC";
            Obscurity_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Obscurity_On.png");
            Obscurity_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Obscurity_Off.png");
            Obscurity_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/FriendlyFire").pickupModelReference;
            ContentAddition.AddArtifactDef(Obscurity_Def);
            Obscurity_Def.unlockableDef = AlwaysLocked;
            /*if (WConfig.EnableObscurityArtifact.Value == false)
            {
                Obscurity_Def.unlockableDef = AlwaysLocked;
            }*/
            #endregion

            #region Flamboyance / Mirror Enigma ; Reroll item tiers
            Flamboyance_Def.cachedName = "RandomlyRainbow";
            Flamboyance_Def.nameToken = "ARTIFACT_RANDOMLYANYTIER_NAME";
            Flamboyance_Def.descriptionToken = "ARTIFACT_RANDOMLYANYTIER_DESC";
            Flamboyance_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Flamboyance_On.png");
            Flamboyance_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Flamboyance_Off.png");
            Flamboyance_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma").pickupModelReference;
            ContentAddition.AddArtifactDef(Flamboyance_Def);
            if (WConfig.Enable_Flamboyance_Artifact.Value == false)
            {
                Flamboyance_Def.unlockableDef = AlwaysLocked;
            }
            #endregion
            #region Doubles / Mirror Swarms ; 2 Player
            Doubles_Def.cachedName = "SwarmsPlayer";
            Doubles_Def.nameToken = "ARTIFACT_DOUBLEPLAYER_NAME";
            Doubles_Def.descriptionToken = "ARTIFACT_DOUBLEPLAYER_DESC";
            Doubles_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Double_On.png");
            Doubles_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Double_Off.png");
            Doubles_Def.pickupModelReference = LegacyResourcesAPI.Load<ArtifactDef>("Artifactdefs/Swarms").pickupModelReference;
            ContentAddition.AddArtifactDef(Doubles_Def);
            if (WConfig.Enable_Doubles_Artifact.Value == false)
            {
                Doubles_Def.unlockableDef = AlwaysLocked;
            }
            #endregion



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
            MakeSimuWave();
        }


        public static void MakeSimuWave()
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


        public void RunArtifactManager_onEnabledArtifactGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == Spiriting_Def)
            {
                Spiriting.OnArtifactEnable();
            }
            else if (artifactDef == Brigade_Def)
            {
                Brigade.OnArtifactEnable();
            }
            else if (artifactDef == Wander_Def)
            {
                Wander.OnArtifactEnable();
            }
            else if (artifactDef == Transpose_Def)
            {
                Transpose.OnArtifactEnable();
            }
            else if (artifactDef == Remodeling_Def)
            {
                Remodeling.OnArtifactEnable();
            }
            else if (artifactDef == Dissimilarity_Def)
            {
                Dissimilarity.OnArtifactEnable();
            }
            else if (artifactDef == Kith_Def)
            {
                Kith.OnArtifactEnable();
            }
            else if (artifactDef == Unison_Def)
            {
                Unison.OnArtifactEnable();
            }
            else if (artifactDef == Obscurity_Def)
            {
                Obscurity.OnArtifactEnable();
            }
            else if (artifactDef == Flamboyance_Def)
            {
                Flamboyance.On_Artifact_Enable();
            }
            else if (artifactDef == Doubles_Def)
            {
                Doubles.On_Artifact_Enable();
            }
        }

        public void RunArtifactManager_onDisableArtifactGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            //Debug.LogWarning(runArtifactManager + " " + artifactDef);
            if (artifactDef == Spiriting_Def)
            {
                Spiriting.OnArtifactDisable();
            }
            else if (artifactDef == Brigade_Def)
            {
                Brigade.OnArtifactDisable();
            }
            else if (artifactDef == Wander_Def)
            {
                Wander.OnArtifactDisable();
            }
            else if (artifactDef == Transpose_Def)
            {
                Transpose.OnArtifactDisable();
            }
            else if (artifactDef == Remodeling_Def)
            {
                Remodeling.OnArtifactDisable();
            }
            else if (artifactDef == Dissimilarity_Def)
            {
                Dissimilarity.OnArtifactDisable();
            }
            else if (artifactDef == Kith_Def)
            {
                Kith.OnArtifactDisable();
            }
            else if (artifactDef == Unison_Def)
            {
                Unison.OnArtifactDisable();
            }
            else if (artifactDef == Unison_Def)
            {
                Obscurity.OnArtifactDisable();
            }
            else if (artifactDef == Flamboyance_Def)
            {
                Flamboyance.On_Artifact_Disable();
            }
            else if (artifactDef == Doubles_Def)
            {
                Doubles.On_Artifact_Disable();
            }

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

