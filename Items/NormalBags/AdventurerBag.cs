using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContainerLibrary.Content;
using PortableStorage.Items.Bags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.NormalBags
{
	public class AdventurerBag : BaseBag
	{
		public AdventurerBag()
		{
			handler = new ItemHandler(18);
			handler.OnContentsChanged += (handler, slot) =>
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					Player player = Main.player[item.owner];

					List<Item> joined = player.inventory.Concat(player.armor).Concat(player.dye).Concat(player.miscEquips).Concat(player.miscDyes).Concat(player.bank.item).Concat(player.bank2.item).Concat(new[] { player.trashItem }).Concat(player.bank3.item).ToList();
					int index = joined.FindIndex(x => x == item);
					if (index < 0) return;

					NetMessage.SendData(MessageID.SyncEquipment, number: item.owner, number2: index);
				}
			};
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Adventurer's Bag");
			Tooltip.SetDefault("Stores 18 stacks of items");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 34;
			item.rare = 1;
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
	}
}