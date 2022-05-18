using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace PortableStorage;

public class PortableStorageConfig: ModConfig
{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("$Mods.PortableStorage.Config.AlchemistBag")]
		
		[Label("$Mods.PortableStorage.Config.AlchemistBagQuickBuff")] 
		[DefaultValue(true)] 
		public bool AlchemistBagQuickBuff;
		
		[Label("$Mods.PortableStorage.Config.AlchemistBagQuickHeal")] 
		[DefaultValue(true)] 
		public bool AlchemistBagQuickHeal;
		
		[Label("$Mods.PortableStorage.Config.AlchemistBagQuickMana")] 
		[DefaultValue(true)] 
		public bool AlchemistBagQuickMana;
}