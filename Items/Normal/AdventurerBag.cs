using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Normal
{
	public class AdventurerBag : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/AdventurerBag";

		public override int SlotCount => 18;
		public override string Name => "Adventurer's";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 32;
			item.rare = ItemRarityID.Blue;
			item.value = 10000 * 5;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}