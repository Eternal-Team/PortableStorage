using System.Collections.Generic;
using ContainerLibrary.Content;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class Bag : BaseBag
	{
		public Bag()
		{
			handler = new ItemHandler(54);
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Storagemaster's Bag");
			Tooltip.SetDefault("Stores 54 stacks of items");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 34;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			//item.value = TheOneLibrary.Utils.Utility.GetItemValue(ItemID.Leather) * 10;
			item.rare = 0;
			item.accessory = true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			//tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag, right-click it or press [c/83fcec:{Utility.GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
		}

		public override TagCompound Save() => new TagCompound
		{
			["Items"] = handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			handler.Load(tag.GetCompound("Items"));
			//if (gui != null && tag.ContainsKey("UIPosition"))
			//{
			//    Vector2 vector = tag.Get<Vector2>("UIPosition");
			//    gui.ui.panelMain.Left.Set(vector.X, 0f);
			//    gui.ui.panelMain.Top.Set(vector.Y, 0f);
			//    gui.ui.panelMain.Recalculate();
			//}
		}

		//public override void NetSend(BinaryWriter writer) => writer.Write(Items);

		//public override void NetRecieve(BinaryReader reader) => Items = TheOneLibrary.Utils.Utility.Read(reader);

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}