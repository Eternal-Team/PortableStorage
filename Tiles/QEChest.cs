using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TheOneLibrary.Base;
using TheOneLibrary.Utility;

namespace PortableStorage.Tiles
{
	public class QEChest : BaseTile
	{
		public override bool Autoload(ref string name, ref string texture)
		{
			texture = PortableStorage.TileTexturePath + "QEChest";
			return base.Autoload(ref name, ref texture);
		}

		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEQEChest>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Quantum Entangled Chest");
			AddMapEntry(Color.Purple, name);
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
						spriteBatch.Draw(PortableStorage.Instance.gemsSide[0], position + new Vector2(2, 4), new Rectangle(6 * (int)qeChest.frequency.colorLeft, 0, 6, 10), Color.White);
						spriteBatch.Draw(PortableStorage.Instance.gemsMiddle[0], position + new Vector2(12, 4), new Rectangle(8 * (int)qeChest.frequency.colorMiddle, 0, 8, 10), Color.White);
						spriteBatch.Draw(PortableStorage.Instance.gemsSide[0], position + new Vector2(24, 4), new Rectangle(6 * (int)qeChest.frequency.colorRight, 0, 6, 10), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
						break;
					case 1:
						spriteBatch.Draw(PortableStorage.Instance.gemsSide[1], position + new Vector2(2, 4), new Rectangle(8 * (int)qeChest.frequency.colorLeft, 0, 8, 4), Color.White);
						spriteBatch.Draw(PortableStorage.Instance.gemsMiddle[1], position + new Vector2(14, 4), new Rectangle(4 * (int)qeChest.frequency.colorMiddle, 0, 4, 4), Color.White);
						spriteBatch.Draw(PortableStorage.Instance.gemsSide[1], position + new Vector2(22, 4), new Rectangle(8 * (int)qeChest.frequency.colorRight, 0, 8, 4), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
						break;
					case 2:
						spriteBatch.Draw(PortableStorage.Instance.gemsSide[2], position + new Vector2(4, 4), new Rectangle(6 * (int)qeChest.frequency.colorLeft, 0, 6, 2), Color.White);
						spriteBatch.Draw(PortableStorage.Instance.gemsMiddle[2], position + new Vector2(14, 4), new Rectangle(4 * (int)qeChest.frequency.colorMiddle, 0, 4, 2), Color.White);
						spriteBatch.Draw(PortableStorage.Instance.gemsSide[2], position + new Vector2(22, 4), new Rectangle(6 * (int)qeChest.frequency.colorRight, 0, 6, 2), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
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