using System;
using System.Linq;
using BaseLibrary.Utility;
using ContainerLibrary;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
	private static void ChooseAmmo(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(1), i => i.MatchRet()))
		{
			cursor.Emit(OpCodes.Ldarg, 0); // player
			cursor.Emit(OpCodes.Ldarg, 1); // weapon
			cursor.Emit(OpCodes.Ldloc, 1); // result

			cursor.EmitDelegate<Func<Player, Item, Item, Item>>((player, weapon, result) =>
			{
				if (result != null) return result;

				foreach (BaseAmmoBag bag in player.inventory.OfModItemType<BaseAmmoBag>())
				{
					ItemStorage storage = bag.GetItemStorage();

					result = storage.FirstOrDefault(item => !item.IsAir && ItemLoader.CanChooseAmmo(weapon, item, player));
					if (result != null) break;
				}

				return result;
			});

			cursor.Emit(OpCodes.Stloc, 1);
		}
	}
}