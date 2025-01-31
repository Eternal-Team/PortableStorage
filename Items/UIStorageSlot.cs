using System;
using BaseLibrary;
using BaseLibrary.Input;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
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

		Padding = new Padding(12);
	}

	protected override void MouseDown(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;
		args.Handled = true;

		Player player = Main.LocalPlayer;
		if (player.itemAnimation != 0 || player.itemTime != 0) return;

		Item.newAndShiny = false;

		// Vanilla behavior is shift-click works regardless of mouseItem 
		if (ItemSlot.ShiftInUse && !Item.IsAir)
		{
			if (storage.SimulateRemoveItem(player, index, out Item temp).IsSuccess())
			{
				int originalAmount = temp.stack;

				temp.position = player.Center;
				temp = player.GetItem(Main.myPlayer, temp, GetItemSettings.InventoryEntityToPlayerInventorySettings);

				int difference = originalAmount - temp.stack;
				storage.RemoveItem(player, index, out _, difference);
			}

			return;
		}

		if (Main.mouseItem.IsAir)
		{
			storage.RemoveItem(player, index, out Main.mouseItem);
		}
		else
		{
			if (Item.type == Main.mouseItem.type || Item.IsAir)
			{
				storage.InsertItem(player, index, ref Main.mouseItem);
			}
			else
			{
				// Only swap items if mouseItem fully fits into a slot and the existing item can be fully removed.
				if (storage.CheckFit(player, index, Main.mouseItem) && storage.SimulateRemoveItem(player, index, out _) == ItemStorage.Result.Success)
				{
					storage.RemoveItem(player, index, out Item item);
					storage.InsertItem(player, index, ref Main.mouseItem);
					Main.mouseItem = item;
				}
			}
		}

		if (!Main.mouseItem.IsAir) AchievementsHelper.NotifyItemPickup(player, Main.mouseItem);

		if (!Main.mouseItem.IsAir || !Item.IsAir)
		{
			Recipe.FindRecipes();
			SoundEngine.PlaySound(SoundID.Grab);
		}
	}

	protected override void MouseHeld(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Right) return;
		args.Handled = true;

		if (Item.IsAir || (Item.maxStack <= 1 && Item.stack <= 1) || Main.LocalPlayer.itemAnimation > 0 || Main.stackSplit > 1)
			return;

		int num = Main.superFastStack + 1;
		for (int i = 0; i < num; i++)
		{
			if ((Main.mouseItem.type == Item.type && ItemLoader.CanStack(Main.mouseItem, Item) || Main.mouseItem.IsAir) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.IsAir))
			{
				if (storage.RemoveItem(Main.LocalPlayer, index, out Item item, 1).IsSuccess())
				{
					if (Main.mouseItem.IsAir)
						Main.mouseItem = item;
					else
						ItemLoader.StackItems(Main.mouseItem, item, out _);
				}

				SoundEngine.PlaySound(SoundID.MenuTick);
				ItemSlot.RefreshStackSplitCooldown();
			}
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(TextureAssets.MagicPixel.Value, Dimensions.Modified(2, 2, -4, -4), new Color(252, 221, 159, 255));

		Vector2 position = Dimensions.TopLeft();
		Vector2 size = Dimensions.Size();

		DrawingUtility.DrawAchievementBorder(spriteBatch, position, size);

		if (Item.IsAir) return;

		ItemSlot.DrawItemIcon(Item, 0, spriteBatch, position + size * 0.5f, 1f, Math.Min(InnerDimensions.Width, InnerDimensions.Height), Color.White);

		if (Item.stack > 1)
		{
			string text = Item.stack.ToString();
			const float textScale = 0.9f;

			Vector2 textPos = InnerDimensions.BottomLeft() + new Vector2(-2f, 0f);
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text, textPos, Color.White, 0f, new Vector2(0f, FontAssets.ItemStack.Value.MeasureString(text).Y - 8f), new Vector2(textScale), -1f, textScale);
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