using PortableStorage.UI.Bags;

namespace PortableStorage.Items.Bags
{
	public class AmmoPouch : BaseAmmoBag
	{
		public override string AmmoType => "Misc";

		public new AmmoBagPanel UI;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ammo Pouch");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of misc. ammo");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
		}
	}
}