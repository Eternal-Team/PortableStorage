using System.ComponentModel;
using BaseLibrary.UI.Elements;
using Terraria.GameContent.UI.Elements;
using TheOneLibrary.Base.UI;

namespace PortableStorage.UI
{
	public class BaseBagUI : BaseUI
	{
		public UIText textLabel;

		//public UIHoverButton buttonQuickStack = new UIHoverButton(Main.chestStackTexture);
		//public UIHoverButton buttonQuickRestack = new UIHoverButton(ModLoader.GetTexture(PortableStorage.Textures.UIPath + "Restack_0"), ModLoader.GetTexture(PortableStorage.Textures.UIPath + "Restack_1"));
		//public UIButton buttonLootAll = new UIButton(ModLoader.GetTexture(PortableStorage.Textures.UIPath + "LootAll"));
		//public UIButton buttonDepositAll = new UIButton(ModLoader.GetTexture(PortableStorage.Textures.UIPath + "DepositAll"));
		//public UIButton buttonRestock = new UIButton(ModLoader.GetTexture(PortableStorage.Textures.UIPath + "Restock"));

		//public UITextButton buttonClose = new UITextButton("X", 4);

		public UIGrid<UIContainerSlot> gridItems;

		public IContainer bag;
	}
}