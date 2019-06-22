using On.Terraria.UI;
using PortableStorage.Items;
using Terraria;
using Player = On.Terraria.Player;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (inv[slot].modItem is BaseBag bag) BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(bag);

			orig(inv, context, slot);
		}

		private static void Player_DropSelectedItem(Player.orig_DropSelectedItem orig, Terraria.Player self)
		{
			if (self.HeldItem.modItem is BaseBag bag) BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(bag);

			orig(self);
		}
	}
}