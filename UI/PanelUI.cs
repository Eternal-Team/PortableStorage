using System;
using BaseLibrary.UI;
using PortableStorage.Items.Bags;
using PortableStorage.UI.Bags;
using Terraria;

namespace PortableStorage.UI
{
	public class PanelUI : BaseUI
	{
		public override void OnInitialize()
		{
		}

		public void HandleUI<T>(BaseBag<T> bag) where T : BaseBagPanel
		{
			if (bag.UI != null) CloseUI(bag);
			else OpenUI(bag);
		}

		public void CloseUI<T>(BaseBag<T> bag) where T : BaseBagPanel
		{
			bag.UIPosition = bag.UI.Position;
			Elements.Remove(bag.UI);
			bag.UI = null;

			Main.PlaySound(bag.CloseSound);
		}

		public void OpenUI<T>(BaseBag<T> bag) where T : BaseBagPanel
		{
			T bagUI = (T)Activator.CreateInstance(typeof(T));
			bagUI.bag = bag;
			bag.UI = bagUI;

			bagUI.Activate();

			if (bag.UIPosition != null)
			{
				bagUI.HAlign = bagUI.VAlign = 0f;
				bagUI.Position = bag.UIPosition.Value;
			}

			Append(bagUI);

			Main.PlaySound(bag.OpenSound);
		}

		//public void HandleUI(BaseQETE te)
		//{
		//	if (te.UIInternal != null) CloseUI(te);
		//	else OpenUI(te);
		//}

		//public void CloseUI(BaseQETE te)
		//{
		//	if (te.UIInternal == null) return;

		//	te.UIPosition = te.UIInternal.Position;
		//	Elements.Remove(te.UIInternal);
		//	Main.PlaySound(te.CloseSound);
		//}

		//public void OpenUI(BaseQETE te)
		//{
		//	BaseTEPanel teUI = (BaseTEPanel)Activator.CreateInstance(te.UIType);
		//	teUI.tileEntity = te;
		//	teUI.Activate();
		//	if (te.UIPosition != null)
		//	{
		//		teUI.HAlign = teUI.VAlign = 0f;
		//		teUI.Position = te.UIPosition.Value;
		//	}

		//	Append(teUI);
		//	Main.PlaySound(te.OpenSound);
		//}
	}
}