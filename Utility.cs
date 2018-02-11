using System.Collections.Generic;
using Terraria;
using Terraria.ID;

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
    }
}