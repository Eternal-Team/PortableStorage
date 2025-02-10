using PortableStorage.IL;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage;

// TODO: What is to be done? (possibly not written by Lenin and possibly ordered by priority)
// -- Existing --
//		(ContainerLibrary) Source items for crafting from storages (visual aid for which inventories are used)

// Normal Bags
// Add Bags to town NPC spawn checks
// Deposit/Loot features
// Bags providing ammo, coins (along with buy/sell behavior), fishing bait/pole
// Alchemist's Bag crafting, quick mana/heal/buff
// Entire behavior for Builder's Reserve, Gardener's Satchel, Wiring Bag
// Picking up items into bags
// All above features could be configurable
// BagPopupText
// Networking

// -- New --
// Sorting feature
// Textures for UI
// Bag slot(s)

// -- Mentioned in PS3, but not fitting the mod --
// Upgrade slots (one use case I could see is some sort of suction attachment that would pull far away items into the bag)
// Creative bag (what would it do)

public class PortableStorage : Mod
{
	internal static PortableStorage Instance = null!;

	internal const string Textures = "PortableStorage/Assets/Textures/";

	public override void Load()
	{
		Instance = this;

		Hooking.Load();

		if (!Main.dedServ)
		{
		}
	}
}