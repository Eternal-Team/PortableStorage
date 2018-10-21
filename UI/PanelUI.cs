using System;
using BaseLibrary.Tiles.TileEntites;
using PortableStorage.Items.Bags;
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

		public void HandleUI<T>(BaseTEWithUI<T> te) where T : BaseTEPanel
		{
			if (te.UI != null) CloseUI(te);
			else OpenUI(te);
		}

		public void CloseUI<T>(BaseTEWithUI<T> te) where T : BaseTEPanel
		{
			if (te.UI == null) return;

			te.UIPosition = te.UI.Position;
			Elements.Remove(te.UI);
			Main.PlaySound(te.CloseSound);
		}

		public void OpenUI<T>(BaseTEWithUI<T> te) where T : BaseTEPanel
		{
			T teUI = Activator.CreateInstance<T>();
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