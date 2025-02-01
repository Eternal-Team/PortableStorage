using System;
using System.Collections.Generic;
using BaseLibrary.Items;
using BaseLibrary.UI;
using ContainerLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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
		bag.Storage = new ItemStorage(9).SetStackOverride(slot => {
			return slot switch {
				8 => 20000,
				7 => 50,
				_ => null
			};
		});
		// Bags.Add((bag.ID, Environment.StackTrace));

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
			/*BagUI.Instance.Display = BagUI.Instance.Display == Display.Visible ? Display.None : Display.Visible;
			BagUI.Instance.SetBag(this);
			BagUI.Instance.Recalculate();*/

			if (BagUI.Instance.Display == Display.Visible)
			{
				BagUI.Instance.Display = Display.None;
				UISystem.UILayer.Remove(BagUI.Instance);
				BagUI.Instance.bag = null;

				Hooking.Hooking.SetLock(Item);
			}
			else
			{
				// UISystem.UILayer.Remove(BagUI.Instance);
				UISystem.UILayer.Add(new BagUI());
				BagUI.Instance.Display = Display.Visible;

				// BookUI.Instance = new BookUI();
				// BookUI.Instance.Display = BookUI.Instance.Display == Display.Visible ? Display.None : Display.Visible;
				BagUI.Instance.Recalculate();
				BagUI.Instance.SetBag(this);

				Hooking.Hooking.SetLock(Item);
			}
		}

		return true;
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
}