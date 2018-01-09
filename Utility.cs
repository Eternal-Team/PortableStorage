using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using TheOneLibrary.Storage;

namespace PortableStorage
{
	public enum Colors
	{
		White,
		Red,
		Green,
		Yellow,
		Purple,
		Blue,
		Orange
	}

	public struct Frequency
	{
		public Colors colorLeft;
		public Colors colorMiddle;
		public Colors colorRight;

		public Frequency(Colors colorLeft, Colors colorMiddle, Colors colorRight)
		{
			this.colorLeft = colorLeft;
			this.colorMiddle = colorMiddle;
			this.colorRight = colorRight;
		}

		public override string ToString() => $"{colorLeft} {colorMiddle} {colorRight}";
	}

	public static class Utility
	{
		public static readonly List<int> Gems = new List<int>
		{
			ItemID.Diamond,
			ItemID.Ruby,
			ItemID.Emerald,
			ItemID.Topaz,
			ItemID.Amethyst,
			ItemID.Sapphire,
			ItemID.Amber
		};

		public static Colors ColorFromItem(Colors existing)
		{
			Main.LocalPlayer.noThrow = 2;
			switch (TheOneLibrary.Utility.Utility.HeldItem.type)
			{
				case ItemID.Diamond: return Colors.White;
				case ItemID.Ruby: return Colors.Red;
				case ItemID.Emerald: return Colors.Green;
				case ItemID.Topaz: return Colors.Yellow;
				case ItemID.Amethyst: return Colors.Purple;
				case ItemID.Sapphire: return Colors.Blue;
				case ItemID.Amber: return Colors.Orange;
				default: return existing;
			}
		}

		public static void MoveCoins(Item[] playerInv, IContainerItem container)
		{
			IList<Item> containerInv = container.GetItems();
			int[] coins = new int[4];

			List<int> coinSlotsPlayer = new List<int>();
			List<int> coinSlotsContainer = new List<int>();

			bool anyCoins = false;
			int[] coinValueArr = new int[40];

			for (int i = 0; i < containerInv.Count; i++)
			{
				coinValueArr[i] = -1;
				if (containerInv[i].stack < 1 || containerInv[i].type < 1)
				{
					coinSlotsContainer.Add(i);
					containerInv[i] = new Item();
				}
				if (containerInv[i] != null && containerInv[i].stack > 0)
				{
					int num = 0;
					switch (containerInv[i].type)
					{
						case 71:
							num = 1;
							break;
						case 72:
							num = 2;
							break;
						case 73:
							num = 3;
							break;
						case 74:
							num = 4;
							break;
					}
					coinValueArr[i] = num - 1;
					if (num > 0)
					{
						coins[num - 1] += containerInv[i].stack;
						coinSlotsContainer.Add(i);
						containerInv[i] = new Item();
						anyCoins = true;
					}
				}
			}

			if (!anyCoins) return;
			Main.PlaySound(7);

			for (int j = 0; j < playerInv.Length; j++)
			{
				if (j != 58 && playerInv[j] != null && playerInv[j].stack > 0)
				{
					int num2 = 0;
					switch (playerInv[j].type)
					{
						case 71:
							num2 = 1;
							break;
						case 72:
							num2 = 2;
							break;
						case 73:
							num2 = 3;
							break;
						case 74:
							num2 = 4;
							break;
					}
					if (num2 > 0)
					{
						coins[num2 - 1] += playerInv[j].stack;
						coinSlotsPlayer.Add(j);
						playerInv[j] = new Item();
					}
				}
			}
			for (int k = 0; k < 3; k++)
			{
				while (coins[k] >= 100)
				{
					coins[k] -= 100;
					coins[k + 1]++;
				}
			}
			for (int l = 0; l < 40; l++)
			{
				if (coinValueArr[l] >= 0 && containerInv[l].type == 0)
				{
					int num3 = l;
					int num4 = coinValueArr[l];
					if (coins[num4] > 0)
					{
						containerInv[num3].SetDefaults(71 + num4);
						containerInv[num3].stack = coins[num4];
						if (containerInv[num3].stack > containerInv[num3].maxStack) containerInv[num3].stack = containerInv[num3].maxStack;
						coins[num4] -= containerInv[num3].stack;
						coinValueArr[l] = -1;
					}

					coinSlotsContainer.Remove(num3);
				}
			}
			for (int m = 0; m < 40; m++)
			{
				if (coinValueArr[m] >= 0 && containerInv[m].type == 0)
				{
					int num5 = m;
					int n = 3;
					while (n >= 0)
					{
						if (coins[n] > 0)
						{
							containerInv[num5].SetDefaults(71 + n);
							containerInv[num5].stack = coins[n];
							if (containerInv[num5].stack > containerInv[num5].maxStack) containerInv[num5].stack = containerInv[num5].maxStack;
							coins[n] -= containerInv[num5].stack;
							coinValueArr[m] = -1;
							break;
						}
						if (coins[n] == 0) n--;
					}

					coinSlotsContainer.Remove(num5);
				}
			}
			while (coinSlotsContainer.Count > 0)
			{
				int num6 = coinSlotsContainer[0];
				int num7 = 3;
				while (num7 >= 0)
				{
					if (coins[num7] > 0)
					{
						containerInv[num6].SetDefaults(71 + num7);
						containerInv[num6].stack = coins[num7];
						if (containerInv[num6].stack > containerInv[num6].maxStack) containerInv[num6].stack = containerInv[num6].maxStack;
						coins[num7] -= containerInv[num6].stack;
						break;
					}
					if (coins[num7] == 0) num7--;
				}

				coinSlotsContainer.RemoveAt(0);
			}
			int num8 = 3;
			while (num8 >= 0 && coinSlotsPlayer.Count > 0)
			{
				int num9 = coinSlotsPlayer[0];
				if (coins[num8] > 0)
				{
					playerInv[num9].SetDefaults(71 + num8);
					playerInv[num9].stack = coins[num8];
					if (playerInv[num9].stack > playerInv[num9].maxStack) playerInv[num9].stack = playerInv[num9].maxStack;
					coins[num8] -= playerInv[num9].stack;
				}
				if (coins[num8] == 0) num8--;
				coinSlotsPlayer.RemoveAt(0);
			}
		}

