using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public class BuilderReserve : BaseSelectableBag
{
	private class BuilderReserveItemStorage : BagStorage
	{
		public BuilderReserveItemStorage(int slots, BaseBag bag) : base(bag, slots)
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
			return (Item.createTile >= TileID.Dirt || Item.createWall > WallID.None) && (this[slot].type == Item.type || !Contains(Item.type));
		}

		public override int GetSlotSize(int slot, Item Item) => int.MaxValue;
	}

	public override string Texture => PortableStorage.AssetPath + "Textures/Items/BuilderReserve";

	public override void OnCreate(ItemCreationContext context)
	{
		base.OnCreate(context);

		Storage = new BuilderReserveItemStorage(9, this);
		SelectedIndex = -1;
	}

	public override ModItem Clone(Item Item)
	{
		BuilderReserve clone = (BuilderReserve)base.Clone(Item);
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

	public override bool? UseItem(Player player) => null;
}