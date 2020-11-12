using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		public const string AssetPath = "PortableStorage/Assets/";

		public static PortableStorage Instance => ModContent.GetInstance<PortableStorage>();

		public NormalBagUI bagState;
		private UserInterface bagUI;

		public override void Load()
		{
			Hooking.Hooking.Load();
			
			if (!Main.dedServ)
			{
				bagState = new NormalBagUI();
				bagUI = new UserInterface();
				bagUI.SetState(bagState);
			}
		}

		public override void PostSetupContent()
		{
			Utility.PostSetupContent();
		}

		public override void AddRecipeGroups()
		{
			Utility.AddRecipeGroups();
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (bagState.Visible) bagUI.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseTextIndex != -1)
			{
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
					"PortableStorage: BagUI",
					delegate
					{
						if (bagState.Visible) bagUI.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}