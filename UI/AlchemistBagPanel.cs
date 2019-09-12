using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.Items.Special;
using Terraria;

namespace PortableStorage.UI
{
	public class AlchemistBagPanel : BaseBagPanel<AlchemistBag>
	{
		public override void OnInitialize()
		{
			base.OnInitialize();

			Width = (408, 0);
			Height = (100 + Container.Handler.Slots / 9 * 44, 0);
			this.Center();

			UIText textPotions = new UIText("Potions")
			{
				Top = (28, 0)
			};
			Append(textPotions);

			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (84, 0),
				Top = (56, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			for (int i = 0; i < 18; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i);
				gridItems.Add(slot);
			}

			UIText textIngredients = new UIText("Ingredients")
			{
				Top = (148, 0)
			};
			Append(textIngredients);

			UIGrid<UIContainerSlot> gridIngredients = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (304, 0),
				Top = (176, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridIngredients);

			for (int i = 18; i < 81; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i);
				gridIngredients.Add(slot);
			}
		}
	}
}