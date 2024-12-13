using BepInEx;
using BepInEx.Configuration;

namespace TrueArtifacts
{
    public class WConfig
    {

        public static ConfigEntry<bool> EnableMirrorArtifacts;
        public static ConfigEntry<bool> EnableTrueArtifacts;

        public static ConfigEntry<bool> MirrorKinDisplay;
        public static ConfigEntry<bool> MirrirSwarmsInfestor;
        public static ConfigEntry<bool> TrueDissoEliteStuff;
        public static ConfigEntry<float> MirrorEnigmaChance;

        public static ConfigEntry<bool> SortAtEnd;
        public static ConfigEntry<bool> DebugPrint;


        internal static void InitConfig()
        {
            ConfigFile configFile = new ConfigFile(Paths.ConfigPath + "\\Wolfo.True_Artifacts.cfg", true);

            MirrorKinDisplay = configFile.Bind(
                "Mirror",
                "Mirror Kin Enemy Display",
                true,
                "Regular Kin does so seemed fitting to do it here too."
            );
            MirrorEnigmaChance = configFile.Bind(
                "Mirror",
                "Mirror Enigma Chance",
                10f,
                "Chance to replace items and equipment with a random one."
            );
            SortAtEnd = configFile.Bind(
              "Main",
              "Sort Artifacts to end of list",
              true,
              "If you don't like or mind the color mish mash"
            );
            DebugPrint = configFile.Bind(
               "Other",
               "Enable Debug Info",
               false,
               "If you do not need this better to leave it off"
           );

            RiskConfig();
        }

        public static void RiskConfig()
        {

        }

    }
}