using System;
using PortableStorage.Items.Bags;
using PortableStorage.TileEntities;
using PortableStorage.UI.Bags;
using PortableStorage.UI.TileEntities;
using Terraria;
using TheOneLibrary.Base.UI;

namespace PortableStorage.UI
{
	public class PanelUI : BaseUI
	{
		public override void OnInitialize()
		{
		}

		public void HandleUI(BaseBag bag)
		{
			if (bag.UI != null) CloseUI(bag);
			else OpenUI(bag);
		}

		public void CloseUI(BaseBag bag)
		{
			bag.UIPosition = bag.UI.Position;
			Elements.Remove(bag.UI);
			Main.PlaySound(bag.CloseSound);
		}

		public void OpenUI(BaseBag bag)
		{
			BaseBagPanel bagUI = (BaseBagPanel)Activator.CreateInstance(bag.UIType);
			bagUI.bag = bag;
			bagUI.Activate();
			if (bag.UIPosition != null)
			{
				bagUI.HAlign = bagUI.VAlign = 0f;
				bagUI.Position = bag.UIPosition.Value;
			}

			Append(bagUI);
			Main.PlaySound(bag.OpenSound);
		}

		public void HandleUI(BaseQETE te)
		{
			if (te.UIInternal != null) CloseUI(te);
			else OpenUI(te);
		}

		public void CloseUI(BaseQETE te)
		{
			if (te.UIInternal == null) return;

			te.UIPosition = te.UIInternal.Position;
			Elements.Remove(te.UIInternal);
			Main.PlaySound(te.CloseSound);
		}

		public void OpenUI(BaseQETE te)
		{
			BaseTEPanel teUI = (BaseTEPanel)Activator.CreateInstance(te.UIType);
			teUI.te = te;
			teUI.Activate();
			if (te.UIPosition != null)
			{
				teUI.HAlign = teUI.VAlign = 0f;
				teUI.Position = te.UIPosition.Value;
			}

			Append(teUI);
			Main.PlaySound(te.OpenSound);
		}
	}
}