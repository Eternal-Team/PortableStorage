using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;

namespace PortableStorage.UI.Bags
{
	public class AmmoBagPanel : BaseBagPanel
	{
		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (40 + bag.Handler.Slots / 9 * 44, 0);
			this.Center();

			textLabel = new UIText(bag.DisplayName.GetTranslation())
			{
				HAlign = 0.5f
			};
			Append(textLabel);

			buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => PortableStorage.Instance.PanelUI.UI.CloseUI(bag);
			Append(buttonClose);

			gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (-28, 1),
				Top = (28, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			for (int i = 0; i < bag.Handler.stacks.Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag, i);
				gridItems.Add(slot);
			}
		}
	}
}