using System.IO;
using PortableStorage.UI.Bags;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class Magazine : BaseAmmoBag<AmmoBagPanel>
	{
		public override string Texture => "PortableStorage/Textures/Items/Magazine";

		public override string AmmoType => "Bullet";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magazine");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of bullets");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 18;
			item.height = 32;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Items"] = Handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			Handler.Load(tag.GetCompound("Items"));
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IronBar, 10);
			recipe.AddIngredient(ItemID.Obsidian, 10);
			recipe.anyIronBar = true;
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}