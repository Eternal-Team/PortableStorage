using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TheOneLibrary.Base.Items;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;

namespace PortableStorage.Items
{
	public abstract class BaseBag : BaseItem, IContainerItem
	{
		public override bool CloneNewInstances => true;

		public GUI gui;

		public void SetupUI<T>() where T : BaseUI, IContainerUI
		{
			if (Main.netMode != NetmodeID.Server)
			{
				T ui = Activator.CreateInstance<T>();
				ui.SetContainer(this);
				UserInterface userInterface = new UserInterface();
				ui.Activate();
				userInterface.SetState(ui);
				gui = new GUI<T>(ui, userInterface);
			}
		}

		public virtual void HandleUI(LegacySoundStyle sound = null)
		{
			if (!PortableStorage.Instance.BagUI.ContainsValue(gui))
			{
				gui.ui.Load();
				PortableStorage.Instance.BagUI.Add(item.modItem, gui);
			}
			else
			{
				gui.ui.Unload();
				PortableStorage.Instance.BagUI.Remove(PortableStorage.Instance.BagUI.First(kvp => kvp.Value == gui).Key);
			}

			Main.PlaySound(sound ?? SoundID.Item59);
		}

		public abstract List<Item> GetItems();

		public abstract Item GetItem(int slot);

		public abstract void SetItem(int slot, Item value);

		public abstract void Sync(int slot = 0);

		public abstract ModItem GetModItem();
	}
}