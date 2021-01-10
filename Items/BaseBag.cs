using System;
using System.Collections.Generic;
using BaseLibrary;
using BaseLibrary.UI;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items
{
	public abstract class BaseBag : BaseItem, IItemStorage, IHasUI
	{
		public Guid ID;
		public ItemStorage Storage;

		public override ModItem Clone(Item item)
		{
			BaseBag clone = (BaseBag)base.Clone(item);
			clone.Storage = Storage.Clone();
			clone.ID = ID;
			return clone;
		}

		public override void OnCreate(ItemCreationContext context)
		{
			ID = Guid.NewGuid();
		}

		public override void SetDefaults()
		{
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = ItemUseStyleID.Swing;
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(Mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(Storage.Count)));
		}

		public override bool ConsumeItem(Player player) => false;

		public override bool? UseItem(Player player)
		{
			PanelUI.Instance.HandleUI(this);

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			PanelUI.Instance.HandleUI(this);
		}

		public override TagCompound Save() => new TagCompound
		{
			["ID"] = ID,
			["Items"] = Storage.Save()
		};

		public override void Load(TagCompound tag)
		{
			ID = tag.Get<Guid>("ID");
			Storage.Load(tag.GetCompound("Items"));
		}

		public ItemStorage GetItemStorage() => Storage;

		public Guid GetID() => ID;

		public LegacySoundStyle CloseSound { get; }
		public LegacySoundStyle OpenSound { get; }
	}
}