using System;
using System.Linq;
using PortableStorage.Items.Bags;
using Terraria;
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
			var bagUIs = Elements.OfType<BaseBagPanel>().ToList();
			if (bagUIs.Any(x => x.bag.ID == bag.ID))
			{
				RemoveChild(bagUIs.First(x => x.bag.ID == bag.ID));
				Main.PlaySound(bag.CloseSound);
			}
			else
			{
				BaseBagPanel bagUI = (BaseBagPanel)Activator.CreateInstance(bag.UIType);
				bagUI.bag = bag;
				bagUI.Activate();
				Append(bagUI);
				Main.PlaySound(bag.OpenSound);
			}
		}
	}
}