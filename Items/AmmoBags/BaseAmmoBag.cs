using System.Linq;
using Terraria;

namespace PortableStorage.Items;

public abstract class BaseAmmoBag : BaseBag
{
	protected class AmmoBagItemStorage : BagStorage
	{
		public AmmoBagItemStorage(int slots, BaseBag bag) : base(bag, slots)
		{
		}

		public override bool IsItemValid(int slot, Item item)
		{
			return Utility.Ammos[(bag as BaseAmmoBag)!.AmmoType].Any(group => group.AmmoItems.Contains(item.type));
		}
	}

	protected abstract string AmmoType { get; }

	public BaseAmmoBag()
	{
		Storage = new AmmoBagItemStorage(9, this);
	}

	// public override void OnCreate(ItemCreationContext context)
	// {
	// 	base.OnCreate(context);
	//
	// 	Storage = new AmmoBagItemStorage(9, this);
	// }
}