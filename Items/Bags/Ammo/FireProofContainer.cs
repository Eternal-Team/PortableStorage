using PortableStorage.UI.Bags;
using Terraria;

namespace PortableStorage.Items.Bags
{
	public class FireProofContainer : BaseAmmoBag
	{
		public override string AmmoType => "Flameable";

		public new AmmoBagPanel UI;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fire-proof Container");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of explosive or flameable ammo");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.value = Item.buyPrice(gold: 20);
		}
	}
}