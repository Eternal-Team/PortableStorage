using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using Terraria;
using Terraria.ID;
using Utility = PortableStorage.Global.Utility;

namespace PortableStorage.Items.Special
{
	public class FishingBelt : BaseBag
	{
		public FishingBelt()
		{
			Handler = new ItemHandler(18);
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
			Handler.IsItemValid += (handler, slot, item) => item.fishingPole > 0 || item.bait > 0 || Utility.FishingWhitelist.Contains(item.type);
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fishing Belt");
			Tooltip.SetDefault("Stores fishing poles, bait and fish");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
		}
	}
}