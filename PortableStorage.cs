using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage
{
	// note: gardener's bag - similar to wand of regrowth

	public class PortableStorage : Mod
	{
		internal static PortableStorage Instance;

		internal static Texture2D textureBlackHole;
		internal static Texture2D textureLootAll;
		internal static Texture2D textureDepositAll;
		internal static Texture2D textureQuickStack;

		internal static int BlackHoleAngle;

		public override void Load()
		{
			Instance = this;

			Hooking.Load();

			if (!Main.dedServ)
			{
				textureBlackHole = ModContent.GetTexture("PortableStorage/Textures/Items/TheBlackHole");
				textureLootAll = ModContent.GetTexture("BaseLibrary/Textures/UI/LootAll");
				textureDepositAll = ModContent.GetTexture("BaseLibrary/Textures/UI/DepositAll");
				textureQuickStack = ModContent.GetTexture("BaseLibrary/Textures/UI/QuickStack");
			}
		}

		public override void Unload() => BaseLibrary.Utility.UnloadNullableTypes();

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