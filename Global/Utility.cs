using Terraria.ModLoader.IO;

namespace PortableStorage.Global
{
	public static class Utility
	{
		public static TagCompound Save(this (Colors left, Colors middle, Colors right) tuple) => new TagCompound
		{
			["Left"] = (short)tuple.left,
			["Middle"] = (short)tuple.middle,
			["Right"] = (short)tuple.right
		};

		public static (Colors left, Colors middle, Colors right) Load(this TagCompound tag) => ((Colors)tag.GetShort("Left"), (Colors)tag.GetShort("Middle"), (Colors)tag.GetShort("Right"));
	}
}