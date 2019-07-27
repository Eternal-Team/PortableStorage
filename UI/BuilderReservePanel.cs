using BaseLibrary;
using BaseLibrary.UI.Elements;
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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PortableStorage.UI
{
	public class BuilderReservePanel : BaseBagPanel<BuilderReserve>
	{
		private new UIGrid<UIBuilderReserveSlot> gridItems;

		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (40 + Container.Handler.Slots / 9 * 44, 0);
			this.Center();

			textLabel = new UIText(Container.DisplayName.GetTranslation())
			{
				HAlign = 0.5f
			};
			Append(textLabel);

			UIButton buttonLootAll = new UIButton(PortableStorage.textureLootAll)
			{
				Size = new Vector2(20)
			};
			buttonLootAll.OnClick += (evt, element) =>
			{
				ItemUtility.LootAll(Container.Handler, Main.LocalPlayer);
				if (Container.Handler.GetItemInSlot(Container.selectedIndex).IsAir) Container.SetIndex(-1);
			};
			buttonLootAll.GetHoverText += () => Language.GetText("LegacyInterface.29").ToString();
			Append(buttonLootAll);

			UIButton buttonDepositAll = new UIButton(PortableStorage.textureDepositAll)
			{
				Size = new Vector2(20),
				Left = (28, 0)
			};
			buttonDepositAll.OnClick += (evt, element) => ItemUtility.DepositAll(Container.Handler, Main.LocalPlayer);
			buttonDepositAll.GetHoverText += () => Language.GetText("LegacyInterface.30").ToString();
			Append(buttonDepositAll);

			buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(Container);
			Append(buttonClose);

			gridItems = new UIGrid<UIBuilderReserveSlot>(9)
			{
				Width = (0, 1),
				Height = (-28, 1),
				Top = (28, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIBuilderReserveSlot slot = new UIBuilderReserveSlot(Container, i);
				gridItems.Add(slot);
			}
		}
	}

	public class UIBuilderReserveSlot : BaseElement, IGridElement<UIBuilderReserveSlot>
	{
		public UIGrid<UIBuilderReserveSlot> Grid { get; set; }
		public ItemHandler Handler => builderReserve.Handler;

		private BuilderReserve builderReserve;

		public int slot;

		public Item Item
		{
			get => Handler.GetItemInSlot(slot);
			set => Handler.SetItemInSlot(slot, value);
		}

		public UIBuilderReserveSlot(BuilderReserve builderReserve, int slot = 0)
		{
			Width = Height = (40, 0);

			this.slot = slot;
			this.builderReserve = builderReserve;
		}

		public override void Click(UIMouseEvent evt)
		{
			UIBuilderReserveSlot otherSlot = Grid.items.FirstOrDefault(x => x.Item.type == Main.mouseItem.type);
			if (otherSlot != null && otherSlot != this && !otherSlot.Item.IsAir)
			{
				otherSlot.Click(evt);
				return;
			}

			if (Main.keyState.IsKeyDown(Keys.LeftAlt) && !Item.IsAir)
			{
				if (builderReserve.selectedIndex == slot) builderReserve.SetIndex(-1);
				else builderReserve.SetIndex(slot);
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
						if (Item.IsAir&&builderReserve.selectedIndex==slot) builderReserve.SetIndex(-1);

						base.Click(evt);

						return;
					}

					if (Main.mouseItem.IsAir)
					{
						Main.mouseItem = Handler.ExtractItem(slot, Item.maxStack);
						if (Item.IsAir && builderReserve.selectedIndex == slot) builderReserve.SetIndex(-1);
					}
					else
					{
						if (Item.IsTheSameAs(Main.mouseItem)) Main.mouseItem = Handler.InsertItem(slot, Main.mouseItem);
						else
						{
							if(builderReserve.selectedIndex == slot)builderReserve.SetIndex(-1);

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

		public override void RightClickContinuous(UIMouseEvent evt)
		{
			if (Handler.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
			{
				Player player = Main.LocalPlayer;
				Item.newAndShiny = false;

				if (player.itemAnimation > 0) return;

				if (Main.stackSplit <= 1 && Main.mouseRight)
				{
					if ((Main.mouseItem.IsTheSameAs(Item) || Main.mouseItem.type == 0) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0))
					{
						if (Main.mouseItem.type == 0)
						{
							Main.mouseItem = Item.Clone();
							Main.mouseItem.stack = 0;
							if (Item.favorited && Item.maxStack == 1) Main.mouseItem.favorited = true;
							Main.mouseItem.favorited = false;
						}

						Main.mouseItem.stack++;
						Handler.Shrink(slot, 1);

						Recipe.FindRecipes();

						Main.soundInstanceMenuTick.Stop();
						Main.soundInstanceMenuTick = Main.soundMenuTick.CreateInstance();
						Main.PlaySound(12);

						Main.stackSplit = Main.stackSplit == 0 ? 15 : Main.stackDelay;
					}
				}
			}
		}

		public override int CompareTo(object obj) => slot.CompareTo(((UIBuilderReserveSlot)obj).slot);

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			Texture2D texture = builderReserve.selectedIndex == slot ? Main.inventoryBack15Texture : Main.inventoryBackTexture;

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

					if (Main.keyState.IsKeyDown(Keys.LeftAlt)) BaseLibrary.Hooking.SetCursor("PortableStorage/Textures/Items/BuilderReserve");
					else if (ItemSlot.ShiftInUse) BaseLibrary.Hooking.SetCursor("Terraria/UI/Cursor_7");
				}
			}
		}
	}
}