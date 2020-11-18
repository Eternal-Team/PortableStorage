using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;

namespace PortableStorage.Items
{
	public abstract class BaseAmmoBag : BaseBag
	{
		protected class AmmoBagItemStorage : ItemStorage
		{
			private BaseAmmoBag bag;

			public AmmoBagItemStorage(int slots, BaseAmmoBag bag) : base(slots)
			{
				this.bag = bag;
			}

			public override void OnContentsChanged(int slot, bool user)
			{
				Recipe.FindRecipes();
				Utility.SyncBag(bag);
			}

			public override bool IsItemValid(int slot, Item item)
			{
				return Utility.Ammos[bag.AmmoType].Any(group => group.AmmoItems.Contains(item.type));
			}
		}

		protected abstract string AmmoType { get; }

		public override void OnCreate(ItemCreationContext context)
		{
			base.OnCreate(context);

			Storage = new AmmoBagItemStorage(9, this);
		}
	}
}