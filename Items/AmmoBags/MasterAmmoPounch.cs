using System;
using System.IO;
using PortableStorage.UI;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class MasterAmmoPounch : BaseAmmoBag
	{
		public override Type UIType => typeof(TheBlackHolePanel);
		public override string AmmoType => "All";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Master Ammo Pounch");
			Tooltip.SetDefault($"Stores {handler.Slots} stacks of ammo");
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