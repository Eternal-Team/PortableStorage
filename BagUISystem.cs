using BaseLibrary.UI;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage
{
	public class BagUISystem : ModSystem
	{
		public static BagUISystem Instance => ModContent.GetInstance<BagUISystem>();

		public NormalBagUI bagState;

		public override void Load()
		{
			if (!Main.dedServ)
			{
				bagState = new NormalBagUI();
				UILayer.Instance.Add(bagState);
			}
		}
	}
}