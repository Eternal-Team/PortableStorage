using System;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		// private static void Player_BuyItem(ILContext il)
		// {
		// 	ILCursor cursor = new ILCursor(il);
		//
		// 	if (cursor.TryGotoNext(i => i.MatchLdloca(0), i => i.MatchLdcI4(4)))
		// 	{
		// 		cursor.RemoveRange(20);
		//
		// 		cursor.Emit(OpCodes.Ldarg_0);
		// 		cursor.Emit(OpCodes.Ldloc, 1);
		// 		cursor.Emit(OpCodes.Ldloc, 2);
		// 		cursor.Emit(OpCodes.Ldloc, 3);
		// 		cursor.Emit(OpCodes.Ldloc, 4);
		//
		// 		cursor.EmitDelegate<Func<Player, long, long, long, long, long>>((player, inventory, bank, bank2, bank3) =>
		// 		{
		// 			long coins = player.inventory.OfType<Wallet>().Sum(wallet => wallet.Coins);
		// 			coins += inventory;
		// 			coins += bank;
		// 			coins += bank2;
		// 			coins += bank3;
		// 			return coins;
		// 		});
		// 	}
		//
		// 	if (cursor.TryGotoNext(i => i.MatchCall<Player>("TryPurchasing")))
		// 	{
		// 		cursor.Remove();
		// 		cursor.Emit(OpCodes.Ldarg, 0);
		// 		cursor.EmitDelegate<Func<int, List<Item[]>, List<Point>, List<Point>, List<Point>, List<Point>, List<Point>, Player, bool>>(Player_TryPurchasing);
		// 	}
		// }
		//
		// private static void Player_CanBuyItem(ILContext il)
		// {
		// 	ILCursor cursor = new ILCursor(il);
		//
		// 	if (cursor.TryGotoNext(i => i.MatchLdloca(0), i => i.MatchLdcI4(4)))
		// 	{
		// 		cursor.RemoveRange(20);
		//
		// 		cursor.Emit(OpCodes.Ldarg_0);
		// 		cursor.Emit(OpCodes.Ldloc, 1);
		// 		cursor.Emit(OpCodes.Ldloc, 2);
		// 		cursor.Emit(OpCodes.Ldloc, 3);
		// 		cursor.Emit(OpCodes.Ldloc, 4);
		//
		// 		cursor.EmitDelegate<Func<Player, long, long, long, long, long>>((player, inventory, bank, bank2, bank3) =>
		// 		{
		// 			long coins = player.inventory.OfType<Wallet>().Sum(wallet => wallet.Coins);
		// 			coins += inventory;
		// 			coins += bank;
		// 			coins += bank2;
		// 			coins += bank3;
		// 			return coins;
		// 		});
		// 	}
		// }
		//
		// private static void Player_SellItem(ILContext il)
		// {
		// 	ILCursor cursor = new ILCursor(il);
		// 	ILLabel label = cursor.DefineLabel();
		//
		// 	if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(2), i => i.MatchBr(out _)))
		// 	{
		// 		cursor.Emit(OpCodes.Ldarg, 0);
		// 		cursor.Emit(OpCodes.Ldloc, 1);
		//
		// 		cursor.EmitDelegate<Func<Player, int, bool>>((player, price) =>
		// 		{
		// 			Wallet wallet = player.inventory.OfType<Wallet>().FirstOrDefault();
		//
		// 			if (wallet != null)
		// 			{
		// 				long addedCoins = price + wallet.Coins;
		// 				wallet.Coins = addedCoins;
		//
		// 				return true;
		// 			}
		//
		// 			return false;
		// 		});
		//
		// 		cursor.Emit(OpCodes.Brfalse, label);
		// 		cursor.Emit(OpCodes.Ldc_I4, 1);
		// 		cursor.Emit(OpCodes.Ret);
		// 		cursor.MarkLabel(label);
		// 	}
		// }
		//
		// private static bool Player_TryPurchasing(int price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, Player player)
		// {
		// 	long priceRemaining = price;
		//
		// 	foreach (Wallet wallet in player.inventory.OfType<Wallet>())
		// 	{
		// 		long walletCoins = wallet.Coins;
		// 		long sub = Math.Min(walletCoins, priceRemaining);
		// 		priceRemaining -= sub;
		// 		wallet.Coins -= sub;
		//
		// 		if (priceRemaining <= 0) return false;
		// 	}
		//
		// 	Dictionary<Point, Item> dictionary = new Dictionary<Point, Item>();
		//
		// 	bool result = false;
		// 	while (priceRemaining > 0L)
		// 	{
		// 		long coinValue = 1000000L;
		// 		for (int coinIndex = 0; coinIndex < 4; coinIndex++)
		// 		{
		// 			if (priceRemaining >= coinValue)
		// 			{
		// 				foreach (Point current in slotCoins)
		// 				{
		// 					if (inv[current.X][current.Y].type == 74 - coinIndex)
		// 					{
		// 						long stack = priceRemaining / coinValue;
		// 						dictionary[current] = inv[current.X][current.Y].Clone();
		// 						if (stack < inv[current.X][current.Y].stack) inv[current.X][current.Y].stack -= (int)stack;
		// 						else
		// 						{
		// 							inv[current.X][current.Y].SetDefaults();
		// 							slotsEmpty.Add(current);
		// 						}
		//
		// 						priceRemaining -= coinValue * (dictionary[current].stack - inv[current.X][current.Y].stack);
		// 					}
		// 				}
		// 			}
		//
		// 			coinValue /= 100L;
		// 		}
		//
		// 		if (priceRemaining > 0L)
		// 		{
		// 			if (slotsEmpty.Count <= 0)
		// 			{
		// 				foreach (KeyValuePair<Point, Item> current2 in dictionary) inv[current2.Key.X][current2.Key.Y] = current2.Value.Clone();
		// 				result = true;
		// 				break;
		// 			}
		//
		// 			slotsEmpty.Sort(DelegateMethods.CompareYReverse);
		// 			Point item = new Point(-1, -1);
		// 			for (int j = 0; j < inv.Count; j++)
		// 			{
		// 				coinValue = 10000L;
		// 				for (int k = 0; k < 3; k++)
		// 				{
		// 					if (priceRemaining >= coinValue)
		// 					{
		// 						foreach (Point current3 in slotCoins)
		// 						{
		// 							if (current3.X == j && inv[current3.X][current3.Y].type == 74 - k && inv[current3.X][current3.Y].stack >= 1)
		// 							{
		// 								List<Point> list = slotsEmpty;
		// 								if (j == 1 && slotEmptyBank.Count > 0) list = slotEmptyBank;
		// 								if (j == 2 && slotEmptyBank2.Count > 0) list = slotEmptyBank2;
		// 								if (--inv[current3.X][current3.Y].stack <= 0)
		// 								{
		// 									inv[current3.X][current3.Y].SetDefaults();
		// 									list.Add(current3);
		// 								}
		//
		// 								dictionary[list[0]] = inv[list[0].X][list[0].Y].Clone();
		// 								inv[list[0].X][list[0].Y].SetDefaults(73 - k);
		// 								inv[list[0].X][list[0].Y].stack = 100;
		// 								item = list[0];
		// 								list.RemoveAt(0);
		// 								break;
		// 							}
		// 						}
		// 					}
		//
		// 					if (item.X != -1 || item.Y != -1) break;
		// 					coinValue /= 100L;
		// 				}
		//
		// 				for (int l = 0; l < 2; l++)
		// 				{
		// 					if (item.X == -1 && item.Y == -1)
		// 					{
		// 						foreach (Point current4 in slotCoins)
		// 						{
		// 							if (current4.X == j && inv[current4.X][current4.Y].type == 73 + l && inv[current4.X][current4.Y].stack >= 1)
		// 							{
		// 								List<Point> list2 = slotsEmpty;
		// 								if (j == 1 && slotEmptyBank.Count > 0) list2 = slotEmptyBank;
		// 								if (j == 2 && slotEmptyBank2.Count > 0) list2 = slotEmptyBank2;
		// 								if (j == 3 && slotEmptyBank3.Count > 0) list2 = slotEmptyBank3;
		// 								if (--inv[current4.X][current4.Y].stack <= 0)
		// 								{
		// 									inv[current4.X][current4.Y].SetDefaults();
		// 									list2.Add(current4);
		// 								}
		//
		// 								dictionary[list2[0]] = inv[list2[0].X][list2[0].Y].Clone();
		// 								inv[list2[0].X][list2[0].Y].SetDefaults(72 + l);
		// 								inv[list2[0].X][list2[0].Y].stack = 100;
		// 								item = list2[0];
		// 								list2.RemoveAt(0);
		// 								break;
		// 							}
		// 						}
		// 					}
		// 				}
		//
		// 				if (item.X != -1 && item.Y != -1)
		// 				{
		// 					slotCoins.Add(item);
		// 					break;
		// 				}
		// 			}
		//
		// 			slotsEmpty.Sort(DelegateMethods.CompareYReverse);
		// 			slotEmptyBank.Sort(DelegateMethods.CompareYReverse);
		// 			slotEmptyBank2.Sort(DelegateMethods.CompareYReverse);
		// 			slotEmptyBank3.Sort(DelegateMethods.CompareYReverse);
		// 		}
		// 	}
		//
		// 	return result;
		// }

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
						if (pItem.modItem is Wallet wallet) coins += wallet.Handler.CountCoins();
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
	}
}