using System;
using PortableStorage.Tiles;
using PortableStorage.UI.TileEntities;
using Terraria.ModLoader.IO;

namespace PortableStorage.TileEntities
{
	public class TEQEChest : BasePSTE
	{
		public override Type TileType => typeof(TileQEChest);
		public override Type UIType => typeof(TEPanel);

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			//tag["Frequency"] = frequency;
			//if (gui != null) tag["UIPosition"] = gui.ui.panelMain.GetDimensions().Position();
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			//frequency = tag.Get<Frequency>("Frequency");

			//if (gui != null && tag.ContainsKey("UIPosition"))
			//{
			//	Vector2 vector = tag.Get<Vector2>("UIPosition");
			//	gui.ui.panelMain.Left.Set(vector.X, 0f);
			//	gui.ui.panelMain.Top.Set(vector.Y, 0f);
			//	gui.ui.panelMain.Recalculate();
			//}
		}

		//public override void NetSend(BinaryWriter writer, bool lightSend) => writer.Write(frequency);

		//public override void NetReceive(BinaryReader reader, bool lightReceive) => frequency = reader.ReadFrequency();
	}
}