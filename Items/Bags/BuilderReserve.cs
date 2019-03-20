using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContainerLibrary;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class BuilderReserve : BaseBag
	{
		public override Type UIType => typeof(BuilderReservePanel);

		public int selectedIndex;

		public BuilderReserve()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot =>
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
			Handler.IsItemValid += (handler, slot, item) => item.createTile > 0 && (handler.stacks.All(x => x.type != item.type) || handler.stacks[slot].type == item.type);
			Handler.GetSlotLimit += slot => int.MaxValue;

			selectedIndex = -1;
		}

		public override ModItem Clone()
		{
			BuilderReserve clone = (BuilderReserve)base.Clone();
			clone.selectedIndex = selectedIndex;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Builder's Reserve");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of tiles");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.autoReuse = true;
			item.useTurn = true;
		}

		public override bool CanUseItem(Player player) => selectedIndex >= 0 && Handler.stacks[selectedIndex].type > 0 && Handler.stacks[selectedIndex].stack > 1;

		public override bool UseItem(Player player) => false;

		public void SetIndex(int index)
		{
			if (index == -1) selectedIndex = item.placeStyle = item.createTile = -1;
			else
			{
				selectedIndex = index;
				Item selectedItem = Handler.stacks[selectedIndex];

				if (selectedItem.createTile >= 0)
				{
					item.createTile = selectedItem.createTile;
					item.createWall = -1;
					item.placeStyle = selectedItem.placeStyle;
				}
			}

			//(UI as DevNullPanel)?.RefreshTextures();
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

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bone, 30);
			recipe.AddIngredient(ItemID.IronCrate);
			recipe.AddTile(TileID.BoneWelder);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}