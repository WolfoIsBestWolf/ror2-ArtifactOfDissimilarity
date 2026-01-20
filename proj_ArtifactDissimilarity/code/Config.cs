using BepInEx;
using BepInEx.Configuration;

namespace ArtifactDissimilarity
{
    public class WConfig
    {

        public static ConfigEntry<bool> EnableDissim;
        public static ConfigEntry<bool> EnableKith;
        public static ConfigEntry<bool> EnableWanderArtifact;
        public static ConfigEntry<bool> EnableRemodelArtifact;
        public static ConfigEntry<bool> EnableSpiritualArtifact;
        public static ConfigEntry<bool> EnableBrigadeArtifact;
        public static ConfigEntry<bool> EnableTransposeArtifact;
        public static ConfigEntry<bool> EnableUnisonArtifact;
        //public static ConfigEntry<bool> EnableObscurityArtifact;
        public static ConfigEntry<bool> Enable_Flamboyance_Artifact;
        public static ConfigEntry<bool> Enable_Doubles_Artifact;

        public static ConfigEntry<bool> DebugPrint;
        //public static ConfigEntry<bool> EnableCustomIneractables;
        //public static ConfigEntry<bool> ChangeBazaarSeer;
        public static ConfigEntry<bool> KithNoMinimumStageCompletion;

        public static ConfigEntry<bool> RemodelRerollOutOfScrap;
        public static ConfigEntry<bool> RemodelRerollMonsterItems;
        public static ConfigEntry<bool> RemodelRerollEquipmentDrones;
        public static ConfigEntry<bool> RemodelRerollDevotion;

        /*public static ConfigEntry<float> SpiritMovement;
        public static ConfigEntry<float> SpiritAttackSpeed;
        public static ConfigEntry<float> SpiritJump;
        public static ConfigEntry<float> SpiritCooldown;
        public static ConfigEntry<float> SpiritDamage;
        public static ConfigEntry<float> SpiritDamagePlayer;
        public static ConfigEntry<float> SpiritProjectileSpeed;*/

        public static ConfigEntry<bool> TransposeRerollHeresy;

        public static ConfigEntry<bool> UnisonDisablePrinters;


