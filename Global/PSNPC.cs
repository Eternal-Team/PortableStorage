using PortableStorage.Items.Ammo;
using PortableStorage.Items.Normal;
using PortableStorage.Items.Special;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage
{
	public class PSNPC : GlobalNPC
	{
		public override void SetupTravelShop(int[] shop, ref int nextSlot)
		{
			if (Main.rand.NextBool(4))
			{
				shop[nextSlot] = ModContent.ItemType<Wallet>();
				nextSlot++;
			}
		}

		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			switch (type)
			{
				case NPCID.Cyborg:
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<FireProofContainer>());
					nextSlot++;
					break;
				case NPCID.Merchant:
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<AmmoPouch>());
					nextSlot++;
					if (NPC.downedBoss2)
					{
						shop.item[nextSlot].SetDefaults(ModContent.ItemType<AdventurerBag>());
						nextSlot++;
					}

					break;
				case NPCID.SkeletonMerchant:
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<SkeletalBag>());
					nextSlot++;
					break;
				case NPCID.WitchDoctor when Main.LocalPlayer.ZoneJungle:
					shop.item[nextSlot].SetDefaults(ModContent.ItemType<DartHolder>());
					nextSlot++;
					break;
			}
		}

		public override void NPCLoot(NPC npc)
		{
			if (npc.type == NPCID.UndeadMiner && Main.rand.NextBool(5)) Item.NewItem(npc.getRect(), ModContent.ItemType<MinersBackpack>());
		}
	}
}