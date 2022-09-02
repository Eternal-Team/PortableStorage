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
		// OnContentsChanged += (_, _, _) => ModContent.GetInstance<BagSyncSystem>().Register(this.bag);
	}

	public override void OnContentsChanged(object user, Operation operation, int slot)
	{
		ModContent.GetInstance<BagSyncSystem>().Register(bag);
	}
}

public enum PickupMode
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

	protected Guid id;

	public Guid ID
	{
		get
		{
			if (id == Guid.Empty)
				OnCreate(null);
			return id;
		}
	}

	protected PickupMode pickupMode;

	public PickupMode PickupMode
	{
		get => pickupMode;
		set
		{
			pickupMode = value;

			ModContent.GetInstance<BagSyncSystem>().Register(this);
		}
	}

	protected ItemStorage Storage;

	public override void OnCreate(ItemCreationContext context)
	{
		if (id != Guid.Empty) return;
		if (Storage is not null) return;

		id = Guid.NewGuid();
		pickupMode = PickupMode.Disabled;

		ModContent.GetInstance<BagSyncSystem>().AllBags.Add(ID, this);
	}

	public override ModItem Clone(Item item)
	{
		BaseBag clone = (BaseBag)base.Clone(item);
		clone.Storage = Storage/*.Clone()*/;
		clone.id = id;
		clone.pickupMode = pickupMode;
		if (ModContent.GetInstance<BagSyncSystem>().AllBags.ContainsKey(id)) ModContent.GetInstance<BagSyncSystem>().AllBags.Remove(id);
		ModContent.GetInstance<BagSyncSystem>().AllBags.Add(id, clone);
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
		tooltips.Add(new TooltipLine(Mod, "debugid", ID.ToString()));
		tooltips.Add(new TooltipLine(Mod, "debugid-1", (ModContent.GetInstance<BagSyncSystem>().AllBags[ID].GetItemStorage() == GetItemStorage()).ToString()));
	}

	public override bool ConsumeItem(Player player) => false;

	public override bool? UseItem(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
			// PanelUI.Instance.HandleUI(ModContent.GetInstance<BagSyncSystem>().AllBags[ID]);
			PanelUI.Instance.HandleUI(this);

		return true;
	}

	public override bool CanRightClick() => true;

	public override void RightClick(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
			// PanelUI.Instance.HandleUI(ModContent.GetInstance<BagSyncSystem>().AllBags[ID]);
			PanelUI.Instance.HandleUI(this);
	}

	public override void SaveData(TagCompound tag)
	{
		tag.Set("ID", ID);
		tag.Set("Items", Storage.Save());
		tag.Set("PickupMode", (byte)PickupMode);
	}

	public override void LoadData(TagCompound tag)
	{
		var bags = ModContent.GetInstance<BagSyncSystem>().AllBags;

		var newID = tag.Get<Guid>("ID");
		Storage.Load(tag.Get<TagCompound>("Items"));
		pickupMode = (PickupMode)tag.GetByte("PickupMode");

		if (newID != ID)
		{
			if (bags.ContainsKey(ID)) bags.Remove(ID);
			if (bags.ContainsKey(newID)) bags.Remove(newID);

			bags.Add(newID, this);
			id = newID;
		}
		// else
		// {
		// 	if (bags.ContainsKey(newID)) bags.Remove(newID);
		// 	bags.Add(newID, this);
		// 	id = newID;
		// }
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(ID);
		Storage.Write(writer);
		writer.Write((byte)PickupMode);
	}

	public override void NetReceive(BinaryReader reader)
	{
		var bags = ModContent.GetInstance<BagSyncSystem>().AllBags;

		var newID = reader.ReadGuid();
		Storage.Read(reader);
		pickupMode = (PickupMode)reader.ReadByte();

		if (newID != ID)
		{
			if (bags.ContainsKey(ID)) bags.Remove(ID);
			if (bags.ContainsKey(newID)) bags.Remove(newID);

			bags.Add(newID, this);
			id = newID;
		}
		// else
		// {
		// 	if (bags.ContainsKey(newID)) bags.Remove(newID);
		//
		// 	bags.Add(newID, this);
		// 	id = newID;
		// }
	}

	public ItemStorage GetItemStorage() => Storage;

	public Guid GetID() => ID;
}