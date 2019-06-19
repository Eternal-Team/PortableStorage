using BaseLibrary;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using PortableStorage.Items.Ammo;
using PortableStorage.Items.Special;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		#region IL
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

		private static void Player_QuickBuff(ILContext il)
		{
			il.Body.Variables.Add(new VariableDefinition(il.Import(typeof(ValueTuple<LegacySoundStyle, bool>))));

			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(
				i => i.MatchLdnull(),
				i => i.MatchStloc(0)))
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

		private static void Player_HasAmmo(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(
				i => i.MatchLdloc(0),
				i => i.MatchLdcI4(58)))
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
		#endregion
		
		private static bool Player_BuyItem(On.Terraria.Player.orig_BuyItem orig, Player self, int price, int customCurrency)
		{
			if (customCurrency != -1) return CustomCurrencyManager.BuyItem(self, price, customCurrency);

			long inventoryCount = Utils.CoinsCount(out bool _, self.inventory, 58, 57, 56, 55, 54);
			long piggyCount = Utils.CoinsCount(out bool _, self.bank.item);
			long safeCount = Utils.CoinsCount(out bool _, self.bank2.item);
			long defendersCount = Utils.CoinsCount(out bool _, self.bank3.item);

			// here
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

			// here
			List<Point> emptyWallet = new List<Point>();
			list.Add(self.inventory);
			list.Add(self.bank.item);
			list.Add(self.bank2.item);
			list.Add(self.bank3.item);

			// here
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

			// here
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
		
		private static Item Player_QuickHeal_GetItemToUse(On.Terraria.Player.orig_QuickHeal_GetItemToUse orig, Player self)
		{
			int lostHealth = self.statLifeMax2 - self.statLife;
			Item result = null;
			int healtGain = -self.statLifeMax2;

			foreach (Item item in self.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items).Concat(self.inventory))
			{
				if (item.stack > 0 && item.type > 0 && item.potion && item.healLife > 0 && ItemLoader.CanUseItem(item, self))
				{
					int healWaste = self.GetHealLife(item, true) - lostHealth;
					if (healtGain < 0)
					{
						if (healWaste > healtGain)
						{
							result = item;
							healtGain = healWaste;
						}
					}
					else if (healWaste < healtGain && healWaste >= 0)
					{
						result = item;
						healtGain = healWaste;
					}
				}
			}

			return result;
		}

		private static void Player_QuickMana(On.Terraria.Player.orig_QuickMana orig, Player self)
		{
			if (self.noItems || self.statMana == self.statManaMax2) return;

			foreach (Item item in self.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items).Concat(self.inventory))
			{
				if (item.stack > 0 && item.type > 0 && item.healMana > 0 && (self.potionDelay == 0 || !item.potion) && ItemLoader.CanUseItem(item, self))
				{
					Main.PlaySound(item.UseSound, self.position);
					if (item.potion)
					{
						if (item.type == ItemID.RestorationPotion)
						{
							self.potionDelay = self.restorationDelayTime;
							self.AddBuff(BuffID.PotionSickness, self.potionDelay);
						}
						else
						{
							self.potionDelay = self.potionDelayTime;
							self.AddBuff(BuffID.PotionSickness, self.potionDelay);
						}
					}

					ItemLoader.UseItem(item, self);
					int healLife = self.GetHealLife(item, true);
					int healMana = self.GetHealMana(item, true);
					self.statLife += healLife;
					self.statMana += healMana;
					if (self.statLife > self.statLifeMax2) self.statLife = self.statLifeMax2;
					if (self.statMana > self.statManaMax2) self.statMana = self.statManaMax2;
					if (healLife > 0 && Main.myPlayer == self.whoAmI) self.HealEffect(healLife);
					if (healMana > 0)
					{
						self.AddBuff(BuffID.ManaSickness, Player.manaSickTime);
						if (Main.myPlayer == self.whoAmI) self.ManaEffect(healMana);
					}

					if (ItemLoader.ConsumeItem(item, self)) item.stack--;
					if (item.stack <= 0) item.TurnToAir();

					Recipe.FindRecipes();
					return;
				}
			}
		}

		private static bool Player_SellItem(On.Terraria.Player.orig_SellItem orig, Player self, int price, int stack)
		{
			if (price <= 0) return false;

			int actualPrice = price / 5;
			if (actualPrice < 1) actualPrice = 1;
			actualPrice *= stack;

			Wallet wallet = self.inventory.OfType<Wallet>().FirstOrDefault();

			if (wallet != null)
			{
				long addedCoins = actualPrice + wallet.Handler.Items.CountCoins();

				wallet.Handler.Items = Utils.CoinsSplit(addedCoins).Select((s, index) =>
				{
					Item coin = new Item();
					coin.SetDefaults(ItemID.CopperCoin + index);
					coin.stack = s;
					return coin;
				}).Reverse().ToList();

				for (int i = 0; i < 4; i++) wallet.Handler.OnContentsChanged.Invoke(i);

				return true;
			}

			Item[] array = new Item[58];
			for (int i = 0; i < 58; i++)
			{
				array[i] = new Item();
				array[i] = self.inventory[i].Clone();
			}

			bool flag = false;
			while (actualPrice >= 1000000)
			{
				if (flag) break;
				int num = -1;
				for (int k = 53; k >= 0; k--)
				{
					if (num == -1 && (self.inventory[k].type == 0 || self.inventory[k].stack == 0))
					{
						num = k;
					}

					while (self.inventory[k].type == 74 && self.inventory[k].stack < self.inventory[k].maxStack && actualPrice >= 1000000)
					{
						self.inventory[k].stack++;
						actualPrice -= 1000000;
						self.DoCoins(k);
						if (self.inventory[k].stack == 0 && num == -1)
						{
							num = k;
						}
					}
				}

				if (actualPrice >= 1000000)
				{
					if (num == -1)
					{
						flag = true;
					}
					else
					{
						self.inventory[num].SetDefaults(74);
						actualPrice -= 1000000;
					}
				}
			}

			while (actualPrice >= 10000)
			{
				if (flag)
				{
					break;
				}

				int num2 = -1;
				for (int l = 53; l >= 0; l--)
				{
					if (num2 == -1 && (self.inventory[l].type == 0 || self.inventory[l].stack == 0))
					{
						num2 = l;
					}

					while (self.inventory[l].type == 73 && self.inventory[l].stack < self.inventory[l].maxStack && actualPrice >= 10000)
					{
						self.inventory[l].stack++;
						actualPrice -= 10000;
						self.DoCoins(l);
						if (self.inventory[l].stack == 0 && num2 == -1)
						{
							num2 = l;
						}
					}
				}

				if (actualPrice >= 10000)
				{
					if (num2 == -1)
					{
						flag = true;
					}
					else
					{
						self.inventory[num2].SetDefaults(73);
						actualPrice -= 10000;
					}
				}
			}

			while (actualPrice >= 100)
			{
				if (flag)
				{
					break;
				}

				int num3 = -1;
				for (int m = 53; m >= 0; m--)
				{
					if (num3 == -1 && (self.inventory[m].type == 0 || self.inventory[m].stack == 0))
					{
						num3 = m;
					}

					while (self.inventory[m].type == 72 && self.inventory[m].stack < self.inventory[m].maxStack && actualPrice >= 100)
					{
						self.inventory[m].stack++;
						actualPrice -= 100;
						self.DoCoins(m);
						if (self.inventory[m].stack == 0 && num3 == -1)
						{
							num3 = m;
						}
					}
				}

				if (actualPrice >= 100)
				{
					if (num3 == -1)
					{
						flag = true;
					}
					else
					{
						self.inventory[num3].SetDefaults(72);
						actualPrice -= 100;
					}
				}
			}

			while (actualPrice >= 1 && !flag)
			{
				int num4 = -1;
				for (int n = 53; n >= 0; n--)
				{
					if (num4 == -1 && (self.inventory[n].type == 0 || self.inventory[n].stack == 0))
					{
						num4 = n;
					}

					while (self.inventory[n].type == 71 && self.inventory[n].stack < self.inventory[n].maxStack && actualPrice >= 1)
					{
						self.inventory[n].stack++;
						actualPrice--;
						self.DoCoins(n);
						if (self.inventory[n].stack == 0 && num4 == -1)
						{
							num4 = n;
						}
					}
				}

				if (actualPrice >= 1)
				{
					if (num4 == -1)
					{
						flag = true;
					}
					else
					{
						self.inventory[num4].SetDefaults(71);
						actualPrice--;
					}
				}
			}

			if (flag)
			{
				for (int num5 = 0; num5 < 58; num5++)
				{
					self.inventory[num5] = array[num5].Clone();
				}

				return false;
			}

			return true;
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

		
	}
}