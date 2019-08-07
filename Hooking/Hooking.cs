using IL.Terraria.UI;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using PortableStorage.Items.Special;
using ReLogic.OS;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Player = On.Terraria.Player;

namespace PortableStorage
{
	public static partial class Hooking
	{
		public static void Load()
		{
			ContainerLibrary.Hooking.AlchemyApplyChance += () => Main.LocalPlayer.inventory.Any(item => item.modItem is AlchemistBag);
			ContainerLibrary.Hooking.ModifyAdjTiles += player => player.adjTile[TileID.Bottles] = player.inventory.Any(item => item.modItem is AlchemistBag);

			Player.TryPurchasing += (orig, price, inv, coins, empty, bank, bank2, bank3) => false;

			IL.Terraria.Player.HasAmmo += Player_HasAmmo;
			IL.Terraria.Player.QuickBuff += Player_QuickBuff;
			IL.Terraria.Player.PickAmmo += Player_PickAmmo;
			ItemSlot.DrawSavings += ItemSlot_DrawSavings;
			ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color;
			IL.Terraria.Player.QuickHeal_GetItemToUse += Player_QuickHeal_GetItemToUse;
			IL.Terraria.Player.QuickMana += Player_QuickMana;
			IL.Terraria.Player.SellItem += Player_SellItem;
			 IL.Terraria.Player.BuyItem += Player_BuyItem;
			IL.Terraria.Player.ItemCheck += Player_ItemCheck;
			IL.Terraria.Player.FishingLevel += Player_FishingLevel;
			IL.Terraria.Player.GetItem += Player_GetItem;
			 HookEndpointManager.Modify(typeof(Terraria.Player).GetMethod("CanBuyItem", BaseLibrary.Utility.defaultFlags), new Action<ILContext>(Player_CanBuyItem));
		}
	}
}