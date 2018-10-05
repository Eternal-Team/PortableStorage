using System.Linq;
using BaseLibrary.Items;
using ContainerLibrary.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public abstract class BaseBag : BaseItem
	{
		public override bool CloneNewInstances => true;

		public ItemStackHandler handler;
		public int ID => item.stringColor;

		public virtual LegacySoundStyle OpenSound => SoundID.Item1;
		public virtual LegacySoundStyle CloseSound => SoundID.Item1;

		public override ModItem Clone()
		{
			BaseBag clone = (BaseBag)base.Clone();
			clone.handler = new ItemStackHandler(handler.stacks.Select(x => x.Clone()).ToList());
			return clone;
		}

		public override void SetDefaults()
		{
			item.stringColor = PortableStorage.Instance.BagID++;
		}

		public override bool UseItem(Player player)
		{
			if (player.whoAmI == Main.LocalPlayer.whoAmI)
			{
				PortableStorage.Instance.BagUI.UI.HandleBag(this);
			}

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			//if (player.whoAmI == Main.LocalPlayer.whoAmI) this.HandleUI();
		}
	}
}