using System.Reflection;
using BaseLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace PortableStorage.IL;

internal static class Hooking
{
	internal static bool[] Locks = new bool[50];
	internal static bool[] ChangedState = new bool[50];

	internal static void SetLock(Item item, bool value)
	{
		int index = -1;
		for (int i = 0; i < 50; i++)
		{
			if (Main.LocalPlayer.inventory[i] == item)
			{
				index = i;
				break;
			}
		}

		if (index == -1) return;

		Locks[index] = value;
	}

	internal static void Load()
	{
		IL_Main.DrawInventory += IL_MainOnDrawInventory;
	}

	// note: hook ItemLoader.RightClick to prevent vanilla sound from playing 
	private static void IL_MainOnDrawInventory(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		#region Item.favorited reset

		ILUtility.TryGotoNext(cursor, i => i.MatchLdcI4(100), i => i.MatchLdcI4(100), i => i.MatchLdcI4(100), i => i.MatchLdcI4(100));

		cursor.EmitLdloc(36);
		cursor.EmitDelegate((int slot) => {
			Item item = Main.LocalPlayer.inventory[slot];

			if (!item.IsAir && ChangedState[slot])
			{
				item.favorited = false;
				ChangedState[slot] = false;
			}
		});

		#endregion

		#region Block interaction with slot

		ILLabel label = null;

		ILUtility.TryGotoNext(cursor, MoveType.After, i => i.MatchLdfld<Player>("inventoryChestStack"), i => i.MatchLdloc(36), i => i.MatchLdelemU1(), i => i.MatchBrtrue(out label));

		ILLabel labelAfterLeftClick = il.DefineLabel();

		FieldInfo field = typeof(Hooking).GetField("Locks", ReflectionUtility.DefaultFlags_Static);

		cursor.EmitLdsfld(field);
		cursor.EmitLdloc(36);
		cursor.EmitLdelemU1();
		cursor.EmitBrtrue(labelAfterLeftClick);

		ILUtility.TryGotoNext(cursor, MoveType.After, i => i.MatchCall<ItemSlot>("LeftClick"));

		cursor.MarkLabel(labelAfterLeftClick);

		cursor.EmitLdloc(36);
		cursor.EmitDelegate((int slot) => Locks[slot] && !Main.LocalPlayer.inventory[slot].IsAir && Main.LocalPlayer.inventory[slot].ModItem is not Bag);
		cursor.EmitBrtrue(label);

		#endregion

		#region Lock icon - hover

		ILUtility.TryGotoNext(cursor, MoveType.AfterLabel,
			i => i.MatchLdsfld<Main>("player"),
			i => i.MatchLdsfld<Main>("myPlayer"),
			i => i.MatchLdelemRef(),
			i => i.MatchLdfld<Player>("inventory"),
			i => i.MatchLdcI4(0),
			i => i.MatchLdloc(36),
			i => i.MatchCall<ItemSlot>("MouseHover"));

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

		ILUtility.TryGotoNext(cursor, MoveType.After, i => i.MatchCall<ItemSlot>("Draw"));

		field = typeof(Main).GetField("spriteBatch", ReflectionUtility.DefaultFlags_Static);
		cursor.EmitLdsfld(field);
		cursor.EmitLdloc(36);
		cursor.EmitLdloc(34);
		cursor.EmitLdloc(35);
		cursor.EmitDelegate((SpriteBatch spriteBatch, int slot, int x, int y) => {
			if (!Locks[slot]) return;

			Item item = Main.LocalPlayer.inventory[slot];

			if (!item.IsAir)
			{
				if (!item.favorited) ChangedState[slot] = true;
				item.favorited = true;
			}

			float inventoryScale = Main.inventoryScale;
			Vector2 position = new Vector2(x, y) + new Vector2(26) * inventoryScale;

			spriteBatch.Draw(TextureAssets.HbLock[0].Value, position, new Rectangle(0, 0, 22, 22), Color.White, 0f, Vector2.Zero, inventoryScale, SpriteEffects.None, 0f);
			spriteBatch.Draw(TextureAssets.HbLock[0].Value, position, new Rectangle(26, 0, 22, 22), Color.White, 0f, Vector2.Zero, inventoryScale, SpriteEffects.None, 0f);
		});

		#endregion
	}
}