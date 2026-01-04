
using RoR2;
using System;
using UnityEngine;

namespace TrueArtifacts.Aritfacts
{
    public class TrueSwarms
    {
        public static void On_Artifact_Enable()
        {
            //Original Swarms doesn't place them right next to each other does it
            //Multiply Team Limit ig
            SpawnCard.onSpawnedServerGlobal += OnSpawnCardOnSpawnedServerGlobal;
            //TeamCatalog.teamDefs[2].softCharacterLimit *= 2;
            //TeamCatalog.teamDefs[4].softCharacterLimit *= 2;

        }


        public static void On_Artifact_Disable()
        {
            SpawnCard.onSpawnedServerGlobal -= OnSpawnCardOnSpawnedServerGlobal;
            //TeamCatalog.teamDefs[2].softCharacterLimit /= 2;
            //TeamCatalog.teamDefs[4].softCharacterLimit /= 2;
        }

        private static bool inSpawn;
        private static void OnSpawnCardOnSpawnedServerGlobal(SpawnCard.SpawnResult result)
        {

            if (!result.success)
            {
                return;
            }
            if (RoR2.Artifacts.SwarmsArtifactManager.inSpawn)
            {
                return;
            }
            if (!inSpawn)
            {
                if (result.spawnRequest.spawnCard as CharacterSpawnCard)
                {
                    TeamIndex? teamIndexOverride = result.spawnRequest.teamIndexOverride;
                    TeamIndex teamIndex = TeamIndex.Player;
                    if (teamIndexOverride.GetValueOrDefault() == teamIndex & teamIndexOverride != null)
                    {
                        return;
                    }
                    CharacterMaster res = result.spawnedInstance.GetComponent<CharacterMaster>();
                    int distance = (int)result.spawnRequest.spawnCard.hullSize + 1;
                    result.position.x += distance;
                    result.position.z += distance;

                    inSpawn = true;
                    try
                    {
                        //Vengence Fix
                        if (result.spawnRequest.spawnCard is MasterCopySpawnCard)
                        {
                            result.spawnRequest.spawnCard = MasterCopySpawnCard.FromMaster(result.spawnedInstance.GetComponent<CharacterMaster>(), true, true, null);
                        }
                        result.spawnRequest.spawnCard.DoSpawn(result.position, result.rotation, result.spawnRequest);
                    }
                    catch (Exception message)
                    {
                        Debug.LogError(message);
                    }
                    inSpawn = false;
                }
                //UnityEngine.Object.Destroy(masterCopySpawnCard);
            }


        }

    }

}

