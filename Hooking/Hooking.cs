using Humanizer;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.UI;
using Main = Terraria.IL_Main;
using Player = Terraria.IL_Player;
using Wiring = Terraria.IL_Wiring;
using WorldGen = Terraria.IL_WorldGen;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
	public static void Load()
	{
		// Player.ChooseAmmo += ChooseAmmo;
		// IL_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlotText;
		Main.UpdateTime_SpawnTownNPCs += SpawnTownNPCs;

		// IL_ItemSlot.DrawSavings += DrawSavings;
		// IL_CustomCurrencyManager.DrawSavings += DrawSavingsCustomCurrency;
		// Player.BuyItem += BuyItem;
		// IL_CustomCurrencyManager.BuyItem += BuyItemCustomCurrency;
		// // On_Player.CanBuyItem += CanBuyItem;
		// Player.SellItem += SellItem;
		//
		// Player.AdjTiles += AdjTiles;
		// Player.QuickMana += QuickMana;
		// Player.QuickHeal_GetItemToUse += QuickHeal;
		// Player.QuickBuff_PickBestFoodItem += PickBestFoodItem;
		// Player.QuickBuff += QuickBuff;
		//
		// Wiring.MassWireOperation += WiringOnMassWireOperation;
		// Player.ItemCheck_UseWiringTools += PlayerOnItemCheck_UseWiringTools;
		//
		// On_Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait += PlayerOnItemCheck_CheckFishingBobber_PickAndConsumeBait;
		// On_Player.Fishing_GetBestFishingPole += PlayerOnFishing_GetBestFishingPole;
		// On_Player.Fishing_GetBait += PlayerOnFishing_GetBait;

		Player.PickupItem += PlayerOnPickupItem;

		// Main.DrawInterface_40_InteractItemIcon += MainOnDrawInterface_40_InteractItemIcon;

		// Item.TurnToAir += ItemOnTurnToAir;
		// Item.ResetStats += ItemOnResetStats;
		// IL_ItemSlot.HandleShopSlot += ItemSlotOnHandleShopSlot;

		WorldGen.KillTile_GetItemDrops += WorldGenOnKillTile_GetItemDrops;
	}

	// bug: mouseitem not causing proper deletion
	// private static void ItemOnResetStats(Item.orig_ResetStats orig, Terraria.Item self, int type)
	// {
	// 	// if (self.ModItem is BaseBag bag)
	// 	// {
	// 	// 	BagSyncSystem.Instance.AllBags.Remove(bag.ID);
	// 	// }
	//
	// 	orig(self, type);
	// }
	//
	// private static void ItemOnTurnToAir(Item.orig_TurnToAir orig, Terraria.Item self)
	// {
	// 	// if (self.ModItem is BaseBag bag)
	// 	// {
	// 	// 	BagSyncSystem.Instance.AllBags.Remove(bag.ID);
	// 	// }
	//
	// 	orig(self);
	// }
}