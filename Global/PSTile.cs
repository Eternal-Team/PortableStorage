using PortableStorage.Items.Bags;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSTile : GlobalTile
	{
		public override void PlaceInWorld(int i, int j, Item item)
		{
			if (item.type == mod.ItemType<BuilderReserve>())
			{
				BuilderReserve devNull = (BuilderReserve)item.modItem;
				Item usedItem = devNull.Handler.stacks[devNull.selectedIndex];
				usedItem.stack--;
				if (usedItem.stack <= 0)
				{
					usedItem.TurnToAir();
					devNull.SetIndex(-1);
				}
			}
		}
	}
}