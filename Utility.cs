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

		public static void QuickStack(IContainerItem container)
		{
			if (Main.LocalPlayer.IsStackingItems()) return;
			IList<Item> Items = container.GetItems();

			bool stacked = false;
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].type > 0 && Items[i].stack > 0 && !Items[i].favorited)
				{
					int type = Items[i].type;
					int stack = Items[i].stack;
					Items[i] = Chest.PutItemInNearbyChest(Items[i], Main.LocalPlayer.Center);
					if (Items[i].type != type || Items[i].stack != stack) stacked = true;
				}
			}

			if (stacked) Main.PlaySound(7);
		}

		public static void LootAll(IContainerItem container)
		{
			Player player = Main.LocalPlayer;
			IList<Item> Items = container.GetItems();

			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].type > 0)
				{
					Items[i].position = player.Center;
					Items[i] = player.GetItem(Main.myPlayer, Items[i]);
				}
			}
		}

		public static void DepositAll(IContainerItem container)
		{
			Player player = Main.LocalPlayer;
			IList<Item> Items = container.GetItems();

			for (int pIndex = 49; pIndex >= 10; pIndex--)
			{
				if (player.inventory[pIndex].stack > 0 && player.inventory[pIndex].type > 0 && !player.inventory[pIndex].favorited)
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