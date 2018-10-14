using System.Linq;
using BaseLibrary.Utility;
using ContainerLibrary;
using PortableStorage.Items.Bags;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace PortableStorage.Global
{
	public class PSPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (PortableStorage.HotkeyBag.JustPressed)
			{
				BaseBag bag = player.Accessory().OfType<BaseBag>().FirstOrDefault();
				if (bag != null) PortableStorage.Instance.PanelUI.UI.HandleBag(bag);
			}
		}

		public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
		{
			if (context != ItemSlot.Context.InventoryItem && context != ItemSlot.Context.InventoryCoin && context != ItemSlot.Context.InventoryAmmo) return false;

			if (!PortableStorage.Instance.PanelUI.UI.Elements.Any()) return false;

			foreach (BaseBagPanel panel in PortableStorage.Instance.PanelUI.UI.Elements.OfType<BaseBagPanel>())
			{
				Item item = inventory[slot];
				if (item.favorited || item.IsAir) return false;

				ItemHandler container = panel.bag.handler;

				for (int i = 0; i < container.Slots; i++)
				{
					inventory[slot] = container.InsertItem(i, item);
					if (inventory[slot].IsAir) break;
				}
			}

			Main.PlaySound(SoundID.Grab);

			return true;
		}
	}
}