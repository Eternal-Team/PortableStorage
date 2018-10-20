using System.Collections.Generic;
using System.Linq;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using On.Terraria;
using PortableStorage.Global;
using PortableStorage.TileEntities;
using PortableStorage.UI;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using static BaseLibrary.BaseLibrary;
using ItemSlot = On.Terraria.UI.ItemSlot;
using Main = Terraria.Main;
using Recipe = Terraria.Recipe;
using UIElement = On.Terraria.UI.UIElement;
using Utility = BaseLibrary.Utility.Utility;

namespace PortableStorage
{
	public partial class PortableStorage : Mod
	{
		public static PortableStorage Instance;

		public static int BagID;
		public static int timer;

		[PathOverride("PortableStorage/Textures/Tiles/QEChest_Glow")]
		public static Texture2D QE_Glow { get; set; }

		[PathOverride("PortableStorage/Textures/Tiles/GemMiddle_0")]
		public static Texture2D QE_Gems { get; set; }

		public GUI<PanelUI> PanelUI;

		public static ModHotKey HotkeyBag;

		public static Dictionary<string, List<int>> ammoTypes;
		public static Dictionary<string, int> tooltipIndexes;

		public override void Load()
		{
			Instance = this;

			TagSerializer.AddSerializer(new FrequencySerializer());

			UIElement.GetElementAt += UIElement_GetElementAt;
			ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
			ItemSlot.DrawSavings += ItemSlot_DrawSavings;
			ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
			Player.CanBuyItem += Player_CanBuyItem;
			Player.BuyItem += Player_BuyItem;
			Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;
			Player.HasAmmo += Player_HasAmmo;
			Player.PickAmmo += Player_PickAmmo;
			Player.QuickHeal_GetItemToUse += Player_QuickHeal_GetItemToUse;
			Player.QuickMana += Player_QuickMana;
			Player.QuickBuff += Player_QuickBuff;
			Player.DropSelectedItem += Player_DropSelectedItem;

			HotkeyBag = this.Register("Open Bag", Keys.B);

			if (!Main.dedServ)
			{
				this.LoadTextures();

				PanelUI = Utility.SetupGUI<PanelUI>();
				PanelUI.Visible = true;
			}
		}

		public override void Unload()
		{
			Utility.UnloadNullableTypes();
		}

		public override void PostSetupContent()
		{
			List<int> miscTypes = new List<int> { AmmoID.FallenStar, AmmoID.Sand, AmmoID.Snowball, AmmoID.CandyCorn, AmmoID.Stake };
			List<int> flameableTypes = new List<int> { AmmoID.Rocket, AmmoID.Gel, AmmoID.Flare, AmmoID.StyngerBolt, AmmoID.JackOLantern };

			ammoTypes = new Dictionary<string, List<int>>
			{
				["Misc"] = itemsCache.Where(item => miscTypes.Contains(item.ammo)).Select(item => item.type).ToList(),
				["Arrow"] = itemsCache.Where(item => item.ammo == AmmoID.Arrow).Select(item => item.type).ToList(),
				["Dart"] = itemsCache.Where(item => item.ammo == AmmoID.Dart).Select(item => item.type).ToList(),
				["Flameable"] = itemsCache.Where(item => flameableTypes.Contains(item.ammo)).Select(item => item.type).ToList(),
				["Bullet"] = itemsCache.Where(item => item.ammo == AmmoID.Bullet).Select(x => x.type).ToList(),
				["Solution"] = itemsCache.Where(item => item.ammo == AmmoID.Solution).Select(item => item.type).ToList(),
				["Coin"] = itemsCache.Where(item => item.ammo == AmmoID.Coin).Select(item => item.type).ToList(),
				["All"] = itemsCache.Where(item => item.ammo > 0).Select(item => item.type).ToList()
			};

			tooltipIndexes = new Dictionary<string, int>();
			foreach (var ammoType in ammoTypes) tooltipIndexes.Add(ammoType.Key, 0);
		}

		public override void PostAddRecipes()
		{
			foreach (ModItem item in this.GetValue<Dictionary<string, ModItem>>("items").Values)
			{
				Recipe recipe = Main.recipe.FirstOrDefault(x => x.createItem.type == item.item.type);
				if (recipe != null) item.item.value = recipe.requiredItem.Sum(x => x.value);
			}
		}

		public override void PreSaveAndQuit()
		{
			PanelUI.UI.Elements.Clear();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int HotbarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Hotbar"));

			if (HotbarIndex != -1 && PanelUI != null) layers.Insert(HotbarIndex + 1, PanelUI.InterfaceLayer);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (TileEntity.ByID.Values.OfType<TEQEChest>().Any(x => x.inScreen && !x.hovered))
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
			}

			if (++timer > 60)
			{
				timer = 0;
				for (int i = 0; i < tooltipIndexes.Count; i++)
				{
					string key = tooltipIndexes.Keys.ElementAt(i);
					tooltipIndexes[key]++;
					if (tooltipIndexes[key] > ammoTypes[key].Count - 1) tooltipIndexes[key] = 0;
				}
			}
		}
	}
}