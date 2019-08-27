using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace PortableStorage.Items.Special
{
	public class GardenerSatchel : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/GardenerSatchel";

		public int selectedIndex;
		public Item SelectedItem => selectedIndex >= 0 ? Handler.GetItemInSlot(selectedIndex) : null;

		public GardenerSatchel()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => Utility.SeedWhitelist.Contains(item.type) && Handler.GetItemInSlot(slot).type == item.type || !Handler.Contains(item.type);
			Handler.GetSlotLimit += slot => int.MaxValue;

			selectedIndex = -1;
		}

		public override ModItem Clone()
		{
			GardenerSatchel clone = (GardenerSatchel)base.Clone();
			clone.selectedIndex = selectedIndex;
			return clone;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.autoReuse = true;
			item.useTurn = true;
			item.useTime = 20;
			item.useAnimation = 20;
			item.rare = ItemRarityID.Orange;
			item.value = 12000 * 5;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player) => true;

		public override bool UseItem(Player player)
		{
			int i = Player.tileTargetX;
			int j = Player.tileTargetY;
			
			ref Tile tile = ref Main.tile[i, j];
			if (tile == null) tile = new Tile();

			if (tile.active() && (tile.type == TileID.Plants || tile.type == TileID.Plants2)) WorldGen.KillTile(i, j);

			if (SelectedItem != null && !SelectedItem.IsAir && WorldGen.PlaceAlch(i, j, SelectedItem.type == ItemID.ShiverthornSeeds ? 6 : SelectedItem.type - 307))
			{
				Handler.Shrink(selectedIndex, 1);
				if (SelectedItem.IsAir) selectedIndex = -1;

				return true;
			}

			if (CanHarvest(tile))
			{
				int style = tile.frameX / 18;
				int drop = 313 + style;
				int seed = 307 + style;

				if (style == 6)
				{
					drop = 2358;
					seed = 2357;
				}

				Item seedItem = new Item();
				seedItem.SetDefaults(seed);
				seedItem.stack = WorldGen.genRand.Next(0, 5);

				seedItem.position = player.Center;
				seedItem.active = true;

				Hooking.BagItemText(item, seedItem, seedItem.stack, false, false);

				Handler.InsertItem(ref seedItem);

				Item.NewItem(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16, drop, WorldGen.genRand.Next(1, 3));

				WorldGen.KillTile(i, j, noItem: true);

				WorldGen.PlaceAlch(i, j, style);

				return true;
			}

			return false;
		}

		private static bool CanHarvest(Tile tile)
		{
			if (tile.type == TileID.BloomingHerbs) return true;

			if (tile.type == TileID.MatureHerbs)
			{
				int plant = tile.frameX / 18;
				if (plant == 0 && Main.dayTime) return true;
				if (plant == 1 && !Main.dayTime) return true;
				if (plant == 3 && !Main.dayTime && (Main.bloodMoon || Main.moonPhase == 0)) return true;
				if (plant == 4 && (Main.raining || Main.cloudAlpha > 0f)) return true;
				if (plant == 5 && !Main.raining && Main.dayTime && Main.time > 40500.0) return true;
			}

			return false;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			//tooltips.Add(new TooltipLine(mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(Handler.Slots)));
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (SelectedItem == null || SelectedItem.IsAir) return;

			spriteBatch.DrawItemInInventory(SelectedItem, position + new Vector2(16) * scale, new Vector2(16) * scale, false);

			string text = SelectedItem.stack < 1000 ? SelectedItem.stack.ToString() : SelectedItem.stack.ToSI("N1");
			Vector2 size = Main.fontMouseText.MeasureString(text);

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, text, position + new Vector2(16, 32) * scale, Color.White, 0f, size * 0.5f, new Vector2(0.8f) * scale);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (SelectedItem == null || SelectedItem.IsAir) return;

			Vector2 position = item.position - Main.screenPosition + new Vector2(16) * scale + new Vector2(0, 2);

			spriteBatch.DrawItemInWorld(SelectedItem, position, new Vector2(16) * scale, rotation);

			string text = SelectedItem.stack < 1000 ? SelectedItem.stack.ToString() : SelectedItem.stack.ToSI("N1");
			Vector2 size = Main.fontMouseText.MeasureString(text);

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, text, position, Color.White, rotation, new Vector2(size.X * 0.5f, -8f), new Vector2(0.75f) * scale);
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
			//SetIndex(tag.GetInt("SelectedIndex"));
		}

		public override void AddRecipes()
		{
			//ModRecipe recipe = new ModRecipe(mod);
			//recipe.AddIngredient(ItemID.Bone, 30);
			//recipe.AddIngredient(ItemID.IronCrate);
			//recipe.AddTile(TileID.BoneWelder);
			//recipe.SetResult(this);
			//recipe.AddRecipe();
		}
	}
}