using BaseLibrary;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		internal static PortableStorage Instance;

		internal static Texture2D textureBlackHole;
		internal static Texture2D textureLootAll;
		internal static Texture2D textureDepositAll;

		public override void Load()
		{
			Instance = this;

			Hooking.Load();

			if (!Main.dedServ)
			{
				textureBlackHole = ModContent.GetTexture("PortableStorage/Textures/Items/TheBlackHole");
				textureLootAll = ModContent.GetTexture("BaseLibrary/Textures/UI/LootAll");
				textureDepositAll = ModContent.GetTexture("BaseLibrary/Textures/UI/DepositAll");
			}
		}

		public override void Unload() => BaseLibrary.Utility.UnloadNullableTypes();

		public override void AddRecipeGroups() => Utility.AddRecipeGroups();

		public override void PostSetupContent() => Utility.PostSetupContent();

		public override object Call(params object[] args)
		{
			if (args.Length != 2 || !(args[0] is string command)) return base.Call(args);

			switch (command)
			{
				case "RegisterIngredient" when args[1] is short ID && !Utility.AlchemistBagWhitelist.Contains(ID):
				{
					Utility.AlchemistBagWhitelist.Add(ID);
					Logger.Info($"Ingredient '{ID}' added to Alchemist's Bag whitelist!");
					break;
				}

				case "RegisterOre" when args[1] is short ID && !Utility.OreWhitelist.Contains(ID):
				{
					Utility.OreWhitelist.Add(ID);
					Logger.Info($"Ore '{ID}' added to Ore whitelist!");
					break;
				}
			}

			return base.Call(args);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Vertebrae, 5);
			recipe.SetResult(ItemID.Leather);
			recipe.AddRecipe();
		}

		public override void PostAddRecipes()
		{
			foreach (ModItem item in this.GetValue<Dictionary<string, ModItem>>("items").Values)
			{
				Recipe recipe = Main.recipe.FirstOrDefault(x => x.createItem.type == item.item.type);
				if (recipe != null) item.item.value = recipe.requiredItem.Sum(x => x.value * x.stack);
			}
		}
	}
}