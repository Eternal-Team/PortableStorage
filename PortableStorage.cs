using Terraria.ModLoader;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		public const string AssetPath = "PortableStorage/Assets/";

		public static PortableStorage Instance => ModContent.GetInstance<PortableStorage>();

		public override void Load()
		{
			Hooking.Hooking.Load();
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
}