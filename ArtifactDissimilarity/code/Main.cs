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

namespace ArtifactDissimilarity
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Wolfo.WolfoArtifacts", "WolfoArtifacts", "3.2.0")]
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

        public void Awake()
        {
            Assets.Init(Info);
            WConfig.InitConfig();
            ArtifactAdded();
            Spiriting.Start();
            Wander.Start();
            Dissimilarity.Start();
            Kith.Start();
            WanderDissim_LunarSeer.Start();
            
            GameModeCatalog.availability.CallWhenAvailable(ModSupport);

            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onEnabledArtifactGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onDisableArtifactGlobal;


            ChatMessageBase.chatMessageTypeToIndex.Add(typeof(Brigade.BrigadeMessage), (byte)ChatMessageBase.chatMessageIndexToType.Count);
            ChatMessageBase.chatMessageIndexToType.Add(typeof(Brigade.BrigadeMessage));

            FixVoidSuppresor();
            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, WhiteToGreen").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier2");
            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, GreenToRed Variant").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier3");
            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, RedToWhite Variant").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier1");


            On.RoR2.Stage.Start += StageStartMethod;
            On.RoR2.Run.Start += RunStartHook;
            On.RoR2.Run.OnDisable += WanderEnder;

            WConfig.EnableObscurityArtifact.Value = false;
            Obscurity.Start();
        }
 

        public static void FixVoidSuppresor()
        {

            Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidSuppressor/iscVoidSuppressor.asset").WaitForCompletion().directorCreditCost = 4;
            //Since we got Void Soupper in Dissim we gotta fix the vanilla up
            GameObject VoidSuppressorPrefab = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidSuppressor/VoidSuppressor.prefab").WaitForCompletion();

            VoidSuppressorPrefab.GetComponent<PurchaseInteraction>().isShrine = true;
            VoidSuppressorPrefab.GetComponent<VoidSuppressorBehavior>().effectColor.a = 0.85f;
            VoidSuppressorPrefab.transform.GetChild(0).GetChild(7).GetChild(1).GetChild(1).gameObject.SetActive(true);


            ItemDef ScrapWhiteSuppressed = Addressables.LoadAssetAsync<ItemDef>(key: "RoR2/DLC1/ScrapVoid/ScrapWhiteSuppressed.asset").WaitForCompletion();
            ItemDef ScrapGreenSuppressed = Addressables.LoadAssetAsync<ItemDef>(key: "RoR2/DLC1/ScrapVoid/ScrapGreenSuppressed.asset").WaitForCompletion();
            ItemDef ScrapRedSuppressed = Addressables.LoadAssetAsync<ItemDef>(key: "RoR2/DLC1/ScrapVoid/ScrapRedSuppressed.asset").WaitForCompletion();

            ScrapWhiteSuppressed.pickupToken = "ITEM_SCRAPWHITE_PICKUP";
            ScrapGreenSuppressed.pickupToken = "ITEM_SCRAPGREEN_PICKUP";
            ScrapRedSuppressed.pickupToken = "ITEM_SCRAPRED_PICKUP";

            ScrapWhiteSuppressed.descriptionToken = "ITEM_SCRAPWHITE_DESC";
            ScrapGreenSuppressed.descriptionToken = "ITEM_SCRAPGREEN_DESC";
            ScrapRedSuppressed.descriptionToken = "ITEM_SCRAPRED_DESC";

            ScrapWhiteSuppressed.deprecatedTier = ItemTier.Tier1;
            ScrapGreenSuppressed.deprecatedTier = ItemTier.Tier2;
            ScrapRedSuppressed.deprecatedTier = ItemTier.Tier3;
            On.RoR2.UI.LogBook.LogBookController.BuildStaticData += (orig) =>
            {
                ScrapWhiteSuppressed.deprecatedTier = ItemTier.NoTier;
                ScrapGreenSuppressed.deprecatedTier = ItemTier.NoTier;
                ScrapRedSuppressed.deprecatedTier = ItemTier.NoTier;
                orig();
                ScrapWhiteSuppressed.deprecatedTier = ItemTier.Tier1;
                ScrapGreenSuppressed.deprecatedTier = ItemTier.Tier2;
                ScrapRedSuppressed.deprecatedTier = ItemTier.Tier3;
            };

            GameObject Hud = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/UI/HUDSimple.prefab").WaitForCompletion();
            try
            {
                Transform SuprresedItems = Hud.transform.GetChild(0).GetChild(8).GetChild(2).GetChild(8).GetChild(0).GetChild(3);
                SuprresedItems.GetComponent<RoR2.UI.ItemInventoryDisplay>().verticalMargin = 8;
            }
            catch (Exception ex) {
                Debug.LogException(ex);
            }

            
            var chestInspect = Addressables.LoadAssetAsync<InspectDef>(key: "RoR2/DLC1/VoidChest/VoidChestInspectDef.asset").WaitForCompletion();
            InspectDef inspectDef = ScriptableObject.CreateInstance<InspectDef>();
            inspectDef.name = "VoidSuppressorInspectDef";
            var Inspect = new RoR2.UI.InspectInfo();
            Inspect.TitleToken = "VOID_SUPPRESSOR_NAME";
            Inspect.DescriptionToken = "VOID_SUPPRESSOR_DESCRIPTION";
            Inspect.FlavorToken = "VOID_SUPPRESSOR_DESCRIPTION";
            Inspect.Visual = chestInspect.Info.Visual;
            inspectDef.Info = Inspect;
            var InspectInfo = VoidSuppressorPrefab.AddComponent<GenericInspectInfoProvider>();
            InspectInfo.InspectInfo = inspectDef;
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
            Rect rec = new Rect(0, 0, 64, 64);

            #region Dissimilarity (Dissonance Interactable)
            Texture2D Dissimilarity_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Dissimilarity_On.png");
            Texture2D Dissimilarity_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Dissimilarity_Off.png");
            Dissimilarity_On.filterMode = FilterMode.Trilinear;
            Dissimilarity_Off.filterMode = FilterMode.Trilinear;
            Sprite Dissimilarity_OnS = Sprite.Create(Dissimilarity_On, rec, new Vector2(0, 0));
            Sprite Dissimilarity_OffS = Sprite.Create(Dissimilarity_Off, rec, new Vector2(0, 0));

            Dissimilarity_Def.cachedName = "MixInteractable";
            Dissimilarity_Def.nameToken = "ARTIFACT_MIX_INTERACTABLE_NAME";
            Dissimilarity_Def.descriptionToken = "ARTIFACT_MIX_INTERACTABLE_DESC";
            Dissimilarity_Def.smallIconSelectedSprite = Dissimilarity_OnS;
            Dissimilarity_Def.smallIconDeselectedSprite = Dissimilarity_OffS;
            Dissimilarity_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MixEnemy").pickupModelPrefab;
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
            Texture2D Kith_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Kith_On.png");
            Texture2D Kith_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Kith_Off.png");
            Kith_On.filterMode = FilterMode.Trilinear;
            Kith_Off.filterMode = FilterMode.Trilinear;
            Sprite Kith_OnS = Sprite.Create(Kith_On, rec, new Vector2(0, 0));
            Sprite Kith_OffS = Sprite.Create(Kith_Off, rec, new Vector2(0, 0));

            Kith_Def.cachedName = "SingleInteractablePerCategory";
            Kith_Def.nameToken = "ARTIFACT_SINGLE_INTERACTABLE_NAME";
            Kith_Def.descriptionToken = "ARTIFACT_SINGLE_INTERACTABLE_DESC";
            Kith_Def.smallIconSelectedSprite = Kith_OnS;
            Kith_Def.smallIconDeselectedSprite = Kith_OffS;
            Kith_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/SingleMonsterType").pickupModelPrefab;
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
            Texture2D Wander_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Wander_On.png");
            Texture2D Wander_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Wander_Off.png");
            Wander_On.filterMode = FilterMode.Trilinear;
            Wander_Off.filterMode = FilterMode.Trilinear;
            Sprite Wander_OnS = Sprite.Create(Wander_On, rec, new Vector2(0, 0));
            Sprite Wander_OffS = Sprite.Create(Wander_Off, rec, new Vector2(0, 0));

            Wander_Def.cachedName = "MeanderStageOrder";
            Wander_Def.nameToken = "ARTIFACT_RANDOM_STAGEORDER_NAME";
            Wander_Def.descriptionToken = "ARTIFACT_RANDOM_STAGEORDER_DESC";
            Wander_Def.smallIconSelectedSprite = Wander_OnS;
            Wander_Def.smallIconDeselectedSprite = Wander_OffS;
            Wander_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma").pickupModelPrefab;
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
            Texture2D Remodeling_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Remodeling_On.png");
            Texture2D Remodeling_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Remodeling_Off.png");
            Remodeling_On.filterMode = FilterMode.Trilinear;
            Remodeling_Off.filterMode = FilterMode.Trilinear;
            Sprite Remodeling_OnS = Sprite.Create(Remodeling_On, rec, new Vector2(0, 0));
            Sprite Remodeling_OffS = Sprite.Create(Remodeling_Off, rec, new Vector2(0, 0));

            Remodeling_Def.cachedName = "RerollItemsAndEquipments";
            Remodeling_Def.nameToken = "ARTIFACT_REROLL_ITEMS_NAME";
            Remodeling_Def.descriptionToken = "ARTIFACT_REROLL_ITEMS_DESC";
            Remodeling_Def.smallIconSelectedSprite = Remodeling_OnS;
            Remodeling_Def.smallIconDeselectedSprite = Remodeling_OffS;
            Remodeling_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/RandomSurvivorOnRespawn").pickupModelPrefab;
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
            Texture2D Spiriting_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Spiriting_On.png");
            Texture2D Spiriting_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Spiriting_Off.png");
            Spiriting_On.filterMode = FilterMode.Trilinear;
            Spiriting_Off.filterMode = FilterMode.Trilinear;
            Sprite Spiriting_OnS = Sprite.Create(Spiriting_On, rec, new Vector2(0, 0));
            Sprite Spiriting_OffS = Sprite.Create(Spiriting_Off, rec, new Vector2(0, 0));

            Spiriting_Def.cachedName = "StatsOnLowHealth";
            Spiriting_Def.nameToken = "ARTIFACT_SPEED_ONLOWHEALTH_NAME";
            Spiriting_Def.descriptionToken = "ARTIFACT_SPEED_ONLOWHEALTH_DESC";
            Spiriting_Def.smallIconSelectedSprite = Spiriting_OnS;
            Spiriting_Def.smallIconDeselectedSprite = Spiriting_OffS;
            Spiriting_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/TeamDeath").pickupModelPrefab;
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
            Texture2D Briaged_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Briaged_On.png");
            Texture2D Briaged_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Briaged_Off.png");
            Briaged_On.filterMode = FilterMode.Trilinear;
            Briaged_Off.filterMode = FilterMode.Trilinear;
            Sprite Briaged_OnS = Sprite.Create(Briaged_On, rec, new Vector2(0, 0));
            Sprite Briaged_OffS = Sprite.Create(Briaged_Off, rec, new Vector2(0, 0));

            Brigade_Def.cachedName = "SingleEliteType";
            Brigade_Def.nameToken = "ARTIFACT_SINGLE_ELITE_NAME";
            Brigade_Def.descriptionToken = "ARTIFACT_SINGLE_ELITE_DESC";
            Brigade_Def.smallIconSelectedSprite = Briaged_OnS;
            Brigade_Def.smallIconDeselectedSprite = Briaged_OffS;
            Brigade_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MonsterTeamGainsItems").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Brigade_Def);
            if (WConfig.EnableBrigadeArtifact.Value == false)
            {
                Brigade_Def.unlockableDef = AlwaysLocked;
            }
            #endregion
            #region Tranpose (Metamorphosis for Loadout)
            Texture2D Transpose_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Transpose_On.png");
            Texture2D Transpose_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Transpose_Off.png");
            Transpose_On.filterMode = FilterMode.Trilinear;
            Transpose_Off.filterMode = FilterMode.Trilinear;
            Sprite Transpose_OnS = Sprite.Create(Transpose_On, rec, new Vector2(0, 0));
            Sprite Transpose_OffS = Sprite.Create(Transpose_Off, rec, new Vector2(0, 0));

            Transpose_Def.cachedName = "RandomLoadoutOnRespawn";
            Transpose_Def.nameToken = "ARTIFACT_REROLL_SKILLS_NAME";
            Transpose_Def.descriptionToken = "ARTIFACT_REROLL_SKILLS_DESC";
            Transpose_Def.smallIconSelectedSprite = Transpose_OnS;
            Transpose_Def.smallIconDeselectedSprite = Transpose_OffS;
            Transpose_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/RandomSurvivorOnRespawn").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Transpose_Def);
            if (WConfig.EnableTransposeArtifact.Value == false)
            {
                Transpose_Def.unlockableDef = AlwaysLocked;
            }
            #endregion
            #region Unison (Single Item Per Tier Per Stage)
            Texture2D Unison_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Unison_On.png");
            Texture2D Unison_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Unison_Off.png");
            Unison_On.filterMode = FilterMode.Trilinear;
            Unison_Off.filterMode = FilterMode.Trilinear;
            Sprite Unison_OnS = Sprite.Create(Unison_On, rec, new Vector2(0, 0));
            Sprite Unison_OffS = Sprite.Create(Unison_Off, rec, new Vector2(0, 0));

            Unison_Def.cachedName = "SingleItemPerTier";
            Unison_Def.nameToken = "ARTIFACT_SINGLE_ITEM_NAME";
            Unison_Def.descriptionToken = "ARTIFACT_SINGLE_ITEM_DESC";
            Unison_Def.smallIconSelectedSprite = Unison_OnS;
            Unison_Def.smallIconDeselectedSprite = Unison_OffS;
            Unison_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/SingleMonsterType").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Unison_Def);
            if (WConfig.EnableUnisonArtifact.Value == false)
            {
                Unison_Def.unlockableDef = AlwaysLocked;
            }
            #endregion
            #region Obscurity (Curse of the Blind)
            Texture2D Egg_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Egg_On.png");
            Texture2D Egg_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Egg_Off.png");
            Egg_On.filterMode = FilterMode.Trilinear;
            Egg_Off.filterMode = FilterMode.Trilinear;
            Sprite Egg_OnS = Sprite.Create(Egg_On, rec, new Vector2(0, 0));
            Sprite Egg_OffS = Sprite.Create(Egg_Off, rec, new Vector2(0, 0));

            Obscurity_Def.cachedName = "ItemsBlind";
            Obscurity_Def.nameToken = "ARTIFACT_BLIND_ITEMS_NAME";
            Obscurity_Def.descriptionToken = "ARTIFACT_BLIND_ITEMS_DESC";
            Obscurity_Def.smallIconSelectedSprite = Egg_OnS;
            Obscurity_Def.smallIconDeselectedSprite = Egg_OffS;
            Obscurity_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/FriendlyFire").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Obscurity_Def);
            if (WConfig.EnableObscurityArtifact.Value == false)
            {
                Obscurity_Def.unlockableDef = AlwaysLocked;
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
            #region Simu Waves
            #region Augment of Brigade
            GameObject InfiniteTowerWaveArtifactSingleEliteType = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactSingleEliteType", true);
            GameObject InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI", false);
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
            GameObject InfiniteTowerWaveArtifactStatsOnLowHealth = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactStatsOnLowHealth", true);
            GameObject InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI", false);
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
            GameObject InfiniteTowerWaveArtifactRandomLoadout = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactRandomLoadout", true);
            GameObject InfiniteTowerCurrentArtifactRandomLoadoutWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactRandomLoadoutWaveUI", false);
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
            RoR2.InfiniteTowerWaveCategory ITBasicWaves = Addressables.LoadAssetAsync<RoR2.InfiniteTowerWaveCategory>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveCategories/CommonWaveCategory.asset").WaitForCompletion();
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

        }






        internal static void ModSupport()
        {
            Dissimilarity.ModSupport();
            

            for (int i = 0; i < ArtifactCatalog.artifactCount; i++)
            {
                ArtifactDef temp = ArtifactCatalog.GetArtifactDef((ArtifactIndex)i);
                if (temp.pickupModelPrefab == null)
                {
                    temp.pickupModelPrefab = RoR2Content.Artifacts.EliteOnly.pickupModelPrefab;
                }
            }
        }


         
    }

}

