using System;
using System.Linq;
using ContainerLibrary;
using PortableStorage.Global;
using PortableStorage.Tiles;
using PortableStorage.UI.TileEntities;
using Terraria.ModLoader.IO;

namespace PortableStorage.TileEntities
{
	public class TEQEChest : BaseQETE, IItemHandler
	{
		public override Type TileType => typeof(TileQEChest);
		public override Type UIType => typeof(QEChestPanel);

		public QEChestPanel UI => PortableStorage.Instance.PanelUI.UI.Elements.OfType<QEChestPanel>().FirstOrDefault(x => x.te.ID == ID);

		public ItemHandler Handler
		{
			get
			{
				if (PSWorld.Instance.qeItemHandlers.TryGetValue(frequency, out ItemHandler handler)) return handler;

				ItemHandler temp = PSWorld.baseItemHandler.Clone();
				PSWorld.Instance.qeItemHandlers.Add(new Frequency(frequency.colors), temp);
				return temp;
			}
		}

		public override TagCompound Save() => new TagCompound
		{
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag) => frequency = tag.Get<Frequency>("Frequency");

		//public override void NetSend(BinaryWriter writer, bool lightSend) => writer.Write(frequency);

		//public override void NetReceive(BinaryReader reader, bool lightReceive) => frequency = reader.ReadFrequency();
	}
}