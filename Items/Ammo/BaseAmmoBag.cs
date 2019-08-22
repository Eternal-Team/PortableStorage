using ContainerLibrary;
using System.Linq;
using Terraria;

namespace PortableStorage.Items.Ammo
{
	public abstract class BaseAmmoBag : BaseBag
	{
		public abstract string AmmoType { get; }

		public BaseAmmoBag()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => Utility.Ammos[AmmoType].Values.SelectMany(x => x).Contains(item.type);
		}
	}
}