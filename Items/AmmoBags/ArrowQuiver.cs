using Terraria;
using Terraria.ID;

namespace PortableStorage.Items;

public class ArrowQuiver : BaseAmmoBag
{
	public override string Texture => PortableStorage.AssetPath + "Textures/Items/ArrowQuiver";

	protected override string AmmoType => "Arrow";

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(silver: 60);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Leather, 8)
			.AddRecipeGroup(Utility.YoYoStrings)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}