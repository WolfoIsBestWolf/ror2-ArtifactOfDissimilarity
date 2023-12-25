/*public static void ShopTerminalBehavior_Start(On.RoR2.ShopTerminalBehavior.orig_Start orig, ShopTerminalBehavior self)
{
    orig(self);
    self.SetPickupIndex(GetNewPickupIndex(self.pickupIndex), false);
}*/

/*public static void OverwriteDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet_CreatePickupInfo_Vector3_Vector3 orig, GenericPickupController.CreatePickupInfo pickupInfo, Vector3 position, Vector3 velocity)
{
    //Like every Starstorm item that drops things for some reason doesn't get overwritten by DropTable
    pickupInfo.pickupIndex = GetNewPickupIndex(pickupInfo.pickupIndex);

    orig(pickupInfo, position, velocity);
}*/


/*
On.RoR2.ArtifactTrialMissionController.Awake += (orig, self) =>
{
    if (self.NetworkcurrentArtifactIndex == -1)
    {
        //int newArtifactIndex = random.Next(ArtifactCatalog.artifactCount);
        var temp = ArtifactCatalog.GetArtifactDef((ArtifactIndex)random.Next(ArtifactCatalog.artifactCount));
        Debug.LogWarning(temp);
        self.SetFieldValue<ArtifactDef>("trialArtifact", temp);
    }
    orig(self);
};
*/

/*
Debug.Log("Loading Artifact of Retooling");
Retooling.nameToken = "Artifact of Retooling";
Retooling.descriptionToken = "Reroll your equipments each stage.";
Retooling.smallIconSelectedSprite = RetoolingOn;
Retooling.smallIconDeselectedSprite = RetoolingOff;
Retooling.pickupModelPrefab = LegacyResourcesAPI.Load<ArtifactDef>("artifactdefs/Enigma").pickupModelPrefab;
ContentAddition.AddArtifactDef(Retooling);


if (EnableRetoolingArtifact.Value == false)
{
    Retooling.unlockableDef = NoMoreArtifact;
}
else
{
    RetoolingCode.ArtifactCompounds = new List<int> { 7, 1, 1, 5, 1, 5, 3, 7, 7 };
    ArtifactCodeAPI.AddCode(Retooling, RetoolingCode);
}
*/
/*public static IEnumerator DelayedChatMessageNonGlobal(string chatMessage, NetworkConnection networkConnection)
{
    yield return new WaitForSeconds(1f);
    Chat.SimpleChatMessage simpleChatMessage = new Chat.SimpleChatMessage
    {
        baseToken = chatMessage
    };

    NetworkWriter networkWriter = new NetworkWriter();
    networkWriter.StartMessage(59);
    networkWriter.Write(simpleChatMessage.GetTypeIndex());
    networkWriter.Write(simpleChatMessage);
    networkWriter.FinishMessage();
    if (networkConnection == null)
    {
        yield break;
    }
    networkConnection.SendWriter(networkWriter, RoR2.Networking.QosChannelIndex.chat.intVal);
    yield break;
}*/
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