        internal static void InitConfig()
        {
            ConfigFile configFile = new ConfigFile(Paths.ConfigPath + "\\Wolfo.Wolfo_Artifacts.cfg", true);

            EnableDissim = configFile.Bind(
                ": Main :",
                "Enable Artifact of Dissimilarity",
                true,
                "Artifact of Dissonance for Interactables"
            );
            EnableKith = configFile.Bind(
                ": Main :",
                "Enable Artifact of Kith",
                true,
                "Artifact of Kin for Interactables. One per interactable category"
            );
            EnableWanderArtifact = configFile.Bind(
                ": Main :",
                "Enable Artifact of Wander",
                true,
                "Stages progress in a random order."
            );
            EnableRemodelArtifact = configFile.Bind(
                ": Main :",
                "Enable Artifact of Remodelling",
                true,
                "Reroll all items and equipment on stage change"
            );
            EnableSpiritualArtifact = configFile.Bind(
                ": Main :",
                "Enable Artifact of Spiriting",
                true,
                "RoR1 Spirit inspired, affects more stats"
            );
            EnableBrigadeArtifact = configFile.Bind(
                ": Main :",
                "Enable Artifact of Brigade",
                true,
                "Artifact of Kin but for Elite Types"
            );
            EnableTransposeArtifact = configFile.Bind(
                ": Main :",
                "Enable Artifact of Transpose",
                true,
                "Get a random skill loadout every stage"
            );
            EnableUnisonArtifact = configFile.Bind(
                ": Main :",
                "Enable Artifact of Unison",
                true,
                "One item per tier"
            );
            Enable_Flamboyance_Artifact = configFile.Bind(
                ": Main :",
                "Enable Artifact of Flamboyance",
                true,
                "Items may be rerolled into a random one of any tier"
            );
            Enable_Doubles_Artifact = configFile.Bind(
                ": Main :",
                "Enable Artifact of Doubles",
                true,
                "Players control 2 survivors or sometimes more."
            );
            /*EnableObscurityArtifact = configFile.Bind(
               ": Main :",
               "Enable Artifact of Obscurity WIP",
               false,
               "Blind Items"
           );*/


            /*ChangeBazaarSeer = configFile.Bind(
                "1a - Dissimilarity",
                "Should Bazaar Seers be randomized too",
                false,
                "If turned on, Lunar Seers in the Bazaar will go to random locations."
            );*/

            KithNoMinimumStageCompletion = configFile.Bind(
                "Kith",
                "Kith allow interactables earlier.",
                true,
                "Allows interactables to spawn a stage earlier than usual."
            );

            TransposeRerollHeresy = configFile.Bind(
                "Transpose",
                "Reroll Heresy items",
                false,
                "Should Transpose reroll Heresy items into other heresy items"
            );


            RemodelRerollOutOfScrap = configFile.Bind(
                "Remodeling",
                "Reroll out of Scrap",
                true,
                "If enabled, Scrap will be rerolled into a regular items\nThis allows you to quickly get a high stack of something (ie Scrap all Reds if you want 1 high stack instead of multiple small stacks).\nBut makes it so you can't take Scrap to Bazaar/Moon Cauldrons."
            );
            RemodelRerollMonsterItems = configFile.Bind(
                "Remodeling",
                "Reroll Monster Teams Inventory",
                true,
                "Reroll the items that Artifact of Evolution gives."
            );

            RemodelRerollEquipmentDrones = configFile.Bind(
                "Remodeling",
                "Reroll Equipment Drone Equipments",
                true,
                "Whether or not their Equipment should also be rerolled"
            );
            RemodelRerollDevotion = configFile.Bind(
                "Remodeling",
                "Reroll Devotion Items",
                true,
                "Should Devotion inventory be rerolled."
            );

            /*SpiritMovement = configFile.Bind(
                "Spiriting",
                "Maximum Movement Speed Bonus",
                3f,
                "Maximum multiplier for Movement Speed."
            );

            SpiritAttackSpeed = configFile.Bind(
                "Spiriting",
                "Maximum Attack Speed Bonus",
                2f,
                "Maximum multiplier for Attack Speed."
            );
            SpiritJump = configFile.Bind(
                "Spiriting",
                "Maximum Jump Bonus",
                1.25f,
                "Maximum Multiplier for Jump height."
            );
            SpiritProjectileSpeed = configFile.Bind(
                "Spiriting",
                "Maximum Projectile Speed Bonus",
                2f,
                "Maximum Multiplier for Projectile Speed. Enemies gain half the bonus"
            );

            SpiritCooldown = configFile.Bind(
                "Spiriting",
                "Maximum Cooldown Reduction",
                0.5f,
                "Maximum Cooldown Reduction in percent (0.0 - 1.0). 1 means 100% reduction at 10 health and 0 means no Reduction."
            );

            SpiritDamage = configFile.Bind(
                "Spiriting",
                "Maximum Damage Reduction (Enemies)",
                0f,
                "Maximum Damage Reduction in percent (0.0 - 1.0). 1 means 100% reduction at 0 health and 0 means no Reduction."
            );
            SpiritDamagePlayer = configFile.Bind(
                "Spiriting",
                "Maximum Damage Reduction (Players)",
                0.4f,
                "Maximum Damage Reduction in percent (0.0 - 1.0). 1 means 100% reduction at 0 health and 0 means no Reduction."
            );
            */

            DebugPrint = configFile.Bind(
                ": Main :",
                "Print Debug Info in console",
                false,
                "Full list of interactables possible on current stage Dissim/Kith"
            );

        }


    }
}