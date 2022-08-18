using System.Collections.Generic;
using PortableStorage.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Global;

public class PSItem : GlobalItem
{
	private static IEnumerable<T> OfModItemType<T>(IEnumerable<Item> source)
	{
		foreach (Item obj in source)
		{
			if (!obj.IsAir && obj.ModItem is T result)
			{
				yield return result;
			}
		}
	}

	public override bool OnPickup(Item item, Player player)
	{
		if (item.IsAir)
			return base.OnPickup(item, player);

		Item temp = item.Clone();

		if (item.IsCurrency)
		{
			foreach (Wallet wallet in OfModItemType<Wallet>(player.inventory))
			{
				if (!wallet.EnablePickup) continue;

				if (wallet.GetItemStorage().InsertItem(player, ref item))
				{
					BagPopupText.NewText(PopupTextContext.RegularItemPickup, wallet.Item, temp, temp.stack - item.stack);
					SoundEngine.PlaySound(SoundID.CoinPickup);
				}

				if (item is null || item.IsAir || !item.active)
					return false;
			}
		}

		if (item.ammo > 0)
		{
			foreach (BaseAmmoBag ammoBag in OfModItemType<BaseAmmoBag>(player.inventory))
			{
				if (!ammoBag.EnablePickup) continue;

				if (ammoBag.GetItemStorage().InsertItem(player, ref item))
				{
					BagPopupText.NewText(PopupTextContext.RegularItemPickup, ammoBag.Item, temp, temp.stack - item.stack);
					SoundEngine.PlaySound(SoundID.Grab);
				}

				if (item is null || item.IsAir || !item.active)
					return false;
			}
		}

		if (item.bait > 0 || Utility.FishingWhitelist.Contains(item.type))
		{
			foreach (FishingBelt fishingBelt in OfModItemType<FishingBelt>(player.inventory))
			{
				if (!fishingBelt.EnablePickup) continue;

				if (fishingBelt.GetItemStorage().InsertItem(player, ref item))
				{
					BagPopupText.NewText(PopupTextContext.RegularItemPickup, fishingBelt.Item, temp, temp.stack - item.stack);
					SoundEngine.PlaySound(SoundID.Grab);
				}

				if (item is null || item.IsAir || !item.active)
					return false;
			}
		}

		if (Utility.OreWhitelist.Contains(item.type))
		{
			foreach (MinersBackpack minersBackpack in OfModItemType<MinersBackpack>(player.inventory))
			{
				if (!minersBackpack.EnablePickup) continue;

				if (minersBackpack.GetItemStorage().InsertItem(player, ref item))
				{
					BagPopupText.NewText(PopupTextContext.RegularItemPickup, minersBackpack.Item, temp, temp.stack - item.stack);
					SoundEngine.PlaySound(SoundID.Grab);
				}

				if (item is null || item.IsAir || !item.active)
					return false;
			}
		}

		// first it should try to put stuff into ingredients (assuming it already has that ingredient), e.g. health potions
		if (Utility.AlchemistBagWhitelist.Contains(item.type) || (item.DamageType != DamageClass.Summon &&
		                                                          ((item.potion && item.healLife > 0) ||
		                                                           (item.healMana > 0 && !item.potion) ||
		                                                           (item.buffType > 0 && item.buffType != BuffID.Rudolph)) && !ItemID.Sets.NebulaPickup[item.type] && !Utility.IsPetItem(item)))
		{
			foreach (AlchemistBag alchemistBag in OfModItemType<AlchemistBag>(player.inventory))
			{
				if (!alchemistBag.EnablePickup) continue;

				if (alchemistBag.GetItemStorage().InsertItem(player, ref item))
				{
					BagPopupText.NewText(PopupTextContext.RegularItemPickup, alchemistBag.Item, temp, temp.stack - item.stack);
					SoundEngine.PlaySound(SoundID.Grab);
				}

				if (item is null || item.IsAir || !item.active)
					return false;
			}
		}

		return base.OnPickup(item, player);
	}

	public override void OpenVanillaBag(string context, Player player, int arg)
	{
		if (context == "crate")
		{
			if ((arg == ItemID.IronCrate && Main.rand.NextBool(20)) || (arg == ItemID.GoldenCrate && Main.rand.NextBool(10)))
				player.QuickSpawnItem(new EntitySource_ItemOpen(player, arg, context), ModContent.ItemType<FishingBelt>());
		}
	}
}