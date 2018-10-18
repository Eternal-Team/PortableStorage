using System.Linq;
using Terraria.ModLoader.IO;

namespace PortableStorage.Global
{
	public struct Frequency
	{
		public Colors[] colors;

		public Frequency(params Colors[] colors) => this.colors = new[] { colors[0], colors[1], colors[2] };

		public Colors this[int index]
		{
			get => colors[index];
			set => colors[index] = value;
		}

		public override int GetHashCode() => int.Parse($"{(int)colors[0]}{(int)colors[1]}{(int)colors[2]}");

		public override bool Equals(object obj)
		{
			if (obj is Frequency freq) return freq.colors.SequenceEqual(colors);
			return base.Equals(obj);
		}
	}

	public class FrequencySerializer : TagSerializer<Frequency, TagCompound>
	{
		public override TagCompound Serialize(Frequency value) => new TagCompound
		{
			["Value"] = value.colors.Select(x => (int)x).ToList()
		};

		public override Frequency Deserialize(TagCompound tag) => new Frequency(tag.GetList<int>("Value").Select(x => (Colors)x).ToArray());
	}
}