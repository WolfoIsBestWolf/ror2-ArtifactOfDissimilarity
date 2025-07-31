using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TrueArtifacts.Aritfacts
{
    public class TrueFrailty
    {
        public static void On_Artifact_Enable()
        {
            //Run.baseGravity = -60;
            //Higher gravity could be cool but fucks up the game too much like jump pads
            On.RoR2.GlobalEventManager.IsImmuneToFallDamage += GlobalEventManager_IsImmuneToFallDamage;
            IL.RoR2.GlobalEventManager.OnCharacterHitGroundServer += FuckedUpFallDamage;
        }

        private static bool GlobalEventManager_IsImmuneToFallDamage(On.RoR2.GlobalEventManager.orig_IsImmuneToFallDamage orig, GlobalEventManager self, CharacterBody body)
        {
            return false;
        }

        public static void On_Artifact_Disable()
        {
            //Is this how IL works??
            On.RoR2.GlobalEventManager.IsImmuneToFallDamage -= GlobalEventManager_IsImmuneToFallDamage;
            IL.RoR2.GlobalEventManager.OnCharacterHitGroundServer -= FuckedUpFallDamage;
        }

        private static void FuckedUpFallDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0f),
                x => x.MatchStfld("RoR2.DamageInfo", "procCoefficient")
                ))
            {
                c.EmitDelegate<Func<DamageInfo, DamageInfo>>((damageInfo) =>
                {
                    if (RunArtifactManager.instance.IsArtifactEnabled(Main.True_Frailty))
                    {
                        damageInfo.damage *= 4f;
                        damageInfo.damageType &= ~DamageType.NonLethal;
                        damageInfo.damageType |= DamageType.BypassOneShotProtection;
                    }
                    return damageInfo;

                });
            }
            else
            {
                Debug.LogWarning("IL Failed: Fall Damage");
            }
        }
    }

}

