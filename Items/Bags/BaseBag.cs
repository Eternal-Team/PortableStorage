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
	public abstract class BaseBag<T> : BaseItem, IItemHandler where T : BaseBagPanel
	{
		public override bool CloneNewInstances => true;

		public ItemHandler Handler { get; set; }
		public int ID => item.stringColor;

		public T UI;
		public Vector2? UIPosition;

		public virtual LegacySoundStyle OpenSound => SoundID.Item1;
		public virtual LegacySoundStyle CloseSound => SoundID.Item1;

		public override ModItem Clone()
		{
			BaseBag<T> clone = (BaseBag<T>)base.Clone();
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