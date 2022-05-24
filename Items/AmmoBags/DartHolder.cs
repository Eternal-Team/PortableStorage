using Terraria;
using Terraria.ID;

namespace PortableStorage.Items;

public class DartHolder : BaseAmmoBag
{
	public override string Texture => PortableStorage.AssetPath + "Textures/Items/DartHolder";

	protected override string AmmoType => "Dart";

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 32;
		Item.height = 28;
		Item.value = Item.buyPrice(gold: 15);
		Item.rare = ItemRarityID.Orange;
	}
}