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
	public class BuilderReserve : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/BuilderReserve";

		public int selectedIndex;
		public Item SelectedItem => selectedIndex >= 0 ? Handler.GetItemInSlot(selectedIndex) : null;

		public BuilderReserve()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => (item.createTile >= 0 || item.createWall >= 0) && (Handler.GetItemInSlot(slot).type == item.type || !Handler.Contains(item.type));
			Handler.GetSlotLimit += slot => int.MaxValue;

			selectedIndex = -1;
		}

		public override ModItem Clone()
		{
			BuilderReserve clone = (BuilderReserve)base.Clone();
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
			item.rare = ItemRarityID.Orange;
			item.value = 12000 * 5;
		}

		public override bool CanUseItem(Player player) => SelectedItem != null && SelectedItem.type > 0 && SelectedItem.stack > 1;

		public override bool UseItem(Player player) => false;

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(Handler.Slots)));
		}

		public void SetIndex(int index)
		{
			if (index == -1) selectedIndex = item.placeStyle = item.createTile = item.createWall = -1;
			else
			{
				selectedIndex = index;
				if (SelectedItem.createTile >= 0)
				{
					item.createTile = SelectedItem.createTile;
					item.createWall = -1;
					item.placeStyle = SelectedItem.placeStyle;
				}
				else if (SelectedItem.createWall >= 0)
				{
					item.createTile = -1;
					item.createWall = SelectedItem.createWall;
					item.placeStyle = SelectedItem.placeStyle;
				}
			}
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
			SetIndex(tag.GetInt("SelectedIndex"));
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