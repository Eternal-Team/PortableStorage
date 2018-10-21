using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items.Bags;
using Terraria;

namespace PortableStorage.UI.Bags
{
	public class DevNullPanel : BaseBagPanel
	{
		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (40 + bag.Handler.Slots / 9 * 44, 0);
			this.Center();
			SetPadding(0);

			textLabel = new UIText(bag.DisplayName.TextFromTranslation())
			{
				Top = (8, 0),
				HAlign = 0.5f
			};
			Append(textLabel);

			buttonClose = new UITextButton("X", 0f)
			{
				Size = new Vector2(20),
				Left = (-28, 1),
				Top = (8, 0),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => PortableStorage.Instance.PanelUI.UI.CloseUI(bag);
			Append(buttonClose);

			gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (-16, 1),
				Height = (-44, 1),
				Left = (8, 0),
				Top = (36, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			DevNull devNull = (DevNull)bag;
			for (int i = 0; i < bag.Handler.stacks.Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag, i);
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
			DevNull devNull = (DevNull)bag;
			if (devNull.selectedIndex >= 0)
			{
				foreach (UIContainerSlot slot in gridItems.items) slot.backgroundTexture = devNull.selectedIndex == slot.slot ? Main.inventoryBack15Texture : Main.inventoryBackTexture;
			}
		}
	}
}