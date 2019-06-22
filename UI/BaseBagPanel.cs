using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items;
using System;
using Terraria.ModLoader;

namespace PortableStorage.UI
{
	public interface IBagPanel
	{
		ItemHandler Handler { get; }
		Guid ID { get; set; }
	}

	public abstract class BaseBagPanel<T> : BaseUIPanel<T>, IBagPanel, IItemHandlerUI where T : BaseBag
	{
		//public T Bag => Main.LocalPlayer.inventory.Concat(Main.mouseItem).OfType<T>().FirstOrDefault(x => x.ID == ID);

		public Guid ID { get; set; }

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;
		public ItemHandler Handler => Container.Handler;
		public Texture2D ShiftClickIcon => ModContent.GetTexture("PortableStorage/Textures/MouseCursor");
	}
}