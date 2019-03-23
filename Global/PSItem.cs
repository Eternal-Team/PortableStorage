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
		// todo: picked up ammo get automatically sent into a bag

		private const float angleDecrement = 0.06981317f;
		private const float scaleDecrement = 0.015f;
		private static readonly Vector2 origin = new Vector2(30);

		private static Dictionary<int, (float scale, float angle)> _blackHoleData;

		public static Dictionary<int, (float scale, float angle)> BlackHoleData => _blackHoleData ?? (_blackHoleData = new Dictionary<int, (float, float)>());

		public override bool OnPickup(Item item, Player player)
		{
			if (item.IsCoin())
			{
				Wallet wallet = player.inventory.OfType<Wallet>().FirstOrDefault();

				if (wallet != null)
				{
					long addedCoins = Utils.CoinsCount(out bool _, new[] { item }) + wallet.Handler.stacks.CountCoins();

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
			if (BlackHoleData.ContainsKey(whoAmI))
			{
				(float scale, float angle) = BlackHoleData[whoAmI];

				if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;
				if (scale > 0f) scale -= scaleDecrement;

				itemScale *= scale.Clamp(0, 1);
				spriteBatch.Draw(Main.extraTexture[50], Main.itemAnimationsRegistered.Contains(item.type) ? new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f / Main.itemAnimations[item.type].FrameCount + 2f) : new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f + 2f), null, lightColor, angle + rotation, origin, itemScale, SpriteEffects.None, 0f);

				BlackHoleData[whoAmI] = (scale, angle);
			}

			return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref itemScale, whoAmI);
		}
	}
}