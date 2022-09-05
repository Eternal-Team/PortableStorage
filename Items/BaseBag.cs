using System;
using System.Collections.Generic;
using System.IO;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using ContainerLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items;

public class BagStorage : ItemStorage
{
	protected readonly BaseBag bag;

	protected BagStorage(BaseBag bag, int size) : base(size)
	{
		this.bag = bag;
	}

	protected override void OnContentsChanged(object user, Operation operation, int slot)
	{
		BagSyncSystem.Instance.Sync(bag.GetID(), PacketID.Inventory);
	}
}

public enum PickupMode : byte
{
	Disabled,
	BeforeInventory,
	AfterInventory,
	ExistingOnly
}

public abstract class BaseBag : BaseItem, IItemStorage, IHasUI
{
	public SoundStyle? GetOpenSound() => new SoundStyle("PortableStorage/Assets/Sounds/BagOpen");
	public SoundStyle? GetCloseSound() => new SoundStyle("PortableStorage/Assets/Sounds/BagClose");

	protected override bool CloneNewInstances => false;

	protected Guid ID;
	public PickupMode PickupMode;
	protected ItemStorage Storage;

	public override void OnCreate(ItemCreationContext context)
	{
		ID = Guid.NewGuid();
		PickupMode = PickupMode.Disabled;

		BagSyncSystem.Instance.AllBags.Add(ID, this);
	}

	public override ModItem Clone(Item item)
	{
		BaseBag clone = (BaseBag)base.Clone(item);
		clone.Storage = Storage /*.Clone()*/; // note: this is a stupid behavior, try to fix elsehow
		clone.ID = ID;
		clone.PickupMode = PickupMode;
		if (BagSyncSystem.Instance.AllBags.ContainsKey(ID)) BagSyncSystem.Instance.AllBags.Remove(ID);
		BagSyncSystem.Instance.AllBags.Add(ID, clone);
		return clone;
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

		OnCreate(null);
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(Storage.Count)));
	}

	public override bool ConsumeItem(Player player) => false;

	public override bool? UseItem(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
			PanelUI.Instance?.HandleUI(this);

		return true;
	}

	public override bool CanRightClick() => true;

	public override void RightClick(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
			PanelUI.Instance?.HandleUI(this);
	}

	public override void SaveData(TagCompound tag)
	{
		tag.Set("ID", ID);
		tag.Set("Items", Storage.Save());
		tag.Set("PickupMode", (byte)PickupMode);
	}

	public override void LoadData(TagCompound tag)
	{
		var bags = BagSyncSystem.Instance.AllBags;

		var newID = tag.Get<Guid>("ID");
		Storage.Load(tag.Get<TagCompound>("Items"));
		PickupMode = (PickupMode)tag.GetByte("PickupMode");

		if (newID != ID)
		{
			if (bags.ContainsKey(ID)) bags.Remove(ID);
			if (bags.ContainsKey(newID)) bags.Remove(newID);

			bags.Add(newID, this);
			ID = newID;
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(ID);
		Storage.Write(writer);
		writer.Write((byte)PickupMode);
	}

	public override void NetReceive(BinaryReader reader)
	{
		var bags = BagSyncSystem.Instance.AllBags;

		var newID = reader.ReadGuid();
		Storage.Read(reader);
		PickupMode = (PickupMode)reader.ReadByte();

		if (newID != ID)
		{
			if (bags.ContainsKey(ID)) bags.Remove(ID);
			if (bags.ContainsKey(newID)) bags.Remove(newID);

			bags.Add(newID, this);
			ID = newID;
		}
	}

	public ItemStorage GetItemStorage() => Storage;

	public Guid GetID() => ID;
}