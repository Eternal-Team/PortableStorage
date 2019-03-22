using System;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using PortableStorage.Global;
using PortableStorage.Items.Bags;
using PortableStorage.UI.Bags;
using Terraria;
using Utility = BaseLibrary.Utility;

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
			Main.LocalPlayer.GetModPlayer<PSPlayer>().UIPositions[bag.ID] = bag.UI.Position;
			Elements.Remove(bag.UI);
			bag.UI = null;

			Main.PlaySound(bag.CloseSound);
		}

		public void OpenUI(BaseBag bag)
		{
			Type type = bag.GetType().GetField("UI", Utility.defaultFlags)?.FieldType;

			if (type == null || !type.IsSubclassOf(typeof(BaseBagPanel))) throw new Exception("Bag must implement field 'UI' with type derived from 'BaseBagPanel'!");

			bag.UI = (BaseBagPanel)Activator.CreateInstance(type);
			bag.UI.bag = bag;

			bag.UI.Activate();

            if (Main.LocalPlayer.GetModPlayer<PSPlayer>().UIPositions.TryGetValue(bag.ID, out Vector2 position))
            {
                bag.UI.HAlign = bag.UI.VAlign = 0;
                bag.UI.Position = position;
            }

            Append(bag.UI);

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