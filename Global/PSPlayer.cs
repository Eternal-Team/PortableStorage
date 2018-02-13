using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace PortableStorage.Global
{
	public class PSPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (PortableStorage.bagKey.JustPressed)
			{
				Item item = TheOneLibrary.Utils.Utility.Accessory.FirstOrDefault(x => x.modItem is BaseBag);

				if (item?.modItem is BaseBag)
				{
					BaseBag bag = (BaseBag)item.modItem;
					bag.HandleUI();
				}
			}
		}

		public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
		{
			if (context != ItemSlot.Context.InventoryItem && context != ItemSlot.Context.InventoryCoin && context != ItemSlot.Context.InventoryAmmo) return false;
			//if (storageAccess.X < 0 || storageAccess.Y < 0)
			//   {
			//       return false;
			//   }
			//   Item item = inventory[slot];
			//   if (item.favorited || item.IsAir)
			//   {
			//       return false;
			//   }
			//   int oldType = item.type;
			//   int oldStack = item.stack;
			//   if (StorageCrafting())
			//   {
			//       if (Main.netMode == 0)
			//       {
			//           GetCraftingAccess().TryDepositStation(item);
			//       }
			//       else
			//       {
			//           NetHelper.SendDepositStation(GetCraftingAccess().ID, item);
			//           item.SetDefaults(0, true);
			//       }
			//   }
			//   else
			//   {
			//       if (Main.netMode == 0)
			//       {
			//           GetStorageHeart().DepositItem(item);
			//       }
			//       else
			//       {
			//           NetHelper.SendDeposit(GetStorageHeart().ID, item);
			//           item.SetDefaults(0, true);
			//       }
			//   }
			//   if (item.type != oldType || item.stack != oldStack)
			//   {
			//       Main.PlaySound(7, -1, -1, 1, 1f, 0f);
			//       StorageGUI.RefreshItems();
			//   }
			return true;
		}
	}
}