using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using WolfoLibrary;

namespace ArtifactDissimilarity.Aritfacts
{
    public class Flamboyance
    {
        public static FlamboyanceDropTable dtAnyTier = ScriptableObject.CreateInstance<FlamboyanceDropTable>();
 
        public static void Start()
        {
            dtAnyTier.name = "dtFlamboyanceAnyTier";
            dtAnyTier.canDropBeReplaced = false;
            dtAnyTier.tier1Weight = 1f;
            dtAnyTier.tier2Weight = 1f;
            dtAnyTier.tier3Weight = 1f;
            dtAnyTier.lunarItemWeight = 1f;
            dtAnyTier.bossWeight = 1.75f;
            dtAnyTier.voidTier1Weight = 1.75f;
            dtAnyTier.voidTier2Weight = 1.75f;
            dtAnyTier.voidTier3Weight = 3f;
            dtAnyTier.voidBossWeight = 3f;
            dtAnyTier.full_equipmentWeight = 0.6f;
            dtAnyTier.full_equipmentWeightLunar = 0.6f;
            dtAnyTier.full_equipmentWeightBoss = 0.6f;
            dtAnyTier.foodTierWeight = 3f;
        }
        public static void On_Artifact_Disable()
        {
            On.RoR2.PickupDropTable.GenerateDrop -= OLD_PickupDropTable_GenerateDrop;
            On.RoR2.PickupDropTable.GenerateUniqueDrops -= OLD_PickupDropTable_GenerateUniqueDrops;
            On.RoR2.PickupDropTable.GeneratePickup -= PickupDropTable_GeneratePickup;
            On.RoR2.PickupDropTable.GenerateDistinctPickups -= PickupDropTable_GenerateDistinctPickups;
        }
        public static void On_Artifact_Enable()
        {
            dtAnyTier.didWeights = false;
            On.RoR2.PickupDropTable.GenerateDrop += OLD_PickupDropTable_GenerateDrop;
            On.RoR2.PickupDropTable.GenerateUniqueDrops += OLD_PickupDropTable_GenerateUniqueDrops;
            On.RoR2.PickupDropTable.GeneratePickup += PickupDropTable_GeneratePickup;
            On.RoR2.PickupDropTable.GenerateDistinctPickups += PickupDropTable_GenerateDistinctPickups;
        }

        private static void PickupDropTable_GenerateDistinctPickups(On.RoR2.PickupDropTable.orig_GenerateDistinctPickups orig, PickupDropTable self, System.Collections.Generic.List<UniquePickup> dest, int desiredCount, Xoroshiro128Plus rng, bool allowLoop)
        {
            orig(self, dest, desiredCount, rng, allowLoop);
            bool replaced = false;
            for (int i = 0; i < dest.Count; i++)
            { 
                if (rng.nextNormalizedFloat < FlamboyanceDropTable.replacementChance)
                {
                    if (!replaced)
                    {
                        replaced = true;
                        MatchTagsToDropTable(self);
                    }
                    dest[i] = dtAnyTier.GeneratePickup(rng);
                }
            }
        }

        private static UniquePickup PickupDropTable_GeneratePickup(On.RoR2.PickupDropTable.orig_GeneratePickup orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            if (Roll(self, rng))
            {
                MatchTagsToDropTable(self); 
                if (WConfig.DebugPrint.Value)
                {
                    var a = orig(dtAnyTier, rng);
                    Debug.LogWarning("Flamboyance Replacing for " + self.name + " | " + a.pickupIndex);
                    return a;
                }
                return orig(dtAnyTier, rng);
            }
            return orig(self, rng);
        }

        public static void MatchTagsToDropTable(PickupDropTable self)
        {
            bool regenerate = false;
            ItemTag[] lenghtPreB = dtAnyTier.bannedItemTags;
            ItemTag[] lenghtPreR = dtAnyTier.requiredItemTags;

            if (self is BasicPickupDropTable)
            {
                dtAnyTier.bannedItemTags = (self as BasicPickupDropTable).bannedItemTags;
                dtAnyTier.requiredItemTags = (self as BasicPickupDropTable).requiredItemTags;
            }
            else if (self is ArenaMonsterItemDropTable)
            {
                dtAnyTier.bannedItemTags = (self as ArenaMonsterItemDropTable).bannedItemTags;
                dtAnyTier.requiredItemTags = (self as ArenaMonsterItemDropTable).requiredItemTags;
            }
            else
            {
                dtAnyTier.bannedItemTags = Array.Empty<ItemTag>();
                dtAnyTier.requiredItemTags = Array.Empty<ItemTag>();
            }

            if (lenghtPreR.Length != dtAnyTier.requiredItemTags.Length)
            {
                regenerate = true;
            }
            else if (lenghtPreB.Length != dtAnyTier.bannedItemTags.Length)
            {
                regenerate = true;
            }
            else if (dtAnyTier.bannedItemTags.Length > 0 || dtAnyTier.requiredItemTags.Length > 0)
            {
                if (!Enumerable.SequenceEqual(lenghtPreR, dtAnyTier.requiredItemTags))
                {
                    regenerate = true;
                }
                else if (!Enumerable.SequenceEqual(lenghtPreB, dtAnyTier.bannedItemTags))
                {
                    regenerate = true;
                }
            }
            if (regenerate)
            {
                dtAnyTier.Regenerate(Run.instance);
            }
        }

 
        public static bool Roll(PickupDropTable dt, Xoroshiro128Plus rng)
        {
            if (WConfig.DebugPrint.Value)
            {
                Debug.Log("Flamboyance Rolling for " + dt.name);
            }
            if (dt is BasicPickupDropTable) //Needs to be the lowest
            {
                if ((dt as BasicPickupDropTable).requiredItemTags.Contains(ItemTag.CanBeTemporary))
                {
                    return rng.nextNormalizedFloat < FlamboyanceDropTable.replacementChance;
                }
                else if (!dt.canDropBeReplaced)
                {
                    return false;
                }
                else if ((dt as BasicPickupDropTable).bannedItemTags.Contains(ItemTag.AIBlacklist))
                {
                    return rng.nextNormalizedFloat < FlamboyanceDropTable.replacementChanceMonster;
                }
                else if ((dt as BasicPickupDropTable).bannedItemTags.Contains(ItemTag.CannotDuplicate))
                {
                    return rng.nextNormalizedFloat < FlamboyanceDropTable.replacementChancePrinter;
                }
                else if (BazaarController.instance)
                {
                    return rng.nextNormalizedFloat < FlamboyanceDropTable.replacementChancePrinter;
                }
                return rng.nextNormalizedFloat < FlamboyanceDropTable.replacementChance;
            }
            if (!dt.canDropBeReplaced)
            {
                return false;
            }
            return rng.nextNormalizedFloat < FlamboyanceDropTable.replacementChance;
        }


