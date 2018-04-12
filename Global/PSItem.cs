using System;
using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static TheOneLibrary.Utils.Utility;

namespace PortableStorage.Global
{
	public class PSItem : GlobalItem
	{
		public override bool OnPickup(Item item, Player player)
		{
			VacuumBag vacuumBagAcc = (VacuumBag)Accessory.FirstOrDefault(x => x.modItem is VacuumBag && HasSpace(((VacuumBag)x.modItem).Items.ToList(), item))?.modItem;
			VacuumBag vacuumBag = (VacuumBag)player.inventory.FirstOrDefault(x => x.modItem is VacuumBag && HasSpace(((VacuumBag)x.modItem).Items.ToList(), item))?.modItem;
			DevNull devNull = (DevNull)player.inventory.FirstOrDefault(x => x.modItem is DevNull && ((DevNull)x.modItem).Items.Any(y => y.type == item.type))?.modItem;

			if (devNull != null)
			{
				Item insert = devNull.Items.First(x => !x.IsAir && x.type == item.type);
				int count = Math.Min(insert.maxStack - insert.stack, item.stack);
				insert.stack += count;

				SyncItem(devNull.item);

				return false;
			}

			if (vacuumBagAcc != null && vacuumBagAcc.active && item.type != ItemID.Heart && item.type != ItemID.Star && !item.IsCoin())
			{
				foreach (int i in InsertItem(item, vacuumBagAcc.Items)) ;
				Main.PlaySound(SoundID.DD2_EtherianPortalOpen.WithVolume(0.05f));

				SyncItem(vacuumBagAcc.item);

				return false;
			}

			if (vacuumBag != null && vacuumBag.active && item.type != ItemID.Heart && item.type != ItemID.Star && !item.IsCoin())
			{
				foreach (int i in InsertItem(item, vacuumBag.Items)) ;
				Main.PlaySound(SoundID.DD2_EtherianPortalOpen.WithVolume(0.05f));

				SyncItem(vacuumBag.item);

				return false;
			}

			return true;
		}
	}
}