using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ModLoader;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
	#region Regular coins
	private static void CanBuyItem(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		int walletIndex = il.AddVariable<long>();

		if (cursor.TryGotoNext(i => i.MatchLdloca(7), i => i.MatchLdcI4(4)))
		{
			cursor.Emit(OpCodes.Ldarg, 0);

			cursor.EmitDelegate<Func<Player, long>>(player =>
			{
				long coins = 0;

				foreach (Item pItem in player.inventory)
				{
					if (pItem.ModItem is Wallet wallet) coins += wallet.GetItemStorage().CountCoins();
				}

				return coins;
			});

			cursor.Emit(OpCodes.Stloc, walletIndex);

			cursor.Index++;
			cursor.Remove();

			cursor.Emit(OpCodes.Ldc_I4, 5);

			cursor.Index++;
			cursor.Emit(OpCodes.Dup);
			cursor.Emit(OpCodes.Ldc_I4, 4);
			cursor.Emit(OpCodes.Ldloc, walletIndex);
			cursor.Emit(OpCodes.Stelem_I8);
		}
	}

	private static void BuyItem(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		int walletIndex = il.AddVariable<long>();

		if (cursor.TryGotoNext(i => i.MatchLdloca(0), i => i.MatchLdcI4(5)))
		{
			cursor.Emit(OpCodes.Ldarg, 0);

			cursor.EmitDelegate<Func<Player, long>>(player =>
			{
				long coins = 0;

				foreach (Item pItem in player.inventory)
				{
					if (pItem.ModItem is Wallet wallet) coins += wallet.GetItemStorage().CountCoins();
				}

				return coins;
			});

			cursor.Emit(OpCodes.Stloc, walletIndex);

			cursor.Index++;
			cursor.Remove();

			cursor.Emit(OpCodes.Ldc_I4, 6);

			cursor.Index++;
			cursor.Emit(OpCodes.Dup);
			cursor.Emit(OpCodes.Ldc_I4, 5);
			cursor.Emit(OpCodes.Ldloc, walletIndex);
			cursor.Emit(OpCodes.Stelem_I8);
		}

		if (cursor.TryGotoNext(i => i.MatchCall<Player>("TryPurchasing")))
		{
			cursor.Remove();
			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.EmitDelegate<Func<int, List<Item[]>, List<Point>, List<Point>, List<Point>, List<Point>, List<Point>, List<Point>, Player, bool>>(TryPurchasing);
		}
	}

	private static MethodInfo Orig_TryPurchasing = typeof(Player).GetMethod("TryPurchasing", ReflectionUtility.DefaultFlags_Static);

	private static bool TryPurchasing(int price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, List<Point> slotEmptyBank4, Player player)
	{
		int priceRemaining = price;

		foreach (Item pItem in player.inventory)
		{
			if (pItem.ModItem is Wallet wallet)
			{
				ItemStorage storage = wallet.GetItemStorage();
				long walletCoins = storage.CountCoins();
				long sub = Math.Min(walletCoins, priceRemaining);
				priceRemaining -= (int)sub;
				storage.RemoveCoins(player, ref sub);

				if (priceRemaining <= 0) return false;
			}
		}

		return Orig_TryPurchasing.InvokeStatic<bool>(priceRemaining, inv, slotCoins, slotsEmpty, slotEmptyBank, slotEmptyBank2, slotEmptyBank3, slotEmptyBank4);
	}

	private static void SellItem(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		ILLabel label = cursor.DefineLabel();

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(0), i => i.MatchStloc(6), i => i.MatchBr(out _)))
		{
			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.Emit(OpCodes.Ldloc, 3);

			cursor.EmitDelegate<Func<Player, int, bool>>((player, price) =>
			{
				foreach (Item pItem in player.inventory)
				{
					if (!pItem.IsAir && pItem.ModItem is Wallet wallet)
					{
						wallet.GetItemStorage().InsertCoins(player, price);
						return true;
					}
				}

				return false;
			});

			cursor.Emit(OpCodes.Brfalse, label);
			cursor.Emit(OpCodes.Ldc_I4, 1);
			cursor.Emit(OpCodes.Ret);
			cursor.MarkLabel(label);
		}
	}

	private static void DrawSavings(ILContext il)
	{
		int walletIndex = il.AddVariable<long>();

		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(i => i.MatchLdloca(1), i => i.MatchLdcI4(4)))
		{
			cursor.Emit(OpCodes.Ldloc, 0);
			cursor.EmitDelegate<Func<Player, long>>(player =>
			{
				long coins = 0;

				foreach (Item pItem in player.inventory)
				{
					if (pItem.ModItem is Wallet wallet) coins += wallet.GetItemStorage().CountCoins();
				}

				return coins;
			});
			cursor.Emit(OpCodes.Stloc, walletIndex);

			cursor.Index++;
			cursor.Remove();

			cursor.Emit(OpCodes.Ldc_I4, 5);

			cursor.Index++;
			cursor.Emit(OpCodes.Dup);
			cursor.Emit(OpCodes.Ldc_I4, 4);
			cursor.Emit(OpCodes.Ldloc, walletIndex);
			cursor.Emit(OpCodes.Stelem_I8);
		}

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg(0), i => i.MatchLdsfld<Lang>("inter")))
		{
			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.Emit(OpCodes.Ldloc, walletIndex);
			cursor.Emit(OpCodes.Ldarg, 1);
			cursor.Emit(OpCodes.Ldarg, 2);

			cursor.EmitDelegate<Action<SpriteBatch, long, float, float>>((sb, walletCount, shopx, shopy) =>
			{
				if (walletCount > 0L)
				{
					int walletType = ModContent.ItemType<Wallet>();
					Main.instance.LoadItem(walletType);

					Texture2D texture = TextureAssets.Item[walletType].Value;
					sb.Draw(texture, Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 40f), texture.Size() * 0.5f), Color.White);
				}
			});
		}
	}
	#endregion

	#region Custom currencies
	private delegate long BuyItemCustomCurrency_Del(CustomCurrencySystem system, ref bool overflowing, Player player);

	private static void BuyItemCustomCurrency(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		int walletCoins = il.AddVariable<long>();

		// count custom coins in Wallets, add them to local variable
		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(0), i => i.MatchLdloca(1), i => i.MatchLdcI4(5)))
		{
			cursor.Emit(OpCodes.Ldloc, 0);
			cursor.Emit(OpCodes.Ldloca, 1);
			cursor.Emit(OpCodes.Ldarg, 0);

			cursor.EmitDelegate<BuyItemCustomCurrency_Del>((CustomCurrencySystem system, ref bool overflowing, Player player) =>
			{
				long val = 0;

				foreach (Item pItem in player.inventory)
				{
					if (pItem.ModItem is Wallet wallet)
					{
						val += system.CountCurrency(out overflowing, wallet.GetItemStorage().ToArray());
					}
				}

				return val;
			});

			cursor.Emit(OpCodes.Stloc, walletCoins);

			cursor.Index += 2;

			cursor.Remove();
			cursor.Emit(OpCodes.Ldc_I4, 6);
		}

		// add wallet coins to other coins before the price check
		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCallvirt<CustomCurrencySystem>("CombineStacks")))
		{
			cursor.Emit(OpCodes.Dup);
			cursor.Emit(OpCodes.Ldc_I4, 5);
			cursor.Emit(OpCodes.Ldloc, walletCoins);
			cursor.Emit(OpCodes.Stelem_I8);
		}

		// replace CustomCurrentSystem.TryPurchasing with our own
		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCallvirt<CustomCurrencySystem>("TryPurchasing")))
		{
			cursor.Index -= 9;
			cursor.Remove();
			cursor.Index += 8;
			cursor.Remove();

			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.Emit(OpCodes.Ldloc, 0);
			cursor.EmitDelegate<Func<int, List<Item[]>, List<Point>, List<Point>, List<Point>, List<Point>, List<Point>, List<Point>, Player, CustomCurrencySystem, bool>>(TryPurchasingCustomCurrency);
		}
	}

	// todo: display currency not working

	private static bool TryPurchasingCustomCurrency(int price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, List<Point> slotEmptyBank4, Player player, CustomCurrencySystem system)
	{
		int priceRemaining = price;

		foreach (Item pItem in player.inventory)
		{
			if (pItem.ModItem is Wallet wallet)
			{
				ItemStorage storage = wallet.GetItemStorage();

				for (var i = 0; i < storage.Count; i++)
				{
					Item item = storage[i];
					if (system.Accepts(item))
					{
						int toRemove = Math.Min(priceRemaining, item.stack);
						storage.ModifyStackSize(player, i, -toRemove);
						priceRemaining -= toRemove;

						if (priceRemaining <= 0) return true;
					}
				}
			}
		}

		return system.TryPurchasing(priceRemaining, inv, slotCoins, slotsEmpty, slotEmptyBank, slotEmptyBank2, slotEmptyBank3, slotEmptyBank4);
	}
	#endregion
}