using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items;
using PortableStorage.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TheOneLibrary.Base;
using TheOneLibrary.Fluid;
using TheOneLibrary.Storage;
using static TheOneLibrary.Utils.Utility;

namespace PortableStorage.Tiles
{
	[BucketDisablePlacement, BucketDisablePickup]
	public class QETank : BaseTile
	{
		public override string Texture => PortableStorage.Textures.TilePath + "QETank";

		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEQETank>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;
			mineResist = 5f;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Quantum Entangled Tank");
			AddMapEntry(Color.Purple, name);
		}

		public override void RightClickCont(int i, int j)
		{
			int ID = mod.GetID<TEQETank>(i, j);
			if (ID == -1) return;

			TEQETank qeTank = (TEQETank)TileEntity.ByID[ID];

			Point16 topLeft = TileEntityTopLeft(i, j);
			int realTileX = topLeft.X * 16;
			int realTileY = topLeft.Y * 16;

			Rectangle left = new Rectangle(realTileX + 2, realTileY + 2, 4, 4);
			Rectangle middle = new Rectangle(realTileX + 14, realTileY + 27, 4, 4);
			Rectangle right = new Rectangle(realTileX + 26, realTileY + 2, 4, 4);

			if (Main.LocalPlayer.HeldItem.type == mod.ItemType<QEBucket>()) // copy frequency
			{
				Main.LocalPlayer.noThrow = 2;
				QEBucket bucket = (QEBucket)Main.LocalPlayer.HeldItem.modItem;
				bucket.frequency = qeTank.frequency;
				SyncItem(bucket.item);
			}
			else if (Main.LocalPlayer.HeldItem.modItem is IFluidContainerItem) // insert fluid
			{
				IFluidContainerItem fluidContainer = (IFluidContainerItem)Main.LocalPlayer.HeldItem.modItem;

				ModFluid fluid = qeTank.GetFluid();
				ModFluid itemFluid = fluidContainer.GetFluid();

				if (itemFluid != null)
				{
					if (fluid == null)
					{
						fluid = TheOneLibrary.Utils.Utility.SetDefaults(itemFluid.type);

						int volume = Math.Min(itemFluid.volume, TEQETank.MaxVolume - fluid.volume);
						fluid.volume += volume;
						itemFluid.volume -= volume;

						if (itemFluid.volume <= 0) itemFluid = null;
					}
					else if (fluid.type == itemFluid.type && fluid.volume < TEQETank.MaxVolume)
					{
						int volume = Math.Min(itemFluid.volume, TEQETank.MaxVolume - fluid.volume);
						fluid.volume += volume;
						itemFluid.volume -= volume;

						if (itemFluid.volume <= 0) itemFluid = null;
					}
				}

				fluidContainer.SetFluid(itemFluid);
				fluidContainer.Sync();

				qeTank.SetFluid(fluid);
				Net.SendQEFluid(qeTank.frequency);
			}
			else // general click
			{
				Frequency frequency = qeTank.frequency;
				bool handleFrequency = false;

				if (left.Contains(Main.MouseWorld))
				{
					frequency.colorLeft = Utility.ColorFromItem(frequency.colorLeft);
					handleFrequency = true;
				}
				else if (middle.Contains(Main.MouseWorld))
				{
					frequency.colorMiddle = Utility.ColorFromItem(frequency.colorMiddle);
					handleFrequency = true;
				}
				else if (right.Contains(Main.MouseWorld))
				{
					frequency.colorRight = Utility.ColorFromItem(frequency.colorRight);
					handleFrequency = true;
				}

				if (handleFrequency)
				{
					qeTank.frequency = frequency;
					SendTEData(qeTank);
				}
			}
		}

		public override void LeftClickCont(int i, int j)
		{
			int ID = mod.GetID<TEQETank>(i, j);
			if (ID == -1) return;

			TEQETank qeTank = (TEQETank)TileEntity.ByID[ID];

			if (Main.LocalPlayer.HeldItem.modItem is IFluidContainerItem) // extract fluid
			{
				IFluidContainerItem fluidContainer = (IFluidContainerItem)Main.LocalPlayer.HeldItem.modItem;

				ModFluid fluid = qeTank.GetFluid();
				ModFluid itemFluid = fluidContainer.GetFluid();
				int capacity = fluidContainer.GetFluidCapacity();

				if (fluid != null)
				{
					if (itemFluid == null)
					{
						itemFluid = TheOneLibrary.Utils.Utility.SetDefaults(fluid.type);

						int volume = Math.Min(fluid.volume, capacity - itemFluid.volume);
						itemFluid.volume += volume;
						fluid.volume -= volume;

						if (fluid.volume <= 0) fluid = null;
					}
					else if (fluid.type == itemFluid.type && itemFluid.volume < capacity)
					{
						int volume = Math.Min(fluid.volume, capacity - itemFluid.volume);
						itemFluid.volume += volume;
						fluid.volume -= volume;

						if (fluid.volume <= 0) fluid = null;
					}
				}

				fluidContainer.SetFluid(itemFluid);
				fluidContainer.Sync();

				qeTank.SetFluid(fluid);
				Net.SendQEFluid(qeTank.frequency);
			}
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			int ID = mod.GetID<TEQETank>(i, j);
			if (ID == -1) return false;

			Tile tile = Main.tile[i, j];
			if (tile.TopLeft())
			{
				Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
				if (Main.drawToScreen) zero = Vector2.Zero;
				Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

				TEQETank qeTank = (TEQETank)TileEntity.ByID[ID];
				ModFluid fluid = qeTank.GetFluid();

				if (fluid != null)
					spriteBatch.Draw(ModLoader.GetTexture(FluidLoader.GetFluid(fluid.Name).Texture), new Rectangle((int)position.X + 6, (int)(position.Y + 6 + (20 - 20 * (fluid.volume / (float)TEQETank.MaxVolume))), 20, (int)(20 * (fluid.volume / (float)TEQETank.MaxVolume))), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			}

			return true;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			Main.specX[nextSpecialDrawIndex] = i;
			Main.specY[nextSpecialDrawIndex] = j;
			nextSpecialDrawIndex++;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			int ID = mod.GetID<TEQETank>(i, j);
			if (ID == -1) return;

			Tile tile = Main.tile[i, j];
			if (tile.TopLeft())
			{
				Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
				if (Main.drawToScreen) zero = Vector2.Zero;
				Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

				TEQETank qeTank = (TEQETank)TileEntity.ByID[ID];

				spriteBatch.Draw(PortableStorage.Textures.gemsMiddle[1], position + new Vector2(2, 2), new Rectangle(4 * (int)qeTank.frequency.colorLeft, 0, 4, 4), Color.White);
				spriteBatch.Draw(PortableStorage.Textures.gemsMiddle[1], position + new Vector2(14, 27), new Rectangle(4 * (int)qeTank.frequency.colorMiddle, 0, 4, 4), Color.White);
				spriteBatch.Draw(PortableStorage.Textures.gemsMiddle[1], position + new Vector2(26, 2), new Rectangle(4 * (int)qeTank.frequency.colorRight, 0, 4, 4), Color.White);
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.QETank>());
			mod.GetTileEntity<TEQETank>().Kill(i, j);
		}
	}
}