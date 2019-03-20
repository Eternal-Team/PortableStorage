using PortableStorage.Items.Bags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PortableStorageNPC : GlobalNPC
	{
		public override void SetupTravelShop(int[] shop, ref int nextSlot)
		{
			shop[nextSlot] = mod.ItemType<Wallet>();
			nextSlot++;
		}

		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (type == NPCID.Cyborg)
			{
				shop.item[nextSlot].SetDefaults(mod.ItemType<FireProofContainer>());
				nextSlot++;
			}
		}
	}
}