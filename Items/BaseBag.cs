using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	protected BaseBag bag;

	public BagStorage(BaseBag bag, int size) : base(size)
	{
		this.bag = bag;
		OnContentsChanged += (_, _, slot) =>
		{
			BaseBag.SyncBag(bag);
			// ModContent.GetInstance<BagChangeSystem>().RegisterItem(bag, slot);
		};
	}
}

public abstract class BaseBag : BaseItem, IItemStorage, IHasUI
{
	internal static void SyncBag(BaseBag item)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			Player player = Main.LocalPlayer;

			int slot = player.inventory.ToList().FindIndex(x => (x.ModItem as BaseBag)?.ID == item.ID);
			if (slot < 0) return;

			NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot);
		}
	}

	public SoundStyle? GetOpenSound() => new SoundStyle("PortableStorage/Assets/Sounds/BagOpen");
	public SoundStyle? GetCloseSound() => new SoundStyle("PortableStorage/Assets/Sounds/BagClose");

	protected override bool CloneNewInstances => false;

	public Guid ID;
	protected ItemStorage storage;
	public bool EnablePickup;

	protected ItemStorage Storage
	{
		set => storage = value;
		get
		{
			// if (storage == null) OnCreate(null);
			return storage;
		}
	}

	public BaseBag()
	{
		ID = Guid.NewGuid();
		EnablePickup = true;

		// if (!BagChangeSystem.Bags.ContainsKey(ID)) BagChangeSystem.Bags.Add(ID, this);
		// BagChangeSystem.Bags.Add(ID, this);

		// if (Main.netMode != NetmodeID.Server) Main.NewText($"Created bag with ID: {ID}");
	}

	public override ModItem Clone(Item item)
	{
		BaseBag clone = (BaseBag)base.Clone(item);
		clone.Storage = Storage.Clone();
		clone.ID = ID;
		clone.EnablePickup = EnablePickup;
		return clone;
	}

	// public override void OnCreate(ItemCreationContext context)
	// {
	// 	ID = Guid.NewGuid();
	//
	// 	if(Main.netMode!= NetmodeID.Server)Main.NewText($"Created bag with ID: {ID}");
	//
	// 	EnablePickup = true;
	// }

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
		if (player.whoAmI == Main.LocalPlayer.whoAmI)
			PanelUI.Instance.HandleUI(this);

		return true;
	}

	public override bool CanRightClick() => true;

	// bug: also plays the default right click sound (grab)
	public override void RightClick(Player player)
	{
		if (player.whoAmI == Main.LocalPlayer.whoAmI)
			PanelUI.Instance.HandleUI(this);
	}

	public override void SaveData(TagCompound tag)
	{
		tag.Set("ID", ID);
		tag.Set("Items", Storage.Save());
		tag.Set("EnablePickup", EnablePickup);
	}

	public override void LoadData(TagCompound tag)
	{
		// if (BagChangeSystem.Bags.ContainsKey(ID)) BagChangeSystem.Bags.Remove(ID);

		ID = tag.Get<Guid>("ID");
		Storage.Load(tag.Get<TagCompound>("Items"));
		EnablePickup = tag.GetBool("EnablePickup");

		// if (!BagChangeSystem.Bags.ContainsKey(ID)) BagChangeSystem.Bags.Add(ID, this);
		// if (BagChangeSystem.Bags.ContainsKey(ID)) BagChangeSystem.Bags.Remove(ID);
		// BagChangeSystem.Bags.Add(ID, this);
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(ID);

		Storage.Write(writer);
		writer.Write(EnablePickup);
	}

	public override void NetReceive(BinaryReader reader)
	{
		// if (BagChangeSystem.Bags.ContainsKey(ID)) BagChangeSystem.Bags.Remove(ID);

		ID = reader.ReadGuid();
		Storage.Read(reader);
		EnablePickup = reader.ReadBoolean();
		
		// if (!BagChangeSystem.Bags.ContainsKey(ID)) BagChangeSystem.Bags.Add(ID, this);
		// if (Main.netMode != NetmodeID.Server) Main.NewText($"Received bag with ID: {ID}");
		// BagChangeSystem.Bags.Add(ID, this);
	}

	public ItemStorage GetItemStorage() => Storage;

	public Guid GetID() => ID;
}