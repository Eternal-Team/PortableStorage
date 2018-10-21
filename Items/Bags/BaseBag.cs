using System;
using System.Linq;
using BaseLibrary.Items;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Bags
{
	public abstract class BaseBag : BaseItem, IItemHandler
	{
		public override bool CloneNewInstances => true;

		public ItemHandler Handler { get; set; }
		public int ID => item.stringColor;

		public BaseBagPanel UI => PortableStorage.Instance.PanelUI.UI.Elements.OfType<BaseBagPanel>().FirstOrDefault(x => x.bag.ID == ID);
		public Vector2? UIPosition;

		public virtual Type UIType { get; }
		public virtual LegacySoundStyle OpenSound => SoundID.Item1;
		public virtual LegacySoundStyle CloseSound => SoundID.Item1;

		public override ModItem Clone()
		{
			BaseBag clone = (BaseBag)base.Clone();
			clone.Handler = Handler.Clone();
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
			if (player.whoAmI == Main.LocalPlayer.whoAmI) PortableStorage.Instance.PanelUI.UI.HandleUI(this);

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			if (player.whoAmI == Main.LocalPlayer.whoAmI) PortableStorage.Instance.PanelUI.UI.HandleUI(this);
		}
	}
}