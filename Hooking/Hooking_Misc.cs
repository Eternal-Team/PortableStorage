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
	private static bool PlayerOnGrabItems_Del(Player player, ref Item item)
	{
		Item temp = item.Clone();

		bool InsertIntoOfType_AfterInventory<T>(ref Item item, SoundStyle sound, Func<T, ItemStorage> selector = null) where T : BaseBag
		{
			selector ??= bag => bag.GetItemStorage();

			foreach (T bag in player.inventory.OfModItemType<T>())
			{
				if (bag.PickupMode != PickupMode.AfterInventory) continue;

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
			if (InsertIntoOfType_AfterInventory<Wallet>(ref item, SoundID.CoinPickup))
				return false;
		}

		if (item.ammo > 0)
		{
			if (InsertIntoOfType_AfterInventory<AmmoPouch>(ref item, SoundID.Grab))
				return false;
		}

		if (item.bait > 0 || Utility.FishingWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_AfterInventory<FishingBelt>(ref item, SoundID.Grab))
				return false;
		}

		if (Utility.OreWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_AfterInventory<MinersBackpack>(ref item, SoundID.Grab))
				return false;
		}

		if (Utility.WiringWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_AfterInventory<WiringBag>(ref item, SoundID.Grab))
				return false;
		}

		if (Utility.AlchemistBagWhitelist.Contains(item.type) || (item.DamageType != DamageClass.Summon && ((item.potion && item.healLife > 0) || (item.healMana > 0 && !item.potion) || (item.buffType > 0 && item.buffType != BuffID.Rudolph)) && !ItemID.Sets.NebulaPickup[item.type] && !Utility.IsPetItem(item)))
		{
			if (InsertIntoOfType_AfterInventory<AlchemistBag>(ref item, SoundID.Grab))
				return false;

			if (InsertIntoOfType_AfterInventory<AlchemistBag>(ref item, SoundID.Grab, bag => bag.IngredientStorage))
				return false;
		}

		if (item.createTile >= TileID.Dirt || item.createWall > WallID.None)
		{
			if (InsertIntoOfType_AfterInventory<BuilderReserve>(ref item, SoundID.Grab))
				return false;
		}

		if (Utility.SeedWhitelist.Contains(item.type))
		{
			if (InsertIntoOfType_AfterInventory<GardenerSatchel>(ref item, SoundID.Grab))
				return false;
		}

		if (item.type is not 184 or 1735 or 1668 or 58 or 1734 or 1867 && !ItemID.Sets.NebulaPickup[item.type])
		{
			if (InsertIntoOfType_AfterInventory<BaseNormalBag>(ref item, SoundID.Grab))
				return false;
		}

		return true;
	}


	private static void PlayerOnGrabItems(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		ILLabel label = cursor.DefineLabel();

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCall<Player>("PickupItem"), i => i.MatchStloc(1), i => i.MatchBr(out _)))
		{
			cursor.Index += 2;

			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.Emit(OpCodes.Ldloca, 1);

			cursor.EmitDelegate(PlayerOnGrabItems_Del);

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

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(1), i => i.MatchLdfld<Item>("type"), i => i.MatchLdcI4(3611), i => i.MatchBneUn(out _)))
			throw new Exception("IL edit failed");

		cursor.Index += 6;
		cursor.Emit(OpCodes.Ldloc, 35);

		cursor.EmitDelegate<Func<int, int>>(wires =>
		{
			foreach (WiringBag bag in Main.LocalPlayer.inventory.OfModItemType<WiringBag>())
			{
				ItemStorage storage = bag.GetItemStorage();

				wires += storage.Where(item => !item.IsAir && item.type == ItemID.Wire).Sum(item => item.stack);
			}

			return wires;
		});
		cursor.Emit(OpCodes.Stloc, 35);
	}

	private static void MainOnDrawInterface_40_InteractItemIcon(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchCallvirt<Main>("LoadItem")))
			throw new Exception("IL edit failed");

		cursor.Index++;
		cursor.Emit(OpCodes.Ldloc, 6);
		cursor.EmitDelegate<Func<float, float>>(scale =>
		{
			Item hotbarItem = Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem];

			if (hotbarItem.IsAir)
				return scale;

			Item selectedItem = hotbarItem.ModItem switch
			{
				BuilderReserve reserve => reserve.SelectedItem,
				GardenerSatchel satchel => satchel.SelectedItem,
				_ => null
			};

			if (selectedItem is null || selectedItem.IsAir)
				return scale;

			scale = 1f;
			Main.instance.LoadItem(selectedItem.type);
			Vector2 size = Item.GetDrawHitbox(hotbarItem.type, null).Size();
			Vector2 position = new Vector2(Main.mouseX + 10, Main.mouseY + 10);

			DrawingUtility.DrawItemInInventory(Main.spriteBatch, selectedItem, position + size * 0.5f, size * 0.5f, 1f, false);

			string text = selectedItem.stack < 1000 ? selectedItem.stack.ToString() : TextUtility.ToSI(selectedItem.stack, "N1");
			Vector2 textSize = FontAssets.MouseText.Value.MeasureString(text);

			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, position + new Vector2(size.X * 0.5f, size.Y), Color.White, 0f, new Vector2(textSize.X * 0.5f, 0f), new Vector2(0.75f));

			return scale;
		});
		cursor.Emit(OpCodes.Stloc, 6);
	}

	private static void PlayerOnItemCheck_UseWiringTools(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		ILLabel label = cursor.DefineLabel();

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg(1), i => i.MatchLdfld<Item>("type"), i => i.MatchLdcI4(509), i => i.MatchBneUn(out _)))
			throw new Exception("IL edit failed");

		cursor.Emit(OpCodes.Br, label);

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg(1), i => i.MatchLdfld<Item>("type"), i => i.MatchLdcI4(510), i => i.MatchBneUn(out _)))
			throw new Exception("IL edit failed");

		cursor.MarkLabel(label);
		cursor.Emit(OpCodes.Ldarg, 0);
		cursor.Emit(OpCodes.Ldarg, 1);
		cursor.EmitDelegate((Player player, Item item) =>
		{
			int tileTargetX = Player.tileTargetX;
			int tileTargetY = Player.tileTargetY;

			// note: i feel like this could be a good place for ItemStorage wrapper of player inventory
			void SelectItemAndBag(ref int index, ref WiringBag bag)
			{
				for (int i = 0; i < 58; i++)
				{
					if (!player.inventory[i].IsAir && player.inventory[i].type == ItemID.Wire)
					{
						index = i;
						return;
					}
				}

				foreach (WiringBag wiringBag in player.inventory.OfModItemType<WiringBag>())
				{
					var items = wiringBag.GetItemStorage();
					for (int i = 0; i < items.Count; i++)
					{
						Item item = items[i];
						if (!item.IsAir && item.type == ItemID.Wire)
						{
							index = i;
							bag = wiringBag;
							return;
						}
					}
				}
			}

			if (item.type == ItemID.Wrench)
			{
				int index = -1;
				WiringBag bag = null;
				SelectItemAndBag(ref index, ref bag);

				if (index >= 0 && WorldGen.PlaceWire(tileTargetX, tileTargetY))
				{
					Item wire = bag is not null ? bag.GetItemStorage()[index] : player.inventory[index];

					if (ItemLoader.ConsumeItem(wire, player))
					{
						if (bag is not null) bag.GetItemStorage().ModifyStackSize(player, index, -1);
						else
						{
							wire.stack--;
							if (wire.stack <= 0) wire.SetDefaults();
						}
					}

					player.ApplyItemTime(item);
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 5, tileTargetX, tileTargetY);
				}
			}
			else if (item.type == ItemID.BlueWrench)
			{
				int index = -1;
				WiringBag bag = null;
				SelectItemAndBag(ref index, ref bag);

				if (index >= 0 && WorldGen.PlaceWire2(tileTargetX, tileTargetY))
				{
					Item wire = bag is not null ? bag.GetItemStorage()[index] : player.inventory[index];

					if (ItemLoader.ConsumeItem(wire, player))
					{
						if (bag is not null) bag.GetItemStorage().ModifyStackSize(player, index, -1);
						else
						{
							wire.stack--;
							if (wire.stack <= 0) wire.SetDefaults();
						}
					}

					player.ApplyItemTime(item);
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 10, tileTargetX, tileTargetY);
				}
			}
			else if (item.type == ItemID.GreenWrench)
			{
				int index = -1;
				WiringBag bag = null;
				SelectItemAndBag(ref index, ref bag);

				if (index >= 0 && WorldGen.PlaceWire3(tileTargetX, tileTargetY))
				{
					Item wire = bag is not null ? bag.GetItemStorage()[index] : player.inventory[index];

					if (ItemLoader.ConsumeItem(wire, player))
					{
						if (bag is not null) bag.GetItemStorage().ModifyStackSize(player, index, -1);
						else
						{
							wire.stack--;
							if (wire.stack <= 0) wire.SetDefaults();
						}
					}

					player.ApplyItemTime(item);
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 12, tileTargetX, tileTargetY);
				}
			}
			else if (item.type == ItemID.YellowWrench)
			{
				int index = -1;
				WiringBag bag = null;
				SelectItemAndBag(ref index, ref bag);

				if (index >= 0 && WorldGen.PlaceWire4(tileTargetX, tileTargetY))
				{
					Item wire = bag is not null ? bag.GetItemStorage()[index] : player.inventory[index];

					if (ItemLoader.ConsumeItem(wire, player))
					{
						if (bag is not null) bag.GetItemStorage().ModifyStackSize(player, index, -1);
						else
						{
							wire.stack--;
							if (wire.stack <= 0) wire.SetDefaults();
						}
					}

					player.ApplyItemTime(item);
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 16, tileTargetX, tileTargetY);
				}
			}
		});
	}

	private static void WiringOnMassWireOperation(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(0), i => i.MatchLdloc(1), i => i.MatchStloc(2)))
			throw new Exception("IL edit failed");

		cursor.Emit(OpCodes.Ldarg, 2);
		cursor.Emit(OpCodes.Ldloca, 0);
		cursor.Emit(OpCodes.Ldloca, 1);

		cursor.EmitDelegate((Player player, ref int wireCount, ref int actuatorCount) =>
		{
			foreach (WiringBag wiringBag in player.inventory.OfModItemType<WiringBag>())
			{
				foreach (Item item in wiringBag.GetItemStorage())
				{
					if (item.IsAir) continue;

					if (item.type == ItemID.Wire) wireCount += item.stack;
					else if (item.type == ItemID.Actuator) actuatorCount += item.stack;
				}
			}
		});

		if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(0), i => i.MatchStloc(6), i => i.MatchBr(out _)))
			throw new Exception("IL edit failed");

		cursor.Emit(OpCodes.Ldarg, 2);
		cursor.Emit(OpCodes.Ldloc, 3);
		cursor.Emit(OpCodes.Ldloc, 4);

		cursor.EmitDelegate((Player player, int usedWires, int usedActuators) =>
		{
			int inInventory = Math.Min(player.CountItem(530), usedWires);
			usedWires -= inInventory;

			for (int j = 0; j < inInventory; j++)
			{
				player.ConsumeItem(530);
			}

			inInventory = Math.Min(player.CountItem(849), usedActuators);
			usedActuators -= inInventory;
			for (int k = 0; k < inInventory; k++)
			{
				player.ConsumeItem(849);
			}

			foreach (WiringBag bag in player.inventory.OfModItemType<WiringBag>())
			{
				ItemStorage storage = bag.GetItemStorage();

				for (int i = 0; i < storage.Count; i++)
				{
					Item storageItem = storage[i];
					if (storageItem.IsAir) continue;

					if (usedWires > 0 && storageItem.type == ItemID.Wire && storage.RemoveItem(player, i, out Item item, usedWires))
					{
						usedWires -= item.stack;
					}

					if (usedActuators > 0 && storageItem.type == ItemID.Actuator && storage.RemoveItem(player, i, out item, usedActuators))
					{
						usedActuators -= item.stack;
					}
				}
			}
		});

		cursor.Emit(OpCodes.Ret);
	}
}