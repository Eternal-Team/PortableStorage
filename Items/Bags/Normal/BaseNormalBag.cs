using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Items.Bags
{
	public abstract class BaseNormalBag : BaseBag
	{
		public abstract int SlotCount { get; }
		public new abstract string Name { get; }

		public new BagPanel UI;

		public BaseNormalBag()
		{
			Handler = new ItemHandler(SlotCount);
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
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault($"{Name} Bag");
			Tooltip.SetDefault($"Stores {SlotCount} stacks of items");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 34;
		}
	}
}