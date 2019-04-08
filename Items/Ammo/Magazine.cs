using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Ammo
{
	public class Magazine : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/Magazine";
		public override string AmmoType => "Bullet";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magazine");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of bullets");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 18;
			item.height = 32;
		}

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
	}
}