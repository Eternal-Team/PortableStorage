using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utils;

namespace PortableStorage.UI
{
	public class DevNullUI : BaseUI, IContainerUI
	{
		public UIText textLabel = new UIText("/dev/null");

		public UITextButton buttonClose = new UITextButton("X", 4);

		public UIGrid gridItems = new UIGrid(7);

		public IContainer devNull;

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = 320;
			panelMain.Height.Pixels = 84;
			Vector2? position = ((DevNull)((IContainerItem)devNull).GetModItem()).UIPosition;
			if (position.HasValue)
			{
				panelMain.Left.Set(position.Value.X, 0f);
				panelMain.Top.Set(position.Value.Y, 0f);
			}
			else panelMain.Center();

			panelMain.SetPadding(0);
			panelMain.BackgroundColor = PanelColor;
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel.HAlign = 0.5f;
			textLabel.Top.Pixels = 8;
			panelMain.Append(textLabel);

			buttonClose.Width.Pixels = 24;
			buttonClose.Height.Pixels = 24;
			buttonClose.Left.Set(-28, 1);
			buttonClose.Top.Pixels = 8;
			buttonClose.OnClick += (evt, element) =>
			{
				PortableStorage.Instance.BagUI.Remove(((DevNull)((IContainerItem)devNull).GetModItem()).guid);
				Main.PlaySound(SoundID.Item59.WithVolume(0.5f));
			};
			panelMain.Append(buttonClose);

			gridItems.Width.Set(-16, 1);
			gridItems.Height.Set(-44, 1);
			gridItems.Left.Pixels = 8;
			gridItems.Top.Pixels = 36;
			gridItems.ListPadding = 4;
			gridItems.OverflowHidden = true;
			panelMain.Append(gridItems);
		}

		public override void Load()
		{
			DevNull devNull = (DevNull)((IContainerItem)this.devNull).GetModItem();
			for (int i = 0; i < devNull.GetItems().Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(devNull, i);
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

							gridItems.items.ForEach(x => ((UIContainerSlot)x).backgroundTexture = Main.inventoryBackTexture);
							slot.backgroundTexture = Main.inventoryBack15Texture;
						}
					}
				};
				slot.OnInteract += () =>
				{
					if (slot.Item.IsAir)
					{
						devNull.SetItem(-1);
						gridItems.items.ForEach(x => ((UIContainerSlot)x).backgroundTexture = Main.inventoryBackTexture);
					}
				};
				gridItems.Add(slot);
			}
		}

		public void SetContainer(IContainer container) => devNull = container;

		public IContainer GetContainer() => devNull;
	}
}