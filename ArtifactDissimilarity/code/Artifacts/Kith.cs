using RoR2;
using System;
using UnityEngine;


namespace ArtifactDissimilarity
{
    public class Kith
    {
        public static readonly System.Random random = new System.Random();

        public static bool KithAdded = false;

        public static SpawnCard KithNoRepeat;

        public static DirectorCardCategorySelection TrimmedSingleInteractableType = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
        public static DirectorCardCategorySelection HelperSingleMixInteractable = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();

        public static void Start()
        {
            HelperSingleMixInteractable.name = "dcccHelperSingleMixInteractable";
            TrimmedSingleInteractableType.name = "dccsSingleInteractable";
        }

        public static WeightedSelection<DirectorCard> SingleInteractableApplier(On.RoR2.SceneDirector.orig_GenerateInteractableCardSelection orig, RoR2.SceneDirector self)
        {
            if (self.interactableCredit != 0)
            {
                self.interactableCredit += 21;
            }
            WeightedSelection<DirectorCard> KithDirectorCards = new WeightedSelection<DirectorCard>();
            if (ClassicStageInfo.instance != null)
            {
                //Debug.Log("Artifact of Kith: SingleInteractableApplier");
                if (RunArtifactManager.instance.IsArtifactEnabled(Main.Kith_Def))
                {
                    if (RunArtifactManager.instance.IsArtifactEnabled(Main.Dissimilarity_Def))
                    {
                        TrimmedSingleInteractableType.Clear();
                        TrimmedSingleInteractableType.CopyFrom(Kith.HelperSingleMixInteractable);

                        if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
                        {
                            TrimmedSingleInteractableType.categories[1].selectionWeight = 0;
                            TrimmedSingleInteractableType.categories[2].selectionWeight = random.Next(1, 5);
                            TrimmedSingleInteractableType.categories[3].selectionWeight = 0;
                        }
                        else
                        {
                            TrimmedSingleInteractableType.categories[1].selectionWeight = random.Next(9, 11);
                            TrimmedSingleInteractableType.categories[2].selectionWeight = random.Next(8, 13);
                            TrimmedSingleInteractableType.categories[3].selectionWeight = random.Next(11, 15);
                            TrimmedSingleInteractableType.categories[6].selectionWeight = random.Next(6, 10);
                        }

                        Filters.ApplyCardRemovingFilters(TrimmedSingleInteractableType);
                        Filters.ApplySandSnow(TrimmedSingleInteractableType);

                        Debug.Log("Artifact of Kith: SingleInteractable using mixInteractables");
                    }
                    else //Main Kith 
                    {
                        TrimmedSingleInteractableType.Clear();
                        TrimmedSingleInteractableType.CopyFrom(ClassicStageInfo.instance.interactableCategories);

                        TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.NoMoreRadioTower));
                        Filters.ApplyCardRemovingFilters(TrimmedSingleInteractableType);
                        if (WConfig.KithNoMinimumStageCompletion.Value == true)
                        {
                            TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.RemoveMinimumStageCompletionTrimmer));
                        }
                    }
                   

                    if (Kith.KithNoRepeat == null)
                    {
                        //Debug.LogWarning("Kith limiter null");
                        Kith.KithNoRepeat = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscSquidTurret");
                    }
                    if (Kith.KithNoRepeat.name.StartsWith("iscEquipment") || Kith.KithNoRepeat.name.StartsWith("iscLunarChest") || Kith.KithNoRepeat.name.StartsWith("iscCategory") || Kith.KithNoRepeat.name.Equals("iscSquidTurret"))
                    {
                        //Debug.Log("Kith limiter " + Kith.KithNoRepeat.name);
                        TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.KithNoRepeatPredicate));
                    }


                    Filters.SingleInteractableTrimmer3(TrimmedSingleInteractableType);

                    DirectorCard ADBarrel1 = new DirectorCard
                    {
                        spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBarrel1"),
                        selectionWeight = 1,
                    };
                    if (TrimmedSingleInteractableType.categories.Length > 5)
                    {
                        TrimmedSingleInteractableType.categories[4].selectionWeight *= 1.5f;
                        TrimmedSingleInteractableType.AddCard(6, ADBarrel1);
                    }
                    else
                    {
                        TrimmedSingleInteractableType.AddCard(0, ADBarrel1);
                    }


                    KithDirectorCards = TrimmedSingleInteractableType.GenerateDirectorCardWeightedSelection();
                    Debug.Log("Artifact of Kith: Generated Trimmed SingleInteractable selection");

                    Kith.KithNoRepeat = KithDirectorCards.choices[0].value.spawnCard;

                    if (WConfig.DebugPrint.Value == true)
                    {
                        Debug.Log("__________________________________________________");
                        Debug.Log("Artifact of Kith: Trimmed Interactable List");
                        Debug.Log("");
                        TrimmedSingleInteractableType.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.TestingPrintCardResults));
                        Debug.Log("__________________________________________________");
                    };

                }
                else
                {
                    //KithDirectorCards = ClassicStageInfo.instance.interactableCategories.GenerateDirectorCardWeightedSelection(); //fallback
                    Debug.LogWarning("Artifact of Kith: Failed to generate normal Interactable Categories, using fallback");
                    return orig(self);
                };
            }
            return KithDirectorCards;
        }


    }
}