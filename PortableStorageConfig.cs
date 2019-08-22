using System.ComponentModel;
using Terraria;
using Terraria.ModLoader.Config;

namespace PortableStorage
{
	public class PortableStorageConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[DefaultValue(true)]
		public bool AlchemistBagQuickBuff;

		[DefaultValue(true)]
		public bool AlchemistBagQuickHeal;

		[DefaultValue(true)]
		public bool AlchemistBagQuickMana;
	}
}