using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactDissimilarity
{
    public class Spiriting
    {
        public static float SpiritSpeedVal;
        public static float SpiritSpeedEnemyVal;
        public static float SpiritAttackSpeedEnemyVal;
        public static float SpiritAttackSpeedPlayerVal;
        public static float SpiritJumpEnemyVal;
        public static float SpiritJumpPlayerVal;
        public static float SpiritCooldownEnemyVal;
        public static float SpiritCooldownPlayerVal;
        public static float SpiritDamageEnemyVal;
        public static float SpiritDamagePlayerVal;
        public static float SpiritProjectileSpeedVal;
        public static float SpiritProjectileSpeedEnemyVal;

        public static void Start()
        {
            SpiritSpeedVal = WConfig.SpiritMovement.Value - 1;
            SpiritSpeedEnemyVal = SpiritSpeedVal * 0.66f;

            SpiritAttackSpeedPlayerVal = WConfig.SpiritAttackSpeed.Value - 1;
            SpiritAttackSpeedEnemyVal = SpiritAttackSpeedPlayerVal * 1.3f;

            SpiritJumpPlayerVal = WConfig.SpiritJump.Value - 1;
            SpiritJumpEnemyVal = (SpiritJumpPlayerVal - 1) * 1.5f + 1;

            SpiritCooldownPlayerVal = WConfig.SpiritCooldown.Value;
            SpiritCooldownEnemyVal = 0.90f;

            SpiritDamageEnemyVal = WConfig.SpiritDamage.Value;
            SpiritDamagePlayerVal = WConfig.SpiritDamagePlayer.Value;

            SpiritProjectileSpeedVal = WConfig.SpiritProjectileSpeed.Value;
            SpiritProjectileSpeedEnemyVal = (SpiritProjectileSpeedVal - 1) * 0.5f + 1;


        }


        private static void RecalcBandCooldown(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            if (self.healthComponent && buffDef == RoR2Content.Buffs.ElementalRingsCooldown && buffDef == DLC1Content.Buffs.ElementalRingVoidCooldown)
            {
                float tempfrac = 1 - (1 - self.healthComponent.combinedHealthFraction) / 0.8f;
                duration *= (tempfrac * SpiritCooldownPlayerVal + 1 - SpiritCooldownPlayerVal);
            }
            orig(self, buffDef, duration);
        }

        public static void RecalcStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, global::RoR2.CharacterBody self)
        {
            if (!self.master | !self.healthComponent | !self.skillLocator) { orig(self); return; }
            float tempfrac = 1;
            if (self.isPlayerControlled)
            {
                tempfrac = 1 - (1 - self.healthComponent.combinedHealthFraction) / 0.9f;
            }
            else
            {
                tempfrac = 1 - (1 - self.healthComponent.combinedHealthFraction) / 0.8f;
            }
            //tempfrac = (float)Decimal.Round((decimal)tempfrac,2); //Maybe looks cleaner idk
            if (tempfrac > 1) { tempfrac = 1; }
            if (tempfrac < 0) { tempfrac = 0; }

            CharacterBody tempmaster = self.master.bodyPrefab.GetComponent<CharacterBody>();

            if (self.isPlayerControlled == true)
            {
                self.baseMoveSpeed = ((1 - tempfrac) * SpiritSpeedVal + 1) * tempmaster.baseMoveSpeed;
                self.baseAcceleration = ((1 - tempfrac) * SpiritSpeedVal + 1) * tempmaster.baseAcceleration;
                self.baseJumpPower = ((1 - tempfrac) * SpiritJumpPlayerVal + 1) * tempmaster.baseJumpPower;

                self.baseDamage = (tempfrac * SpiritDamagePlayerVal + 1 - SpiritDamagePlayerVal) * tempmaster.baseDamage;
                self.levelDamage = (tempfrac * SpiritDamagePlayerVal + 1 - SpiritDamagePlayerVal) * tempmaster.levelDamage;
                self.baseAttackSpeed = ((1 - tempfrac) * SpiritAttackSpeedPlayerVal + 1) * tempmaster.baseAttackSpeed;
            }
            else
            {
                self.baseMoveSpeed = ((1 - tempfrac) * SpiritSpeedEnemyVal + 1) * tempmaster.baseMoveSpeed;
                self.baseAcceleration = ((1 - tempfrac) * SpiritSpeedEnemyVal + 1) * tempmaster.baseAcceleration;
                self.baseJumpPower = ((1 - tempfrac) * SpiritJumpEnemyVal + 1) * tempmaster.baseJumpPower;

                self.baseDamage = (tempfrac * SpiritDamageEnemyVal + 1 - SpiritDamageEnemyVal) * tempmaster.baseDamage;
                self.levelDamage = (tempfrac * SpiritDamageEnemyVal + 1 - SpiritDamageEnemyVal) * tempmaster.levelDamage;
                self.baseAttackSpeed = ((1 - tempfrac) * SpiritAttackSpeedEnemyVal + 1) * tempmaster.baseAttackSpeed;
            }

            //self.PerformAutoCalculateLevelStats();
            orig(self);
            if (self.isPlayerControlled == true)
            {
                if (self.skillLocator.primary)
                {
                    self.skillLocator.primary.cooldownScale *= (tempfrac * SpiritCooldownPlayerVal + 1 - SpiritCooldownPlayerVal);
                }
                if (self.skillLocator.secondary)
                {
                    self.skillLocator.secondary.cooldownScale *= (tempfrac * SpiritCooldownPlayerVal + 1 - SpiritCooldownPlayerVal);
                }
                if (self.skillLocator.utility)
                {
                    self.skillLocator.utility.cooldownScale *= (tempfrac * SpiritCooldownPlayerVal + 1 - SpiritCooldownPlayerVal);
                }
                if (self.skillLocator.special)
                {
                    self.skillLocator.special.cooldownScale *= (tempfrac * SpiritCooldownPlayerVal + 1 - SpiritCooldownPlayerVal);
                }
            }
            else
            {
                if (self.skillLocator.primary)
                {
                    self.skillLocator.primary.cooldownScale *= (tempfrac * SpiritCooldownEnemyVal + 1 - SpiritCooldownEnemyVal);
                    //if (self.skillLocator.primary.cooldownScale > 0.31f && self.name.StartsWith("Bell")) { self.skillLocator.primary.cooldownScale = 0.32f; }
                }
                if (self.skillLocator.secondary)
                {
                    self.skillLocator.secondary.cooldownScale *= (tempfrac * SpiritCooldownEnemyVal + 1 - SpiritCooldownEnemyVal);
                }
                if (self.skillLocator.utility)
                {
                    if (self.name.StartsWith("Vagrant") || self.name.StartsWith("GrandParent") || self.name.StartsWith("ClayBoss")) { tempfrac += 0.5f; }
                    self.skillLocator.utility.cooldownScale *= (tempfrac * SpiritCooldownEnemyVal + 1 - SpiritCooldownEnemyVal);
                }
                if (self.skillLocator.special)
                {
                    if (self.name.StartsWith("Brother") || self.name.StartsWith("SuperRobo")) { return; }
                    self.skillLocator.special.cooldownScale *= (tempfrac * SpiritCooldownEnemyVal + 1 - SpiritCooldownEnemyVal);
                }
            }
        }


        public static void RecalcOnDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, global::RoR2.HealthComponent self, global::RoR2.DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            if (!NetworkServer.active) { return; }
            //self.body.RecalculateStats();
            self.body.MarkAllStatsDirty();
            //Debug.LogWarning(self);
        }

        public static void RecalcOnShield(On.RoR2.HealthComponent.orig_ServerFixedUpdate orig, global::RoR2.HealthComponent self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            if (self.body.outOfDanger == true && self.shield < self.fullShield)
            {
                self.body.RecalculateStats();
            }
        }

        public static void RecalcOnLand(On.RoR2.CharacterMotor.orig_OnLanded orig, global::RoR2.CharacterMotor self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            //self.gameObject.GetComponent<CharacterBody>().RecalculateStats();
            self.gameObject.GetComponent<CharacterBody>().MarkAllStatsDirty();
            //Debug.LogWarning(self);
        }

        public static void RecalcOnSkill(On.RoR2.GenericSkill.orig_OnExecute orig, global::RoR2.GenericSkill self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            self.characterBody.RecalculateStats();
        }

        public static void RecalcOnHeal(On.RoR2.HealthComponent.orig_SendHeal orig, GameObject target, float amount, bool isCrit)
        {
            orig(target, amount, isCrit);
            if (!NetworkServer.active) { return; }
            //target.GetComponent<CharacterBody>().RecalculateStats();
            target.GetComponent<CharacterBody>().MarkAllStatsDirty();
            //Debug.LogWarning(target);
        }

        public static void RecalcProjectileSpeed(On.RoR2.Projectile.ProjectileController.orig_Start orig, global::RoR2.Projectile.ProjectileController self)
        {
            orig(self);
            if (self.owner == null) { return; }

            var temp = self.gameObject.GetComponent<RoR2.Projectile.ProjectileSimple>();
            if (temp == null) { return; }

            //Debug.Log(self.teamFilter.defaultTeam);

            float tempfrac = 1 - (1 - self.owner.GetComponent<CharacterBody>().healthComponent.combinedHealthFraction) / 0.8f;
            if (tempfrac > 1) { tempfrac = 1; }
            if (tempfrac < 0) { tempfrac = 0; }
            if (self.teamFilter.defaultTeam == TeamIndex.Player)
            {
                temp.desiredForwardSpeed *= (SpiritProjectileSpeedVal - tempfrac * (SpiritProjectileSpeedVal - 1));
            }
            else
            {
                temp.desiredForwardSpeed *= (SpiritProjectileSpeedEnemyVal - tempfrac * (SpiritProjectileSpeedEnemyVal - 1));
            }
        }

        public static void RecalcStunDuration(On.RoR2.SetStateOnHurt.orig_SetStun orig, SetStateOnHurt self, float duration)
        {
            orig(self, duration * 0.5f);
        }

    }
}