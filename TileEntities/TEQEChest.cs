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
		public int animState;
		public int animTimer;

		public bool opened;

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j - 1);

			NetMessage.SendTileSquare(Main.myPlayer, i, j - 1, 2);
			NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j - 1, number3: Type);

			return -1;
		}

		public override void Update()
		{
			//if (opened && animState < 2)
			//{
			//	if (++animTimer >= 10)
			//	{
			//		animState++;
			//		animTimer = 0;
			//		WorldGen.TileFrame(Position.X, Position.Y);
			//		WorldGen.TileFrame(Position.X, Position.Y + 1);
			//		WorldGen.TileFrame(Position.X + 1, Position.Y);
			//		WorldGen.TileFrame(Position.X + 1, Position.Y + 1);
			//		//WorldGen.SectionTileFrame(Position.X, Position.Y, Position.X + 1, Position.Y + 1);
			//		//mod.ClientSendTEUpdate(ID);
			//		//this.SendUpdate();
			//	}
			//}
			//else if (!opened && animState > 0)
			//{
			//	if (++animTimer >= 10)
			//	{
			//		animState--;
			//		animTimer = 0;

			//		WorldGen.TileFrame(Position.X, Position.Y);
			//		WorldGen.TileFrame(Position.X, Position.Y + 1);
			//		WorldGen.TileFrame(Position.X + 1, Position.Y);
			//		WorldGen.TileFrame(Position.X + 1, Position.Y + 1);
			//		//mod.ClientSendTEUpdate(ID);
			//		//this.SendUpdate();
			//	}
			//}

			this.HandleUIFar();
		}

		public override TagCompound Save() => new TagCompound
		{
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag)
		{
			frequency = tag.Get<Frequency>("Frequency");
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.Write((int)frequency.colorLeft);
			writer.Write((int)frequency.colorMiddle);
			writer.Write((int)frequency.colorRight);
			writer.Write(opened);
			writer.Write(animState);
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			frequency.colorLeft = (Colors)reader.ReadInt32();
			frequency.colorMiddle = (Colors)reader.ReadInt32();
			frequency.colorRight = (Colors)reader.ReadInt32();
			opened = reader.ReadBoolean();
			animState = reader.ReadInt32();
		}

		public List<Item> GetItems() => PSWorld.Instance.GetItemStorage(frequency);

		public Item GetItem(int slot) => PSWorld.Instance.GetItemStorage(frequency)[slot];

		public void SetItem(int slot, Item value)
		{
			PSWorld.Instance.GetItemStorage(frequency)[slot] = value;
			Net.SyncQEItems();
		}

		public ModTileEntity GetTileEntity() => this;
	}
}