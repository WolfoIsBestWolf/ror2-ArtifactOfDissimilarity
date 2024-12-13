using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace ArtifactDissimilarity
{
    public class Filters
    {
        internal static void Trimmer_DesiredLength(ref DirectorCard[] cards, int requiredCount)
        {
            if (cards.Length <= requiredCount)
            {
                return;
            }
            DirectorCard[] array = HG.ArrayUtils.Clone<DirectorCard>(cards);
            Util.ShuffleArray(array);
            if (array.Length > requiredCount)
            {
                Array.Resize<DirectorCard>(ref array, requiredCount);
            }
            cards = array;
        }

        internal static void Trimmer_WithCategoryName(string categoryName, int requiredCount, ref DirectorCardCategorySelection dccs)
        {
            for (int i = 0; i < dccs.categories.Length; i++)
            {
                if (string.CompareOrdinal(categoryName, dccs.categories[i].name) == 0)
                {
                    Trimmer_DesiredLength(ref dccs.categories[i].cards, requiredCount);
                }
            }
        }

        public static void MixInteractables_Trimmer_Direct(DirectorCardCategorySelection interactableCategories)
        {
            Trimmer_DesiredLength(ref interactableCategories.categories[0].cards, 6);
            if (Run.instance is InfiniteTowerRun)
            {
                Trimmer_DesiredLength(ref interactableCategories.categories[1].cards, 0);
                Trimmer_DesiredLength(ref interactableCategories.categories[2].cards, 2);
                Trimmer_DesiredLength(ref interactableCategories.categories[3].cards, 0);
                Dissimilarity.TrimmedmixInteractablesCards.categories[1].selectionWeight = 0;
                Dissimilarity.TrimmedmixInteractablesCards.categories[2].selectionWeight = Dissimilarity.random.Next(1, 6);
                Dissimilarity.TrimmedmixInteractablesCards.categories[3].selectionWeight = 0;
            }
            else
            {
                Trimmer_DesiredLength(ref interactableCategories.categories[1].cards, 2);
                Trimmer_DesiredLength(ref interactableCategories.categories[2].cards, 3);
                Trimmer_DesiredLength(ref interactableCategories.categories[3].cards, 3);
            }
            Trimmer_DesiredLength(ref interactableCategories.categories[5].cards, 3);
            if (RunArtifactManager.instance.IsArtifactEnabled(Main.Remodeling_Def))
            {
                interactableCategories.categories[6].selectionWeight /= 2;
                Trimmer_DesiredLength(ref interactableCategories.categories[6].cards, 2);
            }
            else
            {
                Trimmer_DesiredLength(ref interactableCategories.categories[6].cards, 4);
            }

            Debug.Log("Artifact of Dissimilarity: Generated Trimmed mixInteractables selection");
            if (WConfig.DebugPrint.Value == true)
            {
                string debugString = "-----------------\n";
                debugString += "Artifact of Dissimilarity: Trimmed Interactable List\n";
                for (int i = 0; i < interactableCategories.categories.Length; i++)
                {
                    ref DirectorCardCategorySelection.Category ptr = ref interactableCategories.categories[i];
                    for (int j = ptr.cards.Length - 1; j >= 0; j--)
                    {
                        debugString += ptr.cards[j].spawnCard.prefab.name;
                        debugString += "\n";
                    }
                }
                debugString += "-----------------";
                Debug.Log(debugString);
            };
        }

        public static void MixInteractableTrimmerWithNames(DirectorCardCategorySelection interactableCategories)
        {
            Trimmer_WithCategoryName("Chests", 6, ref interactableCategories);
            if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
            {
                Trimmer_WithCategoryName("Barrels", 0, ref interactableCategories);
                Trimmer_WithCategoryName("Shrines", 2, ref interactableCategories);
                Trimmer_WithCategoryName("Drones", 0, ref interactableCategories);
                Dissimilarity.TrimmedmixInteractablesCards.categories[1].selectionWeight = 0;
                Dissimilarity.TrimmedmixInteractablesCards.categories[2].selectionWeight = Dissimilarity.random.Next(1, 6);
                Dissimilarity.TrimmedmixInteractablesCards.categories[3].selectionWeight = 0;
            }
            else
            {
                Trimmer_WithCategoryName("Barrels", 2, ref interactableCategories);
                Trimmer_WithCategoryName("Shrines", 3, ref interactableCategories);
                Trimmer_WithCategoryName("Drones", 3, ref interactableCategories);
            }
            Trimmer_WithCategoryName("Rare", 2, ref interactableCategories);
            if (RunArtifactManager.instance.IsArtifactEnabled(Main.Remodeling_Def))
            {
                interactableCategories.categories[6].selectionWeight /= 2;
                Trimmer_WithCategoryName("Duplicator", 2, ref interactableCategories);
            }
            else
            {
                Trimmer_WithCategoryName("Duplicator", 4, ref interactableCategories);
            }
        }


        public static void SingleInteractable_Trimmer(DirectorCardCategorySelection interactableCategories)
        {
            DirectorCardCategorySelection.Category[] categories = interactableCategories.categories;
            for (int i = 0; i < categories.Length; i++)
            {
                Trimmer_DesiredLength(ref categories[i].cards, 1);
            }

            Debug.Log("Artifact of Kith: Generated Trimmed SingleInteractable selection");
            if (WConfig.DebugPrint.Value == true)
            {
                string debugString = "-----------------\n";
                debugString += "Artifact of Kith: Trimmed Interactable List\n";
                for (int i = 0; i < interactableCategories.categories.Length - 1; i++)
                {
                    ref DirectorCardCategorySelection.Category ptr = ref interactableCategories.categories[i];
                    for (int j = ptr.cards.Length - 1; j >= 0; j--)
                    {
                        debugString += ptr.cards[j].spawnCard.prefab.name;
                        debugString += "\n";
                    }
                }
                debugString += "-----------------";
                Debug.Log(debugString);
            };
        }

        public static bool NoMoreRadioTower(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            if (prefab.GetComponent<RadiotowerTerminal>() || card.selectionWeight == 0)
            {
                return false;
            }
            card.selectionWeight = 1;
            return true;
        }



        public static bool SacrificeArtifactTrimmer(DirectorCard card)
        {
            InteractableSpawnCard prefab = (InteractableSpawnCard)card.spawnCard;
            return !(prefab.skipSpawnWhenSacrificeArtifactEnabled);
        }

        public static bool SimulacrumTrimmer(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.GetComponent<ShrineCombatBehavior>() | prefab.GetComponent<OutsideInteractableLocker>());
        }

        public static bool TestingPrintCardResults(DirectorCard card)
        {
            Debug.Log(card.spawnCard);
            return true;
        }

        public static bool RemoveMinimumStageCompletionTrimmer(DirectorCard card)
        {
            //Debug.LogWarning(card.minimumStageCompletions);
            if (card.minimumStageCompletions <= 3)
            {
                card.minimumStageCompletions = 0;
            }
            else if (card.minimumStageCompletions >= 4)
            {
                card.minimumStageCompletions = 1;
            }
            return true;
        }

        public static bool Kith_DoNotRepeatLunarEquipmentOnly(DirectorCard card)
        {
            return !Kith.blacklistedForRepeat.Contains(card.spawnCard);
        }


        //
        public static bool NoMoreScrapper(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !prefab.GetComponent<ScrapperController>();
        }

        public static bool NoMorePrinters(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.name.StartsWith("Duplicator"));
        }

        public static bool ArtifactWorldPredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.name.Equals("DuplicatorWild") | prefab.GetComponent<OutsideInteractableLocker>() | prefab.GetComponent<ShrineCombatBehavior>() | prefab.GetComponent<ScrapperController>());
        }

        public static bool RemoveInteractablesThatNeedTeleporter(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.GetComponent<ShrineBossBehavior>() | prefab.GetComponent<PortalStatueBehavior>() | prefab.GetComponent<SeerStationController>());
        }




        public static void Mix_ApplyCardRemovingFilters(DirectorCardCategorySelection DCCSInput)
        {
            if (Run.instance && Run.instance.name.StartsWith("InfiniteTowerRun"))
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.RemoveInteractablesThatNeedTeleporter));
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.SimulacrumTrimmer));
                //Debug.Log("Artifact of Kith + Command");
            }
            else if (!SceneInfo.instance)
            {
                return;
            }
            else if (SceneInfo.instance.sceneDef.baseSceneName == "artifactworld")
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.RemoveInteractablesThatNeedTeleporter));
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.ArtifactWorldPredicate));
            }
            else if (SceneInfo.instance.sceneDef.baseSceneName == "voidstage")
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.RemoveInteractablesThatNeedTeleporter));
            }

        }





        public static void ApplySandSnow(DirectorCardCategorySelection DCCSInput)
        {
            if (SceneInfo.instance.sceneDef.baseSceneName == "snowyforest" || SceneInfo.instance.sceneDef.baseSceneName == "frozenwall" || SceneInfo.instance.sceneDef.baseSceneName == "itfrozenwall")
            {
                foreach (DirectorCard directorCard in DCCSInput.categories[2].cards)
                {
                    switch (directorCard.spawnCard.name)
                    {
                        case "iscShrineBlood":
                        case "iscShrineBloodSnowy":
                        case "iscShrineBloodSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBloodSnowy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineBoss":
                        case "iscShrineBossSnowy":
                        case "iscShrineBossSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBossSnowy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineChance":
                        case "iscShrineChanceSnowy":
                        case "iscShrineChanceSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChanceSnowy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineCleanse":
                        case "iscShrineCleanseSnowy":
                        case "iscShrineCleanseSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanseSnowy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineCombat":
                        case "iscShrineCombatSnowy":
                        case "iscShrineCombatSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombatSnowy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineRestack":
                        case "iscShrineRestackSnowy":
                        case "iscShrineRestackSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestackSnowy.asset").WaitForCompletion(); ;
                            break;
                    }
                }
            }
            else if (SceneInfo.instance.sceneDef.baseSceneName == "goolake" || SceneInfo.instance.sceneDef.baseSceneName == "itgoolake")
            {
                foreach (DirectorCard directorCard in DCCSInput.categories[2].cards)
                {
                    switch (directorCard.spawnCard.name)
                    {
                        case "iscShrineBlood":
                        case "iscShrineBloodSnowy":
                        case "iscShrineBloodSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBloodSandy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineBoss":
                        case "iscShrineBossSnowy":
                        case "iscShrineBossSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBossSandy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineChance":
                        case "iscShrineChanceSnowy":
                        case "iscShrineChanceSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChanceSandy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineCleanse":
                        case "iscShrineCleanseSnowy":
                        case "iscShrineCleanseSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanseSandy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineCombat":
                        case "iscShrineCombatSnowy":
                        case "iscShrineCombatSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombatSandy.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineRestack":
                        case "iscShrineRestackSnowy":
                        case "iscShrineRestackSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestackSandy.asset").WaitForCompletion(); ;
                            break;
                    }
                }
            }
            else
            {
                foreach (DirectorCard directorCard in DCCSInput.categories[2].cards)
                {
                    switch (directorCard.spawnCard.name)
                    {
                        case "iscShrineBlood":
                        case "iscShrineBloodSnowy":
                        case "iscShrineBloodSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBlood.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineBoss":
                        case "iscShrineBossSnowy":
                        case "iscShrineBossSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineChance":
                        case "iscShrineChanceSnowy":
                        case "iscShrineChanceSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChance.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineCleanse":
                        case "iscShrineCleanseSnowy":
                        case "iscShrineCleanseSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanse.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineCombat":
                        case "iscShrineCombatSnowy":
                        case "iscShrineCombatSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombat.asset").WaitForCompletion(); ;
                            break;
                        case "iscShrineRestack":
                        case "iscShrineRestackSnowy":
                        case "iscShrineRestackSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestack.asset").WaitForCompletion(); ;
                            break;
                    }
                }
            }
        }

    }
}