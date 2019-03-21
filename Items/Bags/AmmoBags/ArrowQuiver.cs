using System.IO;
using PortableStorage.Global;
using PortableStorage.UI.Bags;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class ArrowQuiver : BaseAmmoBag<AmmoBagPanel>
	{
		public override string Texture => "PortableStorage/Textures/Items/ArrowQuiver";

		public override string AmmoType => "Arrow";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arrow Quiver");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of arrows");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
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
			recipe.AddIngredient(ItemID.Leather, 8);
			recipe.AddRecipeGroup(Utility.yoyoStringGroup.GetText());
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}