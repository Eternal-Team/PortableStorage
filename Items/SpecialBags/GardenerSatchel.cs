using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public class GardenerSatchel : BaseSelectableBag
{
	private class GardenerSatchelItemStorage : BagStorage
	{
		public GardenerSatchelItemStorage(int slots, BaseBag bag) : base(bag, slots)
		{
		}

		protected override void OnContentsChanged(object user, Operation operation, int slot)
		{
			base.OnContentsChanged(user, operation, slot);

			if (this[slot].IsAir && bag is BuilderReserve reserve)
			{
				reserve.SelectedIndex = -1;
				BagSyncSystem.Instance.Sync(reserve.ID, PacketID.SelectedIndex);
			}
		}

		public override bool IsItemValid(int slot, Item Item)
		{
			return Utility.SeedWhitelist.Contains(Item.type) && (this[slot].type == Item.type || !Contains(Item.type));
		}

		public override int GetSlotSize(int slot, Item Item) => int.MaxValue;
	}

	public override string Texture => PortableStorage.AssetPath + "Textures/Items/GardenerSatchel";

	public override void OnCreate(ItemCreationContext context)
	{
		base.OnCreate(context);

		Storage = new GardenerSatchelItemStorage(18, this);
		SelectedIndex = -1;
	}

	public override ModItem Clone(Item Item)
	{
		GardenerSatchel clone = (GardenerSatchel)base.Clone(Item);
		clone.SelectedIndex = SelectedIndex;
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
			.Register();
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