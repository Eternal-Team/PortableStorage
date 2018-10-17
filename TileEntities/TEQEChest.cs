using System;
using System.Linq;
using ContainerLibrary;
using PortableStorage.Global;
using PortableStorage.Tiles;
using PortableStorage.UI.TileEntities;

namespace PortableStorage.TileEntities
{
	public class TEQEChest : BasePSTE
	{
		public override Type TileType => typeof(TileQEChest);
		public override Type UIType => typeof(QEChestPanel);

		public QEChestPanel UI => PortableStorage.Instance.PanelUI.UI.Elements.OfType<QEChestPanel>().FirstOrDefault(x => x.te.ID == ID);

		public bool hovered;
		public float scale;

		public Colors[] frequency = { Colors.White, Colors.White, Colors.White };

		public ItemHandler Handler
		{
			get
			{
				if (PSWorld.Instance.qeItemHandlers.ContainsKey(frequency)) return PSWorld.Instance.qeItemHandlers[frequency];

				ItemHandler temp = PSWorld.baseItemHandler.Clone();
				PSWorld.Instance.qeItemHandlers.Add(frequency, temp);
				return temp;
			}
		}

		public override void Update()
		{
			if (!hovered && scale > 0f) scale -= 0.025f;
			else if (hovered && scale < 1f) scale += 0.025f;
		}

		//public override TagCompound Save() => new TagCompound
		//{
		//	["Frequency"] = frequency.Value
		//};

		//public override void Load(TagCompound tag)
		//{
		//	frequency = new Ref<Color>(tag.Get<Color>("Frequency"));
		//}

		//public override void NetSend(BinaryWriter writer, bool lightSend) => writer.Write(frequency);

		//public override void NetReceive(BinaryReader reader, bool lightReceive) => frequency = reader.ReadFrequency();
	}
}