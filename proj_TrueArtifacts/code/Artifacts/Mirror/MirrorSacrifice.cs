using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace TrueArtifacts.Aritfacts
{
    public class MirrorSacrifice
    {
        public static void Start()
        {

            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/Barrel1/Barrel1.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/ShrineBoss/ShrineBoss.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/ShrineBoss/ShrineBossSandy Variant.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/ShrineBoss/ShrineBossSnowy Variant.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/ShrineCleanse/ShrineCleanse.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/ShrineCleanse/ShrineCleanseSandy Variant.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/ShrineCleanse/ShrineCleanseSnowy Variant.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/ShrineGoldshoresAccess/ShrineGoldshoresAccess.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC2/ShrineHalcyonite.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/VoidCamp/VoidCamp.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/CU8/LemurianEgg/LemurianEgg.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();


            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/Base/RadarTower/RadarTower.prefab").WaitForCompletion().AddComponent<MirrorSacrificeAllowed>();
        }

        public static void On_Artifact_Enable()
        {
            if (NetworkServer.active)
            {
                SceneDirector.onGenerateInteractableCardSelection += OnGenerateInteractableCardSelection;
            }
        }

        public static void On_Artifact_Disable()
        {
            if (NetworkServer.active)
            {
                SceneDirector.onGenerateInteractableCardSelection -= OnGenerateInteractableCardSelection;
            }
        }

        private static void OnGenerateInteractableCardSelection(SceneDirector sceneDirector, DirectorCardCategorySelection dccs)
        {
            int stuff = dccs.FindCategoryIndexByName("Void Stuff");
            if (stuff > -1 && dccs.categories[stuff].cards.Length >= 2)
            {
                dccs.categories[stuff].selectionWeight *= 2;
                if (dccs.categories[stuff].cards[0].selectionWeight < 10)
                {
                    dccs.categories[stuff].cards[0].selectionWeight *= 3;
                }
                else if (dccs.categories[stuff].cards[1].selectionWeight < 10)
                {
                    dccs.categories[stuff].cards[1].selectionWeight *= 3;
                }
            }
            dccs.RemoveCardsThatFailFilter(new Predicate<DirectorCard>(Filter));
        }
        internal static bool Filter(DirectorCard card)
        {
            InteractableSpawnCard interactableSpawnCard = card.spawnCard as InteractableSpawnCard;
            if (interactableSpawnCard)
            {
                if (interactableSpawnCard.skipSpawnWhenSacrificeArtifactEnabled)
                {
                    return true;
                }
                if (interactableSpawnCard.skipSpawnWhenDevotionArtifactEnabled && RunArtifactManager.instance.IsArtifactEnabled(CU8Content.Artifacts.Devotion))
                {
                    return true;
                }
                GameObject prefab = card.spawnCard.prefab;
                return prefab.GetComponent<MirrorSacrificeAllowed>();
            }
            return false;
        }




    }
    public class MirrorSacrificeAllowed : MonoBehaviour
    {
    }
}

