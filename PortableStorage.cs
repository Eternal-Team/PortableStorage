using System.IO;
using Terraria.ModLoader;

namespace PortableStorage;

public class PortableStorage : Mod
{
	public const string AssetPath = "PortableStorage/Assets/";

	public override void Load()
	{
		Hooking.Hooking.Load();
		BagPopupText.Load();
	}

	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		BagSyncSystem.Instance.HandlePacket(reader, whoAmI);
	}
}

public class PortableStorageSystem : ModSystem
{
	public override void PostSetupContent()
	{
		Utility.PostSetupContent();
	}

	public override void AddRecipeGroups()
	{
		Utility.AddRecipeGroups();
	}
}