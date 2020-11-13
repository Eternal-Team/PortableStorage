using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader.Container;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static IEnumerable<BaseAmmoBag> GetAmmoBags(Player player)
		{
			foreach (Item pItem in Main.LocalPlayer.inventory)
			{
				if (!pItem.IsAir && pItem.modItem is BaseAmmoBag bag)
				{
					yield return bag;
				}
			}
		}

		private static void HasAmmo(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg(2), i => i.MatchStloc(4), i => i.MatchBr(out _)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldarg, 1);
				cursor.Emit(OpCodes.Ldarg, 2);

				cursor.EmitDelegate<Func<Player, Item, bool, bool>>((player, weapon, hasAmmo) =>
				{
					foreach (BaseAmmoBag bag in GetAmmoBags(player))
					{
						ItemHandler handler = bag.GetItemHandler();

						for (int i = 0; i < handler.Slots; i++)
						{
							Item item = handler.GetItemInSlot(i);
							if (!item.IsAir && item.ammo == weapon.useAmmo) return true;
						}
					}

					return hasAmmo;
				});

				cursor.Emit(OpCodes.Starg, 2);
			}
		}

		private delegate bool PickAmmo_Del(Player player, Item weapon, ref Item ammoItem);

		private static void PickAmmo(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = il.DefineLabel();

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg(4), i => i.MatchLdindU1(), i => i.MatchLdcI4(0)))
			{
				cursor.Emit(OpCodes.Ldarg, 0); // player
				cursor.Emit(OpCodes.Ldarg, 1); // sItem
				cursor.Emit(OpCodes.Ldloca, 0); // item

				cursor.EmitDelegate<PickAmmo_Del>(delegate(Player player, Item weapon, ref Item ammoItem)
				{
					if (!ammoItem.IsAir) return false;

					foreach (BaseAmmoBag bag in GetAmmoBags(player))
					{
						ItemHandler handler = bag.GetItemHandler();

						for (int i = 0; i < handler.Slots; i++)
						{
							Item item = handler.GetItemInSlot(i);
							if (!item.IsAir && item.ammo == weapon.useAmmo)
							{
								ammoItem = item;
								return true;
							}
						}
					}

					return false;
				});

				cursor.Emit(OpCodes.Brfalse, label);

				cursor.Emit(OpCodes.Ldarg, 4);
				cursor.Emit(OpCodes.Ldc_I4, 1);
				cursor.Emit(OpCodes.Stind_I1);

				cursor.MarkLabel(label);
			}
		}

		private static void DrawAmmo(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdloc(1), i => i.MatchLdfld<Item>("fishingPole"), i => i.MatchLdcI4(0)))
			{
				cursor.Emit(OpCodes.Ldloc, 89);
				cursor.Emit(OpCodes.Ldloc, 76);

				cursor.EmitDelegate<Func<int, int, int>>((useAmmo, ammoCount) =>
				{
					foreach (BaseAmmoBag bag in GetAmmoBags(Main.LocalPlayer))
					{
						ItemHandler handler = bag.GetItemHandler();

						for (int i = 0; i < handler.Slots; i++)
						{
							Item item = handler.GetItemInSlot(i);
							if (!item.IsAir && item.ammo == useAmmo) ammoCount += item.stack;
						}
					}

					return ammoCount;
				});

				cursor.Emit(OpCodes.Stloc, 76);
			}
		}
	}
}