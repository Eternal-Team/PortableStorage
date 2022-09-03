using System;
using BaseLibrary;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
		UIText textLabel = new UIText(Lang.GetItemNameValue(Container.Item.type))
		{
			X = { Percent = 50 },
			Settings = { HorizontalAlignment = HorizontalAlignment.Center }
		};
		Add(textLabel);

		UITexture buttonLootAll = new UITexture(ModContent.Request<Texture2D>(BaseLibrary.BaseLibrary.AssetPath + "Textures/UI/LootAll"))
		{
			Size = new Vector2(20),
			HoverText = Language.GetText("Mods.PortableStorage.UI.LootAll")
		};
		buttonLootAll.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;

			args.Handled = true;

			Main.LocalPlayer.LootAll(Container.GetItemStorage());

			SoundEngine.PlaySound(SoundID.Grab);
		};
		Add(buttonLootAll);

		UITexture buttonDepositAll = new UITexture(ModContent.Request<Texture2D>(BaseLibrary.BaseLibrary.AssetPath + "Textures/UI/DepositAll"))
		{
			Size = new Vector2(20),
			X = { Pixels = 28 },
			HoverText = Language.GetText("Mods.PortableStorage.UI.DepositAll")
		};
		buttonDepositAll.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;

			args.Handled = true;

			Main.LocalPlayer.DepositAll(Container.GetItemStorage());

			SoundEngine.PlaySound(SoundID.Grab);
		};
		Add(buttonDepositAll);

		UITexture buttonQuickStack = new UITexture(ModContent.Request<Texture2D>(BaseLibrary.BaseLibrary.AssetPath + "Textures/UI/QuickStack"))
		{
			Size = new Vector2(20),
			X = { Pixels = 56 },
			HoverText = Language.GetText("Mods.PortableStorage.UI.QuickStack")
		};
		buttonQuickStack.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;

			args.Handled = true;

			Main.LocalPlayer.QuickStack(Container.GetItemStorage());

			SoundEngine.PlaySound(SoundID.Grab);
		};
		Add(buttonQuickStack);

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
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;

			bag.PickupMode = bag.PickupMode.NextEnum();
			BagSyncSystem.Instance.Sync(Container.ID, PacketID.PickupMode);

			SoundEngine.PlaySound(SoundID.MenuTick);
		};
		Add(buttonPickup);

		UIText buttonClose = new UIText("X")
		{
			Height = { Pixels = 20 },
			Width = { Pixels = 20 },
			X = { Percent = 100 },
			HoverText = Language.GetText("Mods.BaseLibrary.UI.Close")
		};
		buttonClose.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;

			PanelUI.Instance?.CloseUI(Container);
		};
		buttonClose.OnMouseEnter += _ => buttonClose.Settings.TextColor = Color.Red;
		buttonClose.OnMouseLeave += _ => buttonClose.Settings.TextColor = Color.White;
		Add(buttonClose);
	}
}