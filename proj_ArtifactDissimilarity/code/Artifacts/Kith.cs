using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ArtifactDissimilarity.Aritfacts
{
    public class Kith
    {
        public static readonly System.Random random = new System.Random();

        public static SpawnCard KithNoRepeatChest;
        public static List<SpawnCard> blacklistedForRepeat = new List<SpawnCard>();

        public static DirectorCardCategorySelection dccsTemp;
 

        public static DirectorCardCategorySelection dccsSingleInteractable = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
      

        public static void Start()
        {
            dccsSingleInteractable.name = "dccsSingleInteractable";
        }
        public static void OnArtifactEnable()
        {
            On.RoR2.DirectorCardCategorySelection.GenerateDirectorCardWeightedSelection += Handle_SingleInteractable;
            SceneDirector.onGenerateInteractableCardSelection += ModifyCards;
            if (blacklistedForRepeat.Count == 0)
            {
                blacklistedForRepeat.Add(Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/EquipmentBarrel/iscEquipmentBarrel.asset").WaitForCompletion());
                blacklistedForRepeat.Add(Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/TripleShopEquipment/iscTripleShopEquipment.asset").WaitForCompletion());
                blacklistedForRepeat.Add(Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/Base/LunarChest/iscLunarChest.asset").WaitForCompletion());
                blacklistedForRepeat.Add(Addressables.LoadAssetAsync<SpawnCard>(key: "RoR2/DLC3/TemporaryItemsDistributor/iscTemporaryItemsShop.asset").WaitForCompletion());
            }
            KithNoRepeatChest = blacklistedForRepeat[0];
        }
        public static void OnArtifactDisable()
        {
            SceneDirector.onGenerateInteractableCardSelection -= ModifyCards;
            On.RoR2.DirectorCardCategorySelection.GenerateDirectorCardWeightedSelection -= Handle_SingleInteractable;
        }



        private static void ModifyCards(SceneDirector self, DirectorCardCategorySelection dccs)
        {
 
            dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.FilterOutUnavailableCards));
            if (KithNoRepeatChest != null && blacklistedForRepeat.Contains(KithNoRepeatChest) ||
                Run.instance && Run.instance.stageClearCount == 0)
            {
                dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filters.Kith_DoNotRepeatLunarEquipmentOnly));
            }

            if (WConfig.KithNoMinimumStageCompletion.Value)
            {
                for (int i = 0; i < dccs.categories.Length; i++)
                {
                    ref DirectorCardCategorySelection.Category ptr = ref dccs.categories[i];
                    for (int j = ptr.cards.Length - 1; j >= 0; j--)
                    {
                        DirectorCard card = ptr.cards[j];
                        if (card.minimumStageCompletions > 1)
                        {
                            card.minimumStageCompletions--;
                        }
                    }
                }
            }
           
            //For safetey so the director can always spend credits on something
            DirectorCard ADBarrel1 = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBarrel1"),
                selectionWeight = 1,
            };
            int barrel = dccs.AddCategory("SafeteyBarrel", 0.1f);
            dccs.AddCard(barrel, ADBarrel1);

            dccsTemp = dccs;
            dccsSingleInteractable.CopyFrom(dccs);
 
            int chest = dccsSingleInteractable.FindCategoryIndexByName("Chests");
            if (chest != -1 && dccsSingleInteractable.categories[chest].cards.Length > 0)
            {
                KithNoRepeatChest = dccsSingleInteractable.categories[chest].cards[0].spawnCard;
            }
        }


        private static WeightedSelection<DirectorCard> Handle_SingleInteractable(On.RoR2.DirectorCardCategorySelection.orig_GenerateDirectorCardWeightedSelection orig, DirectorCardCategorySelection self)
        {
            if (self == dccsTemp)
            { 
                Filters.SingleInteractable_Trimmer(self);
            }
            return orig(self);
        }
    }
}