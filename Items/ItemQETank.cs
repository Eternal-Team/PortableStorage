using BaseLibrary.Items;
using PortableStorage.Tiles;

namespace PortableStorage.Items
{
	public class ItemQETank : BaseItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quantum Entangled Tank");
			Tooltip.SetDefault("Stores 8B of fluid\nRight-click on the slots with gems to change frequency");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType<TileQETank>();
		}
	}
}