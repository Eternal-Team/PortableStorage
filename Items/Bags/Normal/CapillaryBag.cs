using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public class CapillaryBag : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/StoragemasterBag";
		public override int SlotCount => 54;
		public override string Name => "Capillary";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.rare = 5;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 20);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
			recipe.AddIngredient(ItemID.Vine, 7);
			recipe.AddIngredient(ItemID.Heart);
			recipe.AddTile(TileID.LivingLoom);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}