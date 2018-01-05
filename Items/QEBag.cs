using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage.Items
{
	public class QEBag : BaseBag, IContainerItem
	{
		public Guid guid = Guid.NewGuid();
		public Frequency frequency;

		public override string Texture => PortableStorage.ItemTexturePath + "QEBag";

		public override ModItem Clone(Item item)
		{
			QEBag clone = (QEBag)base.Clone(item);
			clone.frequency = frequency;
			clone.guid = guid;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quantum Entangled Bag");
			Tooltip.SetDefault("Right-click on a Quantum Entangled Chest to link it");
			ItemID.Sets.ItemNoGravity[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 34;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.value = GetItemValue(mod.ItemType<Bag>()) + GetItemValue(ItemID.ShadowScale) * 25 + GetItemValue(ItemID.DemoniteBar) * 5;
			item.rare = 9;
			item.accessory = true;
		}

		public override void HandleUI()
		{
			if (!PortableStorage.Instance.BagUI.ContainsKey(guid))
			{
				QEBagUI ui = new QEBagUI();
				UserInterface userInterface = new UserInterface();
				ui.Activate();
				userInterface.SetState(ui);
				ui.visible = true;
				ui.Load(this);
				PortableStorage.Instance.BagUI.Add(guid, new GUI(ui, userInterface));
			}
			else PortableStorage.Instance.BagUI.Remove(guid);

			Main.PlaySound(SoundID.DD2_EtherianPortalOpen.WithVolume(0.5f));
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

			HandleUI();
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			spriteBatch.Draw(PortableStorage.gemsSide[0], position + new Vector2(2, 12) * scale, new Rectangle(6 * (int)frequency.colorLeft, 0, 6, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.gemsMiddle[0], position + new Vector2(12, 12) * scale, new Rectangle(8 * (int)frequency.colorMiddle, 0, 8, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.gemsSide[0], position + new Vector2(24, 12) * scale, new Rectangle(6 * (int)frequency.colorRight, 0, 6, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0f);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag, right-click it or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
		}

		public override TagCompound Save() => new TagCompound {["Frequency"] = frequency, ["GUID"] = guid.ToString()};

		public override void Load(TagCompound tag)
		{
			frequency = tag.Get<Frequency>("Frequency");
			guid = tag.ContainsKey("GUID") && !string.IsNullOrEmpty((string)tag["GUID"]) ? Guid.Parse(tag.GetString("GUID")) : Guid.NewGuid();
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType<Bag>());
			recipe.AddIngredient(ItemID.ShadowScale, 25);
			recipe.AddIngredient(ItemID.DemoniteBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod.ItemType<Bag>());
			recipe.AddIngredient(ItemID.TissueSample, 25);
			recipe.AddIngredient(ItemID.CrimtaneBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public IList<Item> GetItems() => mod.GetModWorld<PSWorld>().enderItems[frequency];
	}
}