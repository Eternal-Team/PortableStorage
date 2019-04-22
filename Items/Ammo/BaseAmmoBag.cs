using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using Terraria;
using Terraria.ID;
using Utility = PortableStorage.Global.Utility;

namespace PortableStorage.Items.Ammo
{
	public abstract class BaseAmmoBag : BaseBag
	{
		public abstract string AmmoType { get; }

		public BaseAmmoBag()
		{
			Handler = new ItemHandler(27);
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
			Handler.IsItemValid += (slot, item) => Utility.Ammos[AmmoType].Values.SelectMany(x => x).Contains(item.type);
		}
	}
}