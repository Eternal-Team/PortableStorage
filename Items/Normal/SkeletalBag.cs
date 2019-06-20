namespace PortableStorage.Items.Normal
{
	public class SkeletalBag : BaseNormalBag
	{
		public override string Texture => "PortableStorage/Textures/Items/SkeletalBag";

		public override int SlotCount => 27;
		public override string Name => "Skeletal";

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.headSlot = 32;
			item.rare = 2;
		}
	}
}