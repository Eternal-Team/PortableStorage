using ContainerLibrary;
using PortableStorage.Items.Special;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage
{
	public class PSTile : GlobalTile
	{
		public override void PlaceInWorld(int i, int j, Item item)
		{
			if (item.type == mod.ItemType<BuilderReserve>())
			{
				BuilderReserve builderReserve = (BuilderReserve)item.modItem;
				builderReserve.Handler.Shrink(builderReserve.selectedIndex, 1);
			}
		}
	}
}