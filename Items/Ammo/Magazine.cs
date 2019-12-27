using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Ammo
{
	public class Magazine : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/Magazine";
		public override string AmmoType => "Bullet";

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IronBar, 10);
			recipe.AddIngredient(ItemID.Obsidian, 10);
			recipe.anyIronBar = true;
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 24;
			item.height = 28;
			item.rare = ItemRarityID.Green;
			item.value = 15000 * 5;
		}
	}
}