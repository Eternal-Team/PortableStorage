using BaseLibrary.UI;
using ContainerLibrary;
using PortableStorage.Items;

namespace PortableStorage.UI;

public class WiringBagPanel : BaseBagPanel<WiringBag>
{
	private UIGrid<UIContainerSlot> gridItems;

	public WiringBagPanel(WiringBag bag) : base(bag)
	{
		Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
		Height.Pixels = 40 + (SlotSize + SlotMargin) * Container.GetItemStorage().Count / 9;

		gridItems = new UIGrid<UIContainerSlot>(9)
		{
			Width = { Percent = 100 },
			Height = { Pixels = -28, Percent = 100 },
			Y = { Pixels = 28 },
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
	}

	protected override void Activate()
	{
		base.Activate();

		gridItems.Clear();

		ItemStorage storage = BagSyncSystem.Instance.AllBags[Container.GetID()].GetItemStorage();
		for (int i = 0; i < storage.Count; i++)
		{
			UIContainerSlot slot = new UIContainerSlot(storage, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridItems.Add(slot);
		}
	}
}