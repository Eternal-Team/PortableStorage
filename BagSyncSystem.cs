using System;
using System.Collections.Generic;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using PortableStorage.Items;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PortableStorage;

public enum PacketID : byte
{
	SyncBag
}

public class BagSyncSystem : ModSystem
{
	private List<BaseBag> Bags = new();

	public Dictionary<Guid, BaseBag> AllBags = new();

	public void Register(BaseBag item)
	{
		if (!Bags.Contains(item))
			Bags.Add(item);
	}

	public override void PostUpdateItems()
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			Main.NewText($"[CLIENT {Main.LocalPlayer.whoAmI}] Tracking {AllBags.Count} bags");
		else
			ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral($"[SERVER] Tracking {AllBags.Count} bags"), Color.White);

		if (Main.netMode != NetmodeID.MultiplayerClient)
			return;

		foreach (var bag in Bags)
		{
			if (!AllBags.ContainsKey(bag.ID)) continue;
		
			Player player = Main.LocalPlayer;
		
			ModPacket packet = Mod.GetPacket();
			packet.Write((byte)PacketID.SyncBag);
			packet.Write((byte)player.whoAmI);
			packet.Write(bag.ID);
			var itemStorage = AllBags[bag.ID].GetItemStorage();
			itemStorage.Write(packet);
			packet.Send();
		}

		Bags.Clear();
	}
}