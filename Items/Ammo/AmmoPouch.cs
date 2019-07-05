using Terraria;
using Terraria.ID;

namespace PortableStorage.Items.Ammo
{
	public class AmmoPouch : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/AmmoPouch";

		public override string AmmoType => "Misc";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 30;
			item.value = Item.buyPrice(gold: 3);
			item.rare = ItemRarityID.White;
		}
	}
}