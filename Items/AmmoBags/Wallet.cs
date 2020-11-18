using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;

namespace PortableStorage.Items
{
	public class Wallet : BaseAmmoBag
	{
		public override string Texture => PortableStorage.AssetPath + "Textures/Items/Wallet";

		protected override string AmmoType => "Coin";

		public long Coins
		{
			get => Storage.CountCoins();
			set
			{
				Item[] coins = Utils.CoinsSplit(value).Select((stack, index) =>
				{
					Item coin = new Item();
					coin.SetDefaults(ItemID.CopperCoin + index);
					coin.stack = stack;
					return coin;
				}).Reverse().ToArray();

				for (int i = 0; i < Storage.Length; i++) Storage.SetItemInSlot(i, coins[i]);

				Recipe.FindRecipes();
				Utility.SyncBag(this);
			}
		}

		public override void OnCreate(ItemCreationContext context)
		{
			base.OnCreate(context);

			// todo: allow for custom currencies
			Storage = new AmmoBagItemStorage(4, this);
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