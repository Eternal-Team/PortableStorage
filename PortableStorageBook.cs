using BookLibrary;

namespace PortableStorage;

/// What needs to be explained
/// - Mouse-scroll to modify slot contents (Container Lib)
/// - Bags can be used for crafting
/// - Ammo Bags provide ammo
/// - Wallet provides money, also will consume it after selling
/// - Alchemist's Bag supports quick heal/mana/buff/food (on that note I might allow per-slot configuration for that)
/// - Alchemist's Bag works as bottle and Alchemy Table
/// - Item auto-pickup modes
/// - Fishing Belt supplies bait, also as best fishing pole
/// - How Wiring Bag, Gardener's Satchel and Builder's Reserve works

// TODO: (more of a Container Library thing) display if crafting will use items from storages

public class PortableStorageBook : ModBook
{
	public override void SetStaticDefaults()
	{
		BookCategory bookCategory = new BookCategory { Name = "Items" };
		bookCategory.Items.Add(new BookEntry {
			Name = "NormalBags",
			Mod = this
		});
		AddCategory(bookCategory);
	}
}