using System;
using System.Linq;
using BaseLibrary.Items;
using ContainerLibrary;
using PortableStorage.UI;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public abstract class BaseBag : BaseItem
	{
		public override bool CloneNewInstances => true;

		public ItemHandler handler;
		public int ID => item.stringColor;

		public BaseBagPanel UI => PortableStorage.Instance.BagUI.UI.Elements.OfType<BaseBagPanel>().FirstOrDefault(x => x.bag.ID == ID);

		public virtual Type UIType { get; }
		public virtual LegacySoundStyle OpenSound => SoundID.Item1;
		public virtual LegacySoundStyle CloseSound => SoundID.Item1;

		public override ModItem Clone()
		{
			BaseBag clone = (BaseBag)base.Clone();
			clone.handler = new ItemHandler(handler.stacks.Select(x => x.Clone()).ToList());
			clone.handler.IsItemValid = (Func<int, Item, bool>)handler.IsItemValid.Clone();
			clone.handler.GetSlotLimit = (Func<int, int>)handler.GetSlotLimit.Clone();
			clone.handler.OnContentsChanged = (Action<int>)handler.OnContentsChanged.Clone();
			return clone;
		}

		public override void SetDefaults()
		{
			item.stringColor = PortableStorage.BagID++;
			item.useTime = 5;
			item.useAnimation = 5;
			item.noUseGraphic = true;
			item.useStyle = 1;
			item.rare = 0;
		}

		public override bool UseItem(Player player)
		{
			if (player.whoAmI == Main.LocalPlayer.whoAmI) PortableStorage.Instance.BagUI.UI.HandleBag(this);

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			if (player.whoAmI == Main.LocalPlayer.whoAmI) PortableStorage.Instance.BagUI.UI.HandleBag(this);
		}
	}
}