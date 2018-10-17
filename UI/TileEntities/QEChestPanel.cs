using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using ContainerLibrary;
using PortableStorage.TileEntities;
using Terraria;
using Terraria.Map;

namespace PortableStorage.UI.TileEntities
{
	public class QEChestPanel : BaseTEPanel
	{
		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (256, 0);
			this.Center();
			SetPadding(0);

			textLabel = new UIText(Lang.GetMapObjectName(MapHelper.TileToLookup(PortableStorage.Instance.TileType(te.TileType.Name), 0)))
			{
				Top = (8, 0),
				HAlign = 0.5f
			};
			Append(textLabel);

			gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (-16, 1),
				Height = (-128, 1),
				Left = (8, 0),
				Top = (36, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			Repopulate();
		}

		public void Repopulate()
		{
			gridItems.Clear();

			TEQEChest qeChest = (TEQEChest)te;
			for (int i = 0; i < qeChest.Handler.stacks.Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(qeChest.Handler, i);
				gridItems.Add(slot);
			}
		}
	}
}