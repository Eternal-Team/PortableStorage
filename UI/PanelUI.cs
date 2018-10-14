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

		public void HandleBag(BaseBag bag)
		{
			if (bag.UI != null) CloseBag(bag);
			else OpenBag(bag);
		}

		public void CloseBag(BaseBag bag)
		{
			bag.UIPosition = bag.UI.Position;
			Elements.Remove(bag.UI);
			Main.PlaySound(bag.CloseSound);
		}

		public void OpenBag(BaseBag bag)
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

		public void HandleTE(BasePSTE te)
		{
			if (te.UI != null) CloseTE(te);
			else OpenTE(te);
		}

		public void CloseTE(BasePSTE te)
		{
			te.UIPosition = te.UI.Position;
			Elements.Remove(te.UI);
			Main.PlaySound(te.CloseSound);
		}

		public void OpenTE(BasePSTE te)
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