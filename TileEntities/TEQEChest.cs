using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using PortableStorage.Global;
using PortableStorage.Tiles;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using TheOneLibrary.Utils;

namespace PortableStorage.TileEntities
{
	public class TEQEChest : BaseTE, IContainerTile
	{
		public override bool ValidTile(Tile tile) => tile.type == mod.TileType<QEChest>() && tile.TopLeft();

		public GUI<QEChestUI> gui;

		public Frequency frequency;

		public TEQEChest()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				QEChestUI ui = new QEChestUI();
				ui.SetContainer(this);
				UserInterface userInterface = new UserInterface();
				ui.Activate();
				userInterface.SetState(ui);
				gui = new GUI<QEChestUI>(ui, userInterface);
			}
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j - 1);

			NetMessage.SendTileSquare(Main.myPlayer, i, j - 1, 2);
			NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j - 1, number3: Type);

			return -1;
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			tag["Frequency"] = frequency;
			if (gui != null) tag["UIPosition"] = gui.ui.panelMain.GetDimensions().Position();
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			frequency = tag.Get<Frequency>("Frequency");

			if (gui != null && tag.ContainsKey("UIPosition"))
			{
				Vector2 vector = tag.Get<Vector2>("UIPosition");
				gui.ui.panelMain.Left.Set(vector.X, 0f);
				gui.ui.panelMain.Top.Set(vector.Y, 0f);
				gui.ui.panelMain.Recalculate();
			}
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