using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.Items.Bags;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items
{
	public class TheBlackHole : BaseBag
	{
		[PathOverride("PortableStorage/Textures/Items/TheBlackHole")]
		public new static Texture2D Texture { get; set; }

		public override Type UIType => typeof(TheBlackHolePanel);

		public bool active;
		private float angle;
		private const float angleDecrement = 0.02617994F;
		private Vector2 origin = new Vector2(30);
		private const int maxRange = 160;

		public TheBlackHole()
		{
			handler = new ItemHandler(27);
			handler.OnContentsChanged += slot =>
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					Player player = Main.player[item.owner];

					List<Item> joined = player.inventory.Concat(player.armor).Concat(player.dye).Concat(player.miscEquips).Concat(player.miscDyes).Concat(player.bank.item).Concat(player.bank2.item).Concat(new[] { player.trashItem }).Concat(player.bank3.item).ToList();
					int index = joined.FindIndex(x => x == item);
					if (index < 0) return;

					NetMessage.SendData(MessageID.SyncEquipment, number: item.owner, number2: index);
				}
			};
		}

		public override ModItem Clone()
		{
			TheBlackHole clone = (TheBlackHole)base.Clone();
			clone.active = active;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Black Hole");
			Tooltip.SetDefault($"Stores {handler.Slots} stacks of items\nCollects them in a {maxRange / 16} block radius");
			ItemID.Sets.ItemNoGravity[item.type] = true;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Purple;
			item.noUseGraphic = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => Update(player);

		public override void UpdateInventory(Player player) => Update(player);

		public void Update(Player player)
		{
			List<Item> nearbyItems = Main.item.Where(item => Vector2.Distance(item.Center, player.position) <= maxRange).ToList();
			foreach (Item nearbyItem in nearbyItems)
			{
				if (nearbyItem.scale > 0f) nearbyItem.scale -= 0.01f;
			}
		}

		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;
			scale *= 32f / 60f;
			float scaleMultiplier = (float)Math.Sin(angle).Remap(-1f, 1f, 0.4f, 1f);

			spriteBatch.Draw(Main.extraTexture[50], position + this.origin * scale, null, Color.White, angle, origin + this.origin, scale * scaleMultiplier, SpriteEffects.None, 0f);

			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;
			scale *= 32f / 60f;
			float scaleMultiplier = (float)Math.Sin(angle).Remap(-1f, 1f, 0.4f, 1f);

			spriteBatch.Draw(Main.extraTexture[50], item.position - Main.screenPosition + origin, null, lightColor, angle + rotation, origin, scale * scaleMultiplier, SpriteEffects.None, 0f);

			return false;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Items"] = handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			handler.Load(tag.GetCompound("Items"));
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}