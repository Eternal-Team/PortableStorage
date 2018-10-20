using BaseLibrary.Tiles;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Global;
using PortableStorage.Items;
using PortableStorage.TileEntities;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Utility = BaseLibrary.Utility.Utility;

namespace PortableStorage.Tiles
{
	public class TileQEChest : BaseTile
	{
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
			TEQEChest qeChest = mod.GetTileEntity<TEQEChest>(i, j);
			if (qeChest == null) return;

			PortableStorage.Instance.PanelUI.UI.HandleTE(qeChest);
		}

		public const float MaxAngle = 0.8726646f;
		public const float AngleStep = 0.5235988f;
		public const float Radius = 56f;
		public static Matrix translation = Matrix.CreateTranslation(-0.5f, -0.5f, 0f);

		// return false, move to PostDrawTiles or another PostDraw method
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TEQEChest qeChest = mod.GetTileEntity<TEQEChest>(i, j);
			if (qeChest == null || !Main.tile[i, j].IsTopLeft() || TileEntity.ByID.Values.OfType<TEQEChest>().Any(x => x != qeChest && x.inScreen && !x.hovered)) return true;

			Vector2 position = new Point16(i + 1, j + 1).ToScreenCoordinates();

			qeChest.hovered = new Rectangle(i * 16, j * 16, 32, 32).Contains(Main.MouseWorld);
			qeChest.inScreen = Main.MouseScreen.IsInCircularSector(position, Radius*qeChest.scale, -MaxAngle, MaxAngle) && Main.mouseY <= position.Y;

			if (!qeChest.inScreen && !qeChest.hovered && qeChest.scale > 0f) qeChest.scale -= 0.05f;
			else if (qeChest.scale < 1f) qeChest.scale += 0.05f;
			else if (qeChest.scale <= 0f) return true;

			spriteBatch.Draw(PortableStorage.QE_Glow, position.WithOffscreenRange(), null, Color.Purple, -MathHelper.PiOver2, new Vector2(10, 48), qeChest.scale, SpriteEffects.None, 0f);

			float scale = qeChest.scale * 1.5f;

			for (int k = -1; k <= 1; k++)
			{
				Vector2 origin = position - new Vector2(0, 42 * qeChest.scale).RotatedBy(AngleStep * k);
				Matrix transformation = translation * Matrix.CreateRotationZ(AngleStep * k) * Matrix.CreateTranslation(new Vector3(origin, 0f));

				spriteBatch.DrawWithTransformation(transformation * Matrix.CreateTranslation(Main.offScreenRange, Main.offScreenRange, 0f), sb => sb.Draw(PortableStorage.QE_Gems, Vector2.Zero, new Rectangle(8 * (int)qeChest.frequency[k + 1], 0, 8, 10), Color.White, 0f, new Vector2(4, 5), scale, SpriteEffects.None, 0f));

				if (Main.mouseRight && Main.MouseScreen.IsInPolygon4(Utility.CreatePolygon(new Vector2(8, 10), new Vector2(4, 5), scale).Transform(transformation)))
				{
					qeChest.frequency[k + 1] = Main.LocalPlayer.GetHeldItem().ColorFromItem(qeChest.frequency[k + 1]);
					qeChest.UI?.Repopulate();
				}
			}

			return true;
		}

		//public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		//{
		//	Main.specX[nextSpecialDrawIndex] = i;
		//	Main.specY[nextSpecialDrawIndex] = j;
		//	nextSpecialDrawIndex++;
		//}

		//public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		//{
		//	TEQEChest qeChest = mod.GetTileEntity<TEQEChest>(i, j);
		//	if (qeChest == null) return;

		//	Tile tile = Main.tile[i, j];
		//	if (tile.TopLeft())
		//	{
		//		//	Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		//		//	if (Main.drawToScreen) zero = Vector2.Zero;
		//		//	Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

		//		//	spriteBatch.Draw(PortableStorage.Textures.gemsSide[0], position + new Vector2(5, 9), new Rectangle(6 * (int)qeChest.frequency.colorLeft, 0, 6, 10), Color.White, 0f, new Vector2(3, 5), 1f, SpriteEffects.None, 0f);
		//		//	spriteBatch.Draw(PortableStorage.Textures.gemsMiddle[0], position + new Vector2(12, 4), new Rectangle(8 * (int)qeChest.frequency.colorMiddle, 0, 8, 10), Color.White);
		//		//	spriteBatch.Draw(PortableStorage.Textures.gemsSide[0], position + new Vector2(24, 4), new Rectangle(6 * (int)qeChest.frequency.colorRight, 0, 6, 10), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
		//		//}
		//	}
		//}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			TEQEChest qeChest = mod.GetTileEntity<TEQEChest>(i, j);
			PortableStorage.Instance.PanelUI.UI.CloseTE(qeChest);

			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<ItemQEChest>());
			qeChest.Kill(i, j);
		}
	}
}