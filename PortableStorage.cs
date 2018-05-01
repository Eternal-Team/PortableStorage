using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	[Serializable]
	[AttributeUsage(AttributeTargets.Field)]
	public class PathAttribute : Attribute
	{
		public string Name;
		public int Count;

		public PathAttribute(string name, int count = 0)
		{
			Name = name;
			Count = count;
		}
	}

	public class PortableStorage : Mod
	{
		public struct Textures
		{
			public const string Path = "PortableStorage/Textures/";
			public const string TilePath = Path + "Tiles/";
			public const string ItemPath = Path + "Items/";
			public const string UIPath = Path + "UI/";

			[Null] [Path("GemMiddle", 3)] public static Texture2D[] gemsMiddle;
			[Null] [Path("GemSide", 3)] public static Texture2D[] gemsSide;

			[Null] public static Texture2D ringBig;
			[Null] public static Texture2D ringSmall;

			[Null] public static Texture2D vacuumBagOn;
			[Null] public static Texture2D vacuumBagOff;

			[Null] public static Texture2D lootAll;
			[Null] public static Texture2D depositAll;
			[Null] public static Texture2D[] restack;
			[Null] public static Texture2D restock;
		}

		[Null] public static PortableStorage Instance;

		public Dictionary<ModItem, GUI> BagUI = new Dictionary<ModItem, GUI>();
		[UI("TileEntity")] public Dictionary<ModTileEntity, GUI> TEUI = new Dictionary<ModTileEntity, GUI>();

		[Null] public static ModHotKey bagKey;

		public LegacyGameInterfaceLayer InventoryLayer;

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
				LoadTextures();

				InventoryLayer = new LegacyGameInterfaceLayer("PortableStorage: UI", () => BagUI.Values.Draw() && TEUI.Values.Draw(), InterfaceScaleType.UI);
			}
		}

		public void LoadTextures()
		{
			Textures.lootAll = ModLoader.GetTexture(Textures.UIPath + "LootAll");
			Textures.depositAll = ModLoader.GetTexture(Textures.UIPath + "DepositAll");

			Textures.restack = new Texture2D[2];
			Textures.restack[0] = ModLoader.GetTexture(Textures.UIPath + "Restack_0");
			Textures.restack[1] = ModLoader.GetTexture(Textures.UIPath + "Restack_1");

			Textures.restock = ModLoader.GetTexture(Textures.UIPath + "Restock");

			Textures.vacuumBagOn = ModLoader.GetTexture(Textures.ItemPath + "VacuumBagActive");
			Textures.vacuumBagOff = ModLoader.GetTexture(Textures.ItemPath + "VacuumBagInactive");

			Textures.ringBig = ModLoader.GetTexture(Textures.ItemPath + "RingBig");
			Textures.ringSmall = ModLoader.GetTexture(Textures.ItemPath + "RingSmall");

			Textures.gemsMiddle = new Texture2D[3];
			Textures.gemsSide = new Texture2D[3];
			for (int i = 0; i < 3; i++)
			{
				Textures.gemsMiddle[i] = ModLoader.GetTexture(Textures.TilePath + "GemMiddle_" + i);
				Textures.gemsSide[i] = ModLoader.GetTexture(Textures.TilePath + "GemSide_" + i);
			}
		}

		public override void Unload()
		{
			UnloadNullableTypes();

			GC.Collect();
		}

		public override void PostSetupContent()
		{
			Mod MechTransfer = ModLoader.GetMod("MechTransfer");

			if (MechTransfer != null)
			{
				QEAdapter adapter = new QEAdapter(this);
				MechTransfer.Call("RegisterAdapterReflection", adapter, new[] {TileType<QEChest>()});
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

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int HotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));

			if (HotbarIndex != -1) layers.Insert(HotbarIndex + 1, InventoryLayer);
		}

		public override void PostDrawInterface(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < TEUI.Count; i++) TEUI.ElementAt(i).Value.Draw();
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) => Net.HandlePacket(reader, whoAmI);
	}
}