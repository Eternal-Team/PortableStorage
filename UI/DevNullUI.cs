using Microsoft.Xna.Framework.Input;
using PortableStorage.Items;
using Terraria;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utils;

namespace PortableStorage.UI
{
	public class DevNullUI : BaseBagUI
	{
		public DevNullUI()
		{
			gridItems = new UIGrid<UIContainerSlot>(7);
		}

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = 320;
			panelMain.Height.Pixels = 84;
			panelMain.Center();
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel = new UIText(() => "/dev/null");
			textLabel.HAlign = 0.5f;
			textLabel.Top.Pixels = 8;
			panelMain.Append(textLabel);

			buttonClose.Width.Pixels = 24;
			buttonClose.Height.Pixels = 24;
			buttonClose.Left.Set(-28, 1);
			buttonClose.Top.Pixels = 8;
			buttonClose.OnClick += (evt, element) => ((DevNull)bag).CloseUI();
			panelMain.Append(buttonClose);

			gridItems.Width.Set(-16, 1);
			gridItems.Height.Set(-44, 1);
			gridItems.Left.Pixels = 8;
			gridItems.Top.Pixels = 36;
			gridItems.ListPadding = 4;
			gridItems.OverflowHidden = true;
			panelMain.Append(gridItems);

			DevNull devNull = (DevNull)bag;
			foreach (UIContainerSlot slot in gridItems.items)
			{
				slot.backgroundTexture = devNull.selectedIndex == slot.slot ? Main.inventoryBack15Texture : Main.inventoryBackTexture;
				slot.CanInteract += (item, mouseItem) => (mouseItem.IsAir || mouseItem.createTile >= 0) && !Main.keyState.IsKeyDown(Keys.RightShift);
				slot.OnClick += (a, b) =>
				{
					if (!slot.Item.IsAir)
					{
						if (devNull.selectedIndex == slot.slot)
						{
							devNull.SetItem(-1);

							slot.backgroundTexture = Main.inventoryBackTexture;
						}
						else
						{
							devNull.SetItem(slot.slot);

							gridItems.items.ForEach(x => x.backgroundTexture = Main.inventoryBackTexture);
							slot.backgroundTexture = Main.inventoryBack15Texture;
						}
					}
				};
				slot.OnInteract += () =>
				{
					if (slot.Item.IsAir)
					{
						devNull.SetItem(-1);
						gridItems.items.ForEach(x => x.backgroundTexture = Main.inventoryBackTexture);
					}
				};
			}
		}
	}
}