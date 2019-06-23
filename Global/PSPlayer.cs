using PortableStorage.Items.Special;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSPlayer : ModPlayer
	{
		public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
		{
			if (junk) return;

			if (liquidType == 0 && worldLayer == 3 && Main.rand.NextBool(200 / (power / 100))) caughtType = mod.ItemType<FishingBelt>();
		}
	}
}