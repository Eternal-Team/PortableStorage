using System.IO;
using System.Linq;
using PortableStorage.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Fluid;

namespace PortableStorage
{
	public static class Net
	{
		internal enum MessageType : byte
		{
			SyncQEItem,
			SyncQEFluid
		}

		public static void HandlePacket(BinaryReader reader, int sender)
		{
			MessageType type = (MessageType)reader.ReadByte();
			switch (type)
			{
				case MessageType.SyncQEItem:
					ReceiveQEItem(reader, sender);
					break;
				case MessageType.SyncQEFluid:
					ReceiveQEFluid(reader, sender);
					break;
			}
		}

		#region Items
		public static void ReceiveQEItem(BinaryReader reader, int sender)
		{
			TagCompound tag = TagIO.Read(reader);

			Frequency frequency = tag.Get<Frequency>("Frequency");
			int slot = tag.GetInt("Slot");
			Item item = ItemIO.Load(tag.GetCompound("Item"));

			if (!PSWorld.Instance.enderItems.ContainsKey(frequency)) PSWorld.Instance.enderItems.Add(frequency, Enumerable.Repeat(new Item(), 27).ToList());
			PSWorld.Instance.enderItems[frequency][slot] = item;

			if (Main.netMode == NetmodeID.Server) SendQEItem(frequency, slot, sender);
		}

		public static void SendQEItem(Frequency frequency, int slot, int excludedPlayer = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = PortableStorage.Instance.GetPacket();
			packet.Write((byte)MessageType.SyncQEItem);
			TagIO.Write(new TagCompound
			{
				["Frequency"] = frequency,
				["Slot"] = slot,
				["Item"] = ItemIO.Save(PSWorld.Instance.enderItems[frequency][slot])
			}, packet);
			packet.Send(ignoreClient: excludedPlayer);
		}
		#endregion

		#region Fluids
		public static void ReceiveQEFluid(BinaryReader reader, int sender)
		{
			TagCompound tag = TagIO.Read(reader);

			Frequency frequency = tag.Get<Frequency>("Frequency");
			ModFluid fluid = tag.Get<ModFluid>("Fluid");

			if (!PSWorld.Instance.enderFluids.ContainsKey(frequency)) PSWorld.Instance.enderFluids.Add(frequency, null);
			PSWorld.Instance.enderFluids[frequency] = fluid;

			if (Main.netMode == NetmodeID.Server) SendQEFluid(frequency, sender);
		}

		public static void SendQEFluid(Frequency frequency, int excludedPlayer = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = PortableStorage.Instance.GetPacket();
			packet.Write((byte)MessageType.SyncQEFluid);
			TagIO.Write(new TagCompound
			{
				["Frequency"] = frequency,
				["Fluid"] = PSWorld.Instance.enderFluids[frequency]
			}, packet);
			packet.Send();
		}
		#endregion
	}
}