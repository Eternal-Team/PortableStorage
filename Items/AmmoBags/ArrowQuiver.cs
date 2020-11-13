using Terraria;
using Terraria.ID;

namespace PortableStorage.Items
{
	public class ArrowQuiver : BaseAmmoBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/ArrowQuiver";

		protected override string AmmoType => "Arrow";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Blue;
			item.value = Item.buyPrice(silver: 60);
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
}