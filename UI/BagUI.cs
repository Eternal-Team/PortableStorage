using PortableStorage.Items;
using Terraria.Localization;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utils;

namespace PortableStorage.UI
{
	public class BagUI : BaseBagUI
	{
		public BagUI()
		{
			gridItems = new UIGrid<UIContainerSlot>(9);
		}

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = 408;
			panelMain.Height.Pixels = 308;
			panelMain.Center();
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel = new UIText(() => "Bag");
			textLabel.HAlign = 0.5f;
			textLabel.Top.Pixels = 8;
			panelMain.Append(textLabel);

			buttonQuickStack.Width.Pixels = 24;
			buttonQuickStack.Height.Pixels = 24;
			buttonQuickStack.Left.Pixels = 8;
			buttonQuickStack.Top.Pixels = 8;
			buttonQuickStack.HoverText += () => Language.GetTextValue("GameUI.QuickStackToNearby");
			buttonQuickStack.OnClick += QuickStack;
			panelMain.Append(buttonQuickStack);

			buttonQuickRestack.Width.Pixels = 24;
			buttonQuickRestack.Height.Pixels = 24;
			buttonQuickRestack.Left.Pixels = 40;
			buttonQuickRestack.Top.Pixels = 8;
			buttonQuickRestack.HoverText += () => "Quick restack from nearby chests";
			buttonQuickRestack.OnClick += QuickRestack;
			panelMain.Append(buttonQuickRestack);

			buttonLootAll.Width.Pixels = 24;
			buttonLootAll.Height.Pixels = 24;
			buttonLootAll.Left.Pixels = 72;
			buttonLootAll.Top.Pixels = 8;
			buttonLootAll.HoverText += () => Language.GetTextValue("LegacyInterface.29");
			buttonLootAll.OnClick += LootAll;
			panelMain.Append(buttonLootAll);

			buttonDepositAll.Width.Pixels = 24;
			buttonDepositAll.Height.Pixels = 24;
			buttonDepositAll.Left.Pixels = 104;
			buttonDepositAll.Top.Pixels = 8;
			buttonDepositAll.HoverText += () => Language.GetTextValue("LegacyInterface.30");
			buttonDepositAll.OnClick += DepositAll;
			panelMain.Append(buttonDepositAll);

			buttonRestock.Width.Pixels = 24;
			buttonRestock.Height.Pixels = 24;
			buttonRestock.Left.Pixels = 136;
			buttonRestock.Top.Pixels = 8;
			buttonRestock.HoverText += () => Language.GetTextValue("LegacyInterface.82");
			buttonRestock.OnClick += Restock;
			panelMain.Append(buttonRestock);

			buttonClose.Width.Pixels = 24;
			buttonClose.Height.Pixels = 24;
			buttonClose.Left.Set(-28, 1);
			buttonClose.Top.Pixels = 8;
			buttonClose.OnClick += (evt, element) => ((Bag)bag).CloseUI();
			panelMain.Append(buttonClose);

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