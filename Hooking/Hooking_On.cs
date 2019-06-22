using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using On.Terraria.UI;
using PortableStorage.Items;
using Terraria;
using Player = On.Terraria.Player;
using UIElement = Terraria.UI.UIElement;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (inv[slot].modItem is BaseBag bag) PortableStorage.Instance.PanelUI.UI.CloseUI(bag);

			orig(inv, context, slot);
		}

		private static void Player_DropSelectedItem(Player.orig_DropSelectedItem orig, Terraria.Player self)
		{
			if (self.HeldItem.modItem is BaseBag bag) PortableStorage.Instance.PanelUI.UI.CloseUI(bag);
		}

		private static UIElement UIElement_GetElementAt(On.Terraria.UI.UIElement.orig_GetElementAt orig, UIElement self, Vector2 point)
		{
			if (self is PanelUI ui)
			{
				UIElement uIElement = null;
				for (int i = ui.Elements.Count - 1; i >= 0; i--)
				{
					UIElement current = ui.Elements[i];
					if (current.ContainsPoint(point))
					{
						uIElement = current;
						break;
					}
				}

				if (uIElement != null) return uIElement.GetElementAt(point);
				return self.ContainsPoint(point) ? self : null;
			}

			return orig(self, point);
		}
	}
}