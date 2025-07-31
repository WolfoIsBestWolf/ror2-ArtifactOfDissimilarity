using BepInEx;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using TrueArtifacts.Aritfacts;

namespace TrueArtifacts
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("Wolfo.TrueArtifacts", "TrueArtifacts", "1.0.3")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]

    public class Main : BaseUnityPlugin
    {
        public static ArtifactDef Mirror_Kin = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Mirror_Glass = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Mirror_Swarms = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Mirror_Sacrifice = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Mirror_Honor = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Mirror_Frailty = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Mirror_Enigma = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef Mirror_Spite = ScriptableObject.CreateInstance<ArtifactDef>();

        public static ArtifactDef True_Command = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef True_Dissonance = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef True_Swarms = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef True_Frailty = ScriptableObject.CreateInstance<ArtifactDef>();
        public static ArtifactDef True_Evolution = ScriptableObject.CreateInstance<ArtifactDef>();

        
        public void Awake()
        {
            Assets.Init(Info);
            WConfig.InitConfig();
            CreateArtifacts();
            RunArtifactManager.onArtifactEnabledGlobal += Artifact_Enabled;
            RunArtifactManager.onArtifactDisabledGlobal += Artifact_Disable;


            TrueCommand.Start();
            TrueEvolution.Start();
            MirrorSacrifice.Start();
            MirrorSwarms.Start();
            MirrorEnigma.Start();

            ArtifactCatalog.availability.CallWhenAvailable(CallLate);

         
            //Alt Enigma : Maybe like Tainted Isaac recycling : Leans to close to potentials tbh
            //Alt Devotion : Some different mob, maybe Imps?
            //Alt Frailty : Gravity is Doubled. Fucks up every jump pad so not feasible
            //Alt Death : Linked Health Bars ig or just split damage 

            //True Rebirth : Copy last run or smth
            //True Spite : Some fucked up projectile like Diablo Strike
            //True Delusion : Show every item or like 30 items as delusions but get idk 3 extra
            //TRUE Devotion just fuking mirror swarm but infinite
            #region Alrady Exists
            //Alt Dissonance : Artifact of Dissimilarity (Interactables)
            //Alt Kin : Artifact of Kith (Interactables)
            //Alt Kin : Technically Artifact of Family seems to already exist.
            //Alt Metamorphosis : Artifact of Transpose (Reroll Skills instead)
            //Alt Enigma : Artifact of Remodeling (Reroll Items)
            //Alt Vengence : Artifact of Hunted (Characters in spawn pool instead of on timer)
            //Alt Enigma : Artifact of Remodeling ig like reroll items instead of equip but that does both so idk

            //True Soul : Artifact of Origin (Copies of enemy)
            //True Dissonance : Artifact of Universe similiar but like, even more stupid.
            #endregion

            ChatMessageBase.chatMessageTypeToIndex.Add(typeof(SendMirrorKinTracker), (byte)ChatMessageBase.chatMessageIndexToType.Count);
            ChatMessageBase.chatMessageIndexToType.Add(typeof(SendMirrorKinTracker));

            ChatMessageBase.chatMessageTypeToIndex.Add(typeof(MirrorSwarms.SendMirrorSwarms), (byte)ChatMessageBase.chatMessageIndexToType.Count);
            ChatMessageBase.chatMessageIndexToType.Add(typeof(MirrorSwarms.SendMirrorSwarms));

        }

        public static void CallLate()
        {
            MirrorKin.MakeDCCSPool();


            Mirror_Kin.unlockableDef = RoR2Content.Artifacts.Swarms.unlockableDef;
            Mirror_Kin.pickupModelPrefab = RoR2Content.Artifacts.Swarms.pickupModelPrefab;

            Mirror_Glass.unlockableDef = RoR2Content.Artifacts.Glass.unlockableDef;
            Mirror_Glass.pickupModelPrefab = RoR2Content.Artifacts.Glass.pickupModelPrefab;

            Mirror_Swarms.unlockableDef = RoR2Content.Artifacts.Swarms.unlockableDef;
            Mirror_Swarms.pickupModelPrefab = RoR2Content.Artifacts.Swarms.pickupModelPrefab;

            Mirror_Sacrifice.unlockableDef = RoR2Content.Artifacts.Sacrifice.unlockableDef;
            Mirror_Sacrifice.pickupModelPrefab = RoR2Content.Artifacts.Sacrifice.pickupModelPrefab;

            Mirror_Honor.unlockableDef = RoR2Content.Artifacts.EliteOnly.unlockableDef;
            Mirror_Honor.pickupModelPrefab = RoR2Content.Artifacts.EliteOnly.pickupModelPrefab;

            Mirror_Frailty.unlockableDef = RoR2Content.Artifacts.EliteOnly.unlockableDef;
            Mirror_Frailty.pickupModelPrefab = RoR2Content.Artifacts.EliteOnly.pickupModelPrefab;

            Mirror_Enigma.unlockableDef = RoR2Content.Artifacts.Enigma.unlockableDef;
            Mirror_Enigma.pickupModelPrefab = RoR2Content.Artifacts.Enigma.pickupModelPrefab;


            True_Command.unlockableDef = RoR2Content.Artifacts.Command.unlockableDef;
            True_Command.pickupModelPrefab = RoR2Content.Artifacts.Command.pickupModelPrefab;

            True_Dissonance.unlockableDef = RoR2Content.Artifacts.MixEnemy.unlockableDef;
            True_Dissonance.pickupModelPrefab = RoR2Content.Artifacts.MixEnemy.pickupModelPrefab;

            True_Dissonance.unlockableDef = RoR2Content.Artifacts.MixEnemy.unlockableDef;
            True_Dissonance.pickupModelPrefab = RoR2Content.Artifacts.MixEnemy.pickupModelPrefab;

            True_Swarms.unlockableDef = RoR2Content.Artifacts.Swarms.unlockableDef;
            True_Swarms.pickupModelPrefab = RoR2Content.Artifacts.Swarms.pickupModelPrefab;

            True_Frailty.unlockableDef = RoR2Content.Artifacts.WeakAssKnees.unlockableDef;
            True_Frailty.pickupModelPrefab = RoR2Content.Artifacts.WeakAssKnees.pickupModelPrefab;

            True_Evolution.unlockableDef = RoR2Content.Artifacts.MonsterTeamGainsItems.unlockableDef;
            True_Evolution.pickupModelPrefab = RoR2Content.Artifacts.MonsterTeamGainsItems.pickupModelPrefab;



        }

        public static void CreateArtifacts()
        {
            string ZMirror = "";
            string ZTrue = "";
            string ZTrueMirror = "";
            if (WConfig.SortAtEnd.Value)
            {
                ZTrueMirror = "ZZTM_";
                ZTrue = "ZZT_";
                ZMirror = "ZZM_";
            }

            Rect rec = new Rect(0, 0, 64, 64);
            Vector2 vector = new Vector2(0, 0);

            #region Mirror Kin
            Texture2D Mirror_Kin_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Kin_On.png");
            Texture2D Mirror_Kin_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Kin_Off.png");
            Mirror_Kin_On.filterMode = FilterMode.Trilinear;
            Mirror_Kin_Off.filterMode = FilterMode.Trilinear;

            Mirror_Kin.cachedName = ZMirror+ "SingleMonsterTypeMirrorKin";
            Mirror_Kin.nameToken = "ARTIFACT_MIRROR_KIN_NAME";
            Mirror_Kin.descriptionToken = "ARTIFACT_MIRROR_KIN_DESC";
            Mirror_Kin.smallIconSelectedSprite = Sprite.Create(Mirror_Kin_On, rec, vector); ;
            Mirror_Kin.smallIconDeselectedSprite = Sprite.Create(Mirror_Kin_Off, rec, vector);
            ContentAddition.AddArtifactDef(Mirror_Kin);
            #endregion

            #region Mirror Glass
            Texture2D Mirror_Glass_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Glass_On.png");
            Texture2D Mirror_Glass_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Glass_Off.png");
            Mirror_Glass_On.filterMode = FilterMode.Trilinear;
            Mirror_Glass_Off.filterMode = FilterMode.Trilinear;

            Mirror_Glass.cachedName = ZMirror + "GlassMirrorGlass";
            Mirror_Glass.nameToken = "ARTIFACT_MIRROR_GLASS_NAME";
            Mirror_Glass.descriptionToken = "ARTIFACT_MIRROR_GLASS_DESC";
            Mirror_Glass.smallIconSelectedSprite = Sprite.Create(Mirror_Glass_On, rec, vector); ;
            Mirror_Glass.smallIconDeselectedSprite = Sprite.Create(Mirror_Glass_Off, rec, vector);
            ContentAddition.AddArtifactDef(Mirror_Glass);
            #endregion


            #region True Command
            Texture2D True_Command_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Command_On.png");
            Texture2D True_Command_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Command_Off.png");
            True_Command_On.filterMode = FilterMode.Trilinear;
            True_Command_Off.filterMode = FilterMode.Trilinear;

            True_Command.cachedName = ZTrue + "CommandTrueCommand";
            True_Command.nameToken = "ARTIFACT_TRUE_COMMAND_NAME";
            True_Command.descriptionToken = "ARTIFACT_TRUE_COMMAND_DESC";
            True_Command.smallIconSelectedSprite = Sprite.Create(True_Command_On, rec, vector); ;
            True_Command.smallIconDeselectedSprite = Sprite.Create(True_Command_Off, rec, vector);
            ContentAddition.AddArtifactDef(True_Command);
            #endregion

            #region True Dissonance
            Texture2D True_Dissonance_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Dissonance_On.png");
            Texture2D True_Dissonance_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Dissonance_Off.png");
            True_Dissonance_On.filterMode = FilterMode.Trilinear;
            True_Dissonance_Off.filterMode = FilterMode.Trilinear;

            True_Dissonance.cachedName = ZTrue + "MixEnemyTrueDissonance";
            True_Dissonance.nameToken = "ARTIFACT_TRUE_DISSONANCE_NAME";
            True_Dissonance.descriptionToken = "ARTIFACT_TRUE_DISSONANCE_DESC";
            True_Dissonance.smallIconSelectedSprite = Sprite.Create(True_Dissonance_On, rec, vector); ;
            True_Dissonance.smallIconDeselectedSprite = Sprite.Create(True_Dissonance_Off, rec, vector);
            ContentAddition.AddArtifactDef(True_Dissonance);
            #endregion

            #region True Swarms
            Texture2D True_Swarms_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Swarms_On.png");
            Texture2D True_Swarms_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Swarms_Off.png");
            True_Swarms_On.filterMode = FilterMode.Trilinear;
            True_Swarms_Off.filterMode = FilterMode.Trilinear;

            True_Swarms.cachedName = ZTrue + "SwarmsTrueSwarms";
            True_Swarms.nameToken = "ARTIFACT_TRUE_SWARMS_NAME";
            True_Swarms.descriptionToken = "ARTIFACT_TRUE_SWARMS_DESC";
            True_Swarms.smallIconSelectedSprite = Sprite.Create(True_Swarms_On, rec, vector); ;
            True_Swarms.smallIconDeselectedSprite = Sprite.Create(True_Swarms_Off, rec, vector);
            ContentAddition.AddArtifactDef(True_Swarms);
            #endregion

            #region True Frailty
            Texture2D True_Frailty_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Frailty_On.png");
            Texture2D True_Frailty_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Frailty_Off.png");
            True_Frailty_On.filterMode = FilterMode.Trilinear;
            True_Frailty_Off.filterMode = FilterMode.Trilinear;

            True_Frailty.cachedName = ZTrue + "WeakAssKneesTrueFrailty";
            True_Frailty.nameToken = "ARTIFACT_TRUE_FRAILTY_NAME";
            True_Frailty.descriptionToken = "ARTIFACT_TRUE_FRAILTY_DESC";
            True_Frailty.smallIconSelectedSprite = Sprite.Create(True_Frailty_On, rec, vector);
            True_Frailty.smallIconDeselectedSprite = Sprite.Create(True_Frailty_Off, rec, vector);
            //ContentAddition.AddArtifactDef(True_Frailty);
            #endregion

            #region Mirror Swarms
            Texture2D Mirror_Swarms_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Swarms_On.png");
            Texture2D Mirror_Swarms_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Swarms_Off.png");
            Mirror_Swarms_On.filterMode = FilterMode.Trilinear;
            Mirror_Swarms_Off.filterMode = FilterMode.Trilinear;

            Mirror_Swarms.cachedName = ZMirror + "SwarmsMirrorSwarms";
            Mirror_Swarms.nameToken = "ARTIFACT_MIRROR_SWARMS_NAME";
            Mirror_Swarms.descriptionToken = "ARTIFACT_MIRROR_SWARMS_DESC";
            Mirror_Swarms.smallIconSelectedSprite = Sprite.Create(Mirror_Swarms_On, rec, vector);
            Mirror_Swarms.smallIconDeselectedSprite = Sprite.Create(Mirror_Swarms_Off, rec, vector);
            ContentAddition.AddArtifactDef(Mirror_Swarms);
            #endregion

            #region True Evolution
            Texture2D True_Evolution_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Evolution_On.png");
            Texture2D True_Evolution_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/True_Evolution_Off.png");
            True_Evolution_On.filterMode = FilterMode.Trilinear;
            True_Evolution_Off.filterMode = FilterMode.Trilinear;

            True_Evolution.cachedName = ZTrue + "MonsterTeamGainsItemsTrueEvolution";
            True_Evolution.nameToken = "ARTIFACT_TRUE_EVOLUTION_NAME";
            True_Evolution.descriptionToken = "ARTIFACT_TRUE_EVOLUTION_DESC";
            True_Evolution.smallIconSelectedSprite = Sprite.Create(True_Evolution_On, rec, vector);
            True_Evolution.smallIconDeselectedSprite = Sprite.Create(True_Evolution_Off, rec, vector);
            ContentAddition.AddArtifactDef(True_Evolution);
            #endregion

            #region Mirror Sacrifice
            Texture2D Mirror_Sacrifice_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Sacrifice_On.png");
            Texture2D Mirror_Sacrifice_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Sacrifice_Off.png");
            Mirror_Sacrifice_On.filterMode = FilterMode.Trilinear;
            Mirror_Sacrifice_Off.filterMode = FilterMode.Trilinear;

            Mirror_Sacrifice.cachedName = ZMirror + "SacrificeMirrorSacrifice";
            Mirror_Sacrifice.nameToken = "ARTIFACT_MIRROR_SACRIFICE_NAME";
            Mirror_Sacrifice.descriptionToken = "ARTIFACT_MIRROR_SACRIFICE_DESC";
            Mirror_Sacrifice.smallIconSelectedSprite = Sprite.Create(Mirror_Sacrifice_On, rec, vector);
            Mirror_Sacrifice.smallIconDeselectedSprite = Sprite.Create(Mirror_Sacrifice_Off, rec, vector);
            ContentAddition.AddArtifactDef(Mirror_Sacrifice);
            #endregion

            #region Mirror Honor
            Texture2D Mirror_Honor_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Honor_On.png");
            Texture2D Mirror_Honor_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Honor_Off.png");
            Mirror_Honor_On.filterMode = FilterMode.Trilinear;
            Mirror_Honor_Off.filterMode = FilterMode.Trilinear;

            Mirror_Honor.cachedName = ZMirror + "EliteOnlyMirrorHonor";
            Mirror_Honor.nameToken = "ARTIFACT_MIRROR_HONOR_NAME";
            Mirror_Honor.descriptionToken = "ARTIFACT_MIRROR_HONOR_DESC";
            Mirror_Honor.smallIconSelectedSprite = Sprite.Create(Mirror_Honor_On, rec, vector);
            Mirror_Honor.smallIconDeselectedSprite = Sprite.Create(Mirror_Honor_Off, rec, vector);
            ContentAddition.AddArtifactDef(Mirror_Honor);
            #endregion


            #region Mirror Frailty        
            Texture2D Mirror_Frailty_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Frailty_On.png");
            Texture2D Mirror_Frailty_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Frailty_Off.png");
            Mirror_Frailty_On.filterMode = FilterMode.Trilinear;
            Mirror_Frailty_Off.filterMode = FilterMode.Trilinear;

            Mirror_Frailty.cachedName = ZMirror + "WeakAssKneesMirrorFrailty";
            Mirror_Frailty.nameToken = "ARTIFACT_MIRROR_FRAILTY_NAME";
            Mirror_Frailty.descriptionToken = "ARTIFACT_MIRROR_FRAILTY_DESC";
            Mirror_Frailty.smallIconSelectedSprite = Sprite.Create(Mirror_Frailty_On, rec, vector);
            Mirror_Frailty.smallIconDeselectedSprite = Sprite.Create(Mirror_Frailty_Off, rec, vector);
            ContentAddition.AddArtifactDef(Mirror_Frailty);
            #endregion

            #region Mirror Enigma        
            Texture2D Mirror_Enigma_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Enigma_On.png");
            Texture2D Mirror_Enigma_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Enigma_Off.png");
            Mirror_Enigma_On.filterMode = FilterMode.Trilinear;
            Mirror_Enigma_Off.filterMode = FilterMode.Trilinear;

            Mirror_Enigma.cachedName = ZMirror + "EnigmaMirrorEnigma";
            Mirror_Enigma.nameToken = "ARTIFACT_MIRROR_ENIGMA_NAME";
            Mirror_Enigma.descriptionToken = "ARTIFACT_MIRROR_ENIGMA_DESC";
            Mirror_Enigma.smallIconSelectedSprite = Sprite.Create(Mirror_Enigma_On, rec, vector);
            Mirror_Enigma.smallIconDeselectedSprite = Sprite.Create(Mirror_Enigma_Off, rec, vector);
            ContentAddition.AddArtifactDef(Mirror_Enigma);
            #endregion

            #region Mirror Spite        
            Texture2D Mirror_Spite_On = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Spite_On.png");
            Texture2D Mirror_Spite_Off = Assets.Bundle.LoadAsset<Texture2D>("Assets/TrueArtifacts/Mirror_Spite_Off.png");
            Mirror_Spite_On.filterMode = FilterMode.Trilinear;
            Mirror_Spite_Off.filterMode = FilterMode.Trilinear;

            Mirror_Spite.cachedName = ZMirror + "BombMirrorSpite";
            Mirror_Spite.nameToken = "ARTIFACT_MIRROR_SPITE_NAME";
            Mirror_Spite.descriptionToken = "ARTIFACT_MIRROR_SPITE_DESC";
            Mirror_Spite.smallIconSelectedSprite = Sprite.Create(Mirror_Spite_On, rec, vector);
            Mirror_Spite.smallIconDeselectedSprite = Sprite.Create(Mirror_Spite_Off, rec, vector);
            //ContentAddition.AddArtifactDef(Mirror_Spite);
            #endregion
        }

        private void Artifact_Enabled([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef == True_Command)
            {
                TrueCommand.On_Artifact_Enable();
                runArtifactManager.SetArtifactEnabled(RoR2Content.Artifacts.Command, false);
            }
            else if (artifactDef == True_Dissonance)
            {
                TrueDissonance.On_Artifact_Enable();
                runArtifactManager.SetArtifactEnabled(RoR2Content.Artifacts.MixEnemy, false);
            }
            else if (artifactDef == True_Swarms)
            {
                TrueSwarms.On_Artifact_Enable();
            }
            else if (artifactDef == Mirror_Kin)
            {
                MirrorKin.On_Artifact_Enable();
            }
            else if (artifactDef == Mirror_Glass)
            {
                MirrorGlass.On_Artifact_Enable();
            }
            else if (artifactDef == True_Frailty)
            {
                //TrueFrailty.On_Artifact_Enable();
            }
            else if (artifactDef == Mirror_Swarms)
            {
                MirrorSwarms.On_Artifact_Enable();
            }
            else if (artifactDef == True_Evolution)
            {
                TrueEvolution.On_Artifact_Enable();
            }
            else if (artifactDef == Mirror_Sacrifice)
            {
                MirrorSacrifice.On_Artifact_Enable();
            }
            else if (artifactDef == Mirror_Honor)
            {
                MirrorHonor.On_Artifact_Enable();
            }
            else if (artifactDef == Mirror_Frailty)
            {
                MirrorFrailty.On_Artifact_Enable();
            }
            else if (artifactDef == Mirror_Enigma)
            {
                MirrorEnigma.On_Artifact_Enable();
            }
            else if (artifactDef == Mirror_Spite)
            {
                //MirrorSpite.On_Artifact_Enable();
            }
        }

        private void Artifact_Disable([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {


            if (artifactDef == True_Command)
            {
                TrueCommand.On_Artifact_Disable();
            }
            else if (artifactDef == True_Dissonance)
            {
                TrueDissonance.On_Artifact_Disable();
            }
            else if (artifactDef == True_Swarms)
            {
                TrueSwarms.On_Artifact_Disable();
            }
            else if (artifactDef == Mirror_Kin)
            {
                MirrorKin.On_Artifact_Disable();
            }
            else if (artifactDef == Mirror_Glass)
            {
                MirrorGlass.On_Artifact_Disable();
            }
            else if (artifactDef == True_Frailty)
            {
                //TrueFrailty.On_Artifact_Disable();
            }
            else if (artifactDef == Mirror_Swarms)
            {
                MirrorSwarms.On_Artifact_Disable();
            }
            else if (artifactDef == True_Evolution)
            {
                TrueEvolution.On_Artifact_Disable();
            }
            else if (artifactDef == Mirror_Sacrifice)
            {
                MirrorSacrifice.On_Artifact_Disable();
            }
            else if (artifactDef == Mirror_Honor)
            {
                MirrorHonor.On_Artifact_Disable();
            }
            else if (artifactDef == Mirror_Frailty)
            {
                MirrorFrailty.On_Artifact_Disable();
            }
            else if (artifactDef == Mirror_Enigma)
            {
                MirrorEnigma.On_Artifact_Disable();
            }
            else if (artifactDef == Mirror_Spite)
            {
                //MirrorSpite.On_Artifact_Disable();
            }
        }
    }

}

