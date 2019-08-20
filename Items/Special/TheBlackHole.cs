using BaseLibrary;
using BaseLibrary.Items;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Normal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Special
{
	public class TheBlackHole : BaseItem
	{
		public override string Texture => "PortableStorage/Textures/Items/TheBlackHole";

		private const int maxRange = 480;

		public bool active;

		public override ModItem Clone()
		{
			TheBlackHole clone = (TheBlackHole)base.Clone();
			clone.active = active;
			return clone;
		}

		public override ModItem Clone(Item item)
		{
			var clone = Clone();
			clone.SetValue("item", item);
			return clone;
		}

		public override void SetStaticDefaults()
		{
			ItemID.Sets.ItemNoGravity[item.type] = true;
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
				ref Item it = ref Main.item[i];

				var item1 = it;
				if (it == null || it.IsAir || Vector2.Distance(it.Center, player.Center) > maxRange || player.Inventory().OfType<BaseBag>().All(bag => !bag.Handler.HasSpace(item1)))
				{
					if (it != null && PSItem.BlackHoleData.ContainsKey(i)) PSItem.BlackHoleData.Remove(i);
					continue;
				}

				if (!PSItem.BlackHoleData.ContainsKey(i)) PSItem.BlackHoleData.Add(i, (1, 0));

				if (PSItem.BlackHoleData[i].scale <= 0f)
				{
					if (!ItemLoader.OnPickup(it, player)) it = new Item();
					else if (ItemID.Sets.NebulaPickup[it.type])
					{
						int buffType = it.buffType;
						it = new Item();

						if (Main.netMode == 1) NetMessage.SendData(MessageID.NebulaLevelupRequest, -1, -1, null, player.whoAmI, buffType, player.Center.X, player.Center.Y);
						else player.NebulaLevelup(buffType);
					}
					else if (it.type == 58 || it.type == 1734 || it.type == 1867)
					{
						player.statLife += 20;
						if (Main.myPlayer == player.whoAmI) player.HealEffect(20);
						if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;

						it = new Item();
					}
					else if (it.type == 184 || it.type == 1735 || it.type == 1868)
					{
						player.statMana += 100;
						if (Main.myPlayer == player.whoAmI) player.ManaEffect(100);
						if (player.statMana > player.statManaMax2) player.statMana = player.statManaMax2;

						it = new Item();
					}
					else
					{
						foreach (BaseBag bag in player.inventory.OfType<BaseBag>().OrderBy(bag => !bag.GetType().IsSubclassOf(typeof(BaseNormalBag))))
						{
							if (bag.Handler.HasSpace(it))
							{
								bag.Handler.InsertItem(ref it);
								if (it.IsAir || !it.active) break;
							}
						}
					}

					if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
				}
			}
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			spriteBatch.Draw(PortableStorage.textureBlackHole, position + new Vector2(16) * scale, null, drawColor, active ? -BaseLibrary.Utility.ToRadians(PortableStorage.BlackHoleAngle * 2) : 0f, new Vector2(16), scale, SpriteEffects.None, 0f);

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			spriteBatch.Draw(PortableStorage.textureBlackHole, item.position - Main.screenPosition + new Vector2(16), null, lightColor, (active ? -BaseLibrary.Utility.ToRadians(PortableStorage.BlackHoleAngle * 2) : 0f) + rotation, new Vector2(16), scale, SpriteEffects.None, 0f);

			return false;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Active"] = active
		};

		public override void Load(TagCompound tag) => active = tag.GetBool("Active");

		public override void NetSend(BinaryWriter writer) => writer.Write(active);

		public override void NetRecieve(BinaryReader reader) => active = reader.ReadBoolean();

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