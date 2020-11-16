using System;
using System.Reflection;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static Item[] ItemTextBags;

		private static MethodInfo ValueToName_Static = typeof(PopupText).GetMethod("ValueToName", ReflectionUtility.DefaultFlags_Static);
		private static MethodInfo ValueToName = typeof(PopupText).GetMethod("ValueToName", ReflectionUtility.DefaultFlags);
		private static MethodInfo FindNextItemTextSlot = typeof(PopupText).GetMethod("FindNextItemTextSlot", ReflectionUtility.DefaultFlags_Static);
		private static MethodInfo GetRarity = typeof(RarityLoader).GetMethod("GetRarity", ReflectionUtility.DefaultFlags_Static);

		public static int NewText(Item bag, Item newItem, int stack, bool noStack = false, bool longText = false)
		{
			if (!Main.showItemText) return -1;
			if (newItem.Name == null || !newItem.active) return -1;
			if (Main.netMode == NetmodeID.Server) return -1;

			bool isCoin = newItem.IsACoin;
			for (int i = 0; i < Main.popupText.Length; i++)
			{
				PopupText popupText = Main.popupText[i];
				if (!popupText.active || popupText.notActuallyAnItem || ItemTextBags[i] != bag || popupText.name != newItem.AffixName() && (!isCoin || !popupText.coinText) || popupText.NoStack || noStack) continue;

				string text = $"{newItem.Name} ({popupText.stack + stack})";

				Vector2 vector = FontAssets.MouseText.Value.MeasureString(text);
				if (popupText.lifeTime < 0) popupText.scale = 1f;
				if (popupText.lifeTime < 60) popupText.lifeTime = 60;

				if (isCoin && popupText.coinText)
				{
					int num = 0;
					if (newItem.type == ItemID.CopperCoin) num += stack;
					else if (newItem.type == ItemID.SilverCoin) num += 100 * stack;
					else if (newItem.type == ItemID.GoldCoin) num += 10000 * stack;
					else if (newItem.type == ItemID.PlatinumCoin) num += 1000000 * stack;

					popupText.coinValue += num;
					text = ValueToName_Static.Invoke<string>(null, popupText.coinValue);
					vector = FontAssets.MouseText.Value.MeasureString(text);
					popupText.name = text;
					if (popupText.coinValue >= 1000000)
					{
						if (popupText.lifeTime < 300) popupText.lifeTime = 300;
						popupText.color = new Color(220, 220, 198);
					}
					else if (popupText.coinValue >= 10000)
					{
						if (popupText.lifeTime < 240) popupText.lifeTime = 240;
						popupText.color = new Color(224, 201, 92);
					}
					else if (popupText.coinValue >= 100)
					{
						if (popupText.lifeTime < 180) popupText.lifeTime = 180;
						popupText.color = new Color(181, 192, 193);
					}
					else if (popupText.coinValue >= 1)
					{
						if (popupText.lifeTime < 120) popupText.lifeTime = 120;
						popupText.color = new Color(246, 138, 96);
					}
				}

				popupText.stack += stack;
				popupText.scale = 0f;
				popupText.rotation = 0f;
				popupText.position.X = newItem.position.X + newItem.width * 0.5f - vector.X * 0.5f;
				popupText.position.Y = newItem.position.Y + newItem.height * 0.25f - vector.Y * 0.5f;
				popupText.velocity.Y = -7f;
				popupText.context = PopupTextContext.RegularItemPickup;
				popupText.npcNetID = 0;
				if (popupText.coinText) popupText.stack = 1;

				return i;
			}

			int index = FindNextItemTextSlot.Invoke<int>(null);
			if (index >= 0)
			{
				string text = newItem.AffixName();
				if (stack > 1) text = text + " (" + stack + ")";

				Vector2 vector2 = FontAssets.MouseText.Value.MeasureString(text);
				PopupText popupText = Main.popupText[index];
				popupText.alpha = 1f;
				popupText.alphaDir = -1;
				popupText.active = true;
				popupText.scale = 0f;
				popupText.NoStack = noStack;
				popupText.rotation = 0f;
				popupText.position.X = newItem.position.X + newItem.width * 0.5f - vector2.X * 0.5f;
				popupText.position.Y = newItem.position.Y + newItem.height * 0.25f - vector2.Y * 0.5f;
				popupText.color = Color.White;
				if (newItem.rare == ItemRarityID.Blue) popupText.color = new Color(150, 150, 255);
				else if (newItem.rare == ItemRarityID.Green) popupText.color = new Color(150, 255, 150);
				else if (newItem.rare == ItemRarityID.Orange) popupText.color = new Color(255, 200, 150);
				else if (newItem.rare == ItemRarityID.LightRed) popupText.color = new Color(255, 150, 150);
				else if (newItem.rare == ItemRarityID.Pink) popupText.color = new Color(255, 150, 255);
				else if (newItem.rare == -11) popupText.color = new Color(255, 175, 0);
				else if (newItem.rare == -1) popupText.color = new Color(130, 130, 130);
				else if (newItem.rare == ItemRarityID.LightPurple) popupText.color = new Color(210, 160, 255);
				else if (newItem.rare == ItemRarityID.Lime) popupText.color = new Color(150, 255, 10);
				else if (newItem.rare == ItemRarityID.Yellow) popupText.color = new Color(255, 255, 10);
				else if (newItem.rare == ItemRarityID.Cyan) popupText.color = new Color(5, 200, 255);
				else if (newItem.rare == ItemRarityID.Red) popupText.color = new Color(255, 40, 100);
				else if (newItem.rare == ItemRarityID.Purple) popupText.color = new Color(180, 40, 255);
				else if (newItem.rare >= ItemRarityID.Count) popupText.color = GetRarity.Invoke<ModRarity>(null, newItem.rare).RarityColor;

				ItemTextBags[index] = bag;
				popupText.rarity = newItem.rare;
				popupText.expert = newItem.expert;
				popupText.master = newItem.master;
				popupText.name = newItem.AffixName();
				popupText.stack = stack;
				popupText.velocity.Y = -7f;
				popupText.lifeTime = 60;
				popupText.context = PopupTextContext.RegularItemPickup;
				popupText.npcNetID = 0;
				if (longText) popupText.lifeTime *= 5;

				popupText.coinValue = 0;
				popupText.coinText = newItem.IsACoin;
				if (popupText.coinText)
				{
					if (newItem.type == ItemID.CopperCoin) popupText.coinValue += popupText.stack;
					else if (newItem.type == ItemID.SilverCoin) popupText.coinValue += 100 * popupText.stack;
					else if (newItem.type == ItemID.GoldCoin) popupText.coinValue += 10000 * popupText.stack;
					else if (newItem.type == ItemID.PlatinumCoin) popupText.coinValue += 1000000 * popupText.stack;

					ValueToName.Invoke(popupText);

					popupText.stack = 1;
					if (popupText.coinValue >= 1000000)
					{
						if (popupText.lifeTime < 300) popupText.lifeTime = 300;
						popupText.color = new Color(220, 220, 198);
					}
					else if (popupText.coinValue >= 10000)
					{
						if (popupText.lifeTime < 240) popupText.lifeTime = 240;
						popupText.color = new Color(224, 201, 92);
					}
					else if (popupText.coinValue >= 100)
					{
						if (popupText.lifeTime < 180) popupText.lifeTime = 180;
						popupText.color = new Color(181, 192, 193);
					}
					else if (popupText.coinValue >= 1)
					{
						if (popupText.lifeTime < 120) popupText.lifeTime = 120;
						popupText.color = new Color(246, 138, 96);
					}
				}
			}

			return index;
		}

		private static void DoDraw(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(137), i => i.MatchLdcI4(1), i => i.MatchAdd()))
			{
				cursor.Emit(OpCodes.Ldloc, 137);
				cursor.Emit(OpCodes.Ldloc, 114);
				cursor.Emit(OpCodes.Ldloc, 158);

				cursor.EmitDelegate<Action<int, float, float>>((index, targetScale, num26) =>
				{
					PopupText popupText = Main.popupText[index];
					if (!popupText.active || ItemTextBags[index].IsAir) return;

					Main.instance.LoadItem(ItemTextBags[index].type);

					Texture2D texture = TextureAssets.Item[ItemTextBags[index].type].Value;
					float texScale = Math.Min(20f / texture.Width, 20f / texture.Height);

					float x = popupText.position.X - Main.screenPosition.X + num26 - 25f;
					float y = popupText.position.Y - Main.screenPosition.Y + 10f;
					if (Main.player[Main.myPlayer].gravDir == -1f) y = Main.screenHeight - y;

					Main.spriteBatch.Draw(texture, new Vector2(x, y), null, Color.White, 0f, texture.Size() * 0.5f, texScale * popupText.scale, SpriteEffects.None, 0f);
				});
			}
		}
	}
}