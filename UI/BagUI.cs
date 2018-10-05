using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Bags;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UIPanel = BaseLibrary.UI.Elements.UIPanel;

namespace PortableStorage.UI
{
	public class BagUI : UIPanel
	{
		public UIGrid<UIContainerSlot> gridItems;
		public UIText textLabel;

		public BaseBag bag;

		public BagUI(BaseBag bag) => this.bag = bag;

		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (308, 0);
			this.Center();
			SetPadding(0);
			OnPostDraw += DragUpdate;
			OnMouseDown += DragStart;
			OnMouseUp += DragEnd;

			textLabel = new UIText("Bag")
			{
				HAlign = 0.5f
			};
			textLabel.Top.Pixels = 8;
			Append(textLabel);

			gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (-16, 1),
				Height = (-44, 1),
				Left = (8, 0),
				Top = (36, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			for (int i = 0; i < bag.handler.stacks.Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag.handler, i);
				gridItems.Add(slot);
			}
		}

		#region Dragging
		public Vector2 offset;
		public bool dragging;

		public virtual void DragStart(UIMouseEvent evt, UIElement listeningElement)
		{
			if (evt.Target != this) return;

			CalculatedStyle dimensions = GetDimensions();
			offset = evt.MousePosition - dimensions.Position();
			HAlign = VAlign = 0f;

			dragging = true;
		}

		public virtual void DragEnd(UIMouseEvent evt, UIElement listeningElement) => dragging = false;

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