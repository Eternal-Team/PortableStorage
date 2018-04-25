using System;
using System.Linq;
using Microsoft.Xna.Framework;
using PortableStorage.Items;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using TheOneLibrary.UI.Elements;
using TheOneLibrary.Utils;

namespace PortableStorage.UI
{
	public class QEBagUI : BaseUI, IContainerUI
	{
		public UIText textLabel = new UIText("Quantum Entangled Bag");

		public UIButton buttonLootAll = new UIButton(PortableStorage.lootAll);
		public UIButton buttonDepositAll = new UIButton(PortableStorage.depositAll);

		public UIButton buttonRestock = new UIButton(PortableStorage.restock);

		public UITextButton buttonClose = new UITextButton("X", 4);

		public UIGrid gridItems = new UIGrid(9);

		public UIColor[] colorFrequency = new UIColor[3];

		public IContainer bag;

		public override void OnInitialize()
		{
			panelMain.Width.Pixels = 408;
			panelMain.Height.Pixels = 172;
			panelMain.Center();
			panelMain.SetPadding(0);
			panelMain.BackgroundColor = TheOneLibrary.Utils.Utility.PanelColor;
			panelMain.OnMouseDown += DragStart;
			panelMain.OnMouseUp += DragEnd;
			Append(panelMain);

			textLabel.HAlign = 0.5f;
			textLabel.Top.Pixels = 8;
			panelMain.Append(textLabel);

			buttonLootAll.Width.Pixels = 24;
			buttonLootAll.Height.Pixels = 24;
			buttonLootAll.Left.Pixels = 8;
			buttonLootAll.Top.Pixels = 8;
			buttonLootAll.HoverText += () => Language.GetTextValue("LegacyInterface.29");
			buttonLootAll.OnClick += LootAllClick;
			panelMain.Append(buttonLootAll);

			buttonDepositAll.Width.Pixels = 24;
			buttonDepositAll.Height.Pixels = 24;
			buttonDepositAll.Left.Pixels = 40;
			buttonDepositAll.Top.Pixels = 8;
			buttonDepositAll.HoverText += () => Language.GetTextValue("LegacyInterface.30");
			buttonDepositAll.OnClick += DepositAllClick;
			panelMain.Append(buttonDepositAll);

			buttonRestock.Width.Pixels = 24;
			buttonRestock.Height.Pixels = 24;
			buttonRestock.Left.Pixels = 72;
			buttonRestock.Top.Pixels = 8;
			buttonRestock.HoverText += () => Language.GetTextValue("LegacyInterface.82");
			buttonRestock.OnClick += Restock;
			panelMain.Append(buttonRestock);

			for (int i = 0; i < colorFrequency.Length; i++)
			{
				colorFrequency[i] = new UIColor(Color.White);
				colorFrequency[i].Width.Pixels = 16;
				colorFrequency[i].Height.Pixels = 16;
				colorFrequency[i].Left.Set(-52f - i * 20f, 1f);
				colorFrequency[i].Top.Pixels = 12;
				panelMain.Append(colorFrequency[i]);
			}

			buttonClose.Width.Pixels = 24;
			buttonClose.Height.Pixels = 24;
			buttonClose.Left.Set(-28, 1);
			buttonClose.Top.Pixels = 8;
			buttonClose.OnClick += (evt, element) =>
			{
				PortableStorage.Instance.BagUI.Remove(PortableStorage.Instance.BagUI.First(kvp => kvp.Value.ui == this).Key);

				Main.PlaySound(SoundID.DD2_EtherianPortalOpen.WithVolume(0.5f));
			};
			panelMain.Append(buttonClose);

			gridItems.Width.Set(-16, 1);
			gridItems.Height.Set(-44, 1);
			gridItems.Left.Pixels = 8;
			gridItems.Top.Pixels = 36;
			gridItems.ListPadding = 4;
			gridItems.OverflowHidden = true;
			panelMain.Append(gridItems);
		}

		private void LootAllClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.LocalPlayer.chest == -1 && Main.npcShop == 0)
			{
				TheOneLibrary.Utils.Utility.LootAll(bag);
				Recipe.FindRecipes();
			}
		}

		private void DepositAllClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.LocalPlayer.chest == -1 && Main.npcShop == 0)
			{
				TheOneLibrary.Utils.Utility.DepositAll(bag);
				Recipe.FindRecipes();
			}
		}

		private void Restock(UIMouseEvent evt, UIElement listeningElement)
		{
			if (Main.LocalPlayer.chest == -1 && Main.npcShop == 0)
			{
				TheOneLibrary.Utils.Utility.Restock(bag);
				Recipe.FindRecipes();
			}
		}

		public override void Load()
		{
			gridItems.Clear();
			for (int i = 0; i < 27; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(bag, i);
				gridItems.Add(slot);
			}
		}

		public override void Update(GameTime gameTime)
		{
			for (int i = 0; i < colorFrequency.Length; i++) colorFrequency[2 - i].color = typeof(Color).GetValue<Color>(Enum.GetName(typeof(Colors), ((QEBag)bag).frequency.Colors[i]));

			base.Update(gameTime);
		}

		public void SetContainer(IContainer container) => bag = container;

		public IContainer GetContainer() => bag;
	}
}