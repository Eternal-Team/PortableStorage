using Terraria.ModLoader;

namespace PortableStorage;

// note: energy addons

public class PortableStorage : Mod
{
	public const string AssetPath = "PortableStorage/Assets/";

	public static PortableStorage Instance => ModContent.GetInstance<PortableStorage>();

	public override void Load()
	{
		Hooking.Hooking.Load();
		BagPopupText.Load();
	}

	public override void PostSetupContent()
	{
		Utility.PostSetupContent();
	}

	public override void AddRecipeGroups()
	{
		Utility.AddRecipeGroups();
	}
}