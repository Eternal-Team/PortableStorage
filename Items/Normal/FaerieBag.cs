using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Normal
{
	public class FaerieBag : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/FaerieBag";

		public override int SlotCount => 36;

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 26;
			item.rare = ItemRarityID.LightRed;
			item.value = 30000 * 5;
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