using BaseLibrary.UI.Elements;
using ContainerLibrary;

namespace PortableStorage.UI.Bags
{
	public class BaseBagPanel : UIDraggablePanel
	{
		public IItemHandler bag;

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;
	}
}