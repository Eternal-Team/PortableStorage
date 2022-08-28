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

public class GardenerSatchel : BaseBag
{
	private class GardenerSatchelItemStorage : BagStorage
	{
		public GardenerSatchelItemStorage(int slots, BaseBag bag) : base(bag, slots)
		{
		}

		public override bool IsItemValid(int slot, Item Item)
		{
			return Utility.SeedWhitelist.Contains(Item.type) && (this[slot].type == Item.type || !Contains(Item.type));
		}

		public override int GetSlotSize(int slot, Item Item) => int.MaxValue;
	}

	public override string Texture => PortableStorage.AssetPath + "Textures/Items/GardenerSatchel";

	public Item SelectedItem => selectedIndex >= 0 ? Storage[selectedIndex] : null;

	public int selectedIndex;

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

			ModContent.GetInstance<BagSyncSystem>().Register(this);
		}
	}

	public GardenerSatchel()
	{
		Storage = new GardenerSatchelItemStorage(18, this);
		selectedIndex = -1;
	}

	public override ModItem Clone(Item Item)
	{
		GardenerSatchel clone = (GardenerSatchel)base.Clone(Item);
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
		Item.useTime = 10;
		Item.useAnimation = 10;
		Item.rare = ItemRarityID.Lime;
		Item.value = Item.sellPrice(gold: 5);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.StaffofRegrowth)
			.AddIngredient(ItemID.AncientBattleArmorMaterial)
			.AddIngredient(ItemID.ChlorophyteBar, 3)
			.AddIngredient(ItemID.AncientCloth, 7)
			.AddTile(TileID.LivingLoom)
			.Create();
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			int tileTargetX = Player.tileTargetX;
			int tileTargetY = Player.tileTargetY;

			return Main.tile[tileTargetX, tileTargetY].HasTile && Main.tile[tileTargetX, tileTargetY].TileType == TileID.MatureHerbs;
		}

		return SelectedItem is not null && !SelectedItem.IsAir;
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

	// bug: rightclicking consumes item
	public override bool ConsumeItem(Player player)
	{
		if (SelectedItem is null || SelectedItem.IsAir) return false;

		if (player.altFunctionUse == 2) return false;

		Storage.ModifyStackSize(player, SelectedIndex, -1);
		if (SelectedItem.IsAir) SelectedIndex = -1;

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

	public override bool? UseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			int tileTargetX = Player.tileTargetX;
			int tileTargetY = Player.tileTargetY;

			if (!Main.tile[tileTargetX, tileTargetY].HasTile || Main.tile[tileTargetX, tileTargetY].TileType != TileID.MatureHerbs)
				return false;

			bool killTile = false;
			int flowerType = Main.tile[tileTargetX, tileTargetY].TileFrameX / 18;
			if (flowerType == 0 && Main.dayTime)
				killTile = true;

			if (flowerType == 1 && !Main.dayTime)
				killTile = true;

			if (flowerType == 3 && !Main.dayTime && (Main.bloodMoon || Main.moonPhase == 0))
				killTile = true;

			if (flowerType == 4 && (Main.raining || Main.cloudAlpha > 0f))
				killTile = true;

			if (flowerType == 5 && !Main.raining && Main.dayTime && Main.time > 40500.0)
				killTile = true;

			if (killTile)
			{
				WorldGen.KillTile(tileTargetX, tileTargetY);
				NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, tileTargetX, tileTargetY);
			}

			return false;
		}

		return null;
	}
}