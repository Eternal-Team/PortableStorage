using PortableStorage.Items.Bags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSNPC : GlobalNPC
	{
		public override void SetupTravelShop(int[] shop, ref int nextSlot)
		{
			shop[nextSlot] = mod.ItemType<Wallet>();
			nextSlot++;
		}

		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			switch (type)
			{
				case NPCID.Cyborg:
					shop.item[nextSlot].SetDefaults(mod.ItemType<FireProofContainer>());
					nextSlot++;
					break;
				case NPCID.Merchant:
					shop.item[nextSlot].SetDefaults(mod.ItemType<AmmoPouch>());
					nextSlot++;
					break;
			}
		}
	}
}