using BaseLibrary.Tiles;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Global;
using PortableStorage.Items;
using PortableStorage.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace PortableStorage.Tiles
{
	public class TileQEChest : BaseTile
	{
		[PathOverride("PortableStorage/Textures/Tiles/QEChest_Glow")]
		public static Texture2D QEChest_Glow { get; set; }

		[PathOverride("PortableStorage/Textures/Tiles/GemMiddle_0")]
		public static Texture2D QEChest_Gems { get; set; }

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

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TEQEChest qeChest = mod.GetTileEntity<TEQEChest>(i, j);
			if (qeChest == null || !Main.tile[i, j].IsTopLeft()) return true;

			Vector2 position = new Point16(i + 1, j + 1).ToScreenCoordinates();

			qeChest.hovered = new Rectangle(i * 16, j * 16, 32, 32).Contains(Main.MouseWorld);
			qeChest.inScreen = Main.MouseScreen.IsInCircularSector(position - new Vector2(Main.offScreenRange), Radius, -MaxAngle, MaxAngle) && Main.mouseY <= position.Y - Main.offScreenRange;

			if (!qeChest.inScreen && !qeChest.hovered && qeChest.scale > 0f) qeChest.scale -= 0.05f;
			else if (qeChest.scale < 1f) qeChest.scale += 0.05f;

			if (qeChest.scale < 0f) return true;

			spriteBatch.Draw(QEChest_Glow, position, null, Color.Purple, -MathHelper.PiOver2, new Vector2(10, 48), qeChest.scale, SpriteEffects.None, 0f);

			float scale = qeChest.scale * 1.5f;

			for (int k = -1; k <= 1; k++)
			{
				spriteBatch.Draw(QEChest_Gems, position + new Vector2(0, -42 * qeChest.scale).RotatedBy(AngleStep * k), new Rectangle(8 * (int)qeChest.frequency[k + 1], 0, 8, 10), Color.White, AngleStep * k, new Vector2(4, 5), scale, SpriteEffects.None, 0f);

				Vector2 origin = position - new Vector2(Main.offScreenRange) - new Vector2(0, 42 * qeChest.scale).RotatedBy(AngleStep * k);

				Vector2 p1 = new Vector2(-4, -5).RotatedBy(AngleStep * k) * scale + origin;
				Vector2 p2 = new Vector2(4, -5).RotatedBy(AngleStep * k) * scale + origin;
				Vector2 p3 = new Vector2(4, 5).RotatedBy(AngleStep * k) * scale + origin;
				Vector2 p4 = new Vector2(-4, 5).RotatedBy(AngleStep * k) * scale + origin;

				if (Main.mouseRight && Main.MouseScreen.IsInPolygon4(p1, p2, p3, p4))
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