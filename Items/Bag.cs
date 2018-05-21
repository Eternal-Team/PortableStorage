using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Base.UI;
using static TheOneLibrary.Utils.Utility;

namespace PortableStorage.Items
{
	public class Bag : BaseBag
	{
		public List<Item> Items = new List<Item>();
		public GUI<BagUI> gui;

		public override string Texture => PortableStorage.Textures.ItemPath + "Bag";

		public override ModItem Clone(Item item)
		{
			Bag clone = (Bag)base.Clone(item);
			clone.Items = Items;
			clone.gui = gui;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bag");
			Tooltip.SetDefault("Stores 54 stacks of items");
		}

		public override void SetDefaults()
		{
			if (!Items.Any())
			{
				for (int i = 0; i < 54; i++) Items.Add(new Item());
			}

			if (Main.netMode != NetmodeID.Server) gui = SetupGUI<BagUI>(this);

			item.width = 26;
			item.height = 34;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.value = GetItemValue(ItemID.Leather) * 10;
			item.rare = 0;
			item.accessory = true;
		}

		public override bool UseItem(Player player)
		{
			if (player.whoAmI == Main.LocalPlayer.whoAmI) this.HandleUI();

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			if (player.whoAmI == Main.LocalPlayer.whoAmI) this.HandleUI();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag, right-click it or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			tag["Items"] = Items.Save();
			if (gui != null) tag["UIPosition"] = gui.ui.panelMain.GetDimensions().Position();
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			Items = TheOneLibrary.Utils.Utility.Load(tag);
			if (gui != null && tag.ContainsKey("UIPosition"))
			{
				Vector2 vector = tag.Get<Vector2>("UIPosition");
				gui.ui.panelMain.Left.Set(vector.X, 0f);
				gui.ui.panelMain.Top.Set(vector.Y, 0f);
				gui.ui.panelMain.Recalculate();
			}
		}

		public override void NetSend(BinaryWriter writer) => writer.Write(Items);

		public override void NetRecieve(BinaryReader reader) => Items = TheOneLibrary.Utils.Utility.Read(reader);

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void OnCraft(Recipe recipe)
		{
			Items.Clear();
			for (int i = 0; i < 54; i++) Items.Add(new Item());
		}

		public override Item GetItem(int slot) => Items[slot];

		public override void SetItem(int slot, Item value) => Items[slot] = value;

		public override void Sync(int slot) => SyncItem(item);

		public override List<Item> GetItems() => Items;

		public override ModItem GetModItem() => this;
	}
}