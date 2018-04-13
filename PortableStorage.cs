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
using TheOneLibrary.Utils;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		public static PortableStorage Instance;

		public Dictionary<ModItem, GUI> BagUI = new Dictionary<ModItem, GUI>();
		[UI("TileEntity")] public Dictionary<ModTileEntity, GUI> TEUI = new Dictionary<ModTileEntity, GUI>();

		public const string TexturePath = "PortableStorage/Textures/";
		public const string TileTexturePath = TexturePath + "Tiles/";
		public const string ItemTexturePath = TexturePath + "Items/";
		public const string UITexturePath = TexturePath + "UI/";

		[Null] public static Texture2D[] gemsMiddle;
		[Null] public static Texture2D[] gemsSide;

		[Null] public static Texture2D ringBig;
		[Null] public static Texture2D ringSmall;

		[Null] public static Texture2D vacuumBagOn;
		[Null] public static Texture2D vacuumBagOff;

		[Null] public static Texture2D lootAll;
		[Null] public static Texture2D depositAll;
		[Null] public static Texture2D[] restack;
		[Null] public static Texture2D restock;

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

				InventoryLayer = new LegacyGameInterfaceLayer("PortableStorage: UI", delegate
				{
					BagUI.Values.Draw();
					TEUI.Values.Draw();

					return true;
				}, InterfaceScaleType.UI);
			}
		}

		public void LoadTextures()
		{
			lootAll = ModLoader.GetTexture(UITexturePath + "LootAll");
			depositAll = ModLoader.GetTexture(UITexturePath + "DepositAll");

			restack = new Texture2D[2];
			restack[0] = ModLoader.GetTexture(UITexturePath + "Restack_0");
			restack[1] = ModLoader.GetTexture(UITexturePath + "Restack_1");

			restock = ModLoader.GetTexture(UITexturePath + "Restock");

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

		public override void Unload()
		{
			this.UnloadNullableTypes();

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
			for (int i = 0; i < TEUI.Count; i++)
			{
				KeyValuePair<ModTileEntity, GUI> pair = TEUI.ElementAt(i);
				pair.Key.HandleUIFar();
			}
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) => Net.HandlePacket(reader, whoAmI);
	}
}