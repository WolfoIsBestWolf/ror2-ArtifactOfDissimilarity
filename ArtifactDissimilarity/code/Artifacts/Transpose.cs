using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace ArtifactDissimilarity
{
    public class Transpose
    {
        public static CharacterBody RandomizeLoadoutRespawnMethod(On.RoR2.CharacterMaster.orig_Respawn orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation)
        {
            if (self.playerCharacterMasterController)
            {
                if (!RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.randomSurvivorOnRespawnArtifactDef))
                {
                    RerollLoadout(self.bodyPrefab, self);
                }
            }
            return orig(self, footPosition, rotation);
        }

        private static void RerollLoadout(GameObject bodyPrefab, CharacterMaster master)
        {
            CharacterBody characterBody = bodyPrefab.GetComponent<CharacterBody>();
            Loadout newloadout = new Loadout();
            master.loadout.Copy(newloadout);

            GenericSkill[] skills = bodyPrefab.GetComponents<GenericSkill>();

            int repeatSkills = 0;
            //bool inRepeat = false;
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
                    if (repeatSkills == 4)
                    {
                        i = 0;
                        repeatSkills++;
                        Debug.Log("Rerolling too similiar loadout");
                    }
                }
            }
            newloadout.bodyLoadoutManager.SetSkinIndex(characterBody.bodyIndex, (uint)Main.random.Next(0, RoR2.SkinCatalog.GetBodySkinCount(characterBody.bodyIndex)));
            master.SetLoadoutServer(newloadout);
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