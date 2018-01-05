using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Utility;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		public static PortableStorage Instance;

		public Dictionary<Guid, GUI> BagUI = new Dictionary<Guid, GUI>();
		[UI("TileEntity")] public Dictionary<ModTileEntity, GUI> TEUI = new Dictionary<ModTileEntity, GUI>();

		public const string TexturePath = "PortableStorage/Textures/";
		public const string TileTexturePath = TexturePath + "Tiles/";
		public const string ItemTexturePath = TexturePath + "Items/";

		[Null] public static Texture2D[] gemsMiddle = new Texture2D[3];
		[Null] public static Texture2D[] gemsSide = new Texture2D[3];

		[Null] public static Texture2D ringBig;
		[Null] public static Texture2D ringSmall;

		[Null] public static Texture2D vacuumBagOn;
		[Null] public static Texture2D vacuumBagOff;

		[Null] public static ModHotKey bagKey;

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

				ringBig = ModLoader.GetTexture(ItemTexturePath + "RingBig");
				ringSmall = ModLoader.GetTexture(ItemTexturePath + "RingSmall");

				gemsMiddle = new Texture2D[3];
				gemsSide = new Texture2D[3];
				for (int i = 0; i < 3; i++)
				{
					gemsMiddle[i] = ModLoader.GetTexture(TileTexturePath + "GemMiddle" + i);
					gemsSide[i] = ModLoader.GetTexture(TileTexturePath + "GemSide" + i);
				}
			}
		}

		public override void Unload()
		{
			this.UnloadNullableTypes();

			GC.Collect();
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
	}
}