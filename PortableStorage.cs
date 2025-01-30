using BaseLibrary.UI;
using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage;

public class PortableStorage : Mod
{
	public override void Load()
	{
		Hooking.Hooking.Load();
		
		if (!Main.dedServ)
		{
			UISystem.UILayer.Add(new BagUI());
		}
	}
}