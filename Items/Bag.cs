using System;
using System.Collections.Generic;
using BaseLibrary.Items;
using ContainerLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items;

public class Bag : BaseItem
{
	protected override bool CloneNewInstances => false;

	protected internal ItemStorage Storage;
	protected internal Guid ID;

	public static List<(Guid id, string stacktract)> Bags = [];

	public override ModItem NewInstance(Item entity)
	{
		Bag bag = base.NewInstance(entity) as Bag;
		bag.ID = Guid.NewGuid();
		bag.Storage = new ItemStorage(9);
		Bags.Add((bag.ID, Environment.StackTrace));

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

	// public override void UpdateInventory(Player player)
	// {
	// 	base.UpdateInventory(player);
	// 	
	// 	Main.NewText("kek");
	// }
}