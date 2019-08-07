using BaseLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items.Ammo;
using PortableStorage.Items.Special;
using System;
using System.Linq;
using Terraria;

namespace PortableStorage
{
	public static partial class Hooking
	{
		private static void ItemSlot_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdloc(36), i => i.MatchLdcI4(58), i => i.MatchBlt(out _)))
			{
				cursor.Index += 3;

				cursor.Emit(OpCodes.Ldloc, 35);
				cursor.Emit(OpCodes.Ldloc, 33);

				cursor.EmitDelegate<Func<int, int, int>>((useAmmo, ammoCount) => ammoCount + Main.LocalPlayer.inventory.OfType<BaseAmmoBag>().SelectMany(ammoBag => ammoBag.Handler.Items).Where(item => item.ammo == useAmmo).Sum(item => item.stack));

				cursor.Emit(OpCodes.Stloc, 33);
			}

			if (cursor.TryGotoNext(i => i.MatchLdloc(37), i => i.MatchLdcI4(58), i => i.MatchBlt(out _)))
			{
				cursor.Index += 3;

				cursor.Emit(OpCodes.Ldloc, 33);

				cursor.EmitDelegate<Func<int, int>>(baitCount => baitCount + Main.LocalPlayer.inventory.OfType<FishingBelt>().SelectMany(ammoBag => ammoBag.Handler.Items).Where(item => item.bait > 0).Sum(item => item.stack));

				cursor.Emit(OpCodes.Stloc, 33);
			}
		}

		private static void ItemSlot_DrawSavings(ILContext il)
		{
			int walletIndex = il.AddVariable(typeof(long));

			ILCursor cursor = new ILCursor(il);
			ILLabel label = cursor.DefineLabel();

			if (cursor.TryGotoNext(i => i.MatchStloc(4), i => i.MatchLdloca(1), i => i.MatchLdcI4(3)))
			{
				cursor.Index++;

				cursor.Emit(OpCodes.Ldloc, 0);
				cursor.EmitDelegate<Func<Player, long>>(player => player.inventory.OfType<Wallet>().Sum(wallet => wallet.Coins));
				cursor.Emit(OpCodes.Stloc, walletIndex);

				cursor.Index++;

				cursor.Remove();
				cursor.Emit(OpCodes.Ldc_I4, 4);
			}

			if (cursor.TryGotoNext(i => i.MatchLdloc(4), i => i.MatchStelemI8()))
			{
				cursor.Index += 2;

				cursor.Emit(OpCodes.Dup);
				cursor.Emit(OpCodes.Ldc_I4, 3);
				cursor.Emit(OpCodes.Ldloc, walletIndex);
				cursor.Emit(OpCodes.Stelem_I8);
			}

			if (cursor.TryGotoNext(i => i.MatchLdloc(2), i => i.MatchLdcI4(0), i => i.MatchConvI8(), i => i.MatchBle(out _)))
			{
				cursor.Index += 3;

				cursor.Remove();
				cursor.Emit(OpCodes.Ble, label);
			}

			if (cursor.TryGotoNext(i => i.MatchLdarg(0), i => i.MatchLdsfld(typeof(Lang).GetField("inter", BaseLibrary.Utility.defaultFlags)), i => i.MatchLdcI4(66)))
			{
				cursor.MarkLabel(label);

				cursor.Emit(OpCodes.Ldarg, 0);
				cursor.Emit(OpCodes.Ldloc, walletIndex);
				cursor.Emit(OpCodes.Ldarg, 1);
				cursor.Emit(OpCodes.Ldarg, 2);

				cursor.EmitDelegate<Action<SpriteBatch, long, float, float>>((sb, walletCount, shopx, shopy) =>
				{
					int walletType = PortableStorage.Instance.ItemType<Wallet>();
					if (walletCount > 0L) sb.Draw(Main.itemTexture[walletType], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 40f), Main.itemTexture[walletType].Size() * 0.5f));
				});
			}
		}
	}
}