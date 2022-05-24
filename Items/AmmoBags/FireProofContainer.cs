using Terraria;
using Terraria.ID;

namespace PortableStorage.Items;

public class FireProofContainer : BaseAmmoBag
{
	public override string Texture => PortableStorage.AssetPath + "Textures/Items/FireProofContainer";

	protected override string AmmoType => "Flammable";

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 28;
		Item.height = 26;
		Item.value = Item.buyPrice(gold: 20);
		Item.rare = ItemRarityID.Lime;
	}
}