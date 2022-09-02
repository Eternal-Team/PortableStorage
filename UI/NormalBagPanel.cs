using System.Linq;
using BaseLibrary.UI;
using PortableStorage.Items;
using Terraria.ModLoader;

namespace PortableStorage.UI;

public class NormalBagPanel : BaseBagPanel<BaseNormalBag>
{
	private UIGrid<UIContainerSlot> gridItems;

	public NormalBagPanel(BaseNormalBag bag) : base(bag)
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
		gridItems.Clear();

		var bags = ModContent.GetInstance<BagSyncSystem>().AllBags;
		
		for (int i = 0; i < bags[Container.ID].GetItemStorage().Count; i++)
		{
			UIContainerSlot slot = new UIContainerSlot(bags[Container.ID].GetItemStorage(), i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridItems.Add(slot);
		}
	}
}