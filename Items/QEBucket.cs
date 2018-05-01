using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Global;
using PortableStorage.TileEntities;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Base.Items;
using TheOneLibrary.Fluid;
using TheOneLibrary.Storage;
using static TheOneLibrary.Utils.Utility;

namespace PortableStorage.Items
{
	public class QEBucket : BaseItem, IFluidContainerItem
	{
		public Frequency frequency;

		public override string Texture => PortableStorage.Textures.ItemPath + "QEBucket";

		public override ModItem Clone(Item item)
		{
			QEBucket clone = (QEBucket)base.Clone(item);
			clone.frequency = frequency;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quantum Entangled Bucket");
			Tooltip.SetDefault("Stores 16L of fluid\nRight-click on a Quantum Entangled Tank to link it");
		}

		public override void SetDefaults()
		{
			item.width = 30;
			item.height = 28;
			item.useStyle = 1;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 15;
			item.maxStack = 1;
			item.autoReuse = true;
			item.value = GetItemValue(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>()) + GetItemValue(ItemID.ShadowScale) * 25 + GetItemValue(ItemID.DemoniteBar) * 5;
			item.rare = 9;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool UseItem(Player player)
		{
			ModFluid fluid = GetFluid();
			if (player.altFunctionUse == 2)
			{
				if (fluid != null)
				{
					Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
					if ((!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]) && TileLoader.GetTile(tile.type)?.GetType().GetAttribute<BucketDisablePlacement>() == null)
					{
						if (tile.liquid == 0 || tile.liquidType() == fluid.type)
						{
							Main.PlaySound(19, (int)player.position.X, (int)player.position.Y);

							if (tile.liquid == 0) tile.liquidType(fluid.type);

							int volume = Math.Min(fluid.volume, 255 - tile.liquid);
							tile.liquid += (byte)volume;
							fluid.volume -= volume;
							if (fluid.volume == 0) fluid = null;

							WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);

							if (Main.netMode == 1) NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
						}
					}
				}
			}
			else
			{
				if (!Main.GamepadDisableCursorItemIcon)
				{
					player.showItemIcon = true;
					Main.ItemIconCacheUpdate(item.type);
				}

				Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
				if ((fluid == null || fluid.type == tile.liquidType()) && tile.liquid > 0 && TileLoader.GetTile(tile.type)?.GetType().GetAttribute<BucketDisablePickup>() == null)
				{
					Main.PlaySound(19, (int)player.position.X, (int)player.position.Y);

					if (fluid == null) fluid = TheOneLibrary.Utils.Utility.SetDefaults(tile.liquidType());

					int drain = Math.Min(tile.liquid, TEQETank.MaxVolume - fluid.volume);
					fluid.volume += drain;

					tile.liquid -= (byte)drain;

					if (tile.liquid <= 0)
					{
						tile.lava(false);
						tile.honey(false);
					}

					WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, false);
					if (Main.netMode == 1) NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
					else Liquid.AddWater(Player.tileTargetX, Player.tileTargetY);
				}
			}

			SetFluid(fluid);

			return true;
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			spriteBatch.Draw(PortableStorage.Textures.ringBig, position + new Vector2(4, 14) * scale, new Rectangle(0, 4 * (int)frequency.colorLeft, 22, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.Textures.ringBig, position + new Vector2(4, 18) * scale, new Rectangle(0, 4 * (int)frequency.colorMiddle, 22, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.Textures.ringSmall, position + new Vector2(6, 22) * scale, new Rectangle(0, 4 * (int)frequency.colorRight, 18, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 position = item.position - Main.screenPosition;
			Vector2 origin = new Vector2(15, 16);

			spriteBatch.Draw(PortableStorage.Textures.ringBig, position + origin, new Rectangle(0, 4 * (int)frequency.colorLeft, 22, 4), alphaColor, rotation, origin - new Vector2(4, 16), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.Textures.ringBig, position + origin, new Rectangle(0, 4 * (int)frequency.colorMiddle, 22, 4), alphaColor, rotation, origin - new Vector2(4, 20), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.Textures.ringSmall, position + origin, new Rectangle(0, 4 * (int)frequency.colorRight, 18, 4), alphaColor, rotation, origin - new Vector2(6, 24), scale, SpriteEffects.None, 0f);
		}

		public override TagCompound Save() => new TagCompound { ["Frequency"] = frequency };

		public override void Load(TagCompound tag)
		{
			frequency = tag.Get<Frequency>("Frequency");
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>());
			recipe.AddIngredient(ItemID.ShadowScale, 25);
			recipe.AddIngredient(ItemID.DemoniteBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>());
			recipe.AddIngredient(ItemID.TissueSample, 25);
			recipe.AddIngredient(ItemID.CrimtaneBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public List<ModFluid> GetFluids() => new List<ModFluid> { PSWorld.Instance.GetFluid(frequency) };

		public void SetFluid(ModFluid value, int slot = 0) => PSWorld.Instance.SetFluid(frequency, value);

		public ModFluid GetFluid(int slot = 0) => PSWorld.Instance.GetFluid(frequency);

		public int GetFluidCapacity(int slot = 0) => TEQETank.MaxVolume;

		public void Sync(int slot = 0) => Net.SendQEFluid(frequency);

		public ModItem GetItem() => this;
	}
}