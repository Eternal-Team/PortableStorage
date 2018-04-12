using System.Linq;
using Terraria;
using Terraria.ID;
using TheOneLibrary.Base.Items;
using TheOneLibrary.Base.UI;

namespace PortableStorage.Items
{
	public abstract class BaseBag : BaseItem
	{
		public override bool CloneNewInstances => true;

		public GUI gui;

		public virtual void HandleUI()
		{
			if (!PortableStorage.Instance.BagUI.ContainsValue(gui)) PortableStorage.Instance.BagUI.Add(item.modItem, gui);
			else PortableStorage.Instance.BagUI.Remove(PortableStorage.Instance.BagUI.First(kvp => kvp.Value == gui).Key);

			Main.PlaySound(SoundID.Item59);
		}
	}
}