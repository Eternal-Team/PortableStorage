using BaseLibrary;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using PortableStorage.TileEntities;

namespace PortableStorage.UI.TileEntities
{
	public class QEChestPanel : BaseTEPanel
	{
		public UIGrid<UIContainerSlot> gridItems;

		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (172, 0);
			this.Center();
			SetPadding(0);

			//textLabel = new UIText(Lang.GetMapObjectName(MapHelper.TileToLookup(PortableStorage.Instance.TileType(tileEntity.TileType.Name), 0)))
			//{
			//	Top = (8, 0),
			//	HAlign = 0.5f
			//};
			//Append(textLabel);

			gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (-16, 1),
				Height = (-44, 1),
				Left = (8, 0),
				Top = (36, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			TEQEChest qeChest = (TEQEChest)tileEntity;
			for (int i = 0; i < qeChest.Handler.stacks.Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(qeChest, i);
				gridItems.Add(slot);
			}
		}
	}
}