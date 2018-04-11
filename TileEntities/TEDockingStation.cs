using System.Collections.Generic;
using System.IO;
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
	public class TEDockingStation : BaseTE, IContainerTile
	{
		public Item Bag = new Item();

		public override bool ValidTile(Tile tile) => tile.type == mod.TileType<DockingStation>() && tile.TopLeft();

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j - 1);

			NetMessage.SendTileSquare(Main.myPlayer, i, j - 1, 2);
			NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j - 1, number3: Type);

			return -1;
		}

		// bug: doesn't get run in MP
		public override void Update()
		{
			this.HandleUIFar();
		}

		public override TagCompound Save() => new TagCompound
		{
			["Bag"] = ItemIO.Save(Bag)
		};

		public override void Load(TagCompound tag)
		{
			Bag = tag.Get<Item>("Bag");
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.WriteItem(Bag);
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			Bag = reader.ReadItem();
		}

		public List<Item> GetItems() => ((IContainerItem)Bag.modItem).GetItems();

		public Item GetItem(int slot) => ((IContainerItem)Bag.modItem).GetItem(slot);

		public void SetItem(int slot, Item value) => ((IContainerItem)Bag.modItem).SetItem(slot, value);

		public void Sync(int slot) => ((IContainerItem)Bag.modItem).Sync(slot);

		public ModTileEntity GetTileEntity() => this;
	}
}