using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Utility = PortableStorage.Global.Utility;

namespace PortableStorage.Items.Bags
{
	public abstract class BaseAmmoBag : BaseBag
	{
		public virtual string AmmoType => null;

		public static readonly string colorAmmoHighlight = new Color(193, 102, 79).ToHex();

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
			Handler.IsItemValid += (handler, slot, item) => Utility.Ammos[AmmoType].Values.SelectMany(x => x).Contains(item.type);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (AmmoType == null) return;

			int type = Utility.Ammos[AmmoType].Values.SelectMany(x => x).ElementAt(0);
			//tooltips.Add(new TooltipLine(mod, "PortableStorage:AmmoInfo", $"Accepts [c/{colorAmmoHighlight}:{BaseLibrary.BaseLibrary.itemCache[type].HoverName}]"));
		}
	}
}