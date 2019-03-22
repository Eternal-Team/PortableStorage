using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public class NinjaArsenalBelt : BaseBag
	{
		// todo: implement

		public new NinjaArsenalBeltPanel UI;

		public NinjaArsenalBelt()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot =>
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
			Handler.IsItemValid += (handler, slot, item) => item.thrown;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ninja's Arsenal Belt");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of throwing supplies");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.NinjaShirt);
			recipe.AddIngredient(ItemID.IronChainmail, 3);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}