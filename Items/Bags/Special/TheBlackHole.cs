using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class TheBlackHole : BaseBag
	{
		private const float angleDecrement = 0.05235988F;
		private static readonly Vector2 origin = new Vector2(30);
		private const int maxRange = 160;

		public bool active;
		public float angle;

		public TheBlackHole()
		{
			Handler = new ItemHandler(27);
			Handler.OnContentsChanged += slot =>
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					Player player = Main.player[item.owner];

					List<Item> joined = player.inventory.Concat(player.armor).Concat(player.dye).Concat(player.miscEquips).Concat(player.miscDyes).Concat(player.bank.item).Concat(player.bank2.item).Concat(new[] { player.trashItem }).Concat(player.bank3.item).ToList();
					int index = joined.FindIndex(x => x == item);
					if (index < 0) return;

					NetMessage.SendData(MessageID.SyncEquipment, number: item.owner, number2: index);
				}
			};
		}

		public override ModItem Clone()
		{
			TheBlackHole clone = (TheBlackHole)base.Clone();
			clone.active = active;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Black Hole");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of items\nCollects them in a {maxRange / 16} block radius");
			ItemID.Sets.ItemNoGravity[item.type] = true;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Purple;
			item.noUseGraphic = true;
		}

		public override void UpdateInventory(Player player)
		{
			if (!active) return;

			for (int i = 0; i < Main.item.Length; i++)
			{
				ref Item item = ref Main.item[i];
				if (item == null || item.IsAir || item.IsCoin() || !Handler.stacks.HasSpace(item) || Vector2.Distance(item.Center, player.Center) > maxRange)
				{
					if (item != null && PSItem.BlackHoleData.ContainsKey(i)) PSItem.BlackHoleData.Remove(i);
					continue;
				}

				if (!PSItem.BlackHoleData.ContainsKey(i)) PSItem.BlackHoleData.Add(i, (1, 0));

				if (PSItem.BlackHoleData[i].scale <= 0f)
				{
					if (!ItemLoader.OnPickup(item, player)) item = new Item();
					else if (ItemID.Sets.NebulaPickup[item.type])
					{
						int buffType = item.buffType;
						item = new Item();

						if (Main.netMode == 1) NetMessage.SendData(MessageID.NebulaLevelupRequest, -1, -1, null, player.whoAmI, buffType, player.Center.X, player.Center.Y);
						else player.NebulaLevelup(buffType);
					}
					else if (item.type == 58 || item.type == 1734 || item.type == 1867)
					{
						player.statLife += 20;
						if (Main.myPlayer == player.whoAmI) player.HealEffect(20);
						if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;

						item = new Item();
					}
					else if (item.type == 184 || item.type == 1735 || item.type == 1868)
					{
						player.statMana += 100;
						if (Main.myPlayer == player.whoAmI) player.ManaEffect(100);
						if (player.statMana > player.statManaMax2) player.statMana = player.statManaMax2;

						item = new Item();
					}
					else
					{
						for (int j = 0; j < Handler.Slots; j++)
						{
							item = Handler.InsertItem(j, item);

							if (item.IsAir || !item.active) break;
						}
					}

					if (Main.netMode == 1) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
				}
			}
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;

			scale *= 32f / 60f;
			float scaleMultiplier = !active ? 1f : (float)Math.Sin(angle).Remap(-1f, 1f, 0.4f, 1f);

			spriteBatch.Draw(Main.extraTexture[50], position + TheBlackHole.origin * scale, null, Color.White, angle, origin + TheBlackHole.origin, scale * scaleMultiplier, SpriteEffects.None, 0f);

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;

			scale *= 32f / 60f;
			float scaleMultiplier = !active ? 1f : (float)Math.Sin(angle).Remap(-1f, 1f, 0.4f, 1f);

			spriteBatch.Draw(Main.extraTexture[50], item.position - Main.screenPosition + origin, null, lightColor, angle + rotation, origin, scale * scaleMultiplier, SpriteEffects.None, 0f);

			return false;
		}

		public override TagCompound Save()
		{
			TagCompound tag = base.Save();
			tag["Active"] = active;
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			base.Load(tag);
			active = tag.GetBool("Active");
		}

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			writer.Write(active);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			base.NetRecieve(reader);
			active = reader.ReadBoolean();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.CrystalBall);
			recipe.AddIngredient(ItemID.Ectoplasm, 12);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}