using PortableStorage.TileEntities;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utils;

namespace PortableStorage.UI
{
	public class QEChestUI : BaseUI, ITileEntityUI, IContainerUI
	{
		public UIText textLabel = new UIText("Quantum Entangled Chest");
		public UITextButton buttonClose = new UITextButton("X", 4);
		public UIGrid gridItems = new UIGrid(9);

		public TEQEChest qeChest;

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = 408;
			panelMain.Height.Pixels = 172;
			panelMain.Center();
			panelMain.SetPadding(0);
			panelMain.BackgroundColor = PanelColor;
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel.HAlign = 0.5f;
			textLabel.Top.Pixels = 8;
			panelMain.Append(textLabel);

			buttonClose.Width.Pixels = 24;
			buttonClose.Height.Pixels = 24;
			buttonClose.Left.Set(-28, 1);
			buttonClose.Top.Pixels = 8;
			buttonClose.OnClick += (evt, element) =>
			{
				qeChest.opened = !qeChest.opened;
				PortableStorage.Instance.CloseUI(qeChest.ID);
				Main.PlaySound(SoundID.DD2_EtherianPortalOpen.WithVolume(0.5f));
			};
			panelMain.Append(buttonClose);

			gridItems.Width.Set(-16, 1);
			gridItems.Height.Set(-36, 1);
			gridItems.Left.Pixels = 8;
			gridItems.Top.Pixels = 36;
			gridItems.ListPadding = 4;
			gridItems.OverflowHidden = true;
			panelMain.Append(gridItems);
		}

		public override void Load()
		{
			for (int i = 0; i < qeChest.GetItems().Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(qeChest, i);
				gridItems.Add(slot);
			}
		}

		public void SetTileEntity(ModTileEntity tileEntity) => qeChest = (TEQEChest)tileEntity;

		public void SetContainer(IContainer container) => qeChest = (TEQEChest)container;

		public IContainer GetContainer() => qeChest;
	}
}