using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items;
using Terraria;

namespace PortableStorage.UI;

public abstract class BaseBagPanel<T> : BaseUIPanel<T>, IItemStorageUI where T : BaseBag
{
	protected const int SlotSize = 44;
	protected const int SlotMargin = 4;

	public ItemStorage GetItemStorage() => Container.GetItemStorage();

	// public bool IsVisible() => Display == Display.Visible;
	public string GetCursorTexture(Item item) => Container.Texture;

	// protected UIButton buttonQuickStack;

	protected override void Activate()
	{
		ContainerLibrary.ContainerLibrary.OpenedStorageUIs.Add(this);
	}

	protected override void Deactivate()
	{
		ContainerLibrary.ContainerLibrary.OpenedStorageUIs.Remove(this);
	}

	public BaseBagPanel(BaseBag bag) : base((T)bag)
	{
		UIText textLabel = new UIText(Container.Item.Name)
		{
			X = { Percent = 50 },
			Settings = { HorizontalAlignment = HorizontalAlignment.Center }
		};
		Add(textLabel);

		// UIButton buttonLootAll = new UIButton(PortableStorage.textureLootAll)
		// {
		// 	Size = new Vector2(20),
		// 	HoverText = Language.GetText("LegacyInterface.29")
		// };
		// buttonLootAll.OnClick += args => ItemUtility.LootAll(Container.Handler, Main.LocalPlayer);
		// Add(buttonLootAll);
		//
		// UIButton buttonDepositAll = new UIButton(PortableStorage.textureDepositAll)
		// {
		// 	Size = new Vector2(20),
		// 	X = { Pixels = 28 },
		// 	HoverText = Language.GetText("LegacyInterface.30")
		// };
		// buttonDepositAll.OnClick += args => ItemUtility.DepositAll(Container.Handler, Main.LocalPlayer);
		// Add(buttonDepositAll);
		//
		// buttonQuickStack = new UIButton(PortableStorage.textureQuickStack)
		// {
		// 	Size = new Vector2(20),
		// 	X = { Pixels = 56 },
		// 	HoverText = Language.GetText("LegacyInterface.31")
		// };
		// buttonQuickStack.OnClick += args => ItemUtility.QuickStack(Container.Handler, Main.LocalPlayer);
		// Add(buttonQuickStack);

		UIText buttonClose = new UIText("X")
		{
			Height = { Pixels = 20 },
			Width = { Pixels = 20 },
			X = { Percent = 100 }
		};
		buttonClose.OnClick += args =>
		{
			PanelUI.Instance.CloseUI(Container);
			args.Handled = true;
		};
		buttonClose.OnMouseEnter += args => buttonClose.Settings.TextColor = Color.Red;
		buttonClose.OnMouseLeave += args => buttonClose.Settings.TextColor = Color.White;
		Add(buttonClose);
	}
}