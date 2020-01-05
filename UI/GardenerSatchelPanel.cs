using BaseLibrary;
using BaseLibrary.Input;
using BaseLibrary.UI.New;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items.Special;
using System;
using System.Linq;
using Terraria;
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
			Width.Pixels = 12 + (SlotSize + Padding) * 9;
			Height.Pixels = 44 + SlotSize;

			UIGrid<UIGardenerSatchelSlot> gridItems = new UIGrid<UIGardenerSatchelSlot>(9)
			{
				Width = { Percent = 100 },
				Height = { Pixels = -28, Percent = 100 },
				Y = { Pixels = 28 },
				ListPadding = Padding
			};
			Add(gridItems);

			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIGardenerSatchelSlot slot = new UIGardenerSatchelSlot(Container, i)
				{
					Width = { Pixels = SlotSize },
					Height = { Pixels = SlotSize }
				};
				gridItems.Add(slot);
			}
		}

		private class UIGardenerSatchelSlot : BaseElement, IGridElement<UIGardenerSatchelSlot>
		{
			public ItemHandler Handler => gardenerSatchel.Handler;

			public Item Item
			{
				get => Handler.GetItemInSlot(slot);
				set => Handler.SetItemInSlot(slot, value);
			}

			public UIGrid<UIGardenerSatchelSlot> Grid { get; set; }

			private GardenerSatchel gardenerSatchel;

			public int slot;

			public UIGardenerSatchelSlot(GardenerSatchel gardenerSatchel, int slot = 0)
			{
				Width.Pixels = Height.Pixels = 40;

				this.slot = slot;
				this.gardenerSatchel = gardenerSatchel;
			}

			protected override void MouseClick(MouseButtonEventArgs args)
			{
				UIGardenerSatchelSlot otherSlot = (UIGardenerSatchelSlot)Grid.Children.FirstOrDefault(x => x is UIGardenerSatchelSlot s && s.Item.type == Main.mouseItem.type);
				if (otherSlot != null && otherSlot != this && !otherSlot.Item.IsAir)
				{
					otherSlot.MouseClick(args);
					return;
				}

				if (Main.keyState.IsKeyDown(Keys.LeftAlt) && !Item.IsAir)
				{
					if (gardenerSatchel.selectedIndex == slot) gardenerSatchel.selectedIndex = -1;
					else gardenerSatchel.selectedIndex = slot;
				}
				else

				{
					if (Handler.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
					{
						Item.newAndShiny = false;
						Player player = Main.LocalPlayer;

						if (ItemSlot.ShiftInUse)
						{
							ItemUtility.Loot(Handler, slot, Main.LocalPlayer);
							if (Item.IsAir && gardenerSatchel.selectedIndex == slot) gardenerSatchel.selectedIndex = -1;

							base.MouseClick(args);

							return;
						}

						if (Main.mouseItem.IsAir)
						{
							Main.mouseItem = Handler.ExtractItem(slot, Item.maxStack);
							if (Item.IsAir && gardenerSatchel.selectedIndex == slot) gardenerSatchel.selectedIndex = -1;
						}
						else
						{
							if (Item.IsTheSameAs(Main.mouseItem)) Main.mouseItem = Handler.InsertItem(slot, Main.mouseItem);
							else
							{
								if (gardenerSatchel.selectedIndex == slot) gardenerSatchel.selectedIndex = -1;

								if (Item.stack <= Item.maxStack)
								{
									Item temp = Item;
									Utils.Swap(ref temp, ref Main.mouseItem);
									Item = temp;
								}
							}
						}

						if (Item.stack > 0) AchievementsHelper.NotifyItemPickup(player, Item);

						if (Main.mouseItem.type > 0 || Item.type > 0)
						{
							Recipe.FindRecipes();
							Main.PlaySound(SoundID.Grab);
						}
					}
				}
			}

			public override int CompareTo(BaseElement other) => slot.CompareTo(((UIGardenerSatchelSlot)other).slot);


			protected override void Draw(SpriteBatch spriteBatch)
			{
				Texture2D texture = gardenerSatchel.selectedIndex == slot ? Main.inventoryBack15Texture : Main.inventoryBackTexture;

				spriteBatch.DrawSlot(Dimensions, Color.White, texture);

				float scale = Math.Min(InnerDimensions.Width / texture.Width, InnerDimensions.Height / texture.Height);

				if (!Item.IsAir)
				{
					Texture2D itemTexture = Main.itemTexture[Item.type];
					Rectangle rect = Main.itemAnimations[Item.type] != null ? Main.itemAnimations[Item.type].GetFrame(itemTexture) : itemTexture.Frame();
					Color newColor = Color.White;
					float pulseScale = 1f;
					ItemSlot.GetItemLight(ref newColor, ref pulseScale, Item);
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
					Vector2 position = Dimensions.Position() + Dimensions.Size() * 0.5f;
					Vector2 origin = rect.Size() * 0.5f;

					if (ItemLoader.PreDrawInInventory(Item, spriteBatch, position, rect, Item.GetAlpha(newColor), Item.GetColor(Color.White), origin, drawScale * pulseScale))
					{
						spriteBatch.Draw(itemTexture, position, rect, Item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
						if (Item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, Item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
					}

					ItemLoader.PostDrawInInventory(Item, spriteBatch, position, rect, Item.GetAlpha(newColor), Item.GetColor(Color.White), origin, drawScale * pulseScale);
					if (ItemID.Sets.TrapSigned[Item.type]) spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
					if (Item.stack > 1)
					{
						string text = Item.stack < 1000 ? Item.stack.ToString() : Item.stack.ToSI("N1");
						ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, InnerDimensions.Position() + new Vector2(8, InnerDimensions.Height - Main.fontMouseText.MeasureString(text).Y * scale), Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
					}

					if (IsMouseHovering)
					{
						Main.LocalPlayer.showItemIcon = false;
						Main.ItemIconCacheUpdate(0);
						Main.HoverItem = Item.Clone();
						Main.hoverItemName = Main.HoverItem.Name;

						//if (Main.keyState.IsKeyDown(Keys.LeftAlt))
						//	BaseLibrary.Hooking.SetCursor("PortableStorage/Textures/Items/GardenerSatchel");
						/*else*/
						if (ItemSlot.ShiftInUse) BaseLibrary.Hooking.SetCursor("Terraria/UI/Cursor_7");
					}
				}
			}

			//public override void RightClickContinuous(UIMouseEvent evt)
			//{
			//	if (Handler.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
			//	{
			//		Player player = Main.LocalPlayer;
			//		Item.newAndShiny = false;

			//		if (player.itemAnimation > 0) return;

			//		if (Main.stackSplit <= 1 && Main.mouseRight)
			//		{
			//			if ((Main.mouseItem.IsTheSameAs(Item) || Main.mouseItem.type == 0) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0))
			//			{
			//				if (Main.mouseItem.type == 0)
			//				{
			//					Main.mouseItem = Item.Clone();
			//					Main.mouseItem.stack = 0;
			//					if (Item.favorited && Item.maxStack == 1) Main.mouseItem.favorited = true;
			//					Main.mouseItem.favorited = false;
			//				}

			//				Main.mouseItem.stack++;
			//				Handler.Shrink(slot, 1);

			//				Recipe.FindRecipes();

			//				Main.soundInstanceMenuTick.Stop();
			//				Main.soundInstanceMenuTick = Main.soundMenuTick.CreateInstance();
			//				Main.PlaySound(12);

			//				Main.stackSplit = Main.stackSplit == 0 ? 15 : Main.stackDelay;
			//			}
			//		}
			//	}
			//}
		}
	}
}