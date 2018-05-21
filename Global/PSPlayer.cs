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
				Item item = player.Accessory().FirstOrDefault(x => x.modItem is BaseBag);
				(item?.modItem as BaseBag)?.HandleUI();
			}
		}

		public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
		{
			if (context != ItemSlot.Context.InventoryItem && context != ItemSlot.Context.InventoryCoin && context != ItemSlot.Context.InventoryAmmo) return false;
			if (!PortableStorage.Instance.UIs.dict.Any()) return false;

			foreach (GUI gui in PortableStorage.Instance.UIs.dict.Values)
			{
				if (gui.ui is IContainerUI)
				{
					Item item = inventory[slot];
					if (item.favorited || item.IsAir) return false;

					IContainer container = ((IContainerUI)gui.ui).GetContainer();

					if (!HasSpace(container.GetItems(), item)) return false;

					foreach (int i in InsertItem(item, container.GetItems())) container.Sync(i);
				}
			}

			Main.PlaySound(7);

			return true;
		}
	}
}