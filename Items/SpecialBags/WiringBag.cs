using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public class WiringBag: BaseBag
{
	private class WiringBagItemStorage : BagStorage
	{
		public WiringBagItemStorage(int slots, BaseBag bag) : base(bag, slots)
		{
		}

		public override bool IsItemValid(int slot, Item item)
		{
			return Utility.WiringWhitelist.Contains(item.type);
		}
	}

	// public override string Texture => PortableStorage.AssetPath + "Textures/Items/WiringBag";

	public override void OnCreated(ItemCreationContext context)
	{
		base.OnCreated(context);
	
		Storage = new WiringBagItemStorage(18, this);
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