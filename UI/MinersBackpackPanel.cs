using BaseLibrary.UI;
using ContainerLibrary;
using PortableStorage.Items.Special;

namespace PortableStorage.UI
{
	public class MinersBackpackPanel : BaseBagPanel<MinersBackpack>
	{
		public MinersBackpackPanel(MinersBackpack bag) : base(bag)
		{
			Width.Pixels = 12 + (SlotSize + Padding) * 9;
			Height.Pixels = 40 + (SlotSize + Padding) * Container.Handler.Slots / 9;

			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = { Percent = 100 },
				Height = { Pixels = -28, Percent = 100 },
				Y = { Pixels = 28 },
				ListPadding = Padding
			};

			Add(gridItems);
			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i)
				{
					Width = { Pixels = SlotSize },
					Height = { Pixels = SlotSize }
				};
				gridItems.Add(slot);
			}
		}
	}
}