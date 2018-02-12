using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (PortableStorage.bagKey.JustPressed)
			{
				Item item = TheOneLibrary.Utility.Utility.Accessory.FirstOrDefault(x => x.modItem is BaseBag);

				if (item?.modItem is BaseBag)
				{
					BaseBag bag = (BaseBag)item.modItem;
					bag.HandleUI();
				}
			}
		}
	}
}