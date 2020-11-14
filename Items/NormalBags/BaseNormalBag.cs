using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;

namespace PortableStorage.Items
{
	public abstract class BaseNormalBag : BaseBag, ICraftingStorage
	{
		private class NormalBagHandler : ItemHandler
		{
			private BaseNormalBag bag;

			public NormalBagHandler(BaseNormalBag bag) : base(bag.SlotCount)
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
				return !(item.modItem is BaseBag) && !item.IsACoin /*&& !(item.modItem is TheBlackHole)*/;
			}
		}

		protected abstract int SlotCount { get; }

		public override void OnCreate(ItemCreationContext context)
		{
			base.OnCreate(context);

			Handler = new NormalBagHandler(this);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 34;
		}

		public IEnumerable<int> GetSlotsForCrafting() => Enumerable.Range(0, SlotCount);
	}
}