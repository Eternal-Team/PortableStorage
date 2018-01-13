//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using PortableStorage.Items;
//using System;
//using System.Linq;
//using Microsoft.Xna.Framework.Input;
//using Terraria;
//using Terraria.GameContent.UI.Elements;
//using Terraria.ID;
//using Terraria.Localization;
//using Terraria.ModLoader;
//using Terraria.UI;
//using Terraria.UI.Chat;
//using TheOneLibrary.Base;
//using TheOneLibrary.Base.UI;
//using TheOneLibrary.Base.UI.Elements;
//using TheOneLibrary.UI.Elements;
//using TheOneLibrary.Utility;

//namespace PortableStorage.UI
//{
//	public class DevNullUI : BaseUI
//	{
//		public UIText textLabel = new UIText("/dev/null");

//		public UIHoverButton buttonQuickStack = new UIHoverButton(Main.chestStackTexture);
//		public UIButton buttonLootAll = new UIButton(PortableStorage.lootAll);
//		public UIButton buttonDepositAll = new UIButton(PortableStorage.depositAll);
//		public UITextButton buttonClose = new UITextButton("X", 4);

//		public UIGrid gridItems = new UIGrid(9);

//		public DevNull devNull;

//		public override void OnInitialize()
//		{
//			panelMain.Width.Pixels = 408;
//			panelMain.Height.Pixels = 172;
//			panelMain.Center();
//			panelMain.SetPadding(0);
//			panelMain.BackgroundColor = panelColor;
//			panelMain.OnMouseDown += DragStart;
//			panelMain.OnMouseUp += DragEnd;
//			Append(panelMain);

//			textLabel.HAlign = 0.5f;
//			textLabel.Top.Pixels = 8;
//			panelMain.Append(textLabel);

//			buttonQuickStack.Width.Pixels = 24;
//			buttonQuickStack.Height.Pixels = 24;
//			buttonQuickStack.Left.Pixels = 8;
//			buttonQuickStack.Top.Pixels = 8;
//			buttonQuickStack.HoverText = Language.GetTextValue("GameUI.QuickStackToNearby");
//			buttonQuickStack.OnClick += QuickStackClick;
//			panelMain.Append(buttonQuickStack);

//			buttonLootAll.Width.Pixels = 24;
//			buttonLootAll.Height.Pixels = 24;
//			buttonLootAll.Left.Pixels = 40;
//			buttonLootAll.Top.Pixels = 8;
//			buttonLootAll.HoverText = Language.GetTextValue("LegacyInterface.29");
//			buttonLootAll.OnClick += LootAllClick;
//			panelMain.Append(buttonLootAll);

//			buttonDepositAll.Width.Pixels = 24;
//			buttonDepositAll.Height.Pixels = 24;
//			buttonDepositAll.Left.Pixels = 72;
//			buttonDepositAll.Top.Pixels = 8;
//			buttonDepositAll.HoverText = Language.GetTextValue("LegacyInterface.30");
//			buttonDepositAll.OnClick += DepositAllClick;
//			panelMain.Append(buttonDepositAll);

//			buttonClose.Width.Pixels = 24;
//			buttonClose.Height.Pixels = 24;
//			buttonClose.Left.Set(-28, 1);
//			buttonClose.Top.Pixels = 8;
//			buttonClose.OnClick += (evt, element) =>
//			{
//				PortableStorage.Instance.BagUI.Remove(devNull.guid);
//				Main.PlaySound(SoundID.Item59.WithVolume(0.5f));
//			};
//			panelMain.Append(buttonClose);

//			gridItems.Width.Set(-16, 1);
//			gridItems.Height.Set(-44, 1);
//			gridItems.Left.Pixels = 8;
//			gridItems.Top.Pixels = 36;
//			gridItems.ListPadding = 4;
//			gridItems.OverflowHidden = true;
//			panelMain.Append(gridItems);
//		}

//		private void LootAllClick(UIMouseEvent evt, UIElement listeningElement)
//		{
//			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
//			{
//				Utility.LootAll(devNull);
//				Recipe.FindRecipes();
//			}
//		}

//		private void DepositAllClick(UIMouseEvent evt, UIElement listeningElement)
//		{
//			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
//			{
//				Utility.DepositAll(devNull);
//				Recipe.FindRecipes();
//			}
//		}

//		private void QuickStackClick(UIMouseEvent evt, UIElement listeningElement)
//		{
//			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
//			{
//				Utility.QuickStack(devNull);
//				Recipe.FindRecipes();
//			}
//		}

//		public void Load(DevNull devNull)
//		{
//			this.devNull = devNull;

//			for (int i = 0; i < devNull.GetItems().Count; i++)
//			{
//				UIDevNullSlot slot = new UIDevNullSlot(this, i);
//				gridItems.Add(slot);
//			}
//		}
//	}

//	public class UIDevNullSlot : UIElement
//	{
//		public Texture2D backgroundTexture = Main.inventoryBackTexture;

//		public DevNullUI devNullUI;

//		public int slot;

//		public int maxStack = int.MaxValue;

//		public bool active;

//		public Item Item
//		{
//			get { return devNullUI.devNull.GetItems()[slot]; }
//			set { devNullUI.devNull.GetItems()[slot] = value; }
//		}

//		public UIDevNullSlot(DevNullUI devNullUI, int slot = 0)
//		{
//			Width.Pixels = 40;
//			Height.Pixels = 40;

//			this.slot = slot;
//			this.devNullUI = devNullUI;
//		}

//		public override void Click(UIMouseEvent evt)
//		{
//			base.Click(evt);

//			Main.PlaySound(SoundID.MenuTick);

