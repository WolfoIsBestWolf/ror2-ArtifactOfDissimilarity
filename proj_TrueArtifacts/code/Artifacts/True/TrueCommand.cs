using System;
using UnityEngine;
using RoR2;
using RoR2.Artifacts;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;
using RoR2.UI;

namespace TrueArtifacts.Aritfacts
{
    public class TrueCommand
    {
        //Somehow do rainbow lights and effects or smth
        public static void Start()
        {
            On.RoR2.PickupDropletController.CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 += PickupDropletController_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3;
            On.RoR2.PickupTransmutationManager.RebuildPickupGroups += AddEliteEquipmentsToCommand;

            LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/CommandCube").AddComponent<RainbowItemThing>();
        }

        private static void AddEliteEquipmentsToCommand(On.RoR2.PickupTransmutationManager.orig_RebuildPickupGroups orig)
        {
            for (var i = 0; i < EliteCatalog.eliteDefs.Length; i++)
            {
                EliteDef eliteDef = EliteCatalog.eliteDefs[i];
                EquipmentDef tempEliteEquip = eliteDef.eliteEquipmentDef;
                if (tempEliteEquip != null)
                {
                    if (!eliteDef.name.EndsWith("Gold"))
                    {
                        if (tempEliteEquip.dropOnDeathChance > 0)
                        {
                            tempEliteEquip.canDrop = true;
                            tempEliteEquip.isBoss = true;
                            tempEliteEquip.isLunar = false;
                        }
                    }
                }
            }
            orig();
            for (var i = 0; i < EliteCatalog.eliteDefs.Length; i++)
            {
                EliteDef eliteDef = EliteCatalog.eliteDefs[i];
                EquipmentDef tempEliteEquip = eliteDef.eliteEquipmentDef;

                if (tempEliteEquip != null)
                {
                    if (tempEliteEquip.dropOnDeathChance > 0)
                    {
                        tempEliteEquip.canDrop = false;
                    }
                }
            }

        }


