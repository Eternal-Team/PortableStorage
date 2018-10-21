using System.Linq;
using BaseLibrary.Tiles;
using BaseLibrary.Utility;
using FluidLibrary.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Global;
using PortableStorage.Items;
using PortableStorage.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Utility = BaseLibrary.Utility.Utility;

namespace PortableStorage.Tiles
{
	public class TileQETank : BaseTile
	{
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

		public override void RightClick(int i, int j)
		{
			TEQETank qeTank = mod.GetTileEntity<TEQETank>(i, j);
			if (qeTank == null) return;

			if (Main.keyState.IsKeyDown(Keys.RightShift))
			{
				PortableStorage.Instance.PanelUI.UI.HandleUI(qeTank);
				return;
			}

			//qeTank.Handler.ExtractFluid(0, 100);

			ModFluid f = FluidLoader.GetFluid<Water>().NewInstance();
			f.volume = 255;

			qeTank.Handler.InsertFluid(0, f);
		}

		public const float MaxAngle = 0.8726646f;
		public const float AngleStep = 0.5235988f;
		public const float Radius = 56f;
		public static Matrix translation = Matrix.CreateTranslation(-0.5f, -0.5f, 0f);

		// return false, move to PostDrawTiles or another PostDraw method
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TEQETank qeTank = mod.GetTileEntity<TEQETank>(i, j);
			if (qeTank == null || !Main.tile[i, j].IsTopLeft() || TileEntity.ByID.Values.OfType<TEQETank>().Any(x => x != qeTank && x.inScreen && !x.hovered)) return true;

			Vector2 position = new Point16(i + 1, j + 1).ToScreenCoordinates();

			qeTank.hovered = new Rectangle(i * 16, j * 16, 32, 32).Contains(Main.MouseWorld);
			qeTank.inScreen = Main.MouseScreen.IsInCircularSector(position, Radius * qeTank.scale, -MaxAngle, MaxAngle) && Main.mouseY <= position.Y;

			if (!qeTank.inScreen && !qeTank.hovered && qeTank.scale > 0f) qeTank.scale -= 0.05f;
			else if (qeTank.scale < 1f) qeTank.scale += 0.05f;
			else if (qeTank.scale <= 0f) return true;

			spriteBatch.Draw(PortableStorage.QE_Glow, position.WithOffscreenRange(), null, Color.Purple, -MathHelper.PiOver2, new Vector2(10, 48), qeTank.scale, SpriteEffects.None, 0f);

			float scale = qeTank.scale * 1.5f;

			for (int k = -1; k <= 1; k++)
			{
				Vector2 origin = position - new Vector2(0, 42 * qeTank.scale).RotatedBy(AngleStep * k);
				Matrix transformation = translation * Matrix.CreateRotationZ(AngleStep * k) * Matrix.CreateTranslation(origin.X, origin.Y, 0);

				spriteBatch.DrawWithTransformation(transformation * Utility.OffscreenMatrix, sb => sb.Draw(PortableStorage.QE_Gems, Vector2.Zero, new Rectangle(8 * (int)qeTank.frequency[k + 1], 0, 8, 10), Color.White, 0f, new Vector2(4, 5), scale, SpriteEffects.None, 0f));

				if (Main.mouseRight && Main.MouseScreen.IsInPolygon4(Utility.CreatePolygon(new Vector2(8, 10), new Vector2(4, 5), scale).Transform(transformation)))
				{
					qeTank.frequency[k + 1] = Main.LocalPlayer.GetHeldItem().ColorFromItem(qeTank.frequency[k + 1]);
					//qeChest.UI?.Repopulate();
				}
			}

			return true;
		}

		//public override void RightClickCont(int i, int j)
		//{
		//	TEQETank qeTank = mod.GetTileEntity<TEQETank>(i, j);
		//	if (qeTank == null) return;

		//	Point16 topLeft = TileEntityTopLeft(i, j);
		//	int realTileX = topLeft.X * 16;
		//	int realTileY = topLeft.Y * 16;

		//	Rectangle left = new Rectangle(realTileX + 2, realTileY + 2, 4, 4);
		//	Rectangle middle = new Rectangle(realTileX + 14, realTileY + 27, 4, 4);
		//	Rectangle right = new Rectangle(realTileX + 26, realTileY + 2, 4, 4);

		//	if (Main.LocalPlayer.HeldItem.type == mod.ItemType<QEBucket>()) // copy frequency
		//	{
		//		Main.LocalPlayer.noThrow = 2;
		//		QEBucket bucket = (QEBucket)Main.LocalPlayer.HeldItem.modItem;
		//		bucket.frequency = qeTank.frequency;
		//		SyncItem(bucket.item);
		//	}
		//	else if (Main.LocalPlayer.HeldItem.modItem is IFluidContainerItem) // insert fluid
		//	{
		//		IFluidContainerItem fluidContainer = (IFluidContainerItem)Main.LocalPlayer.HeldItem.modItem;

		//		ModFluid fluid = qeTank.GetFluid();
		//		ModFluid itemFluid = fluidContainer.GetFluid();

		//		if (itemFluid != null)
		//		{
		//			if (fluid == null)
		//			{
		//				fluid = TheOneLibrary.Utils.Utility.SetDefaults(itemFluid.type);

		//				int volume = Math.Min(itemFluid.volume, TEQETank.MaxVolume - fluid.volume);
		//				fluid.volume += volume;
		//				itemFluid.volume -= volume;

