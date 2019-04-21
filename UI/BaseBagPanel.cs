using System;
using System.Linq;
using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.UI
{
	public interface IBagPanel
	{
		ItemHandler Handler { get; }
		Guid ID { get; set; }
	}

	public class BaseBagPanel<T> : UIDraggablePanel, IBagPanel, IItemHandlerUI where T : BaseBag
	{
		public T Bag => Main.LocalPlayer.inventory.Concat(Main.mouseItem).OfType<T>().FirstOrDefault(x => x.ID == ID);

		public Guid ID { get; set; }

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridPotions;
		public ItemHandler Handler => Bag.Handler;
		public Texture2D ShiftClickIcon => ModContent.GetTexture("PortableStorage/Textures/MouseCursor");
	}
}