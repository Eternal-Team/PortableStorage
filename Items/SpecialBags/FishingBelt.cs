using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public class FishingBelt : BaseBag
{
	private class FishingBeltItemStorage : BagStorage
	{
		public FishingBeltItemStorage(int slots, BaseBag bag) : base(bag, slots)
		{
		}

		public override bool IsItemValid(int slot, Item item)
		{
			return item.fishingPole > 0 || item.bait > 0 || Utility.FishingWhitelist.Contains(item.type);
		}
	}

	public override string Texture => PortableStorage.AssetPath + "Textures/Items/FishingBelt";

	public override void OnCreated(ItemCreationContext context)
	{
		base.OnCreated(context);
		
		Storage = new FishingBeltItemStorage(18, this);
	}

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 32;
		Item.height = 26;
		Item.rare = ItemRarityID.Orange;
		Item.value = 25000 * 5;
	}
}