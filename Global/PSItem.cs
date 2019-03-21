using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Bags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSItem : GlobalItem
	{
		public const float angleDecrement = 0.06981317f;
		public const float scaleDecrement = 0.015f;
		private static readonly Vector2 origin = new Vector2(30);

		public static Dictionary<int, (float, float)> BlackHoleData = new Dictionary<int, (float, float)>();

		public override bool OnPickup(Item item, Player player)
		{
			if (BlackHoleData.ContainsKey(item.whoAmI)) BlackHoleData.Remove(item.whoAmI);

			if (item.IsCoin())
			{
				Wallet wallet = player.inventory.OfType<Wallet>().FirstOrDefault();

				if (wallet != null)
				{
					long addedCoins = item.value * 2 + wallet.Handler.stacks.CountCoins();

					wallet.Handler.stacks = Utils.CoinsSplit(addedCoins).Select((stack, index) =>
					{
						Item coin = new Item();
						coin.SetDefaults(ItemID.CopperCoin + index);
						coin.stack = stack;
						return coin;
					}).Reverse().ToList();

					for (int i = 0; i < 4; i++) wallet.Handler.OnContentsChanged.Invoke(i);

					return false;
				}
			}

			return base.OnPickup(item, player);
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float itemScale, int whoAmI)
		{
			if (BlackHoleData.ContainsKey(item.whoAmI))
			{
				(float angle, float scale) = BlackHoleData[item.whoAmI];

				if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;
				if (scale > 0f) scale -= scaleDecrement;

				itemScale *= scale.Clamp(0, 1);
				spriteBatch.Draw(Main.extraTexture[50], Main.itemAnimationsRegistered.Contains(item.type) ? new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f / Main.itemAnimations[item.type].FrameCount + 2f) : new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f + 2f), null, lightColor, angle + rotation, origin, itemScale, SpriteEffects.None, 0f);
			}

			return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref itemScale, whoAmI);
		}
	}
}