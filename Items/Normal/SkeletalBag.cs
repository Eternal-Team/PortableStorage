namespace PortableStorage.Items.Normal
{
	public class SkeletalBag : BaseNormalBag
	{
		//public override string Texture => "PortableStorage/Textures/Items/AdventurerBag";
		public override int SlotCount => 27;
		public override string Name => "Skeletal";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.rare = 2;
		}
	}
}