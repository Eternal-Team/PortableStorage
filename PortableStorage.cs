using BaseLib;
using BaseLib.UI;
using BaseLib.Utility;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace PortableStorage
{
	public class PortableStorage : Mod, IMod
	{
		public static PortableStorage Instance;

		public IDictionary<Guid, GUI> BagUI = new Dictionary<Guid, GUI>();
		public IDictionary<ModTileEntity, GUI> TEUI = new Dictionary<ModTileEntity, GUI>();

		public const string TileTexturePath = "PortableStorage/Textures/Tiles/";
		public const string ItemTexturePath = "PortableStorage/Textures/Items/";

		public Texture2D[] gemsMiddle = new Texture2D[3];
		public Texture2D[] gemsSide = new Texture2D[3];

		public Texture2D vacuumBagOn;
		public Texture2D vacuumBagOff;

		public static ModHotKey bagKey;

		public PortableStorage()
		{
			Properties = new ModProperties
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void PreSaveAndQuit()
		{
			BagUI.Clear();
			TEUI.Clear();
		}

		public override void Load()
		{
			Instance = this;

			bagKey = RegisterHotKey("Open Bag", "B");

			TagSerializer.AddSerializer(new FreqSerializer());

			if (!Main.dedServ)
			{
				vacuumBagOn = ModLoader.GetTexture(ItemTexturePath + "VacuumBagActive");
				vacuumBagOff = ModLoader.GetTexture(ItemTexturePath + "VacuumBagInactive");

				for (int i = 0; i < 3; i++)
				{
					gemsMiddle[i] = ModLoader.GetTexture(TileTexturePath + "GemMiddle" + i);
					gemsSide[i] = ModLoader.GetTexture(TileTexturePath + "GemSide" + i);
				}
			}
		}

		public override void Unload()
		{
			Instance = null;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Vertebrae, 5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(ItemID.Leather);
			recipe.AddRecipe();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

			if (MouseTextIndex != -1)
			{
				layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer(
					"PortableStorage: UI",
					delegate
					{
						BagUI.Values.Draw();
						TEUI.Values.Draw();

						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public IDictionary<ModTileEntity, GUI> GetTEUIs() => TEUI;
	}
}