/*






public static void EliteKinArtifact(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
        {
            if (NetworkServer.active)
            {
                if (DidBrigadeHappen == false)
                {
                    DidBrigadeHappen = true;
                    normalelitetierdefs = EliteAPI.GetCombatDirectorEliteTiers();
                }
                float baseEliteCostMultiplier = 6f;
                float baseEliteDamageBoostCoefficient = 2f;
                float baseEliteHealthBoostCoefficient = 4f;
                EliteDef tempelitedef = TempForUsageEliteDef;
                //Debug.LogWarning(tempelitedef);
                if (tempelitedef == RoR2Content.Elites.Lunar)
                {
                    baseEliteHealthBoostCoefficient /= 2;
                }
                else if (EliteDefsTier2.Contains(tempelitedef))
                {
                    baseEliteCostMultiplier *= 6;
                    baseEliteDamageBoostCoefficient *= 3;
                    baseEliteHealthBoostCoefficient *= 4.5f;
                }

                CombatDirector.EliteTierDef[] array = new CombatDirector.EliteTierDef[5];
                int num = 0;
                CombatDirector.EliteTierDef eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = 1f;
                eliteTierDef.damageBoostCoefficient = 1f;
                eliteTierDef.healthBoostCoefficient = 1f;
                eliteTierDef.eliteTypes = new EliteDef[1];
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => !RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef));
                array[num] = eliteTierDef;
                int num2 = 1;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = baseEliteCostMultiplier;
                eliteTierDef.damageBoostCoefficient = baseEliteDamageBoostCoefficient;
                eliteTierDef.healthBoostCoefficient = baseEliteHealthBoostCoefficient;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                    tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => !RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef) && rules == SpawnCard.EliteRules.Default);
                array[num2] = eliteTierDef;
                int num3 = 2;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = Mathf.LerpUnclamped(1f, baseEliteCostMultiplier, 0.5f);
                eliteTierDef.damageBoostCoefficient = Mathf.LerpUnclamped(1f, baseEliteDamageBoostCoefficient, 0.5f);
                eliteTierDef.healthBoostCoefficient = Mathf.LerpUnclamped(1f, baseEliteHealthBoostCoefficient, 0.5f);
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                    tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef));
                array[num3] = eliteTierDef;
                int num4 = 3;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = baseEliteCostMultiplier;
                eliteTierDef.damageBoostCoefficient = baseEliteDamageBoostCoefficient;
                eliteTierDef.healthBoostCoefficient = baseEliteHealthBoostCoefficient;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                    tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => Run.instance.loopClearCount > 0 && rules == SpawnCard.EliteRules.Default);
                array[num4] = eliteTierDef;
                int num5 = 4;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = baseEliteCostMultiplier;
                eliteTierDef.damageBoostCoefficient = baseEliteDamageBoostCoefficient;
                eliteTierDef.healthBoostCoefficient = baseEliteHealthBoostCoefficient;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                    tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => rules == SpawnCard.EliteRules.Lunar);
                array[num5] = eliteTierDef;

                EliteAPI.OverrideCombatDirectorEliteTiers(array);

            }
            orig(self);
        }








*/
/*  float baseEliteCostMultiplier = 6f;
                float baseEliteDamageBoostCoefficient = 2f;
                float baseEliteHealthBoostCoefficient = 4f;
                
                //Debug.LogWarning(tempelitedef);
                if (tempelitedef == RoR2Content.Elites.Lunar)
                {
                    baseEliteHealthBoostCoefficient /= 2;
                }
                else if (EliteDefsTier2.Contains(tempelitedef))
                {
                    baseEliteCostMultiplier *= 6;
                    baseEliteDamageBoostCoefficient *= 3;
                    baseEliteHealthBoostCoefficient *= 4.5f;
                }

                CombatDirector.EliteTierDef[] array = new CombatDirector.EliteTierDef[5];
                int num = 0;
                CombatDirector.EliteTierDef eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = 1f;
                eliteTierDef.damageBoostCoefficient = 1f;
                eliteTierDef.healthBoostCoefficient = 1f;
                eliteTierDef.eliteTypes = new EliteDef[1];
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => !RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef));
                array[num] = eliteTierDef;
                int num2 = 1;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = baseEliteCostMultiplier;
                eliteTierDef.damageBoostCoefficient = baseEliteDamageBoostCoefficient;
                eliteTierDef.healthBoostCoefficient = baseEliteHealthBoostCoefficient;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                    tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => !RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef) && rules == SpawnCard.EliteRules.Default);
                array[num2] = eliteTierDef;
                int num3 = 2;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = Mathf.LerpUnclamped(1f, baseEliteCostMultiplier, 0.5f);
                eliteTierDef.damageBoostCoefficient = Mathf.LerpUnclamped(1f, baseEliteDamageBoostCoefficient, 0.5f);
                eliteTierDef.healthBoostCoefficient = Mathf.LerpUnclamped(1f, baseEliteHealthBoostCoefficient, 0.5f);
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                    tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef));
                array[num3] = eliteTierDef;
                int num4 = 3;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = baseEliteCostMultiplier;
                eliteTierDef.damageBoostCoefficient = baseEliteDamageBoostCoefficient;
                eliteTierDef.healthBoostCoefficient = baseEliteHealthBoostCoefficient;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                    tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => Run.instance.loopClearCount > 0 && rules == SpawnCard.EliteRules.Default);
                array[num4] = eliteTierDef;
                int num5 = 4;
                eliteTierDef = new CombatDirector.EliteTierDef();
                eliteTierDef.costMultiplier = baseEliteCostMultiplier;
                eliteTierDef.damageBoostCoefficient = baseEliteDamageBoostCoefficient;
                eliteTierDef.healthBoostCoefficient = baseEliteHealthBoostCoefficient;
                eliteTierDef.eliteTypes = new EliteDef[]
                {
                    tempelitedef
                };
                eliteTierDef.isAvailable = ((SpawnCard.EliteRules rules) => rules == SpawnCard.EliteRules.Lunar);
                array[num5] = eliteTierDef;
*/
/*
HelperSingleMixInteractable.AddCard(0, ADChest1);  //15
HelperSingleMixInteractable.AddCard(0, ADChest2);  //30
HelperSingleMixInteractable.AddCard(0, ADCategoryChestDamage);  //15
HelperSingleMixInteractable.AddCard(0, ADCategoryChestHealing);  //15
HelperSingleMixInteractable.AddCard(0, ADCategoryChestUtility);  //15
HelperSingleMixInteractable.AddCard(0, ADTripleShop);  //20
HelperSingleMixInteractable.AddCard(0, ADTripleShopLarge);  //40
HelperSingleMixInteractable.AddCard(0, ADCasinoChest);  //20
HelperSingleMixInteractable.AddCard(0, ADChest1);  //15
HelperSingleMixInteractable.AddCard(0, ADChest2);  //30
HelperSingleMixInteractable.AddCard(0, ADCategoryChestDamage);  //15
HelperSingleMixInteractable.AddCard(0, ADCategoryChestHealing);  //15
HelperSingleMixInteractable.AddCard(0, ADCategoryChestUtility);  //15
HelperSingleMixInteractable.AddCard(0, ADTripleShop);  //20
HelperSingleMixInteractable.AddCard(0, ADTripleShopLarge);  //40
HelperSingleMixInteractable.AddCard(0, ADCasinoChest);  //20
HelperSingleMixInteractable.AddCard(0, ADChest1);  //15
HelperSingleMixInteractable.AddCard(0, ADChest2);  //30
HelperSingleMixInteractable.AddCard(0, ADCategoryChestDamage);  //15
HelperSingleMixInteractable.AddCard(0, ADCategoryChestHealing);  //15
HelperSingleMixInteractable.AddCard(0, ADCategoryChestUtility);  //15
HelperSingleMixInteractable.AddCard(0, ADTripleShop);  //20
HelperSingleMixInteractable.AddCard(0, ADTripleShopLarge);  //40
HelperSingleMixInteractable.AddCard(0, ADCasinoChest);  //20
HelperSingleMixInteractable.AddCard(0, ADEquipmentBarrel);  //1
HelperSingleMixInteractable.AddCard(0, ADLunarChest);  //25
HelperSingleMixInteractable.AddCard(0, ADTripleShopEquipment);  //2
HelperSingleMixInteractable.AddCard(0, ADEquipmentBarrel);  //1
HelperSingleMixInteractable.AddCard(0, ADLunarChest);  //25
HelperSingleMixInteractable.AddCard(0, ADLunarChest);  //25
HelperSingleMixInteractable.AddCard(0, ADTripleShopEquipment);  //2
HelperSingleMixInteractable.AddCard(1, ADChest1Stealthed);  //10
HelperSingleMixInteractable.AddCard(1, ADScrapperB);  //1     
*/