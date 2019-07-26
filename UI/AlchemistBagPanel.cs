using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items.Special;
using Terraria;
using Terraria.Localization;

namespace PortableStorage.UI
{
	public class AlchemistBagPanel : BaseBagPanel<AlchemistBag>
	{
		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (100 + (Container.Handler.Slots + Container.HandlerIngredients.Slots) / 9 * 44, 0);
			this.Center();

			textLabel = new UIText(Container.DisplayName.GetTranslation())
			{
				HAlign = 0.5f
			};
			Append(textLabel);

			UIButton buttonLootAll = new UIButton(PortableStorage.textureLootAll)
			{
				Size = new Vector2(20)
			};
			buttonLootAll.OnClick += (evt, element) =>
			{
				ItemUtility.LootAll(Container.HandlerIngredients, Main.LocalPlayer);
				ItemUtility.LootAll(Container.Handler, Main.LocalPlayer);
			};
			buttonLootAll.GetHoverText += () => Language.GetText("LegacyInterface.29").ToString();
			Append(buttonLootAll);

			UIButton buttonDepositAll = new UIButton(PortableStorage.textureDepositAll)
			{
				Size = new Vector2(20),
				Left = (28, 0)
			};
			buttonDepositAll.OnClick += (evt, element) =>
			{
				ItemUtility.DepositAll(Container.Handler, Main.LocalPlayer);
				ItemUtility.DepositAll(Container.HandlerIngredients, Main.LocalPlayer);
			};
			buttonDepositAll.GetHoverText += () => Language.GetText("LegacyInterface.30").ToString();
			Append(buttonDepositAll);

			buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(Container);
			Append(buttonClose);

			UIText textPotions = new UIText("Potions")
			{
				Top = (28, 0)
			};
			Append(textPotions);

			gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (84, 0),
				Top = (56, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i);
				gridItems.Add(slot);
			}

			UIText textIngredients = new UIText("Ingredients")
			{
				Top = (148, 0)
			};
			Append(textIngredients);

			UIGrid<UIContainerSlot> gridIngredients = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (304, 0),
				Top = (176, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridIngredients);

			for (int i = 0; i < Container.HandlerIngredients.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.HandlerIngredients, i);
				gridIngredients.Add(slot);
			}
		}
	}
}