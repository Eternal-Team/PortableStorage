using System.IO;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class ArrowQuiver : BaseAmmoBag
	{
		public override string AmmoType => "Arrow";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Arrow Quiver");
			Tooltip.SetDefault($"Stores {handler.Slots} stacks of arrows");
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