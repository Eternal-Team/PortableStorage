using Terraria;
using Terraria.ID;

namespace PortableStorage.Items
{
	public class Magazine : BaseAmmoBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/Magazine";

		protected override string AmmoType => "Bullet";

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.width = 24;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 1, silver: 50);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Obsidian, 10)
				.AddRecipeGroup(RecipeGroupID.IronBar, 10)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}