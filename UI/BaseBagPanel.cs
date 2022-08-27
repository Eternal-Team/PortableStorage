using System;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace PortableStorage.UI;

public abstract class BaseBagPanel<T> : BaseUIPanel<T>, IItemStorageUI where T : BaseBag
{
	protected const int SlotSize = 44;
	protected const int SlotMargin = 4;

	public ItemStorage GetItemStorage() => Container.GetItemStorage();

	public string GetCursorTexture(Item item) => Container.Texture;

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

		Main.instance.LoadItem(ItemID.TreasureMagnet);
		UITexture buttonPickup = new UITexture(TextureAssets.Item[ItemID.TreasureMagnet])
		{
			Settings = { ScaleMode = ScaleMode.Stretch },
			Height = { Pixels = 20 },
			Width = { Pixels = 20 },
			X = { Percent = 100, Pixels = -28 },
			HoverText = (Func<string>)(() => Language.GetText("Mods.PortableStorage.UI.Pickup" + bag.PickupMode).ToString())
		};
		buttonPickup.OnMouseDown += args =>
		{
			bag.PickupMode = bag.PickupMode.NextEnum();

			SoundEngine.PlaySound(SoundID.MenuTick);

			args.Handled = true;
		};
		Add(buttonPickup);

		UIText buttonClose = new UIText("X")
		{
			Height = { Pixels = 20 },
			Width = { Pixels = 20 },
			X = { Percent = 100 },
			HoverText = Language.GetText("Mods.PortableStorage.UI.Close")
		};
		buttonClose.OnMouseDown += args =>
		{
			PanelUI.Instance.CloseUI(Container);
			args.Handled = true;
		};
		buttonClose.OnMouseEnter += _ => buttonClose.Settings.TextColor = Color.Red;
		buttonClose.OnMouseLeave += _ => buttonClose.Settings.TextColor = Color.White;
		Add(buttonClose);
	}
}