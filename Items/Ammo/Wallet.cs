using BaseLibrary;
using ContainerLibrary;
using PortableStorage.Global;
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
			get => Handler.Items.CountCoins();
			set
			{
				Handler.Items = Utils.CoinsSplit(value).Select((stack, index) =>
				{
					Item coin = new Item();
					coin.SetDefaults(ItemID.CopperCoin + index);
					coin.stack = stack;
					return coin;
				}).Reverse().ToList();

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