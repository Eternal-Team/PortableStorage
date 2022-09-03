using BaseLibrary.UI;
using ContainerLibrary;
using PortableStorage.Items;
using Terraria;

namespace PortableStorage.UI;

public class AlchemistBagPanel : BaseBagPanel<AlchemistBag>
{
	private UIGrid<UIContainerSlot> gridItems;
	private UIGrid<UIContainerSlot> gridIngredients;
	
	public AlchemistBagPanel(AlchemistBag bag) : base(bag)
	{
		Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
		Height.Pixels = 100 + (SlotSize + SlotMargin) * (Container.GetItemStorage().Count + Container.IngredientStorage.Count) / 9;

		UIText textPotions = new UIText("Potions")
		{
			Y = { Pixels = 28 }
		};
		Add(textPotions);

		gridItems = new UIGrid<UIContainerSlot>(9)
		{
			Width = { Percent = 100 },
			Height = { Pixels = SlotSize * 2 + SlotMargin },
			Y = { Pixels = 56 },
			Settings = { ItemMargin = SlotMargin }
		};
		Add(gridItems);

		for (int i = 0; i < Container.GetItemStorage().Count; i++)
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

		gridIngredients = new UIGrid<UIContainerSlot>(9)
		{
			Width = { Percent = 100 },
			Height = { Pixels = (SlotSize + SlotMargin) * 7 - SlotMargin },
			Y = { Pixels = 56 + 8 + 20 + 8 + SlotSize * 2 + SlotMargin },
			Settings = { ItemMargin = SlotMargin }
		};
		Add(gridIngredients);

		for (int i = 0; i < Container.IngredientStorage.Count; i++)
		{
			UIContainerSlot slot = new UIContainerSlot(Container.IngredientStorage, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridIngredients.Add(slot);
		}
	}
	
	protected override void Activate()
	{
		gridItems.Clear();

		ItemStorage storage = BagSyncSystem.Instance.AllBags[Container.ID].GetItemStorage();
		Main.NewText(storage == Container.GetItemStorage());
		for (int i = 0; i < storage.Count; i++)
		{
			UIContainerSlot slot = new UIContainerSlot(storage, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridItems.Add(slot);
		}
		
		gridIngredients.Clear();
		// todo: pull from all bags
		for (int i = 0; i < Container.IngredientStorage.Count; i++)
		{
			UIContainerSlot slot = new UIContainerSlot(Container.IngredientStorage, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridIngredients.Add(slot);
		}
	}
}