using System;
using System.Collections.Generic;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using PortableStorage.Global;
using PortableStorage.Items.Bags;
using PortableStorage.UI.Bags;
using Terraria;

namespace PortableStorage.UI
{
	public class PanelUI : BaseUI
	{
		private static Dictionary<Type, Type> UICache;

		public PanelUI()
		{
			UICache = new Dictionary<Type, Type>();

			foreach (Type type in PortableStorage.Instance.Code.GetTypes())
			{
				if (type.IsSubclassOfRawGeneric(typeof(BaseBagPanel<>)) && type.BaseType != null && type.BaseType.GenericTypeArguments.Length > 0) UICache[type.BaseType.GenericTypeArguments[0]] = type;
			}
		}

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
			Main.LocalPlayer.GetModPlayer<PSPlayer>().UIPositions[bag.ID] = ((BaseElement)bag.UI).Position;
			Elements.Remove((BaseElement)bag.UI);
			bag.UI = null;

			Main.PlaySound(bag.CloseSound);
		}

		public void OpenUI(BaseBag bag)
		{
			Type bagType = UICache.ContainsKey(bag.GetType()) ? bag.GetType() : bag.GetType().BaseType;

			bag.UI = (IBagPanel)Activator.CreateInstance(UICache[bagType]);
			bag.UI.Bag = bag;

			BaseElement element = (BaseElement)bag.UI;

			element.Activate();

			if (Main.LocalPlayer.GetModPlayer<PSPlayer>().UIPositions.TryGetValue(bag.ID, out Vector2 position))
			{
				element.HAlign = element.VAlign = 0;
				element.Position = position;
			}

			Append(element);

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