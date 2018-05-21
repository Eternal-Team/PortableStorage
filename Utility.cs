using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using TheOneLibrary.Utils;

namespace PortableStorage
{
	public enum Colors : byte
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
		public Colors[] Colors => new[] { colorLeft, colorMiddle, colorRight };
		public Colors colorLeft;
		public Colors colorMiddle;
		public Colors colorRight;

		public Frequency(Colors left, Colors middle, Colors right)
		{
			colorLeft = left;
			colorMiddle = middle;
			colorRight = right;
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
			switch (TheOneLibrary.Utils.Utility.HeldItem.type)
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

		public static void Write(this BinaryWriter writer, Frequency frequency)
		{
			writer.Write((byte)frequency.colorLeft);
			writer.Write((byte)frequency.colorMiddle);
			writer.Write((byte)frequency.colorRight);
		}

		public static Frequency ReadFrequency(this BinaryReader reader) => new Frequency((Colors)reader.ReadByte(), (Colors)reader.ReadByte(), (Colors)reader.ReadByte());

		public static Color ToColor(this Colors color) => typeof(Color).GetValue<Color>(Enum.GetName(typeof(Colors), color));
	}
}