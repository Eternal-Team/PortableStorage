using ContainerLibrary;
using PortableStorage.Global;
using System.Linq;

namespace PortableStorage.Items.Ammo
{
	public abstract class BaseAmmoBag : BaseBag
	{
		public abstract string AmmoType { get; }

		public BaseAmmoBag()
		{
			Handler = new ItemHandler(27);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => Utility.Ammos[AmmoType].Values.SelectMany(x => x).Contains(item.type);
		}
	}
}