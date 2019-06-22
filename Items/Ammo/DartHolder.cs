using Terraria;

namespace PortableStorage.Items.Ammo
{
	public class DartHolder : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/DartHolder";

		public override string AmmoType => "Dart";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dart Holder");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of darts");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 28;
			item.value = Item.buyPrice(gold: 15);
		}
	}
}