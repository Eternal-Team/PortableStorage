// using BaseLibrary.UI;
// using PortableStorage.Items;
//
// namespace PortableStorage.UI
// {
// 	public class AmmoBagPanel : BaseBagPanel<BaseAmmoBag>
// 	{
// 		public AmmoBagPanel(BaseAmmoBag bag) : base(bag)
// 		{
// 			Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
// 			Height.Pixels = 40 + (SlotSize + SlotMargin) * Container.Storage.Count / 9;
//
// 			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
// 			{
// 				Width = { Percent = 100 },
// 				Height = { Pixels = -28, Percent = 100 },
// 				Y = { Pixels = 28 },
// 				Settings = { ItemMargin = SlotMargin }
// 			};
// 			Add(gridItems);
//
// 			for (int i = 0; i < Container.Storage.Count; i++)
// 			{
// 				UIContainerSlot slot = new UIContainerSlot(Container.Storage, i) { Width = { Pixels = SlotSize }, Height = { Pixels = SlotSize } };
// 				gridItems.Add(slot);
// 			}
// 		}
// 	}
// }