//using System.IO;
//using PortableStorage.Global;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria.ModLoader.IO;

//namespace PortableStorage.Items.Bags
//{
//	public class MasterAmmoPounch : BaseAmmoBag
//	{
//		public override string AmmoType => "All";

//		public override void SetStaticDefaults()
//		{
//			DisplayName.SetDefault("Master Ammo Pouch");
//			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of ammo");
//		}

//		public override void SetDefaults()
//		{
//			base.SetDefaults();

//			item.width = 32;
//			item.height = 32;
//		}

//		public override TagCompound Save() => new TagCompound
//		{
//			["Items"] = Handler.Save()
//		};

//		public override void Load(TagCompound tag)
//		{
//			Handler.Load(tag.GetCompound("Items"));
//		}

//		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

//		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));

//		public override void AddRecipes()
//		{
//			ModRecipe recipe = new ModRecipe(mod);
//			recipe.AddRecipeGroup(Utility.tier1HMBarsGroup.GetText(), 15);
//			recipe.AddRecipeGroup(Utility.ichorFlameGroup.GetText(), 10);
//			recipe.AddIngredient(ItemID.SoulofNight, 7);
//			recipe.AddTile(TileID.MythrilAnvil);
//			recipe.SetResult(this);
//			recipe.AddRecipe();
//		}
//	}
//}

