using ArtifactDissimilarity.Aritfacts;
using RoR2;
using RoR2.ExpansionManagement;
using RoR2.ContentManagement;
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
            //0 Chest
            //1 Barrel
            //2 Shrine
            //3 Drones
            //4 MiscDLC
            //5 Rare
            //6 Duplicate
            var rng = Run.instance.stageRngGenerator;
            Trimmer_DesiredLength(ref interactableCategories.categories[0].cards, 6); //Stages have like 8 stages at least usually
            if (Run.instance is InfiniteTowerRun)
            {
                Trimmer_DesiredLength(ref interactableCategories.categories[1].cards, 0);
                Trimmer_DesiredLength(ref interactableCategories.categories[2].cards, rng.RangeInt(2, 4));
                Trimmer_DesiredLength(ref interactableCategories.categories[3].cards, rng.RangeInt(2, 4));
                Dissimilarity.TrimmedmixInteractablesCards.categories[1].selectionWeight = 0;
                Dissimilarity.TrimmedmixInteractablesCards.categories[2].selectionWeight = UnityEngine.Random.RandomRangeInt(1, 6);
                Dissimilarity.TrimmedmixInteractablesCards.categories[3].selectionWeight = 0;
            }
            else
            {
                Trimmer_DesiredLength(ref interactableCategories.categories[1].cards, 2);
                Trimmer_DesiredLength(ref interactableCategories.categories[2].cards, rng.RangeInt(2, 5)); //Stages have 2 - 4 Shrines
                Trimmer_DesiredLength(ref interactableCategories.categories[3].cards, rng.RangeInt(2, 6)); //Stages have 2 - 6 Drones
            }
            Trimmer_DesiredLength(ref interactableCategories.categories[4].cards, 1); //DLC Stuff
            Trimmer_DesiredLength(ref interactableCategories.categories[5].cards, 2); //Rare Junk
            if (RunArtifactManager.instance.IsArtifactEnabled(Defs.Remodeling_Def))
            {
                interactableCategories.categories[6].selectionWeight /= 2;
                Trimmer_DesiredLength(ref interactableCategories.categories[6].cards, 3);
            }
            else
            {
                Trimmer_DesiredLength(ref interactableCategories.categories[6].cards, 5);
            }

            Debug.Log("Artifact of Dissimilarity: Generated Trimmed mixInteractables selection");
            if (WConfig.DebugPrint.Value == true)
            {
                string debugString = "-----------------\n";
                debugString += "Artifact of Dissimilarity: Trimmed Interactable List\n\n";
                for (int i = 0; i < interactableCategories.categories.Length; i++)
                {
                    ref DirectorCardCategorySelection.Category ptr = ref interactableCategories.categories[i];
                    debugString += $"--{ptr.name} wt:{ptr.selectionWeight}--\n";
                    for (int j = ptr.cards.Length - 1; j >= 0; j--)
                    {
                        debugString += ptr.cards[j].GetSpawnCard().prefab.name;
                        debugString += "\n";
                    }
                }
                debugString += "-----------------";
                Debug.Log(debugString);
            }

        }



        public static void SingleInteractable_Trimmer(DirectorCardCategorySelection interactableCategories)
        {
            DirectorCardCategorySelection.Category[] categories = interactableCategories.categories;
            for (int i = 0; i < categories.Length; i++)
            {
                Trimmer_DesiredLength(ref categories[i].cards, 1);
            }
 
            if (WConfig.DebugPrint.Value == true)
            {
                string debugString = "-----------------\n";
                debugString += "Artifact of Kith: Trimmed Interactable List\n";
                for (int i = 0; i < interactableCategories.categories.Length - 1; i++)
                {
                    ref DirectorCardCategorySelection.Category ptr = ref interactableCategories.categories[i];
                    for (int j = ptr.cards.Length - 1; j >= 0; j--)
                    {
                        debugString += ptr.cards[j].GetSpawnCard().prefab.name;
                        debugString += "\n";
                    }
                }
                debugString += "-----------------";
                Debug.Log(debugString);
            };
        }

        public static bool FilterOutUnavailableCards(DirectorCard card)
        {
            if (card.selectionWeight == 0)
            {
                return false;
            }
            if (!card.IsAvailable())
            {
                return false;
            }
            return true;
        }
        public static bool FilterOutDLCRequirement(DirectorCard card)
        {
            SpawnCard spawnCard = card.GetSpawnCard();
            if (Run.instance && spawnCard && spawnCard.prefab)
            {
                ExpansionRequirementComponent component = spawnCard.prefab.GetComponent<ExpansionRequirementComponent>();
 
                bool dlcEnabled = !component || Run.instance.IsExpansionEnabled(component.requiredExpansion);
                bool dlc2Enabled = true;
                IDirectorAvailability[] components = card.spawnCard.prefab.GetComponents<IDirectorAvailability>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (!components[i].IsAvailable())
                    {
                        dlc2Enabled = false;
                        break;
                    }
                }
                return dlcEnabled && dlc2Enabled;
            }
            return true;
        }

        public static bool SimulacrumTrimmer(DirectorCard card)
        {
            GameObject prefab = card.GetSpawnCard().prefab;
            return !(prefab.GetComponent<ShrineCombatBehavior>() | prefab.GetComponent<OutsideInteractableLocker>());
        }

        public static bool TestingPrintCardResults(DirectorCard card)
        {
            Debug.Log(card.GetSpawnCard());
            return true;
        }

      
        public static bool Kith_DoNotRepeatLunarEquipmentOnly(DirectorCard card)
        {
            return !Kith.blacklistedForRepeat.Contains(card.GetSpawnCard());
        }
        public static bool RemoveUnavailable(DirectorCard card)
        {
            return !card.IsAvailable();
        }
 
        public static bool NoMorePrinters(DirectorCard card)
        {
            GameObject prefab = card.GetSpawnCard().prefab;
            return !(prefab.name.StartsWith("Duplicator"));
        }

        public static bool ArtifactWorldPredicate(DirectorCard card)
        {
            GameObject prefab = card.GetSpawnCard().prefab;
            return !(prefab.GetComponent<OutsideInteractableLocker>() || prefab.GetComponent<ShrineCombatBehavior>() || prefab.GetComponent<ScrapperController>());
        }

        public static bool RemoveInteractablesThatNeedTeleporter(DirectorCard card)
        {
            GameObject prefab = card.GetSpawnCard().prefab;
            return !(prefab.GetComponent<ShrineBossBehavior>() || prefab.GetComponent<PortalStatueBehavior>() || prefab.GetComponent<SeerStationController>());
        }




        public static void Mix_ApplyCardRemovingFilters(DirectorCardCategorySelection DCCSInput)
        {
            DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(FilterOutUnavailableCards));
            if (Run.instance is InfiniteTowerRun)
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(RemoveInteractablesThatNeedTeleporter));
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(SimulacrumTrimmer));
            }
            else if (!SceneInfo.instance)
            {
                return;
            }
            else if (SceneInfo.instance.sceneDef.baseSceneName.StartsWith("artifactworld"))
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(RemoveInteractablesThatNeedTeleporter));
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(ArtifactWorldPredicate));
            }
            else if (SceneInfo.instance.sceneDef.baseSceneName == "voidstage")
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(RemoveInteractablesThatNeedTeleporter));
            }

        }





        public static void ApplySandSnow(DirectorCardCategorySelection DCCSInput)
        {
            string scene = SceneInfo.instance.sceneDef.baseSceneName;
            if (scene == "snowyforest" ||
                scene == "frozenwall" ||
                scene == "itfrozenwall" ||
                scene == "nest"
                )
            {
                foreach (DirectorCard directorCard in DCCSInput.categories[2].cards)
                {
                    switch (directorCard.GetSpawnCard().name)
                    {
                        case "iscShrineBlood":
                        case "iscShrineBloodSnowy":
                        case "iscShrineBloodSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBloodSnowy.asset").WaitForCompletion();
                            break;
                        case "iscShrineBoss":
                        case "iscShrineBossSnowy":
                        case "iscShrineBossSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBossSnowy.asset").WaitForCompletion();
                            break;
                        case "iscShrineChance":
                        case "iscShrineChanceSnowy":
                        case "iscShrineChanceSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChanceSnowy.asset").WaitForCompletion();
                            break;
                        case "iscShrineCleanse":
                        case "iscShrineCleanseSnowy":
                        case "iscShrineCleanseSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanseSnowy.asset").WaitForCompletion();
                            break;
                        case "iscShrineCombat":
                        case "iscShrineCombatSnowy":
                        case "iscShrineCombatSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombatSnowy.asset").WaitForCompletion();
                            break;
                        case "iscShrineRestack":
                        case "iscShrineRestackSnowy":
                        case "iscShrineRestackSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestackSnowy.asset").WaitForCompletion();
                            break;
                    }
                }
            }
            else if (scene == "goolake" ||
                    scene == "itgoolake" ||
                    scene == "ironalluvium" ||
                    scene == "ironalluvium2" ||
                    scene == "repurposedcrater"
                    )
            {
                foreach (DirectorCard directorCard in DCCSInput.categories[2].cards)
                {
                    switch (directorCard.GetSpawnCard().name)
                    {
                        case "iscShrineBlood":
                        case "iscShrineBloodSnowy":
                        case "iscShrineBloodSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBloodSandy.asset").WaitForCompletion();
                            break;
                        case "iscShrineBoss":
                        case "iscShrineBossSnowy":
                        case "iscShrineBossSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBossSandy.asset").WaitForCompletion();
                            break;
                        case "iscShrineChance":
                        case "iscShrineChanceSnowy":
                        case "iscShrineChanceSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChanceSandy.asset").WaitForCompletion();
                            break;
                        case "iscShrineCleanse":
                        case "iscShrineCleanseSnowy":
                        case "iscShrineCleanseSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanseSandy.asset").WaitForCompletion();
                            break;
                        case "iscShrineCombat":
                        case "iscShrineCombatSnowy":
                        case "iscShrineCombatSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombatSandy.asset").WaitForCompletion();
                            break;
                        case "iscShrineRestack":
                        case "iscShrineRestackSnowy":
                        case "iscShrineRestackSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestackSandy.asset").WaitForCompletion();
                            break;
                    }
                }
            }
            else
            {
                foreach (DirectorCard directorCard in DCCSInput.categories[2].cards)
                {
                    switch (directorCard.GetSpawnCard().name)
                    {
                        case "iscShrineBlood":
                        case "iscShrineBloodSnowy":
                        case "iscShrineBloodSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBlood/iscShrineBlood.asset").WaitForCompletion();
                            break;
                        case "iscShrineBoss":
                        case "iscShrineBossSnowy":
                        case "iscShrineBossSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion();
                            break;
                        case "iscShrineChance":
                        case "iscShrineChanceSnowy":
                        case "iscShrineChanceSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineChance/iscShrineChance.asset").WaitForCompletion();
                            break;
                        case "iscShrineCleanse":
                        case "iscShrineCleanseSnowy":
                        case "iscShrineCleanseSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCleanse/iscShrineCleanse.asset").WaitForCompletion();
                            break;
                        case "iscShrineCombat":
                        case "iscShrineCombatSnowy":
                        case "iscShrineCombatSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineCombat/iscShrineCombat.asset").WaitForCompletion();
                            break;
                        case "iscShrineRestack":
                        case "iscShrineRestackSnowy":
                        case "iscShrineRestackSandy":
                            directorCard.spawnCard = Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/ShrineRestack/iscShrineRestack.asset").WaitForCompletion();
                            break;
                    }
                }
            }
        }

    }
}