        private static PickupIndex[] OLD_PickupDropTable_GenerateUniqueDrops(On.RoR2.PickupDropTable.orig_GenerateUniqueDrops orig, PickupDropTable self, int maxDrops, Xoroshiro128Plus rng)
        {
            if (Roll(self,rng))
            {
                
                MatchTagsToDropTable(self);
                return orig(dtAnyTier, maxDrops, rng);
            }
            return orig(self,maxDrops,rng);
        }

        private static PickupIndex OLD_PickupDropTable_GenerateDrop(On.RoR2.PickupDropTable.orig_GenerateDrop orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            if (Roll(self, rng))
            {
                MatchTagsToDropTable(self);
                
                return orig(dtAnyTier, rng);
            }
            return orig(self,rng);
        }
    }

    public class FlamboyanceDropTable : BasicPickupDropTable
    {
        
        public float bossEquipmentWeight;
        public float full_equipmentWeight;
        public float full_equipmentWeightLunar;
        public float full_equipmentWeightBoss;
        public bool didWeights = false;

        public static float replacementChance = 0.20f; //1 in 5
        public static float replacementChanceMonster = 0.20f; //1 in 5
        public static float replacementChanceTemp = 0.20f; //1 in 5
        public static float replacementChancePrinter = 0.0715f; //1 in 14

        public void DoWeights(Run run)
        {
            if (!run)
            {
                run = Run.instance;
            }

            run.availableBossDropList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex));
            run.availableBossDropList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex));
            if (run.IsItemAvailable(RoR2Content.Items.TitanGoldDuringTP.itemIndex))
            {
                run.availableBossDropList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Items.TitanGoldDuringTP.itemIndex));
            }
            run.availableEquipmentDropList.Add(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.QuestVolatileBattery.equipmentIndex));
            run.availableFoodTierDropList.Clear();
            for (int i = 0; ItemCatalog.allItemDefs.Length > i; i++)
            {
                if (ItemCatalog.allItemDefs[i].tier == ItemTier.FoodTier)
                {
                    //Debug.Log(ItemCatalog.allItemDefs[i] + " " + run.IsItemAvailable(ItemCatalog.allItemDefs[i].itemIndex));
                    if (run.IsItemAvailable((ItemIndex)i))
                    {
                        run.availableFoodTierDropList.Add(PickupCatalog.FindPickupIndex((ItemIndex)i));
                    }
                }
            }
            availableEliteList = new List<PickupIndex>();
            foreach (PickupIndex pickup in PickupTransmutationManager.equipmentBossGroup)
            {
                if (!run.IsEquipmentExpansionLocked(pickup.equipmentIndex))
                {
                    availableEliteList.Add(pickup);
                }
            }
 
            didWeights = true;
            tier1Weight = 100f / run.availableTier1DropList.Count;
            tier2Weight = 100f / run.availableTier2DropList.Count;
            tier3Weight = 100f / run.availableTier3DropList.Count;
            lunarItemWeight = 100f / Math.Max(15, run.availableLunarItemDropList.Count);
            bossWeight = 100f / Math.Max(15, run.availableBossDropList.Count);


            voidTier1Weight = 100f / Math.Max(15,run.availableVoidTier1DropList.Count + run.availableVoidTier2DropList.Count + run.availableVoidTier3DropList.Count + run.availableVoidBossDropList.Count);
            voidTier2Weight = voidTier1Weight;
            voidTier3Weight = voidTier1Weight;
            voidBossWeight = voidTier1Weight;
            foodTierWeight = 100f / Math.Max(15, run.availableFoodTierDropList.Count);

            full_equipmentWeight = 100f / run.availableEquipmentDropList.Count;
            full_equipmentWeightLunar = Math.Max(15, run.availableLunarEquipmentDropList.Count);
            full_equipmentWeightBoss = Math.Max(15, availableEliteList.Count);

           
        }

        public List<PickupIndex> availableEliteList;


        public override void Regenerate(Run run)
        {
            if (!didWeights)
            {
                DoWeights(run);
            }
            if (bannedItemTags.Length == 0 && requiredItemTags.Length == 0)
            {
                this.equipmentWeight = full_equipmentWeight;
                this.lunarEquipmentWeight = full_equipmentWeightLunar;
                this.bossEquipmentWeight = full_equipmentWeightBoss;
            }
            else
            {
                this.equipmentWeight = 0;
                this.lunarEquipmentWeight = 0;
                this.bossEquipmentWeight = 0;
            }
            GenerateWeightedSelection(run);
            Add(availableEliteList, bossEquipmentWeight);
        }


    }


}

