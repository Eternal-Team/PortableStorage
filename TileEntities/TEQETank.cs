using PortableStorage.Tiles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Base;
using TheOneLibrary.Fluid;
using TheOneLibrary.Storage;
using TheOneLibrary.Utility;

namespace PortableStorage.TileEntities
{
	public class TEQETank : BaseTE, IFluidContainer
	{
		public const int MaxVolume = 16000;

		public override bool ValidTile(Tile tile) => tile.type == mod.TileType<QETank>() && tile.TopLeft();

		public Frequency frequency;

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) return Place(i, j - 1);

			NetMessage.SendTileSquare(Main.myPlayer, i, j - 1, 2);
			NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j - 1, number3: Type);
			return -1;
		}

		public override void OnPlace()
		{
		}

		public override void OnNetPlace() => OnPlace();

		public override void Update()
		{
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
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			frequency.colorLeft = (Colors)reader.ReadInt32();
			frequency.colorMiddle = (Colors)reader.ReadInt32();
			frequency.colorRight = (Colors)reader.ReadInt32();
		}

		public IList<ModFluid> GetFluids() => new List<ModFluid> { mod.GetModWorld<PSWorld>().GetFluidStorage(frequency) };

		public void SetFluid(ModFluid value, int slot = 0) => mod.GetModWorld<PSWorld>().SetFluidStorage(frequency, value);

		public ModFluid GetFluid(int slot = 0) => mod.GetModWorld<PSWorld>().GetFluidStorage(frequency);

		public int GetFluidCapacity(int slot = 0) => MaxVolume;

		public ModTileEntity GetTileEntity() => this;
	}
}