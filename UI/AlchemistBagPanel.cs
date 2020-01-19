using BaseLibrary.UI;
using ContainerLibrary;
using PortableStorage.Items.Special;

namespace PortableStorage.UI
{
	public class AlchemistBagPanel : BaseBagPanel<AlchemistBag>
	{
		public AlchemistBagPanel(AlchemistBag bag) : base(bag)
		{
			Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
			Height.Pixels = 100 + (SlotSize + SlotMargin) * Container.Handler.Slots / 9;

			UIText textPotions = new UIText("Potions")
			{
				Y = { Pixels = 28 }
			};
			Add(textPotions);

			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = { Percent = 100 },
				Height = { Pixels = SlotSize * 2 + SlotMargin },
				Y = { Pixels = 56 },
				ItemMargin = SlotMargin
			};
			Add(gridItems);

			for (int i = 0; i < 18; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i) { Width = { Pixels = SlotSize }, Height = { Pixels = SlotSize } };
				gridItems.Add(slot);
			}

			UIText textIngredients = new UIText("Ingredients")
			{
				Y = { Pixels = 56 + 8 + SlotSize * 2 + SlotMargin }
			};
			Add(textIngredients);

			UIGrid<UIContainerSlot> gridIngredients = new UIGrid<UIContainerSlot>(9)
			{
				Width = { Percent = 100 },
				Height = { Pixels = (SlotSize + SlotMargin) * 7 - SlotMargin },
				Y = { Pixels = 56 + 8 + 20 + 8 + SlotSize * 2 + SlotMargin },
				ItemMargin = SlotMargin
			};
			Add(gridIngredients);

			for (int i = 18; i < 81; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i) { Width = { Pixels = SlotSize }, Height = { Pixels = SlotSize } };
				gridIngredients.Add(slot);
			}
		}
	}
}