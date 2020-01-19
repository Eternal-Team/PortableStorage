using BaseLibrary;
using ContainerLibrary;
using PortableStorage.Items.Special;
using Terraria;

namespace PortableStorage.Items.Normal
{
	public abstract class BaseNormalBag : BaseBag
	{
		public abstract int SlotCount { get; }

		public BaseNormalBag()
		{
			Handler = new ItemHandler(SlotCount);
			Handler.OnContentsChanged += (slot, user) =>
			{
				Recipe.FindRecipes();
				item.SyncBag();
			};
			Handler.IsItemValid += (slot, item) => !(item.modItem is BaseBag) && !item.IsCoin() && !(item.modItem is TheBlackHole);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 34;
		}
	}
}