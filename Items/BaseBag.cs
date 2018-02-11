using TheOneLibrary.Base.Items;

namespace PortableStorage.Items
{
    public abstract class BaseBag : BaseItem
    {
        public override bool CloneNewInstances => true;

        public virtual void HandleUI()
        {
        }
    }
}