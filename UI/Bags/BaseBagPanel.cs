using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Bags;
using Terraria.ModLoader;

namespace PortableStorage.UI.Bags
{
	public interface IBagPanel
	{
		BaseBag Bag { get; set; }
	}

	public class BaseBagPanel<T> : UIDraggablePanel, IBagPanel, IItemHandlerUI where T : BaseBag
	{
		public BaseBag Bag { get; set; }

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;
		public ItemHandler Handler => Bag.Handler;
		public Texture2D ShiftClickIcon => ModContent.GetTexture("PortableStorage/Textures/MouseCursor");
	}
}