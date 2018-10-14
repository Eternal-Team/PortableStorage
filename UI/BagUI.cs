﻿using System;
using Microsoft.Xna.Framework;
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
			if (bag.UIPosition != -Vector2.One)
			{
				bagUI.HAlign = bagUI.VAlign = 0f;
				bagUI.Position = bag.UIPosition;
			}

			Append(bagUI);
			Main.PlaySound(bag.OpenSound);
		}
	}
}