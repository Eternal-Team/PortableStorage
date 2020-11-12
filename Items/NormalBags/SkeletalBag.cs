using Terraria;
using Terraria.ID;

namespace PortableStorage.Items
{
	public class SkeletalBag : BaseNormalBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/SkeletalBag";

		protected override int SlotCount => 27;

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 32;
			item.rare = ItemRarityID.Green;
			item.value = Item.sellPrice(gold: 2, silver: 50);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather, 10)
				.AddIngredient(ItemID.Bone, 30)
				.AddTile(TileID.BoneWelder)
				.Register();
		}
	}
}