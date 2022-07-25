using System;
using System.Collections.Generic;
using System.Linq;
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

	private static bool TryPurchasing(int price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, List<Point> slotEmptyBank4, Player player)
	{
		long priceRemaining = price;

		foreach (Item pItem in player.inventory)
		{
			if (pItem.ModItem is Wallet wallet)
			{
				ItemStorage storage = wallet.GetItemStorage();
				long walletCoins = storage.CountCoins();
				long sub = Math.Min(walletCoins, priceRemaining);
				priceRemaining -= sub;
				storage.RemoveCoins(player, ref sub);

				if (priceRemaining <= 0) return false;
			}
		}

		long num = priceRemaining;
		Dictionary<Point, Item> dictionary = new Dictionary<Point, Item>();
		bool result = false;
		while (num > 0)
		{
			long num2 = 1000000L;
			for (int i = 0; i < 4; i++)
			{
				if (num >= num2)
				{
					foreach (Point slotCoin in slotCoins)
					{
						if (inv[slotCoin.X][slotCoin.Y].type == 74 - i)
						{
							long num3 = num / num2;
							dictionary[slotCoin] = inv[slotCoin.X][slotCoin.Y].Clone();
							if (num3 < inv[slotCoin.X][slotCoin.Y].stack)
							{
								inv[slotCoin.X][slotCoin.Y].stack -= (int)num3;
							}
							else
							{
								inv[slotCoin.X][slotCoin.Y].SetDefaults();
								slotsEmpty.Add(slotCoin);
							}

							num -= num2 * (dictionary[slotCoin].stack - inv[slotCoin.X][slotCoin.Y].stack);
						}
					}
				}

				num2 /= 100;
			}

			if (num <= 0)
				continue;

			if (slotsEmpty.Count > 0)
			{
				slotsEmpty.Sort(DelegateMethods.CompareYReverse);
				Point item = new Point(-1, -1);
				for (int j = 0; j < inv.Count; j++)
				{
					num2 = 10000L;
					for (int k = 0; k < 3; k++)
					{
						if (num >= num2)
						{
							foreach (Point slotCoin2 in slotCoins)
							{
								if (slotCoin2.X == j && inv[slotCoin2.X][slotCoin2.Y].type == 74 - k && inv[slotCoin2.X][slotCoin2.Y].stack >= 1)
								{
									List<Point> list = j switch
									{
										1 when slotEmptyBank.Count > 0 => slotEmptyBank,
										2 when slotEmptyBank2.Count > 0 => slotEmptyBank2,
										3 when slotEmptyBank3.Count > 0 => slotEmptyBank3,
										4 when slotEmptyBank4.Count > 0 => slotEmptyBank4,
										_ => slotsEmpty
									};

									if (--inv[slotCoin2.X][slotCoin2.Y].stack <= 0)
									{
										inv[slotCoin2.X][slotCoin2.Y].SetDefaults();
										list.Add(slotCoin2);
									}

									dictionary[list[0]] = inv[list[0].X][list[0].Y].Clone();
									inv[list[0].X][list[0].Y].SetDefaults(73 - k);
									inv[list[0].X][list[0].Y].stack = 100;
									item = list[0];
									list.RemoveAt(0);
									break;
								}
							}
						}

						if (item.X != -1 || item.Y != -1)
							break;

						num2 /= 100;
					}

					for (int l = 0; l < 2; l++)
					{
						if (item.X != -1 || item.Y != -1)
							continue;

						foreach (Point slotCoin3 in slotCoins)
						{
							if (slotCoin3.X == j && inv[slotCoin3.X][slotCoin3.Y].type == 73 + l && inv[slotCoin3.X][slotCoin3.Y].stack >= 1)
							{
								List<Point> list2 = slotsEmpty;
								if (j == 1 && slotEmptyBank.Count > 0)
									list2 = slotEmptyBank;

								if (j == 2 && slotEmptyBank2.Count > 0)
									list2 = slotEmptyBank2;

								if (j == 3 && slotEmptyBank3.Count > 0)
									list2 = slotEmptyBank3;

								if (j == 4 && slotEmptyBank4.Count > 0)
									list2 = slotEmptyBank4;

								if (--inv[slotCoin3.X][slotCoin3.Y].stack <= 0)
								{
									inv[slotCoin3.X][slotCoin3.Y].SetDefaults();
									list2.Add(slotCoin3);
								}

								dictionary[list2[0]] = inv[list2[0].X][list2[0].Y].Clone();
								inv[list2[0].X][list2[0].Y].SetDefaults(72 + l);
								inv[list2[0].X][list2[0].Y].stack = 100;
								item = list2[0];
								list2.RemoveAt(0);
								break;
							}
						}
					}

					if (item.X != -1 && item.Y != -1)
					{
						slotCoins.Add(item);
						break;
					}
				}

				slotsEmpty.Sort(DelegateMethods.CompareYReverse);
				slotEmptyBank.Sort(DelegateMethods.CompareYReverse);
				slotEmptyBank2.Sort(DelegateMethods.CompareYReverse);
				slotEmptyBank3.Sort(DelegateMethods.CompareYReverse);
				slotEmptyBank4.Sort(DelegateMethods.CompareYReverse);
				continue;
			}

			foreach (KeyValuePair<Point, Item> item2 in dictionary)
			{
				inv[item2.Key.X][item2.Key.Y] = item2.Value.Clone();
			}

			result = true;
			break;
		}

		return result;
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