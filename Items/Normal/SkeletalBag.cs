using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Normal
{
	public class SkeletalBag : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/SkeletalBag";

		public override int SlotCount => 27;

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 32;
			item.rare = ItemRarityID.Green;
			item.value = 25000 * 5;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 10);
			recipe.AddIngredient(ItemID.Bone, 30);
			recipe.AddTile(TileID.BoneWelder);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}