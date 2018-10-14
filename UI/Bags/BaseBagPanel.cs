using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.Items.Bags;

namespace PortableStorage.UI.Bags
{
	public class BaseBagPanel : UIDraggablePanel
	{
		public BaseBag bag;

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;
	}
}