using HG;
using MonoMod.Cil;
using Newtonsoft.Json.Utilities;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace ArtifactDissimilarity.Aritfacts
{
    public partial class Doubles
    {


        public static void Start()
        {
            //GameObject playerMaster = Addressables.LoadAssetAsync<GameObject>(key: "5acc65dc41dfe5840a14db71ba004f72").WaitForCompletion();
            GameObject commandoMaster = Addressables.LoadAssetAsync<GameObject>(key: "f146e1c7699e35b43b70e119b46875e8").WaitForCompletion();
            DoublesCloneMaster = PrefabAPI.InstantiateClone(commandoMaster, "DoublesMaster", false);

           
            var skills = DoublesCloneMaster.GetComponents<AISkillDriver>();
            for (int i = 0; i < skills.Length;i++)
            {
                GameObject.Destroy(skills[i]);
            }
            GameObject.Destroy(DoublesCloneMaster.GetComponent<BaseAI>());
            GameObject.Destroy(DoublesCloneMaster.GetComponent<EntityStateMachine>());

            DoublesCloneMaster.AddComponent<ArtifactDoubles_MasterNetwork>();
            //CloneMaster.AddComponent<MirrorSwarms_MovementController>();

            PrefabAPI.RegisterNetworkPrefab(DoublesCloneMaster);

            SpecificHooks();
        }
 
        public static GameObject DoublesCloneMaster;


        public static void On_Artifact_Disable()
        {
            On.RoR2.CharacterMaster.Respawn_Vector3_Quaternion_bool -= NewClone_OnRespawn;
            On.RoR2.CharacterMaster.TransformBody_string_bool -= Transform_Clones_ForDebug;
            On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex -= MakeMirrorsInvisible;
            On.RoR2.CharacterBody.RecalculateStats -= MatchSpeed;
            On.RoR2.CharacterBody.AddMultiKill -= Sync_MultiKill;
            On.RoR2.SkillLocator.ResetSkills -= SkillLocator_ResetSkills;
            On.JunkPickup.OnTriggerStay -= SyncDrifterJunk;
            IL.EntityStates.Seeker.Meditate.Update -= SyncMeditate;
        }

        public static void On_Artifact_Enable()
        {
            On.RoR2.CharacterMaster.Respawn_Vector3_Quaternion_bool += NewClone_OnRespawn;
            On.RoR2.CharacterMaster.TransformBody_string_bool += Transform_Clones_ForDebug;
            On.RoR2.CharacterBody.GetVisibilityLevel_TeamIndex += MakeMirrorsInvisible;
            On.RoR2.CharacterBody.RecalculateStats += MatchSpeed;
            On.RoR2.CharacterBody.AddMultiKill += Sync_MultiKill;
            On.RoR2.SkillLocator.ResetSkills += SkillLocator_ResetSkills;
            On.JunkPickup.OnTriggerStay += SyncDrifterJunk;
            IL.EntityStates.Seeker.Meditate.Update += SyncMeditate;
 
            RoR2Content.Items.CutHp.hidden = true;
            DLC3Content.Items.Junk.tags = DLC3Content.Items.Junk.tags.Remove(ItemTag.CannotCopy);
        }

        private static void SyncMeditate(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(1),
                x => x.MatchStfld("EntityStates.Seeker.Meditate", "wrapUpAnimationPlayed")))
            {
                c.EmitDelegate<Func<EntityStates.Seeker.Meditate, EntityStates.Seeker.Meditate>>((self) =>
                {
                    if (self.characterBody.TryGetComponent<ArtifactDoubles_IsHost>(out var link))
                    {
                        if (link.otherBody)
                        {
                            var state = link.otherBody.skillLocator.special.stateMachine;
                            if (state.state is EntityStates.Seeker.Meditate)
                            {
                                (state.state as EntityStates.Seeker.Meditate).meditationSuccess = self.meditationSuccess;
                                (state.state as EntityStates.Seeker.Meditate).missedInput = self.missedInput;
                                (state.state as EntityStates.Seeker.Meditate).meditationWrapUp = true;
                            }
                        }
                    }
                    return self;

                });
            }
            else
            {
                Debug.LogWarning("IL Failed: SyncMeditate");
            }
        }
 
      
        private static void Transform_Clones_ForDebug(On.RoR2.CharacterMaster.orig_TransformBody_string_bool orig, CharacterMaster self, string bodyName, bool og)
        {
            orig(self,bodyName, og);
            foreach(ArtifactDoubles_MasterNetwork clone in ArtifactDoubles_MasterNetwork.instances)
            {
                if (clone.originMaster == self && clone.targetMaster)
                {
                    orig(clone.targetMaster, bodyName, og);
                }
            } 
                
        }

        private static void SkillLocator_ResetSkills(On.RoR2.SkillLocator.orig_ResetSkills orig, SkillLocator self)
        {
            orig(self);
            if (self.TryGetComponent<ArtifactDoubles_BodyLinker>(out var Link))
            {
                if (Link.otherBody && Link.otherBody.skillLocator)
                {
                    orig(Link.otherBody.skillLocator);
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
            ArtifactDoubles_BodyLinker var;
            if (self.TryGetComponent<ArtifactDoubles_BodyLinker>(out var))
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

        private static void Sync_MultiKill(On.RoR2.CharacterBody.orig_AddMultiKill orig, CharacterBody self, int kills)
        {
            orig(self, kills);
            ArtifactDoubles_BodyLinker var;
            if (self.TryGetComponent<ArtifactDoubles_BodyLinker>(out var))
            {
                if (var.otherBody)
                {
                    orig(var.otherBody, kills);
                }
            }
        }

        private static VisibilityLevel MakeMirrorsInvisible(On.RoR2.CharacterBody.orig_GetVisibilityLevel_TeamIndex orig, CharacterBody self, TeamIndex observerTeam)
        {
            if (self.GetComponent<ArtifactDoubles_IsClone>())
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



        private static CharacterBody NewClone_OnRespawn(On.RoR2.CharacterMaster.orig_Respawn_Vector3_Quaternion_bool orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation, bool wasRevivedMidStage)
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
            foreach (ArtifactDoubles_MasterNetwork clone in ArtifactDoubles_MasterNetwork.instances)
            {
                if (clone.originMaster == srcCharacterMaster && clone.targetMaster)
                {
                    clone.targetMaster.TransformBody(srcCharacterMaster.bodyPrefab.name);
                }
            }
            if (srcCharacterMaster.GetInRemoteOp())
            {
                return;
            }


            MasterCopySpawnCard spawnCard = MasterCopySpawnCard.FromMaster(srcCharacterMaster, true, true, null);
            if (!spawnCard)
            {
                return;
            }
            spawnCard.prefab = DoublesCloneMaster;
            DoublesCloneMaster.GetComponent<CharacterMaster>().bodyPrefab = srcCharacterMaster.bodyPrefab;

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
                result.spawnedInstance.GetComponent<ArtifactDoubles_MasterNetwork>().originMaster = srcCharacterMaster;
                //result.spawnedInstance.GetComponent<CharacterMaster>().spawnOnStart.bodyPrefab = srcCharacterMaster.bodyPrefab;
            }));
            DirectorCore.instance.TrySpawnObject(directorSpawnRequest);

            Debug.Log("Summoner : " + directorSpawnRequest.summonerBodyObject);

            UnityEngine.Object.Destroy(spawnCard);
        }
 





        public class ArtifactDoubles_MasterNetwork : NetworkBehaviour
        {
            public static List<ArtifactDoubles_MasterNetwork> instances = new List<ArtifactDoubles_MasterNetwork>();
            public CharacterMaster _originMaster;
            public CharacterMaster originMaster
            {
                get
                {
                    return _originMaster;
                }
                set
                {
                    if (_originMaster == value)
                    {
                        return;
                    }
                    _originMaster = value;
                    SetupMaster(value);
                    if (NetworkServer.active)
                    {
                        SetDirtyBit(1U);
                    }
                }
            }
            public Inventory targetInventory;
            public Inventory originInventory;
            public CharacterMaster _targetMaster;
            public CharacterMaster targetMaster  
            {
                get { return _targetMaster; }
                set
                {
                    if (_targetMaster == value)
                    {
                        return;
                    }
                    _targetMaster = value;
                   
                }
            }

            public CharacterBody originBody;
            public CharacterBody _targetBody;        
            public CharacterBody targetBody
            {
                get { return _targetBody; }
                set
                {
                    if (_targetBody == value)
                    {
                        return;
                    }
                    _targetBody = value;
                   
                }
            }
            //targetMaster connection add to client owned objects
            //If the player can send CharacterNetworkTransform updates we surely can somehow make the clone do that too 
            //And Change Network Client Auth TO SAME
            //Has effective auth to FALSE

            public override bool OnSerialize(NetworkWriter writer, bool initialState)
            {
                //Debug.Log("OnSerialize "+ syncVarDirtyBits);
                uint b = base.syncVarDirtyBits;
                writer.Write((byte)b);
                if ((b & 1U) > 0U)
                {
                    writer.Write(_originMaster.gameObject);
                }            
                return b > 0U;
            }
            public override void OnDeserialize(NetworkReader reader, bool initialState)
            {
                //Debug.Log("OnDeserialize");
                byte b = reader.ReadByte();
                if ((b & 1U) > 0U)
                {
                    GameObject obj = reader.ReadGameObject();
                    Debug.Log(obj);
                    if (obj)
                        originMaster = obj.GetComponent<CharacterMaster>();
                }
        
            }



            public void OnEnable()
            {
                instances.AddDistinct(this);
                targetMaster = this.GetComponent<CharacterMaster>();
                targetInventory = targetMaster.GetComponent<Inventory>();
                targetInventory.GiveItemChanneled(RoR2Content.Items.AdaptiveArmor.itemIndex);
                targetMaster.onBodyStart += TargetMaster_onBodyStart;

                extraEquip = DLC3Content.Items.ExtraEquipment.itemIndex;
            }

            public void SetupMaster(CharacterMaster master)
            {
              
                if (!master)
                {
                    Debug.LogError("ArtifactDoubles_Networked: NO MASTER");
                    return;
                }
           
             
                targetMaster.originalBodyPrefab = master.originalBodyPrefab;
                originInventory = master.GetComponent<Inventory>();
                if (NetworkServer.active)
                {
                    master.networkIdentity.clientAuthorityOwner.clientOwnedObjects.Add(targetMaster.networkIdentity.netId);
                    originInventory.onInventoryChanged += OriginInventory_onInventoryChanged;
                    originInventory.HandleInventoryChanged();
                }
  
                master.onBodyStart += OriginMaster_onBodyStart;
                OriginMaster_onBodyStart(master.GetBody());
 
                if (master.playerCharacterMasterController.networkUser.isLocalPlayer)
                {
                    ArtifactDoubles_MovementController var = targetMaster.gameObject.EnsureComponent<ArtifactDoubles_MovementController>();
                    var.originMaster = master;
                }

                originInventory.GiveItemPermanent(RoR2Content.Items.CutHp, 1 - originInventory.GetItemCountPermanent(RoR2Content.Items.CutHp));

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
                    if (NetworkServer.active)
                    {
                        originMaster.networkIdentity.clientAuthorityOwner.clientOwnedObjects.Add(obj.networkIdentity.netId);
                        targetMaster.networkIdentity.m_ClientAuthorityOwner = originMaster.networkIdentity.clientAuthorityOwner;
                        obj.networkIdentity.m_ClientAuthorityOwner = originMaster.networkIdentity.clientAuthorityOwner;
                        obj.GetComponent<CharacterNetworkTransform>().hasEffectiveAuthority = false;
                    }

                    obj.baseNameToken = Language.GetStringFormatted("DOUBLE_PREFIX",originMaster.playerCharacterMasterController.GetDisplayName());

                    SharedBodySetup();
                }
            }

            private void OriginMaster_onBodyStart(CharacterBody obj)
            {
                originBody = obj;
                SharedBodySetup();
            }

            public void SharedBodySetup()
            {
                if (targetBody && originBody)
                {
                    //Has to be here because networked
                    if (NetworkServer.active)
                    {
                        VoidSurvivorController voidTarget = targetBody.GetComponent<VoidSurvivorController>();
                        if (voidTarget)
                        {
                            VoidSurvivorController voidOrigin = originBody.GetComponent<VoidSurvivorController>();
                            var voidSync = targetBody.GetComponent<ArtifactDoubles_SyncVoidSurv>();
                            if (!voidSync)
                            {
                                voidSync = targetBody.gameObject.AddComponent<ArtifactDoubles_SyncVoidSurv>();
                            }
                            voidSync.originVoidSurv = voidOrigin;
                        }
 
                    }
        
                    targetBody.healthComponent.godMode = originBody.healthComponent.godMode;

                    if (targetBody.TryGetComponent<InteractionDriver>(out var drive))
                    {
                        drive.enabled = false;
                    }

                    JunkController drifterTarget = targetBody.GetComponent<JunkController>();
                    if (drifterTarget)
                    {
                        originBody.bodyFlags = originBody.bodyFlags | CharacterBody.BodyFlags.Ungrabbable;
                        drifterTarget.MaxJunk = 6;
                    }
                }
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
                if (targetInventory)
                {
                    if (targetBody && targetBody.teamComponent.teamIndex != TeamIndex.Player)
                    {
                        targetInventory.SetActiveEquipmentSet(0);
                        targetInventory.RemoveItemPermanent(RoR2Content.Items.CutHp, 1);
                        targetInventory.RemoveItemPermanent(RoR2Content.Items.AdaptiveArmor, 1);
                        return;
                    }

                    //SLOT MULT
                    //SET COUPLER

                    //Dont drop equipment from invalidation on clones

                    targetInventory.CopyItemsFrom(originInventory, ourFilter);
                    //CleanTempItems();
                    CopyTempItemsFrom();
                    targetInventory.CopyEquipmentFrom(originInventory, true);
                    
                    if (originInventory.activeEquipmentSet.Length > 0)
                    {
                        targetInventory.SetActiveEquipmentSet(originInventory.activeEquipmentSet[0]);
                    }
                    targetInventory.infusionBonus = originInventory.infusionBonus;
                    targetInventory.beadAppliedDamage = originInventory.beadAppliedDamage;
                    targetInventory.beadAppliedHealth = originInventory.beadAppliedHealth;
                    targetInventory.beadAppliedRegen = originInventory.beadAppliedRegen;
                }

            }

            public void CleanTempItems()
            {
                targetInventory.tempItemsStorage.tempItemStacks.Clear();
                targetInventory.tempItemsStorage.decayToZeroTimeStamps.Clear();
                targetInventory.tempItemsStorage.SyncStacksToDecay();
              
            }
            public void CopyTempItemsFrom()
            {
                targetInventory.tempItemsStorage.tempItemStacks.Clear();
                targetInventory.tempItemsStorage.decayToZeroTimeStamps.Clear();
                using (new Inventory.InventoryChangeScope(targetInventory))
                {
                    List<ItemIndex> list;
                    using (CollectionPool<ItemIndex, List<ItemIndex>>.RentCollection(out list))
                    {
                        originInventory.tempItemsStorage.GetNonZeroIndices(list);
                        foreach (ItemIndex itemIndex in list)
                        {
                            targetInventory.GiveItemTemp(itemIndex, ourFilter(itemIndex) ? originInventory.tempItemsStorage.GetItemRawValue(itemIndex) : 0);                  
                        }
                    }
                }
                targetInventory.tempItemsStorage.SyncStacksToDecay();
            }

            public static readonly Func<ItemIndex, bool> ourFilter = new Func<ItemIndex, bool>(DefaultItemCopyFilter);
            public static ItemIndex extraEquip;
            private static bool DefaultItemCopyFilter(ItemIndex itemIndex)
            {
                return !ItemCatalog.GetItemDef(itemIndex).ContainsTag(ItemTag.CannotCopy);
                //return true;
            }

        }

        public class ArtifactDoubles_MovementController : MonoBehaviour
        {
            public static List<ArtifactDoubles_MovementController> instances = new List<ArtifactDoubles_MovementController>();

            public CharacterMaster _originMaster;
            public CharacterMaster originMaster
            {
                get
                {
                    return _originMaster;
                }
                set
                {
                    if (_originMaster == value)
                    {
                        return;
                    }
                    _originMaster = value;
                    SetupMaster(value);
                }
            }
            public CharacterMaster targetMaster;
            public CharacterBody originBody;
            public CharacterBody targetBody;
            public InputBankTest targetInput;
            public InputBankTest originInputs;

            public CharacterMotor originMotor;
            public CharacterMotor targetMotor;
            public RigidbodyMotor originMotor_Drone;
            public RigidbodyMotor targetMotor_Drone;

            //public EquipmentSlot originEquipmentSlot;
            //public HurtBox targetTarget;

            private float teleportAttemptTimer = 1f;
            public GameObject helperPrefab;

            public ArtifactDoubles_IsHost originBodyIdentifier;
            public ArtifactDoubles_IsClone targetBodyIdentifier;

            public void OnEnable()
            {
                instances.Add(this);
                
                helperPrefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
           
            }

            public void SetupMaster(CharacterMaster master)
            {
                targetMaster = this.GetComponent<CharacterMaster>();
               
                targetMaster.onBodyStart += TargetMaster_onBodyStart;
                originMaster.onBodyStart += OriginMaster_onBodyStart;

                OriginMaster_onBodyStart(originMaster.GetBody());
                TargetMaster_onBodyStart(targetMaster.GetBody());

                Debug.Log("MirrorSwarms_MovementController.SetupMaster " + originMaster + "  " + targetMaster);
            }

            private void OriginMaster_onBodyStart(CharacterBody obj)
            {
                Debug.Log("OriginMaster_onBodyStart " + obj);
                if (obj)
                {
                  
                    originBody = obj;
                    originInputs = originBody.inputBank;
                    originMotor = originBody.characterMotor;
                    originMotor_Drone = originBody.GetComponent<RigidbodyMotor>();
                    //originEquipmentSlot = originBody.equipmentSlot;

                    SharedBodySetup();
                    SetupTargetSearch();
                }
            }

            //WHAT THEç FUCK DO YOU MEAN THEY CAN BUY SHIT
            public void SharedBodySetup()
            {
                if (targetBody && originBody)
                {
                    originBodyIdentifier = originBody.gameObject.GetComponent<ArtifactDoubles_IsHost>();
                    targetBodyIdentifier = targetBody.gameObject.GetComponent<ArtifactDoubles_IsClone>();
                    if (originBodyIdentifier == null)
                    {
                        originBodyIdentifier = originBody.gameObject.AddComponent<ArtifactDoubles_IsHost>();
                    }
                    if (targetBodyIdentifier == null)
                    {
                        targetBodyIdentifier = targetBody.gameObject.AddComponent<ArtifactDoubles_IsClone>();
                    }
                    originBodyIdentifier.otherBody = targetBody;
                    originBodyIdentifier.otherLink = targetBodyIdentifier;
                    targetBodyIdentifier.otherBody = originBody;
                    targetBodyIdentifier.originCollider = originBody.GetComponent<Collider>();
                    targetBodyIdentifier.otherLink = originBodyIdentifier;

        
                    ChefController chefTarget = targetBody.GetComponent<ChefController>();
                    if (chefTarget)
                    {
                        ChefController chefOrigin = originBody.GetComponent<ChefController>();
                        var chefSync = targetBody.GetComponent<ArtifactDoubles_SyncCHEF>();
                        if (!chefSync)
                        {
                            chefSync = targetBody.gameObject.AddComponent<ArtifactDoubles_SyncCHEF>();
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
                    targetMotor_Drone = targetBody.GetComponent<RigidbodyMotor>();
                    targetBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 5);

                    targetBody.GetComponent<EntityStateMachine>().SetNextStateToMain();

                    SharedBodySetup();
                }

            }

            private BullseyeSearch targetFinder = new BullseyeSearch();
            private HurtBox currentTarget;


           
            public void SetupTargetSearch()
            {
                targetFinder.teamMaskFilter = TeamMask.GetUnprotectedTeams(TeamIndex.Player);
                targetFinder.sortMode = BullseyeSearch.SortMode.Angle;
                targetFinder.filterByLoS = true;
                targetFinder.maxAngleFilter = 8f; //10 on equip, 30 on huntress
                //targetFinder.maxDistanceFilter = 40;
                targetFinder.viewer = this.originBody;
       
            }

            private Ray GetAimRay()
            {
                return new Ray
                {
                    direction = this.originInputs.aimDirection,
                    origin = this.originInputs.aimOrigin
                };
            }
            private void UpdateTargets()
            {
                Ray ray = CameraRigController.ModifyAimRayIfApplicable(this.GetAimRay(), originBody.gameObject, out _);
                this.targetFinder.searchOrigin = ray.origin;
                this.targetFinder.searchDirection = ray.direction;
                this.targetFinder.RefreshCandidates();
                this.targetFinder.FilterOutGameObject(base.gameObject);

                currentTarget = this.targetFinder.GetResults().FirstOrDefault<HurtBox>();
            }
            private float stopWatchTimer;
            private float stopMovementTimer;

            public void FixedUpdate()
            {
                if (targetInput && originBody)
                {
                    if (originInputs.interact.wasDown)
                    {
                        stopMovementTimer += Time.fixedDeltaTime;
                    }
                    else
                    {
                        stopMovementTimer = 0f;
                    }
                    if (stopMovementTimer > 0.12f)
                    {
                        targetInput.sprint.PushState(false);
                    }
                    else
                    {
                        targetInput.skill3.PushState(originInputs.skill3.down);
                        targetInput.sprint = originInputs.sprint;
                    }
                    targetInput.skill1.PushState(originInputs.skill1.down);
                    targetInput.skill2.PushState(originInputs.skill2.down);
                    targetInput.skill4.PushState(originInputs.skill4.down);



                    /*if (originEquipmentSlot)
                    {
                        targetTarget = originEquipmentSlot.currentTarget.hurtBox;
                        if (targetTarget == null)
                        {
                            UpdateTargets();
                            originEquipmentSlot.ConfigureTargetFinderForEnemies();
                            targetTarget = originEquipmentSlot.targetFinder.GetResults().FirstOrDefault<HurtBox>();
                        }
                    }*/
                    stopWatchTimer += Time.fixedDeltaTime;
                    if (stopWatchTimer > 0.2f)
                    {
                        UpdateTargets();
                    }
                    if (currentTarget)
                    {
                        targetInput.aimDirection = (currentTarget.transform.position - targetInput.aimOrigin).normalized;
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
                            if (originMotor)
                            {
                                Vector3 additionalDistance = originMotor.velocity / 10;
                                additionalDistance.y = 0;
                                corePosition += additionalDistance;
                            }
                      

                            TeleportHelper.TeleportBody(targetBody, corePosition, false);
                            GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(targetBody.gameObject);
                            if (teleportEffectPrefab)
                            {
                                EffectManager.SimpleEffect(teleportEffectPrefab, corePosition, Quaternion.identity, true);

                            }
                            targetBody.AddTimedBuff(JunkContent.Buffs.IgnoreFallDamage,3f);

                            if (originMotor != null)
                            {
                                targetMotor.rootMotion = originMotor.rootMotion;
                                targetMotor.moveDirection = originMotor.moveDirection;
                                targetMotor.velocity = originMotor.velocity;
                            }
                            else
                            {
                                targetMotor_Drone.rootMotion = originMotor_Drone.rootMotion;
                                targetMotor_Drone.moveVector = originMotor_Drone.moveVector;
                                targetMotor_Drone.rigid.velocity = originMotor_Drone.rigid.velocity;
                            }

                        }
                    }
                }
            }

            public void Update()
            {
                //While inputs are updated on Fixed Update
                if (originInputs)
                {
                    if (stopMovementTimer > 0.12f)
                    {         
                        targetInput.jump.PushState(false);
                        targetInput.moveVector = Vector3.zeroVector;
                    }
                    else
                    {
                        targetInput.moveVector = originInputs.moveVector;
                        targetInput.jump = originInputs.jump;
                    }
                }
            }

            public void OnDisable()
            {
                instances.Remove(this);
                if (targetMaster)
                {
                    targetMaster.onBodyStart -= OriginMaster_onBodyStart;
                }
                if (originMaster)
                {
                    originMaster.onBodyStart -= OriginMaster_onBodyStart;
                }
            }

        }
 
        public class ArtifactDoubles_BodyLinker : MonoBehaviour
        {
            public CharacterBody thisBody;
            public CharacterBody otherBody;



            public ArtifactDoubles_BodyLinker otherLink;
 
            public float preEqualizedMoveSpeed;
            public float preEqualizedAcceleration;

  
            public void OnEnable()
            {
                thisBody = this.GetComponent<CharacterBody>();
            }
            public void OnDisable()
            {
                if (otherLink)
                {
                   
                }
            }
        }

        public class ArtifactDoubles_IsClone : ArtifactDoubles_BodyLinker
        {
            public Collider originCollider;
        }
        public class ArtifactDoubles_IsHost : ArtifactDoubles_BodyLinker
        {

        }




        public class ArtifactDoubles_SyncVoidSurv : MonoBehaviour
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

        public class ArtifactDoubles_SyncCHEF : MonoBehaviour
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


    }
}

