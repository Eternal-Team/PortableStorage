using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContainerLibrary;
using PortableStorage.Items.Ammo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Utility = PortableStorage.Global.Utility;

namespace PortableStorage.Items.Bags
{
	public class MasterAmmoPouch : BaseAmmoBag
	{
		public override string AmmoType => "All";

		public new ItemHandler Handler { get; set; }

		public ItemHandler HandlerBags;

		public MasterAmmoPouch()
		{
			HandlerBags = new ItemHandler(18);
			HandlerBags.OnContentsChanged += slot =>
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
			HandlerBags.IsItemValid += (slot, item) => item.modItem is BaseAmmoBag;
		}

		public override ModItem Clone()
		{
			MasterAmmoPouch clone = (MasterAmmoPouch)base.Clone();
			clone.HandlerBags = HandlerBags;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Master Ammo Pouch");
			Tooltip.SetDefault($"Stores {HandlerBags.Slots} stacks of ammo");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
		}

		public override TagCompound Save()
		{
			TagCompound tag = base.Save();
			tag["Bags"] = HandlerBags.Save();
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			base.Load(tag);
			HandlerBags.Load(tag.GetCompound("Bags"));
		}

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			HandlerBags.Serialize(writer);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			base.NetRecieve(reader);
			HandlerBags.Deserialize(reader);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup(Utility.tier1HMBarsGroup.GetText(), 15);
			recipe.AddRecipeGroup(Utility.ichorFlameGroup.GetText(), 10);
			recipe.AddIngredient(ItemID.SoulofNight, 7);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}