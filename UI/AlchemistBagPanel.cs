using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items.Special;

namespace PortableStorage.UI
{
	public class AlchemistBagPanel : BaseBagPanel<AlchemistBag>
	{
		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (100 + (Bag.Handler.Slots + Bag.HandlerIngredients.Slots) / 9 * 44, 0);
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

			UIText textPotions = new UIText("Potions")
			{
				Top = (28, 0)
			};
			Append(textPotions);

			gridPotions = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (84, 0),
				Top = (56, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridPotions);

			for (int i = 0; i < Bag.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Bag.Handler, i);
				gridPotions.Add(slot);
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

			for (int i = 0; i < Bag.HandlerIngredients.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Bag.HandlerIngredients, i);
				gridIngredients.Add(slot);
			}
		}
	}
}