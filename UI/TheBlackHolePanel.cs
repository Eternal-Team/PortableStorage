using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.Items.Special;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace PortableStorage.UI
{
	public class TheBlackHolePanel : BaseBagPanel<TheBlackHole>
	{
		public override void OnInitialize()
		{
			Size = new Vector2(408, 172);
			this.Center();

			UIAnimatedTexture textureActivation = new UIAnimatedTexture(ModContent.GetTexture("PortableStorage/Textures/Items/TheBlackHole"), new DrawAnimationVertical(8, 8), ScaleMode.Stretch)
			{
				Size = new Vector2(20),
				Animate = Bag.active
			};
			textureActivation.GetHoverText += () => Bag.active ? "Deactivate" : "Activate";
			textureActivation.OnClick += (evt, element) =>
			{
				Bag.active = !Bag.active;
				textureActivation.Animate = Bag.active;
				// todo: sync
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