using BaseLibrary;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items;
using System;
using Terraria;
using Terraria.Localization;

namespace PortableStorage.UI
{
	public interface IBagPanel
	{
		ItemHandler Handler { get; }
		Guid UUID { get; set; }
	}

	public abstract class BaseBagPanel<T> : BaseUIPanel<T>, IBagPanel, IItemHandlerUI where T : BaseBag
	{
		public Guid UUID { get; set; }

		public ItemHandler Handler => Container.Handler;

		public string GetTexture(Item item) => Handler.IsItemValid(0, item) ? Container.Texture : "";

		protected UIButton buttonQuickStack;

		public BaseBagPanel(BaseBag bag) : base((T)bag)
		{
			UIText textLabel = new UIText(Container.DisplayName.GetTranslation())
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

			buttonQuickStack = new UIButton(PortableStorage.textureQuickStack)
			{
				Size = new Vector2(20),
				X = { Pixels = 56 },
				HoverText = Language.GetText("LegacyInterface.31")
			};
			buttonQuickStack.OnClick += args => ItemUtility.QuickStack(Container.Handler, Main.LocalPlayer);
			Add(buttonQuickStack);

			UITextButton buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				X = { Percent = 100 },
				Padding = Padding.Zero,
				RenderPanel = false
			};
			buttonClose.OnClick += args => PanelUI.Instance.CloseUI(Container);
			Add(buttonClose);
		}
	}
}