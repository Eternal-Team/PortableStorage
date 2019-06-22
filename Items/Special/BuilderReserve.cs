using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Global;
using PortableStorage.UI;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Special
{
	// todo: custom mouse icon for selecting tiles

	public class BuilderReserve : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/BuilderReserve";

		public int selectedIndex;

		public BuilderReserve()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => { return (item.createTile >= 0 || item.createWall >= 0) && (Handler.Items.All(x => x.type != item.type) || Handler.Items[slot].type == item.type); };
			Handler.GetSlotLimit += slot => int.MaxValue;

			selectedIndex = -1;
		}

		public override ModItem Clone()
		{
			BuilderReserve clone = (BuilderReserve)base.Clone();
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

		public override bool CanUseItem(Player player) => selectedIndex >= 0 && Handler.Items[selectedIndex].type > 0 && Handler.Items[selectedIndex].stack > 1;

		public override bool UseItem(Player player) => false;

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

			(UI as BuilderReservePanel)?.RefreshTextures();
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (selectedIndex < 0) return;

			Item selectedItem = Handler.Items[selectedIndex];
			if (selectedItem.IsAir) return;

			spriteBatch.DrawItemInInventory(selectedItem, position + new Vector2(16) * scale, new Vector2(16) * scale, false);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (selectedIndex < 0) return;

			Item selectedItem = Handler.Items[selectedIndex];
			if (selectedItem.IsAir) return;

			spriteBatch.DrawItem(selectedItem, item.position - Main.screenPosition + new Vector2(16) * scale + new Vector2(0, 2), new Vector2(16) * scale, rotation, scale, Color.White);
		}

		public override TagCompound Save()
		{
			TagCompound tag = base.Save();
			tag["SelectedIndex"] = selectedIndex;
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			base.Load(tag);
			selectedIndex = tag.GetInt("SelectedIndex");
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