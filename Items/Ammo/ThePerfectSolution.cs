using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Ammo
{
	public class ThePerfectSolution : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/ThePerfectSolution";

		public override string AmmoType => "Solution";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Pink;
			item.value = 75000 * 5;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Glass, 10);
			recipe.AddIngredient(ItemID.HallowedBar, 7);
			recipe.AddIngredient(ItemID.SoulofFright, 5);
			recipe.AddTile(TileID.SteampunkBoiler);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}