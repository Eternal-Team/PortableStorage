using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utils;

namespace PortableStorage.UI
{
	public class QEBagUI : BaseBagUI
	{
		public UIColor[] colorFrequency = new UIColor[3];

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = 408;
			panelMain.Height.Pixels = 172;
			panelMain.Center();
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel = new UIText(() => "Quantum Entangled Bag");
			textLabel.HAlign = 0.5f;
			textLabel.Top.Pixels = 8;
			panelMain.Append(textLabel);

			buttonLootAll.Width.Pixels = 24;
			buttonLootAll.Height.Pixels = 24;
			buttonLootAll.Left.Pixels = 8;
			buttonLootAll.Top.Pixels = 8;
			buttonLootAll.HoverText += () => Language.GetTextValue("LegacyInterface.29");
			buttonLootAll.OnClick += LootAll;
			panelMain.Append(buttonLootAll);

			buttonDepositAll.Width.Pixels = 24;
			buttonDepositAll.Height.Pixels = 24;
			buttonDepositAll.Left.Pixels = 40;
			buttonDepositAll.Top.Pixels = 8;
			buttonDepositAll.HoverText += () => Language.GetTextValue("LegacyInterface.30");
			buttonDepositAll.OnClick += DepositAll;
			panelMain.Append(buttonDepositAll);

			buttonRestock.Width.Pixels = 24;
			buttonRestock.Height.Pixels = 24;
			buttonRestock.Left.Pixels = 72;
			buttonRestock.Top.Pixels = 8;
			buttonRestock.HoverText += () => Language.GetTextValue("LegacyInterface.82");
			buttonRestock.OnClick += Restock;
			panelMain.Append(buttonRestock);

			for (int i = 0; i < colorFrequency.Length; i++)
			{
				colorFrequency[i] = new UIColor(null);
				int i1 = i;
				colorFrequency[i].GetColor += () => ((QEBag)bag).frequency.Colors[2 - i1].ToColor();
				colorFrequency[i].Width.Pixels = 16;
				colorFrequency[i].Height.Pixels = 16;
				colorFrequency[i].Left.Set(-52f - i * 20f, 1f);
				colorFrequency[i].Top.Pixels = 12;
				panelMain.Append(colorFrequency[i]);
			}

			buttonClose.Width.Pixels = 24;
			buttonClose.Height.Pixels = 24;
			buttonClose.Left.Set(-28, 1);
			buttonClose.Top.Pixels = 8;
			buttonClose.OnClick += (evt, element) =>
			{
				PortableStorage.Instance.UIs.dict.Remove((QEBag)bag);

				Main.PlaySound(SoundID.DD2_EtherianPortalOpen.WithVolume(0.5f));
			};
			panelMain.Append(buttonClose);

			gridItems = new UIGrid<UIContainerSlot>(9);
			gridItems.Width.Set(-16, 1);
			gridItems.Height.Set(-44, 1);
			gridItems.Left.Pixels = 8;
			gridItems.Top.Pixels = 36;
			gridItems.ListPadding = 4;
			gridItems.OverflowHidden = true;
			panelMain.Append(gridItems);
		}
	}
}