		public static void QuickStack(IContainerItem container, Func<Item, bool> selector = null)
		{
			if (Main.LocalPlayer.IsStackingItems()) return;
			IList<Item> Items = container.GetItems();

			bool stacked = false;
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].type > 0 && Items[i].stack > 0 && !Items[i].favorited && (selector?.Invoke(Items[i]) ?? true))
				{
					int type = Items[i].type;
					int stack = Items[i].stack;
					Items[i] = Chest.PutItemInNearbyChest(Items[i], Main.LocalPlayer.Center);
					if (Items[i].type != type || Items[i].stack != stack) stacked = true;
				}
			}

			if (stacked) Main.PlaySound(7);
		}

		public static void LootAll(IContainerItem container, Func<Item, bool> selector = null)
		{
			Player player = Main.LocalPlayer;
			IList<Item> Items = container.GetItems();

			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].type > 0 && (selector?.Invoke(Items[i]) ?? true))
				{
					Items[i].position = player.Center;
					Items[i] = player.GetItem(Main.myPlayer, Items[i]);
				}
			}
		}

		public static void DepositAll(IContainerItem container, Func<Item, bool> selector = null)
		{
			Player player = Main.LocalPlayer;
			IList<Item> Items = container.GetItems();

			MoveCoins(player.inventory, container);

			for (int pIndex = 49; pIndex >= 10; pIndex--)
			{
				if (player.inventory[pIndex].stack > 0 && player.inventory[pIndex].type > 0 && !player.inventory[pIndex].favorited && (selector?.Invoke(player.inventory[pIndex]) ?? true))
				{
					if (player.inventory[pIndex].maxStack > 1)
					{
						for (int bIndex = 0; bIndex < Items.Count; bIndex++)
						{
							if (Items[bIndex].stack < Items[bIndex].maxStack && player.inventory[pIndex].IsTheSameAs(Items[bIndex]))
							{
								int stack = player.inventory[pIndex].stack;
								if (player.inventory[pIndex].stack + Items[bIndex].stack > Items[bIndex].maxStack) stack = Items[bIndex].maxStack - Items[bIndex].stack;

								player.inventory[pIndex].stack -= stack;
								Items[bIndex].stack += stack;
								Main.PlaySound(7);

								if (player.inventory[pIndex].stack <= 0)
								{
									player.inventory[pIndex].SetDefaults();
									break;
								}
								if (Items[bIndex].type == 0)
								{
									Items[bIndex] = player.inventory[pIndex].Clone();
									player.inventory[pIndex].SetDefaults();
								}
							}
						}
					}
					if (player.inventory[pIndex].stack > 0)
					{
						for (int bIndex = 0; bIndex < Items.Count; bIndex++)
						{
							if (Items[bIndex].stack == 0)
							{
								Main.PlaySound(7);
								Items[bIndex] = player.inventory[pIndex].Clone();
								player.inventory[pIndex].SetDefaults();
								break;
							}
						}
					}
				}
			}
		}
	}
}