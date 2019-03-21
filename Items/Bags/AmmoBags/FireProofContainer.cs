using System.IO;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class FireProofContainer : BaseAmmoBag<AmmoBagPanel>
	{
		public override string AmmoType => "Flameable";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fire-proof Container");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of explosive or flameable ammo");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.value = Item.buyPrice(gold: 20);
		}

		public override TagCompound Save() => new TagCompound
		{
			["Items"] = Handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			Handler.Load(tag.GetCompound("Items"));
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}