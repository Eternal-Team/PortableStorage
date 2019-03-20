using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public class FaerieBag : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/FaerieBag";
		public override int SlotCount => 36;
		public override string Name => "Faerie";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.rare = 3;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Silk, 15);
			recipe.AddIngredient(ItemID.PixieDust, 10);
			recipe.AddIngredient(ItemID.SoulofLight, 7);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}