        private static void PickupDropletController_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3(On.RoR2.PickupDropletController.orig_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 orig, GenericPickupController.CreatePickupInfo pickupInfo, Vector3 position, Vector3 velocity)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(Main.True_Command))
            {
                pickupInfo.artifactFlag |= GenericPickupController.PickupArtifactFlag.COMMAND;
            }
            orig(pickupInfo, position, velocity);
        }

        public static void On_Artifact_Enable()
        {
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact_UniquePickup += Override_Command_Options;
            SceneDirector.onGenerateInteractableCardSelection += CommandArtifactManager.OnGenerateInteractableCardSelection;
            On.RoR2.Run.IsItemAvailable += Run_IsItemAvailable;
            On.RoR2.Run.IsEquipmentAvailable += Run_IsEquipmentAvailable;
            FirstTimeOverrideJunk();

            //LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/CommandPickerPanel").transform.localScale = Vector3.one*0.85f;     
            int skull = PickupCatalog.FindPickupIndex(JunkContent.Items.SkullCounter.itemIndex).value;
            if (PickupTransmutationManager.pickupGroupMap[skull] != null)
            {
                float longth = 1 - (PickupTransmutationManager.pickupGroupMap[skull].Length / 1000f);
                int rows = PickupTransmutationManager.pickupGroupMap[skull].Length / 12;
                GameObject pannel = LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/CommandPickerPanel");
                pannel.transform.localScale = Vector3.one * longth;
                pannel.GetComponent<PickupPickerPanel>().maxColumnCount = rows;
            }
        }

        public static void On_Artifact_Disable()
        {
            On.RoR2.PickupPickerController.SetOptionsFromPickupForCommandArtifact_UniquePickup -= Override_Command_Options;
            SceneDirector.onGenerateInteractableCardSelection -= CommandArtifactManager.OnGenerateInteractableCardSelection;
            On.RoR2.Run.IsItemAvailable -= Run_IsItemAvailable;
            On.RoR2.Run.IsEquipmentAvailable -= Run_IsEquipmentAvailable;
            LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/CommandPickerPanel").transform.localScale = Vector3.one;
            if (Run.instance)
            {
                Run.instance.availableEquipment.Remove(RoR2Content.Equipment.QuestVolatileBattery.equipmentIndex);
            }
        }

        private static bool Run_IsEquipmentAvailable(On.RoR2.Run.orig_IsEquipmentAvailable orig, Run self, EquipmentIndex equipmentIndex)
        {
            if (equipmentIndex == RoR2Content.Equipment.QuestVolatileBattery.equipmentIndex)
            {
                return true;
            }
            return orig(self, equipmentIndex);
        }

        private static bool Run_IsItemAvailable(On.RoR2.Run.orig_IsItemAvailable orig, Run self, ItemIndex itemIndex)
        {
            ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
            if (itemDef && itemDef.tier > ItemTier.VoidBoss)
            {
                return true;
            }
            return orig(self, itemIndex);
        }


        public static void FirstTimeOverrideJunk()
        {
            int skull = PickupCatalog.FindPickupIndex(JunkContent.Items.SkullCounter.itemIndex).value;
            if (PickupTransmutationManager.pickupGroupMap[skull] != null)
            {
                return;
            }
            //Debug.Log("TRUE COMMAND");
            List<List<PickupIndex>> AllItem = new List<List<PickupIndex>>();
            List<List<PickupIndex>> AllEquipment = new List<List<PickupIndex>>();


            /*Debug.Log(ItemTierCatalog.allItemTierDefs.Length);
            Debug.Log(ItemTierCatalog.itemCount);
            Debug.Log(ItemTierCatalog.itemTierDefs.Length);
            Debug.Log(ItemTierCatalog.itemTierToDef.Count);*/
            //It's like one item tier short for whatever reasons
            for (int i = 0; i < ItemTierCatalog.allItemTierDefs.Length+2; i++) 
            {
                AllItem.Add(new List<PickupIndex>());
            }
            foreach (ItemDef def in ItemCatalog.allItemDefs)
            {
                //Debug.Log(def.tier);
                AllItem[(int)def.tier].Add(PickupCatalog.itemIndexToPickupIndex[(int)def.itemIndex]);
            }
           
            List<PickupIndex> AllItem2 = new List<PickupIndex>();
            for (int i = 0; i < AllItem.Count; i++)
            {
                if (i == (int)ItemTier.Boss)
                {
                    AllItem2.AddRange(AllItem[(int)ItemTier.Lunar]);
                }
                else if (i == (int)ItemTier.Lunar)
                {
                    AllItem2.AddRange(AllItem[(int)ItemTier.Boss]);
                }
                else if(i != (int)ItemTier.NoTier)
                {
                    AllItem2.AddRange(AllItem[i]);
                }
            }
            PickupIndex[] AllOfThe22m = AllItem2.ToArray();

            /*PickupIndex[] AllOfThe22m = new PickupIndex[0];
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemTier1Group);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemTier2Group);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemTier3Group);
            AllOfThe22m = AllOfThe22m.Add(PickupCatalog.itemIndexToPickupIndex[(int)RoR2Content.Items.CaptainDefenseMatrix.itemIndex]);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemTierBossGroup);
            AllOfThe22m = AllOfThe22m.Add(PickupCatalog.itemIndexToPickupIndex[(int)RoR2Content.Items.TitanGoldDuringTP.itemIndex]);
            AllOfThe22m = AllOfThe22m.Add(PickupCatalog.itemIndexToPickupIndex[(int)RoR2Content.Items.Pearl.itemIndex]);
            AllOfThe22m = AllOfThe22m.Add(PickupCatalog.itemIndexToPickupIndex[(int)RoR2Content.Items.ShinyPearl.itemIndex]);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemTierLunarGroup);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemVoidTier1Group);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemVoidTier2Group);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemVoidTier3Group);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.itemVoidBossGroup);*/
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.equipmentNormalGroup);
            AllOfThe22m = AllOfThe22m.Add(PickupCatalog.equipmentIndexToPickupIndex[(int)RoR2Content.Equipment.QuestVolatileBattery.equipmentIndex]);
            AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.equipmentLunarGroup);
            if (PickupTransmutationManager.equipmentBossGroup.Length > 0)
            {
                AllOfThe22m = AllOfThe22m.Add(PickupTransmutationManager.equipmentBossGroup);
            }
 
            PickupTransmutationManager.pickupGroupMap[skull] = AllOfThe22m;
        }



        private static void Override_Command_Options(On.RoR2.PickupPickerController.orig_SetOptionsFromPickupForCommandArtifact_UniquePickup orig, PickupPickerController self, UniquePickup pickupIndex)
        {
            //If item and if Equipment idk should different?
            pickupIndex.pickupIndex = PickupCatalog.FindPickupIndex(JunkContent.Items.SkullCounter.itemIndex);
            orig(self, pickupIndex);
 
        }

        public class RainbowItemThing : MonoBehaviour
        {
            public Color color = Color.white;
            public Color colorOne = Color.white;
            public Color colorTwo = Color.white;
            int colorIndex = 0;
            float progress = 1000f;

            public Light TargetLight;
            public Highlight TargetHighlight;
            public ParticleSystem particleSystem;

            public bool isForUi = false;

            public void Start()
            {
                if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(Main.True_Command))
                {
                    if (!isForUi)
                    {
                        Transform transf = this.gameObject.transform.GetChild(0);
                        for (int i = 1; i < transf.childCount; i++)
                        {
                            transf.GetChild(i).gameObject.SetActive(true);
                            Light light = transf.GetChild(i).GetComponentInChildren<Light>(true);
                            if (light)
                            {
                                light.gameObject.SetActive(false);
                            }
                            if (transf.GetChild(i).childCount > 1)
                            {
                                transf.GetChild(i).GetChild(2).gameObject.SetActive(false);
                            }
                        }
                        TargetHighlight = this.GetComponent<Highlight>();
                        TargetHighlight.highlightColor = Highlight.HighlightColor.custom;
                        colorIndex = UnityEngine.Random.RandomRangeInt(0, 6);

                        TargetLight = transf.GetChild(2).GetComponentInChildren<Light>(true);
                        if (TargetLight)
                        {
                            TargetLight.gameObject.SetActive(true);
                            TargetLight.gameObject.GetComponent<LightIntensityCurve>().maxIntensity *= 10;
                        }
                        transf.GetChild(3).GetChild(2).gameObject.SetActive(true);
                        particleSystem = transf.GetChild(3).GetChild(2).GetComponent<ParticleSystem>();

                        this.GetComponent<GenericDisplayNameProvider>().SetDisplayToken("ARTIFACT_COMMAND_CUBE_RAINBOW");
                    }
                
                }
                else
                {
                    Destroy(this);
                }
            }
 
            public void FixedUpdate()
            {
                progress += 0.01f;

                if (progress >= 1)
                {
                    progress = 0;
                    switch (colorIndex)
                    {
                        case 0:
                            colorOne = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier3Item);
                            colorTwo = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Equipment);
                            break;
                        case 1:
                            colorOne = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Equipment);
                            colorTwo = ColorCatalog.GetColor(ColorCatalog.ColorIndex.BossItem);
                            break;
                        case 2:
                            colorOne = ColorCatalog.GetColor(ColorCatalog.ColorIndex.BossItem);
                            colorTwo = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2Item);
                            break;
                        case 3:
                            colorOne = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier2Item);
                            colorTwo = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem);
                            break;
                        case 4:
                            colorOne = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem);
                            colorTwo = ColorCatalog.GetColor(ColorCatalog.ColorIndex.VoidItem);
                            break;
                        case 5:
                            colorOne = ColorCatalog.GetColor(ColorCatalog.ColorIndex.VoidItem);
                            colorTwo = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Tier3Item);
                            colorIndex = -1;
                            break;
                        default:
                            colorIndex = -1;
                            break;
                    }
                    colorIndex++;
                }
                color = colorOne * (1 - progress) + colorTwo * (progress);
                if (particleSystem)
                {
                    TargetHighlight.CustomColor = color;
                    TargetLight.color = color;
                    particleSystem.startColor = color;
                }
            }
        }
    }

}

