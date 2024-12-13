using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrueArtifacts
{
    public class MirrorDeath
    {
        private static List<HealthComponent> playerHP = new List<HealthComponent>();

        public static void On_Artifact_Disable()
        {
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
        }
        public static void On_Artifact_Enable()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.HealthComponent.Awake += HealthComponent_Awake;
            On.RoR2.HealthComponent.OnDestroy += HealthComponent_OnDestroy;
 
        }

        private static void HealthComponent_OnDestroy(On.RoR2.HealthComponent.orig_OnDestroy orig, HealthComponent self)
        {
            orig(self);
            if (self.body)
            {
                if (self.body.master && self.body.master.playerCharacterMasterController)
                {
                    playerHP.Remove(self);
                }         
            }
            else
            {
                playerHP.Remove(self);
            }
            
        }

        private static void HealthComponent_Awake(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            orig(self);
            if (self.body && self.body.master && self.body.master.playerCharacterMasterController)
            {
                playerHP.Add(self);
            }
        }
       

        private static void GatherHealthComponents(CharacterBody obj)
        {
            if (obj && obj.master && obj.master.playerCharacterMasterController)
            {
                playerHP.Add(obj.healthComponent);
            }
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body && self.body.master && self.body.master.playerCharacterMasterController)
            {
                foreach (HealthComponent hp in playerHP)
                {
                    if (hp)
                    {
                        damageInfo.damage /= playerHP.Count;
                        orig(hp, damageInfo);
                    }
                }
            }
            else
            {
                orig(self, damageInfo);
            }
            
        }
    }

}

