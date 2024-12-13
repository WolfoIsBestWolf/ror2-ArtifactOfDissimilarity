using UnityEngine.Networking;
using RoR2;
using RoR2.Artifacts;
using R2API;

namespace TrueArtifacts
{
    public class MirrorGlass
    {
        //Make sure it only affects players, unlike normal glass
        //I guess can't just, double all items because what if they're from printers? Can we check that somehow
        //Maybe just disable those interactables from working entirely

        //We could double the cost, and just make it not duplicate scrappers yeeah that'd be cool.
        public static void On_Artifact_Disable()
        {
            RecalculateStatsAPI.GetStatCoefficients -= R2API_Stats;
            On.RoR2.CharacterBody.RecalculateStats -= CharacterBody_RecalculateStats;
            On.RoR2.PickupDropletController.OnCollisionEnter -= DoubleItems;
            On.RoR2.PurchaseInteraction.Start -= DoubleItemPrices;
        }

        public static void On_Artifact_Enable()
        {
            RecalculateStatsAPI.GetStatCoefficients += R2API_Stats;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.PickupDropletController.OnCollisionEnter += DoubleItems;
            On.RoR2.PurchaseInteraction.Start += DoubleItemPrices;
        }

        private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.isPlayerControlled)
            {
                self.isGlass = true;
            }
        }

        private static void R2API_Stats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isPlayerControlled)
            {
                args.baseCurseAdd += 1;
            }
        }

        private static void DoubleItems(On.RoR2.PickupDropletController.orig_OnCollisionEnter orig, PickupDropletController self, UnityEngine.Collision collision)
        {
            if (NetworkServer.active && self.alive)
            {
                ItemIndex item = self.pickupIndex.pickupDef.itemIndex;
                if (item != ItemIndex.None)
                {
                    ItemDef def = ItemCatalog.GetItemDef(item);
                    if (def.name.StartsWith("Scrap"))
                    {
                        orig(self, collision);
                        return;
                    }
                }
                self.createPickupInfo.position = self.transform.position;
                self.CreatePickup();
                orig(self, collision);
            }
        }

        private static void DoubleItemPrices(On.RoR2.PurchaseInteraction.orig_Start orig, PurchaseInteraction self)
        {
            orig(self);
            if (self.costType == CostTypeIndex.WhiteItem ||
                self.costType == CostTypeIndex.GreenItem ||
                self.costType == CostTypeIndex.RedItem ||
                self.costType == CostTypeIndex.BossItem)
            {
                self.cost *= 2;
            }
        }
    }

}

