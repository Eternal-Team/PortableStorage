using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Fluid;

namespace PortableStorage
{
	public class PSWorld : ModWorld
	{
		public Dictionary<Frequency, List<Item>> enderItems = new Dictionary<Frequency, List<Item>>();
		public Dictionary<Frequency, ModFluid> enderFluids = new Dictionary<Frequency, ModFluid>();

		public List<Item> GetItemStorage(Frequency frequency)
		{
			if (!enderItems.ContainsKey(frequency))
			{
				List<Item> items = new List<Item>();
				for (int i = 0; i < 27; i++) items.Add(new Item());
				enderItems.Add(frequency, items);
				Net.SyncQE();
			}

			return enderItems[frequency];
		}

		public ModFluid GetFluidStorage(Frequency frequency)
		{
			if (!enderFluids.ContainsKey(frequency))
			{
				enderFluids.Add(frequency, null);
				Net.SyncQE();
			}

			return enderFluids[frequency];
		}

		public void SetFluidStorage(Frequency frequency, ModFluid value)
		{
			if (!enderFluids.ContainsKey(frequency))
			{
				enderFluids.Add(frequency, null);
				Net.SyncQE();
			}

			enderFluids[frequency] = value;
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			tag.Add("Items", enderItems.Where(x => !x.Value.All(y => y.IsAir)).Select(x => new TagCompound { ["Frequency"] = x.Key, ["Items"] = x.Value.Select(ItemIO.Save).ToList() }).ToList());
			tag.Add("Fluids", enderFluids.Where(x => x.Value != null).Select(x => new TagCompound
			{
				["Frequency"] = x.Key,
				["Fluid"] = new TagCompound
				{
					["Type"] = x.Value.Name,
					["Volume"] = x.Value.volume
				}
			}).ToList());

			return tag;
		}

		public override void Load(TagCompound tag)
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
					ModFluid fluid = TheOneLibrary.Utility.Utility.SetDefaults(fluidTag.GetString("Type"));
					fluid.volume = fluidTag.GetInt("Volume");
					return new KeyValuePair<Frequency, ModFluid>(x.Get<Frequency>("Frequency"), fluid);
				}).ToDictionary(x => x.Key, x => x.Value);
			}
		}
	}
}