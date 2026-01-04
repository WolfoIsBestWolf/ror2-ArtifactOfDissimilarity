using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TrueArtifacts.Aritfacts
{
    public class MirrorSpite
    {

        private static GameObject projectilePrefab;

        public static void Start()
        {
            //Molotov 
            //Radius ??
        }
        public static void On_Artifact_Disable()
        {

            GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
        }
        public static void On_Artifact_Enable()
        {
            projectilePrefab = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/Molotov/MolotovSingleProjectile.prefab").WaitForCompletion();
            //Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/Molotov/MolotovProjectileDotZone.prefab").WaitForCompletion();

            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            CharacterBody victimBody = damageReport.victimBody;
            if (!victimBody)
            {
                return;
            }
            Vector3 corePosition = victimBody.corePosition;
            int projectiles = (int)victimBody.bestFitRadius;
            for (int i = 0; i < projectiles; i++)
            {
                float num = 360f / projectiles;
                Vector3 forward = Util.QuaternionSafeLookRotation(victimBody.transform.forward, victimBody.transform.up) * Util.ApplySpread(Vector3.forward, 0f, 0f, 1f, 1f, num * (float)i, 0f);
                FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
                fireProjectileInfo.projectilePrefab = projectilePrefab;
                fireProjectileInfo.position = victimBody.corePosition + Vector3.up * EntityStates.LunarExploderMonster.DeathState.projectileVerticalSpawnOffset;
                fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(forward);
                fireProjectileInfo.owner = victimBody.gameObject;
                fireProjectileInfo.damage = victimBody.damage * 0.5f; //3
                fireProjectileInfo.crit = false;
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
            /*if (EntityStates.LunarExploderMonster.DeathState.deathExplosionEffect)
            {
                EffectManager.SpawnEffect(EntityStates.LunarExploderMonster.DeathState.deathExplosionEffect, new EffectData
                {
                    origin = victimBody.corePosition,
                    scale = 6
                }, true);
            }*/




        }
    }

}

