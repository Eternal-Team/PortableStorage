using ContainerLibrary;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Items.Special
{
	public class FishingBelt : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/FishingBelt";

		public FishingBelt()
		{
			Handler = new ItemHandler(18);
			Handler.OnContentsChanged += slot =>
			{
				Recipe.FindRecipes();
				item.SyncBag();
			};
			Handler.IsItemValid += (slot, item) => item.fishingPole > 0 || item.bait > 0 || Utility.FishingWhitelist.Contains(item.type);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 26;
			item.rare = ItemRarityID.Orange;
			item.value = 25000 * 5;
		}
	}
}