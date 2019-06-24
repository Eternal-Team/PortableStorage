using Terraria;

namespace PortableStorage.Items.Ammo
{
	public class FireProofContainer : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/FireProofContainer";

		public override string AmmoType => "Flammable";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fire-proof Container");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of explosive or flammable ammo");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 26;
			item.value = Item.buyPrice(gold: 20);
		}
	}
}