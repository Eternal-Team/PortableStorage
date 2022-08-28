using System;
using BaseLibrary.Utility;
using ContainerLibrary;
using PortableStorage.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Global;

public class PSItem : GlobalItem
{
	public override bool OnPickup(Item item, Player player)
	{
		if (item.IsAir)
			return base.OnPickup(item, player);

		Item temp = item.Clone();

		// note: config options to specify order?

		#region ExistingItems
		bool InsertIntoOfType_Existing<T>(SoundStyle sound, Func<T, ItemStorage> selector = null) where T : BaseBag
		{
			selector ??= bag => bag.GetItemStorage();

			foreach (T bag in player.inventory.OfModItemType<T>())
			{
				if (bag.PickupMode != PickupMode.ExistingOnly) continue;

				ItemStorage storage = selector(bag);
				if (storage.Contains(item.type) && storage.InsertItem(player, ref item))
				{
					BagPopupText.NewText(PopupTextContext.RegularItemPickup, bag.Item, temp, temp.stack - item.stack);
					SoundEngine.PlaySound(sound);
				}

				if (item is null || item.IsAir || !item.active)
					return true;
			}

			return false;
		}

		if (item.IsCurrency)
		{
			if (InsertIntoOfType_Existing<Wallet>(SoundID.CoinPickup))
				return false;
		}

		if (item.ammo > 0)
		{
			if (InsertIntoOfType_Existing<AmmoPouch>(SoundID.Grab))
				return false;
		}

		if (item.bait > 0 || Utility.FishingWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_Existing<FishingBelt>(SoundID.Grab))
				return false;
		}

		if (item.createTile >= TileID.Dirt || item.createWall > WallID.None)
		{
			if (InsertIntoOfType_Existing<BuilderReserve>(SoundID.Grab))
				return false;
		}

		if (Utility.SeedWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_Existing<GardenerSatchel>(SoundID.Grab))
				return false;
		}

		if (Utility.OreWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_Existing<MinersBackpack>(SoundID.Grab))
				return false;
		}

		if (Utility.WiringWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_Existing<WiringBag>(SoundID.Grab))
				return false;
		}

		if (Utility.AlchemistBagWhitelist.Contains(item.type) || (item.DamageType != DamageClass.Summon && ((item.potion && item.healLife > 0) || (item.healMana > 0 && !item.potion) || (item.buffType > 0 && item.buffType != BuffID.Rudolph)) && !ItemID.Sets.NebulaPickup[item.type] && !Utility.IsPetItem(item)))
		{
			if (InsertIntoOfType_Existing<AlchemistBag>(SoundID.Grab, bag => bag.IngredientStorage))
				return false;

			if (InsertIntoOfType_Existing<AlchemistBag>(SoundID.Grab))
				return false;
		}

		if (item.type is not 184 or 1735 or 1668 or 58 or 1734 or 1867 && !ItemID.Sets.NebulaPickup[item.type])
		{
			if (InsertIntoOfType_Existing<BaseNormalBag>(SoundID.Grab))
				return false;
		}
		#endregion

		#region BeforeInventory
		bool InsertIntoOfType_BeforeInventory<T>(SoundStyle sound, Func<T, ItemStorage> selector = null) where T : BaseBag
		{
			selector ??= bag => bag.GetItemStorage();

			foreach (T bag in player.inventory.OfModItemType<T>())
			{
				if (bag.PickupMode != PickupMode.BeforeInventory) continue;

				ItemStorage storage = selector(bag);
				if (storage.InsertItem(player, ref item))
				{
					BagPopupText.NewText(PopupTextContext.RegularItemPickup, bag.Item, temp, temp.stack - item.stack);
					SoundEngine.PlaySound(sound);
				}

				if (item is null || item.IsAir || !item.active)
					return true;
			}

			return false;
		}

		if (item.IsCurrency)
		{
			if (InsertIntoOfType_BeforeInventory<Wallet>(SoundID.CoinPickup))
				return false;
		}

		if (item.ammo > 0)
		{
			if (InsertIntoOfType_BeforeInventory<BaseAmmoBag>(SoundID.Grab))
				return false;
		}

		if (item.bait > 0 || Utility.FishingWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_BeforeInventory<FishingBelt>(SoundID.Grab))
				return false;
		}

		if (Utility.OreWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_BeforeInventory<MinersBackpack>(SoundID.Grab))
				return false;
		}

		if (Utility.WiringWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_BeforeInventory<WiringBag>(SoundID.Grab))
				return false;
		}

		if (Utility.AlchemistBagWhitelist.Contains(item.type) || (item.DamageType != DamageClass.Summon && ((item.potion && item.healLife > 0) || (item.healMana > 0 && !item.potion) || (item.buffType > 0 && item.buffType != BuffID.Rudolph)) && !ItemID.Sets.NebulaPickup[item.type] && !Utility.IsPetItem(item)))
		{
			if (InsertIntoOfType_BeforeInventory<AlchemistBag>(SoundID.Grab))
				return false;

			if (InsertIntoOfType_BeforeInventory<AlchemistBag>(SoundID.Grab, bag => bag.IngredientStorage))
				return false;
		}

		if (item.createTile >= TileID.Dirt || item.createWall > WallID.None)
		{
			if (InsertIntoOfType_BeforeInventory<BuilderReserve>(SoundID.Grab))
				return false;
		}

		if (Utility.SeedWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_BeforeInventory<GardenerSatchel>(SoundID.Grab))
				return false;
		}

		if (item.type is not 184 or 1735 or 1668 or 58 or 1734 or 1867 && !ItemID.Sets.NebulaPickup[item.type])
		{
			if (InsertIntoOfType_BeforeInventory<BaseNormalBag>(SoundID.Grab))
				return false;
		}
		#endregion

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