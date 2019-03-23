//using PortableStorage.Global;
//using PortableStorage.UI.Bags;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace PortableStorage.Items.Bags
//{
// todo: Handler that combines all the other Handlers

//	public class MasterAmmoPounch : BaseAmmoBag
//	{
//		public override string AmmoType => "All";

//		public new AmmoBagPanel UI;

//		public override void SetStaticDefaults()
//		{
//			DisplayName.SetDefault("Master Ammo Pouch");
//			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of ammo");
//		}

//		public override void SetDefaults()
//		{
//			base.SetDefaults();

//			item.width = 32;
//			item.height = 32;
//		}

//		public override void AddRecipes()
//		{
//			ModRecipe recipe = new ModRecipe(mod);
//			recipe.AddRecipeGroup(Utility.tier1HMBarsGroup.GetText(), 15);
//			recipe.AddRecipeGroup(Utility.ichorFlameGroup.GetText(), 10);
//			recipe.AddIngredient(ItemID.SoulofNight, 7);
//			recipe.AddTile(TileID.MythrilAnvil);
//			recipe.SetResult(this);
//			recipe.AddRecipe();
//		}
//	}
//}

