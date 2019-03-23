using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.Items.Bags;

namespace PortableStorage.UI.Bags
{
	public interface IBagPanel
	{
		BaseBag Bag { get; set; }
	}

	public class BaseBagPanel<T> : UIDraggablePanel, IBagPanel where T : BaseBag
	{
		public BaseBag Bag { get; set; }

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;
	}
}