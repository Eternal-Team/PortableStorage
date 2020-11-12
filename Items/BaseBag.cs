using System;
using BaseLibrary;
using Terraria;
using Terraria.ID;
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

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			if (PortableStorage.Instance.bagState.bag == this) PortableStorage.Instance.bagState.bag = null;
			else PortableStorage.Instance.bagState.Open(this);
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