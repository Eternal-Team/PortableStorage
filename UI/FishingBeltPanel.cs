using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.Items.Special;
using Terraria;

namespace PortableStorage.UI
{
	public class FishingBeltPanel : BaseBagPanel<FishingBelt>
	{
		public override void OnInitialize()
		{
			base.OnInitialize();

			Width = (408, 0);
			Height = (40 + Container.Handler.Slots / 9 * 44, 0);
			this.Center();

			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (-28, 1),
				Top = (28, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i);
				gridItems.Add(slot);
			}
		}
	}
}