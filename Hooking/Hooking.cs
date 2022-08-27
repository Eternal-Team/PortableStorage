using IL.Terraria;
using IL.Terraria.GameContent.UI;
using IL.Terraria.UI;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
	public static void Load()
	{
		Player.ChooseAmmo += ChooseAmmo;
		ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlotText;
		Main.UpdateTime_SpawnTownNPCs += SpawnTownNPCs;

		ItemSlot.DrawSavings += DrawSavings;
		CustomCurrencyManager.DrawSavings += DrawSavingsCustomCurrency;
		Player.BuyItem += BuyItem;
		CustomCurrencyManager.BuyItem += BuyItemCustomCurrency;
		Player.CanBuyItem += CanBuyItem;
		Player.SellItem += SellItem;

		Player.AdjTiles += AdjTiles;
		Player.QuickMana += QuickMana;
		Player.QuickHeal_GetItemToUse += QuickHeal;
		Player.QuickBuff_PickBestFoodItem += PickBestFoodItem;
		Player.QuickBuff += QuickBuff;

		On.Terraria.Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait += PlayerOnItemCheck_CheckFishingBobber_PickAndConsumeBait;
		On.Terraria.Player.Fishing_GetBestFishingPole += PlayerOnFishing_GetBestFishingPole;
		On.Terraria.Player.Fishing_GetBait += PlayerOnFishing_GetBait;

		Player.GrabItems += PlayerOnGrabItems;

		Main.DrawInterface_40_InteractItemIcon += MainOnDrawInterface_40_InteractItemIcon;
	}
}