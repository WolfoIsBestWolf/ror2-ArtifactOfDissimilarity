using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactDissimilarity.Aritfacts
{
    public class Spiriting
    {
        public static float speedPlayers;
        public static float speedMonster;
        public static float attackSpeedMonster;
        public static float attackSpeedPlayers;
        public static float jumpPowerMonster;
        public static float jumpPowerPlayers;
        public static float skillCooldownMonster;
        public static float skillCooldownPlayers;
        public static float damagePlayers;
        public static float damageMonster;
        public static float projectileSpeedPlayer;
        public static float projectileSpeedMonster;

        public static void Start()
        {
            speedPlayers = 2f;
            speedMonster = 1.35f;

            attackSpeedPlayers = 1;
            attackSpeedMonster = 1.5f;

            jumpPowerPlayers = 0.25f;
            jumpPowerMonster = 1f;

            skillCooldownPlayers = 0.5f;
            skillCooldownMonster = 0.90f;

            damagePlayers = 0.25f;
            damageMonster = 0.25f;

            projectileSpeedPlayer = 1f;
            projectileSpeedMonster = 1.5f;


        }
        public static void OnArtifactEnable()
        {
            //Most need to be added on client because they keep have authority for recalc stats.
            On.RoR2.CharacterBody.RecalculateStats += Spiriting.RecalcStats;
            On.RoR2.Projectile.ProjectileController.Start += Spiriting.RecalcProjectileSpeed;
            On.RoR2.HealthComponent.TakeDamage += Spiriting.RecalcOnDamage;
            //On.RoR2.HealthComponent.SendHeal += Spiriting.RecalcOnHeal;
            //On.RoR2.CharacterMotor.OnLanded += Spiriting.RecalcOnLand;       
            On.RoR2.SetStateOnHurt.SetStun += Spiriting.RecalcStunDuration;
            Debug.Log("Added Spirit");
        }
        public static void OnArtifactDisable()
        {
            On.RoR2.CharacterBody.RecalculateStats -= Spiriting.RecalcStats;
            On.RoR2.Projectile.ProjectileController.Start -= Spiriting.RecalcProjectileSpeed;
            On.RoR2.HealthComponent.TakeDamage -= Spiriting.RecalcOnDamage;
            //On.RoR2.HealthComponent.SendHeal -= Spiriting.RecalcOnHeal;
            //On.RoR2.CharacterMotor.OnLanded -= Spiriting.RecalcOnLand;
            On.RoR2.SetStateOnHurt.SetStun -= Spiriting.RecalcStunDuration;
            Debug.Log("Removed Spirit");
        }


        private static void RecalcBandCooldown(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            if (self.healthComponent && buffDef == RoR2Content.Buffs.ElementalRingsCooldown && buffDef == DLC1Content.Buffs.ElementalRingVoidCooldown)
            {
                float tempfrac = 1 - (1 - self.healthComponent.combinedHealthFraction) / 0.8f;
                duration *= (tempfrac * skillCooldownPlayers + 1 - skillCooldownPlayers);
            }
            orig(self, buffDef, duration);
        }

        public static void RecalcStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, global::RoR2.CharacterBody self)
        {
            orig(self);

            if (!self.healthComponent || !self.skillLocator) { return; }
            float tempfrac = 1 - (1 - self.healthComponent.combinedHealthFraction) / 0.8f;
            if (tempfrac > 1) { return; }
            if (tempfrac < 0) { tempfrac = 0; }
            float reversedFrac = 1 - tempfrac;

            if (self.isPlayerControlled == true)
            {
                self.moveSpeed *= reversedFrac * speedPlayers + 1;
                self.acceleration *= reversedFrac * speedPlayers + 1;
                self.jumpPower *= reversedFrac * jumpPowerPlayers + 1;
                self.attackSpeed *= reversedFrac * attackSpeedPlayers + 1;
                self.damage *= 1 - damagePlayers * reversedFrac;
                float skillMult = 1 - skillCooldownPlayers * reversedFrac;
                if (self.skillLocator.primary)
                {
                    self.skillLocator.primary.cooldownScale *= skillMult;
                }
                if (self.skillLocator.secondary)
                {
                    self.skillLocator.secondary.cooldownScale *= skillMult;
                }
                if (self.skillLocator.utility)
                {
                    self.skillLocator.utility.cooldownScale *= skillMult;
                }
                if (self.skillLocator.special)
                {
                    self.skillLocator.special.cooldownScale *= skillMult;
                }
            }
            else
            {
                self.moveSpeed *= reversedFrac * speedMonster + 1;
                self.acceleration *= reversedFrac * speedMonster + 1;
                self.jumpPower *= reversedFrac * jumpPowerMonster + 1;
                self.attackSpeed *= reversedFrac * attackSpeedMonster + 1;
                self.damage *= 1 - damageMonster * reversedFrac;
                float skillMult = 1 - skillCooldownMonster * reversedFrac;

                if (self.skillLocator.primary)
                {
                    self.skillLocator.primary.cooldownScale *= (tempfrac * skillCooldownMonster + 1 - skillCooldownMonster);
                    //if (self.skillLocator.primary.cooldownScale > 0.31f && self.name.StartsWith("Bell")) { self.skillLocator.primary.cooldownScale = 0.32f; }
                }
                if (self.skillLocator.secondary)
                {
                    self.skillLocator.secondary.cooldownScale *= (tempfrac * skillCooldownMonster + 1 - skillCooldownMonster);
                }
                if (self.skillLocator.utility)
                {
                    if (self.name.StartsWith("Vagrant") || self.name.StartsWith("GrandParent") || self.name.StartsWith("ClayBoss")) { tempfrac += 0.5f; }
                    self.skillLocator.utility.cooldownScale *= (tempfrac * skillCooldownMonster + 1 - skillCooldownMonster);
                }
                if (self.skillLocator.special)
                {
                    if (self.name.StartsWith("Brother") || self.name.StartsWith("SuperRobo")) { return; }
                    self.skillLocator.special.cooldownScale *= (tempfrac * skillCooldownMonster + 1 - skillCooldownMonster);
                }
            }
        }


        public static void RecalcStatsOld(On.RoR2.CharacterBody.orig_RecalculateStats orig, global::RoR2.CharacterBody self)
        {
            if (!self.master | !self.healthComponent | !self.skillLocator) { orig(self); return; }
            float tempfrac = 1 - (1 - self.healthComponent.combinedHealthFraction) / 0.8f;

            //tempfrac = (float)Decimal.Round((decimal)tempfrac,2); //Maybe looks cleaner idk
            if (tempfrac > 1) { tempfrac = 1; }
            if (tempfrac < 0) { tempfrac = 0; }
            float reversedFrac = 1 - tempfrac;

            CharacterBody tempmaster = self.master.bodyPrefab.GetComponent<CharacterBody>();

            if (self.isPlayerControlled == true)
            {
                self.baseMoveSpeed = (reversedFrac * speedPlayers + 1) * tempmaster.baseMoveSpeed;
                self.baseAcceleration = (reversedFrac * speedPlayers + 1) * tempmaster.baseAcceleration;
                self.baseJumpPower = (reversedFrac * jumpPowerPlayers + 1) * tempmaster.baseJumpPower;

                self.baseDamage = (tempfrac * damageMonster + 1 - damageMonster) * tempmaster.baseDamage;
                self.levelDamage = (tempfrac * damageMonster + 1 - damageMonster) * tempmaster.levelDamage;
                self.baseAttackSpeed = (reversedFrac * attackSpeedPlayers + 1) * tempmaster.baseAttackSpeed;
            }
            else
            {
                self.baseMoveSpeed = (reversedFrac * speedMonster + 1) * tempmaster.baseMoveSpeed;
                self.baseAcceleration = (reversedFrac * speedMonster + 1) * tempmaster.baseAcceleration;
                self.baseJumpPower = (reversedFrac * jumpPowerMonster + 1) * tempmaster.baseJumpPower;

                self.baseDamage = (tempfrac * damagePlayers + 1 - damagePlayers) * tempmaster.baseDamage;
                self.levelDamage = (tempfrac * damagePlayers + 1 - damagePlayers) * tempmaster.levelDamage;
                self.baseAttackSpeed = (reversedFrac * attackSpeedMonster + 1) * tempmaster.baseAttackSpeed;
            }

            //self.PerformAutoCalculateLevelStats();
            orig(self);



            if (self.isPlayerControlled == true)
            {
                self.moveSpeed *= reversedFrac * 2 + 1;
                self.acceleration *= reversedFrac * 3 + 1;
                self.baseJumpPower = (reversedFrac * jumpPowerPlayers + 1) * tempmaster.baseJumpPower;

                self.baseDamage = (tempfrac * damageMonster + 1 - damageMonster) * tempmaster.baseDamage;
                self.levelDamage = (tempfrac * damageMonster + 1 - damageMonster) * tempmaster.levelDamage;
                self.baseAttackSpeed = (reversedFrac * attackSpeedPlayers + 1) * tempmaster.baseAttackSpeed;
            }
            else
            {
                self.baseMoveSpeed = (reversedFrac * speedMonster + 1) * tempmaster.baseMoveSpeed;
                self.baseAcceleration = (reversedFrac * speedMonster + 1) * tempmaster.baseAcceleration;
                self.baseJumpPower = (reversedFrac * jumpPowerMonster + 1) * tempmaster.baseJumpPower;

                self.baseDamage = (tempfrac * damagePlayers + 1 - damagePlayers) * tempmaster.baseDamage;
                self.levelDamage = (tempfrac * damagePlayers + 1 - damagePlayers) * tempmaster.levelDamage;
                self.baseAttackSpeed = (reversedFrac * attackSpeedMonster + 1) * tempmaster.baseAttackSpeed;
            }


            if (self.isPlayerControlled == true)
            {
                if (self.skillLocator.primary)
                {
                    self.skillLocator.primary.cooldownScale *= (tempfrac * skillCooldownPlayers + 1 - skillCooldownPlayers);
                }
                if (self.skillLocator.secondary)
                {
                    self.skillLocator.secondary.cooldownScale *= (tempfrac * skillCooldownPlayers + 1 - skillCooldownPlayers);
                }
                if (self.skillLocator.utility)
                {
                    self.skillLocator.utility.cooldownScale *= (tempfrac * skillCooldownPlayers + 1 - skillCooldownPlayers);
                }
                if (self.skillLocator.special)
                {
                    self.skillLocator.special.cooldownScale *= (tempfrac * skillCooldownPlayers + 1 - skillCooldownPlayers);
                }
            }
            else
            {
                if (self.skillLocator.primary)
                {
                    self.skillLocator.primary.cooldownScale *= (tempfrac * skillCooldownMonster + 1 - skillCooldownMonster);
                    //if (self.skillLocator.primary.cooldownScale > 0.31f && self.name.StartsWith("Bell")) { self.skillLocator.primary.cooldownScale = 0.32f; }
                }
                if (self.skillLocator.secondary)
                {
                    self.skillLocator.secondary.cooldownScale *= (tempfrac * skillCooldownMonster + 1 - skillCooldownMonster);
                }
                if (self.skillLocator.utility)
                {
                    if (self.name.StartsWith("Vagrant") || self.name.StartsWith("GrandParent") || self.name.StartsWith("ClayBoss")) { tempfrac += 0.5f; }
                    self.skillLocator.utility.cooldownScale *= (tempfrac * skillCooldownMonster + 1 - skillCooldownMonster);
                }
                if (self.skillLocator.special)
                {
                    if (self.name.StartsWith("Brother") || self.name.StartsWith("SuperRobo")) { return; }
                    self.skillLocator.special.cooldownScale *= (tempfrac * skillCooldownMonster + 1 - skillCooldownMonster);
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

        /*public static void RecalcOnShield(On.RoR2.HealthComponent.orig_ServerFixedUpdate orig, global::RoR2.HealthComponent self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            if (self.body.outOfDanger == true && self.shield < self.fullShield)
            {
                self.body.RecalculateStats();
            }
        }*/

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
                temp.desiredForwardSpeed *= (projectileSpeedPlayer - tempfrac * (projectileSpeedPlayer - 1));
            }
            else
            {
                temp.desiredForwardSpeed *= (projectileSpeedMonster - tempfrac * (projectileSpeedMonster - 1));
            }
        }

        public static void RecalcStunDuration(On.RoR2.SetStateOnHurt.orig_SetStun orig, SetStateOnHurt self, float duration)
        {
            orig(self, duration * 0.5f);
        }


        public static void RemoveSpiritBuffsMethod(CharacterBody body, CharacterBody originalBody)
        {
            body.baseMoveSpeed = originalBody.baseMoveSpeed;
            body.baseAcceleration = originalBody.baseAcceleration;
            body.baseJumpPower = originalBody.baseJumpPower;
            body.baseDamage = originalBody.baseDamage;
            body.levelDamage = originalBody.levelDamage;
            body.baseAttackSpeed = originalBody.baseAttackSpeed;
        }
    }
}