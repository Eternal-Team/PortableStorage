using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Ammo;
using PortableStorage.Items.Special;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage
{
	public class PSItem : GlobalItem
	{
		private const float angleDecrement = 0.06981317f;
		private const float scaleDecrement = 0.03f;

		private static Dictionary<int, (float scale, float angle)> _blackHoleData;
		public static Dictionary<int, (float scale, float angle)> BlackHoleData => _blackHoleData ?? (_blackHoleData = new Dictionary<int, (float, float)>());

		public override bool OnPickup(Item item, Player player)
		{
			if (item.IsCoin())
			{
				Wallet wallet = player.inventory.OfType<Wallet>().FirstOrDefault();

				if (wallet != null)
				{
					Hooking.BagItemText(wallet.item, item, item.stack, false, false);

					wallet.Coins += Utils.CoinsCount(out bool _, new[] { item });

					Main.PlaySound(SoundID.CoinPickup);

					return false;
				}
			}

			if (item.ammo > 0)
			{
				BaseAmmoBag ammoBag = player.inventory.OfType<BaseAmmoBag>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (ammoBag != null)
				{
					Main.PlaySound(SoundID.Grab);

					Item temp = item.Clone();

					ammoBag.Handler.InsertItem(ref item);

					Hooking.BagItemText(ammoBag.item, temp, temp.stack - item.stack, false, false);

					if (item.IsAir || !item.active) return false;
				}
			}

			if (item.thrown)
			{
				NinjaArsenalBelt belt = player.inventory.OfType<NinjaArsenalBelt>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (belt != null)
				{
					Main.PlaySound(SoundID.Grab);

					Item temp = item.Clone();

					belt.Handler.InsertItem(ref item);

					Hooking.BagItemText(belt.item, temp, temp.stack - item.stack, false, false);

					if (item.IsAir || !item.active) return false;
				}
			}

			if (item.bait > 0 || Utility.FishingWhitelist.Contains(item.type))
			{
				FishingBelt belt = player.inventory.OfType<FishingBelt>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (belt != null)
				{
					Main.PlaySound(SoundID.Grab);

					Item temp = item.Clone();

					belt.Handler.InsertItem(ref item);

					Hooking.BagItemText(belt.item, temp, temp.stack - item.stack, false, false);

					if (item.IsAir || !item.active) return false;
				}
			}

			if (Utility.OreWhitelist.Contains(item.type))
			{
				MinersBackpack minersBackpack = player.inventory.OfType<MinersBackpack>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (minersBackpack != null)
				{
					Main.PlaySound(SoundID.Grab);

					Item temp = item.Clone();

					minersBackpack.Handler.InsertItem(ref item);

					Hooking.BagItemText(minersBackpack.item, temp, temp.stack - item.stack, false, false);

					if (item.IsAir || !item.active) return false;
				}
			}

			if (Utility.AlchemistBagWhitelist.Contains(item.type))
			{
				AlchemistBag alchemistBag = player.inventory.OfType<AlchemistBag>().FirstOrDefault(bag => bag.Handler.Contains(item.type) && bag.Handler.HasSpace(item));

				if (alchemistBag != null)
				{
					Main.PlaySound(SoundID.Grab);

					Item temp = item.Clone();

					alchemistBag.Handler.InsertItem(ref item, 18, 81);

					Hooking.BagItemText(alchemistBag.item, temp, temp.stack - item.stack, false, false);

					if (item.IsAir || !item.active) return false;
				}
			}

			if (item.potion && item.healLife > 0 || item.healMana > 0 && !item.potion || item.buffType > 0 && !item.summon && item.buffType != BuffID.Rudolph)
			{
				AlchemistBag alchemistBag = player.inventory.OfType<AlchemistBag>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (alchemistBag != null)
				{
					Main.PlaySound(SoundID.Grab);

					Item temp = item.Clone();

					alchemistBag.Handler.InsertItem(ref item, 0, 18);

					Hooking.BagItemText(alchemistBag.item, temp, temp.stack - item.stack, false, false);

					if (item.IsAir || !item.active) return false;
				}
			}

			if (Utility.AlchemistBagWhitelist.Contains(item.type))
			{
				AlchemistBag alchemistBag = player.inventory.OfType<AlchemistBag>().FirstOrDefault(bag => bag.Handler.HasSpace(item));

				if (alchemistBag != null)
				{
					Main.PlaySound(SoundID.Grab);

					Item temp = item.Clone();

					alchemistBag.Handler.InsertItem(ref item, 18, 81);

					Hooking.BagItemText(alchemistBag.item, temp, temp.stack - item.stack, false, false);

					if (item.IsAir || !item.active) return false;
				}
			}

			if (item.createTile >= 0 || item.createWall >= 0)
			{
				BuilderReserve builderReserve = player.inventory.OfType<BuilderReserve>().FirstOrDefault(reserve => reserve.Handler.Items.Any(i => i.type == item.type));
				if (builderReserve != null)
				{
					int index = Array.FindIndex(builderReserve.Handler.Items, i => i.type == item.type);
					if (index != -1)
					{
						Hooking.BagItemText(builderReserve.item, item, item.stack, false, false);

						item = builderReserve.Handler.InsertItem(index, item);
						item.TurnToAir();

						Main.PlaySound(SoundID.Grab);

						return false;
					}
				}
			}

			if (Utility.SeedWhitelist.Contains(item.type))
			{
				GardenerSatchel gardenerSatchel = player.inventory.OfType<GardenerSatchel>().FirstOrDefault(reserve => reserve.Handler.Items.Any(i => i.type == item.type));
				if (gardenerSatchel != null)
				{
					int index = Array.FindIndex(gardenerSatchel.Handler.Items, i => i.type == item.type);
					if (index != -1)
					{
						Hooking.BagItemText(gardenerSatchel.item, item, item.stack, false, false);

						item = gardenerSatchel.Handler.InsertItem(index, item);
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
				spriteBatch.Draw(Main.extraTexture[50], Main.itemAnimationsRegistered.Contains(item.type) ? new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f / Main.itemAnimations[item.type].FrameCount + 2f) : new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f + 2f), null, lightColor, angle + rotation, new Vector2(30), itemScale, SpriteEffects.None, 0f);

				BlackHoleData[whoAmI] = (scale, angle);
			}

			return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref itemScale, whoAmI);
		}

		public override void OpenVanillaBag(string context, Player player, int arg)
		{
			if (context == "crate")
			{
				if (arg == ItemID.IronCrate && Main.rand.NextBool(20) || arg == ItemID.GoldenCrate && Main.rand.NextBool(10)) player.QuickSpawnItem(mod.ItemType<FishingBelt>());
			}
		}
	}
}