using System;
using System.Linq;
using BaseLibrary.Tiles.TileEntites;
using ContainerLibrary;
using PortableStorage.Global;
using PortableStorage.Tiles;
using PortableStorage.UI.TileEntities;
using Terraria.ModLoader.IO;

namespace PortableStorage.TileEntities
{
	public class TEQETank : BaseTEWithUI<QETankPanel>, IFluidHandler
	{
		public override Type TileType => typeof(TileQETank);

		public override QETankPanel UI => PortableStorage.Instance.PanelUI.UI.Elements.OfType<QETankPanel>().FirstOrDefault(x => x.te.ID == ID);

		public Frequency frequency = new Frequency(Colors.White, Colors.White, Colors.White);

		public FluidHandler Handler
		{
			get
			{
				if (PSWorld.Instance.qeFluidHandlers.TryGetValue(frequency, out FluidHandler handler)) return handler;

				FluidHandler temp = PSWorld.baseFluidHandler.Clone();
				PSWorld.Instance.qeFluidHandlers.Add(new Frequency(frequency.colors), temp);
				return temp;
			}
		}

		public bool hovered;
		public bool inScreen;
		public float scale;

		public override TagCompound Save() => new TagCompound
		{
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag) => frequency = tag.Get<Frequency>("Frequency");
	}
}