using ContainerLibrary;
using PortableStorage.Global;
using Utility = PortableStorage.Global.Utility;

namespace PortableStorage.Items.Special
{
	public class MinersBackpack : BaseBag
	{
		public MinersBackpack()
		{
			Handler = new ItemHandler(18);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => Utility.OreWhitelist.Contains(item.type);
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Miner's Backpack");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of ore");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
		}
	}
}