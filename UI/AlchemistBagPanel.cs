using BaseLibrary.UI;
using PortableStorage.Items.SpecialBags;

namespace PortableStorage.UI
{
	public class AlchemistBagPanel : BaseBagPanel<AlchemistBag>
	{
		public AlchemistBagPanel(AlchemistBag bag) : base(bag)
		{
			Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
			Height.Pixels = 100 + (SlotSize + SlotMargin) * Container.GetItemStorage().Count / 9;

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
				Settings = { ItemMargin = SlotMargin }
			};
			Add(gridItems);

			for (int i = 0; i < AlchemistBag.PotionSlots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(Container.GetItemStorage(), i)
				{
					Width = { Pixels = SlotSize },
					Height = { Pixels = SlotSize }
				};
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
				Settings = { ItemMargin = SlotMargin }
			};
			Add(gridIngredients);

			for (int i = AlchemistBag.PotionSlots; i < AlchemistBag.PotionSlots + AlchemistBag.IngredientSlots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(Container.GetItemStorage(), i)
				{
					Width = { Pixels = SlotSize },
					Height = { Pixels = SlotSize }
				};
				gridIngredients.Add(slot);
			}
		}
	}
}