using BaseLibrary.UI;
using PortableStorage.Items;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage;

public class PortableStorage : Mod
{
	internal static PortableStorage Instance = null!;

	public override void Load()
	{
		Instance = this;

		Hooking.Hooking.Load();

		if (!Main.dedServ)
		{
			UISystem.UILayer.Add(new BagUI());
		}
	}
}