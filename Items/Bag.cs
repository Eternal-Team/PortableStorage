using System;
using BaseLibrary.Items;
using BaseLibrary.UI;
using ContainerLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items;

public class Bag : BaseItem, IHasUI
{
	protected override bool CloneNewInstances => false;

	protected internal ItemStorage Storage;
	protected internal Guid ID;

	public override ModItem NewInstance(Item entity)
	{
		Bag bag = base.NewInstance(entity) as Bag;
		bag.ID = Guid.NewGuid();
		bag.Storage = new ItemStorage(9).SetStackOverride(slot => {
			return slot switch {
				8 => 20000,
				7 => 50,
				_ => null
			};
		});

		return bag;
	}

	public override void SetDefaults()
	{
		Item.useTime = 5;
		Item.useAnimation = 5;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.rare = ItemRarityID.White;
		Item.width = 16;
		Item.height = 16;
	}

	public override bool ConsumeItem(Player player) => false;

	public override bool? UseItem(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
		{
			WindowUI.Instance?.HandleUI(this);
			Hooking.Hooking.SetLock(Item);
		}

		return true;
	}

	public override bool CanRightClick() => true;

	public override void RightClick(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
		{
			WindowUI.Instance?.HandleUI(this);
			Hooking.Hooking.SetLock(Item);
		}
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

	public Guid GetID() => ID;

	public SoundStyle? GetOpenSound() => new SoundStyle("PortableStorage/Assets/Sound/BagOpen");

	public SoundStyle? GetCloseSound() => new SoundStyle("PortableStorage/Assets/Sound/BagClose");
}