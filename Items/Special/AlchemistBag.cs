using ContainerLibrary;
using PortableStorage.Global;
using System.Collections.Generic;
using System.IO;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Utility = PortableStorage.Global.Utility;

namespace PortableStorage.Items.Special
{
	public class AlchemistBag : BaseBag, ICraftingStorage
	{
		public override string Texture => "PortableStorage/Textures/Items/AlchemistBag";

		public ItemHandler HandlerIngredients;

		public new ItemHandler CraftingHandler => HandlerIngredients;

		public AlchemistBag()
		{
			Handler = new ItemHandler(18);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => item.buffType > 0 && !item.summon && item.buffType != BuffID.Rudolph && item.buffType != BuffID.MiniMinotaur || item.potion && (item.healLife > 0 || item.healMana > 0);

			HandlerIngredients = new ItemHandler(63);
			HandlerIngredients.OnContentsChanged += slot => item.SyncBag();
			HandlerIngredients.IsItemValid += (slot, item) => Utility.AlchemistBagWhitelist.Contains(item.type);
		}

		public override ModItem Clone()
		{
			AlchemistBag clone = (AlchemistBag)base.Clone();
			clone.HandlerIngredients = HandlerIngredients.Clone();
			return clone;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 32;
			item.height = 32;
			item.rare = ItemRarityID.Orange;
			item.value = 30000 * 5;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, "PortableStorage:BagTooltip", Language.GetText("Mods.PortableStorage.BagTooltip." + GetType().Name).Format(Handler.Slots, HandlerIngredients.Slots)));
		}

		public override TagCompound Save()
		{
			TagCompound tag = base.Save();
			tag["Ingredients"] = HandlerIngredients.Save();
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			base.Load(tag);
			HandlerIngredients.Load(tag.GetCompound("Ingredients"));
		}

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			HandlerIngredients.Serialize(writer);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			base.NetRecieve(reader);
			HandlerIngredients.Deserialize(reader);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 10);
			recipe.AddIngredient(ItemID.FossilOre, 15);
			recipe.AddIngredient(ItemID.AlchemyTable);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}