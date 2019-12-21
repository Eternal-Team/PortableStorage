using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items.Ammo;
using PortableStorage.Items.Special;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage
{
	public static partial class Hooking
	{
		#region Wallet
		private static void Player_BuyItem(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdloca(0), i => i.MatchLdcI4(4)))
			{
				cursor.RemoveRange(20);

				cursor.Emit(OpCodes.Ldarg_0);
				cursor.Emit(OpCodes.Ldloc, 1);
				cursor.Emit(OpCodes.Ldloc, 2);
				cursor.Emit(OpCodes.Ldloc, 3);
				cursor.Emit(OpCodes.Ldloc, 4);

				cursor.EmitDelegate<Func<Player, long, long, long, long, long>>((player, inventory, bank, bank2, bank3) =>
				{
					long coins = player.inventory.OfType<Wallet>().Sum(wallet => wallet.Coins);
					coins += inventory;
					coins += bank;
					coins += bank2;
					coins += bank3;
					return coins;
				});
			}

			if (cursor.TryGotoNext(i => i.MatchCall<Player>("TryPurchasing")))
			{
				cursor.Remove();
				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.EmitDelegate<Func<int, List<Item[]>, List<Point>, List<Point>, List<Point>, List<Point>, List<Point>, Player, bool>>(Player_TryPurchasing);
			}
		}

		private static void Player_CanBuyItem(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdloca(0), i => i.MatchLdcI4(4)))
			{
				cursor.RemoveRange(20);

				cursor.Emit(OpCodes.Ldarg_0);
				cursor.Emit(OpCodes.Ldloc, 1);
				cursor.Emit(OpCodes.Ldloc, 2);
				cursor.Emit(OpCodes.Ldloc, 3);
				cursor.Emit(OpCodes.Ldloc, 4);

				cursor.EmitDelegate<Func<Player, long, long, long, long, long>>((player, inventory, bank, bank2, bank3) =>
				{
					long coins = player.inventory.OfType<Wallet>().Sum(wallet => wallet.Coins);
					coins += inventory;
					coins += bank;
					coins += bank2;
					coins += bank3;
					return coins;
				});
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
						long addedCoins = price + wallet.Coins;
						wallet.Coins = addedCoins;

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

		private static bool Player_TryPurchasing(int price, List<Item[]> inv, List<Point> slotCoins, List<Point> slotsEmpty, List<Point> slotEmptyBank, List<Point> slotEmptyBank2, List<Point> slotEmptyBank3, Player player)
		{
			long priceRemaining = price;

			foreach (Wallet wallet in player.inventory.OfType<Wallet>())
			{
				long walletCoins = wallet.Coins;
				long sub = Math.Min(walletCoins, priceRemaining);
				priceRemaining -= sub;
				wallet.Coins -= sub;

				if (priceRemaining <= 0) return false;
			}

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
				}
			}

			return result;
		}
		#endregion

		#region Ammo
		private static void Player_HasAmmo(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdloc(0), i => i.MatchLdcI4(58)))
			{
				cursor.Index += 3;

				cursor.Emit(OpCodes.Ldarg_0);
				cursor.Emit(OpCodes.Ldarg_1);

				cursor.EmitDelegate<Func<Player, Item, bool>>((player, ammoUser) => player.inventory.OfType<BaseAmmoBag>().Any(ammoBag => ammoBag.Handler.Items.Any(item => item.ammo == ammoUser.useAmmo && item.stack > 0)));

				cursor.Emit(OpCodes.Starg, 2);
			}
		}

		private static void Player_PickAmmo(ILContext il)
		{
			int firstAmmoIndex = il.AddVariable<Item>();

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
				cursor.EmitDelegate<Func<Player, Item, Item>>((player, sItem) => player.inventory.OfType<BaseAmmoBag>().SelectMany(bag => bag.Handler.Items).FirstOrDefault(ammo => ammo.ammo == sItem.useAmmo && ammo.stack > 0));
				cursor.Emit(OpCodes.Stloc, firstAmmoIndex);

				cursor.Emit(OpCodes.Ldloc, firstAmmoIndex);
				cursor.Emit(OpCodes.Brfalse, elseLabel);

				cursor.Emit(OpCodes.Ldloc, firstAmmoIndex);
				cursor.Emit(OpCodes.Stloc, 0);

				cursor.Emit(OpCodes.Ldarg, 4);
				cursor.Emit(OpCodes.Ldc_I4, 1);
				cursor.Emit(OpCodes.Stind_I1);

				cursor.Emit(OpCodes.Br, endLabel);
			}

			if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(1), i => i.MatchLdcI4(54))) cursor.MarkLabel(elseLabel);

			if (cursor.TryGotoNext(i => i.MatchLdarg(4), i => i.MatchLdindU1(), i => i.MatchBrfalse(out _))) cursor.MarkLabel(endLabel);
		}
		#endregion

		#region Alchemist's Bag
		private delegate bool QuickBuffDelegate(Player player, ref LegacySoundStyle sound);

		private static bool QuickBuff(Player player, ref LegacySoundStyle sound)
		{
			if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickBuff) return false;

			foreach (Item item in player.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items))
			{
				if (player.CountBuffs() == player.buffType.Length) return true;

				if (item.stack > 0 && item.type > 0 && item.buffType > 0 && !item.summon && item.buffType != 90)
				{
					int buffType = item.buffType;
					bool useItem = ItemLoader.CanUseItem(item, player);
					for (int i = 0; i < player.buffType.Length; i++)
					{
						if (buffType == 27 && (player.buffType[i] == buffType || player.buffType[i] == 101 || player.buffType[i] == 102))
						{
							useItem = false;
							break;
						}

						if (player.buffType[i] == buffType)
						{
							useItem = false;
							break;
						}

						if (Main.meleeBuff[buffType] && Main.meleeBuff[player.buffType[i]])
						{
							useItem = false;
							break;
						}
					}

					if (Main.lightPet[item.buffType] || Main.vanityPet[item.buffType])
					{
						for (int buffIndex = 0; buffIndex < player.buffType.Length; buffIndex++)
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
						sound = item.UseSound;
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

			return false;
		}

		private static void Player_QuickBuff(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			if (cursor.TryGotoNext(i => i.MatchLdnull(), i => i.MatchStloc(0)))
			{
				cursor.Index += 2;

				cursor.Emit(OpCodes.Ldarg_0);
				cursor.Emit(OpCodes.Ldloca, 0);

				cursor.EmitDelegate<QuickBuffDelegate>(QuickBuff);

				cursor.Emit(OpCodes.Brfalse, label);
				cursor.Emit(OpCodes.Ret);
				cursor.MarkLabel(label);
			}
		}

		private delegate void QuickHealDelegate(Player player, int lostHealth, ref int healthGain, ref Item result);

		private static void QuickHeal(Player player, int lostHealth, ref int healthGain, ref Item result)
		{
			if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickHeal) return;

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
		}

		private static void Player_QuickHeal_GetItemToUse(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(3), i => i.MatchBr(out _)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldloc, 0);
				cursor.Emit(OpCodes.Ldloca, 2);
				cursor.Emit(OpCodes.Ldloca, 1);

				cursor.EmitDelegate<QuickHealDelegate>(QuickHeal);
			}
		}

		private static void Player_QuickMana(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(0), i => i.MatchStloc(0)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);

				cursor.EmitDelegate<Func<Player, bool>>(player =>
				{
					if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickMana) return false;

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

				cursor.Emit(OpCodes.Brfalse, label);
				cursor.Emit(OpCodes.Ret);
				cursor.MarkLabel(label);
			}
		}
		#endregion

		#region Fishing
		private delegate void ItemCheckDelegate(Player player, int index, ref bool foundBait, ref int baitType);

		private static void ItemCheck(Player player, int index, ref bool foundBait, ref int baitType)
		{
			foreach (Item item in player.inventory.OfType<FishingBelt>().SelectMany(belt => belt.Handler.Items))
			{
				if (item.stack > 0 && item.bait > 0)
				{
					bool consumeBait = false;
					int baitPower = 1 + item.bait / 5;

					if (baitPower < 1) baitPower = 1;
					if (player.accTackleBox) baitPower++;
					if (Main.rand.Next(baitPower) == 0) consumeBait = true;
					if (Main.projectile[index].localAI[1] < 0f) consumeBait = true;

					if (Main.projectile[index].localAI[1] > 0f)
					{
						Item fish = new Item();
						fish.SetDefaults((int)Main.projectile[index].localAI[1]);
						if (fish.rare < 0) consumeBait = false;
					}

					if (consumeBait)
					{
						baitType = item.type;
						if (ItemLoader.ConsumeItem(item, player)) item.stack--;
						if (item.stack <= 0) item.SetDefaults();
					}

					foundBait = true;
					break;
				}
			}
		}

		private static void Player_ItemCheck(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(29)))
			{
				cursor.Index += 2;

				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldloc, 26);
				cursor.Emit(OpCodes.Ldloca, 28);
				cursor.Emit(OpCodes.Ldloca, 29);

				cursor.EmitDelegate<ItemCheckDelegate>(ItemCheck);

				cursor.Emit(OpCodes.Ldloc, 28);
				cursor.Emit(OpCodes.Brtrue, label);
			}

			if (cursor.TryGotoNext(i => i.MatchLdloc(28), i => i.MatchBrfalse(out _))) cursor.MarkLabel(label);
		}

		private static int Player_FishingLevel(On.Terraria.Player.orig_FishingLevel orig, Player self)
		{
			Item fishingPole = self.inventory[self.selectedItem];

			if (fishingPole.fishingPole == 0)
			{
				for (int i = 0; i < 58; i++)
				{
					if (self.inventory[i].fishingPole > fishingPole.fishingPole) fishingPole = self.inventory[i];
				}

				foreach (Item item in self.inventory.OfType<FishingBelt>().SelectMany(belt => belt.Handler.Items))
				{
					if (item.fishingPole > fishingPole.fishingPole) fishingPole = item;
				}
			}

			Item bait = new Item();
			for (int i = 0; i < 58; i++)
			{
				if (self.inventory[i].stack > 0 && self.inventory[i].bait > 0)
				{
					if (self.inventory[i].type == 2673) return -1;

					bait = self.inventory[i];
					break;
				}
			}

			if (bait.IsAir)
			{
				foreach (Item item in self.inventory.OfType<FishingBelt>().SelectMany(belt => belt.Handler.Items))
				{
					if (item.stack > 0 && item.bait > 0)
					{
						if (item.type == 2673) return -1;

						bait = item;
						break;
					}
				}
			}

			if (bait.IsAir || fishingPole.fishingPole == 0) return 0;

			int fishingLevel = bait.bait + fishingPole.fishingPole + self.fishingSkill;
			if (Main.raining) fishingLevel = (int)(fishingLevel * 1.2f);
			if (Main.cloudBGAlpha > 0f) fishingLevel = (int)(fishingLevel * 1.1f);
			if (Main.dayTime && (Main.time < 5400.0 || Main.time > 48600.0)) fishingLevel = (int)(fishingLevel * 1.3f);
			if (Main.dayTime && Main.time > 16200.0 && Main.time < 37800.0) fishingLevel = (int)(fishingLevel * 0.8f);
			if (!Main.dayTime && Main.time > 6480.0 && Main.time < 25920.0) fishingLevel = (int)(fishingLevel * 0.8f);
			if (Main.moonPhase == 0) fishingLevel = (int)(fishingLevel * 1.1f);
			if (Main.moonPhase == 1 || Main.moonPhase == 7) fishingLevel = (int)(fishingLevel * 1.05f);
			if (Main.moonPhase == 3 || Main.moonPhase == 5) fishingLevel = (int)(fishingLevel * 0.95f);
			if (Main.moonPhase == 4) fishingLevel = (int)(fishingLevel * 0.9f);

			PlayerHooks.GetFishingLevel(self, fishingPole, bait, ref fishingLevel);
			return fishingLevel;
		}

		private static void Player_GetItem(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(3), i => i.MatchStloc(4)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldarg, 2);

				cursor.EmitDelegate<Func<Player, Item, Item>>((player, item) =>
				{
					if (ItemUtility.BlockGetItem) return item;

					if (item.bait > 0 || Utility.FishingWhitelist.Contains(item.type))
					{
						FishingBelt belt = player.inventory.OfType<FishingBelt>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

						if (belt != null)
						{
							Main.PlaySound(SoundID.Grab);

							belt.Handler.InsertItem(ref item);
							if (item.IsAir || !item.active) return item;
						}
					}

					return item;
				});

				cursor.Emit(OpCodes.Starg, 2);
			}
		}
		#endregion
	}
}