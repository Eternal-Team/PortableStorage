using BaseLibrary.Items;
using BaseLibrary.UI;
using ContainerLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items
{
	public abstract class BaseBag : BaseItem, IItemHandler, ICraftingStorage, IHasUI
	{
		public override bool CloneNewInstances => true;

		public ItemHandler Handler { get; set; }
		public ItemHandler CraftingHandler => Handler;

		public virtual LegacySoundStyle OpenSound => SoundID.Item1;
		public Guid ID { get; set; }
		public BaseUIPanel UI { get; set; }
		public virtual LegacySoundStyle CloseSound => SoundID.Item1;

		public override ModItem Clone()
		{
			BaseBag clone = (BaseBag)base.Clone();
			clone.Handler = Handler.Clone();
			clone.ID = ID;
			return clone;
		}

		public void OverhaulInit()
		{
			//this.SetTag(ItemTags.AllowQuickUse);
		}

		public override void SetDefaults()
		{
			ID = Guid.NewGuid();
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = 1;
			item.rare = 0;
		}

		public override bool UseItem(Player player)
		{
			if (player.whoAmI == Main.LocalPlayer.whoAmI) BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(this);

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			if (player.whoAmI == Main.LocalPlayer.whoAmI) BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(this);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "PortableStorage:BagInfo", "Right-click or use to open"));
		}

		public override TagCompound Save() => new TagCompound
		{
			["ID"] = ID.ToString(),
			["Items"] = Handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			if (!Guid.TryParse(tag.GetString("ID"), out Guid temp)) temp = Guid.NewGuid();
			ID = temp;
			Handler.Load(tag.GetCompound("Items"));
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(ID.ToString());
			Handler.Serialize(writer);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			ID = Guid.Parse(reader.ReadString());
			Handler.Deserialize(reader);
		}
	}
}