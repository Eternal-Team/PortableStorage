using System;
using System.Reflection;
using BaseLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace PortableStorage.Hooking;

internal static class Hooking
{
	internal static bool[] Locks = new bool[50];

	internal static void Load()
	{
		IL_Main.DrawInventory += IL_MainOnDrawInventory;
	}
	
	// BUG: actions like deposit all will ignore Locks
	// TODO: mouse over lock icon
	
	private static void IL_MainOnDrawInventory(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		#region Block interaction with slot

		ILLabel label = null;
		if (!cursor.TryGotoNext(i => i.MatchLdfld<Player>("inventoryChestStack"), i => i.MatchLdloc(36), i => i.MatchLdelemU1(), i => i.MatchBrtrue(out label)))
			throw new Exception("Could not find matching instruction in ILCursor");

		cursor.Index += 4;

		FieldInfo field = typeof(Hooking).GetField("Locks", ReflectionUtility.DefaultFlags_Static);
		cursor.EmitLdsfld(field);
		cursor.EmitLdloc(36);
		cursor.EmitLdelemU1();
		cursor.EmitBrtrue(label);

		#endregion


		#region Draw lock icon

		if (!cursor.TryGotoNext(i => i.MatchCall<ItemSlot>("Draw")))
			throw new Exception("Could not find matching instruction in ILCursor");

		cursor.Index++;

		field = typeof(Main).GetField("spriteBatch", ReflectionUtility.DefaultFlags_Static);
		cursor.EmitLdsfld(field);
		cursor.EmitLdloc(36);
		cursor.EmitLdloc(34);
		cursor.EmitLdloc(35);
		cursor.EmitDelegate((SpriteBatch spriteBatch, int slot, int x, int y) => {
			if (!Locks[slot]) return;

			float inventoryScale = Main.inventoryScale;
			Vector2 position = new Vector2(x, y) + new Vector2(26) * inventoryScale;

			spriteBatch.Draw(TextureAssets.HbLock[0].Value, position, new Rectangle(0, 0, 22, 22), Color.White, 0f, Vector2.Zero, inventoryScale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureAssets.HbLock[0].Value, position, new Rectangle(26, 0, 22, 22), Color.White, 0f, Vector2.Zero, inventoryScale, SpriteEffects.None, 0f);
		});

		#endregion
	}
}