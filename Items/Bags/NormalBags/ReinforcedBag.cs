using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public class ReinforcedBagM : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/ReinforcedBag_M";
		public override int SlotCount => 36;
		public override string Name => "Reinforced";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.rare = 3;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 25);
			recipe.AddIngredient(ItemID.MythrilBar, 5);
			recipe.AddRecipeGroup("Wood", 7);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}

	public class ReinforcedBagO : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/ReinforcedBag_O";
		public override int SlotCount => 36;
		public override string Name => "Reinforced";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.rare = 3;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 25);
			recipe.AddIngredient(ItemID.OrichalcumBar, 5);
			recipe.AddRecipeGroup("Wood", 7);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}