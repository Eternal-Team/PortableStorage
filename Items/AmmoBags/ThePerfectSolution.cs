using Terraria;
using Terraria.ID;

namespace PortableStorage.Items;

public class ThePerfectSolution : BaseAmmoBag
{
	public override string Texture => PortableStorage.AssetPath + "Textures/Items/ThePerfectSolution";

	protected override string AmmoType => "Solution";

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(gold: 7, silver: 50);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Glass, 10)
			.AddIngredient(ItemID.HallowedBar, 7)
			.AddIngredient(ItemID.SoulofFright, 5)
			.AddTile(TileID.SteampunkBoiler)
			.Register();
	}
}