		//				if (itemFluid.volume <= 0) itemFluid = null;
		//			}
		//			else if (fluid.type == itemFluid.type && fluid.volume < TEQETank.MaxVolume)
		//			{
		//				int volume = Math.Min(itemFluid.volume, TEQETank.MaxVolume - fluid.volume);
		//				fluid.volume += volume;
		//				itemFluid.volume -= volume;

		//				if (itemFluid.volume <= 0) itemFluid = null;
		//			}
		//		}

		//		fluidContainer.SetFluid(itemFluid);
		//		fluidContainer.Sync();

		//		qeTank.SetFluid(fluid);
		//		Net.SendQEFluid(qeTank.frequency);
		//	}
		//	else // general click
		//	{
		//		Frequency frequency = qeTank.frequency;
		//		bool handleFrequency = false;

		//		if (left.Contains(Main.MouseWorld))
		//		{
		//			frequency.colorLeft = Main.LocalPlayer.GetHeldItem().ColorFromItem(frequency.colorLeft);
		//			handleFrequency = true;
		//		}
		//		else if (middle.Contains(Main.MouseWorld))
		//		{
		//			frequency.colorMiddle = Main.LocalPlayer.GetHeldItem().ColorFromItem(frequency.colorMiddle);
		//			handleFrequency = true;
		//		}
		//		else if (right.Contains(Main.MouseWorld))
		//		{
		//			frequency.colorRight = Main.LocalPlayer.GetHeldItem().ColorFromItem(frequency.colorRight);
		//			handleFrequency = true;
		//		}

		//		if (handleFrequency)
		//		{
		//			qeTank.frequency = frequency;
		//			SendTEData(qeTank);
		//		}
		//	}
		//}

		//public override void LeftClickCont(int i, int j)
		//{
		//	TEQETank qeTank = mod.GetTileEntity<TEQETank>(i, j);
		//	if (qeTank == null) return;

		//	if (Main.LocalPlayer.HeldItem.modItem is IFluidContainerItem) // extract fluid
		//	{
		//		IFluidContainerItem fluidContainer = (IFluidContainerItem)Main.LocalPlayer.HeldItem.modItem;

		//		ModFluid fluid = qeTank.GetFluid();
		//		ModFluid itemFluid = fluidContainer.GetFluid();
		//		int capacity = fluidContainer.GetFluidCapacity();

		//		if (fluid != null)
		//		{
		//			if (itemFluid == null)
		//			{
		//				itemFluid = TheOneLibrary.Utils.Utility.SetDefaults(fluid.type);

		//				int volume = Math.Min(fluid.volume, capacity - itemFluid.volume);
		//				itemFluid.volume += volume;
		//				fluid.volume -= volume;

		//				if (fluid.volume <= 0) fluid = null;
		//			}
		//			else if (fluid.type == itemFluid.type && itemFluid.volume < capacity)
		//			{
		//				int volume = Math.Min(fluid.volume, capacity - itemFluid.volume);
		//				itemFluid.volume += volume;
		//				fluid.volume -= volume;

		//				if (fluid.volume <= 0) fluid = null;
		//			}
		//		}

		//		fluidContainer.SetFluid(itemFluid);
		//		fluidContainer.Sync();

		//		qeTank.SetFluid(fluid);
		//		Net.SendQEFluid(qeTank.frequency);
		//	}
		//}

		//public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		//{
		//	int ID = mod.GetID<TEQETank>(i, j);
		//	if (ID == -1) return false;

		//	Tile tile = Main.tile[i, j];
		//	if (tile.TopLeft())
		//	{
		//		Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		//		if (Main.drawToScreen) zero = Vector2.Zero;
		//		Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

		//		TEQETank qeTank = (TEQETank)TileEntity.ByID[ID];
		//		ModFluid fluid = qeTank.GetFluid();

		//		if (fluid != null)
		//			spriteBatch.Draw(ModLoader.GetTexture(FluidLoader.GetFluid(fluid.Name).Texture), new Rectangle((int)position.X + 6, (int)(position.Y + 6 + (20 - 20 * (fluid.volume / (float)TEQETank.MaxVolume))), 20, (int)(20 * (fluid.volume / (float)TEQETank.MaxVolume))), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		//	}

		//	return true;
		//}

		//public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		//{
		//	Main.specX[nextSpecialDrawIndex] = i;
		//	Main.specY[nextSpecialDrawIndex] = j;
		//	nextSpecialDrawIndex++;
		//}

		//public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		//{
		//	int ID = mod.GetID<TEQETank>(i, j);
		//	if (ID == -1) return;

		//	Tile tile = Main.tile[i, j];
		//	if (tile.TopLeft())
		//	{
		//		Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		//		if (Main.drawToScreen) zero = Vector2.Zero;
		//		Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

		//		TEQETank qeTank = (TEQETank)TileEntity.ByID[ID];

		//		spriteBatch.Draw(PortableStorage.Textures.gemsMiddle[1], position + new Vector2(2, 2), new Rectangle(4 * (int)qeTank.frequency.colorLeft, 0, 4, 4), Color.White);
		//		spriteBatch.Draw(PortableStorage.Textures.gemsMiddle[1], position + new Vector2(14, 27), new Rectangle(4 * (int)qeTank.frequency.colorMiddle, 0, 4, 4), Color.White);
		//		spriteBatch.Draw(PortableStorage.Textures.gemsMiddle[1], position + new Vector2(26, 2), new Rectangle(4 * (int)qeTank.frequency.colorRight, 0, 4, 4), Color.White);
		//	}
		//}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<ItemQETank>());
			mod.GetTileEntity<TEQETank>().Kill(i, j);
		}
	}
}