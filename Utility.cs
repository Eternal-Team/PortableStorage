using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace PortableStorage
{
	public static class Utility
	{
		public static readonly List<int> Gems = new List<int>
		{
			ItemID.Diamond ,
			ItemID.Ruby    ,
			ItemID.Emerald ,
			ItemID.Sapphire,
			ItemID.Topaz   ,
			ItemID.Amethyst,
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
				case ItemID.Sapphire: return Colors.Blue;
				case ItemID.Topaz: return Colors.Yellow;
				case ItemID.Amethyst: return Colors.Purple;
				case ItemID.Amber: return Colors.Orange;
				default: return existing;
			}
		}
	}
}