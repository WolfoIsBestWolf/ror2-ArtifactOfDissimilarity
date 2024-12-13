using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace TrueArtifacts
{  
    public class TrueDissonance
    {
        private static CombatDirector.EliteTierDef[] eliteTiersBackup;
        public static DirectorCardCategorySelection dccsMixEnemyTRUE;

        public static void On_Artifact_Enable()
        {
            if (dccsMixEnemyTRUE == null)
            {
                MakeDCCS();
            }
            On.RoR2.ClassicStageInfo.RebuildCards += ClassicStageInfo_RebuildCards;
            On.EntityStates.VoidInfestor.Infest.OnEnter += Infest_OnEnter;
            if (ClassicStageInfo.instance && Run.instance)
            {
                ClassicStageInfo.instance.RebuildCards(null, null);
            }
            eliteTiersBackup = CombatDirector.eliteTiers;
            //DoEliteShenanigans();
        }
        public static void DoEliteShenanigans()
        {
            CombatDirector.EliteTierDef[] NewEliteTiers = new CombatDirector.EliteTierDef[CombatDirector.eliteTiers.Length];
            CombatDirector.eliteTiers.CopyTo(NewEliteTiers, 0);

            CombatDirector.EliteTierDef Tier1ForLunar = new CombatDirector.EliteTierDef();
            CombatDirector.EliteTierDef Tier2ForLunar = new CombatDirector.EliteTierDef();
            CombatDirector.EliteTierDef LunarVoidNormal = new CombatDirector.EliteTierDef();

            Tier1ForLunar.isAvailable = (SpawnCard.EliteRules rules) => rules == SpawnCard.EliteRules.Lunar && Run.instance.stageClearCount >= 6;
            Tier2ForLunar.isAvailable = (SpawnCard.EliteRules rules) => rules == SpawnCard.EliteRules.Lunar && Run.instance.stageClearCount >= 6;
            LunarVoidNormal.isAvailable = (SpawnCard.EliteRules rules) => rules == SpawnCard.EliteRules.Default && Run.instance.stageClearCount >= 5;
            LunarVoidNormal.eliteTypes = new EliteDef[]
            {
                RoR2Content.Elites.Lunar,
                DLC1Content.Elites.Void
            };

            foreach (CombatDirector.EliteTierDef tier in CombatDirector.eliteTiers)
            {
                if (tier.eliteTypes.Length >= 5)
                {
                    foreach (EliteDef elite in tier.eliteTypes)
                    {
                        if (elite == DLC2Content.Elites.Aurelionite)
                        {
                            Tier1ForLunar.eliteTypes = tier.eliteTypes;
                            Tier1ForLunar.costMultiplier = tier.costMultiplier;
                            LunarVoidNormal.costMultiplier = tier.costMultiplier;
                        }
                    }
                }
                else if (tier.eliteTypes != null && tier.eliteTypes[0] == RoR2Content.Elites.Poison)
                {
                    Tier2ForLunar.eliteTypes = tier.eliteTypes;
                    Tier2ForLunar.costMultiplier = tier.costMultiplier;
                }
            }

            CombatDirector.eliteTiers =  NewEliteTiers.Add(Tier1ForLunar, Tier2ForLunar, LunarVoidNormal);
        }

       
        

        private static void Infest_OnEnter(On.EntityStates.VoidInfestor.Infest.orig_OnEnter orig, EntityStates.VoidInfestor.Infest self)
        {
            orig(self);
            if (self.attack.teamIndex == TeamIndex.Monster)
            {
                self.attack.teamIndex = TeamIndex.Void;
            }
        }

        public static void On_Artifact_Disable()
        {
            On.RoR2.ClassicStageInfo.RebuildCards -= ClassicStageInfo_RebuildCards;
            On.EntityStates.VoidInfestor.Infest.OnEnter -= Infest_OnEnter;
            if (ClassicStageInfo.instance && Run.instance)
            {
                ClassicStageInfo.instance.RebuildCards(null, null);
            }
            CombatDirector.eliteTiers = eliteTiersBackup;
        }

        public static void MakeDCCS()
        {
            Debug.Log("dccsMixEnemyTRUE");
            //Could instantiate it and add whatever the hell we want if needed
            //Would make some sense to add AWU and TitanGold and Twisted Scavs and all that
            //Or alternatively just have 1 copy from the start instead of adding it every time
            //Make sure to load it later than other mods that edit it then ig
            dccsMixEnemyTRUE = Object.Instantiate(RoR2Content.mixEnemyMonsterCards);
            dccsMixEnemyTRUE.name = "dccsMixEnemyTRUE";
            //DO THING HERE;
            int minimum = 3;

            DirectorCard cscBrother = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Brother/cscBrother.asset").WaitForCompletion(),
                selectionWeight = 1,
                preventOverhead = true,
                minimumStageCompletions = minimum,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard cscMiniVoidRaidCrabPhase1 = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/VoidRaidCrab/cscMiniVoidRaidCrabPhase1.asset").WaitForCompletion(),
                selectionWeight = 1,
                preventOverhead = true,
                minimumStageCompletions = minimum,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard cscFalseSonBoss = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC2/FalseSonBoss/cscFalseSonBoss.asset").WaitForCompletion(),
                selectionWeight = 1,
                preventOverhead = true,
                minimumStageCompletions = minimum,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard cscScavLunar = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/ScavLunar/cscScavLunar.asset").WaitForCompletion(),
                selectionWeight = 4,
                preventOverhead = true,
                minimumStageCompletions = minimum,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard cscSuperRoboBallBoss = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/RoboBallBoss/cscSuperRoboBallBoss.asset").WaitForCompletion(),
                selectionWeight = 2,
                preventOverhead = true,
                minimumStageCompletions = minimum,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard cscTitanGold = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/Base/Titan/cscTitanGold.asset").WaitForCompletion(),
                selectionWeight = 2,
                preventOverhead = true,
                minimumStageCompletions = minimum,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard cscGeepBody = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/Gup/cscGeepBody.asset").WaitForCompletion(),
                selectionWeight = 1,
                preventOverhead = true,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            DirectorCard cscGipBody = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/Gup/cscGipBody.asset").WaitForCompletion(),
                selectionWeight = 1,
                preventOverhead = true,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            Addressables.LoadAssetAsync<GameObject>(key: "RoR2/DLC1/EliteVoid/VoidInfestorBody.prefab").WaitForCompletion().GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.Mechanical;
            DirectorCard cscVoidInfestor = new DirectorCard
            {
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/EliteVoid/cscVoidInfestor.asset").WaitForCompletion(),
                selectionWeight = 1,
                preventOverhead = true,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };

            int cat = dccsMixEnemyTRUE.AddCategory("UniqueBosses", 0.3f);
            dccsMixEnemyTRUE.AddCard(cat, cscBrother); //Has Adaptive
            dccsMixEnemyTRUE.AddCard(cat, cscMiniVoidRaidCrabPhase1); //Has Adaptive
            dccsMixEnemyTRUE.AddCard(cat, cscFalseSonBoss); //?
            dccsMixEnemyTRUE.AddCard(cat, cscScavLunar);
            dccsMixEnemyTRUE.AddCard(cat, cscSuperRoboBallBoss);
            dccsMixEnemyTRUE.AddCard(cat, cscTitanGold);
            dccsMixEnemyTRUE.AddCard(1, cscGeepBody);
            dccsMixEnemyTRUE.AddCard(2, cscGipBody);
            dccsMixEnemyTRUE.AddCard(2, cscVoidInfestor);

            //Adaptive can be up to 5x hp iirc


            cscVoidInfestor.spawnCard.directorCreditCost = 40; //1000 hp is like nothing bro
            cscBrother.spawnCard.directorCreditCost = 1600; //1000 hp is like nothing bro
            cscMiniVoidRaidCrabPhase1.spawnCard.directorCreditCost = 3200;
            cscFalseSonBoss.spawnCard.directorCreditCost = 2400;
            cscScavLunar.spawnCard.directorCreditCost = 3000;
            ((CharacterSpawnCard)cscScavLunar.spawnCard).noElites = true;
            cscSuperRoboBallBoss.spawnCard.directorCreditCost = 960;
            cscTitanGold.spawnCard.directorCreditCost = 720;

        }

        private static void ClassicStageInfo_RebuildCards(On.RoR2.ClassicStageInfo.orig_RebuildCards orig, RoR2.ClassicStageInfo self, RoR2.DirectorCardCategorySelection forcedMonsterCategory, RoR2.DirectorCardCategorySelection forcedInteractableCategory)
        {
            if (forcedMonsterCategory == null)
            {
                forcedMonsterCategory = dccsMixEnemyTRUE;
            }
            orig(self, forcedMonsterCategory, forcedInteractableCategory);
        }



        public class DirectorCard2 : DirectorCard
        {
            public new float selectionWeight;
        }
     }

}

