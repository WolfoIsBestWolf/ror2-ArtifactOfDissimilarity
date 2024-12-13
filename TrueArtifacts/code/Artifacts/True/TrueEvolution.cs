using HG;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace TrueArtifacts
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

            UNCLONED_InventoryPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/MonsterTeamGainsItemsArtifactInventory"), "TrueEvolutionArtifactInventory", true);
            UNCLONED_InventoryPrefab.GetComponent<ArtifactEnabledResponse>().artifact = Main.True_Evolution;       
        }

        public static void On_Artifact_Enable()
        {
            if (NetworkServer.active)
            {
                SpawnCard.onSpawnedServerGlobal += OnServerCardSpawnedGlobal;
                Stage.onServerStageBegin += OnServerStageBegin;
                SceneDirector.onPrePopulateSceneServer += OnPrePopulateSceneServer;
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
        }


        public static void AddItemsFromAllPlayers()
        {
            if (!Run.instance)
            {
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
                    monsterTeamInventory.AddItemsFrom(player.master.inventory, bigAssItemFilter);
                }
            }
        }
        public static readonly Func<ItemIndex, bool> bigAssItemFilter = new Func<ItemIndex, bool>(PlayerToEnemyItemsFilter);

        private static bool PlayerToEnemyItemsFilter(ItemIndex itemIndex)
        {
            ItemDef def = ItemCatalog.GetItemDef(itemIndex);
            if (def.tier == ItemTier.NoTier)
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.AIBlacklist))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.OnKillEffect))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.CannotCopy))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.CannotSteal))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.BrotherBlacklist))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.EquipmentRelated))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.SprintRelated))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.Scrap))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.OnStageBeginEffect))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.HoldoutZoneRelated))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.InteractableRelated))
            {
                return false;
            }
            else if (def.ContainsTag(ItemTag.ObliterationRelated))
            {
                return false;
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

