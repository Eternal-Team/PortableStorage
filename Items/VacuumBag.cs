using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TheOneLibrary.Base.UI;
using TheOneLibrary.Storage;
using TheOneLibrary.Utility;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage.Items
{
    public class VacuumBag : BaseBag, IContainerItem
    {
        public bool active;
        public Guid guid = Guid.NewGuid();
        public List<Item> Items = new List<Item>();

        public override string Texture => PortableStorage.ItemTexturePath + "VacuumBagActive";

        public override ModItem Clone(Item item)
        {
            VacuumBag clone = (VacuumBag)base.Clone(item);
            clone.Items = Items;
            clone.guid = guid;
            clone.active = active;
            return clone;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vacuum Bag");
            Tooltip.SetDefault("Right-click it to enable/disable vacuum mode\nSucks up items!");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 4));
        }

        public override void SetDefaults()
        {
            if (!Items.Any())
            {
                for (int i = 0; i < 27; i++) Items.Add(new Item());
            }

            item.width = 36;
            item.height = 40;
            item.useTime = 5;
            item.useAnimation = 5;
            item.noUseGraphic = true;
            item.useStyle = 1;
            item.value = GetItemValue(ItemID.Leather) * 10;
            item.rare = 1;
            item.accessory = true;
        }

        public override void HandleUI()
        {
            if (!PortableStorage.Instance.BagUI.ContainsKey(guid))
            {
                VacuumBagUI ui = new VacuumBagUI();
                UserInterface userInterface = new UserInterface();
                ui.Activate();
                userInterface.SetState(ui);
                ui.visible = true;
                ui.Load(this);
                PortableStorage.Instance.BagUI.Add(guid, new GUI(ui, userInterface));
            }
            else PortableStorage.Instance.BagUI.Remove(guid);

            Main.PlaySound(SoundID.Item59);
        }

        public override bool UseItem(Player player)
        {
            HandleUI();

            return true;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            item.stack++;
            active = !active;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
        }

        public float posY;
        public bool up;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            posY += up ? -0.08f : 0.08f;
            if (posY <= -2) up = false;
            else if (posY >= 2) up = true;
            spriteBatch.Draw(active ? PortableStorage.vacuumBagOn : PortableStorage.vacuumBagOff, position + new Vector2(0, posY), frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);

            return false;
        }

        public override TagCompound Save() => new TagCompound {["Items"] = Items.Save(), ["GUID"] = guid.ToString(), ["Active"] = active};

        public override void Load(TagCompound tag)
        {
            Items = TheOneLibrary.Utility.Utility.Load(tag);
            guid = tag.ContainsKey("GUID") && !string.IsNullOrEmpty((string)tag["GUID"]) ? Guid.Parse(tag.GetString("GUID")) : Guid.NewGuid();
            active = tag.GetBool("Active");
        }

        public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

        public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Leather, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnCraft(Recipe recipe)
        {
            Items.Clear();
            for (int i = 0; i < 27; i++) Items.Add(new Item());
        }

        public Item GetItem(int slot) => Items[slot];

        public void SetItem(int slot, Item value)
        {
            Items[slot] = value;
            NetUtility.SyncItem(item);
        }

        public List<Item> GetItems() => Items;
        
        public ModItem GetModItem() => this;
    }
}