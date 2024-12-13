using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates.ShrineHalcyonite;
using UnityEngine.AddressableAssets;

namespace TrueArtifacts
{
    public class MirrorHonor
    {
        public static float MaxLevel = 199;

        public static void On_Artifact_Enable()
        {
            /*foreach (DifficultyDef def in DifficultyCatalog.difficultyDefs)
            {
                def.scalingValue *= 2;
            }*/
            On.RoR2.Run.RecalculateDifficultyCoefficent += Run_RecalculateDifficultyCoefficent;
            On.RoR2.InfiniteTowerRun.RecalculateDifficultyCoefficentInternal += InfiniteTowerRun_RecalculateDifficultyCoefficentInternal;
            MaxLevel = Math.Max((Run.ambientLevelCap+1)*2, MaxLevel);
            if (MaxLevel < 1000)
            {
                MaxLevel = 999;
            }
        }

        private static void InfiniteTowerRun_RecalculateDifficultyCoefficentInternal(On.RoR2.InfiniteTowerRun.orig_RecalculateDifficultyCoefficentInternal orig, InfiniteTowerRun self)
        {
            orig(self);
            self.difficultyCoefficient *= 2;
            self.compensatedDifficultyCoefficient *= 2;
            self.ambientLevel = Mathf.Min((self.difficultyCoefficient - 1f) / 0.33f + 1f, 10000f);
            self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
        }

        private static void Run_RecalculateDifficultyCoefficent(On.RoR2.Run.orig_RecalculateDifficultyCoefficent orig, Run self)
        {
            orig(self);
            self.difficultyCoefficient *= 2;
            self.compensatedDifficultyCoefficient *= 2;
            float num4 = 0.7f + self.participatingPlayerCount * 0.3f;
            self.ambientLevel = Mathf.Min((self.difficultyCoefficient - num4) / 0.33f + 1f, MaxLevel);
            self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
        }

        public static void On_Artifact_Disable()
        {
            On.RoR2.Run.RecalculateDifficultyCoefficent -= Run_RecalculateDifficultyCoefficent;
            On.RoR2.InfiniteTowerRun.RecalculateDifficultyCoefficentInternal -= InfiniteTowerRun_RecalculateDifficultyCoefficentInternal;
        }

    }
}

