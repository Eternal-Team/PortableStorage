using ContainerLibrary;
using PortableStorage.Global;
using PortableStorage.Tiles;
using PortableStorage.UI.TileEntities;
using System;
using Terraria.ModLoader.IO;

namespace PortableStorage.TileEntities
{
	public class TEQETank : BasePSTE
	{
		public override Type TileType => typeof(TileQETank);
		public override Type UIType => typeof(QETankPanel);

		//public QEChestPanel UI => PortableStorage.Instance.PanelUI.UI.Elements.OfType<QEChestPanel>().FirstOrDefault(x => x.te.ID == ID);

		public Frequency frequency = new Frequency(Colors.White, Colors.White, Colors.White);

		public FluidHandler Handler
		{
			get
			{
				if (PSWorld.Instance.qeFluidHandlers.TryGetValue(frequency, out FluidHandler handler)) return handler;

				FluidHandler temp = PSWorld.baseFluidHandler.Clone();
				PSWorld.Instance.qeFluidHandlers.Add(frequency, temp);
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