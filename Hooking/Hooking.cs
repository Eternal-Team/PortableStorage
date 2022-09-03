using IL.Terraria.GameContent.UI;
using IL.Terraria.UI;
using On.Terraria;
using PortableStorage.Items;
using Terraria.ModLoader;
using Main = IL.Terraria.Main;
using Player = IL.Terraria.Player;
using Wiring = IL.Terraria.Wiring;

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

		Wiring.MassWireOperation += WiringOnMassWireOperation;
		Player.ItemCheck_UseWiringTools += PlayerOnItemCheck_UseWiringTools;

		On.Terraria.Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait += PlayerOnItemCheck_CheckFishingBobber_PickAndConsumeBait;
		On.Terraria.Player.Fishing_GetBestFishingPole += PlayerOnFishing_GetBestFishingPole;
		On.Terraria.Player.Fishing_GetBait += PlayerOnFishing_GetBait;

		Player.GrabItems += PlayerOnGrabItems;

		Main.DrawInterface_40_InteractItemIcon += MainOnDrawInterface_40_InteractItemIcon;

		Item.TurnToAir += ItemOnTurnToAir;
		Item.ResetStats += ItemOnResetStats;
	}

	// bug: mouseitem not causing proper deletion
	private static void ItemOnResetStats(Item.orig_ResetStats orig, Terraria.Item self, int type)
	{
		// if (self.ModItem is BaseBag bag)
		// {
		// 	BagSyncSystem.Instance.AllBags.Remove(bag.ID);
		// }

		orig(self, type);
	}

	private static void ItemOnTurnToAir(Item.orig_TurnToAir orig, Terraria.Item self)
	{
		// if (self.ModItem is BaseBag bag)
		// {
		// 	BagSyncSystem.Instance.AllBags.Remove(bag.ID);
		// }

		orig(self);
	}
}