using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace PortableStorage
{
	public class PortableStorageConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[DefaultValue(true), Label("$Mods.PortableStorage.Config.AlchemistBagQuickBuff")]
		public bool AlchemistBagQuickBuff;

		[DefaultValue(true), Label("$Mods.PortableStorage.Config.AlchemistBagQuickHeal")]
		public bool AlchemistBagQuickHeal;

		[DefaultValue(true), Label("$Mods.PortableStorage.Config.AlchemistBagQuickMana")]
		public bool AlchemistBagQuickMana;
	}
}