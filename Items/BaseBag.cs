using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PortableStorage.Items
{
	public class NormalBagUI : UIState
	{
		public bool Visible => bag != null;

		public BaseBag bag;

		public void Open(BaseBag bag)
		{
			this.bag = bag;

			RemoveAllChildren();

			DragableUIPanel panel = new DragableUIPanel();
			panel.Width.Set(512f, 0f);
			panel.Height.Set(300f, 0f);
			panel.HAlign = panel.VAlign = 0.5f;
			Append(panel);

			UITextPanel<string> exit = new UITextPanel<string>("X");
			exit.Width.Set(30f, 0f);
			exit.Height.Set(30f, 0f);
			exit.HAlign = 1f;
			exit.OnClick += (evt, element) => { bag = null; };
			panel.Append(exit);

			for (int i = 0; i < bag.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag.Handler, i);
				slot.Left.Set(48f * (i % 9), 0f);
				slot.Top.Set(60f + i / 9 * 48f, 0f);
				panel.Append(slot);
			}
		}
	}

	public class UIContainerSlot : UIElement
	{
		public ItemHandler Handler;

		public Item Item
		{
			get => Handler.GetItemInSlot(slot);
			set => Handler.SetItemInSlot(slot, value);
		}

		public Texture2D backgroundTexture = TextureAssets.InventoryBack.Value;

		public Item PreviewItem;

		public bool ShortStackSize = false;

		public int slot;

		public UIContainerSlot(ItemHandler itemHandler, int slot = 0)
		{
			Width.Pixels = 44;
			Height.Pixels = 44;

			this.slot = slot;
			Handler = itemHandler;
		}

		public override void Click(UIMouseEvent evt)
		{
			if (Handler.IsItemValid?.Invoke(slot, Main.mouseItem) ?? true)
			{
				// args.Handled = true;

				Item.newAndShiny = false;
				Player player = Main.LocalPlayer;

				if (ItemSlot.ShiftInUse)
				{
					Main.LocalPlayer.Loot(Handler, slot);
					return;
				}

				if (Main.mouseItem.IsAir) Handler.ExtractItem(slot, out Main.mouseItem, Item.maxStack, true);
				else
				{
					if (Item.IsTheSameAs(Main.mouseItem)) Handler.InsertItem(slot, ref Main.mouseItem, true);
					else
					{
						if (Item.stack <= Item.maxStack)
						{
							Item temp = Item;
							Utils.Swap(ref temp, ref Main.mouseItem);
							Item = temp;
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

		private void DrawItem(SpriteBatch spriteBatch, Item item, float scale)
		{
			var Dimensions = GetDimensions().ToRectangle();
			CalculatedStyle InnerDimensions = GetInnerDimensions();

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
		
			string text = slot.ToString();
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
				FontAssets.ItemStack.Value, text, InnerDimensions.Position() + new Vector2(8, 0), Color.White, 0f, Vector2.Zero, new Vector2(0.85f), -1f, scale);

			
			if (item.stack > 1)
			{
				text = item.stack.ToString();
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
					FontAssets.ItemStack.Value, text, InnerDimensions.Position() + new Vector2(8, InnerDimensions.Height - FontAssets.ItemStack.Value.MeasureString(text).Y * scale), Color.White, 0f, Vector2.Zero, new Vector2(0.85f), -1f, scale);
			}

			if (IsMouseHovering)
			{
				Main.LocalPlayer.cursorItemIconEnabled = false;
				Main.ItemIconCacheUpdate(0);
				Main.HoverItem = item.Clone();
				Main.hoverItemName = Main.HoverItem.Name;

				// if (ItemSlot.ShiftInUse) BaseLibrary.Hooking.SetCursor("Terraria/UI/Cursor_7");
			}
		}

		public static void DrawSlot(SpriteBatch spriteBatch, Rectangle dimensions, Color? color = null, Texture2D texture = null)
		{
			if (texture == null) texture = TextureAssets.InventoryBack13.Value;

			Point point = new Point(dimensions.X, dimensions.Y);
			Point point2 = new Point(point.X + dimensions.Width - 8, point.Y + dimensions.Height - 8);
			int width = point2.X - point.X - 8;
			int height = point2.Y - point.Y - 8;

			Color value = color ?? UICommon.DefaultUIBlue;
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, 8, 8), new Rectangle(0, 0, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, 8, 8), new Rectangle(44, 0, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, 8, 8), new Rectangle(0, 44, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, 8, 8), new Rectangle(44, 44, 8, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point.Y, width, 8), new Rectangle(8, 0, 36, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point2.Y, width, 8), new Rectangle(8, 44, 36, 8), value);
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + 8, 8, height), new Rectangle(0, 8, 8, 36), value);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + 8, 8, height), new Rectangle(44, 8, 8, 36), value);
			spriteBatch.Draw(texture, new Rectangle(point.X + 8, point.Y + 8, width, height), new Rectangle(8, 8, 36, 36), value);
		}

		public static void DrawSlot(SpriteBatch spriteBatch, CalculatedStyle dimensions, Color? color = null, Texture2D texture = null)
		{
			DrawSlot(spriteBatch, dimensions.ToRectangle(), color, texture);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var Dimensions = GetDimensions().ToRectangle();
			CalculatedStyle InnerDimensions = GetInnerDimensions();

			DrawSlot(spriteBatch, Dimensions, Color.White, !Item.IsAir && Item.favorited ? TextureAssets.InventoryBack10.Value : backgroundTexture);

			float scale = Math.Min(InnerDimensions.Width / backgroundTexture.Width, InnerDimensions.Height / backgroundTexture.Height);

			if (!Item.IsAir) DrawItem(spriteBatch, Item, scale);
			// else if (PreviewItem != null && !PreviewItem.IsAir) spriteBatch.DrawWithEffect(BaseLibrary.BaseLibrary.DesaturateShader, () => DrawItem(spriteBatch, PreviewItem, scale));
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (!Main.mouseRight || !ContainsPoint(Main.MouseScreen)) return;

			Player player = Main.LocalPlayer;
			Item.newAndShiny = false;

			if (Main.stackSplit <= 1)
			{
				if ((Main.mouseItem.IsTheSameAs(Item) || Main.mouseItem.type == ItemID.None) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == ItemID.None))
				{
					if (Main.mouseItem.type == ItemID.None)
					{
						Main.mouseItem = Item.Clone();
						Main.mouseItem.stack = 0;
						if (Item.favorited && Item.maxStack == 1) Main.mouseItem.favorited = true;
						Main.mouseItem.favorited = false;
					}

					Main.mouseItem.stack++;
					Handler.Shrink(slot, 1, true);

					Recipe.FindRecipes();

					SoundEngine.PlaySound(12);
					ItemSlot.RefreshStackSplitCooldown();
				}
			}
		}
	}

	public abstract class BaseBag : BaseItem, IItemHandler
	{
		public Guid ID;
		public ItemHandler Handler;

		public override ModItem Clone(Item item)
		{
			BaseBag clone = (BaseBag)base.Clone(item);
			clone.Handler = Handler.Clone();
			clone.ID = ID;
			return clone;
		}

		public override void OnCreate(ItemCreationContext context)
		{
			ID = Guid.NewGuid();
			Handler = new ItemHandler(27);
		}

		public override void SetDefaults()
		{
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = ItemUseStyleID.Swing;
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(Mod, "test", ID.ToString()));
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			ModContent.GetInstance<PortableStorage>().bagState.Open(this);
		}

		public override TagCompound Save() => new TagCompound
		{
			["ID"] = ID,
			["Items"] = Handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			ID = tag.Get<Guid>("ID");
			Handler.Load(tag.GetCompound("Items"));
		}

		public bool InsertItem(int slot, ref Item item, bool user) => Handler.InsertItem(slot, ref item, user);

		public bool ExtractItem(int slot, out Item item, bool user) => Handler.ExtractItem(slot, out item, user: user);

		public ItemHandler GetItemHandler() => Handler;
	}
}