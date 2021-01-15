using Terraria;
using Terraria.ID;

namespace PortableStorage.Items
{
	public class AmmoPouch : BaseAmmoBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/AmmoPouch";

		protected override string AmmoType => "Misc";

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.width = 28;
			Item.height = 30;
			Item.value = Item.buyPrice(gold: 3);
			Item.rare = ItemRarityID.White;
		}
	}
}