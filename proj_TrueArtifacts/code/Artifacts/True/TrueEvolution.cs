using HG;
using R2API;
using RoR2;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace TrueArtifacts.Aritfacts
{
    public class TrueEvolution
    {
        private static GameObject UNCLONED_InventoryPrefab;

        private static Inventory monsterTeamInventory;
        private static int currentItemIterator = 0;

        public static void Start()
        {
            Run.onRunStartGlobal += OnRunStartGlobal;
            Run.onRunDestroyGlobal += OnRunDestroyGlobal;

            //Was this really needed
            UNCLONED_InventoryPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/MonsterTeamGainsItemsArtifactInventory"), "TrueEvolutionArtifactInventory", true);
            UNCLONED_InventoryPrefab.GetComponent<ArtifactEnabledResponse>().artifact = Main.True_Evolution;       
        }

        public static ItemTag[] bannedItemTags = Array.Empty<ItemTag>();

        public static void On_Artifact_Enable()
        {
            if (NetworkServer.active)
            {
                SpawnCard.onSpawnedServerGlobal += OnServerCardSpawnedGlobal;
                Stage.onServerStageBegin += OnServerStageBegin;
                SceneDirector.onPrePopulateSceneServer += OnPrePopulateSceneServer;

                bannedItemTags = new ItemTag[]
                {
                    ItemTag.AIBlacklist,
                    WConfig.TrueEvoExtraTags.Value ? ItemTag.BrotherBlacklist : ItemTag.AIBlacklist,
                    WConfig.TrueEvoExtraTags.Value ? ItemTag.OnKillEffect : ItemTag.AIBlacklist,
                    ItemTag.EquipmentRelated,
                    ItemTag.SprintRelated,
                    ItemTag.CannotCopy,
                    ItemTag.CannotSteal,
                    ItemTag.Scrap,
                    //(ItemTag)95,
                    ItemTag.HoldoutZoneRelated,
                    ItemTag.OnStageBeginEffect,
                    ItemTag.ObliterationRelated,
                    ItemTag.RebirthBlacklist,
                };


            }
            
            currentItemIterator = -1;
            AddItemsFromAllPlayers();
        }

        public static void On_Artifact_Disable()
        {
            if (NetworkServer.active)
            {
                SpawnCard.onSpawnedServerGlobal -= OnServerCardSpawnedGlobal;
                Stage.onServerStageBegin -= OnServerStageBegin;
                SceneDirector.onPrePopulateSceneServer -= OnPrePopulateSceneServer;
            }
            bannedItemTags = Array.Empty<ItemTag>();
        }


        public static void AddItemsFromAllPlayers()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            if (!Run.instance)
            {
                return;
            }
            if (!monsterTeamInventory)
            {
                Debug.Log("Trying to add to Null TrueEvo inventory");
                return;
            }      
            if (currentItemIterator != Run.instance.stageClearCount)
            {
                currentItemIterator = Run.instance.stageClearCount;
                monsterTeamInventory.itemAcquisitionOrder.Clear();
                int[] array = monsterTeamInventory.itemStacks;
                int num = 0;
                ArrayUtils.SetAll<int>(array, num);
                foreach (PlayerCharacterMasterController player in PlayerCharacterMasterController.instances)
                {
                    if (player && player.master && player.master.inventory)
                    {
                        monsterTeamInventory.AddItemsFrom(player.master.inventory, bigAssItemFilter);
                    }
                 
                }
            }
        }
        public static readonly Func<ItemIndex, bool> bigAssItemFilter = new Func<ItemIndex, bool>(PlayerToEnemyItemsFilter);

        private static bool PlayerToEnemyItemsFilter(ItemIndex itemIndex)
        {
            ItemDef def = ItemCatalog.GetItemDef(itemIndex);
            if (def == null)
            {
                return false;
            }
            foreach (ItemTag value2 in bannedItemTags)
            {
                if (Array.IndexOf<ItemTag>(def.tags, value2) != -1)
                {
                    return false;
                }
            }
            return true;
        }

 
        private static void OnServerStageBegin(Stage stage)
        {
            AddItemsFromAllPlayers();
        }
        private static void OnPrePopulateSceneServer(SceneDirector sceneDirector)
        {
            AddItemsFromAllPlayers();
        }

        private static void OnRunStartGlobal(Run run)
        {
            if (NetworkServer.active)
            {
                currentItemIterator = 0;
                monsterTeamInventory = UnityEngine.Object.Instantiate<GameObject>(UNCLONED_InventoryPrefab).GetComponent<Inventory>();
                NetworkServer.Spawn(monsterTeamInventory.gameObject);
            }
        }
        private static void OnRunDestroyGlobal(Run run)
        {
            if (monsterTeamInventory)
            {
                NetworkServer.Destroy(monsterTeamInventory.gameObject);
            }
            monsterTeamInventory = null;
        }



        private static void OnServerCardSpawnedGlobal(SpawnCard.SpawnResult spawnResult)
        {
            CharacterMaster characterMaster = spawnResult.spawnedInstance ? spawnResult.spawnedInstance.GetComponent<CharacterMaster>() : null;
            if (!characterMaster)
            {
                return;
            }
            if (characterMaster.teamIndex == TeamIndex.Player)
            {
                return;
            }
            characterMaster.inventory.AddItemsFrom(monsterTeamInventory);
        }

    }

}

