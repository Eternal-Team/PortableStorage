using Terraria.ID;
using Terraria.ModLoader;
using TheOneLibrary.Base.Items;

namespace PortableStorage.Items
{
	public class DockingStation : BaseItem
	{
		public override string Texture => PortableStorage.Textures.ItemPath + "DockingStation";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Docking Station");
			Tooltip.SetDefault("Allows items to be extracted from bags automatically");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType<Tiles.DockingStation>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Obsidian, 15);
			recipe.AddIngredient(ItemID.DemoniteBar, 15);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Obsidian, 15);
			recipe.AddIngredient(ItemID.CrimtaneBar, 15);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}