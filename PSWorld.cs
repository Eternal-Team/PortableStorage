using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Fluid;

namespace PortableStorage
{
	public enum Colors
	{
		Red,
		White,
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

		public Frequency(Colors colorLeft = Colors.White, Colors colorMiddle = Colors.White, Colors colorRight = Colors.White)
		{
			this.colorLeft = colorLeft;
			this.colorMiddle = colorMiddle;
			this.colorRight = colorRight;
		}

		public override string ToString() => $"{colorLeft} {colorMiddle} {colorRight}";
	}

	public class PSWorld : ModWorld
	{
		public Dictionary<Frequency, List<Item>> enderItems = new Dictionary<Frequency, List<Item>>();
		public Dictionary<Frequency, ModFluid> enderFluids = new Dictionary<Frequency, ModFluid>();

		public override void Initialize()
		{
			List<Item> items = new List<Item>();
			for (int i = 0; i < 27; i++) items.Add(new Item());
			enderItems[new Frequency(Colors.White)] = items;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Frequencies"] = enderItems.Keys.ToList(),
			["Items"] = enderItems.Values.Select(x => x.Select(ItemIO.Save).ToList()).ToList(),

			["KeysFluids"] = enderFluids.Keys.ToList(),
			["ValuesFluids"] = enderFluids.Values.ToList()
		};

		public override void Load(TagCompound tag)
		{
			IList<Frequency> frequencies = tag.GetList<Frequency>("Frequencies");
			IList<List<Item>> items = tag.GetList<List<Item>>("Items");
			enderItems = frequencies.Zip(items, (frequency, list) => new KeyValuePair<Frequency, List<Item>>(frequency, list)).ToDictionary(x => x.Key, x => x.Value);
			if (!enderItems.ContainsKey(new Frequency(Colors.White)))
			{
				List<Item> empty = new List<Item>();
				for (int i = 0; i < 27; i++) empty.Add(new Item());
				enderItems.Add(new Frequency(Colors.White), empty);
			}

			IList<Frequency> keysFluids = tag.GetList<Frequency>("KeysFluids");
			IList<ModFluid> valuesFluids = tag.GetList<ModFluid>("ValuesFluids");
			enderFluids = keysFluids.Zip(valuesFluids, (freq, list) => new KeyValuePair<Frequency, ModFluid>(freq, list)).ToDictionary(x => x.Key, x => x.Value);
			if (!enderFluids.ContainsKey(new Frequency(Colors.White))) enderFluids.Add(new Frequency(Colors.White), null);
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetReceive(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}