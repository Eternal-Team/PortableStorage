using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items.Bags;
using Terraria;

namespace PortableStorage.UI.Bags
{
	public class TheBlackHolePanel : BaseBagPanel<TheBlackHole>
	{
		public override void OnInitialize()
		{
			Size = new Vector2(408, 172);
			this.Center();

			UITexture textureActivation = new UITexture(Main.extraTexture[50], ScaleMode.Stretch)
			{
				Size = new Vector2(20)
			};
			textureActivation.GetHoverText += () => ((TheBlackHole)Bag).active ? "Deactivate" : "Activate";
			textureActivation.OnClick += (evt, element) =>
			{
				TheBlackHole blackHole = (TheBlackHole)Bag;
				blackHole.active = !blackHole.active;
				// todo: sync
			};
			textureActivation.OnPreDraw += spriteBatch =>
			{
				TheBlackHole blackHole = (TheBlackHole)Bag;
				textureActivation.Rotation = blackHole.active ? blackHole.angle : 0f;
			};
			Append(textureActivation);

			textLabel = new UIText(Bag.DisplayName.GetTranslation())
			{
				HAlign = 0.5f
			};
			Append(textLabel);

			buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => PortableStorage.Instance.PanelUI.UI.CloseUI(Bag);
			Append(buttonClose);

			gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (-28, 1),
				Top = (28, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			for (int i = 0; i < Bag.Handler.stacks.Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Bag, i);
				gridItems.Add(slot);
			}
		}
	}
}