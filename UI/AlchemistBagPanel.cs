using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.Items.Special;

namespace PortableStorage.UI
{
	public class AlchemistBagPanel : BaseBagPanel<AlchemistBag>
	{
		public override void OnInitialize()
		{
			base.OnInitialize();

			Width = (12 + (SlotSize + Padding) * 9, 0);
			Height = (100 + (SlotSize + Padding) * Container.Handler.Slots / 9, 0);
			this.Center();

			UIText textPotions = new UIText("Potions")
			{
				Top = (28, 0)
			};
			Append(textPotions);

			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (SlotSize * 2 + Padding, 0),
				Top = (56, 0),
				OverflowHidden = true,
				ListPadding = Padding
			};
			Append(gridItems);

			for (int i = 0; i < 18; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i) { Width = (SlotSize, 0), Height = (SlotSize, 0) };
				gridItems.Add(slot);
			}

			UIText textIngredients = new UIText("Ingredients")
			{
				Top = (56 + 8 + SlotSize * 2 + Padding, 0)
			};
			Append(textIngredients);

			UIGrid<UIContainerSlot> gridIngredients = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = ((SlotSize + Padding) * 7 - Padding, 0),
				Top = (56 + 8 + 20 + 8 + SlotSize * 2 + Padding, 0),
				OverflowHidden = true,
				ListPadding = Padding
			};
			Append(gridIngredients);

			for (int i = 18; i < 81; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i) { Width = (SlotSize, 0), Height = (SlotSize, 0) };
				gridIngredients.Add(slot);
			}
		}
	}
}