using BaseLibrary.Utility;
using Terraria.ModLoader;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		public static PortableStorage Instance;

		public override void Load()
		{
			Instance = this;
		}

		public override void Unload()
		{
			Utility.UnloadNullableTypes();
		}
	}
}