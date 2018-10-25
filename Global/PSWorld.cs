using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Global
{
	public class PSWorld : ModWorld
	{
		public static PSWorld Instance;

		public Dictionary<Frequency, ItemHandler> qeItemHandlers;
		public Dictionary<Frequency, FluidHandler> qeFluidHandlers;
		public static ItemHandler baseItemHandler;
		public static FluidHandler baseFluidHandler;

		public override void Initialize()
		{
			Instance = this;

			baseItemHandler = new ItemHandler(27);
			baseFluidHandler = new FluidHandler();
			baseFluidHandler.GetSlotLimit += slot => 255 * 8;

			qeItemHandlers = new Dictionary<Frequency, ItemHandler>();
			qeFluidHandlers = new Dictionary<Frequency, FluidHandler>();
		}

		public override TagCompound Save() => new TagCompound
		{
			["QEItems"] = qeItemHandlers.Select(x => new TagCompound
			{
				["Frequency"] = x.Key,
				["Items"] = x.Value.Save()
			}).ToList(),
			["QEFluids"] = qeFluidHandlers.Select(x => new TagCompound
			{
				["Frequency"] = x.Key,
				["Fluids"] = x.Value.Save()
			}).ToList()
		};

		public override void Load(TagCompound tag)
		{
			qeItemHandlers = tag.GetList<TagCompound>("QEItems").ToDictionary(c => c.Get<Frequency>("Frequency"), c => baseItemHandler.Clone().Load(c.GetCompound("Items")));
			qeFluidHandlers = tag.GetList<TagCompound>("QEFluids").ToDictionary(c => c.Get<Frequency>("Frequency"), c => baseFluidHandler.Clone().Load(c.GetCompound("Fluids")));
		}
	}
}