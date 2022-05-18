using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public abstract class BaseNormalBag : BaseBag /*, ICraftingStorage*/
{
	private class NormalBagItemStorage : ItemStorage
	{
		private BaseNormalBag bag;

		public NormalBagItemStorage(BaseNormalBag bag) : base(bag.SlotCount)
		{
			this.bag = bag;
		}

		// public override void OnContentsChanged(int slot, bool user)
		// {
		// 	Recipe.FindRecipes();
		// 	Utility.SyncBag(bag);
		// }

		public override bool IsItemValid(int slot, Item item)
		{
			return item.ModItem is not BaseBag && !item.IsACoin /*&& !(item.ModItem is TheBlackHole)*/;
		}
	}

	protected abstract int SlotCount { get; }

	public override void OnCreate(ItemCreationContext context)
	{
		base.OnCreate(context);

		Storage = new NormalBagItemStorage(this);
	}

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 26;
		Item.height = 34;
	}

	public IEnumerable<int> GetSlotsForCrafting() => Enumerable.Range(0, SlotCount);
}