using System.Linq;
using PortableStorage.Items.Bags;
using TheOneLibrary.Base.UI;

namespace PortableStorage.UI
{
	public class AllBagsUI : BaseUI
	{
		public override void OnInitialize()
		{
		}

		public void HandleBag(BaseBag bag)
		{
			var bagUIs = Elements.OfType<BagUI>().ToList();
			if (bagUIs.Any(x => x.bag.ID == bag.ID)) RemoveChild(bagUIs.First(x => x.bag.ID == bag.ID));
			else
			{
				BagUI bagUI = new BagUI(bag);
				bagUI.Activate();
				Append(bagUI);
			}
		}
	}
}