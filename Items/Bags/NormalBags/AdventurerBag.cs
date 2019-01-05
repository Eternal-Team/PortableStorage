using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public class AdventurerBag : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/AdventurerBag";
		public override int SlotCount => 18;
		public override string Name => "Adventurer's";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.rare = 1;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}