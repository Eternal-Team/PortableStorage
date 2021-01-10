// using System;
// using System.Linq;
// using BaseLibrary.UI;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Microsoft.Xna.Framework.Input;
// using Terraria;
// using Terraria.GameContent.Achievements;
// using Terraria.ID;
// using Terraria.ModLoader;
// using Terraria.UI;
// using Terraria.UI.Chat;
//
// namespace PortableStorage.UI
// {
// 	public class BuilderReservePanel : BaseBagPanel<BuilderReserve>
// 	{
// 		public BuilderReservePanel(BuilderReserve bag) : base(bag)
// 		{
// 			Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
// 			Height.Pixels = 40 + (SlotSize + SlotMargin) * Container.Storage.Count / 9;
//
// 			UIGrid<UIBuilderReserveSlot> gridItems = new UIGrid<UIBuilderReserveSlot>(9)
// 			{
// 				Width = { Percent = 100 },
// 				Height = { Pixels = -28, Percent = 100 },
// 				Y = { Pixels = 28 },
// 				Settings = { ItemMargin = SlotMargin }
// 			};
// 			Add(gridItems);
//
// 			for (int i = 0; i < Container.Storage.Count; i++)
// 			{
// 				UIBuilderReserveSlot slot = new UIBuilderReserveSlot(Container, i)
// 				{
// 					Width = { Pixels = SlotSize },
// 					Height = { Pixels = SlotSize }
// 				};
// 				gridItems.Add(slot);
// 			}
// 		}
//
// 		private class UIBuilderReserveSlot : BaseElement, IGridElement<UIBuilderReserveSlot>
// 		{
// 			private ItemHandler Handler => builderReserve.Handler;
//
// 			private Item Item
// 			{
// 				get => Handler.GetItemInSlot(slot);
// 				set => Handler.SetItemInSlot(slot, value);
// 			}
//
// 			public UIGrid<UIBuilderReserveSlot> Grid { get; set; }
//
// 			private BuilderReserve builderReserve;
//
// 			private int slot;
//
// 			public UIBuilderReserveSlot(BuilderReserve builderReserve, int slot = 0)
// 			{
// 				MinWidth = MinHeight = 40;
//
// 				this.slot = slot;
// 				this.builderReserve = builderReserve;
// 			}
//
// 			protected override void MouseClick(MouseButtonEventArgs args)
// 			{
// 				UIBuilderReserveSlot otherSlot = (UIBuilderReserveSlot)Grid.Children.FirstOrDefault(x => x is UIBuilderReserveSlot s && s.Item.type == Main.mouseItem.type);
// 				if (otherSlot != null && otherSlot != this && !otherSlot.Item.IsAir)
// 				{
// 					otherSlot.MouseClick(args);
// 					return;
// 				}
//
// 				if (Main.keyState.IsKeyDown(Keys.LeftAlt) && !Item.IsAir)
// 				{
// 					if (builderReserve.selectedIndex == slot) builderReserve.SetIndex(-1);
// 					else builderReserve.SetIndex(slot);
// 				}
// 				else
// 				{
// 					if (Handler.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
// 					{
// 						Item.newAndShiny = false;
// 						Player player = Main.LocalPlayer;
//
// 						if (ItemSlot.ShiftInUse)
// 						{
// 							ItemUtility.Loot(Handler, slot, Main.LocalPlayer);
// 							if (Item.IsAir && builderReserve.selectedIndex == slot) builderReserve.SetIndex(-1);
//
//
// 							return;
// 						}
//
// 						if (Main.mouseItem.IsAir)
// 						{
// 							Main.mouseItem = Handler.ExtractItem(slot, Item.maxStack);
// 							if (Item.IsAir && builderReserve.selectedIndex == slot) builderReserve.SetIndex(-1);
// 						}
// 						else
// 						{
// 							if (Item.IsTheSameAs(Main.mouseItem)) Main.mouseItem = Handler.InsertItem(slot, Main.mouseItem);
// 							else
// 							{
// 								if (builderReserve.selectedIndex == slot) builderReserve.SetIndex(-1);
//
// 								if (Item.stack <= Item.maxStack)
// 								{
// 									Item temp = Item;
// 									Utils.Swap(ref temp, ref Main.mouseItem);
// 									Item = temp;
// 								}
// 							}
// 						}
//
// 						if (Item.stack > 0) AchievementsHelper.NotifyItemPickup(player, Item);
//
// 						if (Main.mouseItem.type > 0 || Item.type > 0)
// 						{
// 							Recipe.FindRecipes();
// 							Main.PlaySound(SoundID.Grab);
// 						}
// 					}
// 				}
// 			}
//
// 			public override int CompareTo(BaseElement other) => slot.CompareTo(((UIBuilderReserveSlot)other).slot);
//
// 			protected override void Draw(SpriteBatch spriteBatch)
// 			{
// 				Texture2D texture = builderReserve.selectedIndex == slot ? Main.inventoryBack15Texture : Main.inventoryBackTexture;
//
// 				spriteBatch.DrawSlot(Dimensions, Color.White, texture);
//
// 				float scale = Math.Min(InnerDimensions.Width / texture.Width, InnerDimensions.Height / texture.Height);
//
// 				if (!Item.IsAir)
// 				{
// 					Texture2D itemTexture = Main.itemTexture[Item.type];
// 					Rectangle rect = Main.itemAnimations[Item.type] != null ? Main.itemAnimations[Item.type].GetFrame(itemTexture) : itemTexture.Frame();
// 					Color newColor = Color.White;
// 					float pulseScale = 1f;
// 					ItemSlot.GetItemLight(ref newColor, ref pulseScale, Item);
// 					int height = rect.Height;
// 					int width = rect.Width;
// 					float drawScale = 1f;
//
// 					float availableWidth = InnerDimensions.Width;
// 					if (width > availableWidth || height > availableWidth)
// 					{
// 						if (width > height) drawScale = availableWidth / width;
// 						else drawScale = availableWidth / height;
// 					}
//
// 					drawScale *= scale;
// 					Vector2 position = Dimensions.Position() + Dimensions.Size() * 0.5f;
// 					Vector2 origin = rect.Size() * 0.5f;
//
// 					if (ItemLoader.PreDrawInInventory(Item, spriteBatch, position, rect, Item.GetAlpha(newColor), Item.GetColor(Color.White), origin, drawScale * pulseScale))
// 					{
// 						spriteBatch.Draw(itemTexture, position, rect, Item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
// 						if (Item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position, rect, Item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
// 					}
//
// 					ItemLoader.PostDrawInInventory(Item, spriteBatch, position, rect, Item.GetAlpha(newColor), Item.GetColor(Color.White), origin, drawScale * pulseScale);
// 					if (ItemID.Sets.TrapSigned[Item.type]) spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
// 					if (Item.stack > 1)
// 					{
// 						string text = Item.stack < 1000 ? Item.stack.ToString() : Item.stack.ToSI("N1");
// 						ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, InnerDimensions.Position() + new Vector2(8, InnerDimensions.Height - Main.fontMouseText.MeasureString(text).Y * scale), Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
// 					}
//
// 					if (IsMouseHovering)
// 					{
// 						Main.LocalPlayer.showItemIcon = false;
// 						Main.ItemIconCacheUpdate(0);
// 						Main.HoverItem = Item.Clone();
// 						Main.hoverItemName = Main.HoverItem.Name;
//
// 						if (Main.keyState.IsKeyDown(Keys.LeftAlt)) BaseLibrary.Hooking.SetCursor("PortableStorage/Textures/Items/BuilderReserve");
// 						else if (ItemSlot.ShiftInUse) BaseLibrary.Hooking.SetCursor("Terraria/UI/Cursor_7");
// 					}
// 				}
// 			}
//
// 			protected override void MouseHeld(MouseButtonEventArgs args)
// 			{
// 				if (args.Button != MouseButton.Right) return;
//
// 				if (Handler.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
// 				{
// 					args.Handled = true;
//
// 					Player player = Main.LocalPlayer;
// 					Item.newAndShiny = false;
//
// 					if (Main.stackSplit <= 1)
// 					{
// 						if ((Main.mouseItem.IsTheSameAs(Item) || Main.mouseItem.type == 0) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0))
// 						{
// 							if (Main.mouseItem.type == 0)
// 							{
// 								Main.mouseItem = Item.Clone();
// 								Main.mouseItem.stack = 0;
// 								if (Item.favorited && Item.maxStack == 1) Main.mouseItem.favorited = true;
// 								Main.mouseItem.favorited = false;
// 							}
//
// 							Main.mouseItem.stack++;
// 							Handler.Shrink(slot, 1);
//
// 							Recipe.FindRecipes();
//
// 							Main.soundInstanceMenuTick.Stop();
// 							Main.soundInstanceMenuTick = Main.soundMenuTick.CreateInstance();
// 							Main.PlaySound(12);
//
// 							Main.stackSplit = Main.stackSplit == 0 ? 15 : Main.stackDelay;
// 						}
// 					}
// 				}
// 			}
// 		}
// 	}
// }

