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
	public class PortableStorage : Mod
	{
		[Texture]
		public struct Textures
		{
			public const string Path = "PortableStorage/Textures/";
			public const string TilePath = Path + "Tiles/";
			public const string ItemPath = Path + "Items/";
			public const string UIPath = Path + "UI/";

			[Null, Texture(TilePath + "GemMiddle", 3)] public static Texture2D[] gemsMiddle;
			[Null, Texture(TilePath + "GemSide", 3)] public static Texture2D[] gemsSide;
			[Null, Texture(ItemPath + "RingBig")] public static Texture2D ringBig;
			[Null, Texture(ItemPath + "RingSmall")] public static Texture2D ringSmall;
			[Null, Texture(ItemPath + "VacuumBagActive")] public static Texture2D vacuumBagOn;
			[Null, Texture(ItemPath + "VacuumBagInactive")] public static Texture2D vacuumBagOff;
			[Null, Texture(UIPath + "Restack", 2)] public static Texture2D[] restack;
			[Null, Texture(UIPath + "LootAll")] public static Texture2D lootAll;
			[Null, Texture(UIPath + "DepositAll")] public static Texture2D depositAll;
			[Null, Texture(UIPath + "Restock")] public static Texture2D restock;
		}

		[Null] public static PortableStorage Instance;

		public Dictionary<ModItem, GUI> BagUI = new Dictionary<ModItem, GUI>();
		[UI("TileEntity")] public Dictionary<ModTileEntity, GUI> TEUI = new Dictionary<ModTileEntity, GUI>();

		[Null] public static ModHotKey bagKey;

		public LegacyGameInterfaceLayer InventoryLayer;

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

		public override void Unload()
		{
			UnloadNullableTypes();
		}

		public override void PreSaveAndQuit()
		{
			BagUI.Clear();
			TEUI.Clear();
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