using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;

namespace PortableStorage.Items.SpecialBags
{
	public class MinerBackpack : BaseBag
	{
		private class MinerBackpackItemStorage : ItemStorage
		{
			private MinerBackpack bag;

			public MinerBackpackItemStorage(int slots, MinerBackpack bag) : base(slots)
			{
				this.bag = bag;
			}

			public override void OnContentsChanged(int slot, bool user)
			{
				Recipe.FindRecipes();
				Utility.SyncBag(bag);
			}

			public override bool IsItemValid(int slot, Item item)
			{
				return Utility.OreWhitelist.Contains(item.type) || Utility.ExplosiveWhitelist.Contains(item.type);
			}
		}

		public override string Texture => PortableStorage.AssetPath + "Textures/Items/MinersBackpack";

		public override void OnCreate(ItemCreationContext context)
		{
			Storage = new MinerBackpackItemStorage(18, this);
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Green;
			item.value = Item.buyPrice(gold: 1);
		}
	}
}