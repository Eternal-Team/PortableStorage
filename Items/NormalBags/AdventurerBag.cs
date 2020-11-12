using Terraria.ID;

namespace PortableStorage.Items
{
	public class AdventurerBag : BaseNormalBag
	{
		protected override int SlotCount => 18;

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather,8)
				.AddTile(TileID.WorkBenches)
				.Register();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 32;
			item.rare = ItemRarityID.Blue;
			item.value = 10000 * 5;
		}
	}
}