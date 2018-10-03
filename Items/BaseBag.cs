using System.Collections.Generic;
using System.Linq;
using BaseLibrary.Items;
using BaseLibrary.Utility;
using ContainerLibrary.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public abstract class BaseBag : BaseItem
	{
		public override bool CloneNewInstances => true;

		public ItemStackHandler handler;

		public static LegacySoundStyle OpenSound { get; set; } = SoundID.Item1;
		public static LegacySoundStyle CloseSound { get; set; } = SoundID.Item1;

		public override ModItem Clone()
		{
			BaseBag clone = (BaseBag)base.Clone();
			clone.handler = new ItemStackHandler(handler.stacks.Select(x => x.Clone()).ToList());
			return clone;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "nane", handler.stacks.Where((x, i) => !x.IsAir).Select((x, i) => $"[{i}: {x.ToString()}]").Aggregate("\n")));
		}

		public override bool UseItem(Player player)
		{
			//if (player.whoAmI == Main.LocalPlayer.whoAmI) this.HandleUI();

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			for (int count = 0; count < 5; count++)
			{
				Item i = new Item();
				i.SetDefaults(Main.rand.Next(0, ItemLoader.ItemCount));
				handler.InsertItem(Main.rand.Next(handler.GetSlots()), i);
			}

			//if (player.whoAmI == Main.LocalPlayer.whoAmI) this.HandleUI();
		}
	}
}