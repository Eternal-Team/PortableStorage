using System;
using BaseLibrary;
using MonoMod.RuntimeDetour;
using On.Terraria.UI;
using Terraria;
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

			MonoModHooks.RequestNativeAccess();
			IDetour detour = new Hook(typeof(Player).GetMethod("CanBuyItem", Utility.defaultFlags), new Func<Func<Player, int, int, bool>, Player, int, int, bool>(Player_CanBuyItem));

			On.Terraria.Player.DropSelectedItem += Player_DropSelectedItem;
			On.Terraria.Player.BuyItem += Player_BuyItem;
			On.Terraria.Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;
			On.Terraria.Player.HasAmmo += Player_HasAmmo;
			On.Terraria.Player.PickAmmo += Player_PickAmmo;
			On.Terraria.Player.QuickHeal_GetItemToUse += Player_QuickHeal_GetItemToUse;
			On.Terraria.Player.QuickMana += Player_QuickMana;
			On.Terraria.Player.QuickBuff += Player_QuickBuff;
		}
	}
}