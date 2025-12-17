/*
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace TrueArtifacts.Aritfacts
{
    public class MirrorEnigma
    {
        public static MirrorEnigmaDropTable dtAnyTier = ScriptableObject.CreateInstance<MirrorEnigmaDropTable>();
 
        public static void Start()
        {
            dtAnyTier.name = "dtMirrorEnigmaAnyTier";
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
            On.RoR2.PickupDropTable.GenerateDrop -= PickupDropTable_GenerateDrop;
            On.RoR2.PickupDropTable.GenerateUniqueDrops -= PickupDropTable_GenerateUniqueDrops;
            On.RoR2.PickupDropTable.GeneratePickup -= PickupDropTable_GeneratePickup;
            On.RoR2.PickupDropTable.GenerateDistinctPickups -= PickupDropTable_GenerateDistinctPickups;
        }
        public static void On_Artifact_Enable()
        {

            On.RoR2.PickupDropTable.GenerateDrop += PickupDropTable_GenerateDrop;
            On.RoR2.PickupDropTable.GenerateUniqueDrops += PickupDropTable_GenerateUniqueDrops;
            On.RoR2.PickupDropTable.GeneratePickup += PickupDropTable_GeneratePickup;
            On.RoR2.PickupDropTable.GenerateDistinctPickups += PickupDropTable_GenerateDistinctPickups;
        }

        private static void PickupDropTable_GenerateDistinctPickups(On.RoR2.PickupDropTable.orig_GenerateDistinctPickups orig, PickupDropTable self, System.Collections.Generic.List<UniquePickup> dest, int desiredCount, Xoroshiro128Plus rng, bool allowLoop)
        {
            if (Roll(self, rng))
            {
                MatchTagsToDropTable(self);
                orig(dtAnyTier, dest, desiredCount, rng, allowLoop);
                return;
            }
            orig(self, dest, desiredCount, rng, allowLoop);
        }

        private static UniquePickup PickupDropTable_GeneratePickup(On.RoR2.PickupDropTable.orig_GeneratePickup orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            if (Roll(self, rng))
            {
                MatchTagsToDropTable(self);
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
                dtAnyTier.bannedItemTags = new ItemTag[0];
                dtAnyTier.requiredItemTags = new ItemTag[0];
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
            int chance = MirrorEnigmaDropTable.replacementChance;
            if (dt is BasicPickupDropTable) //Needs to be the lowest
            {
                if (!dt.canDropBeReplaced && !(dt as BasicPickupDropTable).requiredItemTags.Contains(ItemTag.CanBeTemporary))
                {
                    return false;
                }
                 
                if ((dt as BasicPickupDropTable).bannedItemTags.Contains(ItemTag.AIBlacklist))
                {
                    chance = MirrorEnigmaDropTable.replacementChanceMonster;
                }
                else if ((dt as BasicPickupDropTable).requiredItemTags.Contains(ItemTag.CanBeTemporary))
                {
                    chance = MirrorEnigmaDropTable.replacementChanceMonster;
                }
                else if (dt.name.StartsWith("dtDupli"))
                {
                    chance = MirrorEnigmaDropTable.replacementChancePrinter;
                }
                else if (SceneInfo.instance && SceneInfo.instance.sceneDef.baseSceneName == "bazaar")
                {
                    chance = MirrorEnigmaDropTable.replacementChancePrinter;
                }
            }
            else if (!dt.canDropBeReplaced)
            {
                return false;
            }
            return rng.RangeInt(0, 100) < chance;
        }


        private static PickupIndex[] PickupDropTable_GenerateUniqueDrops(On.RoR2.PickupDropTable.orig_GenerateUniqueDrops orig, PickupDropTable self, int maxDrops, Xoroshiro128Plus rng)
        {
            if (Roll(self,rng))
            {
                MatchTagsToDropTable(self);
                return orig(dtAnyTier, maxDrops, rng);
            }
            return orig(self,maxDrops,rng);
        }

        private static PickupIndex PickupDropTable_GenerateDrop(On.RoR2.PickupDropTable.orig_GenerateDrop orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            if (Roll(self, rng))
            {
                MatchTagsToDropTable(self);
                return orig(dtAnyTier, rng);
            }
            return orig(self,rng);
        }
    }

    public class MirrorEnigmaDropTable : BasicPickupDropTable
    {
        public float bossEquipmentWeight;
        public float full_equipmentWeight;
        public float full_equipmentWeightLunar;
        public float full_equipmentWeightBoss;
        public bool didWeights = true;
        public static int replacementChance = 10;
        public static int replacementChanceMonster = 12;
        public static int replacementChancePrinter = 5;

        public void DoWeights(Run run)
        {
            if (!run)
            {
                run = Run.instance;
            }
            didWeights = true;
            tier1Weight = 100f / run.availableTier1DropList.Count;
            tier2Weight = 100f / run.availableTier2DropList.Count;
            tier3Weight = 80f / run.availableTier3DropList.Count;
            lunarItemWeight = 80f / run.availableLunarItemDropList.Count;
            bossWeight = 80f / run.availableBossDropList.Count;
            voidTier1Weight = 60f / run.availableVoidTier1DropList.Count;
            voidTier2Weight = 30f / run.availableVoidTier2DropList.Count;
            voidTier3Weight = 20f / run.availableVoidTier3DropList.Count;
            voidBossWeight = 10f / run.availableVoidBossDropList.Count;
            foodTierWeight = 100f / 5;

            full_equipmentWeight = 50f / run.availableEquipmentDropList.Count;
            full_equipmentWeightLunar = 50f / run.availableLunarEquipmentDropList.Count;
            full_equipmentWeightBoss = 50f / PickupTransmutationManager.equipmentBossGroup.Length;
          
        }
    

        public override void Regenerate(Run run)
        {
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
            this.selector.AddChoice(new UniquePickup(PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex)), bossWeight);
            this.selector.AddChoice(new UniquePickup(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex)), bossWeight);
            this.selector.AddChoice(new UniquePickup(PickupCatalog.FindPickupIndex(RoR2Content.Items.TitanGoldDuringTP.itemIndex)), bossWeight);
            
            if (bossEquipmentWeight > 0)
            {
                foreach (PickupIndex pickup in PickupTransmutationManager.equipmentBossGroup)
                {
                    if (!run.IsEquipmentExpansionLocked(pickup.equipmentIndex))
                    {
                        this.selector.AddChoice(new UniquePickup(pickup), bossEquipmentWeight);
                    }
                }
            }
           
            for (int i = 0; ItemCatalog.allItemDefs.Length < i; i++)
            {
                if (ItemCatalog.allItemDefs[i].tier == ItemTier.FoodTier)
                {
                    if (!run.IsItemExpansionLocked(ItemCatalog.allItemDefs[i].itemIndex))
                    {
                        this.selector.AddChoice(new UniquePickup(PickupCatalog.FindPickupIndex(ItemCatalog.allItemDefs[i].itemIndex)), foodTierWeight);
                    }
                }
            }

        }

      
    }


}

*/