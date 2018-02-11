using Terraria.ID;
using Terraria.ModLoader;
using TheOneLibrary.Base.Items;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage.Items
{
    public class QETank : BaseItem
    {
        public override string Texture => PortableStorage.ItemTexturePath + "QETank";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quantum Entangled Tank");
            Tooltip.SetDefault("Right-click on the slots with gems to change frequency");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = GetItemValue(ItemID.Obsidian) * 15 + GetItemValue(ItemID.Glass) * 15 + GetItemValue(ItemID.ShadowScale) * 25 + GetItemValue(ItemID.DemoniteBar) * 5;
            item.createTile = mod.TileType<Tiles.QETank>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Obsidian, 15);
            recipe.AddIngredient(ItemID.Glass, 15);
            recipe.AddIngredient(ItemID.ShadowScale, 25);
            recipe.AddIngredient(ItemID.DemoniteBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Obsidian, 15);
            recipe.AddIngredient(ItemID.Glass, 15);
            recipe.AddIngredient(ItemID.TissueSample, 25);
            recipe.AddIngredient(ItemID.CrimtaneBar, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}