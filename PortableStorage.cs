using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using PortableStorage.Items;
using PortableStorage.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Utility = PortableStorage.Global.Utility;

namespace PortableStorage
{
	// todo: add bag slot

	public class PortableStorage : Mod
	{
		public static PortableStorage Instance;

		public GUI<PanelUI> PanelUI;
		internal List<Guid> BagCache = new List<Guid>();

		public override void Load()
		{
			Instance = this;


			Hooking.Hooking.Initialize();

			if (!Main.dedServ)
			{
				PanelUI = BaseLibrary.Utility.SetupGUI<PanelUI>();
				PanelUI.Visible += () => PanelUI.UI.Elements.Count > 0;
			}
		}

		public override void Unload()
		{
			BaseLibrary.Utility.UnloadNullableTypes();
		}

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

			if (HotbarIndex != -1 && PanelUI != null) layers.Insert(HotbarIndex + 1, PanelUI.InterfaceLayer);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			// note: central behaviour from BaseLibrary

			if (!Main.playerInventory)
			{
				List<IBagPanel> bagPanels = PanelUI.UI.Elements.Cast<IBagPanel>().ToList();
				for (int i = 0; i < bagPanels.Count; i++)
				{
					BaseBag panel = Main.LocalPlayer.inventory.OfType<BaseBag>().FirstOrDefault(x => x.ID == bagPanels[i].ID);
					BagCache.Add(bagPanels[i].ID);
					PanelUI.UI.CloseUI(panel);
				}
			}
			else
			{
				while (BagCache.Count > 0)
				{
					BaseBag panel = Main.LocalPlayer.inventory.OfType<BaseBag>().FirstOrDefault(x => x.ID == BagCache[0]);
					PanelUI.UI.OpenUI(panel);
					BagCache.RemoveAt(0);
				}
			}

			PanelUI?.Update(gameTime);
		}

		public override void PreSaveAndQuit()
		{
			PanelUI?.UI.Elements.Clear();
			BagCache.Clear();
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) => Utility.Networking.HandlePacket(reader, whoAmI);
	}
}