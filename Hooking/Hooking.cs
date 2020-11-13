using IL.Terraria;
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
			Player.BuyItem += BuyItem;
			Player.CanBuyItem += CanBuyItem;
			On.Terraria.Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3, bank4) => false;
			Player.SellItem += SellItem;

			Main.UpdateTime_SpawnTownNPCs += SpawnTownNPCs;

			Player.AdjTiles += AdjTiles;

			Player.QuickMana += QuickMana;
			Player.QuickHeal_GetItemToUse += QuickHeal;
			Player.QuickBuff_PickBestFoodItem += PickBestFoodItem;
			Player.QuickBuff += QuickBuff;

			// On.Terraria.Player.FishingLevel += Player_FishingLevel;
			// IL.Terraria.Player.ItemCheck += Player_ItemCheck;
			// IL.Terraria.Player.GetItem += Player_GetItem;

			// if (ItemTextBags == null)
			// {
			// ItemTextBags = new Item[20];
			// for (int i = 0; i < ItemTextBags.Length; i++) ItemTextBags[i] = new Item();
			// }

			// IL.Terraria.ItemText.Update += ItemText_Update;
			// IL.Terraria.Main.DoDraw += Main_DoDraw;
		}
	}
}