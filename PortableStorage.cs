using BaseLibrary.UI;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items;
using PortableStorage.Items.Bags;
using PortableStorage.UI;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using ItemSlot = On.Terraria.UI.ItemSlot;

namespace PortableStorage
{
    public partial class PortableStorage : Mod
    {
        public static PortableStorage Instance;
        public int BagID;

        public GUI<BagUI> BagUI;

        public static ModHotKey HotkeyBag;

        public override void Load()
        {
            Instance = this;

            On.Terraria.UI.UIElement.GetElementAt += UIElement_GetElementAt;
            // todo: probably need to do rightclick
            ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
            ItemSlot.DrawSavings += ItemSlot_DrawSavings;
            On.Terraria.Player.CanBuyItem += Player_CanBuyItem;
            //Player.BuyItem += Player_BuyItem;

            HotkeyBag = this.Register("Open Bag", Keys.B);

            if (!Main.dedServ)
            {
                this.LoadTextures();

                BagUI = Utility.SetupGUI<BagUI>();
                BagUI.Visible = true;
            }
        }

        public override void Unload()
        {
            Utility.UnloadNullableTypes();
        }

        public override void PostAddRecipes()
        {
            foreach (ModItem item in this.GetValue<Dictionary<string, ModItem>>("items").Values)
            {
                Recipe recipe = Main.recipe.FirstOrDefault(x => x.createItem.type == item.item.type);
                if (recipe != null) item.item.value = recipe.requiredItem.Sum(x => x.value);
            }
        }

        public override void PreSaveAndQuit()
        {
            BagUI.UI.Elements.Clear();
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int InventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

            if (BagUI != null && InventoryIndex != -1) layers.Insert(InventoryIndex + 1, BagUI.InterfaceLayer);
        }
    }

    public partial class PortableStorage
    {
        private UIElement UIElement_GetElementAt(On.Terraria.UI.UIElement.orig_GetElementAt orig, UIElement self, Vector2 point)
        {
            if (self is BagUI ui)
            {
                UIElement uIElement = null;
                for (int i = ui.Elements.Count - 1; i >= 0; i--)
                {
                    if (ui.Elements[i].ContainsPoint(point)) uIElement = ui.Elements[i];
                }

                if (uIElement != null) return uIElement.GetElementAt(point);
                return self.ContainsPoint(point) ? self : null;
            }

            return orig?.Invoke(self, point);
        }

        private void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (inv[slot].modItem is BaseBag bag && bag.UI != null) BagUI.UI.CloseBag(bag);

            orig(inv, context, slot);
        }

        //private bool Player_BuyItem(Player.orig_BuyItem orig, Terraria.Player self, int price, int customCurrency)
        //{
        //    if (customCurrency != -1)
        //    {
        //        return CustomCurrencyManager.BuyItem(this, price, customCurrency);
        //    }

        //    bool flag;
        //    long num = Utils.CoinsCount(out flag, this.inventory, 58, 57, 56, 55, 54);
        //    long num2 = Utils.CoinsCount(out flag, this.bank.item);
        //    long num3 = Utils.CoinsCount(out flag, this.bank2.item);
        //    long num4 = Utils.CoinsCount(out flag, this.bank3.item);
        //    long num5 = Utils.CoinsCombineStacks(out flag, num, num2, num3, num4);
        //    if (num5 < price)
        //    {
        //        return false;
        //    }

        //    List<Item[]> list = new List<Item[]>();
        //    Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
        //    List<Point> list2 = new List<Point>();
        //    List<Point> list3 = new List<Point>();
        //    List<Point> list4 = new List<Point>();
        //    List<Point> list5 = new List<Point>();
        //    List<Point> list6 = new List<Point>();
        //    list.Add(this.inventory);
        //    list.Add(this.bank.item);
        //    list.Add(this.bank2.item);
        //    list.Add(this.bank3.item);
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        dictionary[i] = new List<int>();
        //    }

        //    dictionary[0] = new List<int>
        //    {
        //        58,
        //        57,
        //        56,
        //        55,
        //        54
        //    };
        //    for (int j = 0; j < list.Count; j++)
        //    {
        //        for (int k = 0; k < list[j].Length; k++)
        //        {
        //            if (!dictionary[j].Contains(k) && list[j][k].type >= 71 && list[j][k].type <= 74)
        //            {
        //                list3.Add(new Point(j, k));
        //            }
        //        }
        //    }

