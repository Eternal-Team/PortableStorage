using BaseLib.Items;

namespace PortableStorage.Items
{
	public class BaseBag : BaseItem
	{
		public override bool CloneNewInstances => true;

		public virtual void HandleUI() { }
	}
}