using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader;
using TheOneLibrary.Base;
using TheOneLibrary.Utility;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage
{
    public class PSItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            VacuumBag vacuumBagAcc = (VacuumBag)Accessory.FirstOrDefault(x => x.modItem is VacuumBag && HasSpace(((VacuumBag)x.modItem).Items.ToList(), item))?.modItem;
            VacuumBag vacuumBag = (VacuumBag)player.inventory.FirstOrDefault(x => x.modItem is VacuumBag && HasSpace(((VacuumBag)x.modItem).Items.ToList(), item))?.modItem;

            if (vacuumBagAcc != null && vacuumBagAcc.active)
            {
                InsertItem(item, vacuumBagAcc.Items.ToList());
                NetUtility.SyncItem(vacuumBagAcc.item);
                return false;
            }
            if (vacuumBag != null && vacuumBag.active)
            {
                InsertItem(item, vacuumBag.Items.ToList());
                NetUtility.SyncItem(vacuumBag.item);
                return false;
            }

            return true;
        }
    }
}