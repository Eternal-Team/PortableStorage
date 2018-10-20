﻿using BaseLibrary.UI.Elements;
using BaseLibrary.Utility;
using ContainerLibrary;
using PortableStorage.TileEntities;
using Terraria;
using Terraria.Map;

namespace PortableStorage.UI.TileEntities
{
	public class QETankPanel : BaseTEPanel
	{
		public UITank tankFluid;

		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (172, 0);
			this.Center();
			SetPadding(0);

			textLabel = new UIText(Lang.GetMapObjectName(MapHelper.TileToLookup(PortableStorage.Instance.TileType(te.TileType.Name), 0)))
			{
				Top = (8, 0),
				HAlign = 0.5f
			};
			Append(textLabel);

			tankFluid = new UITank(((TEQETank)te).Handler)
			{
				Width = (-16, 1),
				Height = (-44, 1),
				Left = (8, 0),
				Top = (36, 0)
			};
			Append(tankFluid);
		}
	}
}