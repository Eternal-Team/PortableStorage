using PortableStorage.Items.Ammo;
using PortableStorage.Items.Normal;
using PortableStorage.Items.Special;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSNPC : GlobalNPC
	{
		public override void SetupTravelShop(int[] shop, ref int nextSlot)
		{
			if (Main.rand.NextBool(10))
			{
				shop[nextSlot] = mod.ItemType<Wallet>();
				nextSlot++; 
			}
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
				case NPCID.SkeletonMerchant:
					shop.item[nextSlot].SetDefaults(mod.ItemType<SkeletalBag>());
					nextSlot++;
					break;
				case NPCID.WitchDoctor when Main.LocalPlayer.ZoneJungle:
					shop.item[nextSlot].SetDefaults(mod.ItemType<DartHolder>());
					nextSlot++;
					break;
			}
		}

		public override void NPCLoot(NPC npc)
		{
			if (npc.type == NPCID.UndeadMiner && Main.rand.NextBool(10)) Item.NewItem(npc.getRect(), mod.ItemType<MinersBackpack>());
		}
	}
}