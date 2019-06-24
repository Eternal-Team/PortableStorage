using BaseLibrary;
using PortableStorage.Items.Special;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage
{
	// todo: add bag slot
	// todo: add fluid + energy addons

	public class PortableStorage : Mod
	{
		internal static PortableStorage Instance;

		public override void Load()
		{
			Instance = this;

			ContainerLibrary.Hooking.AlchemyApplyChance += () => Main.LocalPlayer.inventory.Any(item => item.modItem is AlchemistBag);
			ContainerLibrary.Hooking.ModifyAdjTiles += player => player.adjTile[TileID.Bottles] = player.inventory.Any(item => item.modItem is AlchemistBag);

			Hooking.Hooking.Load();
		}

		public override void Unload()
		{
			Utility.UnloadNullableTypes();
		}

		public override void AddRecipeGroups()
		{
			Global.Utility.AddRecipeGroups();
		}

		public override void PostSetupContent()
		{
			Global.Utility.PostSetupContent();
		}

		public override object Call(params object[] args)
		{
			if (args.Length != 2 || !(args[0] is string command)) return base.Call(args);

			switch (command)
			{
				case "RegisterIngredient" when args[1] is short ID && !Global.Utility.AlchemistBagWhitelist.Contains(ID):
				{
					Global.Utility.AlchemistBagWhitelist.Add(ID);
					Logger.Info($"Ingredient '{ID}' added to Alchemist's Bag whitelist!");
					break;
				}

				case "RegisterOre" when args[1] is short ID && !Global.Utility.OreWhitelist.Contains(ID):
				{
					Global.Utility.OreWhitelist.Add(ID);
					Logger.Info($"Ore '{ID}' added to Ore whitelist!");
					break;
				}
			}

			return base.Call(args);
		}

		public override void PostAddRecipes()
		{
			foreach (ModItem item in this.GetValue<Dictionary<string, ModItem>>("items").Values)
			{
				Recipe recipe = Main.recipe.FirstOrDefault(x => x.createItem.type == item.item.type);
				if (recipe != null) item.item.value = recipe.requiredItem.Sum(x => x.value*x.stack);
			}
		}
	}
}