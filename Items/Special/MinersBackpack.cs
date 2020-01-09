using ContainerLibrary;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Items.Special
{
	public class MinersBackpack : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/MinersBackpack";

		public MinersBackpack()
		{
			Handler = new ItemHandler(18);
			Handler.OnContentsChanged += slot =>
			{
				Recipe.FindRecipes();
				item.SyncBag();
			};
			Handler.IsItemValid += (slot, item) => Utility.OreWhitelist.Contains(item.type) || Utility.ExplosiveWhitelist.Contains(item.type);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Green;
			item.value = 10000 * 5;
		}
	}
}