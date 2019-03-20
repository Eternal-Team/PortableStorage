using System;
using BaseLibrary;
using BaseLibrary.Tiles.TileEntites;
using BaseLibrary.UI.Elements;
using Microsoft.Xna.Framework;
using PortableStorage.Global;
using Terraria.Audio;
using Terraria.ID;
using Colors = PortableStorage.Global.Colors;

namespace PortableStorage.TileEntities
{
	public abstract class BaseQETE : BaseTE
	{
		public abstract Type UIType { get; }

		public BaseElement UIInternal
		{
			get => this.GetValue<BaseElement>("UI");
			set => this.SetValue("UI", value);
		}

		public Vector2? UIPosition;

		public virtual LegacySoundStyle OpenSound => SoundID.Item1;
		public virtual LegacySoundStyle CloseSound => SoundID.Item1;

		public bool hovered;
		public bool inScreen;
		public float scale;

		public Frequency frequency = new Frequency(Colors.White, Colors.White, Colors.White);
	}
}