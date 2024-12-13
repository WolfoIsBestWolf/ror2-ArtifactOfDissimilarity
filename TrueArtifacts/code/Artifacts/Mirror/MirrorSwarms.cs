using HG;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace TrueArtifacts
{
    public class MirrorSwarms
    {
        public static void Start()
        {
            //Host copy over entity states from main body?
            ////Would desync if the cooldowns are desynced ig


            //It just fucking, like refuses to send the network state machine, why??


            //Add Net ID of Clone to connection, owned objects


            //Gets sent but with the hosts net id so it doesn't ever apply?

            //Multikill Syncer?
            //Maybe we can respawn a clone after TP or Simu boss wave or before big fights idk?

            //Reset Skills for both
        }



        public static void On_Artifact_Disable()
        {
            On.RoR2.CharacterMaster.Respawn -= NewClone_OnRespawn;
            On.RoR2.CharacterMaster.TransformBody -= Transform_Clones_ForDebug;
            On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex -= MakeMirrorsInvisible;
            On.RoR2.CharacterBody.RecalculateStats -= MatchSpeed;
            On.RoR2.CharacterBody.AddMultiKill -= CharacterBody_AddMultiKill;
            On.RoR2.SkillLocator.ResetSkills -= SkillLocator_ResetSkills;
        }

        public static void On_Artifact_Enable()
        {
            On.RoR2.CharacterMaster.Respawn += NewClone_OnRespawn;
            On.RoR2.CharacterMaster.TransformBody += Transform_Clones_ForDebug;
            On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex += MakeMirrorsInvisible;
            On.RoR2.CharacterBody.RecalculateStats += MatchSpeed;
            On.RoR2.CharacterBody.AddMultiKill += CharacterBody_AddMultiKill;
            On.RoR2.SkillLocator.ResetSkills += SkillLocator_ResetSkills;
       
            RoR2Content.Items.CutHp.hidden = true;
        }

        private static void Transform_Clones_ForDebug(On.RoR2.CharacterMaster.orig_TransformBody orig, CharacterMaster self, string bodyName)
        {
            orig(self,bodyName);
            foreach(MirrorSwarms_ServerSided clone in MirrorSwarms_ServerSided.instances)
            {
                if (clone.originMaster == self && clone.targetMaster)
                {
                    orig(clone.targetMaster, bodyName);
                }
            } 
                
        }

        private static void SkillLocator_ResetSkills(On.RoR2.SkillLocator.orig_ResetSkills orig, SkillLocator self)
        {
            orig(self);
            MirrorSwarms_BodyLinker var;
            if (self.TryGetComponent<MirrorSwarms_BodyLinker>(out var))
            {
                if (var.otherBody && var.otherBody.skillLocator)
                {
                    orig(var.otherBody.skillLocator);
                }
            }
        }

        //Clone gets Harpooned
        //Recalc stats, divide the Speed evenly
        //Then they should be equal for now.
        //But it doesn't count as that?

        //1 starts sprinting
        //Recalc stats,
        //Notice off speed
        //does the shit
        //2nd recalcs to sprint
        //Fucks it up ig??

        //Equalized speed better since it accounts for slowing effects

        private static void MatchSpeed(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            MirrorSwarms_BodyLinker var;
            if (self.TryGetComponent<MirrorSwarms_BodyLinker>(out var))
            {
                self.hasOneShotProtection = true;
                var.preEqualizedMoveSpeed = self.moveSpeed;
                var.preEqualizedAcceleration = self.acceleration;
                if (var.otherLink)
                { 
                    //Debug.Log(self.moveSpeed + " Other " +var.otherBody.moveSpeed);
                    if (var.otherBody.moveSpeed != self.moveSpeed && var.otherBody._isSprinting == self._isSprinting)
                    {
                        float speed = (var.preEqualizedMoveSpeed + var.otherLink.preEqualizedMoveSpeed) / 2;
                        float acc = (var.preEqualizedAcceleration + var.otherLink.preEqualizedAcceleration) / 2;
                        self.moveSpeed = speed;
                        self.acceleration = acc;
                        var.otherBody.moveSpeed = speed;
                        var.otherBody.acceleration = acc;
                    }
                }
            }
        }

        private static void CharacterBody_AddMultiKill(On.RoR2.CharacterBody.orig_AddMultiKill orig, CharacterBody self, int kills)
        {
            orig(self, kills);
            MirrorSwarms_BodyLinker var;
            if (self.TryGetComponent<MirrorSwarms_BodyLinker>(out var))
            {
                if (var.otherBody)
                {
                    orig(var.otherBody, kills);
                }
            }
        }

        private static VisibilityLevel MakeMirrorsInvisible(On.RoR2.CharacterBody.orig_GetVisibilityLevel_TeamIndex orig, CharacterBody self, TeamIndex observerTeam)
        {
            if (self.GetComponent<MirrorSwarms_IsClone>())
            {
                if (observerTeam == TeamIndex.Player)
                {
                    return orig(self, observerTeam);
                }
                else if (observerTeam != TeamIndex.Void)
                {
                    return VisibilityLevel.Cloaked;
                }
            }
            return orig(self, observerTeam);
        }



        private static CharacterBody NewClone_OnRespawn(On.RoR2.CharacterMaster.orig_Respawn orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation, bool wasRevivedMidStage)
        {
            var a = orig(self, footPosition, rotation, wasRevivedMidStage);
            if (self.playerCharacterMasterController)
            {
                CreateDoppelganger(self);
            }
            return a;
        }




        private static void CreateDoppelganger(CharacterMaster srcCharacterMaster)
        {
            if (srcCharacterMaster == null)
            {
                Debug.LogWarning("Attempted to make Clone of null master");
                return;
            }

            MasterCopySpawnCard spawnCard = MasterCopySpawnCard.FromMaster(srcCharacterMaster, true, true, null);
            if (!spawnCard)
            {
                return;
            }
            Vector3 core = srcCharacterMaster.GetBody().corePosition;

            float x = UnityEngine.Random.Range(1f, 3f);
            float z = UnityEngine.Random.Range(1f, 3f);
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                x *= -1;
            }
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                z *= -1;
            }
            Vector3 corePosition = new Vector3(core.x + x, core.y, core.z + z);
 

            DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
            {
                position = corePosition,
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
            };

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, directorPlacementRule, Run.instance.bossRewardRng);
            directorSpawnRequest.teamIndexOverride = TeamIndex.Player;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.summonerBodyObject = srcCharacterMaster.GetBodyObject();
            directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
            {
                Chat.SendBroadcastChat(new SendMirrorSwarms
                {
                    originMaster = srcCharacterMaster.gameObject,
                    targetMaster = result.spawnedInstance,
                });
            }));
            DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

            Debug.Log("Summoner : " + directorSpawnRequest.summonerBodyObject);

            UnityEngine.Object.Destroy(spawnCard);
        }
 







        public class MirrorSwarms_ServerSided : MonoBehaviour
        {
            public static List<MirrorSwarms_ServerSided> instances = new List<MirrorSwarms_ServerSided>();

            public Inventory targetInventory;
            public Inventory originInventory;
            public CharacterMaster targetMaster;
            public CharacterMaster originMaster;

            public CharacterBody targetBody;
            //targetMaster connection add to client owned objects
            //If the player can send CharacterNetworkTransform updates we surely can somehow make the clone do that too 
            //And Change Network Client Auth TO SAME
            //Has effective auth to FALSE

            public void OnEnable()
            {
                instances.Add(this);
                if (originMaster)
                {
                    targetMaster = this.GetComponent<CharacterMaster>();
                    originMaster.networkIdentity.clientAuthorityOwner.clientOwnedObjects.Add(targetMaster.networkIdentity.netId);
                    originInventory = originMaster.GetComponent<Inventory>();
                    targetInventory = targetMaster.GetComponent<Inventory>();

                    originInventory.onInventoryChanged += OriginInventory_onInventoryChanged;
                    originInventory.HandleInventoryChanged();

                    originMaster.onBodyStart += OriginMaster_onBodyStart;
                    targetMaster.onBodyStart += TargetMaster_onBodyStart;

                    OriginMaster_onBodyStart(originMaster.GetBody());
                    TargetMaster_onBodyStart(targetMaster.GetBody());
                }
            }

            private void RespawnIfInfestedDeath(CharacterBody obj)
            {
                Debug.Log("Clone died " + obj);
                if (obj && obj.teamComponent.teamIndex != TeamIndex.Player)
                {
                    CreateDoppelganger(originMaster);
                }
            }

            private void TargetMaster_onBodyStart(CharacterBody obj)
            {
                if (obj)
                {
                    targetBody = obj;
                    obj.teamComponent.teamIndex = TeamIndex.Player;
                    originMaster.networkIdentity.clientAuthorityOwner.clientOwnedObjects.Add(obj.networkIdentity.netId);
                    targetMaster.networkIdentity.m_ClientAuthorityOwner = originMaster.networkIdentity.clientAuthorityOwner;
                    obj.networkIdentity.m_ClientAuthorityOwner = originMaster.networkIdentity.clientAuthorityOwner;
                    obj.GetComponent<CharacterNetworkTransform>().hasEffectiveAuthority = false;
                }
            }

            private void OriginMaster_onBodyStart(CharacterBody obj)
            {
               
            }

            public void OnDisable()
            {
                instances.Remove(this);
                if (originInventory)
                {
                    originInventory.onInventoryChanged -= OriginInventory_onInventoryChanged;
                }
                if (originMaster)
                {
                    originMaster.onBodyStart -= OriginMaster_onBodyStart;
                }
                if (targetMaster)
                {
                    targetMaster.onBodyStart -= TargetMaster_onBodyStart;
                }
                RespawnIfInfestedDeath(targetBody);
            }

            private void OriginInventory_onInventoryChanged()
            {
                originInventory.GiveItem(RoR2Content.Items.CutHp, 1 - originInventory.GetItemCount(RoR2Content.Items.CutHp));
                if (targetInventory)
                {
                    if (targetBody && targetBody.teamComponent.teamIndex != TeamIndex.Player)
                    {
                        targetInventory.RemoveItem(RoR2Content.Items.CutHp, 1);
                        targetInventory.RemoveItem(RoR2Content.Items.AdaptiveArmor, 1);
                        return;
                    }
                    targetInventory.itemAcquisitionOrder.Clear();
                    int[] array = targetInventory.itemStacks;
                    int num = 0;
                    ArrayUtils.SetAll<int>(array, num);

                    targetInventory.AddItemsFrom(originInventory.itemStacks, defaultItemCopyFilterDelegate);
                    targetInventory.SetEquipmentIndex(originInventory.currentEquipmentState.equipmentIndex);
                    targetInventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
                    targetInventory.GiveItem(RoR2Content.Items.CutHp);

                    targetInventory.infusionBonus = originInventory.infusionBonus;
                    targetInventory.beadAppliedDamage = originInventory.beadAppliedDamage;
                    targetInventory.beadAppliedHealth = originInventory.beadAppliedHealth;
                    targetInventory.beadAppliedRegen = originInventory.beadAppliedRegen;
                }

            }



            public static readonly Func<ItemIndex, bool> defaultItemCopyFilterDelegate = new Func<ItemIndex, bool>(DefaultItemCopyFilter);
            private static bool DefaultItemCopyFilter(ItemIndex itemIndex)
            {
                return true;
            }

        }

        public class MirrorSwarms_MovementController : MonoBehaviour
        {
            public static List<MirrorSwarms_MovementController> instances = new List<MirrorSwarms_MovementController>();

            public CharacterMaster originMaster;
            public CharacterMaster targetMaster;
            public CharacterBody originBody;
            public CharacterBody targetBody;
            public InputBankTest targetInput;
            public InputBankTest originInputs;
            public CharacterMotor targetMotor;
            public EquipmentSlot originEquipmentSlot;
            public HurtBox targetTarget;

            private float teleportAttemptTimer = 1f;
            public GameObject helperPrefab;

            public MirrorSwarms_IsHost originBodyIdentifier;
            public MirrorSwarms_IsClone targetBodyIdentifier;

            public void OnEnable()
            {
                if (originMaster)
                {
                    instances.Add(this);
                    targetMaster = this.GetComponent<CharacterMaster>();
                    if (targetMaster)
                    {
                        Destroy(targetMaster.GetComponent<BaseAI>());
                        targetMaster.aiComponents = new BaseAI[0];
                    }

                    targetMaster.onBodyStart += TargetMaster_onBodyStart;
                    originMaster.onBodyStart += OriginMaster_onBodyStart;

                    OriginMaster_onBodyStart(originMaster.GetBody());
                    TargetMaster_onBodyStart(targetMaster.GetBody());
                }
                helperPrefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
                Debug.Log("MirrorSwarms_MovementController.OnEnable "+originMaster + "  " + targetMaster);
            }

            private void OriginMaster_onBodyStart(CharacterBody obj)
            {            
                if (obj)
                {
                    Debug.Log("OriginMaster_onBodyStart " + obj);
                    originBody = obj;
                    originInputs = originBody.inputBank;
                    originEquipmentSlot = originBody.equipmentSlot;

                    SharedBodySetup();
                }
            }


            public void SharedBodySetup()
            {
                if (targetBody && originBody)
                {
                    originBodyIdentifier = originBody.gameObject.GetComponent<MirrorSwarms_IsHost>();
                    targetBodyIdentifier = targetBody.gameObject.GetComponent<MirrorSwarms_IsClone>();
                    if (originBodyIdentifier == null)
                    {
                        originBodyIdentifier = originBody.gameObject.AddComponent<MirrorSwarms_IsHost>();
                    }
                    if (targetBodyIdentifier == null)
                    {
                        targetBodyIdentifier = targetBody.gameObject.AddComponent<MirrorSwarms_IsClone>();
                    }
                    originBodyIdentifier.otherBody = targetBody;
                    originBodyIdentifier.otherLink = targetBodyIdentifier;
                    targetBodyIdentifier.otherBody = originBody;
                    targetBodyIdentifier.otherLink = originBodyIdentifier;

                    VoidSurvivorController voidTarget = targetBody.GetComponent<VoidSurvivorController>();
                    if (voidTarget)
                    {
                        VoidSurvivorController voidOrigin = originBody.GetComponent<VoidSurvivorController>();
                        var voidSync = targetBody.GetComponent<MirrorSwarms_SyncVoidSurv>();
                        if (!voidSync)
                        {
                            voidSync = targetBody.gameObject.AddComponent<MirrorSwarms_SyncVoidSurv>();
                        }
                        voidSync.originVoidSurv = voidOrigin;
                    }

                    ChefController chefTarget = targetBody.GetComponent<ChefController>();
                    if (chefTarget)
                    {
                        ChefController chefOrigin = originBody.GetComponent<ChefController>();
                        var chefSync = targetBody.GetComponent<MirrorSwarms_SyncCHEF>();
                        if (!chefSync)
                        {
                            chefSync = targetBody.gameObject.AddComponent<MirrorSwarms_SyncCHEF>();
                        }
                        chefSync.originCHEF = chefOrigin;
                    }
                }
            }

            private void TargetMaster_onBodyStart(CharacterBody obj)
            {        
                if (obj)
                {
                    Debug.Log("TargetMaster_onBodyStart " + obj);
                    targetBody = obj;
                    targetBody.GetComponent<NetworkIdentity>().ForceAuthority(true);
                    targetInput = targetBody.inputBank;
                    targetMotor = targetBody.characterMotor;
                    targetBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 5);

                    SharedBodySetup();
                }

            }




            public void FixedUpdate()
            {
                if (targetInput && originBody)
                {
                    if (originEquipmentSlot)
                    {
                        targetTarget = originEquipmentSlot.currentTarget.hurtBox;
                        if (targetTarget == null)
                        {
                            originEquipmentSlot.ConfigureTargetFinderForEnemies();
                            targetTarget = originEquipmentSlot.targetFinder.GetResults().FirstOrDefault<HurtBox>();
                        }
                    }
                    if (targetTarget)
                    {
                        targetInput.aimDirection = (targetTarget.transform.position - targetInput.aimOrigin).normalized;
                    }
                    else
                    {
                        targetInput.aimDirection = originInputs.aimDirection;
                    }
                    targetInput.interact = originInputs.interact;
                    targetInput.activateEquipment = originInputs.activateEquipment;
                    //targetBody.isSprinting = originBody.isSprinting;


                    float distance = Vector3.Distance(originBody.corePosition, targetBody.corePosition);
                    if (distance > 40f)
                    {
                        this.teleportAttemptTimer -= Time.fixedDeltaTime;
                        if (this.teleportAttemptTimer <= 0f)
                        {
                            this.teleportAttemptTimer = 1f;

                            Vector3 additionalDistance = originBody.characterMotor.velocity / 10;
                            additionalDistance.y = 0;
                            float x = UnityEngine.Random.Range(1f, 3f);
                            float z = UnityEngine.Random.Range(1f, 3f);
                            if (UnityEngine.Random.Range(0, 2) == 1)
                            {
                               x *= -1;
                            }
                            if (UnityEngine.Random.Range(0, 2) == 1)
                            {
                                z *= -1;
                            }
                            Vector3 corePosition = new Vector3(originBody.corePosition.x+x, originBody.corePosition.y, originBody.corePosition.z+z);
                            corePosition += additionalDistance;

                            TeleportHelper.TeleportBody(targetBody, corePosition, false);
                            GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(targetBody.gameObject);
                            if (teleportEffectPrefab)
                            {
                                EffectManager.SimpleEffect(teleportEffectPrefab, corePosition, Quaternion.identity, true);

                            }
                            targetBody.AddTimedBuff(JunkContent.Buffs.IgnoreFallDamage,3f);
                            targetMotor.rootMotion = originBody.characterMotor.rootMotion;
                            targetMotor.moveDirection = originBody.characterMotor.moveDirection;
                            targetMotor.velocity = originBody.characterMotor.velocity;
                        }
                    }
                }
            }

            public void Update()
            {
                if (originInputs)
                {
                    targetInput.skill1.PushState(originInputs.skill1.down);
                    targetInput.skill2.PushState(originInputs.skill2.down);
                    targetInput.skill4.PushState(originInputs.skill4.down);
                    if (originInputs.interact.wasDown)
                    {
                        targetInput.moveVector = Vector3.zeroVector;
                    }
                    else
                    {
                        targetInput.moveVector = originInputs.moveVector;
                        targetInput.skill3.PushState(originInputs.skill3.down);
                        targetInput.sprint = originInputs.sprint;
                        targetInput.jump = originInputs.jump;
                    }
                }
            }

            public void OnDisable()
            {
                instances.Remove(this);
                targetMaster.onBodyStart -= TargetMaster_onBodyStart;
                originMaster.onBodyStart -= OriginMaster_onBodyStart;
                if (originBodyIdentifier)
                {
                    originBodyIdentifier.numberOfClones--;
                }
            }

        }


        public class MirrorSwarms_BodyLinker : MonoBehaviour
        {
            public CharacterBody thisBody;
            public CharacterBody otherBody;
            public MirrorSwarms_BodyLinker otherLink;
    
            public bool added;
            public int numberOfClones;
            public float preEqualizedMoveSpeed;
            public float preEqualizedAcceleration;

            public void Add(MirrorSwarms_BodyLinker link)
            {
                if (link && !link.added)
                {
                    link.added = true;
                    numberOfClones++;
                }
            }

            public void OnEnable()
            {
                thisBody = this.GetComponent<CharacterBody>();
            }
            public void OnDisable()
            {
                if (otherLink)
                {
                    otherLink.numberOfClones--;
                }
            }
        }

        public class MirrorSwarms_SyncVoidSurv : MonoBehaviour
        {
            public VoidSurvivorController originVoidSurv;
            public VoidSurvivorController targetVoidSurv;

            public void OnEnable()
            {
                if (!targetVoidSurv)
                {
                    targetVoidSurv = GetComponent<VoidSurvivorController>();
                }
                if (targetVoidSurv)
                {
                    targetVoidSurv.corruptionForFullDamage = 0;
                    targetVoidSurv.corruptionForFullHeal = 0;
                    targetVoidSurv.corruptionFractionPerSecondWhileCorrupted = 0;
                    targetVoidSurv.corruptionPerCrit = 0;
                    targetVoidSurv.corruptionPerSecondOutOfCombat = 0;
                    targetVoidSurv.corruptionPerSecondInCombat = 0;
                }
            }

            public void FixedUpdate()
            {
                if (originVoidSurv)
                {
                    targetVoidSurv._corruption = originVoidSurv.corruption;
                }
            }
        }

        public class MirrorSwarms_SyncCHEF : MonoBehaviour
        {
            public ChefController originCHEF;
            public ChefController targetCHEF;

            public void OnEnable()
            {
                if (!targetCHEF)
                {
                    targetCHEF = GetComponent<ChefController>();
                }
            }

            public void FixedUpdate()
            {
                if (originCHEF)
                {
                    targetCHEF.localCleaverFired = originCHEF.localCleaverFired;
                    targetCHEF._cleaverAway = originCHEF._cleaverAway;
                    targetCHEF._recallCleaver = originCHEF._recallCleaver;
                }
            }
        }

        public class MirrorSwarms_IsClone : MirrorSwarms_BodyLinker
        {
         
        }
        public class MirrorSwarms_IsHost : MirrorSwarms_BodyLinker
        {
         
        }

        public class SendMirrorSwarms : ChatMessageBase
        {
            public override string ConstructChatString()
            {
                Debug.Log("SendMirrorSwarms | " + originMaster + "  " + targetMaster);
                CharacterMaster master = originMaster.GetComponent<CharacterMaster>();

                if (NetworkServer.active)
                {
                    MirrorSwarms_ServerSided var = targetMaster.AddComponent<MirrorSwarms_ServerSided>();
                    var.originMaster = master;
                    var.OnEnable();
                }


                if (master && master.playerCharacterMasterController.networkUser.isLocalPlayer)
                {
                    MirrorSwarms_MovementController var = targetMaster.AddComponent<MirrorSwarms_MovementController>();
                    var.originMaster = master;
                    var.OnEnable();
                }
                else
                {
                    CharacterMaster master2 = originMaster.GetComponent<CharacterMaster>();
                    GameObject.Destroy(targetMaster.GetComponent<BaseAI>());
                    master2.aiComponents = new BaseAI[0];
                }
                return null;
            }

            public GameObject originMaster;
            public GameObject targetMaster;
            public override void Serialize(NetworkWriter writer)
            {
                base.Serialize(writer);
                writer.Write(originMaster);
                writer.Write(targetMaster);
            }
            public override void Deserialize(NetworkReader reader)
            {
                base.Deserialize(reader);
                originMaster = reader.ReadGameObject();
                targetMaster = reader.ReadGameObject();
            }
        }
    }
}

