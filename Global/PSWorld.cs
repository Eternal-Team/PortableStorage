﻿using System.Collections.Generic;
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
				["Fluid"] = x.Value.Save()
			}).ToList()
		};

		public override void Load(TagCompound tag)
		{
			foreach (TagCompound compound in tag.GetList<TagCompound>("QEItems")) qeItemHandlers.Add(compound.Get<Frequency>("Frequency"), baseItemHandler.Clone().Load(compound.GetCompound("Items")));
			foreach (TagCompound compound in tag.GetList<TagCompound>("QEFluids")) qeFluidHandlers.Add(compound.Get<Frequency>("Frequency"), baseFluidHandler.Clone().Load(compound.GetCompound("Fluids")));
		}
	}
}