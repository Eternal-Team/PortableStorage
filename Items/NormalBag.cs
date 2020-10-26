using Terraria.ID;

namespace PortableStorage.Items
{
	public class NormalBag : BaseBag
	{
		public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.DirtBlock, 2).Register();
	}
}