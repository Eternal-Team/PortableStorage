using Terraria;
using Terraria.ID;

namespace PortableStorage.Items
{
	public class ThePerfectSolution : BaseAmmoBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/ThePerfectSolution";

		protected override string AmmoType => "Solution";

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Glass, 10)
				.AddIngredient(ItemID.HallowedBar, 7)
				.AddIngredient(ItemID.SoulofFright, 5)
				.AddTile(TileID.SteampunkBoiler)
				.Register();
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Pink;
			item.value = Item.buyPrice(gold: 7, silver: 50);
		}
	}
}