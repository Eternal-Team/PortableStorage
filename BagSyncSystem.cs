using System;
using System.Collections.Generic;
using System.IO;
using BaseLibrary.Utility;
using ContainerLibrary;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage;

public enum PacketID : byte
{
	Inventory,
	PickupMode,
	SelectedIndex
}

public class BagSyncSystem : ModSystem
{
	public static BagSyncSystem Instance => ModContent.GetInstance<BagSyncSystem>();

	private Dictionary<PacketID, HashSet<Guid>> BagsToSync = new()
	{
		{ PacketID.Inventory, new HashSet<Guid>() },
		{ PacketID.PickupMode, new HashSet<Guid>() },
		{ PacketID.SelectedIndex, new HashSet<Guid>() }
	};

	public readonly Dictionary<Guid, BaseBag> AllBags = new();

	public void Sync(Guid id, PacketID type)
	{
		BagsToSync[type].Add(id);
	}

	public override void PostUpdateItems()
	{
		// if (Main.netMode == NetmodeID.MultiplayerClient)
		// 	Main.NewText($"[CLIENT {Main.LocalPlayer.whoAmI}] Tracking {AllBags.Count} bags");
		// else
		// 	ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"[SERVER] Tracking {AllBags.Count} bags"), Color.White);

		if (Main.netMode != NetmodeID.MultiplayerClient)
			return;

		foreach (var id in BagsToSync[PacketID.Inventory])
		{
			if (!AllBags.ContainsKey(id)) continue;

			// todo: change this to something like virtual BaseBag.GetPacket
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)PacketID.Inventory);
			packet.Write((byte)Main.LocalPlayer.whoAmI);
			packet.Write(id);
			AllBags[id].GetItemStorage().Write(packet);
			if (AllBags[id] is AlchemistBag alchemistBag) alchemistBag.IngredientStorage.Write(packet);
			packet.Send();
		}

		BagsToSync[PacketID.Inventory].Clear();

		foreach (var id in BagsToSync[PacketID.PickupMode])
		{
			if (!AllBags.ContainsKey(id)) continue;

			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)PacketID.PickupMode);
			packet.Write((byte)Main.LocalPlayer.whoAmI);
			packet.Write(id);
			packet.Write((byte)AllBags[id].PickupMode);
			packet.Send();
		}

		BagsToSync[PacketID.PickupMode].Clear();

		foreach (var id in BagsToSync[PacketID.SelectedIndex])
		{
			if (!AllBags.ContainsKey(id) || AllBags[id] is not BaseSelectableBag bag) continue;

			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)PacketID.SelectedIndex);
			packet.Write((byte)Main.LocalPlayer.whoAmI);
			packet.Write(id);
			packet.Write(bag.SelectedIndex);
			packet.Send();
		}

		BagsToSync[PacketID.SelectedIndex].Clear();
	}

	internal void HandlePacket(BinaryReader reader, int whoAmI)
	{
		PacketID packetType = (PacketID)reader.ReadByte();

		switch (packetType)
		{
			case PacketID.Inventory:
			{
				byte sender = reader.ReadByte();
				Guid id = reader.ReadGuid();

				// if (!AllBags.ContainsKey(id)) return;

				ItemStorage storage = AllBags[id].GetItemStorage();
				storage.Read(reader);

				if (AllBags[id] is AlchemistBag alchemistBag) alchemistBag.IngredientStorage.Read(reader);

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)PacketID.Inventory);
					packet.Write(sender);
					packet.Write(id);
					storage.Write(packet);
					if (AllBags[id] is AlchemistBag a) a.IngredientStorage.Write(packet);
					packet.Send(-1, sender);
				}

				break;
			}
			case PacketID.PickupMode:
			{
				byte sender = reader.ReadByte();
				Guid id = reader.ReadGuid();
				byte pickupMode = reader.ReadByte();

				// if (!AllBags.ContainsKey(id)) return;

				AllBags[id].PickupMode = (PickupMode)pickupMode;

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)PacketID.PickupMode);
					packet.Write(sender);
					packet.Write(id);
					packet.Write(pickupMode);
					packet.Send(-1, sender);
				}

				break;
			}
			case PacketID.SelectedIndex:
			{
				byte sender = reader.ReadByte();
				Guid id = reader.ReadGuid();
				int selectedIndex = reader.ReadInt32();

				// if (!AllBags.ContainsKey(id)) return;

				((BaseSelectableBag)AllBags[id]).SelectedIndex = selectedIndex;

				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)PacketID.SelectedIndex);
					packet.Write(sender);
					packet.Write(id);
					packet.Write(selectedIndex);
					packet.Send();
				}

				break;
			}
		}
	}
}