using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSTile : GlobalTile
	{
		public override void PlaceInWorld(int i, int j, Item item)
		{
			if (item.type == mod.ItemType<DevNull>())
			{
				DevNull devNull = (DevNull)item.modItem;
				devNull.Items[devNull.selectedIndex].stack--;
			}
		}
	}
}