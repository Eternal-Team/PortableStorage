﻿using System.Collections.Generic;
using BaseLibrary;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Container;

namespace PortableStorage.Items.SpecialBags
{
	public class AlchemistBag : BaseBag, ICraftingStorage
	{
		private class AlchemistBagItemStorage : ItemStorage
		{
			private AlchemistBag bag;

			public AlchemistBagItemStorage(AlchemistBag bag) : base(PotionSlots + IngredientSlots)
			{
				this.bag = bag;
			}

			// public override void OnContentsChanged(int slot, bool user)
			// {
			// 	Recipe.FindRecipes();
			// 	Utility.SyncBag(bag);
			// }

			public override bool IsItemValid(int slot, Item item)
			{
				if (slot < PotionSlots)
				{
					return item.DamageType != DamageClass.Summon &&
					       (item.potion && item.healLife > 0 ||
					        item.healMana > 0 && !item.potion ||
					        item.buffType > 0 && item.buffType != BuffID.Rudolph) && !ItemID.Sets.NebulaPickup[item.type] && !IsPetItem(item);
				}

				return Utility.AlchemistBagWhitelist.Contains(item.type);
			}

			private static bool IsPetItem(Item item)
			{
				bool checkItem = item.type > ItemID.None && item.shoot > ProjectileID.None;
				bool checkBuff = item.buffType > 0 && item.buffType < Main.vanityPet.Length;
				if (checkItem)
				{
					checkBuff = Main.vanityPet[item.buffType] || Main.lightPet[item.buffType];
				}

				return checkItem && checkBuff;
			}
		}

		public override string Texture => PortableStorage.AssetPath + "Textures/Items/AlchemistBag";

		public const int PotionSlots = 18;
		private const int IngredientSlots = 63;

		public override void OnCreate(ItemCreationContext context)
		{
			Storage = new AlchemistBagItemStorage(this);
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

		public IEnumerable<int> GetSlotsForCrafting()
		{
			for (int i = PotionSlots; i < IngredientSlots; i++) yield return i;
		}
	}
}