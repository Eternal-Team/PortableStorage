using System;
using System.Linq;
using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Bags;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.UI.Bags
{
	public interface IBagPanel
	{
		BaseBag Bag { get; }
		Guid ID { get; set; }
	}

    public class BaseBagPanel<T> : UIDraggablePanel, IBagPanel, IItemHandlerUI where T : BaseBag
	{
		public BaseBag Bag => Main.LocalPlayer.inventory.Concat(Main.mouseItem).OfType<BaseBag>().FirstOrDefault(x => x.ID == ID);
		public Guid ID { get; set; }

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;
		public ItemHandler Handler => Bag.Handler;
		public Texture2D ShiftClickIcon => ModContent.GetTexture("PortableStorage/Textures/MouseCursor");
	}
}