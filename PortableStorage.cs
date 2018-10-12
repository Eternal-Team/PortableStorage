using System.Collections.Generic;
using System.Linq;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items;
using PortableStorage.Items.Bags;
using PortableStorage.UI;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using ItemSlot = On.Terraria.UI.ItemSlot;
using Player = On.Terraria.Player;

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
			ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
			ItemSlot.DrawSavings += ItemSlot_DrawSavings;
			Player.CanBuyItem += Player_CanBuyItem;
			Player.BuyItem += Player_BuyItem;
			Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;

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

		private void ItemSlot_DrawSavings(ItemSlot.orig_DrawSavings orig, SpriteBatch sb, float shopx, float shopy, bool horizontal)
		{
			Terraria.Player player = Main.LocalPlayer;
			int customCurrencyForSavings = typeof(Terraria.UI.ItemSlot).GetValue<int>("_customCurrencyForSavings");

			if (customCurrencyForSavings != -1)
			{
				CustomCurrencyManager.DrawSavings(sb, customCurrencyForSavings, shopx, shopy, horizontal);
				return;
			}

			long piggyCount = Utils.CoinsCount(out bool _, player.bank.item);
			long safeCount = Utils.CoinsCount(out bool _, player.bank2.item);
			long defendersCount = Utils.CoinsCount(out bool _, player.bank3.item);
			long walletCount = player.inventory.OfType<Wallet>().Sum(wallet => wallet.handler.stacks.CountCoins(out bool _));

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

		private bool Player_BuyItem(Player.orig_BuyItem orig, Terraria.Player self, int price, int customCurrency)
		{
			if (customCurrency != -1) return CustomCurrencyManager.BuyItem(self, price, customCurrency);

			long inventoryCount = Utils.CoinsCount(out bool _, self.inventory, 58, 57, 56, 55, 54);
			long piggyCount = Utils.CoinsCount(out bool _, self.bank.item);
			long safeCount = Utils.CoinsCount(out bool _, self.bank2.item);
			long defendersCount = Utils.CoinsCount(out bool _, self.bank3.item);
			long walletCount = self.inventory.OfType<Wallet>().Sum(wallet => wallet.handler.stacks.CountCoins(out bool _));

			long combined = Utils.CoinsCombineStacks(out bool _, inventoryCount, piggyCount, safeCount, defendersCount, walletCount);

			if (combined < price) return false;

			List<Item[]> list = new List<Item[]>();
			Dictionary<int, List<int>> ignoredSlots = new Dictionary<int, List<int>>();
			List<Point> coins = new List<Point>();
			List<Point> emptyInventory = new List<Point>();
			List<Point> emptyPiggy = new List<Point>();
			List<Point> emptySafe = new List<Point>();
			List<Point> emptyDefenders = new List<Point>();
			List<Point> emptyWallet = new List<Point>();
			list.Add(self.inventory);
			list.Add(self.bank.item);
			list.Add(self.bank2.item);
			list.Add(self.bank3.item);
			list.AddRange(self.inventory.OfType<Wallet>().Select(x => x.handler.stacks.ToArray()));
			for (int i = 0; i < list.Count; i++) ignoredSlots[i] = new List<int>();

			ignoredSlots[0] = new List<int>
			{
				58,
				57,
				56,
				55,
				54
			};
			for (int j = 0; j < list.Count; j++)
			{
				for (int k = 0; k < list[j].Length; k++)
				{
					if (!ignoredSlots[j].Contains(k) && list[j][k].IsCoin())
					{
						coins.Add(new Point(j, k));
					}
				}
			}

			int num6 = 0;
			for (int l = list[num6].Length - 1; l >= 0; l--)
			{
				if (!ignoredSlots[num6].Contains(l) && (list[num6][l].type == 0 || list[num6][l].stack == 0)) emptyInventory.Add(new Point(num6, l));
			}

			num6 = 1;
			for (int m = list[num6].Length - 1; m >= 0; m--)
			{
				if (!ignoredSlots[num6].Contains(m) && (list[num6][m].type == 0 || list[num6][m].stack == 0)) emptyPiggy.Add(new Point(num6, m));
			}

			num6 = 2;
			for (int n = list[num6].Length - 1; n >= 0; n--)
			{
				if (!ignoredSlots[num6].Contains(n) && (list[num6][n].type == 0 || list[num6][n].stack == 0)) emptySafe.Add(new Point(num6, n));
			}

			num6 = 3;
			for (int num7 = list[num6].Length - 1; num7 >= 0; num7--)
			{
				if (!ignoredSlots[num6].Contains(num7) && (list[num6][num7].type == 0 || list[num6][num7].stack == 0)) emptyDefenders.Add(new Point(num6, num7));
			}

			num6 = 4;
			for (int i = num6; i < list.Count - 4; i++)
			{
				for (int n = list[i].Length - 1; n >= 0; n--)
				{
					if (!ignoredSlots[i].Contains(n) && (list[i][n].type == 0 || list[i][n].stack == 0)) emptyWallet.Add(new Point(i, n));
				}
			}

			return !Player_TryPurchasing(price, list, coins, emptyInventory, emptyPiggy, emptySafe, emptyDefenders, emptyWallet);
		}

		private bool Player_CanBuyItem(Player.orig_CanBuyItem orig, Terraria.Player self, int price, int customCurrency)
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

		private bool Player_TryPurchasing(int price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, List<Point> slotEmptyWallet)
		{
			long priceRemaining = price;
			Dictionary<Point, Item> dictionary = new Dictionary<Point, Item>();

			bool result = false;
			while (priceRemaining > 0L)
			{
				long coinValue = 1000000L;
				for (int coinIndex = 0; coinIndex < 4; coinIndex++)
				{
					if (priceRemaining >= coinValue)
					{
						foreach (Point current in slotCoins)
						{
							if (inv[current.X][current.Y].type == 74 - coinIndex)
							{
								long stack = priceRemaining / coinValue;
								dictionary[current] = inv[current.X][current.Y].Clone();
								if (stack < inv[current.X][current.Y].stack) inv[current.X][current.Y].stack -= (int)stack;
								else
								{
									inv[current.X][current.Y].SetDefaults();
									slotsEmpty.Add(current);
								}

								priceRemaining -= coinValue * (dictionary[current].stack - inv[current.X][current.Y].stack);
							}
						}
					}

					coinValue /= 100L;
				}

				if (priceRemaining > 0L)
				{
					if (slotsEmpty.Count <= 0)
					{
						foreach (KeyValuePair<Point, Item> current2 in dictionary) inv[current2.Key.X][current2.Key.Y] = current2.Value.Clone();
						result = true;
						break;
					}

					slotsEmpty.Sort(DelegateMethods.CompareYReverse);
					Point item = new Point(-1, -1);
					for (int j = 0; j < inv.Count; j++)
					{
						coinValue = 10000L;
						for (int k = 0; k < 3; k++)
						{
							if (priceRemaining >= coinValue)
							{
								foreach (Point current3 in slotCoins)
								{
									if (current3.X == j && inv[current3.X][current3.Y].type == 74 - k && inv[current3.X][current3.Y].stack >= 1)
									{
										List<Point> list = slotsEmpty;
										if (j == 1 && slotEmptyBank.Count > 0) list = slotEmptyBank;
										if (j == 2 && slotEmptyBank2.Count > 0) list = slotEmptyBank2;
										if (j > 3 && slotEmptyWallet.Count > 0) list = slotEmptyWallet;
										if (--inv[current3.X][current3.Y].stack <= 0)
										{
											inv[current3.X][current3.Y].SetDefaults();
											list.Add(current3);
										}

										dictionary[list[0]] = inv[list[0].X][list[0].Y].Clone();
										inv[list[0].X][list[0].Y].SetDefaults(73 - k);
										inv[list[0].X][list[0].Y].stack = 100;
										item = list[0];
										list.RemoveAt(0);
										break;
									}
								}
							}

							if (item.X != -1 || item.Y != -1) break;
							coinValue /= 100L;
						}

						for (int l = 0; l < 2; l++)
						{
							if (item.X == -1 && item.Y == -1)
							{
								foreach (Point current4 in slotCoins)
								{
									if (current4.X == j && inv[current4.X][current4.Y].type == 73 + l && inv[current4.X][current4.Y].stack >= 1)
									{
										List<Point> list2 = slotsEmpty;
										if (j == 1 && slotEmptyBank.Count > 0) list2 = slotEmptyBank;
										if (j == 2 && slotEmptyBank2.Count > 0) list2 = slotEmptyBank2;
										if (j == 3 && slotEmptyBank3.Count > 0) list2 = slotEmptyBank3;
										if (j > 3 && slotEmptyWallet.Count > 0) list2 = slotEmptyWallet;
										if (--inv[current4.X][current4.Y].stack <= 0)
										{
											inv[current4.X][current4.Y].SetDefaults();
											list2.Add(current4);
										}

										dictionary[list2[0]] = inv[list2[0].X][list2[0].Y].Clone();
										inv[list2[0].X][list2[0].Y].SetDefaults(72 + l);
										inv[list2[0].X][list2[0].Y].stack = 100;
										item = list2[0];
										list2.RemoveAt(0);
										break;
									}
								}
							}
						}

						if (item.X != -1 && item.Y != -1)
						{
							slotCoins.Add(item);
							break;
						}
					}

					slotsEmpty.Sort(DelegateMethods.CompareYReverse);
					slotEmptyBank.Sort(DelegateMethods.CompareYReverse);
					slotEmptyBank2.Sort(DelegateMethods.CompareYReverse);
					slotEmptyBank3.Sort(DelegateMethods.CompareYReverse);
					slotEmptyWallet.Sort(DelegateMethods.CompareYReverse);
				}
			}

			return result;
		}
	}
}