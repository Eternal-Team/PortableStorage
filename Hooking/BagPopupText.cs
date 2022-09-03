using System;
using System.Reflection;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage;

public static class BagPopupText
{
	private static Item[] ItemTextBags;

	private static MethodInfo FindNextItemTextSlot = typeof(PopupText).GetMethod("FindNextItemTextSlot", ReflectionUtility.DefaultFlags_Static);
	private static MethodInfo GetRarity = typeof(RarityLoader).GetMethod("GetRarity", ReflectionUtility.DefaultFlags_Static);

	public static int NewText(PopupTextContext context, Item bag, Item newItem, int stack, bool noStack = false, bool longText = false)
	{
		if (!Main.showItemText || newItem.Name is null || !newItem.active || Main.netMode == NetmodeID.Server)
			return -1;

		bool isCoin = newItem.IsACoin;
		for (int i = 0; i < 20; i++)
		{
			PopupText popupText = Main.popupText[i];
			if (!popupText.active || popupText.notActuallyAnItem || ItemTextBags[i] != bag || (popupText.name != newItem.AffixName() && (!isCoin || !popupText.coinText)) || popupText.NoStack || noStack)
				continue;

			string text = $"{newItem.Name} ({popupText.stack + stack})";

			Vector2 size = FontAssets.MouseText.Value.MeasureString(text) + new Vector2(24f, 0f);
			if (popupText.lifeTime < 0)
				popupText.scale = 1f;

			if (popupText.lifeTime < 60)
				popupText.lifeTime = 60;

			if (isCoin && popupText.coinText)
			{
				int num = 0;
				if (newItem.type == ItemID.CopperCoin)
					num += stack;
				else if (newItem.type == ItemID.SilverCoin)
					num += 100 * stack;
				else if (newItem.type == ItemID.GoldCoin)
					num += 10000 * stack;
				else if (newItem.type == ItemID.PlatinumCoin)
					num += 1000000 * stack;

				popupText.coinValue += num;
				text = PopupText.ValueToName(popupText.coinValue);
				size = FontAssets.MouseText.Value.MeasureString(text) + new Vector2(24f, 0f);
				popupText.name = text;
				if (popupText.coinValue >= 1000000)
				{
					if (popupText.lifeTime < 300)
						popupText.lifeTime = 300;

					popupText.color = new Color(220, 220, 198);
				}
				else if (popupText.coinValue >= 10000)
				{
					if (popupText.lifeTime < 240)
						popupText.lifeTime = 240;

					popupText.color = new Color(224, 201, 92);
				}
				else if (popupText.coinValue >= 100)
				{
					if (popupText.lifeTime < 180)
						popupText.lifeTime = 180;

					popupText.color = new Color(181, 192, 193);
				}
				else if (popupText.coinValue >= 1)
				{
					if (popupText.lifeTime < 120)
						popupText.lifeTime = 120;

					popupText.color = new Color(246, 138, 96);
				}
			}

			popupText.stack += stack;
			popupText.scale = 0f;
			popupText.rotation = 0f;
			popupText.position.X = newItem.position.X + newItem.width * 0.5f - size.X * 0.5f;
			popupText.position.Y = newItem.position.Y + newItem.height * 0.25f - size.Y * 0.5f;
			popupText.velocity.Y = -7f;
			popupText.context = context;
			popupText.npcNetID = 0;
			if (popupText.coinText)
				popupText.stack = 1;

			return i;
		}

		int slotIndex = FindNextItemTextSlot.InvokeStatic<int>();
		if (slotIndex >= 0)
		{
			string text3 = newItem.AffixName();
			if (stack > 1)
				text3 = text3 + " (" + stack + ")";

			Vector2 size = FontAssets.MouseText.Value.MeasureString(text3) + new Vector2(24f, 0f);
			PopupText popupText = Main.popupText[slotIndex];
			PopupText.ResetText(popupText);
			popupText.active = true;
			popupText.position.X = newItem.position.X + newItem.width * 0.5f - size.X * 0.5f;
			popupText.position.Y = newItem.position.Y + newItem.height * 0.25f - size.Y * 0.5f;

			popupText.color = Color.White;
			if (newItem.rare == ItemRarityID.Blue)
				popupText.color = new Color(150, 150, 255);
			else if (newItem.rare == ItemRarityID.Green)
				popupText.color = new Color(150, 255, 150);
			else if (newItem.rare == ItemRarityID.Orange)
				popupText.color = new Color(255, 200, 150);
			else if (newItem.rare == ItemRarityID.LightRed)
				popupText.color = new Color(255, 150, 150);
			else if (newItem.rare == ItemRarityID.Pink)
				popupText.color = new Color(255, 150, 255);
			else if (newItem.rare == ItemRarityID.Master)
				popupText.master = true;
			else if (newItem.rare == ItemRarityID.Quest)
				popupText.color = new Color(255, 175, 0);
			else if (newItem.rare == ItemRarityID.Gray)
				popupText.color = new Color(130, 130, 130);
			else if (newItem.rare == ItemRarityID.LightPurple)
				popupText.color = new Color(210, 160, 255);
			else if (newItem.rare == ItemRarityID.Lime)
				popupText.color = new Color(150, 255, 10);
			else if (newItem.rare == ItemRarityID.Yellow)
				popupText.color = new Color(255, 255, 10);
			else if (newItem.rare == ItemRarityID.Cyan)
				popupText.color = new Color(5, 200, 255);
			else if (newItem.rare == ItemRarityID.Red)
				popupText.color = new Color(255, 40, 100);
			else if (newItem.rare == ItemRarityID.Purple)
				popupText.color = new Color(180, 40, 255);
			else if (newItem.rare >= ItemRarityID.Count)
				popupText.color = GetRarity.InvokeStatic<ModRarity>(newItem.rare).RarityColor;

			ItemTextBags[slotIndex] = bag;
			popupText.rarity = newItem.rare;
			popupText.expert = newItem.expert;
			popupText.master = newItem.master;
			popupText.name = newItem.AffixName();
			popupText.stack = stack;
			popupText.velocity.Y = -7f;
			popupText.lifeTime = 60;
			popupText.context = context;
			if (longText)
				popupText.lifeTime *= 5;

			popupText.coinValue = 0;
			popupText.coinText = newItem.type is >= ItemID.CopperCoin and <= ItemID.PlatinumCoin;
			if (popupText.coinText)
			{
				if (newItem.type == ItemID.CopperCoin)
					popupText.coinValue += popupText.stack;
				else if (newItem.type == ItemID.SilverCoin)
					popupText.coinValue += 100 * popupText.stack;
				else if (newItem.type == ItemID.GoldCoin)
					popupText.coinValue += 10000 * popupText.stack;
				else if (newItem.type == ItemID.PlatinumCoin)
					popupText.coinValue += 1000000 * popupText.stack;

				popupText.stack = 1;
				if (popupText.coinValue >= 1000000)
				{
					if (popupText.lifeTime < 300)
						popupText.lifeTime = 300;

					popupText.color = new Color(220, 220, 198);
				}
				else if (popupText.coinValue >= 10000)
				{
					if (popupText.lifeTime < 240)
						popupText.lifeTime = 240;

					popupText.color = new Color(224, 201, 92);
				}
				else if (popupText.coinValue >= 100)
				{
					if (popupText.lifeTime < 180)
						popupText.lifeTime = 180;

					popupText.color = new Color(181, 192, 193);
				}
				else if (popupText.coinValue >= 1)
				{
					if (popupText.lifeTime < 120)
						popupText.lifeTime = 120;

					popupText.color = new Color(246, 138, 96);
				}
			}
		}

		return slotIndex;
	}

