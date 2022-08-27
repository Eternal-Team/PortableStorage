using System;
using System.Linq;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PortableStorage.UI
{
	public class GardenerSatchelPanel : BaseBagPanel<GardenerSatchel>
	{
		public GardenerSatchelPanel(GardenerSatchel bag) : base(bag)
		{
			Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
			Height.Pixels = 40 + (SlotSize + SlotMargin) * Container.GetItemStorage().Count / 9;

			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = { Percent = 100 },
				Height = { Pixels = -28, Percent = 100 },
				Y = { Pixels = 28 },
				Settings = { ItemMargin = SlotMargin }
			};
			Add(gridItems);

			for (int i = 0; i < Container.GetItemStorage().Count; i++)
			{
				UIGardenerSatchelSlot slot = new UIGardenerSatchelSlot(Container, i)
				{
					Width = { Pixels = SlotSize },
					Height = { Pixels = SlotSize }
				};
				gridItems.Add(slot);
			}
		}

		private class UIGardenerSatchelSlot : BaseElement
		{
			public UIContainerSlotSettings Settings = UIContainerSlotSettings.Default;

			private GardenerSatchel gardenerSatchel;
			private ItemStorage storage;
			private int slot;

			public Item Item => storage[slot];

			public UIGardenerSatchelSlot(GardenerSatchel gardenerSatchel, int slot)
			{
				Width.Pixels = 44;
				Height.Pixels = 44;

				this.slot = slot;
				this.gardenerSatchel = gardenerSatchel;
				storage = gardenerSatchel.GetItemStorage();
			}

			protected override void MouseDown(MouseButtonEventArgs args)
			{
				if (args.Button != MouseButton.Left) return;

				args.Handled = true;

				UIGardenerSatchelSlot otherSlot = (UIGardenerSatchelSlot)Parent.Children.FirstOrDefault(x => x is UIGardenerSatchelSlot s && s.Item.type == Main.mouseItem.type);
				if (otherSlot != null && otherSlot != this && !otherSlot.Item.IsAir)
				{
					otherSlot.MouseDown(args);
					return;
				}

				if (Main.keyState.IsKeyDown(Keys.LeftAlt) && !Item.IsAir)
				{
					if (gardenerSatchel.SelectedIndex == slot) gardenerSatchel.SelectedIndex = -1;
					else gardenerSatchel.SelectedIndex = slot;
				}
				else
				{
					if (storage.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
					{
						Item.newAndShiny = false;
						Player player = Main.LocalPlayer;

						if (ItemSlot.ShiftInUse)
						{
							Main.LocalPlayer.Loot(storage, slot);
							return;
						}

						if (Main.mouseItem.IsAir) storage.RemoveItem(Main.LocalPlayer, slot, out Main.mouseItem, Item.maxStack);
						else
						{
							if (Item.type == Main.mouseItem.type) storage.InsertItem(Main.LocalPlayer, slot, ref Main.mouseItem);
							else
							{
								if (Item.stack <= Item.maxStack)
								{
									storage.SwapStacks(Main.LocalPlayer, slot, ref Main.mouseItem);
								}
							}
						}

						if (Item.stack > 0) AchievementsHelper.NotifyItemPickup(player, Item);

						if (Main.mouseItem.type > ItemID.None || Item.type > ItemID.None)
						{
							Recipe.FindRecipes();
							SoundEngine.PlaySound(SoundID.Grab);
						}
					}
				}
			}

			public override int CompareTo(BaseElement other) => other is UIGardenerSatchelSlot uiSlot ? slot.CompareTo(uiSlot.slot) : 0;

			private void DrawItem(SpriteBatch spriteBatch, Item item, float scale)
			{
				Main.instance.LoadItem(item.type);
				Texture2D itemTexture = TextureAssets.Item[item.type].Value;

				Rectangle rect = Main.itemAnimations[item.type] != null ? Main.itemAnimations[item.type].GetFrame(itemTexture) : itemTexture.Frame();
				Color newColor = Color.White;
				float pulseScale = 1f;
				ItemSlot.GetItemLight(ref newColor, ref pulseScale, item);
				int height = rect.Height;
				int width = rect.Width;
				float drawScale = 1f;

				float availableWidth = InnerDimensions.Width;
				if (width > availableWidth || height > availableWidth)
				{
					if (width > height) drawScale = availableWidth / width;
					else drawScale = availableWidth / height;
				}

				drawScale *= scale;
				Vector2 position = Dimensions.TopLeft() + Dimensions.Size() * 0.5f;
				Vector2 origin = rect.Size() * 0.5f;

				if (ItemLoader.PreDrawInInventory(item, spriteBatch, position - rect.Size() * 0.5f * drawScale, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, drawScale * pulseScale))
				{
					spriteBatch.Draw(itemTexture, position, rect, item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
					if (item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
				}

				ItemLoader.PostDrawInInventory(item, spriteBatch, position - rect.Size() * 0.5f * drawScale, rect, item.GetAlpha(newColor), item.GetColor(Color.White), origin, drawScale * pulseScale);
				if (ItemID.Sets.TrapSigned[item.type]) spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
				if (item.stack > 1)
				{
					string text = !Settings.ShortStackSize || item.stack < 1000 ? item.stack.ToString() : TextUtility.ToSI(item.stack, "N1");
					float texscale = 0.75f;
					// note: i dont think this will scale well with larger slot sizes
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, InnerDimensions.TopLeft() + new Vector2(8, InnerDimensions.Height - FontAssets.MouseText.Value.MeasureString(text).Y * texscale), Color.White, 0f, Vector2.Zero, new Vector2(texscale));
				}

				if (IsMouseHovering)
				{
					Main.LocalPlayer.cursorItemIconEnabled = false;
					Main.ItemIconCacheUpdate(0);
					Main.HoverItem = item.Clone();
					Main.hoverItemName = Main.HoverItem.Name;

					if (ItemSlot.ShiftInUse) CustomCursor.CustomCursor.SetCursor("Terraria/Images/UI/Cursor_7");
				}
			}

			protected override void Draw(SpriteBatch spriteBatch)
			{
				var texture = gardenerSatchel.SelectedIndex == slot ? TextureAssets.InventoryBack15.Value : Settings.SlotTexture;
				DrawingUtility.DrawSlot(spriteBatch, Dimensions, texture, Color.White);

				float scale = Math.Min(InnerDimensions.Width / (float)texture.Width, InnerDimensions.Height / (float)texture.Height);

				if (!Item.IsAir) DrawItem(spriteBatch, Item, scale);
				// else if (options.GhostItem != null && !options.GhostItem.IsAir) spriteBatch.DrawWithEffect(BaseLibrary.BaseLibrary.DesaturateShader, () => DrawItem(spriteBatch, options.GhostItem, scale));
			}

			protected override void MouseHeld(MouseButtonEventArgs args)
			{
				if (args.Button != MouseButton.Right) return;

				args.Handled = true;

				if (storage.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
				{
					Item.newAndShiny = false;

					if (Main.stackSplit <= 1)
					{
						if ((Main.mouseItem.type == Item.type || Main.mouseItem.type == ItemID.None) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == ItemID.None))
						{
							if (Main.mouseItem.type == ItemID.None)
							{
								Main.mouseItem = Item.Clone();
								Main.mouseItem.stack = 0;
								if (Item.favorited && Item.maxStack == 1) Main.mouseItem.favorited = true;
								Main.mouseItem.favorited = false;
							}

							Main.mouseItem.stack++;
							storage.ModifyStackSize(Main.LocalPlayer, slot, -1);

							SoundEngine.PlaySound(SoundID.MenuTick);
							ItemSlot.RefreshStackSplitCooldown();
						}
					}
				}
			}

			protected override void MouseScroll(MouseScrollEventArgs args)
			{
				if (!Main.keyState.IsKeyDown(Keys.LeftAlt)) return;

				args.Handled = true;

				if (args.OffsetY > 0)
				{
					if (Main.mouseItem.type == Item.type && Main.mouseItem.stack < Main.mouseItem.maxStack)
					{
						Main.mouseItem.stack++;
						storage.ModifyStackSize(Main.LocalPlayer, slot, -1);
					}
					else if (Main.mouseItem.IsAir)
					{
						Main.mouseItem = Item.Clone();
						Main.mouseItem.stack = 1;
						storage.ModifyStackSize(Main.LocalPlayer, slot, -1);
					}
				}
				else if (args.OffsetY < 0)
				{
					if (Item.type == Main.mouseItem.type && storage.ModifyStackSize(Main.LocalPlayer, slot, 1))
					{
						if (--Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();
					}
					else if (Item.IsAir)
					{
						Item cloned = Main.mouseItem.Clone();
						cloned.stack = 1;
						storage.InsertItem(Main.LocalPlayer, slot, ref cloned);
						if (--Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();
					}
				}
			}
		}
	}
}