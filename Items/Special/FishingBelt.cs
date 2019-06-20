using ContainerLibrary;
using PortableStorage.Global;
using Utility = PortableStorage.Global.Utility;

namespace PortableStorage.Items.Special
{
	public class FishingBelt : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/FishingBelt";

		public FishingBelt()
		{
			Handler = new ItemHandler(18);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => item.fishingPole > 0 || item.bait > 0 || Utility.FishingWhitelist.Contains(item.type);
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fishing Belt");
			Tooltip.SetDefault("Stores fishing poles, bait and fish");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 26;
		}
	}
}