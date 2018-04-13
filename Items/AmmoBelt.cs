using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static TheOneLibrary.Utils.Utility;

namespace PortableStorage.Items
{
	public class AmmoBelt : BaseBag
	{
		public List<Item> Items = new List<Item>();

		public override string Texture => PortableStorage.ItemTexturePath + "AmmoBelt";

		public override ModItem Clone(Item item)
		{
			AmmoBelt clone = (AmmoBelt)base.Clone(item);
			clone.Items = Items;
			clone.gui = gui;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ammo Belt");
			Tooltip.SetDefault("Stores 27 stacks of ammo, restocks your ammo slots");
		}

		public override void SetDefaults()
		{
			if (!Items.Any())
			{
				for (int i = 0; i < 27; i++) Items.Add(new Item());
			}

			SetupUI<AmmoBeltUI>();

			item.width = 30;
			item.height = 14;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.value = GetItemValue(ItemID.Leather) * 10;
			item.rare = 1;
			item.accessory = true;
		}

		public override bool UseItem(Player player)
		{
			if (player.whoAmI == Main.LocalPlayer.whoAmI) HandleUI();

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			if (player.whoAmI == Main.LocalPlayer.whoAmI) HandleUI();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag, right-click it or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
		}

		public override void UpdateInventory(Player player)
		{
			if (Ammo.Select(x => x.type).Any(x => Items.Select(y => y.type).Contains(x)))
			{
				for (int i = 0; i < Items.Count; i++)
				{
					Item ammo = Ammo.FirstOrDefault(x => x.type == Items[i].type);
					if (ammo != null)
					{
						int count = Math.Min(Items[i].stack, ammo.maxStack - ammo.stack);
						ammo.stack += count;
						Items[i].stack -= count;
						if (Items[i].stack <= 0) Items[i].TurnToAir();
						Sync();
					}
				}
			}
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (Ammo.Select(x => x.type).Any(x => Items.Select(y => y.type).Contains(x)))
			{
				for (int i = 0; i < Items.Count; i++)
				{
					Item ammo = Ammo.FirstOrDefault(x => x.type == Items[i].type);
					if (ammo != null)
					{
						int count = Math.Min(Items[i].stack, ammo.maxStack - ammo.stack);
						ammo.stack += count;
						Items[i].stack -= count;
						if (Items[i].stack <= 0) Items[i].TurnToAir();
						Sync();
					}
				}
			}
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

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));

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
			for (int i = 0; i < 27; i++) Items.Add(new Item());
		}

		public override Item GetItem(int slot) => Items[slot];

		public override void SetItem(int slot, Item value) => Items[slot] = value;

		public override void Sync(int slot = 0) => SyncItem(item);

		public override List<Item> GetItems() => Items;

		public override ModItem GetModItem() => this;
	}
}