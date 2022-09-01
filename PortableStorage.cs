using System.Collections.Generic;
using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage;

public class BagSyncSystem : ModSystem
{
	private List<BaseBag> Bags = new();

	public void Register(BaseBag item)
	{
		if (!Bags.Contains(item))
			Bags.Add(item);
	}

	public override void PostUpdateItems()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
			return;

		foreach (BaseBag bag in Bags)
		{
			Player player = Main.LocalPlayer;

			int slot = player.inventory.ToList().FindIndex(x => (x.ModItem as BaseBag)?.ID == bag.ID);
			if (slot < 0) continue;

			NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, slot);
		}

		Bags.Clear();
	}
}

public class PortableStorage : Mod
{
	public const string AssetPath = "PortableStorage/Assets/";

	public static PortableStorage Instance => ModContent.GetInstance<PortableStorage>();

	public override void Load()
	{
		Hooking.Hooking.Load();
		BagPopupText.Load();
	}

	public override void PostSetupContent()
	{
		Utility.PostSetupContent();
	}
}

public class PortableStorageSystem : ModSystem
{
	public override void AddRecipeGroups()
	{
		Utility.AddRecipeGroups();
	}
}