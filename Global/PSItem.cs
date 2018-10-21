using System.Linq;
using BaseLibrary.Utility;
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
		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		#region Constant fields
		public const float angleDecrement = 0.06981317f;
		public const float scaleDecrement = 0.015f;
		private static readonly Vector2 origin = new Vector2(30);
		#endregion

		#region Instance fields
		public bool markedForSuction;
		public float angle;
		public float scale = 1f;
		#endregion

		public override bool OnPickup(Item item, Player player)
		{
			markedForSuction = false;
			scale = 1f;
			angle = 0f;

			if (item.IsCoin() && player.inventory.OfType<Wallet>().Any())
			{
				Wallet wallet = player.inventory.OfType<Wallet>().First();

				long addedCoins = Utils.CoinsCount(out bool _, new[] { item }) + wallet.Handler.stacks.CountCoins();

				wallet.Handler.stacks = Utils.CoinsSplit(addedCoins).Select((x, index) =>
				{
					Item coin = new Item();
					coin.SetDefaults(ItemID.CopperCoin + index);
					coin.stack = x;
					return coin;
				}).Reverse().ToList();

				for (int i = 0; i < 4; i++) wallet.Handler.OnContentsChanged.Invoke(i);

				return false;
			}

			return base.OnPickup(item, player);
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (markedForSuction)
			{
				if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;
				if (this.scale > 0f) this.scale -= scaleDecrement;

				scale *= this.scale;
				spriteBatch.Draw(Main.extraTexture[50], Main.itemAnimationsRegistered.Contains(item.type) ? new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f / Main.itemAnimations[item.type].FrameCount + 2f) : new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f + 2f), null, Color.White, angle + rotation, origin, this.scale, SpriteEffects.None, 0f);
			}

			return true;
		}
	}
}