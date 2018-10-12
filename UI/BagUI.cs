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
			if (Elements.OfType<BaseBagPanel>().Any(x => x.bag.ID == bag.ID)) CloseBag(bag);
			else OpenBag(bag);
		}

		public void CloseBag(BaseBag bag)
		{
			Elements.RemoveAll(x => x is BaseBagPanel panel && panel.bag.ID == bag.ID);
			Main.PlaySound(bag.CloseSound);
		}

		public void OpenBag(BaseBag bag)
		{
			BaseBagPanel bagUI = (BaseBagPanel)Activator.CreateInstance(bag.UIType);
			bagUI.bag = bag;
			bagUI.Activate();
			Append(bagUI);
			Main.PlaySound(bag.OpenSound);
		}
	}
}