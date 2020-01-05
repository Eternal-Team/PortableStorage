using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Normal
{
	public class CapillaryBag : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/CapillaryBag";

		public override int SlotCount => 54;

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 20);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
			recipe.AddIngredient(ItemID.Vine, 7);
			recipe.AddIngredient(ItemID.LifeFruit);
			recipe.AddTile(TileID.LivingLoom);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 30;
			item.rare = ItemRarityID.Lime;
			item.value = 150000 * 5;
		}
	}
}