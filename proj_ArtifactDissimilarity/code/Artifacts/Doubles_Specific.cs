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


        public static void SpecificHooks()
        {

            On.RoR2.SeekerController.UpdatePetalsUI += RandomNullRefCheck_SeekerController_UpdatePetalsUI;
            On.RoR2.SojournVehicleBase.OnPassengerEnter += SojournSyncLocalUser;

            On.RoR2.CharacterMaster.UpdateBodyGodMode += CharacterMaster_UpdateBodyGodMode;

            On.RoR2.Projectile.ProjectileTransferItem.GrantItemToImpacted_HurtBox += AlwaysPutNeutronWeightToCloneHost;
            //On.RoR2.Inventory.RemoveEquipmentSet += DontDropExcessEquipmentFromClonesWhenItemDisabled;
            On.RoR2.Inventory.DispatchSwitchToNextEquipmentInSet += DisableExtraEquipItemFromMessingUpClones;
            On.RoR2.EquipmentSlot.ExecuteIfReady += SyncEquipmentActivations;
        }

        private static void DisableExtraEquipItemFromMessingUpClones(On.RoR2.Inventory.orig_DispatchSwitchToNextEquipmentInSet orig, Inventory self)
        {
            if (self.TryGetComponent<ArtifactDoubles_MasterNetwork>(out var link))
            {
                return;
            }
            orig(self);
        }

        private static bool SyncEquipmentActivations(On.RoR2.EquipmentSlot.orig_ExecuteIfReady orig, EquipmentSlot self)
        {
            bool temp = orig(self);
            if (temp && self.TryGetComponent<ArtifactDoubles_IsHost>(out var link))
            {
                link.otherBody.equipmentSlot.ExecuteIfReady();
            }
            return temp;
        }

        private static void DontDropExcessEquipmentFromClonesWhenItemDisabled(On.RoR2.Inventory.orig_RemoveEquipmentSet orig, Inventory self)
        {
            if (self.GetComponent<ArtifactDoubles_MasterNetwork>())
            {
                return;
            }
            orig(self);
        }

        private static void AlwaysPutNeutronWeightToCloneHost(On.RoR2.Projectile.ProjectileTransferItem.orig_GrantItemToImpacted_HurtBox orig, RoR2.Projectile.ProjectileTransferItem self, HurtBox hurtbox)
        {
            orig(self, hurtbox);
            if (self.hasGivenItem)
            {
                if (hurtbox.healthComponent.body.TryGetComponent<ArtifactDoubles_IsClone>(out var link))
                {
                    if (link.otherBody)
                    {
                        link.otherBody.inventory.GiveItemPermanent(self.itemIndex, self.count);
                    }
                }
            }
        }

        private static void CharacterMaster_UpdateBodyGodMode(On.RoR2.CharacterMaster.orig_UpdateBodyGodMode orig, CharacterMaster self)
        {
            orig(self);
            if (self.GetBody() != null)
            {
                if (self.GetBody().TryGetComponent<ArtifactDoubles_IsHost>(out var link) && link.otherBody)
                {
                    link.otherBody.healthComponent.godMode = self.godMode;
                }
            }
        }


        private static void SojournSyncLocalUser(On.RoR2.SojournVehicleBase.orig_OnPassengerEnter orig, SojournVehicleBase self, GameObject passenger)
        {
            orig(self, passenger);
            if (self.characterBody.TryGetComponent<ArtifactDoubles_IsClone>(out var link) && link.otherBody)
            {
                foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
                {
                    if (networkUser.master == link.otherBody.master)
                    {
                        self.localUser = networkUser.localUser;
                    }
                }
            }
        }

        private static void RandomNullRefCheck_SeekerController_UpdatePetalsUI(On.RoR2.SeekerController.orig_UpdatePetalsUI orig, SeekerController self, byte value)
        {
            if (!self.overlayInstanceChildLocator)
            {
                return;
            }
            orig(self, value);
        }


        private static void SyncDrifterJunk(On.JunkPickup.orig_OnTriggerStay orig, JunkPickup self, Collider other)
        {
            if (other.TryGetComponent<ArtifactDoubles_IsClone>(out var a))
            {
                orig(self, a.originCollider);
            }
            orig(self, other);
        }


    }

}

