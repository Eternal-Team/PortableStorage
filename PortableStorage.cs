using BaseLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		internal static Texture2D textureBlackHole;
		internal static Texture2D textureLootAll;
		internal static Texture2D textureDepositAll;
		internal static Texture2D textureQuickStack;

		internal static int BlackHoleAngle;

		public override void Load()
		{
			Hooking.Load();

			if (!Main.dedServ)
			{
				textureBlackHole = ModContent.GetTexture("PortableStorage/Textures/Items/TheBlackHole");
				textureLootAll = ModContent.GetTexture("BaseLibrary/Textures/UI/LootAll");
				textureDepositAll = ModContent.GetTexture("BaseLibrary/Textures/UI/DepositAll");
				textureQuickStack = ModContent.GetTexture("BaseLibrary/Textures/UI/QuickStack");
			}
		}

		public override void Unload() => this.UnloadNullableTypes();

		public override void AddRecipeGroups() => Utility.AddRecipeGroups();

		public override void PostSetupContent() => Utility.PostSetupContent();

		public override void UpdateUI(GameTime gameTime)
		{
			BlackHoleAngle++;
			if (BlackHoleAngle > 360) BlackHoleAngle = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Vertebrae, 5);
			recipe.SetResult(ItemID.Leather);
			recipe.AddRecipe();
		}
	}
}