using BaseLibrary;
using ContainerLibrary;
using PortableStorage.Items.Special;

namespace PortableStorage.Items.Normal
{
	public abstract class BaseNormalBag : BaseBag
	{
		public abstract int SlotCount { get; }
		public new abstract string Name { get; }

		public BaseNormalBag()
		{
			Handler = new ItemHandler(SlotCount);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => !(item.modItem is BaseBag) || !item.IsCoin() || !(item.modItem is TheBlackHole);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 34;
		}
	}
}