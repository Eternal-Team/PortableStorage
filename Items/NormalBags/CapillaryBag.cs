using Terraria;
using Terraria.ID;

namespace PortableStorage.Items;

public class CapillaryBag : BaseNormalBag
{
	public override string Texture => PortableStorage.AssetPath + "Textures/Items/CapillaryBag";

	protected override int SlotCount => 54;

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 26;
		Item.height = 30;
		Item.rare = ItemRarityID.Lime;
		Item.value = Item.sellPrice(gold: 15);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Leather, 20)
			.AddIngredient(ItemID.ChlorophyteBar, 15)
			.AddIngredient(ItemID.Vine, 7)
			.AddIngredient(ItemID.LifeFruit)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}