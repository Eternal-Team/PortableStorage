using BaseLibrary;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using On.Terraria;
using On.Terraria.UI;
using System;

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

			Player.DropSelectedItem += Player_DropSelectedItem;
			Player.BuyItem += Player_BuyItem;
			Player.SellItem += Player_SellItem;
			Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;
			Player.PickAmmo += Player_PickAmmo;
			Player.QuickHeal_GetItemToUse += Player_QuickHeal_GetItemToUse;
			Player.QuickMana += Player_QuickMana;

			IL.Terraria.Player.HasAmmo += Player_HasAmmo;
			IL.Terraria.Player.QuickBuff += Player_QuickBuff;

			HookEndpointManager.Modify(typeof(Terraria.Player).GetMethod("CanBuyItem", Utility.defaultFlags), new Action<ILContext>(Player_CanBuyItem));
		}
	}
}