using BaseLib.UI;
using ContainerLib2;
using ContainerLib2.Container;
using PortableStorage.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using static BaseLib.Utility.Utility;

namespace PortableStorage.Items
{
	public class AmmoBag : BaseBag, IContainerItem
	{
		public Guid guid = Guid.NewGuid();
		public IList<Item> Items = new List<Item>();

		public override string Texture => PortableStorage.ItemTexturePath + "Bag";

		public override bool CloneNewInstances => true;

		public override ModItem Clone(Item item)
		{
			AmmoBag clone = (AmmoBag)base.Clone(item);
			clone.Items = Items;
			clone.guid = guid;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ammo Bag");
			Tooltip.SetDefault("Use the bag, right-click it or press [c/83fcec:B] while having it in an accessory slot to open it\nRestocks your ammo slots!");
		}

		public override void SetDefaults()
		{
			for (int i = 0; i < 27; i++) Items.Add(new Item());

			item.width = 32;
			item.height = 34;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.value = GetItemValue(ItemID.Leather) * 10;
			item.rare = 1;
			item.accessory = true;
		}

		public override void HandleUI()
		{
			if (!PortableStorage.Instance.BagUI.ContainsKey(guid))
			{
				AmmoBagUI ui = new AmmoBagUI();
				UserInterface userInterface = new UserInterface();
				ui.Activate();
				userInterface.SetState(ui);
				ui.visible = true;
				ui.Load(this);
				PortableStorage.Instance.BagUI.Add(guid, new GUI(ui, userInterface));
			}
			else PortableStorage.Instance.BagUI.Remove(guid);

			Main.PlaySound(SoundID.Item59);
		}

		public override bool UseItem(Player player)
		{
			HandleUI();

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			HandleUI();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine tooltip = tooltips.Find(x => x.mod == "Terraria" && x.Name == "Tooltip0");
			tooltip.text = $"Use the bag, right-click it or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it";
		}

		public override void UpdateInventory(Player player)
		{
			List<Item> list = player.inventory.Where((x, i) => i >= 54 && i <= 57).ToList();

			if (list.Select(x => x.type).Any(x => Items.Select(y => y.type).Contains(x)))
			{
				for (int i = 0; i < Items.Count; i++)
				{
					Item ammo = list.FirstOrDefault(x => x.type == Items[i].type);
					if (ammo != null)
					{
						int count = Math.Min(Items[i].stack, ammo.maxStack - ammo.stack);
						ammo.stack += count;
						Items[i].stack -= count;
						if (Items[i].stack <= 0) Items[i].TurnToAir();
					}
				}
			}
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			List<Item> list = player.inventory.Where((x, i) => i >= 54 && i <= 57).ToList();

			if (list.Select(x => x.type).Any(x => Items.Select(y => y.type).Contains(x)))
			{
				for (int i = 0; i < Items.Count; i++)
				{
					Item ammo = list.FirstOrDefault(x => x.type == Items[i].type);
					if (ammo != null)
					{
						int count = Math.Min(Items[i].stack, ammo.maxStack - ammo.stack);
						ammo.stack += count;
						Items[i].stack -= count;
						if (Items[i].stack <= 0) Items[i].TurnToAir();
					}
				}
			}
		}

		public override TagCompound Save() => new TagCompound { ["Items"] = Items.Save(), ["GUID"] = guid.ToString() };

		public override void Load(TagCompound tag)
		{
			Items = ContainerLib2.Utility.Load(tag);
			guid = tag.ContainsKey("GUID") && !string.IsNullOrEmpty((string)tag["GUID"]) ? Guid.Parse(tag.GetString("GUID")) : Guid.NewGuid();
		}

		public override void NetSend(BinaryWriter writer) => writer.Write(Items);

		public override void NetRecieve(BinaryReader reader) => Items = ContainerLib2.Utility.Read(reader);

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public IList<Item> GetItems() => Items;

		public Item GetItem(int slot) => Items[slot];

		public void SetItem(Item item, int slot) => Items[slot] = item;
	}
}