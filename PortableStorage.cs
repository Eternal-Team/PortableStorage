using System;
using System.Collections.Generic;
using System.IO;
using BaseLibrary.Utility;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage;

// note: energy addons

public enum PacketTypes : byte
{
	SyncBagContents
}

// public class BagChangeSystem : ModSystem
// {
// 	public static Dictionary<Guid, BaseBag> Bags = new();
//
// 	private Dictionary<BaseBag, HashSet<int>> data = new();
//
// 	public void RegisterItem(BaseBag item, int slot)
// 	{
// 		if (!data.ContainsKey(item)) data.Add(item, new HashSet<int>());
// 		data[item].Add(slot);
// 	}
//
// 	public override void PostUpdateItems()
// 	{
// 		// if (!Main.dedServ) Main.NewText($"Client: currently tracking {Bags.Count} bags");
// 		// else
// 		// {
// 		// 	NetworkText text = NetworkText.FromFormattable($"Server: currently tracking {Bags.Count} bags");
// 		// 	ChatHelper.BroadcastChatMessage(text, Color.White);
// 		// }
//
// 		if (data.Count >= 0 && Main.netMode != NetmodeID.Server) Recipe.FindRecipes();
//
// 		if (Main.netMode == NetmodeID.MultiplayerClient)
// 		{
// 			foreach (var (key, value) in data)
// 			{
// 				ModPacket packet = Mod.GetPacket();
// 				packet.Write((byte)PacketTypes.SyncBagContents);
// 				packet.Write(Main.LocalPlayer.whoAmI);
// 				packet.Write(key.ID);
// 				packet.Write(value.Count);
// 				foreach (int slot in value)
// 				{
// 					packet.Write(slot);
// 					key.GetItemStorage().WriteSlot(packet, slot);
// 				}
//
// 				packet.Send(ignoreClient: Main.LocalPlayer.whoAmI);
// 			}
// 		}
//
// 		data.Clear();
// 	}
// }

public class PortableStorage : Mod
{
	public const string AssetPath = "PortableStorage/Assets/";

	public static PortableStorage Instance => ModContent.GetInstance<PortableStorage>();

	public override void Load()
	{
		Hooking.Hooking.Load();
		BagPopupText.Load();
	}

	public override void PostSetupContent()
	{
		Utility.PostSetupContent();
	}

	public override void AddRecipeGroups()
	{
		Utility.AddRecipeGroups();
	}

	// public override void HandlePacket(BinaryReader reader, int whoAmI)
	// {
	// 	PacketTypes type = (PacketTypes)reader.ReadByte();
	// 	if (type == PacketTypes.SyncBagContents)
	// 	{
	// 		int ignoreClient = -1;
	// 		if (Main.netMode == NetmodeID.Server) ignoreClient = reader.ReadInt32();
	// 		Guid id = reader.ReadGuid();
	// 		HashSet<int> slots = new();
	//
	// 		int count = reader.ReadInt32();
	// 		for (int i = 0; i < count; i++)
	// 		{
	// 			int slot = reader.ReadInt32();
	// 			BagChangeSystem.Bags[id].GetItemStorage().ReadSlot(reader, slot);
	// 			slots.Add(slot);
	// 		}
	//
	// 		if (Main.netMode == NetmodeID.Server)
	// 		{
	// 			var packet = GetPacket();
	// 			packet.Write((byte)PacketTypes.SyncBagContents);
	// 			packet.Write(id);
	// 			packet.Write(slots.Count);
	// 			foreach (int slot in slots)
	// 			{
	// 				packet.Write(slot);
	// 				BagChangeSystem.Bags[id].GetItemStorage().WriteSlot(packet, slot);
	// 			}
	//
	// 			packet.Send(-1, ignoreClient);
	// 		}
	// 	}
	// }
}