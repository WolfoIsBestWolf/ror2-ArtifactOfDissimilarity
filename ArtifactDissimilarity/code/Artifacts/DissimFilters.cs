using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace ArtifactDissimilarity
{
    public class Filters
    {


        internal static void NewTrimmer1(ref DirectorCard[] cards, int requiredCount, ref ClassicStageInfo self)
        {
            if (cards.Length <= requiredCount)
            {
                return;
            }
            DirectorCard[] array = HG.ArrayUtils.Clone<DirectorCard>(cards);
            RoR2.Util.ShuffleArray(array);
            if (array.Length > requiredCount)
            {
                Array.Resize<DirectorCard>(ref array, requiredCount);
            }
            cards = array;
        }

        internal static void NewTrimmer2(string categoryName, int requiredCount, ref ClassicStageInfo self)
        {
            DirectorCardCategorySelection.Category[] categories = self.interactableCategories.categories;
            for (int i = 0; i < categories.Length; i++)
            {
                if (string.CompareOrdinal(categoryName, categories[i].name) == 0)
                {
                    NewTrimmer1(ref categories[i].cards, requiredCount, ref self);
                }
            }
        }

        public static void MixInteractableTrimmer3(DirectorCardCategorySelection interactableCategories)
        {
            var ADseedServer = Run.instance.stageRng.nextUlong;
            var ADrng = new Xoroshiro128Plus(ADseedServer);
            //Debug.Log(ADseedServer);

            ClassicStageInfo CSI_Dummy = new ClassicStageInfo();
            CSI_Dummy.interactableCategories = interactableCategories;
            CSI_Dummy.interactableCategories.CopyFrom(Dissimilarity.TrimmedmixInteractablesCards);
            NewTrimmer2("Chests", 6, ref CSI_Dummy);
            if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
            {
                NewTrimmer2("Barrels", 0, ref CSI_Dummy);
                NewTrimmer2("Shrines", 2, ref CSI_Dummy);
                NewTrimmer2("Drones", 0, ref CSI_Dummy);
                Dissimilarity.TrimmedmixInteractablesCards.categories[1].selectionWeight = 0;
                Dissimilarity.TrimmedmixInteractablesCards.categories[2].selectionWeight = Dissimilarity.random.Next(1, 6);
                Dissimilarity.TrimmedmixInteractablesCards.categories[3].selectionWeight = 0;
            }
            else
            {
                NewTrimmer2("Barrels", 2, ref CSI_Dummy);
                NewTrimmer2("Shrines", 3, ref CSI_Dummy);
                NewTrimmer2("Drones", 3, ref CSI_Dummy);
            }
            NewTrimmer2("Rare", 2, ref CSI_Dummy);
            if (RunArtifactManager.instance.IsArtifactEnabled(Main.Remodeling_Def))
            {
                interactableCategories.categories[6].selectionWeight /= 2;
                NewTrimmer2("Duplicator", 2, ref CSI_Dummy);
            }
            else
            {
                NewTrimmer2("Duplicator", 4, ref CSI_Dummy);
            }
        }

        public static void SingleInteractableTrimmer3(DirectorCardCategorySelection interactableCategories)
        {
            var ADseedServer = Run.instance.stageRng.nextUlong;
            var ADrng = new Xoroshiro128Plus(ADseedServer);
            //Debug.Log(ADseedServer);

            ClassicStageInfo CSI_Dummy = new ClassicStageInfo();
            CSI_Dummy.interactableCategories = interactableCategories;
            CSI_Dummy.interactableCategories.CopyFrom(Kith.TrimmedSingleInteractableType);
            NewTrimmer2("Chests", 1, ref CSI_Dummy);
            if (Run.instance.name.Equals("InfiniteTowerRun(Clone)"))
            {
                NewTrimmer2("Barrels", 0, ref CSI_Dummy);
                NewTrimmer2("Drones", 0, ref CSI_Dummy);
                NewTrimmer2("Misc", 0, ref CSI_Dummy);
            }
            else
            {
                NewTrimmer2("Barrels", 1, ref CSI_Dummy);
                NewTrimmer2("Drones", 1, ref CSI_Dummy);
                NewTrimmer2("Misc", 1, ref CSI_Dummy);
            }
            NewTrimmer2("Shrines", 1, ref CSI_Dummy);
            NewTrimmer2("Rare", 1, ref CSI_Dummy);
            NewTrimmer2("Duplicator", 1, ref CSI_Dummy);
            NewTrimmer2("Void Stuff", 1, ref CSI_Dummy);
        }

        public static bool NoMoreRadioTower(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            if (prefab.GetComponent<RoR2.RadiotowerTerminal>() || card.selectionWeight == 0)
            {
                return false;
            }
            card.selectionWeight = 1;
            return true;
        }

        public static bool CommandArtifactTrimmer(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.GetComponent<RoR2.ShopTerminalBehavior>() | prefab.GetComponent<RoR2.MultiShopController>() | prefab.GetComponent<RoR2.ScrapperController>() | prefab.GetComponent<RoR2.RouletteChestController>());
        }

        public static bool SacrificeArtifactTrimmer(DirectorCard card)
        {
            InteractableSpawnCard prefab = (InteractableSpawnCard)card.spawnCard;
            return !(prefab.skipSpawnWhenSacrificeArtifactEnabled);
        }

        public static bool SimulacrumTrimmer(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.GetComponent<RoR2.ShrineCombatBehavior>() | prefab.GetComponent<RoR2.OutsideInteractableLocker>() | prefab.GetComponent<RoR2.ShrineBossBehavior>() | prefab.GetComponent<RoR2.SeerStationController>() | prefab.GetComponent<RoR2.PortalStatueBehavior>());
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

        public static bool KithNoRepeatPredicate(DirectorCard card)
        {

            if (Kith.KithNoRepeat.name.StartsWith("iscLunarChest"))
            {
                return !(card.spawnCard.name.StartsWith("iscLunarChest"));
            }
            else if (Kith.KithNoRepeat.name.StartsWith("iscCategory"))
            {
                string temp = Kith.KithNoRepeat.name;
                return !(card.spawnCard.name.Contains(temp));
            }
            else if (Kith.KithNoRepeat.name.StartsWith("iscEquipment") || Kith.KithNoRepeat.name.EndsWith("Equipment"))
            {
                return !(card.spawnCard.name.StartsWith("iscEquipment") && !card.spawnCard.name.EndsWith("Equipment"));
            }

            //Default aka Stage1
            return !(card.spawnCard.name.StartsWith("iscEquipment") | card.spawnCard.name.EndsWith("Equipment") | card.spawnCard.name.Contains("iscLunarChest"));
        }


        //
        public static bool NoMoreScrapper(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !prefab.GetComponent<ScrapperController>();
        }

        public static bool RemodelingPredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.name.Contains("Duplicator"));
        }

        public static bool ArtifactWorldPredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.name.Contains("DuplicatorWild") | prefab.GetComponent<RoR2.OutsideInteractableLocker>() | prefab.GetComponent<RoR2.ShrineBossBehavior>() | prefab.GetComponent<RoR2.ShrineCombatBehavior>() | prefab.GetComponent<RoR2.PortalStatueBehavior>() | prefab.GetComponent<RoR2.ScrapperController>() | prefab.GetComponent<RoR2.SeerStationController>());
        }

        public static bool VoidStagePredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return !(prefab.GetComponent<RoR2.ShrineBossBehavior>() | prefab.GetComponent<RoR2.PortalStatueBehavior>() | prefab.GetComponent<RoR2.SeerStationController>());
        }

        public static bool SimulacrumPredicate(DirectorCard card)
        {
            GameObject prefab = card.spawnCard.prefab;
            return false;
        }
        //




        public static void ApplyCardRemovingFilters(DirectorCardCategorySelection DCCSInput)
        {
            //No Printers, Scrappers or Multishops whatever
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef))
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.CommandArtifactTrimmer));
                //Debug.Log("Artifact of Dissimilarity + Command");
            }
            else
            {
                if (RunArtifactManager.instance.IsArtifactEnabled(Main.RiskyConformity))
                {
                    DCCSInput.categories[6].selectionWeight = 0;
                    //Debug.Log("Artifact of Dissimilarity + RiskyConformity");
                }
                if (RunArtifactManager.instance.IsArtifactEnabled(Main.Remodeling_Def))
                {
                    DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.RemodelingPredicate));
                    //Debug.Log("Artifact of Dissimilarity + Remodeling");
                }
            }
            //No Chests
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Sacrifice))
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.SacrificeArtifactTrimmer));
                //Debug.Log("Artifact of Dissim + Sacrifice");
            }


            if (Run.instance.name.StartsWith("InfiniteTowerRun"))
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.SimulacrumTrimmer));
                //Debug.Log("Artifact of Kith + Command");
            }
            else if (SceneInfo.instance.sceneDef.baseSceneName == "artifactworld")
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.ArtifactWorldPredicate));
            }
            else if (SceneInfo.instance.sceneDef.baseSceneName == "voidstage")
            {
                DCCSInput.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.VoidStagePredicate));
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