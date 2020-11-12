using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;

namespace PortableStorage.Items
{
	public abstract class BaseNormalBag : BaseBag
	{
		private class NormalBagHandler : ItemHandler
		{
			public NormalBagHandler(int slots) : base(slots)
			{
				
			}
			
			public override void OnContentsChanged(int slot, bool user)
			{
				Recipe.FindRecipes();
				// sync
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
			
			Handler = new NormalBagHandler(SlotCount);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 34;
		}
	}
}