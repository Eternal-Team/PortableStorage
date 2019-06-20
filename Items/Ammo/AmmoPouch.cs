using Terraria;

namespace PortableStorage.Items.Ammo
{
	public class AmmoPouch : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/AmmoPouch";

		public override string AmmoType => "Misc";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ammo Pouch");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of misc. ammo");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 30;
			item.value = Item.buyPrice(gold: 3);
		}
	}
}