using System.Linq;
using PortableStorage.Items.Bags;
using TheOneLibrary.Base.UI;

namespace PortableStorage.UI
{
	public class BagUI : BaseUI
	{
		public override void OnInitialize()
		{
		}

		public void HandleBag(BaseBag bag)
		{
			var bagUIs = Elements.OfType<BagPanel>().ToList();
			if (bagUIs.Any(x => x.bag.ID == bag.ID)) RemoveChild(bagUIs.First(x => x.bag.ID == bag.ID));
			else
			{
				BagPanel bagUI = new BagPanel();
				bagUI.bag = bag;
				bagUI.Activate();
				Append(bagUI);
			}
		}
	}
}