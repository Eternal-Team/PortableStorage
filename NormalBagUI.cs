using BaseLibrary.UI;
using PortableStorage.Items;

namespace PortableStorage
{
	public class NormalBagUI : BaseState
	{
		public BaseBag bag;

		public void Open(BaseBag bag)
		{
			this.bag = bag;
			Display = Display.Visible;
			
			Clear();

			UIDraggablePanel panel = new UIDraggablePanel
			{
				Width = { Pixels = 512 },
				Height = { Pixels = 300 },
				X = { Percent = 50 },
				Y = { Percent = 50 }
			};
			Add(panel);

			// UITextPanel<string> exit = new UITextPanel<string>("X");
			// exit.Width.Set(30f, 0f);
			// exit.Height.Set(30f, 0f);
			// exit.HAlign = 1f;
			// exit.OnClick += (evt, element) => BagUISystem.Instance.bagState.bag = null;
			// panel.Add(exit);

			for (int i = 0; i < bag.Storage.Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag.Storage, i)
				{
					X = { Pixels = 48 * (i % 9) },
					Y = { Pixels = 60 + i / 9 * 48 }
				};
				panel.Add(slot);
			}
		}
	}
}