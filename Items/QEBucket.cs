using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Fluid;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage.Items
{
	public class QEBucket : BaseBag
	{
		public QEBucket()
		{
			Main.NewText("Constructor test");
		}

		public Frequency frequency;

		public override string Texture => PortableStorage.ItemTexturePath + "QEBucket";

		public override ModItem Clone(Item item)
		{
			QEBucket clone = (QEBucket)base.Clone(item);
			clone.frequency = frequency;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quantum Entangled Bucket");
			Tooltip.SetDefault("Right-click on a Quantum Entangled Tank to link it");
		}

		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 34;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.value = GetItemValue(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>()) + GetItemValue(ItemID.ShadowScale) * 25 + GetItemValue(ItemID.DemoniteBar) * 5;
			item.rare = 9;
			item.accessory = true;
		}

		public override bool UseItem(Player player)
		{
			return true;
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			spriteBatch.Draw(PortableStorage.ringBig, position + new Vector2(4, 16) * scale, new Rectangle(0, 4 * (int)frequency.colorLeft, 22, 4), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.ringBig, position + new Vector2(4, 20) * scale, new Rectangle(0, 4 * (int)frequency.colorMiddle, 22, 4), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.ringSmall, position + new Vector2(6, 24) * scale, new Rectangle(0, 4 * (int)frequency.colorRight, 18, 4), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			//tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag, right-click it or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
		}

		public override TagCompound Save() => new TagCompound {["Frequency"] = frequency};

		public override void Load(TagCompound tag)
		{
			frequency = tag.Get<Frequency>("Frequency");
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>());
			recipe.AddIngredient(ItemID.ShadowScale, 25);
			recipe.AddIngredient(ItemID.DemoniteBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>());
			recipe.AddIngredient(ItemID.TissueSample, 25);
			recipe.AddIngredient(ItemID.CrimtaneBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}