using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items;
using PortableStorage.TileEntities;
using PortableStorage.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TheOneLibrary.Base;
using TheOneLibrary.Utility;

namespace PortableStorage.Tiles
{
	public class QEChest : BaseTile
	{
		public override string Texture => PortableStorage.TileTexturePath + "QEChest";

		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEQEChest>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Quantum Entangled Chest");
			AddMapEntry(Color.Purple, name);
		}

		public override void RightClick(int i, int j)
		{
			int ID = mod.GetID<TEQEChest>(i, j);
			if (ID == -1) return;

			TEQEChest qeChest = (TEQEChest)TileEntity.ByID[ID];

			Point16 topLeft = TheOneLibrary.Utility.Utility.TileEntityTopLeft(i, j);
			int realTileX = topLeft.X * 16;
			int realTileY = topLeft.Y * 16;
			Rectangle left = new Rectangle(realTileX + 2, realTileY + 4, 6, 10);
			Rectangle middle = new Rectangle(realTileX + 12, realTileY + 4, 8, 10);
			Rectangle right = new Rectangle(realTileX + 24, realTileY + 4, 6, 10);

			if (Main.LocalPlayer.HeldItem.type != mod.ItemType<QEBag>())
			{
				Frequency frequency = qeChest.frequency;
				bool handleFrequency = false;

				if (left.Contains(Main.MouseWorld) && qeChest.animState == 0)
				{
					frequency.colorLeft = Utility.ColorFromItem(frequency.colorLeft);
					handleFrequency = true;
				}
				else if (middle.Contains(Main.MouseWorld) && qeChest.animState == 0)
				{
					frequency.colorMiddle = Utility.ColorFromItem(frequency.colorMiddle);
					handleFrequency = true;
				}
				else if (right.Contains(Main.MouseWorld) && qeChest.animState == 0)
				{
					frequency.colorRight = Utility.ColorFromItem(frequency.colorRight);
					handleFrequency = true;
				}
				else
				{
					qeChest.opened = !qeChest.opened;
					PortableStorage.Instance.HandleUI<QEChestUI>(ID);

					Main.PlaySound(SoundID.DD2_EtherianPortalOpen.WithVolume(0.5f));
				}
				if (handleFrequency)
				{
					qeChest.frequency = frequency;
				}

				qeChest.SendUpdate();
			}
			else
			{
				Main.LocalPlayer.noThrow = 2;
				QEBag bag = (QEBag)Main.LocalPlayer.HeldItem.modItem;
				bag.frequency = qeChest.frequency;
				NetUtility.SyncItem(bag.item);
			}
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			Main.specX[nextSpecialDrawIndex] = i;
			Main.specY[nextSpecialDrawIndex] = j;
			nextSpecialDrawIndex++;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			int ID = mod.GetID<TEQEChest>(i, j);
			if (ID == -1) return;

			Tile tile = Main.tile[i, j];
			if (tile.TopLeft())
			{
				Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
				if (Main.drawToScreen) zero = Vector2.Zero;
				Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

				TEQEChest qeChest = (TEQEChest)TileEntity.ByID[ID];

				switch (qeChest.animState)
				{
					case 0:
						spriteBatch.Draw(PortableStorage.gemsSide[0], position + new Vector2(5, 9), new Rectangle(6 * (int)qeChest.frequency.colorLeft, 0, 6, 10), Color.White, 0f, new Vector2(3, 5), 1f, SpriteEffects.None, 0f);
						spriteBatch.Draw(PortableStorage.gemsMiddle[0], position + new Vector2(12, 4), new Rectangle(8 * (int)qeChest.frequency.colorMiddle, 0, 8, 10), Color.White);
						spriteBatch.Draw(PortableStorage.gemsSide[0], position + new Vector2(24, 4), new Rectangle(6 * (int)qeChest.frequency.colorRight, 0, 6, 10), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
						break;
					case 1:
						spriteBatch.Draw(PortableStorage.gemsSide[1], position + new Vector2(2, 4), new Rectangle(8 * (int)qeChest.frequency.colorLeft, 0, 8, 4), Color.White);
						spriteBatch.Draw(PortableStorage.gemsMiddle[1], position + new Vector2(14, 4), new Rectangle(4 * (int)qeChest.frequency.colorMiddle, 0, 4, 4), Color.White);
						spriteBatch.Draw(PortableStorage.gemsSide[1], position + new Vector2(22, 4), new Rectangle(8 * (int)qeChest.frequency.colorRight, 0, 8, 4), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
						break;
					case 2:
						spriteBatch.Draw(PortableStorage.gemsSide[2], position + new Vector2(4, 4), new Rectangle(6 * (int)qeChest.frequency.colorLeft, 0, 6, 2), Color.White);
						spriteBatch.Draw(PortableStorage.gemsMiddle[2], position + new Vector2(14, 4), new Rectangle(4 * (int)qeChest.frequency.colorMiddle, 0, 4, 2), Color.White);
						spriteBatch.Draw(PortableStorage.gemsSide[2], position + new Vector2(22, 4), new Rectangle(6 * (int)qeChest.frequency.colorRight, 0, 6, 2), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
						break;
				}
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			int ID = mod.GetID<TEQEChest>(i, j);
			if (ID != -1) PortableStorage.Instance.CloseUI(ID);

			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.QEChest>());
			mod.GetTileEntity<TEQEChest>().Kill(i, j);
		}
	}
}