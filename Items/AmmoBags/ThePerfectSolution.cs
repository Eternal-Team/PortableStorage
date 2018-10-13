using System;
using System.IO;
using PortableStorage.UI;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class ThePerfectSolution : BaseAmmoBag
	{
		public override string AmmoType => "Solution";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Perfect Solution");
			Tooltip.SetDefault($"Stores {handler.Slots} stacks of solutions");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
		}

		public override TagCompound Save() => new TagCompound
		{
			["Items"] = handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			handler.Load(tag.GetCompound("Items"));
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}