using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items.Special;
using Terraria;

namespace PortableStorage.UI
{
	public class BuilderReservePanel : BaseBagPanel<BuilderReserve>
	{
		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (40 + Bag.Handler.Slots / 9 * 44, 0);
			this.Center();

			textLabel = new UIText(Bag.DisplayName.GetTranslation())
			{
				HAlign = 0.5f
			};
			Append(textLabel);

			buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => PortableStorage.Instance.PanelUI.UI.CloseUI(Bag);
			Append(buttonClose);

			gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (-28, 1),
				Top = (28, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			BuilderReserve devNull = Bag;
			for (int i = 0; i < Bag.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Bag.Handler, i);
				slot.ClickOverride += () =>
				{
					if (!slot.Item.IsAir && Main.keyState.IsKeyDown(Keys.RightShift))
					{
						if (devNull.selectedIndex == slot.slot) devNull.SetIndex(-1);
						else devNull.SetIndex(slot.slot);

						return true;
					}

					return false;
				};
				slot.OnInteract += () =>
				{
					if (slot.Item.IsAir && slot.slot == devNull.selectedIndex) devNull.SetIndex(-1);
				};
				gridItems.Add(slot);
			}

			RefreshTextures();
		}

		public void RefreshTextures()
		{
			BuilderReserve devNull = Bag;
			if (devNull.selectedIndex >= 0)
			{
				foreach (UIContainerSlot slot in gridItems.items) slot.backgroundTexture = devNull.selectedIndex == slot.slot ? Main.inventoryBack15Texture : Main.inventoryBackTexture;
			}
		}
	}
}