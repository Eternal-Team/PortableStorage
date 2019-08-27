using BaseLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace PortableStorage
{
	public static partial class Hooking
	{
		private static Item[] ItemTextBags;

		private static void ItemText_Update(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			
			if (cursor.TryGotoNext(i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<ItemText>("active")))
			{
				cursor.Index += 3;

				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.EmitDelegate<Action<ItemText>>(itemText =>
				{
					int index = Array.FindLastIndex(Main.itemText, text => text == itemText);

					if (index != -1) ItemTextBags[index] = new Item();
				});
			}
		}

		private static string ValueToName(int coinValue)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int i = coinValue;
			while (i > 0)
			{
				if (i >= 1000000)
				{
					i -= 1000000;
					num++;
				}
				else if (i >= 10000)
				{
					i -= 10000;
					num2++;
				}
				else if (i >= 100)
				{
					i -= 100;
					num3++;
				}
				else if (i >= 1)
				{
					i--;
					num4++;
				}
			}

			string text = "";
			if (num > 0)
			{
				text = text + num + $" {Language.GetTextValue("Currency.Platinum")} ";
			}

			if (num2 > 0)
			{
				text = text + num2 + $" {Language.GetTextValue("Currency.Gold")} ";
			}

			if (num3 > 0)
			{
				text = text + num3 + $" {Language.GetTextValue("Currency.Silver")} ";
			}

			if (num4 > 0)
			{
				text = text + num4 + $" {Language.GetTextValue("Currency.Copper")} ";
			}

			if (text.Length > 1)
			{
				text = text.Substring(0, text.Length - 1);
			}

			return text;
		}

		private static void ValueToName(ItemText itemText)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int i = itemText.coinValue;
			while (i > 0)
			{
				if (i >= 1000000)
				{
					i -= 1000000;
					num++;
				}
				else if (i >= 10000)
				{
					i -= 10000;
					num2++;
				}
				else if (i >= 100)
				{
					i -= 100;
					num3++;
				}
				else if (i >= 1)
				{
					i--;
					num4++;
				}
			}

			itemText.name = "";
			if (num > 0)
			{
				itemText.name = itemText.name + num + string.Format(" {0} ", Language.GetTextValue("Currency.Platinum"));
			}

			if (num2 > 0)
			{
				itemText.name = itemText.name + num2 + string.Format(" {0} ", Language.GetTextValue("Currency.Gold"));
			}

			if (num3 > 0)
			{
				itemText.name = itemText.name + num3 + string.Format(" {0} ", Language.GetTextValue("Currency.Silver"));
			}

			if (num4 > 0)
			{
				itemText.name = itemText.name + num4 + string.Format(" {0} ", Language.GetTextValue("Currency.Copper"));
			}

			if (itemText.name.Length > 1)
			{
				itemText.name = itemText.name.Substring(0, itemText.name.Length - 1);
			}
		}

		internal static void BagItemText(Item bag, Item newItem, int stack, bool noStack, bool longText)
		{
			bool flag = newItem.type >= 71 && newItem.type <= 74;
			if (!Main.showItemText) return;
			if (newItem.Name == null || !newItem.active) return;
			if (Main.netMode == NetmodeID.Server) return;
			for (int i = 0; i < 20; i++)
			{
				ItemText itemText = Main.itemText[i];

				if (itemText.active && ItemTextBags[i] == bag && (itemText.name == newItem.AffixName() || flag && itemText.coinText) && !itemText.NoStack && !noStack)
				{
					string text = $"{newItem.Name} ({itemText.stack + stack})";
					string text2 = newItem.Name;
					if (itemText.stack > 1)
					{
						object obj = text2;
						text2 = $"{obj} ({itemText.stack})";
					}

					Vector2 vector = Main.fontMouseText.MeasureString(text) + new Vector2(24f, 0f);

					if (itemText.lifeTime < 0) itemText.scale = 1f;
					if (itemText.lifeTime < 60) itemText.lifeTime = 60;
					if (flag && itemText.coinText)
					{
						int coinValue = 0;
						if (newItem.type == 71) coinValue += newItem.stack;
						else if (newItem.type == 72) coinValue += 100 * newItem.stack;
						else if (newItem.type == 73) coinValue += 10000 * newItem.stack;
						else if (newItem.type == 74) coinValue += 1000000 * newItem.stack;
						itemText.coinValue += coinValue;
						text = ValueToName(itemText.coinValue);
						vector = Main.fontMouseText.MeasureString(text) + new Vector2(24f, 0f);

						itemText.name = text;
						if (itemText.coinValue >= 1000000)
						{
							if (itemText.lifeTime < 300) itemText.lifeTime = 300;
							itemText.color = new Color(220, 220, 198);
						}
						else if (itemText.coinValue >= 10000)
						{
							if (itemText.lifeTime < 240) itemText.lifeTime = 240;
							itemText.color = new Color(224, 201, 92);
						}
						else if (itemText.coinValue >= 100)
						{
							if (itemText.lifeTime < 180) itemText.lifeTime = 180;
							itemText.color = new Color(181, 192, 193);
						}
						else if (itemText.coinValue >= 1)
						{
							if (itemText.lifeTime < 120) itemText.lifeTime = 120;
							itemText.color = new Color(246, 138, 96);
						}
					}

					itemText.stack += stack;
					itemText.scale = 0f;
					itemText.rotation = 0f;
					itemText.position.X = newItem.position.X + newItem.width * 0.5f - vector.X * 0.5f;
					itemText.position.Y = newItem.position.Y + newItem.height * 0.25f - vector.Y * 0.5f;
					itemText.velocity.Y = -7f;
					if (itemText.coinText)
					{
						itemText.stack = 1;
					}

					return;
				}
			}

			int emptyIndex = -1;
			for (int j = 0; j < 20; j++)
			{
				if (!Main.itemText[j].active)
				{
					emptyIndex = j;
					break;
				}
			}

			if (emptyIndex == -1)
			{
				double num3 = Main.bottomWorld;
				for (int k = 0; k < 20; k++)
				{
					if (num3 > Main.itemText[k].position.Y)
					{
						emptyIndex = k;
						num3 = Main.itemText[k].position.Y;
					}
				}
			}

			if (emptyIndex >= 0)
			{
				ItemText itemText = Main.itemText[emptyIndex];

				string text3 = newItem.AffixName();
				if (stack > 1)
				{
					object obj2 = text3;
					text3 = string.Concat(obj2, " (", stack, ")");
				}

				Vector2 vector2 = Main.fontMouseText.MeasureString(text3) + new Vector2(24f, 0f);
				itemText.alpha = 1f;
				itemText.alphaDir = -1;
				itemText.active = true;
				itemText.scale = 0f;
				itemText.NoStack = noStack;
				itemText.rotation = 0f;
				itemText.position.X = newItem.position.X + newItem.width * 0.5f - vector2.X * 0.5f;
				itemText.position.Y = newItem.position.Y + newItem.height * 0.25f - vector2.Y * 0.5f;
				itemText.color = Color.White;
				ItemTextBags[emptyIndex] = bag;

				if (newItem.rare == 1)
				{
					itemText.color = new Color(150, 150, 255);
				}
				else if (newItem.rare == 2)
				{
					itemText.color = new Color(150, 255, 150);
				}
				else if (newItem.rare == 3)
				{
					itemText.color = new Color(255, 200, 150);
				}
				else if (newItem.rare == 4)
				{
					itemText.color = new Color(255, 150, 150);
				}
				else if (newItem.rare == 5)
				{
					itemText.color = new Color(255, 150, 255);
				}
				else if (newItem.rare == -11)
				{
					itemText.color = new Color(255, 175, 0);
				}
				else if (newItem.rare == -1)
				{
					itemText.color = new Color(130, 130, 130);
				}
				else if (newItem.rare == 6)
				{
					itemText.color = new Color(210, 160, 255);
				}
				else if (newItem.rare == 7)
				{
					itemText.color = new Color(150, 255, 10);
				}
				else if (newItem.rare == 8)
				{
					itemText.color = new Color(255, 255, 10);
				}
				else if (newItem.rare == 9)
				{
					itemText.color = new Color(5, 200, 255);
				}
				else if (newItem.rare == 10)
				{
					itemText.color = new Color(255, 40, 100);
				}
				else if (newItem.rare >= 11)
				{
					itemText.color = new Color(180, 40, 255);
				}

				itemText.expert = newItem.expert;
				itemText.name = newItem.AffixName();
				itemText.stack = stack;
				itemText.velocity.Y = -7f;
				itemText.lifeTime = 60;
				if (longText)
				{
					itemText.lifeTime *= 5;
				}

				itemText.coinValue = 0;
				itemText.coinText = newItem.type >= 71 && newItem.type <= 74;
				if (itemText.coinText)
				{
					if (newItem.type == 71)
					{
						itemText.coinValue += itemText.stack;
					}
					else if (newItem.type == 72)
					{
						itemText.coinValue += 100 * itemText.stack;
					}
					else if (newItem.type == 73)
					{
						itemText.coinValue += 10000 * itemText.stack;
					}
					else if (newItem.type == 74)
					{
						itemText.coinValue += 1000000 * itemText.stack;
					}

					ValueToName(itemText);
					itemText.stack = 1;
					int num4 = emptyIndex;
					if (Main.itemText[num4].coinValue >= 1000000)
					{
						if (Main.itemText[num4].lifeTime < 300)
						{
							Main.itemText[num4].lifeTime = 300;
						}

						Main.itemText[num4].color = new Color(220, 220, 198);
						return;
					}

					if (Main.itemText[num4].coinValue >= 10000)
					{
						if (Main.itemText[num4].lifeTime < 240)
						{
							Main.itemText[num4].lifeTime = 240;
						}

						Main.itemText[num4].color = new Color(224, 201, 92);
						return;
					}

					if (Main.itemText[num4].coinValue >= 100)
					{
						if (Main.itemText[num4].lifeTime < 180)
						{
							Main.itemText[num4].lifeTime = 180;
						}

						Main.itemText[num4].color = new Color(181, 192, 193);
						return;
					}

					if (Main.itemText[num4].coinValue >= 1)
					{
						if (Main.itemText[num4].lifeTime < 120)
						{
							Main.itemText[num4].lifeTime = 120;
						}

						Main.itemText[num4].color = new Color(246, 138, 96);
					}
				}
			}
		}
		
		private static void Main_DoDraw(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(0), i => i.MatchStloc(130), i => i.MatchBr(out _)))
			{
				cursor.EmitDelegate<Action>(() =>
				{
					float scale = ItemText.TargetScale;
					if (scale == 0f) scale = 1f;

					for (int i = 0; i < Main.itemText.Length; i++)
					{
						ItemText itemText = Main.itemText[i];

						if (itemText.active)
						{
							string text = itemText.name;
							if (itemText.stack > 1) text = $"{text} ({itemText.stack})";

							Vector2 size = Main.fontMouseText.MeasureString(text);
							Vector2 origin = size * 0.5f;
							float num79 = itemText.scale / scale;
							float r = itemText.color.R;
							float g = itemText.color.G;
							float b = itemText.color.B;
							float a = itemText.color.A;
							r *= num79 * itemText.alpha * 0.3f;
							g *= num79 * itemText.alpha * 0.3f;
							b *= num79 * itemText.alpha * 0.3f;
							a *= num79 * itemText.alpha;
							Color color = new Color((int)r, (int)g, (int)b, (int)a);
							for (int num84 = 0; num84 < 5; num84++)
							{
								float num85 = 0f;
								float num86 = 0f;
								if (num84 == 0)
								{
									num85 -= scale * 2f;
								}
								else if (num84 == 1)
								{
									num85 += scale * 2f;
								}
								else if (num84 == 2)
								{
									num86 -= scale * 2f;
								}
								else if (num84 == 3)
								{
									num86 += scale * 2f;
								}
								else
								{
									r = itemText.color.R * num79 * itemText.alpha;
									b = itemText.color.B * num79 * itemText.alpha;
									g = itemText.color.G * num79 * itemText.alpha;
									a = itemText.color.A * num79 * itemText.alpha;
									color = new Color((int)r, (int)g, (int)b, (int)a);
								}

								if (num84 < 4)
								{
									a = itemText.color.A * num79 * itemText.alpha;
									color = new Color(0, 0, 0, (int)a);
								}

								float num87 = itemText.position.Y - Main.screenPosition.Y + num86;
								if (Main.player[Main.myPlayer].gravDir == -1f)
								{
									num87 = Main.screenHeight - num87;
								}

								Main.spriteBatch.DrawString(Main.fontMouseText, text, new Vector2(itemText.position.X - Main.screenPosition.X + num85 + origin.X, num87 + origin.Y), color, itemText.rotation, origin, itemText.scale, SpriteEffects.None, 0f);
							}

							if (!ItemTextBags[i].IsAir)
							{
								Texture2D texture = ItemTextBags[i].GetTexture();
								float texScale = Math.Min(20f / texture.Width, 20f / texture.Height);

								float x = itemText.position.X - Main.screenPosition.X - 10f - 4f;
								float y = itemText.position.Y - Main.screenPosition.Y + 10f;
								if (Main.player[Main.myPlayer].gravDir == -1f) y = Main.screenHeight - y;

								Main.spriteBatch.Draw(texture, new Vector2(x, y), null, Color.White * num79 * itemText.alpha, 0f, texture.Size() * 0.5f, texScale * itemText.scale, SpriteEffects.None, 0f);
							}
						}
					}
				});

				cursor.Emit(OpCodes.Br, label);
			}

			if (cursor.TryGotoNext(i => i.MatchLdsfld<Main>("netMode"))) cursor.MarkLabel(label);
		}
	}
}