using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TheOneLibrary.Base.Items;
using TheOneLibrary.Storage;

namespace PortableStorage.Items
{
	public abstract class BaseBag : BaseItem, IContainerItem
	{
		public override bool CloneNewInstances => true;

		public abstract List<Item> GetItems();

		public abstract Item GetItem(int slot);

		public abstract void SetItem(int slot, Item value);

		public abstract void Sync(int slot = 0);

		public abstract ModItem GetModItem();
	}
}