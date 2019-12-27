using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Ammo
{
	public class ArrowQuiver : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/ArrowQuiver";

		public override string AmmoType => "Arrow";

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 8);
			recipe.AddRecipeGroup("PortableStorage:YoYoStrings");
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Blue;
			item.value = 6000 * 5;
		}
	}
}