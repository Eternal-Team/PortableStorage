using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items.Ammo;
using Terraria;
using Terraria.Localization;

namespace PortableStorage.UI
{
	public class WalletPanel : BaseBagPanel<Wallet>
	{
		public WalletPanel(Wallet wallet) : base(wallet)
		{
			Width.Pixels = 12 + (SlotSize + SlotMargin) * 4;
			Height.Pixels = 44 + SlotSize;

			Clear();

			UIText textLabel = new UIText(Container.DisplayName.GetDefault())
			{
				X = { Percent = 50 },
				HorizontalAlignment = HorizontalAlignment.Center
			};
			Add(textLabel);

			UIButton buttonLootAll = new UIButton(PortableStorage.textureLootAll)
			{
				Size = new Vector2(20),
				HoverText = Language.GetText("LegacyInterface.29")
			};
			buttonLootAll.OnClick += args => ItemUtility.LootAll(Container.Handler, Main.LocalPlayer);
			Add(buttonLootAll);

			UIButton buttonDepositAll = new UIButton(PortableStorage.textureDepositAll)
			{
				Size = new Vector2(20),
				X = { Pixels = 28 },
				HoverText = Language.GetText("LegacyInterface.30")
			};
			buttonDepositAll.OnClick += args => ItemUtility.DepositAll(Container.Handler, Main.LocalPlayer);
			Add(buttonDepositAll);

			UITextButton buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				X = { Percent = 100 },
				Padding = Padding.Zero,
				RenderPanel = false
			};
			buttonClose.OnClick += args => PanelUI.Instance.CloseUI(Container);
			Add(buttonClose);

			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(4)
			{
				Width = { Percent = 100 },
				Height = { Pixels = -28, Percent = 100 },
				Y = { Pixels = 28 },
				ItemMargin = SlotMargin
			};
			Add(gridItems);

			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i)
				{
					ShortStackSize = true,
					Width = { Pixels = SlotSize },
					Height = { Pixels = SlotSize }
				};
				gridItems.Add(slot);
			}
		}
	}
}