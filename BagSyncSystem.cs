using System;
using System.Collections.Generic;
using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage;

// note: one way to track what storages dont exist is to hook into TurnToAir and SetDefaults (type=0)

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
		Main.NewText($"Tracking {AllBags.Count} bags");
		
		if (Main.netMode != NetmodeID.MultiplayerClient)
			return;

		foreach (BaseBag bag in Bags)
		{
			Player player = Main.LocalPlayer;

			int slot = player.inventory.ToList().FindIndex(x => (x.ModItem as BaseBag)?.ID == bag.ID);
			if (slot < 0) continue;

			NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot);
		}

		Bags.Clear();
	}
}