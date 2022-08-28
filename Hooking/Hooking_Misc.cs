using System;
using System.Linq;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
	private static void PlayerOnGrabItems(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		ILLabel label = cursor.DefineLabel();

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCall<Player>("PickupItem"), i => i.MatchStloc(1), i => i.MatchBr(out _)))
		{
			cursor.Index += 2;

			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.Emit(OpCodes.Ldloc, 1);

			cursor.EmitDelegate<Func<Player, Item, bool>>((player, item) =>
			{
				Item temp = item.Clone();

				bool InsertIntoOfType_AfterInventory<T>(SoundStyle sound) where T : BaseBag
				{
					foreach (T bag in player.inventory.OfModItemType<T>())
					{
						if (bag.PickupMode != PickupMode.AfterInventory) continue;

						ItemStorage storage = bag.GetItemStorage();
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
					if (InsertIntoOfType_AfterInventory<Wallet>(SoundID.CoinPickup))
						return false;
				}

				if (item.ammo > 0)
				{
					if (InsertIntoOfType_AfterInventory<AmmoPouch>(SoundID.Grab))
						return false;
				}

				if (item.bait > 0 || Utility.FishingWhitelist.Contains(item.type))
				{
					if (InsertIntoOfType_AfterInventory<FishingBelt>(SoundID.Grab))
						return false;
				}

				if (Utility.OreWhitelist.Contains(item.type))
				{
					if (InsertIntoOfType_AfterInventory<MinersBackpack>(SoundID.Grab))
						return false;
				}

				// todo: first it should try to put stuff into ingredients (assuming it already has that ingredient), e.g. health potions
				if (Utility.AlchemistBagWhitelist.Contains(item.type) || (item.DamageType != DamageClass.Summon && ((item.potion && item.healLife > 0) || (item.healMana > 0 && !item.potion) || (item.buffType > 0 && item.buffType != BuffID.Rudolph)) && !ItemID.Sets.NebulaPickup[item.type] && !Utility.IsPetItem(item)))
				{
					if (InsertIntoOfType_AfterInventory<AlchemistBag>(SoundID.Grab))
						return false;
				}

				if (item.type is not 184 or 1735 or 1668 or 58 or 1734 or 1867 && !ItemID.Sets.NebulaPickup[item.type])
				{
					if (InsertIntoOfType_AfterInventory<BaseNormalBag>(SoundID.Grab))
						return false;
				}

				return true;
			});

			cursor.Emit(OpCodes.Brfalse, label);
		}

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(0), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(0)))
		{
			cursor.MarkLabel(label);
		}
	}

	private static void SpawnTownNPCs_Del(Player player, ref int coins, ref bool condArmsDealer, ref bool condDemolitionist, ref bool condDyeTrader)
	{
		long walletCoins = 0;

		foreach (Item pItem in player.inventory)
		{
			if (pItem.ModItem is BaseBag bag)
			{
				if (bag is Wallet wallet)
				{
					walletCoins += wallet.GetItemStorage().CountCoins();
					continue;
				}

				ItemStorage storage = bag.GetItemStorage();
				foreach (Item item in storage)
				{
					if (item.IsAir) continue;

					if (item.ammo == AmmoID.Bullet || item.useAmmo == AmmoID.Bullet) condArmsDealer = true;
					if (ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[item.type]) condDemolitionist = true;
					if (item.dye > 0 || item.type is >= ItemID.TealMushroom and <= ItemID.DyeVat or >= ItemID.StrangePlant1 and <= ItemID.StrangePlant4) condDyeTrader = true;
				}
			}
		}

		coins = (int)Utils.Clamp(walletCoins + coins, 0, int.MaxValue);
	}

	private static void SpawnTownNPCs(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdsfld<Main>("player"), i => i.MatchLdloc(85), i => i.MatchLdelemRef()))
		{
			cursor.Emit<Main>(OpCodes.Ldsfld, "player");
			cursor.Emit(OpCodes.Ldloc, 85);
			cursor.Emit(OpCodes.Ldelem_Ref);

			cursor.Emit(OpCodes.Ldloca, 33);
			cursor.Emit(OpCodes.Ldloca, 36);
			cursor.Emit(OpCodes.Ldloca, 37);
			cursor.Emit(OpCodes.Ldloca, 38);

			cursor.EmitDelegate(SpawnTownNPCs_Del);
		}
	}

	private static void ItemSlotText(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(i => i.MatchLdloc(1), i => i.MatchLdfld<Item>("fishingPole"), i => i.MatchLdcI4(0)))
		{
			cursor.Emit(OpCodes.Ldloc, 1);
			cursor.Emit(OpCodes.Ldloc, 35);

			cursor.EmitDelegate<Func<Item, int, int>>((weapon, ammoCount) =>
			{
				foreach (BaseAmmoBag bag in Main.LocalPlayer.inventory.OfModItemType<BaseAmmoBag>())
				{
					ItemStorage storage = bag.GetItemStorage();

					ammoCount += storage.Where(item => !item.IsAir && ItemLoader.CanChooseAmmo(weapon, item, Main.LocalPlayer)).Sum(item => item.stack);
				}

				return ammoCount;
			});

			cursor.Emit(OpCodes.Stloc, 35);
		}

		if (cursor.TryGotoNext(i => i.MatchLdloc(1), i => i.MatchLdfld<Item>("tileWand"), i => i.MatchLdcI4(0)))
		{
			cursor.Emit(OpCodes.Ldloc, 35);

			cursor.EmitDelegate<Func<int, int>>(bait =>
			{
				foreach (FishingBelt bag in Main.LocalPlayer.inventory.OfModItemType<FishingBelt>())
				{
					ItemStorage storage = bag.GetItemStorage();

					bait += storage.Where(item => !item.IsAir && item.bait > 0).Sum(item => item.stack);
				}

				return bait;
			});

			cursor.Emit(OpCodes.Stloc, 35);
		}
	}

	private static void MainOnDrawInterface_40_InteractItemIcon(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCallvirt<Main>("LoadItem")))
			throw new Exception("IL edit failed");

		cursor.Index++;
		cursor.Emit(OpCodes.Ldloca, 6);
		cursor.EmitDelegate((ref float scale) =>
		{
			Item hotbarItem = Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem];

			if (hotbarItem.IsAir)
				return;

			Item selectedItem = hotbarItem.ModItem switch
			{
				BuilderReserve reserve => reserve.SelectedItem,
				GardenerSatchel satchel => satchel.SelectedItem,
				_ => null
			};

			if (selectedItem is null || selectedItem.IsAir)
				return;

			scale = 1f;
			Main.instance.LoadItem(selectedItem.type);
			Vector2 size = Item.GetDrawHitbox(hotbarItem.type, null).Size();
			Vector2 position = new Vector2(Main.mouseX + 10, Main.mouseY + 10);

			DrawingUtility.DrawItemInInventory(Main.spriteBatch, selectedItem, position + size * 0.5f, size * 0.5f, 1f, false);

			string text = selectedItem.stack < 1000 ? selectedItem.stack.ToString() : TextUtility.ToSI(selectedItem.stack, "N1");
			Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text);

			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, position + new Vector2(size.X * 0.5f, size.Y), Color.White, 0f, new Vector2(textSize.X * 0.5f, 0f), new Vector2(0.75f));
		});
	}
}