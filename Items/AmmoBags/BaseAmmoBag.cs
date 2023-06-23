using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

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

	public override void OnCreated(ItemCreationContext context)
	{
		base.OnCreated(context);

		Storage = new AmmoBagItemStorage(9, this);
	}
}