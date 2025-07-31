using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace ArtifactDissimilarity
{
    public class Obscurity
    {
        public static Sprite BlindT1;
        public static Sprite BlindT2;
        public static Sprite BlindT3;
        public static Sprite BlindBoss;
        public static Sprite BlindLunar;
        public static Sprite BlindVoid;
        public static Sprite BlindEquipment;
        public static Sprite BlindMod;
         
        public static void Start()
        {
            Texture2D texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Blind/BlindT1.png");
            texture.wrapMode = TextureWrapMode.Clamp;
            BlindT1 = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

            texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Blind/BlindT2.png");
            texture.wrapMode = TextureWrapMode.Clamp;
            BlindT2 = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

            texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Blind/BlindT3.png");
            texture.wrapMode = TextureWrapMode.Clamp;
            BlindT3 = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

            texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Blind/BlindBoss.png");
            texture.wrapMode = TextureWrapMode.Clamp;
            BlindBoss = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

            texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Blind/BlindLunar.png");
            texture.wrapMode = TextureWrapMode.Clamp;
            BlindLunar = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

            texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Blind/BlindVoid.png");
            texture.wrapMode = TextureWrapMode.Clamp;
            BlindVoid = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

            texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Blind/BlindEq.png");
            texture.wrapMode = TextureWrapMode.Clamp;
            BlindEquipment = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));

            texture = Assets.Bundle.LoadAsset<Texture2D>("Assets/Artifacts/Blind/BlindMod.png");
            texture.wrapMode = TextureWrapMode.Clamp;
            BlindMod = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f));
        }

        public static void OnArtifactEnable()
        {
            On.RoR2.PickupDisplay.RebuildModel += Hide_PickupOverworld;
             
            On.RoR2.GenericPickupController.GetDisplayName += Hide_GetDisplayName;
            On.RoR2.Chat.PlayerPickupChatMessage.ConstructChatString += Hide_PickupChatMessage;
            IL.RoR2.UI.PingIndicator.RebuildPing += Hide_PingChat;

            On.RoR2.CharacterModel.UpdateItemDisplay += Hide_PickupDisplays;

            On.RoR2.UI.ItemIcon.SetItemIndex += Hide_ItemIcon;
            //On.RoR2.UI.EquipmentIcon.SetDisplayData += Hide_EquipmentIcon;

           
 
        }
        public static void OnArtifactDisable()
        {
        }

        private static void Hide_PickupDisplays(On.RoR2.CharacterModel.orig_UpdateItemDisplay orig, CharacterModel self, Inventory inventory)
        {
        }



        private static void Hide_PingChat(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallvirt("RoR2.ShopTerminalBehavior", "get_pickupIndexIsHidden")))
            {
                c.TryGotoNext(MoveType.After,
                x => x.MatchLdfld("RoR2.PickupDef", "nameToken"));
                c.EmitDelegate<Func<string, string>>((damageInfo) =>
                {
                    return "SHOP_ITEM_MYSTERY_TITLE";

                });
            }
            else
            {
                Debug.LogWarning("IL Failed: Hide_PingChat");
            }
        }

        private static string Hide_GetDisplayName(On.RoR2.GenericPickupController.orig_GetDisplayName orig, GenericPickupController self)
        {
            string temp = Language.GetString("SHOP_ITEM_MYSTERY_TITLE");
            PickupDef pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);
            if (pickupDef != null)
            {
                string hex = ColorUtility.ToHtmlStringRGB(pickupDef.baseColor);
                temp = "<color=#" + hex + ">" + temp + "</color>";
                return temp;
            }
            return temp;
        }

        private static string Hide_PickupChatMessage(On.RoR2.Chat.PlayerPickupChatMessage.orig_ConstructChatString orig, Chat.PlayerPickupChatMessage self)
        {
            self.pickupToken = "SHOP_ITEM_MYSTERY_TITLE";
            return orig(self);
        }

 

        /*private static void Hide_EquipmentIcon(On.RoR2.UI.EquipmentIcon.orig_SetDisplayData orig, RoR2.UI.EquipmentIcon self, ValueType newDisplayData)
        {
            orig(self, newDisplayData);
            if (self.iconImage && self.iconImage != null)
            {
                self.iconImage.texture = BlindEquipment.texture;
                if (self.tooltipProvider)
                {
                    self.tooltipProvider.titleToken = "SHOP_ITEM_MYSTERY_TITLE";
                    self.tooltipProvider.bodyToken = "INSPECT_INFO_MISSING_DESCRIPTION";
                }
            }
           
        }*/

      
        
        private static void Hide_ItemIcon(On.RoR2.UI.ItemIcon.orig_SetItemIndex orig, RoR2.UI.ItemIcon self, ItemIndex newItemIndex, int newItemCount)
        {
            //Is this Equipment Icon too??
            orig(self, newItemIndex, newItemCount);

            self.tooltipProvider.titleToken = "SHOP_ITEM_MYSTERY_TITLE";
            self.tooltipProvider.bodyToken = "INSPECT_INFO_MISSING_DESCRIPTION";
            ItemDef itemDef = ItemCatalog.GetItemDef(newItemIndex);
            switch (itemDef.tier)
            {
                case ItemTier.NoTier:
                    break;
                case ItemTier.Tier1:
                    self.image.texture = BlindT1.texture;
                    break;
                case ItemTier.Tier2:
                    self.image.texture = BlindT2.texture;
                    break;
                case ItemTier.Tier3:
                    self.image.texture = BlindT3.texture;
                    break;
                case ItemTier.Boss:
                    self.image.texture = BlindBoss.texture;
                    break;
                case ItemTier.Lunar:
                    self.image.texture = BlindLunar.texture;
                    break;
                case ItemTier.VoidTier1:
                case ItemTier.VoidTier2:
                case ItemTier.VoidTier3:
                case ItemTier.VoidBoss:
                    self.image.texture = BlindVoid.texture;
                    break;
                default:
                    self.image.texture = BlindMod.texture;
                    break;

            }


        }

        private static void Hide_PickupOverworld(On.RoR2.PickupDisplay.orig_RebuildModel orig, PickupDisplay self, GameObject modelObjectOverride)
        {
            self.hidden = true;

            orig(self, modelObjectOverride);
        }





    }
}