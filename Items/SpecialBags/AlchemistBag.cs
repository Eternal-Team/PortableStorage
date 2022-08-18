using System.Collections.Generic;
using ContainerLibrary;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public class AlchemistBag : BaseBag, ICraftingStorage
{
	private class AlchemistBagItemStorage : BagStorage
	{
		public AlchemistBagItemStorage(BaseBag bag) : base(bag, PotionSlots + IngredientSlots)
		{
		}

		public override bool IsItemValid(int slot, Item item)
		{
			if (slot < PotionSlots)
			{
				return item.DamageType != DamageClass.Summon &&
				       ((item.potion && item.healLife > 0) ||
				        (item.healMana > 0 && !item.potion) ||
				        (item.buffType > 0 && item.buffType != BuffID.Rudolph)) && !ItemID.Sets.NebulaPickup[item.type] && !Utility.IsPetItem(item);
			}

			return Utility.AlchemistBagWhitelist.Contains(item.type);
		}
	}

	public override string Texture => PortableStorage.AssetPath + "Textures/Items/AlchemistBag";


	// note: separate into 2 ItemStorages?
	public const int PotionSlots = 18;
	public const int IngredientSlots = 63;

	public AlchemistBag()
	{
		Storage = new AlchemistBagItemStorage(this);
	}
	
	// public override void OnCreate(ItemCreationContext context)
	// {
	// 	base.OnCreate(context);
	//
	// 	Storage = new AlchemistBagItemStorage(this);
	// }

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

	public IEnumerable<int> GetSlotsForCrafting()
	{
		for (int i = PotionSlots; i < IngredientSlots; i++) yield return i;
	}
}