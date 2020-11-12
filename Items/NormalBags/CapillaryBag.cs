using Terraria.ID;

namespace PortableStorage.Items
{
	public class CapillaryBag : BaseNormalBag
	{
		protected override int SlotCount => 54;

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 32;
			item.rare = ItemRarityID.Lime;
			item.value = 150000 * 5;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather, 20)
				.AddIngredient(ItemID.ChlorophyteBar, 15)
				.AddIngredient(ItemID.Vine, 7)
				.AddIngredient(ItemID.LifeFruit)
				.AddTile(TileID.LivingLoom)
				.Register();
		}
	}
}