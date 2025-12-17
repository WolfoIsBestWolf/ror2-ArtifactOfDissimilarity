using HG;
using RoR2;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace ArtifactDissimilarity.Aritfacts
{
    public class Kith
    {
        public static readonly System.Random random = new System.Random();

        public static SpawnCard KithNoRepeat;
        public static DirectorCardCategorySelection tempSaftey;
        //This DCCS is entirely unneeded by now but oh well
        public static DirectorCardCategorySelection dccsSingleInteractable = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static List<SpawnCard> blacklistedForRepeat = new List<SpawnCard>();

        public static void Start()
        {
            dccsSingleInteractable.name = "dccsSingleInteractable";
        }
        public static void OnArtifactEnable()
        {
            SceneDirector.onGenerateInteractableCardSelection += ModifyCards;
            if (blacklistedForRepeat.Count == 0)
            {
                blacklistedForRepeat.Add(LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscEquipmentBarrel"));
                blacklistedForRepeat.Add(LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscTripleShopEquipment"));
                blacklistedForRepeat.Add(LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscLunarChest"));
            }
            Kith.KithNoRepeat = null;
        }
        public static void OnArtifactDisable()
        {
            SceneDirector.onGenerateInteractableCardSelection -= ModifyCards;
        }



        private static void ModifyCards(SceneDirector self, DirectorCardCategorySelection dccs)
        {
            //It would be very ideal to TRimm this AFTER every normal artifact trims and removes certainl interactables,
            On.RoR2.DirectorCardCategorySelection.GenerateDirectorCardWeightedSelection += Handle_SingleInteractable;
            if (self.interactableCredit != 0)
            {
                self.interactableCredit += 15;
            }


            dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.NoMoreRadioTower));
            if (KithNoRepeat != null && blacklistedForRepeat.Contains(KithNoRepeat))
            {
                dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.Kith_DoNotRepeatLunarEquipmentOnly));
            }
            else if (Run.instance && Run.instance.stageClearCount == 0)
            {
                dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.Kith_DoNotRepeatLunarEquipmentOnly));
            }

            //For safetey so the director can always spend credits on something
            DirectorCard ADBarrel1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBarrel1"),
                selectionWeight = 1,
            };
            int barrel = dccs.AddCategory("SafeteyBarrel", 0.1f);
            dccs.AddCard(barrel, ADBarrel1);

            tempSaftey = dccs;
            dccsSingleInteractable.CopyFrom(dccs);
            if (dccsSingleInteractable.categories[0].cards.Length > 0)
            {
                Kith.KithNoRepeat = dccsSingleInteractable.categories[0].cards[0].spawnCard;
            }
        }


        private static WeightedSelection<DirectorCard> Handle_SingleInteractable(On.RoR2.DirectorCardCategorySelection.orig_GenerateDirectorCardWeightedSelection orig, DirectorCardCategorySelection self)
        {
            On.RoR2.DirectorCardCategorySelection.GenerateDirectorCardWeightedSelection -= Handle_SingleInteractable;
            if (self == tempSaftey)
            {
                Filters.SingleInteractable_Trimmer(self);
                if (WConfig.KithNoMinimumStageCompletion.Value == true)
                {
                    self.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.RemoveMinimumStageCompletionTrimmer));
                }
            }
            return orig(self);
        }
    }
}