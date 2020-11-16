using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSItem : GlobalItem
	{
		public override bool OnPickup(Item item, Player player)
		{
			if (BagUISystem.Instance.bagState.bag != null)
				Hooking.Hooking.NewText(BagUISystem.Instance.bagState.bag.item, item, item.stack);

			return base.OnPickup(item, player);
		}
	}
}