using System;
using System.Collections.Generic;
using BaseLibrary;
using BaseLibrary.UI;
using ContainerLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items;

public abstract class BaseBag : BaseItem, IItemStorage, IHasUI
{
	public SoundStyle? OpenSound => new SoundStyle("PortableStorage/Assets/Sounds/BagOpen");
	public SoundStyle? CloseSound => new SoundStyle("PortableStorage/Assets/Sounds/BagClose");

	public Guid ID;
	protected ItemStorage storage;

	protected ItemStorage Storage
	{
		set => storage = value;
		get
		{
			if (storage == null) OnCreate(null);
			return storage;
		}
	}

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

	public override void SetStaticDefaults()
	{
		SacrificeTotal = 1;
	}

	public override void SetDefaults()
	{
		Item.useTime = 5;
		Item.useAnimation = 5;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.rare = ItemRarityID.White;
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

	// bug: also plays the default right click sound (grab)
	public override void RightClick(Player player)
	{
		PanelUI.Instance.HandleUI(this);
	}

	public override void SaveData(TagCompound tag)
	{
		tag.Set("ID", ID);
		tag.Set("Items", Storage.Save());
	}

	public override void LoadData(TagCompound tag)
	{
		ID = tag.Get<Guid>("ID");
		Storage.Load(tag.Get<TagCompound>("Items"));
	}

	public ItemStorage GetItemStorage() => Storage;

	public Guid GetID() => ID;
}