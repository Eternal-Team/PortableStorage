using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace PortableStorage
{
	public class BagUISystem : ModSystem
	{
		public static BagUISystem Instance => ModContent.GetInstance<BagUISystem>();

		public NormalBagUI bagState;
		private UserInterface bagUI;

		public override void Load()
		{
			if (!Main.dedServ)
			{
				bagState = new NormalBagUI();
				bagUI = new UserInterface();
				bagUI.SetState(bagState);
			}
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

		public override void UpdateUI(GameTime gameTime)
		{
			if (bagState.Visible) bagUI.Update(gameTime);
		}
	}
}