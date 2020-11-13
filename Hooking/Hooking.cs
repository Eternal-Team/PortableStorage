﻿using IL.Terraria;
using IL.Terraria.UI;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		public static void Load()
		{
			Player.HasAmmo += HasAmmo;
			Player.PickAmmo += PickAmmo;
			ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += DrawAmmo;

			ItemSlot.DrawSavings += DrawSavings;

			// On.Terraria.Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;
			// On.Terraria.Player.FishingLevel += Player_FishingLevel;

			// IL.Terraria.Player.QuickBuff += Player_QuickBuff;
			// IL.Terraria.Player.QuickHeal_GetItemToUse += Player_QuickHeal_GetItemToUse;
			// IL.Terraria.Player.QuickMana += Player_QuickMana;
			// IL.Terraria.Player.SellItem += Player_SellItem;
			// IL.Terraria.Player.BuyItem += Player_BuyItem;
			// IL.Terraria.Player.ItemCheck += Player_ItemCheck;
			// IL.Terraria.Player.GetItem += Player_GetItem;

			// IL.Terraria.Main.UpdateTime_SpawnTownNPCs += Main_UpdateTime_SpawnTownNPCs;

			// if (ItemTextBags == null)
			// {
			// ItemTextBags = new Item[20];
			// for (int i = 0; i < ItemTextBags.Length; i++) ItemTextBags[i] = new Item();
			// }

			// IL.Terraria.ItemText.Update += ItemText_Update;
			// IL.Terraria.Main.DoDraw += Main_DoDraw;

			// HookEndpointManager.Modify(typeof(Player).GetMethod("CanBuyItem", BaseLibrary.Utility.defaultFlags), new Action<ILContext>(Player_CanBuyItem));
		}
	}
}