	private static void DrawItemTextPopups(On.Terraria.Main.orig_DrawItemTextPopups orig, float scaleTarget)
	{
		for (int i = 0; i < 20; i++)
		{
			PopupText popupText = Main.popupText[i];
			if (!popupText.active)
				continue;

			string text = popupText.name;
			if (popupText.stack > 1)
				text = text + " (" + popupText.stack + ")";

			Vector2 vector = FontAssets.MouseText.Value.MeasureString(text);
			Vector2 origin = new Vector2(vector.X * 0.5f, vector.Y * 0.5f);
			float num = popupText.scale / scaleTarget;
			int num2 = (int)(255f - 255f * num);
			float num3 = popupText.color.R;
			float num4 = popupText.color.G;
			float num5 = popupText.color.B;
			float num6 = popupText.color.A;
			num3 *= num * popupText.alpha * 0.3f;
			num5 *= num * popupText.alpha * 0.3f;
			num4 *= num * popupText.alpha * 0.3f;
			num6 *= num * popupText.alpha;
			Color color2 = Color.Black;
			float scale = 1f;
			switch (popupText.context)
			{
				case PopupTextContext.ItemPickupToVoidContainer:
					color2 = new Color(127, 20, 255) * 0.4f;
					scale = 0.8f;
					break;
				case PopupTextContext.SonarAlert:
					color2 = Color.Blue * 0.4f;
					if (popupText.npcNetID != 0)
						color2 = Color.Red * 0.4f;
					scale = 1f;
					break;
			}

			float num7 = num2 / 255f;
			for (int j = 0; j < 5; j++)
			{
				Color color = color2;
				float num8 = 0f;
				float num9 = 0f;
				switch (j)
				{
					case 0:
						num8 -= scaleTarget * 2f;
						break;
					case 1:
						num8 += scaleTarget * 2f;
						break;
					case 2:
						num9 -= scaleTarget * 2f;
						break;
					case 3:
						num9 += scaleTarget * 2f;
						break;
					default:
						color = popupText.color * num * popupText.alpha * scale;
						break;
				}

				if (j < 4)
				{
					num6 = popupText.color.A * num * popupText.alpha;
					color = new Color(0, 0, 0, (int)num6);
				}

				if (color2 != Color.Black && j < 4)
				{
					num8 *= 1.3f + 1.3f * num7;
					num9 *= 1.3f + 1.3f * num7;
				}

				float num10 = popupText.position.Y - Main.screenPosition.Y + num9;
				if (Main.player[Main.myPlayer].gravDir == -1f)
					num10 = Main.screenHeight - num10;

				if (color2 != Color.Black && j < 4)
				{
					Color color3 = color2;
					color3.A = (byte)MathHelper.Lerp(60f, 127f, Utils.GetLerpValue(0f, 255f, num6, true));
					Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(popupText.position.X - Main.screenPosition.X + num8 + origin.X, num10 + origin.Y), Color.Lerp(color, color3, 0.5f), popupText.rotation, origin, popupText.scale, SpriteEffects.None, 0f);
					Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(popupText.position.X - Main.screenPosition.X + num8 + origin.X, num10 + origin.Y), color3, popupText.rotation, origin, popupText.scale, SpriteEffects.None, 0f);
				}
				else
				{
					Main.spriteBatch.DrawString(FontAssets.MouseText.Value, text, new Vector2(popupText.position.X - Main.screenPosition.X + num8 + origin.X, num10 + origin.Y), color, popupText.rotation, origin, popupText.scale, SpriteEffects.None, 0f);
				}

				if (!ItemTextBags[i].IsAir)
				{
					Main.instance.LoadItem(ItemTextBags[i].type);
					var texture = TextureAssets.Item[ItemTextBags[i].type].Value;
					float texScale = Math.Min(20f / texture.Width, 20f / texture.Height);

					float x = popupText.position.X - Main.screenPosition.X - 10f - 4f;
					float y = popupText.position.Y - Main.screenPosition.Y + 10f;
					if (Main.player[Main.myPlayer].gravDir == -1f) y = Main.screenHeight - y;

					Main.spriteBatch.Draw(texture, new Vector2(x, y), null, Color.White * num * popupText.alpha, popupText.rotation, texture.Size() * 0.5f, texScale * popupText.scale, SpriteEffects.None, 0f);
				}
			}
		}
	}

	private static void PopupTextOnResetText(On.Terraria.PopupText.orig_ResetText orig, PopupText text)
	{
		orig(text);

		int index = Array.FindLastIndex(Main.popupText, t => t == text);
		if (index != -1) ItemTextBags[index] = new Item();
	}

	public static void Load()
	{
		ItemTextBags = new Item[Main.popupText.Length];
		for (int i = 0; i < ItemTextBags.Length; i++) ItemTextBags[i] = new Item();
		On.Terraria.Main.DrawItemTextPopups += DrawItemTextPopups;
		On.Terraria.PopupText.ResetText += PopupTextOnResetText;
	}
}