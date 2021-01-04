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

			// if (!Main.dedServ)
			// {
			// 	bagState = new NormalBagUI();
			// 	bagUI = new UserInterface();
			// 	bagUI.SetState(bagState);
			// }
		}

		// public NormalBagUI bagState;
		// private UserInterface bagUI;

		// public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		// {
		// 	int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		// 	if (mouseTextIndex != -1)
		// 	{
		// 		layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
		// 			"PortableStorage: BagUI",
		// 			delegate
		// 			{
		// 				if (bagState.Visible) bagUI.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
		// 				return true;
		// 			},
		// 			InterfaceScaleType.UI)
		// 		);
		// 	}
		// }
		//
		// public override void UpdateUI(GameTime gameTime)
		// {
		// 	if (bagState.Visible) bagUI.Update(gameTime);
		// }

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