using System.Collections.Generic;
using System.Linq;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items.Bags;
using PortableStorage.UI;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
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
			ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
			Player.CanBuyItem += Player_CanBuyItem;
			Player.BuyItem += Player_BuyItem;
			Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;
			Player.HasAmmo += Player_HasAmmo;
			Player.PickAmmo += Player_PickAmmo;
			
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
			long walletCount = player.inventory.OfType<Wallet>().Sum(wallet => wallet.handler.stacks.CountCoins());

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

		private void ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
		{
			Terraria.Player player = Main.LocalPlayer;

			Item item = inv[slot];
			float inventoryScale = Main.inventoryScale;
			Color color = Color.White;
			if (lightColor != Color.Transparent) color = lightColor;
			int num = -1;
			bool flag = false;
			int num2 = 0;
			if (PlayerInput.UsingGamepadUI)
			{
				switch (context)
				{
					case 0:
					case 1:
					case 2:
						num = slot;
						break;
					case 3:
					case 4:
						num = 400 + slot;
						break;
					case 5:
						num = 303;
						break;
					case 6:
						num = 300;
						break;
					case 7:
						num = 1500;
						break;
					case 8:
					case 9:
					case 10:
					case 11:
						num = 100 + slot;
						break;
					case 12:
						if (inv == player.dye)
						{
							num = 120 + slot;
						}

						if (inv == player.miscDyes)
						{
							num = 185 + slot;
						}

						break;
					case 15:
						num = 2700 + slot;
						break;
					case 16:
						num = 184;
						break;
					case 17:
						num = 183;
						break;
					case 18:
						num = 182;
						break;
					case 19:
						num = 180;
						break;
					case 20:
						num = 181;
						break;
					case 22:
						if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig != -1)
						{
							num = 700 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig;
						}

						if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall != -1)
						{
							num = 1500 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall + 1;
						}

						break;
				}

				flag = UILinkPointNavigator.CurrentPoint == num;
				if (context == 0)
				{
					int drawMode = player.DpadRadial.GetDrawMode(slot);
					num2 = drawMode;
					if (num2 > 0 && !PlayerInput.CurrentProfile.UsingDpadHotbar())
					{
						num2 = 0;
					}
				}
			}

			Texture2D texture2D = Main.inventoryBackTexture;
			Color color2 = Main.inventoryBack;
			bool flag2 = false;
			if (item.type > 0 && item.stack > 0 && item.favorited && context != 13 && context != 21 && context != 22 && context != 14)
			{
				texture2D = Main.inventoryBack10Texture;
			}
			else if (item.type > 0 && item.stack > 0 && Terraria.UI.ItemSlot.Options.HighlightNewItems && item.newAndShiny && context != 13 && context != 21 && context != 14 && context != 22)
			{
				texture2D = Main.inventoryBack15Texture;
				float num3 = Main.mouseTextColor / 255f;
				num3 = num3 * 0.2f + 0.8f;
				color2 = color2.MultiplyRGBA(new Color(num3, num3, num3));
			}
			else if (PlayerInput.UsingGamepadUI && item.type > 0 && item.stack > 0 && num2 != 0 && context != 13 && context != 21 && context != 22)
			{
				texture2D = Main.inventoryBack15Texture;
				float num4 = Main.mouseTextColor / 255f;
				num4 = num4 * 0.2f + 0.8f;
				if (num2 == 1)
				{
					color2 = color2.MultiplyRGBA(new Color(num4, num4 / 2f, num4 / 2f));
				}
				else
				{
					color2 = color2.MultiplyRGBA(new Color(num4 / 2f, num4, num4 / 2f));
				}
			}
			else if (context == 0 && slot < 10)
			{
				texture2D = Main.inventoryBack9Texture;
			}
			else if (context == 10 || context == 8 || context == 16 || context == 17 || context == 19 || context == 18 || context == 20)
			{
				texture2D = Main.inventoryBack3Texture;
			}
			else if (context == 11 || context == 9)
			{
				texture2D = Main.inventoryBack8Texture;
			}
			else if (context == 12)
			{
				texture2D = Main.inventoryBack12Texture;
			}
			else if (context == 3)
			{
				texture2D = Main.inventoryBack5Texture;
			}
			else if (context == 4)
			{
				texture2D = Main.inventoryBack2Texture;
			}
			else if (context == 7 || context == 5)
			{
				texture2D = Main.inventoryBack4Texture;
			}
			else if (context == 6)
			{
				texture2D = Main.inventoryBack7Texture;
			}
			else if (context == 13)
			{
				byte b = 200;
				if (slot == Main.player[Main.myPlayer].selectedItem)
				{
					texture2D = Main.inventoryBack14Texture;
					b = 255;
				}

				color2 = new Color(b, b, b, b);
			}
			else if (context == 14 || context == 21)
			{
				flag2 = true;
			}
			else if (context == 15)
			{
				texture2D = Main.inventoryBack6Texture;
			}
			else if (context == 22)
			{
				texture2D = Main.inventoryBack4Texture;
			}

			if (context == 0 && typeof(ItemSlot).GetValue<int[]>("inventoryGlowTime")?[slot] > 0 && !inv[slot].favorited)
			{
				float scale = Main.invAlpha / 255f;
				Color value = new Color(63, 65, 151, 255) * scale;
				Color value2 = Main.hslToRgb(typeof(ItemSlot).GetValue<int[]>("inventoryGlowHue")[slot], 1f, 0.5f) * scale;
				float num5 = typeof(ItemSlot).GetValue<int[]>("inventoryGlowTime")[slot] / 300f;
				num5 *= num5;
				color2 = Color.Lerp(value, value2, num5 / 2f);
				texture2D = Main.inventoryBack13Texture;
			}

			if ((context == 4 || context == 3) && typeof(ItemSlot).GetValue<int[]>("inventoryGlowTimeChest")?[slot] > 0 && !inv[slot].favorited)
			{
				float scale2 = Main.invAlpha / 255f;
				Color value3 = new Color(130, 62, 102, 255) * scale2;
				if (context == 3)
				{
					value3 = new Color(104, 52, 52, 255) * scale2;
				}

				Color value4 = Main.hslToRgb(typeof(ItemSlot).GetValue<int[]>("inventoryGlowHueChest")[slot], 1f, 0.5f) * scale2;
				float num6 = typeof(ItemSlot).GetValue<int[]>("inventoryGlowTimeChest")[slot] / 300f;
				num6 *= num6;
				color2 = Color.Lerp(value3, value4, num6 / 2f);
				texture2D = Main.inventoryBack13Texture;
			}

			if (flag)
			{
				texture2D = Main.inventoryBack14Texture;
				color2 = Color.White;
			}

			if (!flag2)
			{
				spriteBatch.Draw(texture2D, position, null, color2, 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);
			}

			int num7 = -1;
			switch (context)
			{
				case 8:
					if (slot == 0)
					{
						num7 = 0;
					}

					if (slot == 1)
					{
						num7 = 6;
					}

					if (slot == 2)
					{
						num7 = 12;
					}

					break;
				case 9:
					if (slot == 10)
					{
						num7 = 3;
					}

					if (slot == 11)
					{
						num7 = 9;
					}

					if (slot == 12)
					{
						num7 = 15;
					}

					break;
				case 10:
					num7 = 11;
					break;
				case 11:
					num7 = 2;
					break;
				case 12:
					num7 = 1;
					break;
				case 16:
					num7 = 4;
					break;
				case 17:
					num7 = 13;
					break;
				case 18:
					num7 = 7;
					break;
				case 19:
					num7 = 10;
					break;
				case 20:
					num7 = 17;
					break;
			}

			if ((item.type <= 0 || item.stack <= 0) && num7 != -1)
			{
				Texture2D texture2D2 = Main.extraTexture[54];
				Rectangle rectangle = texture2D2.Frame(3, 6, num7 % 3, num7 / 3);
				rectangle.Width -= 2;
				rectangle.Height -= 2;
				spriteBatch.Draw(texture2D2, position + texture2D.Size() / 2f * inventoryScale, rectangle, Color.White * 0.35f, 0f, rectangle.Size() / 2f, inventoryScale, SpriteEffects.None, 0f);
			}

			Vector2 vector = texture2D.Size() * inventoryScale;
			if (item.type > 0 && item.stack > 0)
			{
				Texture2D texture2D3 = Main.itemTexture[item.type];
				Rectangle rectangle2;
				if (Main.itemAnimations[item.type] != null)
				{
					rectangle2 = Main.itemAnimations[item.type].GetFrame(texture2D3);
				}
				else
				{
					rectangle2 = texture2D3.Frame(1, 1, 0, 0);
				}

				Color newColor = color;
				float num8 = 1f;
				Terraria.UI.ItemSlot.GetItemLight(ref newColor, ref num8, item, false);
				float num9 = 1f;
				if (rectangle2.Width > 32 || rectangle2.Height > 32)
				{
					if (rectangle2.Width > rectangle2.Height)
					{
						num9 = 32f / rectangle2.Width;
					}
					else
					{
						num9 = 32f / rectangle2.Height;
					}
				}

				num9 *= inventoryScale;
				Vector2 position2 = position + vector / 2f - rectangle2.Size() * num9 / 2f;
				Vector2 origin = rectangle2.Size() * (num8 / 2f - 0.5f);
				if (ItemLoader.PreDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(newColor), item.GetColor(color), origin, num9 * num8))
				{
					spriteBatch.Draw(texture2D3, position2, rectangle2, item.GetAlpha(newColor), 0f, origin, num9 * num8, SpriteEffects.None, 0f);
					if (item.color != Color.Transparent)
					{
						spriteBatch.Draw(texture2D3, position2, rectangle2, item.GetColor(color), 0f, origin, num9 * num8, SpriteEffects.None, 0f);
					}
				}

				ItemLoader.PostDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(newColor), item.GetColor(color), origin, num9 * num8);
				if (ItemID.Sets.TrapSigned[item.type])
				{
					spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f) * inventoryScale, new Rectangle(4, 58, 8, 8), color, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
				}

				if (item.stack > 1)
				{
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, item.stack.ToString(), position + new Vector2(10f, 26f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
				}

				int num10 = -1;
				if (context == 13)
				{
					if (item.DD2Summon)
					{
						for (int i = 0; i < 58; i++)
						{
							if (inv[i].type == 3822)
							{
								num10 += inv[i].stack;
							}
						}

						if (num10 >= 0)
						{
							num10++;
						}
					}

					if (item.useAmmo > 0)
					{
						int useAmmo = item.useAmmo;
						num10 = 0;
						for (int j = 0; j < 58; j++)
						{
							if (inv[j].ammo == useAmmo)
							{
								num10 += inv[j].stack;
							}
						}

						num10 += player.inventory.OfType<BaseAmmoBag>().SelectMany(x => x.handler.stacks).Where(x => x.ammo == useAmmo).Sum(x => x.stack);
					}

					if (item.fishingPole > 0)
					{
						num10 = 0;
						for (int k = 0; k < 58; k++)
						{
							if (inv[k].bait > 0)
							{
								num10 += inv[k].stack;
							}
						}
					}

					if (item.tileWand > 0)
					{
						int tileWand = item.tileWand;
						num10 = 0;
						for (int l = 0; l < 58; l++)
						{
							if (inv[l].type == tileWand)
							{
								num10 += inv[l].stack;
							}
						}
					}

					if (item.type == 509 || item.type == 851 || item.type == 850 || item.type == 3612 || item.type == 3625 || item.type == 3611)
					{
						num10 = 0;
						for (int m = 0; m < 58; m++)
						{
							if (inv[m].type == 530)
							{
								num10 += inv[m].stack;
							}
						}
					}
				}

				if (num10 != -1)
				{
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, num10.ToString(), position + new Vector2(8f, 30f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale * 0.8f), -1f, inventoryScale);
				}

				if (context == 13)
				{
					string text = string.Concat(slot + 1);
					if (text == "10")
					{
						text = "0";
					}

					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, position + new Vector2(8f, 4f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
				}

				if (context == 13 && item.potion)
				{
					Vector2 position3 = position + texture2D.Size() * inventoryScale / 2f - Main.cdTexture.Size() * inventoryScale / 2f;
					Color color3 = item.GetAlpha(color) * (player.potionDelay / (float)player.potionDelayTime);
					spriteBatch.Draw(Main.cdTexture, position3, null, color3, 0f, default(Vector2), num9, SpriteEffects.None, 0f);
				}

				if ((context == 10 || context == 18) && item.expertOnly && !Main.expertMode)
				{
					Vector2 position4 = position + texture2D.Size() * inventoryScale / 2f - Main.cdTexture.Size() * inventoryScale / 2f;
					Color white = Color.White;
					spriteBatch.Draw(Main.cdTexture, position4, null, white, 0f, default(Vector2), num9, SpriteEffects.None, 0f);
				}
			}
			else if (context == 6)
			{
				Texture2D trashTexture = Main.trashTexture;
				Vector2 position5 = position + texture2D.Size() * inventoryScale / 2f - trashTexture.Size() * inventoryScale / 2f;
				spriteBatch.Draw(trashTexture, position5, null, new Color(100, 100, 100, 100), 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);
			}

			if (context == 0 && slot < 10)
			{
				float num11 = inventoryScale;
				string text2 = string.Concat(slot + 1);
				if (text2 == "10")
				{
					text2 = "0";
				}

				Color inventoryBack = Main.inventoryBack;
				int num12 = 0;
				if (Main.player[Main.myPlayer].selectedItem == slot)
				{
					num12 -= 3;
					inventoryBack.R = 255;
					inventoryBack.B = 0;
					inventoryBack.G = 210;
					inventoryBack.A = 100;
					num11 *= 1.4f;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text2, position + new Vector2(6f, 4 + num12) * inventoryScale, inventoryBack, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
			}

			if (num != -1)
			{
				UILinkPointNavigator.SetPosition(num, position + vector * 0.75f);
			}
		}

		private bool Player_BuyItem(Player.orig_BuyItem orig, Terraria.Player self, int price, int customCurrency)
		{
			if (customCurrency != -1) return CustomCurrencyManager.BuyItem(self, price, customCurrency);

			long inventoryCount = Utils.CoinsCount(out bool _, self.inventory, 58, 57, 56, 55, 54);
			long piggyCount = Utils.CoinsCount(out bool _, self.bank.item);
			long safeCount = Utils.CoinsCount(out bool _, self.bank2.item);
			long defendersCount = Utils.CoinsCount(out bool _, self.bank3.item);
			long walletCount = self.inventory.OfType<Wallet>().Sum(wallet => wallet.handler.stacks.CountCoins());

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
			long walletCount = self.inventory.OfType<Wallet>().Sum(wallet => wallet.handler.stacks.CountCoins());
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

		private bool Player_HasAmmo(Player.orig_HasAmmo orig, Terraria.Player self, Item ammoUser, bool canUse)
		{
			if (ammoUser.useAmmo > 0) canUse = self.inventory.Any(item => item.ammo == ammoUser.useAmmo && item.stack > 0) || self.inventory.OfType<BaseAmmoBag>().Any(ammoBag => ammoBag.handler.stacks.Any(item => item.ammo == ammoUser.useAmmo && item.stack > 0));
			return canUse;
		}

		private void Player_PickAmmo(Player.orig_PickAmmo orig, Terraria.Player self, Item sItem, ref int shoot, ref float speed, ref bool canShoot, ref int Damage, ref float KnockBack, bool dontConsume)
		{
			Item item = new Item();

			Item firstAmmo = self.inventory.OfType<BaseAmmoBag>().SelectMany(x => x.handler.stacks).FirstOrDefault(ammo => ammo.ammo == sItem.useAmmo && ammo.stack > 0);
			if (firstAmmo != null)
			{
				item = firstAmmo;
				canShoot = true;
			}
			else
			{
				bool hasInAmmoSlots = false;

				for (int i = 54; i < 58; i++)
				{
					if (self.inventory[i].ammo == sItem.useAmmo && self.inventory[i].stack > 0)
					{
						item = self.inventory[i];
						canShoot = true;
						hasInAmmoSlots = true;
						break;
					}
				}

				if (!hasInAmmoSlots)
				{
					for (int j = 0; j < 54; j++)
					{
						if (self.inventory[j].ammo == sItem.useAmmo && self.inventory[j].stack > 0)
						{
							item = self.inventory[j];
							canShoot = true;
							break;
						}
					}
				}
			}

			if (canShoot)
			{
				if (sItem.type == 1946)
				{
					shoot = 338 + item.type - 771;
					if (shoot > ProjectileID.RocketSnowmanIV) shoot = ProjectileID.RocketSnowmanIV;
				}
				else if (sItem.useAmmo == AmmoID.Rocket) shoot += item.shoot;
				else if (sItem.useAmmo == 780) shoot += item.shoot;
				else if (item.shoot > 0) shoot = item.shoot;

				if (sItem.type == 3019 && shoot == 1) shoot = 485;
				if (sItem.type == 3052) shoot = 495;
				if (sItem.type == 3245 && shoot == 21) shoot = 532;
				if (shoot == 42)
				{
					if (item.type == 370)
					{
						shoot = 65;
						Damage += 5;
					}
					else if (item.type == 408)
					{
						shoot = 68;
						Damage += 5;
					}
					else if (item.type == 1246)
					{
						shoot = 354;
						Damage += 5;
					}
				}

				if (self.HeldItem.type == 2888 && shoot == 1) shoot = 469;
				if (self.magicQuiver && (sItem.useAmmo == AmmoID.Arrow || sItem.useAmmo == AmmoID.Stake))
				{
					KnockBack = (int)(KnockBack * 1.1);
					speed *= 1.1f;
				}

				speed += item.shootSpeed;
				if (item.ranged)
				{
					if (item.damage > 0)
					{
						Damage += (int)(item.damage * self.rangedDamage);
					}
				}
				else Damage += item.damage;

				if (sItem.useAmmo == AmmoID.Arrow && self.archery)
				{
					if (speed < 20f)
					{
						speed *= 1.2f;
						if (speed > 20f) speed = 20f;
					}

					Damage = (int)(Damage * 1.2);
				}

				KnockBack += item.knockBack;
				ItemLoader.PickAmmo(item, self, ref shoot, ref speed, ref Damage, ref KnockBack);
				bool dontConsumeAmmo = dontConsume;
				if (sItem.type == 3245)
				{
					if (Main.rand.Next(3) == 0) dontConsumeAmmo = true;
					else if (self.thrownCost33 && Main.rand.Next(100) < 33) dontConsumeAmmo = true;
					else if (self.thrownCost50 && Main.rand.Next(100) < 50) dontConsumeAmmo = true;
				}

				if (sItem.type == 3475 && Main.rand.Next(3) != 0) dontConsumeAmmo = true;
				if (sItem.type == 3540 && Main.rand.Next(3) != 0) dontConsumeAmmo = true;
				if (self.magicQuiver && sItem.useAmmo == AmmoID.Arrow && Main.rand.Next(5) == 0) dontConsumeAmmo = true;
				if (self.ammoBox && Main.rand.Next(5) == 0) dontConsumeAmmo = true;
				if (self.ammoPotion && Main.rand.Next(5) == 0) dontConsumeAmmo = true;
				if (sItem.type == 1782 && Main.rand.Next(3) == 0) dontConsumeAmmo = true;
				if (sItem.type == 98 && Main.rand.Next(3) == 0) dontConsumeAmmo = true;
				if (sItem.type == 2270 && Main.rand.Next(2) == 0) dontConsumeAmmo = true;
				if (sItem.type == 533 && Main.rand.Next(2) == 0) dontConsumeAmmo = true;
				if (sItem.type == 1929 && Main.rand.Next(2) == 0) dontConsumeAmmo = true;
				if (sItem.type == 1553 && Main.rand.Next(2) == 0) dontConsumeAmmo = true;
				if (sItem.type == 434 && self.itemAnimation < (int)(sItem.useAnimation / PlayerHooks.TotalMeleeSpeedMultiplier(self, sItem)) - 2) dontConsumeAmmo = true;
				if (self.ammoCost80 && Main.rand.Next(5) == 0) dontConsumeAmmo = true;
				if (self.ammoCost75 && Main.rand.Next(4) == 0) dontConsumeAmmo = true;
				if (shoot == 85 && self.itemAnimation < self.itemAnimationMax - 6) dontConsumeAmmo = true;
				if ((shoot == 145 || shoot == 146 || shoot == 147 || shoot == 148 || shoot == 149) && self.itemAnimation < self.itemAnimationMax - 5) dontConsumeAmmo = true;
				dontConsumeAmmo |= !PlayerHooks.ConsumeAmmo(self, sItem, item) | !ItemLoader.ConsumeAmmo(sItem, item, self);
				if (!dontConsumeAmmo && item.consumable)
				{
					PlayerHooks.OnConsumeAmmo(self, sItem, item);
					ItemLoader.OnConsumeAmmo(sItem, item, self);
					item.stack--;
					if (item.stack <= 0)
					{
						item.active = false;
						item.TurnToAir();
					}
				}
			}
		}
	}
}