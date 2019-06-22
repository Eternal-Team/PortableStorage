using ContainerLibrary;
using PortableStorage.Global;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Special
{
	public class BuilderReserve : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/BuilderReserve";

		public int selectedIndex;

		public BuilderReserve()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => (item.createTile > 0 || item.createWall > 0) && (Handler.Items.All(x => x.type != item.type) || Handler.Items[slot].type == item.type);
			Handler.GetSlotLimit += slot => int.MaxValue;

			selectedIndex = -1;
		}

		public override ModItem Clone()
		{
			BuilderReserve clone = (BuilderReserve) base.Clone();
			clone.selectedIndex = selectedIndex;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Builder's Reserve");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of tiles or walls");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.autoReuse = true;
			item.useTurn = true;
		}

		public override bool CanUseItem(Player player)
		{
			return selectedIndex >= 0 && Handler.Items[selectedIndex].type > 0 && Handler.Items[selectedIndex].stack > 1;
		}

		public override bool UseItem(Player player)
		{
			return false;
		}

		public void SetIndex(int index)
		{
			if (index == -1) selectedIndex = item.placeStyle = item.createTile = item.createWall = -1;
			else
			{
				selectedIndex = index;
				Item selectedItem = Handler.Items[selectedIndex];

				if (selectedItem.createTile >= 0)
				{
					item.createTile = selectedItem.createTile;
					item.createWall = -1;
					item.placeStyle = selectedItem.placeStyle;
				}
				else if (selectedItem.createWall >= 0)
				{
					item.createTile = -1;
					item.createWall = selectedItem.createWall;
					item.placeStyle = selectedItem.placeStyle;
				}
			}

			//(UI as DevNullPanel)?.RefreshTextures();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bone, 30);
			recipe.AddIngredient(ItemID.IronCrate);
			recipe.AddTile(TileID.BoneWelder);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}