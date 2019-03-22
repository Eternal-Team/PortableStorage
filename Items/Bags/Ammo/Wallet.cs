using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Items.Bags
{
	public class Wallet : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/Wallet";
		public override string AmmoType => "Coin";

		public new WalletPanel UI;

		public Wallet()
		{
			Handler = new ItemHandler(4);
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
			Handler.IsItemValid += (handler, slot, item) => item.type == ItemID.PlatinumCoin - slot;
			Handler.GetSlotLimit += slot => int.MaxValue;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wallet");
			Tooltip.SetDefault("Stores coins\nIt is seemingly bottomless!");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 14;
			item.value = Item.buyPrice(gold: 5);
		}
	}
}