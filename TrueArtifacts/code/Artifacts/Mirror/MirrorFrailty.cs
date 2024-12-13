using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace TrueArtifacts
{
    public class MirrorFrailty
    {
        public static void On_Artifact_Disable()
        {
            On.RoR2.JumpVolume.OnTriggerStay -= JumpVolume_OnTriggerStay;
            On.RoR2.SetGravity.OnEnable -= SetGravity_OnEnable;
            Run.baseGravity /= 2;
            Physics.gravity = new Vector3(0f, Run.baseGravity, 0f);
        }
        public static void On_Artifact_Enable()
        {
            On.RoR2.JumpVolume.OnTriggerStay += JumpVolume_OnTriggerStay;
            On.RoR2.SetGravity.OnEnable += SetGravity_OnEnable;
            Run.baseGravity *= 2;
            Physics.gravity = new Vector3(0f, Run.baseGravity, 0f);
        }

        private static void JumpVolume_OnTriggerStay(On.RoR2.JumpVolume.orig_OnTriggerStay orig, JumpVolume self, Collider other)
        {
            orig(self,other);
            CharacterMotor component = other.GetComponent<CharacterMotor>();
            if (component && component.hasEffectiveAuthority && !component.doNotTriggerJumpVolumes)
            {
                Debug.Log(component);
                component.velocity = self.jumpVelocity*1.4f;
            }
        }

        private static void SetGravity_OnEnable(On.RoR2.SetGravity.orig_OnEnable orig, SetGravity self)
        {
            self.newGravity *= 2;
            orig(self);
        }

     }

}

