using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader.Container;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static void Player_HasAmmo(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg(2), i => i.MatchStloc(4), i => i.MatchBr(out _)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldarg, 1);
				cursor.Emit(OpCodes.Ldarg, 2);

				cursor.EmitDelegate<Func<Player, Item, bool,bool>>((player, weapon, hasAmmo) =>
				{
					foreach (Item pItem in player.inventory)
					{
						if (!pItem.IsAir && pItem.modItem is BaseAmmoBag bag)
						{
							ItemHandler handler = bag.GetItemHandler();

							for (int i = 0; i < handler.Slots; i++)
							{
								Item item = handler.GetItemInSlot(i);
								if (!item.IsAir && item.ammo == weapon.useAmmo) return true;
							}
						}
					}

					return hasAmmo;
				});

				cursor.Emit(OpCodes.Starg, 2);
			}
		}

		private delegate bool PickAmmo_Del(Player player, Item weapon, ref Item ammoItem);

		private static void Player_PickAmmo(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);
			ILLabel label = il.DefineLabel();
			
			// il.Body.Variables.Add(new VariableDefinition(il.Import(typeof(int))));
			// int index = il.Body.Variables.Count - 1;

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg(4), i => i.MatchLdindU1(), i => i.MatchLdcI4(0)))
			{
				cursor.Emit(OpCodes.Ldarg, 0); // player
				cursor.Emit(OpCodes.Ldarg, 1); // sItem
				cursor.Emit(OpCodes.Ldloca, 0); // item

				// vanilla ammo slots > bags > inventory

				cursor.EmitDelegate<PickAmmo_Del>(PickAmmo);

				cursor.Emit(OpCodes.Brfalse, label);
				
				cursor.Emit(OpCodes.Ldarg, 4);
				cursor.Emit(OpCodes.Ldc_I4, 1);
				cursor.Emit(OpCodes.Stind_I1);

				cursor.MarkLabel(label);
			}

			// if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(-1), i => i.MatchStloc(2))) cursor.MarkLabel(label);

			bool PickAmmo(Player player, Item weapon, ref Item ammoItem)
			{
				if (!ammoItem.IsAir) return false;

				foreach (Item pItem in player.inventory)
				{
					if (!pItem.IsAir && pItem.modItem is BaseAmmoBag bag)
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
				}

				return false;
			}
		}
	}
}