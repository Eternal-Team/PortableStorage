using ContainerLibrary;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Items.Ammo
{
	public class Wallet : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/Wallet";

		public override string AmmoType => "Coin";

		public long Coins
		{
			get => Handler.CoinsValue();
			set
			{
				Item[] coins = Utils.CoinsSplit(value).Select((stack, index) =>
				{
					Item coin = new Item();
					coin.SetDefaults(ItemID.CopperCoin + index);
					coin.stack = stack;
					return coin;
				}).Reverse().ToArray();

				for (int i = 0; i < Handler.Slots; i++) Handler.SetItemInSlot(i, coins[i]);

				item.SyncBag();
			}
		}

		public Wallet()
		{
			Handler = new ItemHandler(4);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => item.type == ItemID.PlatinumCoin - slot;
			Handler.GetSlotLimit += slot => int.MaxValue;
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