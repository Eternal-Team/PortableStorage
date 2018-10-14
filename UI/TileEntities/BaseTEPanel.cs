using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.TileEntities;

namespace PortableStorage.UI.TileEntities
{
	public class BaseTEPanel : UIDraggablePanel
	{
		public BasePSTE te;

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;
	}
}