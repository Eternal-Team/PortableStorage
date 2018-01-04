using PortableStorage.Items;
using System.Linq;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage
{
	public class PSPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (PortableStorage.bagKey.JustPressed)
			{
				Item item = AccessoryItems.FirstOrDefault(x => x.modItem is BaseBag);

				if (item?.modItem is BaseBag)
				{
					BaseBag bag = (BaseBag)item.modItem;
					bag.HandleUI();
				}
			}
		}
	}
}