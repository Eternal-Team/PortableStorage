using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items;
using PortableStorage.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;
using TheOneLibrary.Base;
using static TheOneLibrary.Utils.Utility;

namespace PortableStorage.Tiles
{
	public class DockingStation : BaseTile
	{
		public override string Texture => PortableStorage.TileTexturePath + "DockingStation";

		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] {16, 16};
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEDockingStation>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Docking Station");
			AddMapEntry(Color.Purple, name);
		}

		public override void RightClick(int i, int j)
		{
			int ID = mod.GetID<TEDockingStation>(i, j);
			if (ID == -1) return;

			TEDockingStation dockingStation = (TEDockingStation)TileEntity.ByID[ID];

			Player p = Main.LocalPlayer;

			if (p.inventory[p.selectedItem].modItem is BaseBag || dockingStation.Bag.modItem is BaseBag)
			{
				Item temp = HeldItem;
				p.inventory[p.selectedItem] = dockingStation.Bag;
				dockingStation.Bag = temp;

				SendTEData(dockingStation);
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
			int ID = mod.GetID<TEDockingStation>(i, j);
			if (ID == -1) return;

			Tile tile = Main.tile[i, j];
			if (tile.TopLeft())
			{
				Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
				if (Main.drawToScreen) zero = Vector2.Zero;

				TEDockingStation dockingStation = (TEDockingStation)TileEntity.ByID[ID];

				Item Item = dockingStation.Bag;

				if (!Item.IsAir)
				{
					Texture2D textureItem = Main.itemTexture[Item.type];
					Rectangle rect = Main.itemAnimations[Item.type] != null ? Main.itemAnimations[Item.type].GetFrame(textureItem) : textureItem.Frame();
					int width = rect.Width;
					int height = rect.Height;
					float scale = 1f;
					if (width > 20 || height > 20) scale = width > height ? 20f / width : 20f / height;
					scale *= Item.scale;
					Color colorLight = Lighting.GetColor(i, j);
					Color color = colorLight;
					float scale2 = 1f;
					ItemSlot.GetItemLight(ref color, ref scale2, Item);
					scale *= scale2;
					Main.spriteBatch.Draw(textureItem, new Vector2(i * 16 - (int)Main.screenPosition.X + 16, j * 16 - (int)Main.screenPosition.Y + 16) + zero, rect, color, 0f, new Vector2(width / 2f, height / 2f), scale, SpriteEffects.None, 0f);
					if (Item.color != default(Color)) Main.spriteBatch.Draw(textureItem, new Vector2(i * 16 - (int)Main.screenPosition.X + 16, j * 16 - (int)Main.screenPosition.Y + 16) + zero, rect, Item.GetColor(colorLight), 0f, new Vector2(width / 2f, height / 2f), scale, SpriteEffects.None, 0f);
				}
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			int ID = mod.GetID<TEDockingStation>(i, j);
			if (ID != -1) PortableStorage.Instance.CloseUI(ID);

			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.DockingStation>());
			mod.GetTileEntity<TEDockingStation>().Kill(i, j);
		}
	}
}