using BaseLibrary;
using BaseLibrary.Items;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Global;
using PortableStorage.Items.Normal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Utility = BaseLibrary.Utility;

namespace PortableStorage.Items.Special
{
	public class TheBlackHole : BaseItem
	{
		public override string Texture => "PortableStorage/Textures/Items/TheBlackHole";

		private const int maxRange = 160;

		public bool active;

		public override ModItem Clone()
		{
			TheBlackHole clone = (TheBlackHole)base.Clone();
			clone.active = active;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemNoGravity[item.type] = true;
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(1, 360));
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Purple;
			item.noUseGraphic = true;
			item.value = 55555 * 5;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(maxRange / 16)));
		}

		public override bool ConsumeItem(Player player) => false;
		
		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			active = !active;
		}

		public override void UpdateInventory(Player player)
		{
			if (!active) return;

			for (int i = 0; i < Main.item.Length; i++)
			{
				ref Item item = ref Main.item[i];

				var item1 = item;
				if (item == null || item.IsAir || Vector2.Distance(item.Center, player.Center) > maxRange || player.Inventory().OfType<BaseBag>().All(bag => !bag.Handler.HasSpace(item1)))
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
						foreach (BaseBag bag in player.inventory.OfType<BaseBag>().OrderBy(bag => !bag.GetType().IsSubclassOf(typeof(BaseNormalBag))))
						{
							if (bag.Handler.HasSpace(item))
							{
								bag.Handler.InsertItem(ref item);
								if (item.IsAir || !item.active) break;
							}
						}
					}

					if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
				}
			}
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			Texture2D texture = ModContent.GetTexture("PortableStorage/Textures/Items/TheBlackHole");

			spriteBatch.Draw(texture, position + new Vector2(16, 0) * scale, null, drawColor, active ? -Utility.ToRadians(Main.itemAnimations[item.type].Frame * 2) : 0f, new Vector2(16), scale, SpriteEffects.None, 0f);

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Texture2D texture = ModContent.GetTexture("PortableStorage/Textures/Items/TheBlackHole");

			spriteBatch.Draw(texture, item.position - Main.screenPosition + new Vector2(16, 16), null, lightColor, (active ? -Utility.ToRadians(Main.itemAnimations[item.type].Frame * 2) : 0f) + rotation, new Vector2(16), scale, SpriteEffects.None, 0f);

			return false;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Active"] = active
		};

		public override void Load(TagCompound tag)
		{
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