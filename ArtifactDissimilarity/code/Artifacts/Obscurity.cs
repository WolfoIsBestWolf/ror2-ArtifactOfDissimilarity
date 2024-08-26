using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ArtifactDissimilarity
{
    public class Obscurity
    {
        public static Sprite questionMark_White;  
        public static Sprite questionMark_Red;
        public static Sprite questionMark_Orange;
        public static Sprite questionMark_Yellow;
        public static Sprite questionMark_Green;
        public static Sprite questionMark_Blue;
        public static Sprite questionMark_Pink;


        public static void Setup()
        {
            On.RoR2.ShopTerminalBehavior.PreStartClient += ShopTerminalBehavior_PreStartClient;
            On.RoR2.PickupDisplay.SetPickupIndex += PickupDisplay_SetPickupIndex;
            On.RoR2.UI.ItemInventoryDisplay.UpdateDisplay += RemoveItemDisplay;
            On.RoR2.UI.ItemInventoryDisplay.AllocateIcons += ItemInventoryDisplay_AllocateIcons;
            On.RoR2.UI.ItemIcon.SetItemIndex += ItemIcon_SetItemIndex;


            On.RoR2.ItemDisplay.SetVisibilityLevel += ItemDisplay_SetVisibilityLevel;

            On.RoR2.UI.PickupPickerPanel.OnCreateButton += PickupPickerPanel_OnCreateButton;
            //Maybe instead of 0 Items shown, show question mark icons with the stack number, harder to do

            //PickupPicker Pannel Questionmarks ig ig

            //PickupCatalog.GetHiddenPickupDisplayPrefab();
        }

        private static void ItemDisplay_SetVisibilityLevel(On.RoR2.ItemDisplay.orig_SetVisibilityLevel orig, ItemDisplay self, VisibilityLevel newVisibilityLevel)
        {
            orig(self, VisibilityLevel.Invisible);
        }

        private static void PickupPickerPanel_OnCreateButton(On.RoR2.UI.PickupPickerPanel.orig_OnCreateButton orig, RoR2.UI.PickupPickerPanel self, int index, RoR2.UI.MPButton button)
        {
            throw new NotImplementedException();
        }

        private static void ItemIcon_SetItemIndex(On.RoR2.UI.ItemIcon.orig_SetItemIndex orig, RoR2.UI.ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            //Is this Equipment Icon too??
            orig(self, newItemIndex, newItemCount);

            ItemDef itemDef = ItemCatalog.GetItemDef(newItemIndex);

            switch(itemDef.tier)
            {
                case ItemTier.Tier1:
                    break;
                case ItemTier.Tier2:
                    break;
                case ItemTier.Tier3:
                    break;
                case ItemTier.Boss:
                    break;
                case ItemTier.Lunar:
                    break;
                case ItemTier.VoidTier1:
                case ItemTier.VoidTier2:
                case ItemTier.VoidTier3:
                case ItemTier.VoidBoss:
                    break;
            }


        }

        private static void ItemInventoryDisplay_AllocateIcons(On.RoR2.UI.ItemInventoryDisplay.orig_AllocateIcons orig, RoR2.UI.ItemInventoryDisplay self, int desiredItemCount)
        {
            //If Inventory
            //If Tab Inventory
            //NOT Enemy Inventory
            //NOT Run End Inventory

            orig(self, 0);
        }

        private static void RemoveItemDisplay(On.RoR2.UI.ItemInventoryDisplay.orig_UpdateDisplay orig, RoR2.UI.ItemInventoryDisplay self)
        {
            orig(self);
        }

        private static void PickupDisplay_SetPickupIndex(On.RoR2.PickupDisplay.orig_SetPickupIndex orig, PickupDisplay self, PickupIndex newPickupIndex, bool newHidden)
        {
            orig(self, newPickupIndex, true);
        }

        private static void ShopTerminalBehavior_PreStartClient(On.RoR2.ShopTerminalBehavior.orig_PreStartClient orig, ShopTerminalBehavior self)
        {
            orig(self);
            self.hidden = true;
            self.Networkhidden = true;
        }
    }
}