using System.Collections.Generic;
using System.IO;
using PortableStorage.Global;
using PortableStorage.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Base;
using TheOneLibrary.Storage;
using TheOneLibrary.Utils;

namespace PortableStorage.TileEntities
{
	public class TEQEChest : BaseTE, IContainerTile
	{
		public override bool ValidTile(Tile tile) => tile.type == mod.TileType<QEChest>() && tile.TopLeft();

		public Frequency frequency;

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j - 1);

			NetMessage.SendTileSquare(Main.myPlayer, i, j - 1, 2);
			NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j - 1, number3: Type);

			return -1;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag)
		{
			frequency = tag.Get<Frequency>("Frequency");
		}

		public override void NetSend(BinaryWriter writer, bool lightSend) => writer.Write(frequency);

		public override void NetReceive(BinaryReader reader, bool lightReceive) => frequency = reader.ReadFrequency();

		public List<Item> GetItems() => PSWorld.Instance.GetItems(frequency);

		public Item GetItem(int slot) => PSWorld.Instance.GetItems(frequency)[slot];

		public void SetItem(int slot, Item value) => PSWorld.Instance.GetItems(frequency)[slot] = value;

		public void Sync(int slot) => Net.SendQEItem(frequency, slot);

		public ModTileEntity GetTileEntity() => this;
	}
}