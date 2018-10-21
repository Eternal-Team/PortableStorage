using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public abstract class BaseAmmoBag : BaseBag
	{
		public override Type UIType => typeof(AmmoBagPanel);
		public virtual string AmmoType => null;

		public static readonly string colorAmmoHighlight = new Color(193, 102, 79).ColorToHex();

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
			Handler.IsItemValid += (handler, slot, item) => PortableStorage.ammoTypes[AmmoType].Contains(item.type);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (AmmoType == null) return;

			int type = PortableStorage.ammoTypes[AmmoType][PortableStorage.tooltipIndexes[AmmoType]];
			tooltips.Add(new TooltipLine(mod, "PortableStorage:AmmoInfo", $"Accepts [c/{colorAmmoHighlight}:{BaseLibrary.BaseLibrary.itemsCache[type].HoverName}]"));
		}
	}
}