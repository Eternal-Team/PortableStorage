using System.Linq;
using BaseLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items;
using PortableStorage.Items.Ammo;
using PortableStorage.UI;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using ItemSlot = On.Terraria.UI.ItemSlot;
using Language = Terraria.Localization.Language;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static UIElement UIElement_GetElementAt(On.Terraria.UI.UIElement.orig_GetElementAt orig, UIElement self, Vector2 point)
		{
			if (self is PanelUI ui)
			{
				UIElement uIElement = null;
				for (int i = ui.Elements.Count - 1; i >= 0; i--)
				{
					UIElement current = ui.Elements[i];
					if (current.ContainsPoint(point))
					{
						uIElement = current;
						break;
					}
				}

				if (uIElement != null) return uIElement.GetElementAt(point);
				return self.ContainsPoint(point) ? self : null;
			}

			return orig(self, point);
		}

		private static void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (inv[slot].modItem is BaseBag bag && bag.UI != null) PortableStorage.Instance.PanelUI.UI.CloseUI(bag);

			orig(inv, context, slot);
		}

		private static void ItemSlot_DrawSavings(ItemSlot.orig_DrawSavings orig, SpriteBatch sb, float shopx, float shopy, bool horizontal)
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
			long walletCount = player.inventory.OfType<Wallet>().Sum(wallet => wallet.Handler.stacks.CountCoins());

			long combined = Utils.CoinsCombineStacks(out bool _, piggyCount, safeCount, defendersCount, walletCount);
			if (combined > 0L)
			{
				int walletType = PortableStorage.Instance.ItemType<Wallet>();

				if (defendersCount > 0L) sb.Draw(Main.itemTexture[ItemID.DefendersForge], Utils.CenteredRectangle(new Vector2(shopx + 92f, shopy + 45f), Main.itemTexture[ItemID.DefendersForge].Size() * 0.65f), null, Color.White);
				if (walletCount > 0L) sb.Draw(Main.itemTexture[walletType], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 40f), Main.itemTexture[walletType].Size() * 0.5f));
				if (safeCount > 0L) sb.Draw(Main.itemTexture[ItemID.Safe], Utils.CenteredRectangle(new Vector2(shopx + 80f, shopy + 50f), Main.itemTexture[ItemID.Safe].Size() * 0.65f), null, Color.White);
				if (piggyCount > 0L) sb.Draw(Main.itemTexture[ItemID.PiggyBank], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 60f), Main.itemTexture[ItemID.PiggyBank].Size() * 0.65f), null, Color.White);
				Terraria.UI.ItemSlot.DrawMoney(sb, Language.GetTextValue("LegacyInterface.66"), shopx, shopy, Utils.CoinsSplit(combined), horizontal);
			}
		}

		private static int[] inventoryGlowTime;
		private static float[] inventoryGlowHue;
		private static int[] inventoryGlowTimeChest;
		private static float[] inventoryGlowHueChest;

		private static void ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
		{
			if (inventoryGlowTime == null) inventoryGlowTime = typeof(Terraria.UI.ItemSlot).GetValue<int[]>("inventoryGlowTime");
			if (inventoryGlowHue == null) inventoryGlowHue = typeof(Terraria.UI.ItemSlot).GetValue<float[]>("inventoryGlowHue");
			if (inventoryGlowTimeChest == null) inventoryGlowTimeChest = typeof(Terraria.UI.ItemSlot).GetValue<int[]>("inventoryGlowTimeChest");
			if (inventoryGlowHueChest == null) inventoryGlowHueChest = typeof(Terraria.UI.ItemSlot).GetValue<float[]>("inventoryGlowHueChest");

			Player player = Main.LocalPlayer;

			Item item = inv[slot];
			float inventoryScale = Main.inventoryScale;
			int drawMode = 0;
			Color color = Color.White;
			if (lightColor != Color.Transparent) color = lightColor;
			int linkpointNav = -1;
			bool flag = false;
			if (PlayerInput.UsingGamepadUI)
			{
				switch (context)
				{
					case 0:
					case 1:
					case 2:
						linkpointNav = slot;
						break;
					case 3:
					case 4:
						linkpointNav = 400 + slot;
						break;
					case 5:
						linkpointNav = 303;
						break;
					case 6:
						linkpointNav = 300;
						break;
					case 7:
						linkpointNav = 1500;
						break;
					case 8:
					case 9:
					case 10:
					case 11:
						linkpointNav = 100 + slot;
						break;
					case 12:
						if (inv == player.dye) linkpointNav = 120 + slot;

						if (inv == player.miscDyes) linkpointNav = 185 + slot;

						break;
					case 15:
						linkpointNav = 2700 + slot;
						break;
					case 16:
						linkpointNav = 184;
						break;
					case 17:
						linkpointNav = 183;
						break;
					case 18:
						linkpointNav = 182;
						break;
					case 19:
						linkpointNav = 180;
						break;
					case 20:
						linkpointNav = 181;
						break;
					case 22:
						if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig != -1) linkpointNav = 700 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeBig;

						if (UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall != -1) linkpointNav = 1500 + UILinkPointNavigator.Shortcuts.CRAFT_CurrentRecipeSmall + 1;

						break;
				}

				flag = UILinkPointNavigator.CurrentPoint == linkpointNav;
				if (context == 0)
				{
					drawMode = player.DpadRadial.GetDrawMode(slot);
					if (drawMode > 0 && !PlayerInput.CurrentProfile.UsingDpadHotbar()) drawMode = 0;
				}
			}

			Texture2D texture2D = Main.inventoryBackTexture;
			Color color2 = Main.inventoryBack;
			bool flag2 = false;

			if (item.type > 0 && item.stack > 0 && item.favorited && context != 13 && context != 21 && context != 22 && context != 14)
				texture2D = Main.inventoryBack10Texture;
			else if (item.type > 0 && item.stack > 0 && Terraria.UI.ItemSlot.Options.HighlightNewItems && item.newAndShiny && context != 13 && context != 21 && context != 14 && context != 22)
			{
				texture2D = Main.inventoryBack15Texture;
				float num3 = Main.mouseTextColor / 255f;
				num3 = num3 * 0.2f + 0.8f;
				color2 = color2.MultiplyRGBA(new Color(num3, num3, num3));
			}
			else if (PlayerInput.UsingGamepadUI && item.type > 0 && item.stack > 0 && drawMode != 0 && context != 13 && context != 21 && context != 22)
			{
				texture2D = Main.inventoryBack15Texture;
				float num4 = Main.mouseTextColor / 255f;
				num4 = num4 * 0.2f + 0.8f;
				color2 = color2.MultiplyRGBA(drawMode == 1 ? new Color(num4, num4 / 2f, num4 / 2f) : new Color(num4 / 2f, num4, num4 / 2f));
			}
			else if (context == 0 && slot < 10) texture2D = Main.inventoryBack9Texture;
			else if (context == 10 || context == 8 || context == 16 || context == 17 || context == 19 || context == 18 || context == 20) texture2D = Main.inventoryBack3Texture;
			else if (context == 11 || context == 9) texture2D = Main.inventoryBack8Texture;
			else if (context == 12) texture2D = Main.inventoryBack12Texture;
			else if (context == 3) texture2D = Main.inventoryBack5Texture;
			else if (context == 4) texture2D = Main.inventoryBack2Texture;
			else if (context == 7 || context == 5) texture2D = Main.inventoryBack4Texture;
			else if (context == 6) texture2D = Main.inventoryBack7Texture;
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
				flag2 = true;
			else if (context == 15)
				texture2D = Main.inventoryBack6Texture;
			else if (context == 22) texture2D = Main.inventoryBack4Texture;

			if (context == 0 && inventoryGlowTime[slot] > 0 && !inv[slot].favorited)
			{
				float scale = Main.invAlpha / 255f;
				Color value = new Color(63, 65, 151, 255) * scale;
				Color value2 = Main.hslToRgb(inventoryGlowHue[slot], 1f, 0.5f) * scale;
				float num5 = inventoryGlowTime[slot] / 300f;
				num5 *= num5;
				color2 = Color.Lerp(value, value2, num5 / 2f);
				texture2D = Main.inventoryBack13Texture;
			}

			if ((context == 4 || context == 3) && inventoryGlowTimeChest[slot] > 0 && !inv[slot].favorited)
			{
				float scale2 = Main.invAlpha / 255f;
				Color value3 = new Color(130, 62, 102, 255) * scale2;
				if (context == 3) value3 = new Color(104, 52, 52, 255) * scale2;

				Color value4 = Main.hslToRgb(inventoryGlowHueChest[slot], 1f, 0.5f) * scale2;
				float num6 = inventoryGlowTimeChest[slot] / 300f;
				num6 *= num6;
				color2 = Color.Lerp(value3, value4, num6 / 2f);
				texture2D = Main.inventoryBack13Texture;
			}

			if (flag)
			{
				texture2D = Main.inventoryBack14Texture;
				color2 = Color.White;
			}

			if (!flag2) spriteBatch.Draw(texture2D, position, null, color2, 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);

			int num7 = -1;
			switch (context)
			{
				case 8:
					if (slot == 0) num7 = 0;
					else if (slot == 1) num7 = 6;
					else if (slot == 2) num7 = 12;
					break;
				case 9:
					if (slot == 10) num7 = 3;
					else if (slot == 11) num7 = 9;
					else if (slot == 12) num7 = 15;
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
				Rectangle rectangle2 = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(texture2D3) : texture2D3.Frame();

				Color newColor = color;
				float num8 = 1f;
				Terraria.UI.ItemSlot.GetItemLight(ref newColor, ref num8, item);
				float num9 = 1f;
				if (rectangle2.Width > 32 || rectangle2.Height > 32)
				{
					if (rectangle2.Width > rectangle2.Height) num9 = 32f / rectangle2.Width;
					else num9 = 32f / rectangle2.Height;
				}

				num9 *= inventoryScale;
				Vector2 position2 = position + vector / 2f - rectangle2.Size() * num9 / 2f;
				Vector2 origin = rectangle2.Size() * (num8 / 2f - 0.5f);
				if (ItemLoader.PreDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(newColor), item.GetColor(color), origin, num9 * num8))
				{
					spriteBatch.Draw(texture2D3, position2, rectangle2, item.GetAlpha(newColor), 0f, origin, num9 * num8, SpriteEffects.None, 0f);
					if (item.color != Color.Transparent) spriteBatch.Draw(texture2D3, position2, rectangle2, item.GetColor(color), 0f, origin, num9 * num8, SpriteEffects.None, 0f);
				}

				ItemLoader.PostDrawInInventory(item, spriteBatch, position2, rectangle2, item.GetAlpha(newColor), item.GetColor(color), origin, num9 * num8);
				if (ItemID.Sets.TrapSigned[item.type]) spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f) * inventoryScale, new Rectangle(4, 58, 8, 8), color, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);

				if (item.stack > 1) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, item.stack.ToString(), position + new Vector2(10f, 26f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);

				int ammoCount = -1;
				if (context == 13)
				{
					if (item.DD2Summon)
					{
						for (int i = 0; i < 58; i++)
							if (inv[i].type == 3822)
								ammoCount += inv[i].stack;

						if (ammoCount >= 0) ammoCount++;
					}

					if (item.useAmmo > 0)
					{
						int useAmmo = item.useAmmo;
						ammoCount = 0;
						for (int j = 0; j < 58; j++)
							if (inv[j].ammo == useAmmo)
								ammoCount += inv[j].stack;

						ammoCount += player.inventory.OfType<BaseAmmoBag>().SelectMany(x => x.Handler.stacks).Where(x => x.ammo == useAmmo).Sum(x => x.stack);
					}

					if (item.fishingPole > 0)
					{
						ammoCount = 0;
						for (int k = 0; k < 58; k++)
							if (inv[k].bait > 0)
								ammoCount += inv[k].stack;
					}

					if (item.tileWand > 0)
					{
						int tileWand = item.tileWand;
						ammoCount = 0;
						for (int l = 0; l < 58; l++)
							if (inv[l].type == tileWand)
								ammoCount += inv[l].stack;
					}

					if (item.type == 509 || item.type == 851 || item.type == 850 || item.type == 3612 || item.type == 3625 || item.type == 3611)
					{
						ammoCount = 0;
						for (int m = 0; m < 58; m++)
							if (inv[m].type == 530)
								ammoCount += inv[m].stack;
					}
				}

				if (ammoCount != -1) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, ammoCount.ToString(), position + new Vector2(8f, 30f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale * 0.8f), -1f, inventoryScale);

				if (context == 13)
				{
					string text = string.Concat(slot + 1);
					if (text == "10") text = "0";

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
				string slotText = string.Concat(slot + 1);
				if (slotText == "10") slotText = "0";

				Color inventoryBack = Main.inventoryBack;
				int num12 = 0;
				if (Main.player[Main.myPlayer].selectedItem == slot)
				{
					num12 -= 3;
					inventoryBack.R = 255;
					inventoryBack.B = 0;
					inventoryBack.G = 210;
					inventoryBack.A = 100;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, slotText, position + new Vector2(6f, 4 + num12) * inventoryScale, inventoryBack, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
			}

			if (linkpointNav != -1) UILinkPointNavigator.SetPosition(linkpointNav, position + vector * 0.75f);
		}
	}
}