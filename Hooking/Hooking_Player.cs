using BaseLibrary;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items.Ammo;
using PortableStorage.Items.Special;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static bool Player_BuyItem(On.Terraria.Player.orig_BuyItem orig, Player self, int price, int customCurrency)
		{
			if (customCurrency != -1) return CustomCurrencyManager.BuyItem(self, price, customCurrency);

			long inventoryCount = Utils.CoinsCount(out bool _, self.inventory, 58, 57, 56, 55, 54);
			long piggyCount = Utils.CoinsCount(out bool _, self.bank.item);
			long safeCount = Utils.CoinsCount(out bool _, self.bank2.item);
			long defendersCount = Utils.CoinsCount(out bool _, self.bank3.item);

			long walletCount = self.inventory.OfType<Wallet>().Sum(wallet => wallet.Handler.Items.CountCoins());
			long combined = Utils.CoinsCombineStacks(out bool _, inventoryCount, piggyCount, safeCount, defendersCount, walletCount);

			if (combined < price) return false;

			List<Item[]> list = new List<Item[]>();
			Dictionary<int, List<int>> ignoredSlots = new Dictionary<int, List<int>>();
			List<Point> coins = new List<Point>();
			List<Point> emptyInventory = new List<Point>();
			List<Point> emptyPiggy = new List<Point>();
			List<Point> emptySafe = new List<Point>();
			List<Point> emptyDefenders = new List<Point>();

			list.Add(self.inventory);
			list.Add(self.bank.item);
			list.Add(self.bank2.item);
			list.Add(self.bank3.item);

			list.AddRange(self.inventory.OfType<Wallet>().Select(x => x.Handler.Items.ToArray()));
			for (int i = 0; i < list.Count; i++) ignoredSlots[i] = new List<int>();

			ignoredSlots[0] = new List<int>
			{
				58,
				57,
				56,
				55,
				54
			};
			for (int j = 0; j < list.Count; j++)
			{
				for (int k = 0; k < list[j].Length; k++)
				{
					if (!ignoredSlots[j].Contains(k) && list[j][k].IsCoin())
					{
						coins.Add(new Point(j, k));
					}
				}
			}

			int num6 = 0;
			for (int l = list[num6].Length - 1; l >= 0; l--)
			{
				if (!ignoredSlots[num6].Contains(l) && (list[num6][l].type == 0 || list[num6][l].stack == 0)) emptyInventory.Add(new Point(num6, l));
			}

			num6 = 1;
			for (int m = list[num6].Length - 1; m >= 0; m--)
			{
				if (!ignoredSlots[num6].Contains(m) && (list[num6][m].type == 0 || list[num6][m].stack == 0)) emptyPiggy.Add(new Point(num6, m));
			}

			num6 = 2;
			for (int n = list[num6].Length - 1; n >= 0; n--)
			{
				if (!ignoredSlots[num6].Contains(n) && (list[num6][n].type == 0 || list[num6][n].stack == 0)) emptySafe.Add(new Point(num6, n));
			}

			num6 = 3;
			for (int num7 = list[num6].Length - 1; num7 >= 0; num7--)
			{
				if (!ignoredSlots[num6].Contains(num7) && (list[num6][num7].type == 0 || list[num6][num7].stack == 0)) emptyDefenders.Add(new Point(num6, num7));
			}

			// todo
			List<Point> emptyWallet = new List<Point>();
			num6 = 4;
			for (int i = num6; i < list.Count - 4; i++)
			{
				for (int n = list[i].Length - 1; n >= 0; n--)
				{
					if (!ignoredSlots[i].Contains(n) && (list[i][n].type == 0 || list[i][n].stack == 0)) emptyWallet.Add(new Point(i, n));
				}
			}

			return !Player_TryPurchasing(price, list, coins, emptyInventory, emptyPiggy, emptySafe, emptyDefenders, emptyWallet);
		}

		private static void Player_BuyItem(ILContext il)
		{
			int emptyWalletIndex = il.AddVariable(typeof(List<Point>));

			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchCall(typeof(Utils).GetMethod("CoinsCombineStacks", Utility.defaultFlags))))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldarg_0);

				cursor.EmitDelegate<Func<bool, long[], Player, long>>((overflowing, coinsCount, player) =>
				{
					long walletCount = player.inventory.OfType<Wallet>().Sum(wallet => wallet.Handler.Items.CountCoins());
					Array.Resize(ref coinsCount, 5);
					coinsCount[4] = walletCount;
					return Utils.CoinsCombineStacks(out overflowing, coinsCount);
				});
			}

			if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(13), i => i.MatchBr(out _)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldloc, 5);

				cursor.EmitDelegate<Func<Player, List<Item[]>, List<Item[]>>>((player, list) =>
				{
					list.AddRange(player.inventory.OfType<Wallet>().Select(x => x.Handler.Items.ToArray()));
					return list;
				});

				cursor.Emit(OpCodes.Stloc, 5);
			}
		}

		private static void Player_CanBuyItem(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchCall(typeof(Utils).GetMethod("CoinsCombineStacks", Utility.defaultFlags))))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldarg_0);

				cursor.EmitDelegate<Func<bool, long[], Player, long>>((overflowing, coinsCount, player) =>
				{
					long walletCount = player.inventory.OfType<Wallet>().Sum(wallet => wallet.Handler.Items.CountCoins());
					Array.Resize(ref coinsCount, 5);
					coinsCount[4] = walletCount;
					return Utils.CoinsCombineStacks(out overflowing, coinsCount);
				});
			}
		}

		private static void Player_HasAmmo(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdloc(0), i => i.MatchLdcI4(58)))
			{
				cursor.Index += 3;

				cursor.Emit(OpCodes.Ldarg_0);
				cursor.Emit(OpCodes.Ldarg_1);

				cursor.EmitDelegate<Func<Player, Item, bool>>((player, ammoUser) => player.inventory.OfType<BaseAmmoBag>().Any(ammoBag => ammoBag.Handler.Items.Any(item => item.ammo == ammoUser.useAmmo && item.stack > 0)));

				cursor.Emit(OpCodes.Starg, il.Method.Parameters.FirstOrDefault(x => x.Name == "canUse"));
			}
		}

		private static void Player_PickAmmo(ILContext il)
		{
			int firstAmmoIndex = il.AddVariable(typeof(Item));
			int canShootIndex = il.GetArgumentIndex("canShoot");

			ILCursor cursor = new ILCursor(il);
			ILLabel elseLabel = cursor.DefineLabel();
			ILLabel endLabel = cursor.DefineLabel();

			cursor.Emit(OpCodes.Newobj, typeof(Item).GetConstructors()[0]);
			cursor.Emit(OpCodes.Stloc, firstAmmoIndex);

			if (cursor.TryGotoNext(i => i.MatchNewobj(typeof(Item).GetConstructors()[0]), i => i.MatchStloc(0)))
			{
				cursor.Index += 2;

				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldarg, 1);
				cursor.EmitDelegate<Func<Player, Item, Item>>((player, sItem) => player.inventory.OfType<BaseAmmoBag>().SelectMany(x => x.Handler.Items).FirstOrDefault(ammo => ammo.ammo == sItem.useAmmo && ammo.stack > 0));
				cursor.Emit(OpCodes.Stloc, firstAmmoIndex);

				cursor.Emit(OpCodes.Ldloc, firstAmmoIndex);
				cursor.Emit(OpCodes.Brfalse, elseLabel);

				cursor.Emit(OpCodes.Ldloc, firstAmmoIndex);
				cursor.Emit(OpCodes.Stloc, 0);

				cursor.Emit(OpCodes.Ldarg, canShootIndex);
				cursor.Emit(OpCodes.Ldc_I4, 1);
				cursor.Emit(OpCodes.Stind_I1);

				cursor.Emit(OpCodes.Br, endLabel);
			}

			if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(1), i => i.MatchLdcI4(54))) cursor.MarkLabel(elseLabel);

			if (cursor.TryGotoNext(i => i.MatchLdarg(3), i => i.MatchLdindU1(), i => i.MatchBrfalse(out _))) cursor.MarkLabel(endLabel);
		}

		private static void Player_QuickBuff(ILContext il)
		{
			il.Body.Variables.Add(new VariableDefinition(il.Import(typeof(ValueTuple<LegacySoundStyle, bool>))));

			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdnull(), i => i.MatchStloc(0)))
			{
				cursor.Index += 2;
				ILLabel label = cursor.DefineLabel();

				cursor.Emit(OpCodes.Ldarg_0);

				cursor.EmitDelegate<Func<Player, ValueTuple<LegacySoundStyle, bool>>>(player =>
				{
					LegacySoundStyle legacySoundStyle = null;

					foreach (Item item in player.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items))
					{
						if (player.CountBuffs() == 22) return (null, true);

						if (item.stack > 0 && item.type > 0 && item.buffType > 0 && !item.summon && item.buffType != 90)
						{
							int buffType = item.buffType;
							bool useItem = ItemLoader.CanUseItem(item, player);
							for (int buffIndex = 0; buffIndex < 22; buffIndex++)
							{
								if (buffType == 27 && (player.buffType[buffIndex] == buffType || player.buffType[buffIndex] == 101 || player.buffType[buffIndex] == 102))
								{
									useItem = false;
									break;
								}

								if (player.buffType[buffIndex] == buffType)
								{
									useItem = false;
									break;
								}

								if (Main.meleeBuff[buffType] && Main.meleeBuff[player.buffType[buffIndex]])
								{
									useItem = false;
									break;
								}
							}

							if (Main.lightPet[item.buffType] || Main.vanityPet[item.buffType])
							{
								for (int buffIndex = 0; buffIndex < 22; buffIndex++)
								{
									if (Main.lightPet[player.buffType[buffIndex]] && Main.lightPet[item.buffType]) useItem = false;
									if (Main.vanityPet[player.buffType[buffIndex]] && Main.vanityPet[item.buffType]) useItem = false;
								}
							}

							if (item.mana > 0 && useItem)
							{
								if (player.statMana >= (int)(item.mana * player.manaCost))
								{
									player.manaRegenDelay = (int)player.maxRegenDelay;
									player.statMana -= (int)(item.mana * player.manaCost);
								}
								else useItem = false;
							}

							if (player.whoAmI == Main.myPlayer && item.type == 603 && !Main.cEd) useItem = false;

							if (buffType == 27)
							{
								buffType = Main.rand.Next(3);
								if (buffType == 0) buffType = 27;
								if (buffType == 1) buffType = 101;
								if (buffType == 2) buffType = 102;
							}

							if (useItem)
							{
								ItemLoader.UseItem(item, player);
								legacySoundStyle = item.UseSound;
								int buffTime = item.buffTime;
								if (buffTime == 0) buffTime = 3600;

								player.AddBuff(buffType, buffTime);
								if (item.consumable)
								{
									if (ItemLoader.ConsumeItem(item, player)) item.stack--;
									if (item.stack <= 0) item.TurnToAir();
								}
							}
						}
					}

					return (legacySoundStyle, false);
				});

				Type type = typeof(ValueTuple<LegacySoundStyle, bool>);

				cursor.Emit(OpCodes.Stloc, 7);

				cursor.Emit(OpCodes.Ldloc, 7);
				cursor.Emit(OpCodes.Ldfld, type.GetField("Item1", Utility.defaultFlags));
				cursor.Emit(OpCodes.Stloc, 0);

				cursor.Emit(OpCodes.Ldloc, 7);
				cursor.Emit(OpCodes.Ldfld, type.GetField("Item2", Utility.defaultFlags));
				cursor.Emit(OpCodes.Brfalse, label);
				cursor.Emit(OpCodes.Ret);

				cursor.MarkLabel(label);
			}
		}

		private static void Player_QuickHeal_GetItemToUse(ILContext il)
		{
			Type type = typeof(ValueTuple<Item, int>);
			int tupleIndex = il.AddVariable(type);

			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(3), i => i.MatchBr(out _)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldloc, 0);
				cursor.Emit(OpCodes.Ldloc, 2);

				cursor.EmitDelegate<Func<Player, int, int, ValueTuple<Item, int>>>((player, lostHealth, healthGain) =>
				{
					Item result = null;

					foreach (Item item in player.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items))
					{
						if (item.stack > 0 && item.type > 0 && item.potion && item.healLife > 0 && ItemLoader.CanUseItem(item, player))
						{
							int healWaste = player.GetHealLife(item, true) - lostHealth;
							if (healthGain < 0)
							{
								if (healWaste > healthGain)
								{
									result = item;
									healthGain = healWaste;
								}
							}
							else if (healWaste < healthGain && healWaste >= 0)
							{
								result = item;
								healthGain = healWaste;
							}
						}
					}

					return (result, healthGain);
				});

				cursor.Emit(OpCodes.Stloc, tupleIndex);

				cursor.Emit(OpCodes.Ldloc, tupleIndex);
				cursor.Emit(OpCodes.Ldfld, type.GetField("Item1", Utility.defaultFlags));
				cursor.Emit(OpCodes.Stloc, 1);

				cursor.Emit(OpCodes.Ldloc, tupleIndex);
				cursor.Emit(OpCodes.Ldfld, type.GetField("Item2", Utility.defaultFlags));
				cursor.Emit(OpCodes.Stloc, 3);
			}
		}

		private static void Player_QuickMana(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel jumpToFor = cursor.DefineLabel();
			ILLabel jumpToMyCode = cursor.DefineLabel();

			if (cursor.TryGotoNext(i => i.MatchBneUn(out _), i => i.MatchRet()))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Bne_Un, jumpToMyCode);

				cursor.Index++;

				cursor.MarkLabel(jumpToMyCode);

				cursor.Emit(OpCodes.Ldarg, 0);

				cursor.EmitDelegate<Func<Player, bool>>(player =>
				{
					foreach (Item item in player.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items))
					{
						if (item.stack > 0 && item.type > 0 && item.healMana > 0 && (player.potionDelay == 0 || !item.potion) && ItemLoader.CanUseItem(item, player))
						{
							Main.PlaySound(item.UseSound, player.position);
							if (item.potion)
							{
								if (item.type == ItemID.RestorationPotion)
								{
									player.potionDelay = player.restorationDelayTime;
									player.AddBuff(BuffID.PotionSickness, player.potionDelay);
								}
								else
								{
									player.potionDelay = player.potionDelayTime;
									player.AddBuff(BuffID.PotionSickness, player.potionDelay);
								}
							}

							ItemLoader.UseItem(item, player);
							int healLife = player.GetHealLife(item, true);
							int healMana = player.GetHealMana(item, true);
							player.statLife += healLife;
							player.statMana += healMana;
							if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;
							if (player.statMana > player.statManaMax2) player.statMana = player.statManaMax2;
							if (healLife > 0 && Main.myPlayer == player.whoAmI) player.HealEffect(healLife);
							if (healMana > 0)
							{
								player.AddBuff(BuffID.ManaSickness, Player.manaSickTime);
								if (Main.myPlayer == player.whoAmI) player.ManaEffect(healMana);
							}

							if (ItemLoader.ConsumeItem(item, player)) item.stack--;
							if (item.stack <= 0) item.TurnToAir();

							Recipe.FindRecipes();
							return true;
						}
					}

					return false;
				});

				cursor.Emit(OpCodes.Brfalse, jumpToFor);
				cursor.Emit(OpCodes.Ret);

				if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(0), i => i.MatchBr(out _))) cursor.MarkLabel(jumpToFor);
			}
		}

		private static void Player_SellItem(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(2), i => i.MatchBr(out _)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldloc, 1);

				cursor.EmitDelegate<Func<Player, int, bool>>((player, price) =>
				{
					Wallet wallet = player.inventory.OfType<Wallet>().FirstOrDefault();

					if (wallet != null)
					{
						long addedCoins = price + wallet.Handler.Items.CountCoins();

						wallet.Handler.Items = Utils.CoinsSplit(addedCoins).Select((stack, index) =>
						{
							Item coin = new Item();
							coin.SetDefaults(ItemID.CopperCoin + index);
							coin.stack = stack;
							return coin;
						}).Reverse().ToList();

						for (int i = 0; i < 4; i++) wallet.Handler.OnContentsChanged.Invoke(i);

						return true;
					}

					return false;
				});

				cursor.Emit(OpCodes.Brfalse, label);
				cursor.Emit(OpCodes.Ldc_I4, 1);
				cursor.Emit(OpCodes.Ret);
				cursor.MarkLabel(label);
			}
		}

		private static bool Player_TryPurchasing(int price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, List<Point> slotEmptyWallet)
		{
			long priceRemaining = price;
			Dictionary<Point, Item> dictionary = new Dictionary<Point, Item>();

			bool result = false;
			while (priceRemaining > 0L)
			{
				long coinValue = 1000000L;
				for (int coinIndex = 0; coinIndex < 4; coinIndex++)
				{
					if (priceRemaining >= coinValue)
					{
						foreach (Point current in slotCoins)
						{
							if (inv[current.X][current.Y].type == 74 - coinIndex)
							{
								long stack = priceRemaining / coinValue;
								dictionary[current] = inv[current.X][current.Y].Clone();
								if (stack < inv[current.X][current.Y].stack) inv[current.X][current.Y].stack -= (int)stack;
								else
								{
									inv[current.X][current.Y].SetDefaults();
									slotsEmpty.Add(current);
								}

								priceRemaining -= coinValue * (dictionary[current].stack - inv[current.X][current.Y].stack);
							}
						}
					}

					coinValue /= 100L;
				}

				// todo: this seems to handle change, try adding all to wallet
				if (priceRemaining > 0L)
				{
					if (slotsEmpty.Count <= 0)
					{
						foreach (KeyValuePair<Point, Item> current2 in dictionary) inv[current2.Key.X][current2.Key.Y] = current2.Value.Clone();
						result = true;
						break;
					}

					slotsEmpty.Sort(DelegateMethods.CompareYReverse);
					Point item = new Point(-1, -1);
					for (int j = 0; j < inv.Count; j++)
					{
						coinValue = 10000L;
						for (int k = 0; k < 3; k++)
						{
							if (priceRemaining >= coinValue)
							{
								foreach (Point current3 in slotCoins)
								{
									if (current3.X == j && inv[current3.X][current3.Y].type == 74 - k && inv[current3.X][current3.Y].stack >= 1)
									{
										List<Point> list = slotsEmpty;
										if (j == 1 && slotEmptyBank.Count > 0) list = slotEmptyBank;
										if (j == 2 && slotEmptyBank2.Count > 0) list = slotEmptyBank2;
										if (j > 3 && slotEmptyWallet.Count > 0) list = slotEmptyWallet;
										if (--inv[current3.X][current3.Y].stack <= 0)
										{
											inv[current3.X][current3.Y].SetDefaults();
											list.Add(current3);
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

							if (item.X != -1 || item.Y != -1) break;
							coinValue /= 100L;
						}

						for (int l = 0; l < 2; l++)
						{
							if (item.X == -1 && item.Y == -1)
							{
								foreach (Point current4 in slotCoins)
								{
									if (current4.X == j && inv[current4.X][current4.Y].type == 73 + l && inv[current4.X][current4.Y].stack >= 1)
									{
										List<Point> list2 = slotsEmpty;
										if (j == 1 && slotEmptyBank.Count > 0) list2 = slotEmptyBank;
										if (j == 2 && slotEmptyBank2.Count > 0) list2 = slotEmptyBank2;
										if (j == 3 && slotEmptyBank3.Count > 0) list2 = slotEmptyBank3;
										if (j > 3 && slotEmptyWallet.Count > 0) list2 = slotEmptyWallet;
										if (--inv[current4.X][current4.Y].stack <= 0)
										{
											inv[current4.X][current4.Y].SetDefaults();
											list2.Add(current4);
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
					slotEmptyWallet.Sort(DelegateMethods.CompareYReverse);
				}
			}

			return result;
		}

		private static void Player_TryPurchasing(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
		}
	}
}