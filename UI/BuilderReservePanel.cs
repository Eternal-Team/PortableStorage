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
			Height = (40 + Container.Handler.Slots / 9 * 44, 0);
			this.Center();

			textLabel = new UIText(Container.DisplayName.GetTranslation())
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
			buttonClose.OnClick += (evt, element) => BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(Container);
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

			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i) {ShortStackSize = true};
				slot.ClickOverride += () =>
				{
					if (!slot.Item.IsAir && Main.keyState.IsKeyDown(Keys.LeftAlt))
					{
						if (Container.selectedIndex == slot.slot) Container.SetIndex(-1);
						else Container.SetIndex(slot.slot);

						return true;
					}

					return false;
				};
				slot.OnInteract += () =>
				{
					if (!Main.mouseItem.IsAir && !slot.Item.IsAir && Main.mouseItem.type != slot.Item.type) Container.SetIndex(-1);
					if (slot.Item.IsAir && slot.slot == Container.selectedIndex) Container.SetIndex(-1);
				};
				slot.OnPostDraw += spriteBatch =>
				{
					if (slot.IsMouseHovering && !slot.Item.IsAir && Main.keyState.IsKeyDown(Keys.LeftAlt)) BaseLibrary.Hooking.SetCursor("PortableStorage/Textures/Items/BuilderReserve");
				};
				gridItems.Add(slot);
			}

			RefreshTextures();
		}

		public void RefreshTextures()
		{
			foreach (UIContainerSlot slot in gridItems.items) slot.backgroundTexture = Container.selectedIndex == slot.slot ? Main.inventoryBack15Texture : Main.inventoryBackTexture;
		}
	}
}