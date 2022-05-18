// using Terraria;
// using Terraria.ID;
// using Terraria.ModLoader;
// using Terraria.ModLoader.Container;
//
// namespace PortableStorage.Items.SpecialBags;
//
// public class MinerBackpack : BaseBag
// {
// 	private class MinerBackpackItemStorage : ItemStorage
// 	{
// 		private MinerBackpack bag;
//
// 		public MinerBackpackItemStorage(int slots, MinerBackpack bag) : base(slots)
// 		{
// 			this.bag = bag;
// 		}
//
// 		// public void OnContentsChanged(object user, int slot, Item item)
// 		// {
// 		// 	Recipe.FindRecipes();
// 		// 	Utility.SyncBag(bag);
// 		// }
//
// 		public override bool IsItemValid(int slot, Item item)
// 		{
// 			return Utility.OreWhitelist.Contains(item.type) || Utility.ExplosiveWhitelist.Contains(item.type);
// 		}
// 	}
//
// 	public override string Texture => PortableStorage.AssetPath + "Textures/Items/MinersBackpack";
//
// 	public override void OnCreate(ItemCreationContext context)
// 	{
// 		Storage = new MinerBackpackItemStorage(18, this);
// 	}
//
// 	public override void SetDefaults()
// 	{
// 		base.SetDefaults();
//
// 		Item.width = 32;
// 		Item.height = 32;
// 		Item.rare = ItemRarityID.Green;
// 		Item.value = Item.buyPrice(gold: 1);
// 	}
// }