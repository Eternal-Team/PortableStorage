using BaseLibrary.Utility;
using ContainerLibrary;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
	private static void PlayerOnItemCheck_CheckFishingBobber_PickAndConsumeBait(On.Terraria.Player.orig_ItemCheck_CheckFishingBobber_PickAndConsumeBait orig, Player player, Projectile bobber, out bool pullTheBobber, out int baitTypeUsed)
	{
		pullTheBobber = false;
		baitTypeUsed = 0;
		int foundBaitSlot = -1;

		for (int i = 54; i < 58; i++)
		{
			Item item = player.inventory[i];
			if (item.stack > 0 && item.bait > 0)
			{
				foundBaitSlot = i;
				break;
			}
		}

		if (foundBaitSlot == -1)
		{
			for (int i = 0; i < 50; i++)
			{
				Item item = player.inventory[i];
				if (item.stack > 0 && item.bait > 0)
				{
					foundBaitSlot = i;
					break;
				}
			}
		}

		// found bait in player inventory
		if (foundBaitSlot > -1)
		{
			Item bait = player.inventory[foundBaitSlot];
			bool flag = false;
			float num2 = 1f + bait.bait / 6f;
			if (num2 < 1f)
				num2 = 1f;

			if (player.accTackleBox)
				num2 += 1f;

			if (Main.rand.NextFloat() * num2 < 1f)
				flag = true;

			if (bobber.localAI[1] == -1f)
				flag = true;

			if (bobber.localAI[1] > 0f)
			{
				Item fishedItem = new Item();
				fishedItem.SetDefaults((int)bobber.localAI[1]);
				if (fishedItem.rare < ItemRarityID.White)
					flag = false;
			}

			baitTypeUsed = bait.type;
			if (baitTypeUsed == 2673)
				flag = true;

			if (CombinedHooks.CanConsumeBait(player, bait) ?? flag)
			{
				if (bait.type is 4361 or 4362)
					NPC.LadyBugKilled(player.Center, bait.type == 4362);

				bait.stack--;
				if (bait.stack <= 0)
					bait.SetDefaults();
			}

			pullTheBobber = true;
		}
		else
		{
			foreach (FishingBelt belt in player.inventory.OfModItemType<FishingBelt>())
			{
				ItemStorage storage = belt.GetItemStorage();
				for (int i = 0; i < storage.Count; i++)
				{
					Item bait = storage[i];
					if (bait.IsAir || bait.bait <= 0) continue;

					bool useBait = false;
					float num2 = 1f + bait.bait / 6f;
					if (num2 < 1f)
						num2 = 1f;

					if (player.accTackleBox)
						num2 += 1f;

					if (Main.rand.NextFloat() * num2 < 1f)
						useBait = true;

					if (bobber.localAI[1] == -1f)
						useBait = true;

					if (bobber.localAI[1] > 0f)
					{
						Item fishedItem = new Item();
						fishedItem.SetDefaults((int)bobber.localAI[1]);
						if (fishedItem.rare < ItemRarityID.White)
							useBait = false;
					}

					baitTypeUsed = bait.type;
					if (baitTypeUsed == 2673)
						useBait = true;

					if (CombinedHooks.CanConsumeBait(player, bait) ?? useBait)
					{
						if (bait.type is ItemID.LadyBug or ItemID.GoldLadyBug)
							NPC.LadyBugKilled(player.Center, bait.type == ItemID.GoldLadyBug);

						belt.GetItemStorage().ModifyStackSize(player, i, -1);
					}

					pullTheBobber = true;
				}
			}
		}
	}

	private static void PlayerOnFishing_GetBestFishingPole(On.Terraria.Player.orig_Fishing_GetBestFishingPole orig, Player player, out Item pole)
	{
		pole = player.inventory[player.selectedItem];
		if (pole.fishingPole != 0)
			return;

		for (int i = 0; i < 58; i++)
		{
			if (player.inventory[i].fishingPole > pole.fishingPole)
			{
				pole = player.inventory[i];
			}
		}

		foreach (FishingBelt belt in player.inventory.OfModItemType<FishingBelt>())
		{
			foreach (Item item in belt.GetItemStorage())
			{
				if (item.fishingPole > pole.fishingPole) pole = item;
			}
		}
	}

	private static void PlayerOnFishing_GetBait(On.Terraria.Player.orig_Fishing_GetBait orig, Player player, out Item bait)
	{
		bait = null;

		for (int i = 54; i < 58; i++)
		{
			if (player.inventory[i].stack > 0 && player.inventory[i].bait > 0)
			{
				bait = player.inventory[i];
				return;
			}
		}

		for (int i = 0; i < 50; i++)
		{
			if (player.inventory[i].stack > 0 && player.inventory[i].bait > 0)
			{
				bait = player.inventory[i];
				return;
			}
		}

		foreach (FishingBelt belt in player.inventory.OfModItemType<FishingBelt>())
		{
			foreach (Item item in belt.GetItemStorage())
			{
				if (!item.IsAir && item.bait > 0)
				{
					bait = item;
					return;
				}
			}
		}
	}
}