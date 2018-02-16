using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using static TheOneLibrary.Utils.Utility;

namespace PortableStorage.Global
{
	public class PSPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (PortableStorage.bagKey.JustPressed)
			{
				Item item = Accessory.FirstOrDefault(x => x.modItem is BaseBag);

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

			if (!PortableStorage.Instance.BagUI.Values.Concat(PortableStorage.Instance.TEUI.Values).Any()) return false;

			foreach (GUI gui in PortableStorage.Instance.BagUI.Values.Concat(PortableStorage.Instance.TEUI.Values))
			{
				if (gui.key is IContainerUI)
				{
					Item item = inventory[slot];
					if (item.favorited || item.IsAir) return false;

					IContainer container = ((IContainerUI)gui.key).GetContainer();

					if (!HasSpace(container.GetItems(), item)) return false;

					InsertItem(item, container.GetItems());

					if (container is IContainerTile)
					{
						ModTileEntity te = ((IContainerTile)container).GetTileEntity();
						SendTEUpdate(te.ID, te.Position);
					}
					else SyncItem(((IContainerItem)container).GetModItem().item);
				}
			}

			Main.PlaySound(7);

			return true;
		}
	}
}