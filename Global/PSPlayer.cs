﻿using System.Linq;
using ContainerLibrary;
using PortableStorage.UI.Bags;
using PortableStorage.UI.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace PortableStorage.Global
{
	public class PSPlayer : ModPlayer
	{
		public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
		{
			if (context != ItemSlot.Context.InventoryItem && context != ItemSlot.Context.InventoryCoin && context != ItemSlot.Context.InventoryAmmo) return false;

			if (!PortableStorage.Instance.PanelUI.UI.Elements.Any()) return false;

			foreach (UIElement panel in PortableStorage.Instance.PanelUI.UI.Elements)
			{
				Item item = inventory[slot];
				if (item.favorited || item.IsAir) return false;

				ItemHandler container = panel is BaseBagPanel ? ((BaseBagPanel)panel).bag.Handler : ((BaseTEPanel)panel).tileEntity.Handler;

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