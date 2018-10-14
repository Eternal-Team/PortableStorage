using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public class StoragemasterBag : BaseNormalBag
	{
		public override int SlotCount => 54;
		public override string Name => "Storagemaster's";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.rare = 5;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 55);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 10);
			recipe.AddRecipeGroup("Wood", 20);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}