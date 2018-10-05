using System.Collections.Generic;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using PortableStorage.UI;
using Terraria.ModLoader;
using Terraria.UI;
using UIElement = On.Terraria.UI.UIElement;

namespace PortableStorage
{
	public class PortableStorage : Mod
	{
		public static PortableStorage Instance;
		public int BagID;

		public GUI<AllBagsUI> BagUI;

		public override void Load()
		{
			Instance = this;

			BagUI = Utility.SetupGUI<AllBagsUI>();
			BagUI.Visible = true;

			UIElement.GetElementAt += (orig, self, point) =>
			{
				if (self is AllBagsUI ui)
				{
					Terraria.UI.UIElement uIElement = null;
					for (int i = ui.Elements.Count - 1; i >= 0; i--)
					{
						if (ui.Elements[i].ContainsPoint(point)) uIElement = ui.Elements[i];
					}

					if (uIElement != null) return uIElement.GetElementAt(point);
					return self.ContainsPoint(point) ? self : null;
				}

				return orig?.Invoke(self, point);
			};
		}

		public override void Unload()
		{
			Utility.UnloadNullableTypes();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int InventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

			if (BagUI != null && InventoryIndex != -1) layers.Insert(InventoryIndex + 1, BagUI.InterfaceLayer);
		}
	}
}