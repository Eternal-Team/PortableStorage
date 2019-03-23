using System;
using BaseLibrary;
using MonoMod.RuntimeDetour;
using On.Terraria;
using On.Terraria.UI;
using Terraria.ModLoader;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		public static void Initialize()
		{
			UIElement.GetElementAt += UIElement_GetElementAt;

			ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
			ItemSlot.DrawSavings += ItemSlot_DrawSavings;
			ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
			ItemSlot.OverrideHover += ItemSlot_OverrideHover;

			MonoModHooks.RequestNativeAccess();
			IDetour detour = new Hook(typeof(Terraria.Player).GetMethod("CanBuyItem", Utility.defaultFlags), new Func<Func<Terraria.Player, int, int, bool>, Terraria.Player, int, int, bool>(Player_CanBuyItem));

			Player.DropSelectedItem += Player_DropSelectedItem;
			Player.BuyItem += Player_BuyItem;
			Player.SellItem += Player_SellItem;
			Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;
			Player.HasAmmo += Player_HasAmmo;
			Player.PickAmmo += Player_PickAmmo;
			Player.QuickHeal_GetItemToUse += Player_QuickHeal_GetItemToUse;
			Player.QuickMana += Player_QuickMana;
			Player.QuickBuff += Player_QuickBuff;

			Main.DrawInterface_36_Cursor += Main_DrawInterface_36_Cursor;
		}

		private static bool Player_SellItem(Player.orig_SellItem orig, Terraria.Player self, int price, int stack)
		{
			return orig(self, price, stack);
		}
	}
}