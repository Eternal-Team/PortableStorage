using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using TheOneLibrary.Utility;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage.Items
{
	public class AmmoBelt : BaseBag, IContainerItem
	{
		public Guid guid = Guid.NewGuid();
		public List<Item> Items = new List<Item>();

		public override string Texture => PortableStorage.ItemTexturePath + "AmmoBelt";

		public override ModItem Clone(Item item)
		{
			AmmoBelt clone = (AmmoBelt)base.Clone(item);
			clone.Items = Items;
			clone.guid = guid;
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

		public override void HandleUI()
		{
			if (!PortableStorage.Instance.BagUI.ContainsKey(guid))
			{
				AmmoBeltUI ui = new AmmoBeltUI();
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
			tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag, right-click it or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
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
						NetUtility.SyncItem(item);
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
						NetUtility.SyncItem(item);
					}
				}
			}
		}

		public override TagCompound Save() => new TagCompound {["Items"] = Items.Save(), ["GUID"] = guid.ToString()};

		public override void Load(TagCompound tag)
		{
			Items = TheOneLibrary.Utility.Utility.Load(tag);
			guid = tag.ContainsKey("GUID") && !string.IsNullOrEmpty((string)tag["GUID"]) ? Guid.Parse(tag.GetString("GUID")) : Guid.NewGuid();
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

		public Item GetItem(int slot) => Items[slot];

		public void SetItem(int slot, Item value)
		{
			Items[slot] = value;
			NetUtility.SyncItem(item);
		}

		public List<Item> GetItems() => Items;

		public ModItem GetModItem() => this;
	}
}