using System;
using BaseLibrary;
using BaseLibrary.Input;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PortableStorage.Items;

// TODO: visual aid if item is not valid for slot

public class UIStorageSlot : BaseElement
{
	private readonly ItemStorage storage;
	private readonly int index;

	private Item Item => storage[index];

	public UIStorageSlot(ItemStorage storage, int index)
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

		if (ItemSlot.ShiftInUse)
		{
			storage.RemoveItem(player, index, out Item temp);
			temp.position = player.Center;
			temp = player.GetItem(Main.myPlayer, temp, GetItemSettings.LootAllSettings);
			if (!temp.IsAir)
			{
				storage.InsertItem(player, index, ref temp);
			}

			return;
		}

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

			Vector2 textPos = InnerDimensions.BottomLeft() - new Vector2(0f, FontAssets.MouseText.Value.MeasureString(text).Y) * texscale;
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