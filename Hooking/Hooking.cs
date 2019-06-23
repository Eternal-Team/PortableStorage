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
		public static void Load()
		{
			#region On

			ItemSlot.LeftClick_ItemArray_int_int += ItemSlot_LeftClick;
			Player.DropSelectedItem += Player_DropSelectedItem;
			Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;

			#endregion

			#region IL

			IL.Terraria.Player.HasAmmo += Player_HasAmmo;
			IL.Terraria.Player.QuickBuff += Player_QuickBuff;
			IL.Terraria.Player.PickAmmo += Player_PickAmmo;
			IL.Terraria.UI.ItemSlot.DrawSavings += ItemSlot_DrawSavings;
			IL.Terraria.UI.ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
			IL.Terraria.Player.QuickHeal_GetItemToUse += Player_QuickHeal_GetItemToUse;
			IL.Terraria.Player.QuickMana += Player_QuickMana;
			IL.Terraria.Player.SellItem += Player_SellItem;
			IL.Terraria.Player.BuyItem += Player_BuyItem;
			IL.Terraria.Player.ItemCheck += Player_ItemCheck;
			IL.Terraria.Player.FishingLevel += Player_FishingLevel;
			HookEndpointManager.Modify(typeof(Terraria.Player).GetMethod("CanBuyItem", Utility.defaultFlags), new Action<ILContext>(Player_CanBuyItem));

			#endregion
		}
	}
}