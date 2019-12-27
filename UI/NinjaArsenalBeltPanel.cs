using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.Items.Special;

namespace PortableStorage.UI
{
	public class NinjaArsenalBeltPanel : BaseBagPanel<NinjaArsenalBelt>
	{
		public override void OnInitialize()
		{
			base.OnInitialize();

			Width = (12 + (SlotSize + Padding) * 9, 0);
			Height = (40 + (SlotSize + Padding) * Container.Handler.Slots / 9, 0);
			this.Center();

			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (-28, 1),
				Top = (28, 0),
				OverflowHidden = true,
				ListPadding = Padding
			};
			Append(gridItems);

			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i) { Width = (SlotSize, 0), Height = (SlotSize, 0) };
				gridItems.Add(slot);
			}
		}
	}
}