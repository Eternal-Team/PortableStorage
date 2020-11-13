using Terraria;
using Terraria.ID;

namespace PortableStorage.Items
{
	public class DartHolder : BaseAmmoBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/DartHolder";

		protected override string AmmoType => "Dart";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 28;
			item.value = Item.buyPrice(gold: 15);
			item.rare = ItemRarityID.Orange;
		}
	}
}