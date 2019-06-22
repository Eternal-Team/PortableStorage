using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Ammo;
using PortableStorage.Items.Special;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSItem : GlobalItem
	{
		private const float angleDecrement = 0.06981317f;
		private const float scaleDecrement = 0.015f;
		private static readonly Vector2 origin = new Vector2(30);

		private static Dictionary<int, (float scale, float angle)> _blackHoleData;
		public static Dictionary<int, (float scale, float angle)> BlackHoleData => _blackHoleData ?? (_blackHoleData = new Dictionary<int, (float, float)>());

		// todo: tiles/walls don't go inside Builder's Reserve
		public override bool OnPickup(Item item, Player player)
		{
			if (item.IsCoin())
			{
				Wallet wallet = player.inventory.OfType<Wallet>().FirstOrDefault();

				if (wallet != null)
				{
					wallet.Coins += Utils.CoinsCount(out bool _, new[] {item});

					Main.PlaySound(SoundID.CoinPickup);

					return false;
				}
			}
			else if (item.ammo > 0)
			{
				BaseAmmoBag ammoBag = player.inventory.OfType<BaseAmmoBag>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (ammoBag != null)
				{
					Main.PlaySound(SoundID.Grab);

					int index = ammoBag.Handler.Items.FindIndex(other => other.type == item.type && other.stack < other.maxStack);
					if (index != -1)
					{
						item = ammoBag.Handler.InsertItem(index, item);
						if (item.IsAir || !item.active) return false;
					}

					for (int j = 0; j < ammoBag.Handler.Slots; j++)
					{
						item = ammoBag.Handler.InsertItem(j, item);

						if (item.IsAir || !item.active) return false;
					}
				}
			}
			else if (Utility.OreWhitelist.Contains(item.type))
			{
				MinersBackpack minersBackpack = player.inventory.OfType<MinersBackpack>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (minersBackpack != null)
				{
					Main.PlaySound(SoundID.Grab);

					int index = minersBackpack.Handler.Items.FindIndex(other => other.type == item.type && other.stack < other.maxStack);
					if (index != -1)
					{
						item = minersBackpack.Handler.InsertItem(index, item);
						if (item.IsAir || !item.active) return false;
					}

					for (int j = 0; j < minersBackpack.Handler.Slots; j++)
					{
						item = minersBackpack.Handler.InsertItem(j, item);

						if (item.IsAir || !item.active) return false;
					}
				}
			}
			else if (Utility.AlchemistBagWhitelist.Contains(item.type))
			{
				AlchemistBag alchemistBag = player.inventory.OfType<AlchemistBag>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (alchemistBag != null)
				{
					Main.PlaySound(SoundID.Grab);

					int index = alchemistBag.Handler.Items.FindIndex(other => other.type == item.type && other.stack < other.maxStack);
					if (index != -1)
					{
						item = alchemistBag.Handler.InsertItem(index, item);
						if (item.IsAir || !item.active) return false;
					}

					for (int j = 0; j < alchemistBag.Handler.Slots; j++)
					{
						item = alchemistBag.Handler.InsertItem(j, item);

						if (item.IsAir || !item.active) return false;
					}
				}
			}
			else if (item.createTile >= 0 || item.createWall >= 0)
			{
				BuilderReserve builderReserve = player.inventory.OfType<BuilderReserve>().FirstOrDefault(reserve => reserve.Handler.Items.Any(i => i.type == item.type));
				if (builderReserve != null)
				{
					int index = builderReserve.Handler.Items.FindIndex(i => i.type == item.type);
					if (index != -1)
					{
						item = builderReserve.Handler.InsertItem(index, item);
						item.TurnToAir();

						Main.PlaySound(SoundID.Grab);

						return false;
					}
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