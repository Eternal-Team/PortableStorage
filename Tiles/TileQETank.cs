//using BaseLibrary.Tiles;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using PortableStorage.Items;
//using PortableStorage.TileEntities;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ModLoader;
//using Terraria.ObjectData;

//namespace PortableStorage.Tiles
//{
//	public class TileQETank : BaseTile
//	{
//		public override void SetDefaults()
//		{
//			Main.tileSolidTop[Type] = true;
//			Main.tileFrameImportant[Type] = true;
//			Main.tileNoAttach[Type] = false;
//			Main.tileLavaDeath[Type] = false;
//			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
//			TileObjectData.newTile.Origin = new Point16(0, 1);
//			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
//			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEQETank>().Hook_AfterPlacement, -1, 0, false);
//			TileObjectData.addTile(Type);
//			disableSmartCursor = true;
//			mineResist = 5f;

//			ModTranslation name = CreateMapEntryName();
//			name.SetDefault("Quantum Entangled Tank");
//			AddMapEntry(Color.Purple, name);
//		}

//		public override void RightClick(int i, int j)
//		{
//			//TEQETank qeTank = mod.GetTileEntity<TEQETank>(i, j);
//			//if (qeTank == null) return;

//			//if (Main.keyState.IsKeyDown(Keys.RightShift))
//			//{
//			//	//qeTank.Handler.ExtractFluid(0, 100);

//			//	ModFluid f = FluidLoader.GetFluid<Water>().NewInstance();
//			//	f.volume = 255;

//			//	qeTank.Handler.InsertFluid(0, f);

//			//	return;
//			//}

//			//PortableStorage.Instance.PanelUI.UI.HandleUI(qeTank);
//		}

//		public const float MaxAngle = 0.8726646f;
//		public const float AngleStep = 0.5235988f;
//		public const float Radius = 56f;
//		public static Matrix translation = Matrix.CreateTranslation(-0.5f, -0.5f, 0f);

//		// return false, move to PostDrawTiles or another PostDraw method
//		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
//		{
//			//TEQETank qeTank = mod.GetTileEntity<TEQETank>(i, j);
//			//if (qeTank == null || !Main.tile[i, j].IsTopLeft() || TileEntity.ByID.Values.OfType<TEQETank>().Any(x => x != qeTank && x.inScreen && !x.hovered)) return true;

//			//Vector2 position = new Point16(i + 1, j + 1).ToScreenCoordinates();

//			//qeTank.hovered = new Rectangle(i * 16, j * 16, 32, 32).Contains(Main.MouseWorld);
//			//qeTank.inScreen = Main.MouseScreen.IsInCircularSector(position, Radius * qeTank.scale, -MaxAngle, MaxAngle) && Main.mouseY <= position.Y;

//			//if (!qeTank.inScreen && !qeTank.hovered && qeTank.scale > 0f) qeTank.scale -= 0.05f;
//			//else if (qeTank.scale < 1f) qeTank.scale += 0.05f;
//			//else if (qeTank.scale <= 0f) return true;

//			//spriteBatch.Draw(PortableStorage.QE_Glow, position.WithOffscreenRange(), null, Color.Purple, -MathHelper.PiOver2, new Vector2(10, 48), qeTank.scale, SpriteEffects.None, 0f);

//			//float scale = qeTank.scale * 1.5f;

//			//for (int k = -1; k <= 1; k++)
//			//{
//			//	Vector2 origin = position - new Vector2(0, 42 * qeTank.scale).RotatedBy(AngleStep * k);
//			//	Matrix transformation = translation * Matrix.CreateRotationZ(AngleStep * k) * Matrix.CreateTranslation(origin.X, origin.Y, 0);

//			//	spriteBatch.DrawWithTransformation(transformation * Utility.OffscreenMatrix, sb => sb.Draw(PortableStorage.QE_Gems, Vector2.Zero, new Rectangle(8 * (int)qeTank.frequency[k + 1], 0, 8, 10), Color.White, 0f, new Vector2(4, 5), scale, SpriteEffects.None, 0f));

//			//	if (Main.mouseRight && Main.MouseScreen.IsInPolygon4(Utility.CreatePolygon(new Vector2(8, 10), new Vector2(4, 5), scale).Transform(transformation)))
//			//	{
//			//		qeTank.frequency[k + 1] = Main.LocalPlayer.GetHeldItem().ColorFromItem(qeTank.frequency[k + 1]);
//			//	}
//			//}

//			return true;
//		}

//		public override void KillMultiTile(int i, int j, int frameX, int frameY)
//		{
//			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<ItemQETank>());
//			mod.GetTileEntity<TEQETank>().Kill(i, j);
//		}
//	}
//}

