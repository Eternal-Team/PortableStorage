using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using TheOneLibrary.Utility;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage.Items
{
	public class VacuumBag : BaseBag, IContainerItem
	{
		public bool active;
		public Guid guid = Guid.NewGuid();
		public IList<Item> Items = new List<Item>();

		public override string Texture => PortableStorage.ItemTexturePath + "VacuumBagActive";

		public override ModItem Clone(Item item)
		{
			VacuumBag clone = (VacuumBag)base.Clone(item);
			clone.Items = Items;
			clone.guid = guid;
			clone.active = active;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vacuum Bag");
			Tooltip.SetDefault("Use the bag or press [c/83fcec:B] while having it in an accessory slot to open it\nRight-click it to enable/disable vacuum mode\nSucks up items!");

			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
		}

		public override void SetDefaults()
		{
			Items.Clear();
			for (int i = 0; i < 27; i++) Items.Add(new Item());

			item.width = 36;
			item.height = 40;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.value = GetItemValue(ItemID.Leather) * 10;
			item.rare = 1;
			item.accessory = true;
		}

		public override void HandleUI()
		{
			if (!PortableStorage.Instance.BagUI.ContainsKey(guid))
			{
				VacuumBagUI ui = new VacuumBagUI();
				UserInterface userInterface = new UserInterface();
				ui.Activate();
				userInterface.SetState(ui);
				ui.visible = true;
				ui.Load(this);
				PortableStorage.Instance.BagUI.Add(guid, new GUI(ui, userInterface));
			}
			else PortableStorage.Instance.BagUI.Remove(guid);

			Main.PlaySound(SoundID.Item59);
		}

		public override bool UseItem(Player player)
		{
			HandleUI();

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;
			active = !active;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine tooltip = tooltips.Find(x => x.mod == "Terraria" && x.Name == "Tooltip0");
			tooltip.text = $"Use the bag or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it";
		}

		public float posY;
		public bool up;
		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			posY += up ? -0.08f : 0.08f;
			if (posY <= -2) up = false;
			else if (posY >= 2) up = true;
			spriteBatch.Draw(active ? PortableStorage.Instance.vacuumBagOn : PortableStorage.Instance.vacuumBagOff, position + new Vector2(0, posY), frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

			return false;
		}

		public override TagCompound Save() => new TagCompound { ["Items"] = Items.Save(), ["GUID"] = guid.ToString(), ["Active"] = active };

		public override void Load(TagCompound tag)
		{
			Items = TheOneLibrary.Utility.Utility.Load(tag);
			guid = tag.ContainsKey("GUID") && !string.IsNullOrEmpty((string)tag["GUID"]) ? Guid.Parse(tag.GetString("GUID")) : Guid.NewGuid();
			active = tag.GetBool("Active");
		}

		public override void NetSend(BinaryWriter writer) => writer.Write(Items);

		public override void NetRecieve(BinaryReader reader) => Items = TheOneLibrary.Utility.Utility.Read(reader);

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void OnCraft(Recipe recipe)
		{
			Items.Clear();
			for (int i = 0; i < 27; i++) Items.Add(new Item());
		}

		public IList<Item> GetItems() => Items;
	}
}