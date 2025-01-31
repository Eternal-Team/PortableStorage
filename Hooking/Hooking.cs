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

		#region Lock icon - hover

		if (!cursor.TryGotoNext(MoveType.AfterLabel,
			    i => i.MatchLdsfld<Main>("player"),
			    i => i.MatchLdsfld<Main>("myPlayer"),
			    i => i.MatchLdelemRef(),
			    i => i.MatchLdfld<Player>("inventory"),
			    i => i.MatchLdcI4(0),
			    i => i.MatchLdloc(36),
			    i => i.MatchCall<ItemSlot>("MouseHover")))
			throw new Exception("Could not find matching instruction in ILCursor");

		label = il.DefineLabel();

		cursor.EmitLdloc(36);
		cursor.EmitLdloc(34);
		cursor.EmitLdloc(35);
		cursor.EmitDelegate((int slot, int x, int y) => {
			if (!Locks[slot])
				return false;

			Vector2 position = new Vector2(x, y) + new Vector2(26) * Main.inventoryScale;
			Vector2 size = new Vector2(22) * Main.inventoryScale;

			if (!(Main.mouseX >= position.X) || !(Main.mouseX <= position.X + size.X) || !(Main.mouseY >= position.Y) || !(Main.mouseY <= position.Y + size.Y)) return false;

			Main.instance.MouseText(PortableStorage.Instance.GetLocalization("UI.Locked").ToString());

			return true;
		});
		cursor.EmitBrtrue(label);

		cursor.Index += 7;
		cursor.MarkLabel(label);

		#endregion

		#region Lock icon - draw

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