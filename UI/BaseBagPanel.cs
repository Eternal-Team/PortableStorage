using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
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

		public override void OnInitialize()
		{
			UIText textLabel = new UIText(Container.DisplayName.GetTranslation())
			{
				HAlign = 0.5f,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			Append(textLabel);

			UIButton buttonLootAll = new UIButton(PortableStorage.textureLootAll)
			{
				Size = new Vector2(20),
				HoverText = Language.GetText("LegacyInterface.29")
			};
			buttonLootAll.OnClick += (evt, element) => ItemUtility.LootAll(Container.Handler, Main.LocalPlayer);
			Append(buttonLootAll);

			UIButton buttonDepositAll = new UIButton(PortableStorage.textureDepositAll)
			{
				Size = new Vector2(20),
				Left = (28, 0),
				HoverText = Language.GetText("LegacyInterface.30")
			};
			buttonDepositAll.OnClick += (evt, element) => ItemUtility.DepositAll(Container.Handler, Main.LocalPlayer);
			Append(buttonDepositAll);

			UIButton buttonQuickStack = new UIButton(PortableStorage.textureQuickStack)
			{
				Size = new Vector2(20),
				Left = (56, 0),
				HoverText = Language.GetText("LegacyInterface.31")
			};
			buttonQuickStack.OnClick += (evt, element) => ItemUtility.QuickStack(Container.Handler, Main.LocalPlayer);
			Append(buttonQuickStack);

			UITextButton buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				Padding = (0, 0, 0, 0),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(Container);
			Append(buttonClose);
		}
	}
}