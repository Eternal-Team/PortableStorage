using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;

namespace PortableStorage.UI.Bags
{
	public class WalletPanel : BaseBagPanel
	{
		public override void OnInitialize()
		{
			Width = (188, 0);
			Height = (84, 0);
			this.Center();
			SetPadding(0);

			textLabel = new UIText(bag.DisplayName.TextFromTranslation())
			{
				Top = (8, 0),
				HAlign = 0.5f
			};
			Append(textLabel);

			buttonClose = new UITextButton("X", 0f)
			{
				Size = new Vector2(20),
				Left = (-28, 1),
				Top = (8, 0),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => PortableStorage.Instance.BagUI.UI.CloseBag(bag);
			Append(buttonClose);

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
	}
}