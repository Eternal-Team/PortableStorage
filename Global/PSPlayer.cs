using System;
using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.UI.Bags;
using PortableStorage.UI.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace PortableStorage.Global
{
	public class PSPlayer : ModPlayer
	{
		public Dictionary<Guid, Vector2> UIPositions = new Dictionary<Guid, Vector2>();

		public override bool ShiftClickSlot(Item[] inventory, int context, int slot)
		{
			Item item = inventory[slot];

			if (context != ItemSlot.Context.InventoryItem && context != ItemSlot.Context.InventoryCoin && context != ItemSlot.Context.InventoryAmmo) return false;

			if (!PortableStorage.Instance.PanelUI.UI.Elements.Any(panel => ((BaseBagPanel)panel).bag.Handler.stacks.HasSpace(item))) return false;

			foreach (UIElement panel in PortableStorage.Instance.PanelUI.UI.Elements)
			{
				if (item.favorited || item.IsAir) return false;

				ItemHandler container = panel is BaseBagPanel ? ((BaseBagPanel)panel).bag.Handler : ((BaseTEPanel)panel).tileEntity.Handler;

				for (int i = 0; i < container.Slots; i++)
				{
					inventory[slot] = container.InsertItem(i, item);
					if (inventory[slot].IsAir) break;
				}
			}

			Main.PlaySound(SoundID.Grab);

			return true;
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				["UIPositions"] = UIPositions.Select(position => new TagCompound
				{
					["ID"] = position.Key.ToString(),
					["Position"] = position.Value
				}).ToList()
			};
		}

		public override void Load(TagCompound tag)
		{
			UIPositions = tag.GetList<TagCompound>("UIPositions").ToDictionary(c => Guid.Parse(c.Get<string>("ID")), c => c.Get<Vector2>("Position"));
		}
	}
}