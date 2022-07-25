using IL.Terraria;
using IL.Terraria.GameContent.UI;
using IL.Terraria.UI;
using MonoMod.Cil;
using CustomCurrencySystem = On.Terraria.GameContent.UI.CustomCurrencySystem;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
	public static void Load()
	{
		Player.ChooseAmmo += ChooseAmmo;
		ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlotText;
		Main.UpdateTime_SpawnTownNPCs += SpawnTownNPCs;

		ItemSlot.DrawSavings += DrawSavings;
		Player.BuyItem += BuyItem;
		CustomCurrencyManager.BuyItem += BuyItemCustomCurrency;
		Player.CanBuyItem += CanBuyItem;
		On.Terraria.Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3, bank4) => false;
		Player.SellItem += SellItem;

		Player.AdjTiles += AdjTiles;
		Player.QuickMana += QuickMana;
		Player.QuickHeal_GetItemToUse += QuickHeal;
		Player.QuickBuff_PickBestFoodItem += PickBestFoodItem;
		Player.QuickBuff += QuickBuff;

		On.Terraria.Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait += PlayerOnItemCheck_CheckFishingBobber_PickAndConsumeBait;
		On.Terraria.Player.Fishing_GetBestFishingPole += PlayerOnFishing_GetBestFishingPole;
		On.Terraria.Player.Fishing_GetBait += PlayerOnFishing_GetBait;

		// ItemTextBags = new Terraria.Item[Terraria.Main.popupText.Length];
		// for (int i = 0; i < ItemTextBags.Length; i++) ItemTextBags[i] = new Terraria.Item();
		// Main.DoDraw += DoDraw;
	}
}