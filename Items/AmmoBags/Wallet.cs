using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items
{
	public class Wallet : BaseAmmoBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/Wallet";

		protected override string AmmoType => "Coin";

		public override void OnCreate(ItemCreationContext context)
		{
			base.OnCreate(context);

			// todo: allow for custom currencies
			Handler = new AmmoBagItemHandler(4, this);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 28;
			item.value = Item.buyPrice(gold: 5);
			item.rare = ItemRarityID.Blue;
		}
	}
}