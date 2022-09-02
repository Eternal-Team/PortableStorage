using System.IO;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace PortableStorage.Items;

public class BuilderReserve : BaseBag
{
	private class BuilderReserveItemStorage : BagStorage
	{
		public BuilderReserveItemStorage(int slots, BaseBag bag) : base(bag, slots)
		{
		}

		public override void OnContentsChanged(object user, Operation operation, int slot)
		{
			base.OnContentsChanged(user,operation,slot);
			
			if (this[slot].IsAir && bag is BuilderReserve reserve)
				reserve.SelectedIndex = -1;
		}

		public override bool IsItemValid(int slot, Item Item)
		{
			return (Item.createTile >= TileID.Dirt || Item.createWall > WallID.None) && (this[slot].type == Item.type || !Contains(Item.type));
		}

		public override int GetSlotSize(int slot, Item Item) => int.MaxValue;
	}

	public override string Texture => PortableStorage.AssetPath + "Textures/Items/BuilderReserve";

	public Item SelectedItem => selectedIndex >= 0 ? Storage[selectedIndex] : null;

	private int selectedIndex;

	public int SelectedIndex
	{
		get => selectedIndex;
		set
		{
			if (value == -1)
			{
				selectedIndex = Item.placeStyle = Item.createTile = Item.createWall = -1;

				Item.ClearNameOverride();
			}
			else
			{
				selectedIndex = value;
				if (SelectedItem.createTile >= TileID.Dirt)
				{
					Item.createTile = SelectedItem.createTile;
					Item.createWall = -1;
					Item.placeStyle = SelectedItem.placeStyle;
				}
				else if (SelectedItem.createWall > WallID.None)
				{
					Item.createTile = -1;
					Item.createWall = SelectedItem.createWall;
					Item.placeStyle = SelectedItem.placeStyle;
				}

				Item.SetNameOverride(Lang.GetItemNameValue(Item.type) + $" ({SelectedItem.Name})");
			}

			ModContent.GetInstance<BagSyncSystem>().Register(this);
		}
	}

	public override void OnCreate(ItemCreationContext context)
	{
		base.OnCreate(context);
		
		Storage = new BuilderReserveItemStorage(9, this);
		selectedIndex = -1;
	}

	public override ModItem Clone(Item Item)
	{
		BuilderReserve clone = (BuilderReserve)base.Clone(Item);
		clone.selectedIndex = selectedIndex;
		return clone;
	}

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 32;
		Item.height = 32;
		Item.autoReuse = true;
		Item.useTurn = true;
		Item.consumable = true;
		Item.rare = ItemRarityID.Orange;
		Item.value = 12000 * 5;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Bone, 30)
			.AddIngredient(ItemID.IronCrate)
			.AddTile(TileID.BoneWelder)
			.Register();
	}

	public override bool CanUseItem(Player player) => SelectedItem is not null && !SelectedItem.IsAir;

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
		if (SelectedItem is null || SelectedItem.IsAir || player.altFunctionUse == 2 || rightClicked) return false;

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

	public override bool? UseItem(Player player) => null;
}