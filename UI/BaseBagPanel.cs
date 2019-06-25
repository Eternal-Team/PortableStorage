using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.Items;
using System;
using Terraria;

namespace PortableStorage.UI
{
	public interface IBagPanel
	{
		ItemHandler Handler { get; }
		Guid ID { get; set; }
	}

	public abstract class BaseBagPanel<T> : BaseUIPanel<T>, IBagPanel, IItemHandlerUI, IHasCursorOverride where T : BaseBag
	{
		public Guid ID { get; set; }

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;
		public ItemHandler Handler => Container.Handler;

		public string GetTexture(Item item) => Handler.IsItemValid(0, item) ? Container.Texture : "";
	}
}