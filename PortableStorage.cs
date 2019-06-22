using BaseLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items.Special;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace PortableStorage
{
	// todo: add bag slot
	// todo: add fluid + energy addons

	public class PortableStorage : Mod
	{
		public static PortableStorage Instance;

		//public GUI<PanelUI> PanelUI;
		internal List<Guid> BagCache = new List<Guid>();

		public override void Load()
		{
			Instance = this;

			Hooking.Hooking.Initialize();

			if (!Main.dedServ)
			{
				//PanelUI = Utility.SetupGUI<PanelUI>();
				//PanelUI.Visible += () => PanelUI.UI.Elements.Count > 0;

				//ContainerLibrary.ContainerLibrary.CheckAlchemy += () => (33, Main.LocalPlayer.inventory.OfType<AlchemistBag>().Any());
				//ContainerLibrary.ContainerLibrary.ModifyAdjTiles += () => Main.LocalPlayer.adjTile[TileID.Bottles] = true;
			}
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
				if (recipe != null) item.item.value = recipe.requiredItem.Sum(x => x.value);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int HotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));

			//if (HotbarIndex != -1 && PanelUI != null) layers.Insert(HotbarIndex + 1, PanelUI.InterfaceLayer);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			// note: central behaviour from BaseLibrary? interface (IItemUI) or attributes

			//if (!Main.playerInventory)
			//{
			//	List<IBagPanel> bagPanels = PanelUI.UI.Elements.Cast<IBagPanel>().ToList();
			//	for (int i = 0; i < bagPanels.Count; i++)
			//	{
			//		BaseBag panel = Main.LocalPlayer.inventory.OfType<BaseBag>().FirstOrDefault(x => x.ID == bagPanels[i].ID);
			//		BagCache.Add(bagPanels[i].ID);
			//		PanelUI.UI.CloseUI(panel);
			//	}
			//}
			//else
			//{
			//	while (BagCache.Count > 0)
			//	{
			//		BaseBag panel = Main.LocalPlayer.inventory.OfType<BaseBag>().FirstOrDefault(x => x.ID == BagCache[0]);
			//		PanelUI.UI.OpenUI(panel);
			//		BagCache.RemoveAt(0);
			//	}
			//}

			//PanelUI?.Update(gameTime);
		}

		public override void PreSaveAndQuit()
		{
			//PanelUI?.UI.Elements.Clear();
			BagCache.Clear();
		}
	}
}