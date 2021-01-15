using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;

namespace PortableStorage.Items
{
	public class Wallet : BaseAmmoBag
	{
		public class WalletItemStorage : ItemStorage
		{
			private Wallet wallet;

			public WalletItemStorage(int size, Wallet wallet) : base(size)
			{
				this.wallet = wallet;
			}

			public override bool IsItemValid(int slot, Item item)
			{
				switch (slot)
				{
					case 0:
						return item.type == ItemID.PlatinumCoin;
					case 1:
						return item.type == ItemID.GoldCoin;
					case 2:
						return item.type == ItemID.SilverCoin;
					case 3:
						return item.type == ItemID.CopperCoin;
					default:
						return CustomCurrencyManager.IsCustomCurrency(item);
				}
			}

			public override int GetSlotSize(int slot, Item item) => int.MaxValue;
		}

		public override string Texture => PortableStorage.AssetPath + "Textures/Items/Wallet";

		protected override string AmmoType => "Coin";

		public override void OnCreate(ItemCreationContext context)
		{
			base.OnCreate(context);

			Storage = new WalletItemStorage(18, this);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			Item.width = 28;
			Item.height = 28;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Blue;
		}
	}
}