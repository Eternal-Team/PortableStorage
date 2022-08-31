using System.Linq;
using BaseLibrary.UI;
using PortableStorage.Items;

namespace PortableStorage.UI;

public class NormalBagPanel : BaseBagPanel<BaseNormalBag>
{
	public NormalBagPanel(BaseNormalBag bag) : base(bag)
	{
		Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
		Height.Pixels = 40 + (SlotSize + SlotMargin) * Container.GetItemStorage().Count / 9;

		UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
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

	// protected override void Activate()
	// {
	// 	var grid = Children.FirstOrDefault(x=>x is UIGrid<UIContainerSlot>);
	// 	if (grid is null) return;
	// 	
	// 	grid.Clear();
	// 	
	// 	for (int i = 0; i < Container.GetItemStorage().Count; i++)
	// 	{
	// 		UIContainerSlot slot = new UIContainerSlot(Container.GetItemStorage(), i)
	// 		{
	// 			Width = { Pixels = SlotSize },
	// 			Height = { Pixels = SlotSize }
	// 		};
	// 		grid.Add(slot);
	// 	}
	// }
}