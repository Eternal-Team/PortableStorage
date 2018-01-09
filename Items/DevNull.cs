using PortableStorage.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage.Items
{
	public class DevNull : BaseBag, IContainerItem
	{
		public Guid guid = Guid.NewGuid();
		public IList<Item> Items = new List<Item>();

		public Item selectedItem;

		//public override string Texture => PortableStorage.ItemTexturePath + "AmmoBelt";

		public override ModItem Clone(Item item)
		{
			DevNull clone = (DevNull)base.Clone(item);
			clone.Items = Items;
			clone.guid = guid;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("/dev/null");
			//Tooltip.SetDefault("Restocks your ammo slots!");
		}

		public override void SetDefaults()
		{
			Items.Clear();
			for (int i = 0; i < 27; i++) Items.Add(new Item());

			item.width = 30;
			item.height = 14;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.createTile = TileID.Stone;
			//item.value = GetItemValue(ItemID.Leather) * 10;
			item.rare = 1;
			item.accessory = true;
		}

		public override void HandleUI()
		{
			if (!PortableStorage.Instance.BagUI.ContainsKey(guid))
			{
				DevNullUI ui = new DevNullUI();
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
			if (selectedItem != null)
			{
				Main.NewText("Item: " + selectedItem);
				Main.NewText("Create Tile: " + selectedItem.createTile);
				Main.NewText("Place style: " + selectedItem.useStyle);
			}

			//HandleUI();

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

		public override TagCompound Save() => new TagCompound { ["Items"] = Items.Save(), ["GUID"] = guid.ToString() };

		public override void Load(TagCompound tag)
		{
			Items = TheOneLibrary.Utility.Utility.Load(tag);
			guid = tag.ContainsKey("GUID") && !string.IsNullOrEmpty((string)tag["GUID"]) ? Guid.Parse(tag.GetString("GUID")) : Guid.NewGuid();
		}

		public override void NetSend(BinaryWriter writer) => writer.Write(Items);

		public override void NetRecieve(BinaryReader reader) => Items = TheOneLibrary.Utility.Utility.Read(reader);

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

		public IList<Item> GetItems() => Items;
	}
}