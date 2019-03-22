using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using PortableStorage.Global;
using PortableStorage.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace PortableStorage
{
	// todo: add bag slot

	public class PortableStorage : Mod
	{
		public static PortableStorage Instance;

		public GUI<PanelUI> PanelUI;

		public override void Load()
		{
			Instance = this;

			TagSerializer.AddSerializer(new FrequencySerializer());

			Hooking.Hooking.Initialize();

			if (!Main.dedServ)
			{
				PanelUI = BaseLibrary.Utility.SetupGUI<PanelUI>();
				PanelUI.Visible = true;
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
			if (args.Length < 1 || !(args[0] is string command)) return base.Call(args);

			switch (command)
			{
				case "RegisterIngredient" when args.Length == 2 && args[1] is short ID && !Utility.AlchemistBagWhitelist.Contains(ID):
				{
					Utility.AlchemistBagWhitelist.Add(ID);
					Logger.Info($"Ingredient '{ID}' added to Alchemist's Bag whitelist!");
					break;
				}
			}

			return base.Call(args);
		}

		public override void PostAddRecipes()
		{
			foreach (ModItem item in BaseLibrary.Utility.GetValue<Dictionary<string, ModItem>>(this, "items").Values)
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
			PanelUI?.Update(gameTime);
		}

		public override void PreSaveAndQuit()
		{
			PanelUI?.UI.Elements.Clear();
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) => Utility.Networking.HandlePacket(reader, whoAmI);
	}
}