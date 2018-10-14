using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Bags;
using Terraria;
using Terraria.UI;

namespace PortableStorage.UI.Bags
{
	public class BaseBagPanel : UIPanel
	{
		public BaseBag bag;

		public UIText textLabel;
		public UITextButton buttonClose;
		public UIGrid<UIContainerSlot> gridItems;

		#region Dragging
		private Vector2 offset;
		private bool dragging;

		public void DragStart(UIMouseEvent evt, UIElement listeningElement)
		{
			if (evt.Target != this) return;

			CalculatedStyle dimensions = GetDimensions();
			offset = evt.MousePosition - dimensions.Position();
			HAlign = VAlign = 0f;

			dragging = true;
		}

		public void DragEnd(UIMouseEvent evt, UIElement listeningElement) => dragging = false;

		public void DragUpdate(SpriteBatch spriteBatch)
		{
			if (ContainsPoint(Main.MouseScreen))
			{
				BaseLibrary.BaseLibrary.InUI = true;
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.showItemIcon = false;
				Main.ItemIconCacheUpdate(0);
			}
			else BaseLibrary.BaseLibrary.InUI = false;

			if (dragging)
			{
				CalculatedStyle dimensions = GetDimensions();

				Left = ((Main.MouseScreen.X - offset.X).Clamp(0, Main.screenWidth - dimensions.Width), 0);
				Top = ((Main.MouseScreen.Y - offset.Y).Clamp(0, Main.screenHeight - dimensions.Height), 0);

				Recalculate();
			}
		}
		#endregion
	}
}