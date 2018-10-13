using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContainerLibrary;
using PortableStorage.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public abstract class BaseNormalBag : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/Bag";
		public override Type UIType => typeof(BagPanel);

		public virtual int SlotCount { get; }
		public new virtual string Name { get; }

		public BaseNormalBag()
		{
			handler = new ItemHandler(SlotCount);
			handler.OnContentsChanged += slot =>
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					Player player = Main.player[item.owner];

					List<Item> joined = player.inventory.Concat(player.armor).Concat(player.dye).Concat(player.miscEquips).Concat(player.miscDyes).Concat(player.bank.item).Concat(player.bank2.item).Concat(new[] { player.trashItem }).Concat(player.bank3.item).ToList();
					int index = joined.FindIndex(x => x == item);
					if (index < 0) return;

					NetMessage.SendData(MessageID.SyncEquipment, number: item.owner, number2: index);
				}
			};
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault($"{Name} Bag");
			Tooltip.SetDefault($"Stores {SlotCount} stacks of items");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 34;
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