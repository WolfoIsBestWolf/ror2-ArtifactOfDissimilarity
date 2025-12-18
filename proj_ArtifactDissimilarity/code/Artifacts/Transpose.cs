using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace ArtifactDissimilarity.Aritfacts
{
    public class Transpose
    {
        public static void OnArtifactEnable()
        {
            On.RoR2.CharacterMaster.Respawn_Vector3_Quaternion_bool += Transpose.RandomizeLoadoutRespawnMethod;
            On.RoR2.CharacterMaster.PickRandomSurvivorBodyPrefab += Transpose.Transpose_Metamorphosis;
            if (WConfig.TransposeRerollHeresy.Value == true)
            {
                On.RoR2.SceneDirector.Start += Transpose.RandomizeHeresyItems;
            }
            if (NetworkServer.active)
            {
                foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
                {
                    playerCharacterMasterController.StopAllCoroutines();
                    playerCharacterMasterController.StartCoroutine(Main.DelayedRespawn(playerCharacterMasterController, 0.35f));
                    //Transpose.RerollLoadout(playerCharacterMasterController.master.GetBodyObject(), playerCharacterMasterController.master);
                }
            }
            Debug.Log("Added Transpose");
        }
        public static void OnArtifactDisable()
        {
            On.RoR2.CharacterMaster.Respawn_Vector3_Quaternion_bool -= Transpose.RandomizeLoadoutRespawnMethod;
            On.RoR2.CharacterMaster.PickRandomSurvivorBodyPrefab -= Transpose.Transpose_Metamorphosis;
            if (WConfig.TransposeRerollHeresy.Value == true)
            {
                On.RoR2.SceneDirector.Start -= Transpose.RandomizeHeresyItems;
            }
            foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
            {
                playerCharacterMasterController.networkUser.CopyLoadoutToMaster();
                playerCharacterMasterController.StopAllCoroutines();
                playerCharacterMasterController.StartCoroutine(Main.DelayedRespawn(playerCharacterMasterController, 0.1f));
            }
            Debug.Log("Removed Transpose");
        }

        public static CharacterBody RandomizeLoadoutRespawnMethod(On.RoR2.CharacterMaster.orig_Respawn_Vector3_Quaternion_bool orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation, bool midStageRevive)
        {
            if (self.playerCharacterMasterController)
            {
                if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.randomSurvivorOnRespawnArtifactDef))
                {
                    RerollLoadout(self.bodyPrefab, self);
                }
            }
            return orig(self, footPosition, rotation, midStageRevive);
        }

        public static void RerollLoadout(GameObject bodyPrefab, CharacterMaster master)
        {
            CharacterBody characterBody = bodyPrefab.GetComponent<CharacterBody>();
            Loadout newloadout = new Loadout();
            master.loadout.Copy(newloadout);

            GenericSkill[] skills = bodyPrefab.GetComponents<GenericSkill>();

            int repeatSkills = 0;
            //bool inRepeat = false;
            int rerolls = 0;
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] != null)
                {
                    uint oldSkillVariant = master.loadout.bodyLoadoutManager.GetSkillVariant(characterBody.bodyIndex, i);
                    uint skillVariant = (uint)UnityEngine.Random.Range(0, skills[i].skillFamily.variants.Length);
                    newloadout.bodyLoadoutManager.SetSkillVariant(characterBody.bodyIndex, i, skillVariant);
                    if (oldSkillVariant == skillVariant)
                    {
                        repeatSkills++;
                    }
                    if (repeatSkills == 4 && rerolls != 3)
                    {
                        i = 0;
                        rerolls++;
                        Debug.Log("Rerolling too similiar loadout");
                    }
                }
            }
            int skinIndex = Main.random.Next(0, RoR2.SkinCatalog.GetBodySkinCount(characterBody.bodyIndex));
            newloadout.bodyLoadoutManager.SetSkinIndex(characterBody.bodyIndex, (uint)skinIndex);
            master.SetLoadoutServer(newloadout);

            CharacterBody body = master.GetBody();
            if (body)
            {
                body.SetLoadoutServer(newloadout);
                ModelSkinController model = body.modelLocator.modelTransform.gameObject.GetComponent<ModelSkinController>();
                if (model)
                {
                    model.ApplySkin(skinIndex);
                }
            }
            Debug.Log("Rerolled " + bodyPrefab + "'s Loadout ");
        }

        public static GameObject Transpose_Metamorphosis(On.RoR2.CharacterMaster.orig_PickRandomSurvivorBodyPrefab orig, Xoroshiro128Plus rng, NetworkUser networkUser, bool allowHidden)
        {
            GameObject bodyPrefab = orig(rng, networkUser, allowHidden);
            if (networkUser.master)
            {
                RerollLoadout(bodyPrefab, networkUser.master);
            }
            return bodyPrefab;
        }

        public static void RandomizeHeresyItems(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (!NetworkServer.active) { return; }
            foreach (var playerController in PlayerCharacterMasterController.instances)
            {
                Inventory inv = playerController.master.inventory;

                List<ItemDef> HeresyItemList = new List<ItemDef>() {
                    RoR2Content.Items.LunarPrimaryReplacement,
                    RoR2Content.Items.LunarSecondaryReplacement,
                    RoR2Content.Items.LunarUtilityReplacement,
                    RoR2Content.Items.LunarSpecialReplacement
                };
                List<int> HeresyItemCounts = new List<int>();

                for (int i = 0; i < HeresyItemList.Count; i++)
                {
                    int count = inv.GetItemCount(HeresyItemList[i]);
                    HeresyItemCounts.Add(count);
                    inv.RemoveItem(HeresyItemList[i], count);
                }

                for (int i = 0; i < 4; i++)
                {
                    int randomItem = Main.random.Next(HeresyItemList.Count);
                    int randomCount = Main.random.Next(HeresyItemCounts.Count);

                    inv.GiveItem(HeresyItemList[randomItem], HeresyItemCounts[randomCount]);
                    HeresyItemList.Remove(HeresyItemList[randomItem]);
                    HeresyItemCounts.Remove(HeresyItemCounts[randomCount]);
                }
            }

        }
    }
}