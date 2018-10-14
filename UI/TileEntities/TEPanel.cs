using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using ContainerLibrary;

namespace PortableStorage.UI.TileEntities
{
	public class TEPanel : BaseTEPanel
	{
		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (172, 0);
			this.Center();
			SetPadding(0);

			textLabel = new UIText( /*bag.DisplayName.TextFromTranslation()*/"Quantum Entangled Chest")
			{
				Top = (8, 0),
				HAlign = 0.5f
			};
			Append(textLabel);

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

			//for (int i = 0; i < bag.handler.stacks.Count; i++)
			//{
			//	UIContainerSlot slot = new UIContainerSlot(bag.handler, i);
			//	gridItems.Add(slot);
			//}
		}
	}
}