using System;
using System.Collections.Generic;
using BaseLibrary;
using BaseLibrary.Input;
using BaseLibrary.Items;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PortableStorage.Items;

public class Bag : BaseItem
{
	protected override bool CloneNewInstances => false;

	protected internal ItemStorage Storage;
	protected internal Guid ID;

	public static List<(Guid id, string stacktract)> Bags = [];

	public override ModItem NewInstance(Item entity)
	{
		Bag bag = base.NewInstance(entity) as Bag;
		bag.ID = Guid.NewGuid();
		bag.Storage = new ItemStorage(9);
		Bags.Add((bag.ID, Environment.StackTrace));

		return bag;
	}

	public override void SetDefaults()
	{
		Item.useTime = 5;
		Item.useAnimation = 5;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.rare = ItemRarityID.White;
		Item.width = 16;
		Item.height = 16;
	}

	public override bool ConsumeItem(Player player) => false;

	public override bool? UseItem(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
		{
			/*BagUI.Instance.Display = BagUI.Instance.Display == Display.Visible ? Display.None : Display.Visible;
			BagUI.Instance.SetBag(this);
			BagUI.Instance.Recalculate();*/

			UISystem.UILayer.Remove(BagUI.Instance);
			UISystem.UILayer.Add(new BagUI());
			BagUI.Instance.Display = Display.Visible;

			// BookUI.Instance = new BookUI();
			// BookUI.Instance.Display = BookUI.Instance.Display == Display.Visible ? Display.None : Display.Visible;
			BagUI.Instance.Recalculate();
			BagUI.Instance.SetBag(this);
		}

		return true;
	}

	public override void SaveData(TagCompound tag)
	{
		tag.Set("ID", ID);
		tag.Set("Items", Storage.Save());
	}

	public override void LoadData(TagCompound tag)
	{
		ID = tag.Get<Guid>("ID");
		Storage.Load(tag.Get<TagCompound>("Items"));
	}
}

public class BagUI : UIPanel
{
	public static BagUI Instance = null!;

	private Bag bag;

	private UIText text;
	private UIGrid<UIContainerSlot> gridItems;

	public BagUI()
	{
		Instance = this;

		Size = Dimension.FromPixels(516, 300);
		Position = Dimension.FromPercent(50);
		Display = Display.None;

		text = new UIText("GUID") {
			Size = new Dimension(0, 20, 100, 0),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				TextColor = Color.White,
				BorderColor = Color.Black
			}
		};
		base.Add(text);

		gridItems = new UIGrid<UIContainerSlot>(9) {
			Size = new Dimension(0, -28, 100, 100),
			Position = Dimension.FromPixels(0, 28),
			Settings = {
				ItemMargin = 4
			}
		};
		base.Add(gridItems);
	}

	public void SetBag(Bag bag)
	{
		this.bag = bag;
		text.Text = bag.ID.ToString();

		gridItems.Clear();

		for (int i = 0; i < bag.Storage.Count; i++)
		{
			gridItems.Add(new UIContainerSlot(bag.Storage, i) {
				Size = Dimension.FromPixels(52)
			});
		}
	}
}

// TODO: visual aid if item is not valid for slot
public class UIContainerSlot : BaseElement
{
	private readonly ItemStorage storage;
	private readonly int index;

	private Item Item => storage[index];

	public UIContainerSlot(ItemStorage storage, int index)
	{
		this.storage = storage;
		this.index = index;

		Padding = new Padding(8);
	}

	protected override void MouseDown(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;
		args.Handled = true;

		Player player = Main.LocalPlayer;
		if (player.itemAnimation != 0 || player.itemTime != 0) return;

		Item.newAndShiny = false;

		if (Main.mouseItem.IsAir)
		{
			storage.RemoveItem(player, index, out Main.mouseItem);
		}
		else
		{
			storage.InsertItem(player, index, ref Main.mouseItem);
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(TextureAssets.MagicPixel.Value, Dimensions, new Color(40, 25, 14, 100));

		Vector2 position = Dimensions.TopLeft();
		Vector2 size = Dimensions.Size();

		DrawingUtility.DrawAchievementBorder(spriteBatch, position, size);

		if (Item.IsAir) return;

		ItemSlot.DrawItemIcon(Item, 0, spriteBatch, position + size * 0.5f, 1f, Math.Min(InnerDimensions.Width, InnerDimensions.Height), Color.White);

		if (Item.stack > 1)
		{
			string text = Item.stack.ToString();
			float texscale = 0.75f;

			Vector2 textPos = InnerDimensions.BottomLeft() - new Vector2(-16f, FontAssets.MouseText.Value.MeasureString(text).Y + 4f) * texscale;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, textPos, Color.White, 0f, Vector2.Zero, new Vector2(texscale));
		}

		if (IsMouseHovering)
		{
			Main.LocalPlayer.cursorItemIconEnabled = false;
			Main.ItemIconCacheUpdate(0);
			Main.HoverItem = Item.Clone();
			Main.hoverItemName = Main.HoverItem.Name;
		}
	}
}