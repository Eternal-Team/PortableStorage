using BaseLibrary;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public class BagPanel : BaseUIPanel<Bag>
{
	public BagPanel(Bag container) : base(container)
	{
		Settings.Texture = ModContent.Request<Texture2D>(PortableStorage.Textures + "InventoryBG");
		Padding = new Padding(16);

		UIText text = new UIText(container.GetID().ToString()) {
			Size = new Dimension(0, 20, 100, 0),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				TextColor = Color.White,
				BorderColor = Color.Black
			}
		};
		base.Add(text);

		UIGrid<UIStorageSlot> gridItems = new UIGrid<UIStorageSlot>(9) {
			Size = new Dimension(0, -28, 100, 100),
			Position = Dimension.FromPixels(0, 28),
			Settings = {
				ItemMargin = 4
			}
		};
		base.Add(gridItems);

		for (int i = 0; i < container.Storage.Count; i++)
		{
			gridItems.Add(new UIStorageSlot(container.Storage, i) {
				Size = Dimension.FromPixels(52)
			});
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		DrawingUtility.DrawAchievementBorder(spriteBatch, Dimensions.TopLeft(), Dimensions.Size(), new Color(170, 170, 170, 255));
	}
}