using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace TrueArtifacts.Aritfacts
{
    public class TrueSpite
    {

        public static void On_Artifact_Enable()
        {
            if (!DiabloStrike)
            {
                DiabloStrike = Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/Captain/CaptainAirstrikeAltProjectile.prefab").WaitForCompletion();
                DiabloStrike.AddComponent<MatchFuseToDuration>().defaultFuseTime = 20;
            }
            GlobalEventManager.onCharacterDeathGlobal += OnServerCharacterDeath;
            RoR2Application.onFixedUpdate += ProcessBombQueue;
        }

        public static void On_Artifact_Disable()
        {
            GlobalEventManager.onCharacterDeathGlobal -= OnServerCharacterDeath;
            RoR2Application.onFixedUpdate -= ProcessBombQueue;
        }

        private static void SpawnBomb(RoR2.Artifacts.BombArtifactManager.BombRequest bombRequest, float groundY)
        {
            Vector3 spawnPosition = bombRequest.spawnPosition;
            spawnPosition.y = groundY + 2f;

            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
            {
                crit = false,
                owner = bombRequest.attacker,
                position = spawnPosition,
                projectilePrefab = DiabloStrike,
                rotation = new Quaternion(0, 0, 0, 0),
                //rotation = bombRequest.attacker.transform.rotation,
                speedOverride = 0,
                damage = bombRequest.bombBaseDamage
            };
            fireProjectileInfo.fuseOverride = 8;
            fireProjectileInfo.force = 10000;
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }


        private static void OnServerCharacterDeath(DamageReport damageReport)
        {
            CharacterBody victimBody = damageReport.victimBody;
            Vector3 corePosition = victimBody.corePosition;
            int num = Mathf.Min(maxBombCount, Mathf.CeilToInt(victimBody.bestFitRadius * extraBombPerRadius));
            for (int i = 0; i < num; i++)
            {
                Vector3 b = UnityEngine.Random.insideUnitSphere * (bombSpawnBaseRadius + victimBody.bestFitRadius * bombSpawnRadiusCoefficient);
                RoR2.Artifacts.BombArtifactManager.BombRequest item = new RoR2.Artifacts.BombArtifactManager.BombRequest
                {
                    spawnPosition = corePosition,
                    raycastOrigin = corePosition + b,
                    bombBaseDamage = victimBody.damage * 40,
                    attacker = victimBody.gameObject,
                    teamIndex = damageReport.victimTeamIndex,
                    velocityY = UnityEngine.Random.Range(5f, 25f)
                };
                bombRequestQueue.Enqueue(item);
            }
        }


        private static void ProcessBombQueue()
        {
            if (bombRequestQueue.Count > 0)
            {
                RoR2.Artifacts.BombArtifactManager.BombRequest bombRequest = bombRequestQueue.Dequeue();
                Ray ray = new Ray(bombRequest.raycastOrigin + new Vector3(0f, maxBombStepUpDistance, 0f), Vector3.down);
                float maxDistance = maxBombStepUpDistance + maxBombFallDistance;
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    SpawnBomb(bombRequest, raycastHit.point.y);
                }
            }
        }


        private static GameObject DiabloStrike;
        private static readonly Queue<RoR2.Artifacts.BombArtifactManager.BombRequest> bombRequestQueue = new Queue<RoR2.Artifacts.BombArtifactManager.BombRequest>();

        private static readonly int maxBombCount = 30;
        private static readonly float extraBombPerRadius = 0.5f;
        private static readonly float bombSpawnBaseRadius = 6f;
        private static readonly float bombSpawnRadiusCoefficient = 6f;

        private static readonly float bombFuseTimeout = 8f;
        private static readonly float maxBombStepUpDistance = 8f;


        private static readonly float maxBombFallDistance = 60f;


        public class MatchFuseToDuration : MonoBehaviour
        {
            public float defaultFuseTime;
            public void Start()
            {
                float fuse = GetComponent<ProjectileImpactExplosion>().lifetime;
                var Ghost = GetComponent<ProjectileController>().ghost;
                if (Ghost)
                {
                    ObjectScaleCurve[] scale = Ghost.GetComponentsInChildren<ObjectScaleCurve>();
                    foreach (ObjectScaleCurve scaleCurve in scale)
                    {
                        if (scaleCurve.timeMax == defaultFuseTime)
                        {
                            scaleCurve.timeMax = fuse;
                        }
                    }
                    ObjectTransformCurve[] scale2 = Ghost.GetComponentsInChildren<ObjectTransformCurve>();
                    foreach (ObjectTransformCurve scaleCurve in scale2)
                    {
                        if (scaleCurve.timeMax == defaultFuseTime)
                        {
                            scaleCurve.timeMax = fuse;
                        }
                    }
                    defaultFuseTime = fuse;
                }
            }
        }


    }

}

