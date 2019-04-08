using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items;
using PortableStorage.Items.Ammo;
using PortableStorage.Items.Special;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static bool Player_CanBuyItem(Func<Player, int, int, bool> orig, Player self, int price, int customCurrency)
		{
			if (customCurrency != -1) return CustomCurrencyManager.BuyItem(self, price, customCurrency);

			long inventoryCount = Utils.CoinsCount(out bool _, self.inventory, 58, 57, 56, 55, 54);
			long piggyCount = Utils.CoinsCount(out bool _, self.bank.item);
			long safeCount = Utils.CoinsCount(out bool _, self.bank2.item);
			long defendersCount = Utils.CoinsCount(out bool _, self.bank3.item);
			long walletCount = self.inventory.OfType<Wallet>().Sum(wallet => wallet.Handler.stacks.CountCoins());
			long combined = Utils.CoinsCombineStacks(out bool _, inventoryCount, piggyCount, safeCount, defendersCount, walletCount);

			return combined >= price;
		}

		private static void Player_DropSelectedItem(On.Terraria.Player.orig_DropSelectedItem orig, Player self)
		{
			if (self.inventory[self.selectedItem].modItem is BaseBag bag && bag.UI != null) PortableStorage.Instance.PanelUI.UI.CloseUI(bag);

			orig(self);
		}

		private static bool Player_BuyItem(On.Terraria.Player.orig_BuyItem orig, Player self, int price, int customCurrency)
		{
			if (customCurrency != -1) return CustomCurrencyManager.BuyItem(self, price, customCurrency);

			long inventoryCount = Utils.CoinsCount(out bool _, self.inventory, 58, 57, 56, 55, 54);
			long piggyCount = Utils.CoinsCount(out bool _, self.bank.item);
			long safeCount = Utils.CoinsCount(out bool _, self.bank2.item);
			long defendersCount = Utils.CoinsCount(out bool _, self.bank3.item);
			long walletCount = self.inventory.OfType<Wallet>().Sum(wallet => wallet.Handler.stacks.CountCoins());

			long combined = Utils.CoinsCombineStacks(out bool _, inventoryCount, piggyCount, safeCount, defendersCount, walletCount);

			if (combined < price) return false;

			List<Item[]> list = new List<Item[]>();
			Dictionary<int, List<int>> ignoredSlots = new Dictionary<int, List<int>>();
			List<Point> coins = new List<Point>();
			List<Point> emptyInventory = new List<Point>();
			List<Point> emptyPiggy = new List<Point>();
			List<Point> emptySafe = new List<Point>();
			List<Point> emptyDefenders = new List<Point>();
			List<Point> emptyWallet = new List<Point>();
			list.Add(self.inventory);
			list.Add(self.bank.item);
			list.Add(self.bank2.item);
			list.Add(self.bank3.item);
			list.AddRange(self.inventory.OfType<Wallet>().Select(x => x.Handler.stacks.ToArray()));
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

		private static bool Player_HasAmmo(On.Terraria.Player.orig_HasAmmo orig, Player self, Item ammoUser, bool canUse)
		{
			if (ammoUser.useAmmo > 0) canUse = self.inventory.Any(item => item.ammo == ammoUser.useAmmo && item.stack > 0) || self.inventory.OfType<BaseAmmoBag>().Any(ammoBag => ammoBag.Handler.stacks.Any(item => item.ammo == ammoUser.useAmmo && item.stack > 0));
			return canUse;
		}

		private static void Player_PickAmmo(On.Terraria.Player.orig_PickAmmo orig, Player self, Item sItem, ref int shoot, ref float speed, ref bool canShoot, ref int Damage, ref float KnockBack, bool dontConsume)
		{
			Item item = new Item();

			Item firstAmmo = self.inventory.OfType<BaseAmmoBag>().SelectMany(x => x.Handler.stacks).FirstOrDefault(ammo => ammo.ammo == sItem.useAmmo && ammo.stack > 0);
			if (firstAmmo != null)
			{
				item = firstAmmo;
				canShoot = true;
			}
			else
			{
				bool hasInAmmoSlots = false;

				for (int i = 54; i < 58; i++)
				{
					if (self.inventory[i].ammo == sItem.useAmmo && self.inventory[i].stack > 0)
					{
						item = self.inventory[i];
						canShoot = true;
						hasInAmmoSlots = true;
						break;
					}
				}

				if (!hasInAmmoSlots)
				{
					for (int j = 0; j < 54; j++)
					{
						if (self.inventory[j].ammo == sItem.useAmmo && self.inventory[j].stack > 0)
						{
							item = self.inventory[j];
							canShoot = true;
							break;
						}
					}
				}
			}

			if (canShoot)
			{
				if (sItem.type == 1946)
				{
					shoot = 338 + item.type - 771;
					if (shoot > ProjectileID.RocketSnowmanIV) shoot = ProjectileID.RocketSnowmanIV;
				}
				else if (sItem.useAmmo == AmmoID.Rocket) shoot += item.shoot;
				else if (sItem.useAmmo == 780) shoot += item.shoot;
				else if (item.shoot > 0) shoot = item.shoot;

				if (sItem.type == 3019 && shoot == 1) shoot = 485;
				if (sItem.type == 3052) shoot = 495;
				if (sItem.type == 3245 && shoot == 21) shoot = 532;
				if (shoot == 42)
				{
					if (item.type == 370)
					{
						shoot = 65;
						Damage += 5;
					}
					else if (item.type == 408)
					{
						shoot = 68;
						Damage += 5;
					}
					else if (item.type == 1246)
					{
						shoot = 354;
						Damage += 5;
					}
				}

				if (self.HeldItem.type == 2888 && shoot == 1) shoot = 469;
				if (self.magicQuiver && (sItem.useAmmo == AmmoID.Arrow || sItem.useAmmo == AmmoID.Stake))
				{
					KnockBack = (int)(KnockBack * 1.1);
					speed *= 1.1f;
				}

				speed += item.shootSpeed;
				if (item.ranged)
				{
					if (item.damage > 0)
					{
						Damage += (int)(item.damage * self.rangedDamage);
					}
				}
				else Damage += item.damage;

				if (sItem.useAmmo == AmmoID.Arrow && self.archery)
				{
					if (speed < 20f)
					{
						speed *= 1.2f;
						if (speed > 20f) speed = 20f;
					}

					Damage = (int)(Damage * 1.2);
				}

				KnockBack += item.knockBack;
				ItemLoader.PickAmmo(item, self, ref shoot, ref speed, ref Damage, ref KnockBack);
				bool dontConsumeAmmo = dontConsume;
				if (sItem.type == 3245)
				{
					if (Main.rand.Next(3) == 0) dontConsumeAmmo = true;
					else if (self.thrownCost33 && Main.rand.Next(100) < 33) dontConsumeAmmo = true;
					else if (self.thrownCost50 && Main.rand.Next(100) < 50) dontConsumeAmmo = true;
				}

				if (sItem.type == 3475 && Main.rand.Next(3) != 0) dontConsumeAmmo = true;
				if (sItem.type == 3540 && Main.rand.Next(3) != 0) dontConsumeAmmo = true;
				if (self.magicQuiver && sItem.useAmmo == AmmoID.Arrow && Main.rand.Next(5) == 0) dontConsumeAmmo = true;
				if (self.ammoBox && Main.rand.Next(5) == 0) dontConsumeAmmo = true;
				if (self.ammoPotion && Main.rand.Next(5) == 0) dontConsumeAmmo = true;
				if (sItem.type == 1782 && Main.rand.Next(3) == 0) dontConsumeAmmo = true;
				if (sItem.type == 98 && Main.rand.Next(3) == 0) dontConsumeAmmo = true;
				if (sItem.type == 2270 && Main.rand.Next(2) == 0) dontConsumeAmmo = true;
				if (sItem.type == 533 && Main.rand.Next(2) == 0) dontConsumeAmmo = true;
				if (sItem.type == 1929 && Main.rand.Next(2) == 0) dontConsumeAmmo = true;
				if (sItem.type == 1553 && Main.rand.Next(2) == 0) dontConsumeAmmo = true;
				if (sItem.type == 434 && self.itemAnimation < (int)(sItem.useAnimation / PlayerHooks.TotalMeleeSpeedMultiplier(self, sItem)) - 2) dontConsumeAmmo = true;
				if (self.ammoCost80 && Main.rand.Next(5) == 0) dontConsumeAmmo = true;
				if (self.ammoCost75 && Main.rand.Next(4) == 0) dontConsumeAmmo = true;
				if (shoot == 85 && self.itemAnimation < self.itemAnimationMax - 6) dontConsumeAmmo = true;
				if ((shoot == 145 || shoot == 146 || shoot == 147 || shoot == 148 || shoot == 149) && self.itemAnimation < self.itemAnimationMax - 5) dontConsumeAmmo = true;
				dontConsumeAmmo |= !PlayerHooks.ConsumeAmmo(self, sItem, item) | !ItemLoader.ConsumeAmmo(sItem, item, self);
				if (!dontConsumeAmmo && item.consumable)
				{
					Main.NewText(item);
					PlayerHooks.OnConsumeAmmo(self, sItem, item);
					ItemLoader.OnConsumeAmmo(sItem, item, self);
					item.stack--;
					if (item.stack <= 0)
					{
						item.active = false;
						item.TurnToAir();
					}
				}
			}
		}

		private static Item Player_QuickHeal_GetItemToUse(On.Terraria.Player.orig_QuickHeal_GetItemToUse orig, Player self)
		{
			int lostHealth = self.statLifeMax2 - self.statLife;
			Item result = null;
			int healtGain = -self.statLifeMax2;

			foreach (Item item in self.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.stacks).Concat(self.inventory))
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

			foreach (Item item in self.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.stacks).Concat(self.inventory))
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

		private static void Player_QuickBuff(On.Terraria.Player.orig_QuickBuff orig, Player self)
		{
			if (self.noItems) return;

			LegacySoundStyle sound = null;

			foreach (Item item in self.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.stacks).Concat(self.inventory))
			{
				if (self.CountBuffs() == 22) return;
				if (item.stack > 0 && item.type > 0 && item.buffType > 0 && !item.summon && item.buffType != BuffID.Rudolph)
				{
					int buffType = item.buffType;
					bool canUseItem = ItemLoader.CanUseItem(item, self);
					for (int j = 0; j < self.buffType.Length; j++)
					{
						if (buffType == BuffID.FairyBlue && (self.buffType[j] == buffType || self.buffType[j] == BuffID.FairyRed || self.buffType[j] == BuffID.FairyGreen))
						{
							canUseItem = false;
							break;
						}

						if (self.buffType[j] == buffType)
						{
							canUseItem = false;
							break;
						}

						if (Main.meleeBuff[buffType] && Main.meleeBuff[self.buffType[j]])
						{
							canUseItem = false;
							break;
						}
					}

					if (Main.lightPet[item.buffType] || Main.vanityPet[item.buffType])
					{
						for (int k = 0; k < self.buffType.Length; k++)
						{
							if (Main.lightPet[self.buffType[k]] && Main.lightPet[item.buffType]) canUseItem = false;
							if (Main.vanityPet[self.buffType[k]] && Main.vanityPet[item.buffType]) canUseItem = false;
						}
					}

					if (item.mana > 0 && canUseItem)
					{
						if (self.statMana >= (int)(item.mana * self.manaCost))
						{
							self.manaRegenDelay = (int)self.maxRegenDelay;
							self.statMana -= (int)(item.mana * self.manaCost);
						}
						else canUseItem = false;
					}

					if (self.whoAmI == Main.myPlayer && item.type == ItemID.Carrot && !Main.cEd) canUseItem = false;

					if (buffType == BuffID.FairyBlue)
					{
						buffType = Main.rand.Next(3);
						if (buffType == 0) buffType = BuffID.FairyBlue;
						if (buffType == 1) buffType = BuffID.FairyRed;
						if (buffType == 2) buffType = BuffID.FairyBlue;
					}

					if (canUseItem)
					{
						ItemLoader.UseItem(item, self);
						sound = item.UseSound;
						int buffTime = item.buffTime;
						if (buffTime == 0) buffTime = 3600;

						self.AddBuff(buffType, buffTime);
						if (item.consumable)
						{
							if (ItemLoader.ConsumeItem(item, self)) item.stack--;
							if (item.stack <= 0) item.TurnToAir();
						}
					}
				}
			}

			if (sound != null)
			{
				Main.PlaySound(sound, self.position);
				Recipe.FindRecipes();
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
				long addedCoins = actualPrice + wallet.Handler.stacks.CountCoins();

				wallet.Handler.stacks = Utils.CoinsSplit(addedCoins).Select((s, index) =>
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
	}
}