using BaseLib.Elements;
using BaseLib.UI;
using BaseLib.Utility;
using ContainerLib2.Container;
using PortableStorage.Items;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;

namespace PortableStorage.UI
{
	public class BagUI : BaseUI
	{
		public UIText textLabel = new UIText("Bag");
		public UITextButton buttonClose = new UITextButton("X", 4);
		public UIGrid gridItems = new UIGrid(9);

		public Guid guid = Guid.Empty;

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = 408;
			panelMain.Height.Pixels = 308;
			panelMain.Center();
			panelMain.SetPadding(0);
			panelMain.BackgroundColor = panelColor;
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel.HAlign = 0.5f;
			textLabel.Top.Pixels = 8;
			panelMain.Append(textLabel);

			buttonClose.Width.Pixels = 24;
			buttonClose.Height.Pixels = 24;
			buttonClose.Left.Set(-28, 1);
			buttonClose.Top.Pixels = 8;
			buttonClose.OnClick += (evt, element) =>
			{
				PortableStorage.Instance.BagUI.Remove(guid);
				Main.PlaySound(SoundID.Item59.WithVolume(0.5f));
			};
			panelMain.Append(buttonClose);

			gridItems.Width.Set(-16, 1);
			gridItems.Height.Set(-44, 1);
			gridItems.Left.Pixels = 8;
			gridItems.Top.Pixels = 36;
			gridItems.ListPadding = 4;
			gridItems.OverflowHidden = true;
			panelMain.Append(gridItems);
		}

		public void Load(Bag bag)
		{
			guid = bag.guid;

			for (int i = 0; i < bag.GetItems().Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag, i);
				gridItems.Add(slot);
			}
		}
	}
}