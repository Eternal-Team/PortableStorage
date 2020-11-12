using PortableStorage.Items;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace PortableStorage
{
	public class NormalBagUI : UIState
	{
		public bool Visible => bag != null;

		public BaseBag bag;

		public void Open(BaseBag bag)
		{
			this.bag = bag;

			RemoveAllChildren();

			DragableUIPanel panel = new DragableUIPanel();
			panel.Width.Set(512f, 0f);
			panel.Height.Set(300f, 0f);
			panel.HAlign = panel.VAlign = 0.5f;
			Append(panel);

			UITextPanel<string> exit = new UITextPanel<string>("X");
			exit.Width.Set(30f, 0f);
			exit.Height.Set(30f, 0f);
			exit.HAlign = 1f;
			exit.OnClick += (evt, element) => PortableStorage.Instance.bagState.bag = null;
			panel.Append(exit);

			for (int i = 0; i < bag.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag.Handler, i);
				slot.Left.Set(48f * (i % 9), 0f);
				slot.Top.Set(60f + i / 9 * 48f, 0f);
				panel.Append(slot);
			}
		}
	}
}