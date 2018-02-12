using System;
using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader;
using TheOneLibrary.Utility;

namespace PortableStorage.Global
{
	public class PSItem : GlobalItem
	{
		public override bool OnPickup(Item item, Player player)
		{
			VacuumBag vacuumBagAcc = (VacuumBag)TheOneLibrary.Utility.Utility.Accessory.FirstOrDefault(x => x.modItem is VacuumBag && TheOneLibrary.Utility.Utility.HasSpace(((VacuumBag)x.modItem).Items.ToList(), item))?.modItem;
			VacuumBag vacuumBag = (VacuumBag)player.inventory.FirstOrDefault(x => x.modItem is VacuumBag && TheOneLibrary.Utility.Utility.HasSpace(((VacuumBag)x.modItem).Items.ToList(), item))?.modItem;
			DevNull devNull = (DevNull)player.inventory.FirstOrDefault(x => x.modItem is DevNull && ((DevNull)x.modItem).Items.Any(y => y.type == item.type))?.modItem;

			if (devNull != null)
			{
				Item insert = devNull.Items.First(x => !x.IsAir && x.type == item.type);
				int count = Math.Min(insert.maxStack - insert.stack, item.stack);
				insert.stack += count;

				NetUtility.SyncItem(devNull.item);

				return false;
			}

			if (vacuumBagAcc != null && vacuumBagAcc.active)
			{
				TheOneLibrary.Utility.Utility.InsertItem(item, vacuumBagAcc.Items.ToList());

				NetUtility.SyncItem(vacuumBagAcc.item);

				return false;
			}
			if (vacuumBag != null && vacuumBag.active)
			{
				TheOneLibrary.Utility.Utility.InsertItem(item, vacuumBag.Items.ToList());

				NetUtility.SyncItem(vacuumBag.item);

				return false;
			}

			return true;
		}
	}
}