//			if (!Main.keyState.IsKeyDown(Keys.LeftAlt))
//			{
//				if (Item.IsAir)
//				{
//					if (!Main.mouseItem.IsAir)
//					{
//						if (devNullUI.gridItems.items.Select(x => (UIDevNullSlot)x).Any(x => x.Item.type == Main.mouseItem.type))
//						{
//							Item item = devNullUI.gridItems.items.Select(x => (UIDevNullSlot)x).FirstOrDefault(x => x.Item.type == Main.mouseItem.type)?.Item;
//							if (item != null)
//							{
//								int count = Math.Min(Main.mouseItem.stack, item.maxStack - item.stack);
//								Main.mouseItem.stack -= count;
//								item.stack += count;
//								if (Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();
//							}
//						}
//						else
//						{
//							Item = Main.mouseItem.Clone();
//							if (maxStack > 0) Item.maxStack = maxStack;
//							int count = Math.Min(Main.mouseItem.stack, Item.maxStack);
//							Item.stack = count;
//							Main.mouseItem.stack -= count;
//							if (Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();
//						}
//					}
//				}
//				else
//				{
//					if (Main.mouseItem.IsAir)
//					{
//						Main.mouseItem = Item.Clone();
//						Item temp = new Item();
//						temp.SetDefaults(Item.type);
//						Main.mouseItem.maxStack = temp.maxStack;
//						int count = Math.Min(Item.stack, Main.mouseItem.maxStack);
//						Main.mouseItem.stack = count;
//						Item.stack -= count;
//						if (Item.stack <= 0)
//						{
//							Item.TurnToAir();
//							active = false;
//							devNullUI.devNull.selectedIndex = -1;
//						}
//					}
//					else if (!Main.mouseItem.IsAir && Main.mouseItem.type == Item.type)
//					{
//						if (maxStack > 0) Item.maxStack = maxStack;
//						int count = Math.Min(Main.mouseItem.stack, Item.maxStack - Item.stack);
//						Main.mouseItem.stack -= count;
//						Item.stack += count;
//						if (Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();
//					}
//				}
//			}
//			else
//			{
//				if (!Item.IsAir)
//				{
//					if (!active)
//					{
//						devNullUI.gridItems.items.ForEach(x => ((UIDevNullSlot)x).active = false);
//						devNullUI.devNull.SetItem(slot);
//						active = true;
//					}
//					else
//					{
//						devNullUI.devNull.SetItem(-1);
//						active = false;
//					}
//				}
//			}

//			NetUtility.SyncEntity(MessageID.SyncItem, slot);
//		}

//		public override void RightClick(UIMouseEvent evt)
//		{
//			base.RightClick(evt);

//			Main.PlaySound(SoundID.MenuTick);

//			NetUtility.SyncEntity(MessageID.SyncItem, slot);
//		}
		
//		public override int CompareTo(object obj) => slot.CompareTo(((UIDevNullSlot)obj).slot);

//		protected override void DrawSelf(SpriteBatch spriteBatch)
//		{
//			CalculatedStyle dimensions = GetInnerDimensions();

//			float scale = Math.Min(dimensions.Width / backgroundTexture.Width, dimensions.Height / backgroundTexture.Height);
//			spriteBatch.Draw(!Item.IsAir && active ? Main.inventoryBack15Texture : backgroundTexture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

//			if (!Item.IsAir)
//			{
//				Texture2D itemTexture = Main.itemTexture[Item.type];
//				Rectangle rect = Main.itemAnimations[Item.type] != null ? Main.itemAnimations[Item.type].GetFrame(itemTexture) : itemTexture.Frame();
//				Color newColor = Color.White;
//				float pulseScale = 1f;
//				ItemSlot.GetItemLight(ref newColor, ref pulseScale, Item);
//				int height = rect.Height;
//				int width = rect.Width;
//				float drawScale = 1f;
//				float availableWidth = dimensions.Width - 8;
//				if (width > availableWidth || height > availableWidth)
//				{
//					if (width > height) drawScale = availableWidth / width;
//					else drawScale = availableWidth / height;
//				}
//				drawScale *= scale;
//				Vector2 vector = backgroundTexture.Size() * scale;
//				Vector2 position2 = dimensions.Position() + vector / 2f - rect.Size() * drawScale / 2f;
//				Vector2 origin = rect.Size() * (pulseScale / 2f - 0.5f);

//				if (ItemLoader.PreDrawInInventory(Item, spriteBatch, position2, rect, Item.GetAlpha(newColor), Item.GetColor(Color.White), origin, drawScale * pulseScale))
//				{
//					spriteBatch.Draw(itemTexture, position2, rect, Item.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
//					if (Item.color != Color.Transparent) spriteBatch.Draw(itemTexture, position2, rect, Item.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
//				}
//				ItemLoader.PostDrawInInventory(Item, spriteBatch, position2, rect, Item.GetAlpha(newColor), Item.GetColor(Color.White), origin, drawScale * pulseScale);
//				if (ItemID.Sets.TrapSigned[Item.type]) spriteBatch.Draw(Main.wireTexture, dimensions.Position() + new Vector2(40f, 40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
//				if (Item.stack > 1) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, Item.stack.ToSI("F1"), dimensions.Position() + new Vector2(10f, 26f) * scale, Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);

//				if (IsMouseHovering)
//				{
//					Main.LocalPlayer.showItemIcon = false;
//					Main.ItemIconCacheUpdate(0);
//					Main.HoverItem = Item.Clone();
//					Main.hoverItemName = Main.HoverItem.Name;
//				}
//			}
//		}
//	}
//}