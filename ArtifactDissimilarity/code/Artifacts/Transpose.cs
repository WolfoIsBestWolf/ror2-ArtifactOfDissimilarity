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
                //self.playerCharacterMasterController.networkUser.CopyLoadoutToMaster();
                CharacterBody tempbod = orig(self, footPosition, rotation);

                Loadout newloadout = new Loadout();
                newloadout.Copy(self.loadout);
                int globalrepeat = 0;
                int repeat = 0;
                do
                {
                    if (repeat == tempbod.skillLocator.skillSlotCount)
                    {
                        globalrepeat++;
                        Debug.Log("Repeat Loadout " + globalrepeat + ", rerolling again");
                    }
                    repeat = 0;
                    for (int i = 0; i < tempbod.skillLocator.skillSlotCount; i++)
                    {
                        var tempgenericskill = tempbod.skillLocator.GetSkillAtIndex(i);
                        uint skillVariant = (uint)UnityEngine.Random.Range(0, tempgenericskill.skillFamily.variants.Length);
                        newloadout.bodyLoadoutManager.SetSkillVariant(tempbod.bodyIndex, i, skillVariant);

                        string tempSkillOldName = tempgenericskill.skillDef.ToString();
                        string tempSkillOldType = tempgenericskill.skillDef.GetType().ToString();
                        tempSkillOldName = tempSkillOldName.Replace(" (" + tempSkillOldType + ")", "");
                        var tempSkillindexOld = tempgenericskill.skillFamily.GetVariantIndex(tempSkillOldName);
                        var tempSkillindexNew = newloadout.bodyLoadoutManager.GetSkillVariant(tempbod.bodyIndex, i);

                        if (tempSkillindexNew == tempSkillindexOld)
                        {
                            repeat++;
                        }
                        self.loadout.bodyLoadoutManager.SetSkillVariant(tempbod.bodyIndex, i, skillVariant);


                        //Debug.LogWarning(tempgenericskill.skillFamily + " " + tempgenericskill.skillFamily.variants.Length + " different variants");
                        //Debug.LogWarning("slot " + i + ": variant " + skillVariant);
                        //Debug.LogWarning("Old Index " + tempSkillindexOld);
                        //Debug.LogWarning("New Index " + tempSkillindexNew);
                        //Debug.LogWarning("Repeats "+ repeat);
                        //Debug.LogWarning(tempbod.skillLocator.skillSlotCount);
                    }

                } while (repeat == tempbod.skillLocator.skillSlotCount && globalrepeat < 2);
                self.loadout.bodyLoadoutManager.SetSkinIndex(tempbod.bodyIndex, (uint)Main.random.Next(0, RoR2.SkinCatalog.GetBodySkinCount(tempbod.bodyIndex)));
                tempbod.SetLoadoutServer(newloadout);
                Debug.Log("Rerolled " + tempbod.name + "'s Loadout ");
                return tempbod;
            }
            return orig(self, footPosition, rotation);
        }

        public static CharacterBody UnRandomizeLoadoutRespawnMethod(On.RoR2.CharacterMaster.orig_Respawn orig, CharacterMaster self, Vector3 footPosition, Quaternion rotation)
        {
            if (self.playerCharacterMasterController)
            {
                self.playerCharacterMasterController.networkUser.CopyLoadoutToMaster();
            }
            return orig(self, footPosition, rotation);
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


        //Obsolete
        public static void RandomizeLoadoutRespawn(On.RoR2.Stage.orig_RespawnCharacter orig, global::RoR2.Stage self, global::RoR2.CharacterMaster characterMaster)
        {
            orig(self, characterMaster);

            if (NetworkServer.active)
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Main.Transpose_Def))
                {
                    var tempbod = characterMaster.GetBody();
                    Loadout newloadout = new Loadout();
                    newloadout.Copy(characterMaster.loadout);
                    int globalrepeat = 0;
                    int repeat = 0;
                    do
                    {
                        if (repeat == tempbod.skillLocator.skillSlotCount)
                        {
                            globalrepeat++;
                            Debug.Log("Repeat Loadout " + globalrepeat + ", rerolling again");
                        }
                        repeat = 0;
                        for (int i = 0; i < tempbod.skillLocator.skillSlotCount; i++)
                        {
                            var tempgenericskill = tempbod.skillLocator.GetSkillAtIndex(i);
                            uint skillVariant = (uint)UnityEngine.Random.Range(0, tempgenericskill.skillFamily.variants.Length);
                            newloadout.bodyLoadoutManager.SetSkillVariant(tempbod.bodyIndex, i, skillVariant);

                            string tempSkillOldName = tempgenericskill.skillDef.ToString();
                            string tempSkillOldType = tempgenericskill.skillDef.GetType().ToString();
                            tempSkillOldName = tempSkillOldName.Replace(" (" + tempSkillOldType + ")", "");
                            var tempSkillindexOld = tempgenericskill.skillFamily.GetVariantIndex(tempSkillOldName);
                            var tempSkillindexNew = newloadout.bodyLoadoutManager.GetSkillVariant(tempbod.bodyIndex, i);

                            if (tempSkillindexNew == tempSkillindexOld)
                            {
                                repeat++;
                            }
                            characterMaster.loadout.bodyLoadoutManager.SetSkillVariant(tempbod.bodyIndex, i, skillVariant);


                            //Debug.LogWarning(tempgenericskill.skillFamily + " " + tempgenericskill.skillFamily.variants.Length + " different variants");
                            //Debug.LogWarning("slot " + i + ": variant " + skillVariant);
                            //Debug.LogWarning("Old Index " + tempSkillindexOld);
                            //Debug.LogWarning("New Index " + tempSkillindexNew);
                            //Debug.LogWarning("Repeats "+ repeat);
                            //Debug.LogWarning(tempbod.skillLocator.skillSlotCount);
                        }

                    } while (repeat == tempbod.skillLocator.skillSlotCount && globalrepeat < 2);

                    tempbod.SetLoadoutServer(newloadout);

                    characterMaster.loadout.bodyLoadoutManager.SetSkinIndex(tempbod.bodyIndex, (uint)UnityEngine.Random.Range(0, RoR2.SkinCatalog.GetBodySkinCount(tempbod.bodyIndex)));
                    Debug.Log("Rerolled " + tempbod.name + "'s Loadout ");
                }
            }
        }
    }
}