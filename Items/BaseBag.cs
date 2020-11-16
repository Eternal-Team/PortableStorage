﻿using System;
using System.Collections.Generic;
using BaseLibrary;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items
{
	public abstract class BaseBag : BaseItem, IItemHandler
	{
		public Guid ID;
		public ItemHandler Handler;

		public override ModItem Clone(Item item)
		{
			BaseBag clone = (BaseBag)base.Clone(item);
			clone.Handler = Handler.Clone();
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
			tooltips.Add(new TooltipLine(Mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(Handler.Slots)));
		}

		public override bool ConsumeItem(Player player) => false;

		public override bool UseItem(Player player)
		{
			if (BagUISystem.Instance.bagState.bag == this) BagUISystem.Instance.bagState.bag = null;
			else BagUISystem.Instance.bagState.Open(this);

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			if (BagUISystem.Instance.bagState.bag == this) BagUISystem.Instance.bagState.bag = null;
			else BagUISystem.Instance.bagState.Open(this);
		}

		public override TagCompound Save() => new TagCompound
		{
			["ID"] = ID,
			["Items"] = Handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			ID = tag.Get<Guid>("ID");
			Handler.Load(tag.GetCompound("Items"));
		}

		public ItemHandler GetItemHandler() => Handler;
	}
}