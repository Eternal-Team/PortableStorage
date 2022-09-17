using System.IO;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace PortableStorage.Items;

public abstract class BaseSelectableBag : BaseBag
{
	public Item SelectedItem => selectedIndex >= 0 ? Storage[selectedIndex] : null;

	private int selectedIndex;

	public int SelectedIndex
	{
		get => selectedIndex;
		set
		{
			if (value == -1)
			{
				selectedIndex = Item.placeStyle = Item.createTile = -1;

				Item.ClearNameOverride();
			}
			else
			{
				selectedIndex = value;
				if (SelectedItem.createTile >= TileID.Dirt)
				{
					Item.createTile = SelectedItem.createTile;
					Item.placeStyle = SelectedItem.placeStyle;
				}

				Item.SetNameOverride(Lang.GetItemNameValue(Item.type) + $" ({SelectedItem.Name})");
			}
		}
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);

		tag["SelectedIndex"] = selectedIndex;
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		SelectedIndex = tag.GetInt("SelectedIndex");
	}

	public override void NetSend(BinaryWriter writer)
	{
		base.NetSend(writer);

		writer.Write(SelectedIndex);
	}

	public override void NetReceive(BinaryReader reader)
	{
		base.NetReceive(reader);

		SelectedIndex = reader.ReadInt32();
	}

	private bool rightClicked;

	public override void RightClick(Player player)
	{
		base.RightClick(player);

		rightClicked = true;
	}

	public override bool ConsumeItem(Player player)
	{
		if (SelectedItem is null || SelectedItem.IsAir || player.altFunctionUse == 2 || rightClicked)
		{
			rightClicked = false;
			return false;
		}

		rightClicked = false;
		Storage.ModifyStackSize(player, SelectedIndex, -1);

		return false;
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (SelectedItem is null || SelectedItem.IsAir) return true;

		DrawingUtility.DrawItemInInventory(spriteBatch, SelectedItem, position + frame.Size() * 0.5f * scale, frame.Size() * 0.5f, scale, false);

		return true;
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (SelectedItem is null || SelectedItem.IsAir) return;

		string text = SelectedItem.stack < 1000 ? SelectedItem.stack.ToString() : TextUtility.ToSI(SelectedItem.stack, "N1");
		Vector2 size = FontAssets.MouseText.Value.MeasureString(text);

		ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, position + new Vector2(frame.Width * 0.5f, frame.Height) * scale, Color.White, 0f, size * 0.5f, new Vector2(0.8f) * scale);
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		if (SelectedItem is null || SelectedItem.IsAir) return true;

		Vector2 position = Item.position - Main.screenPosition + new Vector2(16) * scale;

		DrawingUtility.DrawItemInWorld(spriteBatch, SelectedItem, position, new Vector2(16) * scale, rotation);

		return true;
	}
}