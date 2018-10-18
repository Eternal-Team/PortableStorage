using Terraria;
using Terraria.ID;

namespace PortableStorage.Global
{
	public static class Utility
	{
		public static Colors ColorFromItem(this Item item, Colors existing)
		{
			Main.LocalPlayer.noThrow = 2;
			switch (item.type)
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
	}
}