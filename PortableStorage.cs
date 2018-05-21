using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base;
using TheOneLibrary.Base.UI;
using static TheOneLibrary.Utils.Utility;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		public struct Textures
		{
			public const string Path = "PortableStorage/Textures/";
			public const string TilePath = Path + "Tiles/";
			public const string ItemPath = Path + "Items/";
			public const string UIPath = Path + "UI/";

			[Texture(TilePath + "GemMiddle", 3)] public static Texture2D[] gemsMiddle;
			[Texture(TilePath + "GemSide", 3)] public static Texture2D[] gemsSide;
			[Texture(ItemPath + "RingBig")] public static Texture2D ringBig;
			[Texture(ItemPath + "RingSmall")] public static Texture2D ringSmall;
			[Texture(ItemPath + "VacuumBagActive")] public static Texture2D vacuumBagOn;
			[Texture(ItemPath + "VacuumBagInactive")] public static Texture2D vacuumBagOff;
			[Texture(UIPath + "Restack", 2)] public static Texture2D[] restack;
			[Texture(UIPath + "LootAll")] public static Texture2D lootAll;
			[Texture(UIPath + "DepositAll")] public static Texture2D depositAll;
			[Texture(UIPath + "Restock")] public static Texture2D restock;
		}

		public static PortableStorage Instance;
		public static ModHotKey bagKey;

		public GUIs UIs = new GUIs("Vanilla: Hotbar");

		public override void Load()
		{
			Instance = this;

			bagKey = RegisterHotKey("Open Bag", "B");

			TagSerializer.AddSerializer(new FreqSerializer());

			if (!Main.dedServ) LoadTextures();
		}

		public override void PreSaveAndQuit()
		{
			UIs.Clear();
		}

		public override void PostSetupContent()
		{
			Mod MechTransfer = ModLoader.GetMod("MechTransfer");

			if (MechTransfer != null)
			{
				QEAdapter adapter = new QEAdapter(this);
				MechTransfer.Call("RegisterAdapterReflection", adapter, new[] { TileType<QEChest>() });
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Vertebrae, 5);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(ItemID.Leather);
			recipe.AddRecipe();
		}

		public override void UpdateUI(GameTime gameTime)
		{
			UIs.HandleUIsFar();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			UIs.Draw(layers);
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) => Net.HandlePacket(reader, whoAmI);
	}
}