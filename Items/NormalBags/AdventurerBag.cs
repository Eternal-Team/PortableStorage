using Terraria;
using Terraria.ID;

namespace PortableStorage.Items
{
	public class AdventurerBag : BaseNormalBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/AdventurerBag";

		protected override int SlotCount => 18;

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.width = 26;
			Item.height = 32;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(gold: 1);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Leather, 8)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}