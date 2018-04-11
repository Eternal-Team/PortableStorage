using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Base;
using TheOneLibrary.Fluid;

namespace PortableStorage.Global
{
	public class PSWorld : ModWorld
	{
		[Null] public static PSWorld Instance;

		public Dictionary<Frequency, List<Item>> enderItems = new Dictionary<Frequency, List<Item>>();
		public Dictionary<Frequency, ModFluid> enderFluids = new Dictionary<Frequency, ModFluid>();

		public override void Initialize()
		{
			Instance = this;
		}

		public void EnsureFrequencyExists(Frequency frequency, bool fluid = false)
		{
			if (fluid && !enderFluids.ContainsKey(frequency)) enderFluids.Add(frequency, null);
			else if (!fluid && !enderItems.ContainsKey(frequency)) enderItems.Add(frequency, Enumerable.Repeat(new Item(), 27).ToList());
		}

		public List<Item> GetItems(Frequency frequency)
		{
			EnsureFrequencyExists(frequency);
			return enderItems[frequency];
		}

		public ModFluid GetFluid(Frequency frequency)
		{
			EnsureFrequencyExists(frequency, true);
			return enderFluids[frequency];
		}

		public void SetFluid(Frequency frequency, ModFluid value)
		{
			EnsureFrequencyExists(frequency,true);
			enderFluids[frequency] = value;
		}
		
		public List<TagCompound> SaveItems() => enderItems.Where(x => !x.Value.All(y => y.IsAir)).Select(x => new TagCompound { ["Frequency"] = x.Key, ["Items"] = x.Value.Select(ItemIO.Save).ToList() }).ToList();

		public List<TagCompound> SaveFluids() => enderFluids.Where(x => x.Value != null).Select(x => new TagCompound
		{
			["Frequency"] = x.Key,
			["Fluid"] = new TagCompound
			{
				["Type"] = x.Value.Name,
				["Volume"] = x.Value.volume
			}
		}).ToList();

		public void LoadItems(TagCompound tag)
		{
			if (tag.ContainsKey("Items"))
			{
				if (tag.ContainsKey("Frequencies"))
				{
					IList<Frequency> frequencies = tag.GetList<Frequency>("Frequencies");
					IList<List<Item>> items = tag.GetList<List<Item>>("Items");
					enderItems = frequencies.Zip(items, (frequency, list) => new KeyValuePair<Frequency, List<Item>>(frequency, list)).ToDictionary(x => x.Key, x => x.Value);
				}
				else
				{
					List<TagCompound> tags = tag.GetList<TagCompound>("Items").ToList();
					enderItems = tags.Select(x => new KeyValuePair<Frequency, List<Item>>(x.Get<Frequency>("Frequency"), x.GetList<Item>("Items").ToList())).ToDictionary(x => x.Key, x => x.Value);
				}
			}
		}

		public void LoadFluids(TagCompound tag)
		{
			if (tag.ContainsKey("KeysFluids") && tag.ContainsKey("ValuesFluids"))
			{
				IList<Frequency> keysFluids = tag.GetList<Frequency>("KeysFluids");
				IList<ModFluid> valuesFluids = tag.GetList<ModFluid>("ValuesFluids");
				enderFluids = keysFluids.Zip(valuesFluids, (freq, list) => new KeyValuePair<Frequency, ModFluid>(freq, list)).ToDictionary(x => x.Key, x => x.Value);
			}
			else if (tag.ContainsKey("Fluids"))
			{
				List<TagCompound> tags = tag.GetList<TagCompound>("Fluids").ToList();
				enderFluids = tags.Select(x =>
				{
					TagCompound fluidTag = x.Get<TagCompound>("Fluid");
					ModFluid fluid = TheOneLibrary.Utils.Utility.SetDefaults(fluidTag.GetString("Type"));
					fluid.volume = fluidTag.GetInt("Volume");
					return new KeyValuePair<Frequency, ModFluid>(x.Get<Frequency>("Frequency"), fluid);
				}).ToDictionary(x => x.Key, x => x.Value);
			}
		}

		public override TagCompound Save() => new TagCompound
		{
			{"Items", SaveItems()},
			{"Fluids", SaveFluids()}
		};

		public override void Load(TagCompound tag)
		{
			LoadItems(tag);
			LoadFluids(tag);
		}
	}
}