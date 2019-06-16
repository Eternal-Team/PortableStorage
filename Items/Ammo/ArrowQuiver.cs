using PortableStorage.Global;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Ammo
{
	public class ArrowQuiver : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/Quiver";

		public override string AmmoType => "Arrow";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arrow Quiver");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of arrows");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 26;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 8);
			recipe.AddRecipeGroup(Utility.yoyoStringGroup.GetText());
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}