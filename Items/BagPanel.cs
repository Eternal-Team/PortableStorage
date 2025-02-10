using BaseLibrary;
using BaseLibrary.Input;
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
		Settings.CaptureAllInputs = true;
		Settings.Texture = ModContent.Request<Texture2D>(PortableStorage.Textures + "InventoryBG");

		Size = Dimension.FromPixels(532, 108);
		Padding = new Padding(10);

		UIText text = new UIText("Bilbo Baggins") {
			Size = new Dimension(0, 20, 100, 0),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				TextColor = new Color(40, 25, 14),
				BorderColor = Color.Transparent
			}
		};
		base.Add(text);

		UITexture buttonClose = new UITexture(ModContent.Request<Texture2D>(PortableStorage.Textures + "CloseButton")) {
			Position = Dimension.FromPercent(100, 0),
			Size = Dimension.FromPixels(20),
			Margin = new Margin(0,0,6,0),
			HoverText = "Close"
		};
		buttonClose.OnClick += args => {
			if (args.Button != MouseButton.Left) return;

			WindowUI.Instance?.CloseUI(container);
		};
		base.Add(buttonClose);

		UIGrid<UIStorageSlot> gridItems = new UIGrid<UIStorageSlot>(9) {
			Size = new Dimension(500, -28, 0, 100),
			Position = new Dimension(0, 28, 50, 0),
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
		if (IsMouseHovering)
		{
			Main.LocalPlayer.mouseInterface = true;
			Main.instance.SetMouseNPC(-1, -1);
			Main.LocalPlayer.cursorItemIconEnabled = false;
			Main.LocalPlayer.cursorItemIconText = string.Empty;
			Main.signHover = -1;
			Main.ItemIconCacheUpdate(0);
			Main.mouseText = true;
			Main.HoverItem = new Item();
			Main.hoverItemName = "";
		}

		if (Settings.Texture is not null)
		{
			spriteBatch.Draw(Settings.Texture.Value, Dimensions.TopLeft().OffsetBy(6, 6), new Rectangle(0, 0, Dimensions.Width - 12, Dimensions.Height - 12), Color.White);
		}

		DrawingUtility.DrawAchievementBorder(spriteBatch, Dimensions.TopLeft(), Dimensions.Size(), new Color(170, 170, 170, 255));
	}
}