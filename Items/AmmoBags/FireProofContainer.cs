using Terraria;
using Terraria.ID;

namespace PortableStorage.Items
{
	public class FireProofContainer : BaseAmmoBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/FireProofContainer";

		protected override string AmmoType => "Flammable";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 26;
			item.value = Item.buyPrice(gold: 20);
			item.rare = ItemRarityID.Lime;
		}
	}
}