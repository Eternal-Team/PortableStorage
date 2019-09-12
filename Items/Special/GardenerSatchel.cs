using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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
			Handler = new ItemHandler(18);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => Utility.SeedWhitelist.Contains(item.type) && (Handler.GetItemInSlot(slot).type == item.type || !Handler.Contains(item.type));
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
			item.rare = ItemRarityID.Lime;
			item.value = Item.sellPrice(gold: 5);
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

				Item.NewItem(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16, seed, WorldGen.genRand.Next(0, 5));
				Item.NewItem(Player.tileTargetX * 16, Player.tileTargetY * 16, 16, 16, drop, WorldGen.genRand.Next(1, 3));

				WorldGen.KillTile(i, j, noItem: true);

				WorldGen.PlaceAlch(i, j, style);

				return true;
			}

			if (SelectedItem != null && !SelectedItem.IsAir)
			{
				switch (SelectedItem.type)
				{
					case ItemID.DaybloomSeeds:
					case ItemID.MoonglowSeeds:
					case ItemID.BlinkrootSeeds:
					case ItemID.DeathweedSeeds:
					case ItemID.WaterleafSeeds:
					case ItemID.FireblossomSeeds:
					case ItemID.ShiverthornSeeds:
						if (WorldGen.PlaceAlch(i, j, SelectedItem.type == ItemID.ShiverthornSeeds ? 6 : SelectedItem.type - 307))
						{
							Handler.Shrink(selectedIndex, 1);
							if (SelectedItem.IsAir) selectedIndex = -1;
							return true;
						}

						return false;
					case ItemID.CorruptSeeds:
					case ItemID.CrimsonSeeds:
						if (Main.tile[i, j].active() && Main.tile[i, j].type == TileID.Dirt)
						{
							WorldGen.PlaceTile(i, j, TileID.CorruptGrass);

							Handler.Shrink(selectedIndex, 1);
							if (SelectedItem.IsAir) selectedIndex = -1;
						}

						return false;

					case ItemID.JungleGrassSeeds:
						if (Main.tile[i, j].active() && Main.tile[i, j].type == TileID.Mud)
						{
							WorldGen.PlaceTile(i, j, TileID.JungleGrass);

							Handler.Shrink(selectedIndex, 1);
							if (SelectedItem.IsAir) selectedIndex = -1;
						}

						return false;
					case ItemID.MushroomGrassSeeds:
						if (Main.tile[i, j].active() && Main.tile[i, j].type == TileID.Mud)
						{
							WorldGen.PlaceTile(i, j, TileID.MushroomGrass);

							Handler.Shrink(selectedIndex, 1);
							if (SelectedItem.IsAir) selectedIndex = -1;
						}

						return false;
					case ItemID.HallowedSeeds:
						if (Main.tile[i, j].active() && Main.tile[i, j].type == TileID.Dirt)
						{
							WorldGen.PlaceTile(i, j, TileID.HallowedGrass);

							Handler.Shrink(selectedIndex, 1);
							if (SelectedItem.IsAir) selectedIndex = -1;
						}

						return false;
					case ItemID.GrassSeeds:
						if (Main.tile[i, j].active() && Main.tile[i, j].type == TileID.Dirt)
						{
							WorldGen.PlaceTile(i, j, TileID.Grass);

							Handler.Shrink(selectedIndex, 1);
							if (SelectedItem.IsAir) selectedIndex = -1;
						}

						return false;
					case ItemID.Acorn:
						if (WorldGen.PlaceObject(i, j, TileID.Saplings))
						{
							Handler.Shrink(selectedIndex, 1);
							if (SelectedItem.IsAir) selectedIndex = -1;
							return true;
						}

						return false;
					case ItemID.PumpkinSeed:
						if (PlacePumpkin(i, j))
						{
							Handler.Shrink(selectedIndex, 1);
							if (SelectedItem.IsAir) selectedIndex = -1;
							return true;
						}

						return false;
				}
			}

			return false;
		}

		private static bool PlacePumpkin(int x, int superY)
		{
			const ushort type = 254;
			int num = WorldGen.genRand.Next(6) * 36;
			if (x < 5 || x > Main.maxTilesX - 5 || superY < 5 || superY > Main.maxTilesY - 5) return false;

			bool canPlace = true;
			for (int i = x - 1; i < x + 1; i++)
			{
				for (int j = superY - 1; j < superY + 1; j++)
				{
					if (Main.tile[i, j] == null) Main.tile[i, j] = new Tile();

					if (Main.tile[i, j].active() && Main.tile[i, j].type != 3 && Main.tile[i, j].type != 73 && Main.tile[i, j].type != 113 && Main.tile[i, j].type != 110 && (Main.tile[i, j].type != 185 || Main.tile[i, j].frameY != 0)) canPlace = false;

					if (Main.tile[i, j].liquid > 0) canPlace = false;
				}

				if (!WorldGen.SolidTile(i, superY + 1) || Main.tile[i, superY + 1].type != 2 && Main.tile[i, superY + 1].type != 109) canPlace = false;
			}

			if (canPlace)
			{
				Main.tile[x - 1, superY - 1].active(true);
				Main.tile[x - 1, superY - 1].frameY = (short)num;
				Main.tile[x - 1, superY - 1].frameX = 0;
				Main.tile[x - 1, superY - 1].type = type;
				Main.tile[x, superY - 1].active(true);
				Main.tile[x, superY - 1].frameY = (short)num;
				Main.tile[x, superY - 1].frameX = 18;
				Main.tile[x, superY - 1].type = type;
				Main.tile[x - 1, superY].active(true);
				Main.tile[x - 1, superY].frameY = (short)(num + 18);
				Main.tile[x - 1, superY].frameX = 0;
				Main.tile[x - 1, superY].type = type;
				Main.tile[x, superY].active(true);
				Main.tile[x, superY].frameY = (short)(num + 18);
				Main.tile[x, superY].frameX = 18;
				Main.tile[x, superY].type = type;

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
			tooltips.Add(new TooltipLine(mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(Handler.Slots)));
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
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.StaffofRegrowth);
			recipe.AddIngredient(ItemID.AncientBattleArmorMaterial);
			recipe.AddIngredient(ItemID.ChlorophyteBar, 3);
			recipe.AddIngredient(ItemID.AncientCloth, 7);
			recipe.AddTile(TileID.LivingLoom);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}