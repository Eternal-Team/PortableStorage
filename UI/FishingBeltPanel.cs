using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items.Special;
using Terraria;
using Terraria.Localization;

namespace PortableStorage.UI
{
	public class FishingBeltPanel : BaseBagPanel<FishingBelt>
	{
		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (40 + Container.Handler.Slots / 9 * 44, 0);
			this.Center();

			textLabel = new UIText(Container.DisplayName.GetTranslation())
			{
				HAlign = 0.5f,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			Append(textLabel);

			UIButton buttonLootAll = new UIButton(PortableStorage.textureLootAll)
			{
				Size = new Vector2(20),
				HoverText = Language.GetText("LegacyInterface.29")
			};
			buttonLootAll.OnClick += (evt, element) => ItemUtility.LootAll(Container.Handler, Main.LocalPlayer);
			Append(buttonLootAll);

			UIButton buttonDepositAll = new UIButton(PortableStorage.textureDepositAll)
			{
				Size = new Vector2(20),
				Left = (28, 0),
				HoverText = Language.GetText("LegacyInterface.30")
			};
			buttonDepositAll.OnClick += (evt, element) => ItemUtility.DepositAll(Container.Handler, Main.LocalPlayer);
			Append(buttonDepositAll);

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
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i);
				gridItems.Add(slot);
			}
		}
	}
}