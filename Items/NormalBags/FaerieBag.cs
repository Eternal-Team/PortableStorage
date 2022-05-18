using Terraria;
using Terraria.ID;

namespace PortableStorage.Items;

public class FaerieBag : BaseNormalBag
{
	public override string Texture => PortableStorage.AssetPath + "Textures/Items/FaerieBag";

	protected override int SlotCount => 36;

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 28;
		Item.height = 26;
		Item.rare = ItemRarityID.LightRed;
		Item.value = Item.sellPrice(gold: 3);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Silk, 15)
			.AddIngredient(ItemID.PixieDust, 10)
			.AddIngredient(ItemID.SoulofLight, 7)
			.AddTile(TileID.MythrilAnvil)
			.Register();
	}
}