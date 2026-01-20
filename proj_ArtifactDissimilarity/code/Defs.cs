using RoR2;
using R2API;
using R2API.ScriptableObjects;
using R2API.Utils;
using UnityEngine;
using ArtifactDissimilarity.Aritfacts;

namespace ArtifactDissimilarity
{
    public static class Defs
    {
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

        public static void MakeArtifacts()
        {
            RunArtifactManager.onArtifactEnabledGlobal += ArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += ArtifactDisabledGlobal;


            UnlockableDef AlwaysLocked = ScriptableObject.CreateInstance<UnlockableDef>();
            AlwaysLocked.cachedName = "NoMoreArtifact";

            #region Dissimilarity (Dissonance Interactable)
            Dissimilarity_Def.cachedName = "MixInteractable";
            Dissimilarity_Def.nameToken = "ARTIFACT_MIX_INTERACTABLE_NAME";
            Dissimilarity_Def.descriptionToken = "ARTIFACT_MIX_INTERACTABLE_DESC";
            Dissimilarity_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Dissimilarity_On.png");
            Dissimilarity_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Dissimilarity_Off.png");
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
            Wander_Def.cachedName = "MixStageOrder";
            Wander_Def.nameToken = "ARTIFACT_RANDOM_STAGEORDER_NAME";
            Wander_Def.descriptionToken = "ARTIFACT_RANDOM_STAGEORDER_DESC";
            Wander_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Wander_On.png");
            Wander_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Wander_Off.png");
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
            Remodeling_Def.cachedName = "RandomItemsPerStage";
            Remodeling_Def.nameToken = "ARTIFACT_REROLL_ITEMS_NAME";
            Remodeling_Def.descriptionToken = "ARTIFACT_REROLL_ITEMS_DESC";
            Remodeling_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Remodeling_On.png");
            Remodeling_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Remodeling_Off.png");
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
            Spiriting_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Spiriting_On.png");
            Spiriting_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Spiriting_Off.png");
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
            Transpose_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Transpose_On.png");
            Transpose_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Transpose_Off.png");
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
            Unison_Def.smallIconSelectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Unison_On.png");
            Unison_Def.smallIconDeselectedSprite = Assets.Bundle.LoadAsset<Sprite>("Assets/Artifacts/Unison_Off.png");
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
        }




        public static void ArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
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

        public static void ArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
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


    }
}