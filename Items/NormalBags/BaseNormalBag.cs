using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public abstract class BaseNormalBag : BaseBag, ICraftingStorage
{
	private class NormalBagItemStorage : BagStorage
	{
		public NormalBagItemStorage(BaseNormalBag bag) : base(bag, bag.SlotCount)
		{
		}

		public override bool IsItemValid(int slot, Item item)
		{
			return item.ModItem is not BaseBag && !item.IsACoin;
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

	public ItemStorage GetCraftingStorage() => Storage;
}