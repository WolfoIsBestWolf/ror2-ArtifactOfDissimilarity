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
    [BepInPlugin("com.Wolfo.WolfoArtifacts", "WolfoArtifacts", "2.5.2")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class Main : BaseUnityPlugin
    {
        public static readonly System.Random random = new System.Random();

        public static bool DreamPrimordialBool = false;

        public static SceneDef WanderPreviousSceneDef = null;

        public static ArtifactDef Command = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Command");
        public static ArtifactDef MonsterTeamGain = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MonsterTeamGainsItems");
        public static ArtifactDef RiskyConformity;
        //public static ArtifactDef tempartifact = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/FriendlyFire");
        //
        //
        public static ArtifactDef Dissimilarity_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Kith_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Wander_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Remodeling_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Spiriting_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Brigade_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Transpose_Def = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Unison_Def = ScriptableObject.CreateInstance<ArtifactDef>();

        public static SceneExitController tempexitcontroller;

        public void Awake()
        {
            WConfig.InitConfig();
            ArtifactAdded();

            Spiriting.Start();
            Wander.Start();
            Dissimilarity.Start();
            Kith.Start();

            GameModeCatalog.availability.CallWhenAvailable(ModSupport);

            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onEnabledArtifactGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onDisableArtifactGlobal;

            On.RoR2.SceneDirector.Start += OneTimeSceneDic;
            RoR2.SceneDirector.onGenerateInteractableCardSelection += SceneDirector_onGenerateInteractableCardSelection;

            On.RoR2.SceneDirector.Start += ArtifactCheckerOnStageAwake;

            FixVoidSuppresor();

            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, WhiteToGreen").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier2");
            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, GreenToRed Variant").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier3");
            LegacyResourcesAPI.Load<GameObject>("Prefabs/networkedobjects/LunarCauldron, RedToWhite Variant").GetComponent<ShopTerminalBehavior>().dropTable = LegacyResourcesAPI.Load<BasicPickupDropTable>("DropTables/dtDuplicatorTier1");


            //What the hell are these
            //This was like some, carry over items between Void Fields
            //As to not make going to it multiple times hella boring
            //But like I wouldn't do that to myself once to begin with
            /* On.RoR2.ArenaMissionController.Awake += (orig, self) =>
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
            };*/
        }

        public static void FixVoidSuppresor()
        {

            Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC1/VoidSuppressor/iscVoidSuppressor.asset").WaitForCompletion().directorCreditCost = 5;
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
        }


        public static void ArtifactCheckerOnStageAwake(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            if (NetworkServer.active && RunArtifactManager.instance)
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(Brigade_Def))
                {
                    Brigade.EliteKinAsMethod();
                }
            }
            orig(self);
        }

        public static void RunStartHook(On.RoR2.Run.orig_Start orig, Run self)
        {
            //Debug.LogWarning(self);
            //Debug.LogWarning(Run.instance);
            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Wander_Def))
                {
                    Wander.WanderSetup();
                }
            }
            orig(self);
            if (NetworkServer.active)
            {
                //ArenaInventory = new Inventory();
                Kith.KithNoRepeat = null;
                Brigade.SetupBrigade();
            }
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
                Wander.WanderUnSet();
            }
            orig(self);
        }

        public static void StageStartMethod(On.RoR2.Stage.orig_Start orig, RoR2.Stage self)
        {
            //Wander.WanderStageStart();

            orig(self);
            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Brigade_Def))
                {
                    if (!SceneInfo.instance) { return; }
                    self.StartCoroutine(DelayedChatMessage(Brigade.SendBrigadeMessage(), 1.5f));
                }
                RoR2.ArtifactTrialMissionController.trialArtifact = ArtifactCatalog.GetArtifactDef((ArtifactIndex)random.Next(ArtifactCatalog.artifactCount));
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

            int[] invoutput = new int[EquipmentCatalog.equipmentCount];

            System.Collections.Generic.List<EquipmentIndex> FullEquipmentList = new System.Collections.Generic.List<EquipmentIndex>();
            FullEquipmentList.AddRange(EquipmentCatalog.equipmentList);

            for (var i = 0; i < invoutput.Length; i++)
            {
                EquipmentDef tempDef = EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i);

                if (tempDef.passiveBuffDef && tempDef.passiveBuffDef.isElite)
                {
                    string tempname = tempDef.name;
                    if (tempname.StartsWith("EliteGold") || tempname.StartsWith("EliteEcho") || tempname.StartsWith("EliteSecretSpeed"))
                    {
                    }
                    else
                    {
                        Remodeling.EliteEquipmentList.Add((EquipmentIndex)i);
                    }
                }
            }

            On.RoR2.SceneDirector.Start -= OneTimeSceneDic;
        }

        public static void ArtifactAdded()
        {
            UnlockableDef NoMoreArtifact = ScriptableObject.CreateInstance<UnlockableDef>();
            NoMoreArtifact.cachedName = "NoMoreArtifact";
            Dissimilarity_Def.cachedName = "MixInteractable";
            Kith_Def.cachedName = "SingleInteractablePerCategory";
            Wander_Def.cachedName = "MeanderStageOrder";
            Remodeling_Def.cachedName = "RerollItemsAndEquipments";
            Spiriting_Def.cachedName = "StatsOnLowHealth";
            Transpose_Def.cachedName = "RandomLoadoutOnRespawn";
            Brigade_Def.cachedName = "SingleEliteType";
            Unison_Def.cachedName = "SingleItemPerTier";


            //Texture garbage
            Texture2D DissArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D DissArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D KithArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D KithArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D WanderArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D WanderArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D RemodelingArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D RemodelingArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D SpiritingArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D SpiritingArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D BrigadeArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D BrigadeArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D TransposeArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D TransposeArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D UnisonArtifactOn = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            Texture2D UnisonArtifactOff = new Texture2D(64, 64, TextureFormat.RGBA32, false);

            DissArtifactOn.filterMode = FilterMode.Trilinear;
            DissArtifactOff.filterMode = FilterMode.Trilinear;
            KithArtifactOn.filterMode = FilterMode.Trilinear;
            KithArtifactOff.filterMode = FilterMode.Trilinear;
            WanderArtifactOn.filterMode = FilterMode.Trilinear;
            WanderArtifactOff.filterMode = FilterMode.Trilinear;
            RemodelingArtifactOn.filterMode = FilterMode.Trilinear;
            RemodelingArtifactOff.filterMode = FilterMode.Trilinear;
            SpiritingArtifactOn.filterMode = FilterMode.Trilinear;
            SpiritingArtifactOff.filterMode = FilterMode.Trilinear;
            BrigadeArtifactOn.filterMode = FilterMode.Trilinear;
            BrigadeArtifactOff.filterMode = FilterMode.Trilinear;
            TransposeArtifactOn.filterMode = FilterMode.Trilinear;
            TransposeArtifactOff.filterMode = FilterMode.Trilinear;
            UnisonArtifactOn.filterMode = FilterMode.Trilinear;
            UnisonArtifactOff.filterMode = FilterMode.Trilinear;


            DissArtifactOn.LoadImage(Properties.Resources.Dissimilarity_on, true);
            DissArtifactOff.LoadImage(Properties.Resources.Dissimilarity_off, true);
            KithArtifactOn.LoadImage(Properties.Resources.Kith_on, true);
            KithArtifactOff.LoadImage(Properties.Resources.Kith_off, true);
            WanderArtifactOn.LoadImage(Properties.Resources.Wander_on, true);
            WanderArtifactOff.LoadImage(Properties.Resources.Wander_off, true);
            RemodelingArtifactOn.LoadImage(Properties.Resources.Remodeling_on, true);
            RemodelingArtifactOff.LoadImage(Properties.Resources.Remodeling_off, true);
            SpiritingArtifactOn.LoadImage(Properties.Resources.Spiriting_on, true);
            SpiritingArtifactOff.LoadImage(Properties.Resources.Spiriting_off, true);
            BrigadeArtifactOn.LoadImage(Properties.Resources.Briaged_on, true);
            BrigadeArtifactOff.LoadImage(Properties.Resources.Brigade_off, true);
            TransposeArtifactOn.LoadImage(Properties.Resources.Transpose_on, true);
            TransposeArtifactOff.LoadImage(Properties.Resources.Transpose_off, true);
            UnisonArtifactOn.LoadImage(Properties.Resources.Unison_On, true);
            UnisonArtifactOff.LoadImage(Properties.Resources.Unison_Off, true);

            Rect rec = new Rect(0, 0, DissArtifactOn.width, DissArtifactOn.height);
            Sprite DisimOn = Sprite.Create(DissArtifactOn, rec, new Vector2(0, 0));
            Sprite DisimOff = Sprite.Create(DissArtifactOff, rec, new Vector2(0, 0));
            Sprite KithOn = Sprite.Create(KithArtifactOn, rec, new Vector2(0, 0));
            Sprite KithOff = Sprite.Create(KithArtifactOff, rec, new Vector2(0, 0));
            Sprite WanderOn = Sprite.Create(WanderArtifactOn, rec, new Vector2(0, 0));
            Sprite WanderOff = Sprite.Create(WanderArtifactOff, rec, new Vector2(0, 0));
            Sprite RemodelingOn = Sprite.Create(RemodelingArtifactOn, rec, new Vector2(0, 0));
            Sprite RemodelingOff = Sprite.Create(RemodelingArtifactOff, rec, new Vector2(0, 0));
            Sprite SpiritingOn = Sprite.Create(SpiritingArtifactOn, rec, new Vector2(0, 0));
            Sprite SpiritingOff = Sprite.Create(SpiritingArtifactOff, rec, new Vector2(0, 0));
            Sprite BrigadeOn = Sprite.Create(BrigadeArtifactOn, rec, new Vector2(0, 0));
            Sprite BrigadeOff = Sprite.Create(BrigadeArtifactOff, rec, new Vector2(0, 0));
            Sprite TransposeOn = Sprite.Create(TransposeArtifactOn, rec, new Vector2(0, 0));
            Sprite TransposeOff = Sprite.Create(TransposeArtifactOff, rec, new Vector2(0, 0));
            Sprite UnisonArtifactOnS = Sprite.Create(UnisonArtifactOn, rec, new Vector2(0, 0));
            Sprite UnisonArtifactOffS = Sprite.Create(UnisonArtifactOff, rec, new Vector2(0, 0));

            //
            ArtifactCode DissimCode = ScriptableObject.CreateInstance<ArtifactCode>();
            ArtifactCode KithCode = ScriptableObject.CreateInstance<ArtifactCode>();
            ArtifactCode WanderCode = ScriptableObject.CreateInstance<ArtifactCode>();
            ArtifactCode RemodelCode = ScriptableObject.CreateInstance<ArtifactCode>();
            ArtifactCode SpiritingCode = ScriptableObject.CreateInstance<ArtifactCode>();

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
            Dissimilarity_Def.nameToken = "Artifact of Dissimilarity";
            Dissimilarity_Def.descriptionToken = "Interactables can appear outside their usual environments.";
            Dissimilarity_Def.smallIconSelectedSprite = DisimOn;
            Dissimilarity_Def.smallIconDeselectedSprite = DisimOff;
            Dissimilarity_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MixEnemy").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Dissimilarity_Def);
            if (WConfig.EnableDissim.Value == false)
            {
                Dissimilarity_Def.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //DissimCode.ArtifactCompounds = new List<int> { 1, 1, 5, 1, 1, 1, 5, 1, 1 };
                DissimCode.topRow = new Vector3Int(1, 1, 5);
                DissimCode.topRow = new Vector3Int(1, 1, 1);
                DissimCode.topRow = new Vector3Int(5, 1, 1);
                ArtifactCodeAPI.AddCode(Dissimilarity_Def, DissimCode);
            }

            //Debug.Log("Loading Artifact of Kith");
            Kith_Def.nameToken = "Artifact of Kith";
            Kith_Def.descriptionToken = "Each Interactable category will only contain one entry per stage.";
            Kith_Def.smallIconSelectedSprite = KithOn;
            Kith_Def.smallIconDeselectedSprite = KithOff;
            Kith_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/SingleMonsterType").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Kith_Def);
            if (WConfig.EnableKith.Value == false)
            {
                Kith_Def.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //KithCode.ArtifactCompounds = new List<int> { 3, 5, 3, 3, 5, 3, 1, 1, 1 };
                KithCode.topRow = new Vector3Int(3, 5, 3);
                KithCode.topRow = new Vector3Int(3, 5, 3);
                KithCode.topRow = new Vector3Int(1, 1, 1);
                ArtifactCodeAPI.AddCode(Kith_Def, KithCode);
            }

            //Debug.Log("Loading Artifact of Wander");
            Wander_Def.nameToken = "Artifact of Wander";
            //Wander.descriptionToken = "Stages progress in a random order.";
            //Wander.descriptionToken = "Stages progress in a random order.\nSimulacrum/Prismatic: Start in a random stage, normal stage order.";
            //Wander.descriptionToken = "Stages progress in a random order.\nAlt Gamemodes: normal stage order.";
            Wander_Def.descriptionToken = "Stages progress in a random order.\nSimulacrum: stages are in order.";
            Wander_Def.smallIconSelectedSprite = WanderOn;
            Wander_Def.smallIconDeselectedSprite = WanderOff;
            Wander_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Wander_Def);
            if (WConfig.EnableWanderArtifact.Value == false)
            {
                Wander_Def.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //WanderCode.ArtifactCompounds = new List<int> { 3, 7, 3, 7, 7, 7, 3, 7, 3 };
                WanderCode.topRow = new Vector3Int(3, 7, 3);
                WanderCode.topRow = new Vector3Int(7, 7, 7);
                WanderCode.topRow = new Vector3Int(3, 7, 3);
                ArtifactCodeAPI.AddCode(Wander_Def, WanderCode);
            }


            On.RoR2.Stage.Start += StageStartMethod;
            On.RoR2.Run.Start += RunStartHook;
            On.RoR2.Run.OnDisable += WanderEnder;
            On.RoR2.SceneDirector.PlaceTeleporter += WanderLunarTeleporter;

            //Debug.Log("Loading Artifact of Remodeling");
            Remodeling_Def.nameToken = "Artifact of Remodeling";
            Remodeling_Def.descriptionToken = "Reroll all passive items and equipments each stage.";
            Remodeling_Def.smallIconSelectedSprite = RemodelingOn;
            Remodeling_Def.smallIconDeselectedSprite = RemodelingOff;
            Remodeling_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/RandomSurvivorOnRespawn").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Remodeling_Def);



            if (WConfig.EnableRemodelArtifact.Value == false)
            {
                Remodeling_Def.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //RemodelCode.ArtifactCompounds = new List<int> { 1, 7, 1, 1, 5, 1, 1, 7, 1 };
                RemodelCode.topRow = new Vector3Int(1, 7, 1);
                RemodelCode.topRow = new Vector3Int(1, 5, 1);
                RemodelCode.topRow = new Vector3Int(1, 7, 1);
                ArtifactCodeAPI.AddCode(Remodeling_Def, RemodelCode);
            }


            //Debug.Log("Loading Artifact of Spiriting");
            Spiriting_Def.nameToken = "Artifact of Spiriting";
            Spiriting_Def.descriptionToken = "All characters move and attack faster the lower their health gets.";
            Spiriting_Def.smallIconSelectedSprite = SpiritingOn;
            Spiriting_Def.smallIconDeselectedSprite = SpiritingOff;
            Spiriting_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/TeamDeath").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Spiriting_Def);
            if (WConfig.EnableSpiritualArtifact.Value == false)
            {
                Spiriting_Def.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //SpiritingCode.ArtifactCompounds = new List<int> { 5, 3, 5, 1, 3, 1, 5, 3, 5 };
                SpiritingCode.topRow = new Vector3Int(5, 3, 5);
                SpiritingCode.topRow = new Vector3Int(1, 3, 1);
                SpiritingCode.topRow = new Vector3Int(5, 3, 5);
                ArtifactCodeAPI.AddCode(Spiriting_Def, SpiritingCode);
            }


            //Debug.Log("Loading Artifact of Brigade");
            Brigade_Def.nameToken = "Artifact of Brigade";
            Brigade_Def.descriptionToken = "All elites will be the same type per stage.";
            Brigade_Def.smallIconSelectedSprite = BrigadeOn;
            Brigade_Def.smallIconDeselectedSprite = BrigadeOff;
            Brigade_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/MonsterTeamGainsItems").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Brigade_Def);
            if (WConfig.EnableBrigadeArtifact.Value == false)
            {
                Brigade_Def.unlockableDef = NoMoreArtifact;
            }
            else
            {
                //BrigadeCode.ArtifactCompounds = new List<int> { 7, 1, 1, 5, 1, 5, 3, 7, 7 };
                //ArtifactCodeAPI.AddCode(Brigade, BrigadeCode);
            }

            //Debug.Log("Loading Artifact of Transpose");
            Transpose_Def.nameToken = "Artifact of Transpose";
            Transpose_Def.descriptionToken = "Get a randomized skill loadout every stage.";
            Transpose_Def.smallIconSelectedSprite = TransposeOn;
            Transpose_Def.smallIconDeselectedSprite = TransposeOff;
            Transpose_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/RandomSurvivorOnRespawn").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Transpose_Def);
            if (WConfig.EnableTransposeArtifact.Value == false)
            {
                Transpose_Def.unlockableDef = NoMoreArtifact;
            }


            Unison_Def.nameToken = "Artifact of Unison";
            Unison_Def.descriptionToken = "All item tiers only contain one item per stage.\nPrinters are unaffected.";
            Unison_Def.smallIconSelectedSprite = UnisonArtifactOnS;
            Unison_Def.smallIconDeselectedSprite = UnisonArtifactOffS;
            Unison_Def.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/SingleMonsterType").pickupModelPrefab;
            ContentAddition.AddArtifactDef(Unison_Def);
            if (WConfig.EnableUnisonArtifact.Value == false)
            {
                Unison_Def.unlockableDef = NoMoreArtifact;
            }




            //Simu Stuff
            GameObject InfiniteTowerWaveArtifactSingleEliteType = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactSingleEliteType", true);
            GameObject InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacSingleEliteTypeDisabledPrerequisite = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();
            //ArtifactDef 

            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<ArtifactEnabler>().artifactDef = Brigade_Def;
            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI;
            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<CombatDirector>().eliteBias = 0.25f;
            InfiniteTowerWaveArtifactSingleEliteType.GetComponent<InfiniteTowerWaveController>().baseCredits = 200;

            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Brigade_Def.smallIconSelectedSprite;
            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "Wave {0} - Augment of Brigade";
            InfiniteTowerCurrentArtifactSingleEliteTypeWaveUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "All elites will be of the same type.";

            ArtifacSingleEliteTypeDisabledPrerequisite.bannedArtifact = Brigade_Def;
            ArtifacSingleEliteTypeDisabledPrerequisite.name = "ArtifacSingleEliteTypeDisabledPrerequisite";

            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactSingleEliteType = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifactSingleEliteType, weight = 2, prerequisites = ArtifacSingleEliteTypeDisabledPrerequisite };
            //

            //
            GameObject InfiniteTowerWaveArtifactRandomLoadout = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactRandomLoadout", true);
            GameObject InfiniteTowerCurrentArtifactRandomLoadoutWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactRandomLoadoutWaveUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacRandomLoadoutDisabledPrerequisite = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();
            //ArtifactDef 

            InfiniteTowerWaveArtifactRandomLoadout.GetComponent<ArtifactEnabler>().artifactDef = Transpose_Def;
            InfiniteTowerWaveArtifactRandomLoadout.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerCurrentArtifactRandomLoadoutWaveUI;
            InfiniteTowerWaveArtifactRandomLoadout.GetComponent<InfiniteTowerWaveController>().baseCredits = 200;

            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Transpose_Def.smallIconSelectedSprite;
            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "Wave {0} - Augment of Transpose";
            InfiniteTowerCurrentArtifactRandomLoadoutWaveUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "Use a random loadout for this wave.";

            ArtifacRandomLoadoutDisabledPrerequisite.bannedArtifact = Transpose_Def;
            ArtifacRandomLoadoutDisabledPrerequisite.name = "ArtifacRandomLoadoutDisabledPrerequisite";

            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactRandomLoadout = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifactRandomLoadout, weight = 1f, prerequisites = ArtifacRandomLoadoutDisabledPrerequisite };
            //

            //
            GameObject InfiniteTowerWaveArtifactStatsOnLowHealth = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactStatsOnLowHealth", true);
            GameObject InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacStatsOnLowHealthDisabledPrerequisite = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();

            InfiniteTowerWaveArtifactStatsOnLowHealth.GetComponent<ArtifactEnabler>().artifactDef = Spiriting_Def;
            InfiniteTowerWaveArtifactStatsOnLowHealth.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI;
            InfiniteTowerWaveArtifactStatsOnLowHealth.GetComponent<InfiniteTowerWaveController>().baseCredits = 200;

            InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Spiriting_Def.smallIconSelectedSprite;
            InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "Wave {0} - Augment of Spiriting";
            InfiniteTowerCurrentArtifactStatsOnLowHealthWaveUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "Characters move and attack faster at lower health.";

            ArtifacStatsOnLowHealthDisabledPrerequisite.bannedArtifact = Spiriting_Def;
            ArtifacStatsOnLowHealthDisabledPrerequisite.name = "ArtifacStatsOnLowHealthDisabledPrerequisite";

            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactStatsOnLowHealth = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifactStatsOnLowHealth, weight = 1f, prerequisites = ArtifacStatsOnLowHealthDisabledPrerequisite };
            //
            /*
            GameObject InfiniteTowerWaveArtifact_Unison = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveArtifactBomb.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifactSingleItemPerTier", true);
            GameObject InfiniteTowerWaveArtifact_UnisonUI = R2API.PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerCurrentArtifactBombWaveUI.prefab").WaitForCompletion(), "InfiniteTowerWaveArtifact_UnisonUI", false);
            InfiniteTowerWaveArtifactPrerequisites ArtifacPreq_Unision = ScriptableObject.CreateInstance<RoR2.InfiniteTowerWaveArtifactPrerequisites>();

            InfiniteTowerWaveArtifact_Unison.GetComponent<ArtifactEnabler>().artifactDef = Unison_Def;
            InfiniteTowerWaveArtifact_Unison.GetComponent<InfiniteTowerWaveController>().overlayEntries[1].prefab = InfiniteTowerWaveArtifact_UnisonUI;
            InfiniteTowerWaveArtifact_Unison.GetComponent<InfiniteTowerWaveController>().baseCredits *= 1.25f;

            InfiniteTowerWaveArtifact_UnisonUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = Unison_Def.smallIconSelectedSprite;
            InfiniteTowerWaveArtifact_UnisonUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<RoR2.UI.InfiniteTowerWaveCounter>().token = "Wave {0} - Augment of Unison";
            InfiniteTowerWaveArtifact_UnisonUI.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<RoR2.UI.LanguageTextMeshController>().token = "All item tiers contain only one item.";

            ArtifacPreq_Unision.bannedArtifact = Unison_Def;
            ArtifacPreq_Unision.name = "ArtifacPreq_Unision";

            InfiniteTowerWaveCategory.WeightedWave ITBasicArtifactUnison = new InfiniteTowerWaveCategory.WeightedWave { wavePrefab = InfiniteTowerWaveArtifact_Unison, weight = 1f, prerequisites = ArtifacPreq_Unision };
            */

            RoR2.InfiniteTowerWaveCategory ITBasicWaves = Addressables.LoadAssetAsync<RoR2.InfiniteTowerWaveCategory>(key: "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/InfiniteTowerWaveCategories/CommonWaveCategory.asset").WaitForCompletion();
            ITBasicWaves.wavePrefabs = ITBasicWaves.wavePrefabs.Add(ITBasicArtifactSingleEliteType, ITBasicArtifactRandomLoadout, ITBasicArtifactStatsOnLowHealth);

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

        public void RunArtifactManager_onEnabledArtifactGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            if (artifactDef == Spiriting_Def)
            {
                //For some reason needs to be added on Clients?
                On.RoR2.CharacterBody.RecalculateStats += Spiriting.RecalcStats;
                On.RoR2.HealthComponent.TakeDamage += Spiriting.RecalcOnDamage;
                On.RoR2.HealthComponent.SendHeal += Spiriting.RecalcOnHeal;
                On.RoR2.CharacterMotor.OnLanded += Spiriting.RecalcOnLand;
                On.RoR2.Projectile.ProjectileController.Start += Spiriting.RecalcProjectileSpeed;
                On.RoR2.SetStateOnHurt.SetStun += Spiriting.RecalcStunDuration;
                Debug.Log("Added Spirit");
            }
            else if (artifactDef == Brigade_Def)
            {
                if (Brigade.ForUsageEliteDefList.Count > 0 && SceneInfo.instance && Run.instance)
                {
                    Brigade.EliteKinAsMethod();
                    string token = Brigade.SendBrigadeMessage();
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = token
                    });
                }

                Debug.Log("Added Brigade");
            }
            else if (artifactDef == Wander_Def)
            {
                On.RoR2.Run.PickNextStageScene += Wander.WanderNoRepeatStages;
                if (Run.instance)
                {
                    Wander.WanderSetup();
                    Debug.Log("Added Wander");
                }
            }
            else if (artifactDef == Transpose_Def)
            {
                On.RoR2.CharacterMaster.Respawn += Transpose.RandomizeLoadoutRespawnMethod;
                On.RoR2.CharacterMaster.PickRandomSurvivorBodyPrefab += Transpose.Transpose_Metamorphosis;
                if (WConfig.TransposeRerollHeresy.Value == true)
                {
                    On.RoR2.SceneDirector.Start += Transpose.RandomizeHeresyItems;
                }
                if (NetworkServer.active)
                {
                    foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                    {
                        playerCharacterMasterController.StopAllCoroutines();
                        playerCharacterMasterController.StartCoroutine(DelayedRespawn(playerCharacterMasterController, 0.35f));
                        //Transpose.RerollLoadout(playerCharacterMasterController.master.GetBodyObject(), playerCharacterMasterController.master);
                    }
                }
                Debug.Log("Added Transpose");
            }
            else if (artifactDef == Remodeling_Def)
            {
                On.RoR2.SceneDirector.Start += Remodeling.RandomizeMain;
                Debug.Log("Added Remodeling");
            }
            else if (artifactDef == Dissimilarity_Def)
            {
                if (Kith.KithAdded == false)
                {
                    On.RoR2.SceneDirector.GenerateInteractableCardSelection += Dissimilarity.MixInteractableApplier;
                    Dissimilarity.DissimAdded = true;
                }
            }
            else if (artifactDef == Kith_Def)
            {
                if (Dissimilarity.DissimAdded == true)
                {
                    On.RoR2.SceneDirector.GenerateInteractableCardSelection -= Dissimilarity.MixInteractableApplier;
                    Dissimilarity.DissimAdded = false;
                }
                //Handles both Kith & Kith+Dissim
                On.RoR2.SceneDirector.GenerateInteractableCardSelection += Kith.SingleInteractableApplier;
                Kith.KithAdded = true;
            }
            else if (artifactDef == Unison_Def)
            {
                Unison.OnArtifactEnable();
            }
        }

        public void RunArtifactManager_onDisableArtifactGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifactDef)
        {
            //Debug.LogWarning(runArtifactManager + " " + artifactDef);

            if (artifactDef == Spiriting_Def)
            {
                On.RoR2.CharacterBody.RecalculateStats -= Spiriting.RecalcStats;
                On.RoR2.HealthComponent.TakeDamage -= Spiriting.RecalcOnDamage;
                On.RoR2.HealthComponent.SendHeal -= Spiriting.RecalcOnHeal;
                On.RoR2.Projectile.ProjectileController.Start -= Spiriting.RecalcProjectileSpeed;
                On.RoR2.CharacterMotor.OnLanded -= Spiriting.RecalcOnLand;
                On.RoR2.SetStateOnHurt.SetStun -= Spiriting.RecalcStunDuration;
                Debug.Log("Removed Spirit");
                //Because we fuck up the base stats just respawning seems the easiest way to fix it
                foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                {
                    playerCharacterMasterController.StopAllCoroutines();
                    playerCharacterMasterController.StartCoroutine(DelayedRespawn(playerCharacterMasterController, 0.1f));
                    CharacterBody body = playerCharacterMasterController.master.GetBody();
                    if (body)
                    {
                        //Spiriting.RemoveSpiritBuffsMethod(body, playerCharacterMasterController.master.bodyPrefab.GetComponent<CharacterBody>());
                    }
                    
                }
            }
            else if (artifactDef == Brigade_Def)
            {
                //On.RoR2.CombatDirector.Awake -= EliteKinArtifact;
                if (Brigade.DidBrigadeHappen == true)
                {
                    CombatDirector.eliteTiers = Brigade.normalelitetierdefs;
                    Brigade.DidBrigadeHappen = false;
                    Debug.Log("UnBrigading");
                }
            }
            else if (artifactDef == Wander_Def)
            {
                On.RoR2.Run.PickNextStageScene -= Wander.WanderNoRepeatStages;
                if (Run.instance)
                {
                    Wander.WanderUnSet();
                }
            }
            else if (artifactDef == Transpose_Def)
            {
                On.RoR2.CharacterMaster.Respawn -= Transpose.RandomizeLoadoutRespawnMethod;
                On.RoR2.CharacterMaster.PickRandomSurvivorBodyPrefab -= Transpose.Transpose_Metamorphosis;
                if (WConfig.TransposeRerollHeresy.Value == true)
                {
                    On.RoR2.SceneDirector.Start -= Transpose.RandomizeHeresyItems;
                }
                foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                {
                    playerCharacterMasterController.networkUser.CopyLoadoutToMaster();
                    playerCharacterMasterController.StopAllCoroutines();
                    playerCharacterMasterController.StartCoroutine(DelayedRespawn(playerCharacterMasterController, 0.1f));
                    /*CharacterBody body = playerCharacterMasterController.master.GetBody();
                    if (body)
                    {
                        body.SetLoadoutServer(playerCharacterMasterController.master.loadout);
                        ModelSkinController model = body.modelLocator.modelTransform.gameObject.GetComponent<ModelSkinController>();
                        if (model)
                        {
                            model.ApplySkin((int)playerCharacterMasterController.master.loadout.bodyLoadoutManager.GetSkinIndex(playerCharacterMasterController.master.backupBodyIndex));
                        }
                    }*/
                };
            }
            else if (artifactDef == Remodeling_Def)
            {
                On.RoR2.SceneDirector.Start -= Remodeling.RandomizeMain;
            }
            else if (artifactDef == Dissimilarity_Def)
            {
                if (Kith.KithAdded == false)
                {
                    On.RoR2.SceneDirector.GenerateInteractableCardSelection -= Dissimilarity.MixInteractableApplier;
                    Dissimilarity.DissimAdded = true;
                }
            }
            else if (artifactDef == Kith_Def)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Dissimilarity_Def))
                {
                    On.RoR2.SceneDirector.GenerateInteractableCardSelection += Dissimilarity.MixInteractableApplier;
                    Dissimilarity.DissimAdded = true;
                }
                //Handles both Kith & Kith+Dissim
                On.RoR2.SceneDirector.GenerateInteractableCardSelection -= Kith.SingleInteractableApplier;
                Kith.KithAdded = false;
            }
            else if (artifactDef == Unison_Def)
            {
                Unison.OnArtifactDisable();

            }
        }



        public static void WanderLunarTeleporter(On.RoR2.SceneDirector.orig_PlaceTeleporter orig, global::RoR2.SceneDirector self)
        {
            if (self.teleporterSpawnCard != null)
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(Wander_Def))
                {
                    if (Run.instance.NetworkstageClearCount >= 2)
                    {
                        self.teleporterSpawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarTeleporter");
                    }
                }
                else if (RunArtifactManager.instance.IsArtifactEnabled(Dissimilarity_Def))
                {
                    if (Run.instance.NetworkstageClearCount >= 3 && random.Next(2) == 1)
                    {
                        self.teleporterSpawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarTeleporter");
                    }
                    else if (Run.instance.NetworkstageClearCount % Run.stagesPerLoop == Run.stagesPerLoop - 1)
                    {
                        self.teleporterSpawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarTeleporter");
                        //Debug.LogWarning("End of Loop Primordial");
                    }
                    else if (SceneInfo.instance.sceneDef.baseSceneName == "skymeadow")
                    {
                        self.teleporterSpawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarTeleporter");
                        //Debug.LogWarning("SkyMeadow Primordial");
                    }
                }
            }
            orig(self);
            if (DreamPrimordialBool == true)
            {
                DreamPrimordialBool = false;
                On.RoR2.Language.GetString_string -= DreamPrimordial;
            }


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




        internal static void ModSupport()
        {
            Dissimilarity.ModSupport();

            RiskyConformity = ArtifactCatalog.FindArtifactDef("RiskyArtifactOfConformity");

            if (RiskyConformity == null)
            {
                RiskyConformity = Command;
            }

            for (int i = 0; i < ArtifactCatalog.artifactCount; i++)
            {
                ArtifactDef temp = ArtifactCatalog.GetArtifactDef((ArtifactIndex)i);
                if (temp.pickupModelPrefab == null)
                {
                    temp.pickupModelPrefab = RoR2Content.Artifacts.EliteOnly.pickupModelPrefab;
                }
            }
        }


        private void SceneDirector_onGenerateInteractableCardSelection(SceneDirector sceneDirector, DirectorCardCategorySelection dccs)
        {
            if (RunArtifactManager.instance)
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(Remodeling_Def))
                {
                    dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.RemodelingPredicate));
                    //Debug.Log("Artifact of Dissimilarity + Tossing");
                }
            }
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

