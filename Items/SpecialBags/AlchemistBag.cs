using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary.Utility;
using ContainerLibrary;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items;

public class AlchemistBag : BaseBag, ICraftingStorage
{
	private class AlchemistBagPotionStorage : BagStorage
	{
		public AlchemistBagPotionStorage(BaseBag bag) : base(bag, PotionSlots)
		{
		}

		public override bool IsItemValid(int slot, Item item)
		{
			return item.DamageType != DamageClass.Summon &&
			       ((item.potion && item.healLife > 0) ||
			        (item.healMana > 0 && !item.potion) ||
			        (item.buffType > 0 && item.buffType != BuffID.Rudolph)) && !ItemID.Sets.NebulaPickup[item.type] && !Utility.IsPetItem(item);
		}
	}

	private class AlchemistBagIngredientStorage : BagStorage
	{
		public AlchemistBagIngredientStorage(BaseBag bag) : base(bag, IngredientSlots)
		{
		}

		public override bool IsItemValid(int slot, Item item)
		{
			return Utility.AlchemistBagWhitelist.Contains(item.type);
		}
	}

	public override string Texture => PortableStorage.AssetPath + "Textures/Items/AlchemistBag";

	public const int PotionSlots = 18;
	public const int IngredientSlots = 63;

	public ItemStorage IngredientStorage;

	public override void OnCreate(ItemCreationContext context)
	{
		base.OnCreate(context);

		Storage = new AlchemistBagPotionStorage(this);
		IngredientStorage = new AlchemistBagIngredientStorage(this);
	}

	public override ModItem Clone(Item item)
	{
		AlchemistBag clone = (AlchemistBag)base.Clone(item);
		clone.IngredientStorage = IngredientStorage/*.Clone()*/;
		return clone;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);

		tag.Set("Ingredient", IngredientStorage.Save());
	}

	public override void LoadData(TagCompound tag)
	{
		var bags = BagSyncSystem.Instance.AllBags;

		var newID = tag.Get<Guid>("ID");
		PickupMode = (PickupMode)tag.GetByte("PickupMode");

		var items = tag.GetCompound("Items").GetList<Item>("Value");
		if (items.Count == PotionSlots + IngredientSlots)
		{
			Storage.SetValue("Items", items.Take(PotionSlots).ToArray());
			IngredientStorage.SetValue("Items", items.Skip(PotionSlots).ToArray());
		}
		else
		{
			Storage.Load(tag.Get<TagCompound>("Items"));
			IngredientStorage.Load(tag.Get<TagCompound>("Ingredient"));
		}

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
		base.NetSend(writer);

		IngredientStorage.Write(writer);
	}

	public override void NetReceive(BinaryReader reader)
	{
		base.NetReceive(reader);

		IngredientStorage.Read(reader);
	}

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 3);
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(PotionSlots, IngredientSlots)));
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Leather, 10)
			.AddIngredient(ItemID.FossilOre, 15)
			.AddIngredient(ItemID.AlchemyTable)
			.AddTile(TileID.Anvils)
			.Register();
	}

	public ItemStorage GetCraftingStorage() => IngredientStorage;
}