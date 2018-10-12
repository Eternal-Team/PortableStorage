using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContainerLibrary;
using PortableStorage.Items.Bags;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items
{
	public class Wallet : BaseBag
	{
		public override Type UIType => typeof(WalletPanel);

		public Wallet()
		{
			handler = new ItemHandler(4);
			handler.OnContentsChanged += slot =>
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
			handler.IsItemValid += (slot, item) => item.type == ItemID.PlatinumCoin - slot;
			handler.GetSlotLimit += slot => int.MaxValue;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wallet");
			Tooltip.SetDefault("Stores coins\nIt is seemingly bottomless");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
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
	}
}