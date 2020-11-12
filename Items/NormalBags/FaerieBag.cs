using Terraria.ID;

namespace PortableStorage.Items
{
	public class FaerieBag : BaseNormalBag
	{
		protected override int SlotCount => 36;

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 32;
			item.rare = ItemRarityID.LightRed;
			item.value = 30000 * 5;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Silk, 15)
				.AddIngredient(ItemID.PixieDust, 10)
				.AddIngredient(ItemID.SoulofLight, 7)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}