        //    int num6 = 0;
        //    for (int l = list[num6].Length - 1; l >= 0; l--)
        //    {
        //        if (!dictionary[num6].Contains(l) && (list[num6][l].type == 0 || list[num6][l].stack == 0))
        //        {
        //            list2.Add(new Point(num6, l));
        //        }
        //    }

        //    num6 = 1;
        //    for (int m = list[num6].Length - 1; m >= 0; m--)
        //    {
        //        if (!dictionary[num6].Contains(m) && (list[num6][m].type == 0 || list[num6][m].stack == 0))
        //        {
        //            list4.Add(new Point(num6, m));
        //        }
        //    }

        //    num6 = 2;
        //    for (int n = list[num6].Length - 1; n >= 0; n--)
        //    {
        //        if (!dictionary[num6].Contains(n) && (list[num6][n].type == 0 || list[num6][n].stack == 0))
        //        {
        //            list5.Add(new Point(num6, n));
        //        }
        //    }

        //    num6 = 3;
        //    for (int num7 = list[num6].Length - 1; num7 >= 0; num7--)
        //    {
        //        if (!dictionary[num6].Contains(num7) && (list[num6][num7].type == 0 || list[num6][num7].stack == 0))
        //        {
        //            list6.Add(new Point(num6, num7));
        //        }
        //    }

        //    bool flag2 = Terraria.Player.TryPurchasing(price, list, list3, list2, list4, list5, list6);
        //    return !flag2;
        //}

        private bool Player_CanBuyItem(On.Terraria.Player.orig_CanBuyItem orig, Player self, int price, int customCurrency)
        {
            if (customCurrency != -1) return CustomCurrencyManager.BuyItem(self, price, customCurrency);

            long inventoryCount = Utils.CoinsCount(out bool _, self.inventory, 58, 57, 56, 55, 54);
            long piggyCount = Utils.CoinsCount(out bool _, self.bank.item);
            long safeCount = Utils.CoinsCount(out bool _, self.bank2.item);
            long defendersCount = Utils.CoinsCount(out bool _, self.bank3.item);
            long walletCount = self.inventory.Where(x => x.modItem is Wallet).Sum(x => ((Wallet)x.modItem).handler.stacks.CountCoins(out bool _));
            long combined = Utils.CoinsCombineStacks(out bool _, inventoryCount, piggyCount, safeCount, defendersCount, walletCount);

            return combined >= price;
        }

        private void ItemSlot_DrawSavings(ItemSlot.orig_DrawSavings orig, SpriteBatch sb, float shopx, float shopy, bool horizontal)
        {
            Player player = Main.LocalPlayer;
            int customCurrencyForSavings = typeof(Terraria.UI.ItemSlot).GetValue<int>("_customCurrencyForSavings");

            if (customCurrencyForSavings != -1)
            {
                CustomCurrencyManager.DrawSavings(sb, customCurrencyForSavings, shopx, shopy, horizontal);
                return;
            }

            long piggyCount = Utils.CoinsCount(out bool _, player.bank.item);
            long safeCount = Utils.CoinsCount(out bool _, player.bank2.item);
            long defendersCount = Utils.CoinsCount(out bool _, player.bank3.item);
            long walletCount = player.inventory.Where(x => x.modItem is Wallet).Sum(x => ((Wallet)x.modItem).handler.stacks.CountCoins(out bool _));

            long combined = Utils.CoinsCombineStacks(out bool _, piggyCount, safeCount, defendersCount, walletCount);
            if (combined > 0L)
            {
                if (defendersCount > 0L) sb.Draw(Main.itemTexture[ItemID.DefendersForge], Utils.CenteredRectangle(new Vector2(shopx + 92f, shopy + 45f), Main.itemTexture[ItemID.DefendersForge].Size() * 0.65f), null, Color.White);
                if (walletCount > 0L) sb.Draw(Main.itemTexture[ItemType<Wallet>()], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 40f), Main.itemTexture[ItemType<Wallet>()].Size() * 0.5f));
                if (safeCount > 0L) sb.Draw(Main.itemTexture[ItemID.Safe], Utils.CenteredRectangle(new Vector2(shopx + 80f, shopy + 50f), Main.itemTexture[ItemID.Safe].Size() * 0.65f), null, Color.White);
                if (piggyCount > 0L) sb.Draw(Main.itemTexture[ItemID.PiggyBank], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 60f), Main.itemTexture[ItemID.PiggyBank].Size() * 0.65f), null, Color.White);
                Terraria.UI.ItemSlot.DrawMoney(sb, Language.GetTextValue("LegacyInterface.66"), shopx, shopy, Utils.CoinsSplit(combined), horizontal);
            }
        }
    }
}