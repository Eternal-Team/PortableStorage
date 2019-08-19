using ContainerLibrary;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Special
{
	public class NinjaArsenalBelt : BaseBag
	{
		public override string Texture => "PortableStorage/Textures/Items/NinjaArsenalBelt";

		public NinjaArsenalBelt()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => item.thrown;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 30;
			item.rare = ItemRarityID.Blue;
			item.value = 9000 * 5;
		}

		public override void UpdateInventory(Player player)
		{
			if (UI != null) return;

			for (int i = 0; i < 10; i++)
			{
				ref Item item = ref player.inventory[i];

				if (item == null || item.IsAir || !item.thrown || item.stack == item.maxStack) continue;

				for (int j = 0; j < Handler.Slots; j++)
				{
					Item handlerItem = Handler.GetItemInSlot(i);

					if (handlerItem.type == item.type)
					{
						int count = Math.Min(item.maxStack - item.stack, handlerItem.stack);
						item.stack += count;
						handlerItem.stack -= count;
						if (handlerItem.stack <= 0) handlerItem.TurnToAir();
					}
				}
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.NinjaShirt);
			recipe.AddIngredient(ItemID.Chain, 3);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}