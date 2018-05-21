using Terraria;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Base.UI.Elements;
using TheOneLibrary.Storage;
using TheOneLibrary.UI.Elements;

namespace PortableStorage.UI
{
	public class BaseBagUI : BaseUI, IContainerUI
	{
		public UIText textLabel;

		public UIHoverButton buttonQuickStack = new UIHoverButton(Main.chestStackTexture);
		public UIHoverButton buttonQuickRestack = new UIHoverButton(PortableStorage.Textures.restack);
		public UIButton buttonLootAll = new UIButton(PortableStorage.Textures.lootAll);
		public UIButton buttonDepositAll = new UIButton(PortableStorage.Textures.depositAll);
		public UIButton buttonRestock = new UIButton(PortableStorage.Textures.restock);

		public UITextButton buttonClose = new UITextButton("X", 4);

		public UIGrid<UIContainerSlot> gridItems;

		public IContainer bag;

		public void LootAll(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.LocalPlayer.chest == -1 && Main.npcShop == 0)
			{
				TheOneLibrary.Utils.Utility.LootAll(bag);
				Recipe.FindRecipes();
			}
		}

		public void DepositAll(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.LocalPlayer.chest == -1 && Main.npcShop == 0)
			{
				TheOneLibrary.Utils.Utility.DepositAll(bag);
				Recipe.FindRecipes();
			}
		}

		public void Restock(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.LocalPlayer.chest == -1 && Main.npcShop == 0)
			{
				TheOneLibrary.Utils.Utility.Restock(bag);
				Recipe.FindRecipes();
			}
		}

		public void QuickRestack(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.LocalPlayer.chest == -1 && Main.npcShop == 0)
			{
				TheOneLibrary.Utils.Utility.QuickRestack(bag);
				Recipe.FindRecipes();
			}
		}

		public void QuickStack(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.LocalPlayer.chest == -1 && Main.npcShop == 0)
			{
				TheOneLibrary.Utils.Utility.QuickStack(bag);
				Recipe.FindRecipes();
			}
		}

		public void SetContainer(IContainer container)
		{
			bag = container;

			for (int i = 0; i < bag.GetItems().Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag, i);
				gridItems.Add(slot);
			}
		}

		public IContainer GetContainer() => bag;
	}
}