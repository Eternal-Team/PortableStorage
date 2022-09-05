using System;
using BaseLibrary.Utility;
using ContainerLibrary;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
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
			int inInventory = Math.Min(player.CountItem(ItemID.Wire), usedWires);
			usedWires -= inInventory;

			for (int j = 0; j < inInventory; j++)
			{
				player.ConsumeItem(ItemID.Wire);
			}

			inInventory = Math.Min(player.CountItem(ItemID.Actuator), usedActuators);
			usedActuators -= inInventory;
			for (int k = 0; k < inInventory; k++)
			{
				player.ConsumeItem(ItemID.Actuator);
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