using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseLibrary;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using PortableStorage.UI.Bags;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class NinjaArsenalBelt : BaseBag
	{
		public override Type UIType => typeof(AlchemistBagPanel);

		public NinjaArsenalBelt()
		{
			Handler = new ItemHandler(18);
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
			Handler.IsItemValid += (handler, slot, item) => item.buffType > 0 && !item.summon && item.buffType != BuffID.Rudolph || item.potion && item.healLife > 0 || item.healMana > 0;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ninja's Arsenal Belt");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of throwing supplies");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
		}

		public static readonly (string key, string value, string color)[] tooltip =
		{
			("QuickHeal", "quick heal", new Color(23, 237, 47).ToHex()),
			("QuickMana", "quick mana", new Color(33, 124, 221).ToHex()),
			("QuickBuff", "quick buff", new Color(230, 255, 109).ToHex())
		};

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string quickHeal = PlayerInput.CurrentProfile.InputModes[InputMode.Keyboard].KeyStatus["QuickHeal"][0];

			//tooltips.Add(new TooltipLine(mod, "PortableStorage:AlchemistBagInfo", $"Pressing [c/{colorQuickHeal}:{quickHeal}] will quick heal you using the potions in the belt"));
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
			recipe.AddIngredient(ItemID.NinjaShirt);
			recipe.AddIngredient(ItemID.IronChainmail, 3);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}