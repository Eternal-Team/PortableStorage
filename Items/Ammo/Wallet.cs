using ContainerLibrary;
using PortableStorage.Global;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Items.Ammo
{
	public class Wallet : BaseAmmoBag
	{
		public override string AmmoType => "Coin";

		public Wallet()
		{
			Handler = new ItemHandler(4);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => item.type == ItemID.PlatinumCoin - slot;
			Handler.GetSlotLimit += slot => int.MaxValue;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wallet");
			Tooltip.SetDefault("Stores coins\nIt is seemingly bottomless!");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 28;
			item.height = 14;
			item.value = Item.buyPrice(gold: 5);
		}
	}
}