using System;
using System.Collections.Generic;
using BaseLibrary;
using On.Terraria;
using PortableStorage.Items.Bags;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;
using Item = Terraria.Item;
using Main = Terraria.Main;
using NetMessage = Terraria.NetMessage;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static void Recipe_Create(Recipe.orig_Create orig, Terraria.Recipe self)
		{
			for (int i = 0; i < Terraria.Recipe.maxRequirements; i++)
			{
				Item item = self.requiredItem[i];
				if (item.type == 0) break;
				int amount = item.stack;
				if (self is ModRecipe modRecipe) amount = modRecipe.ConsumeItem(item.type, item.stack);
				if (self.alchemy && Main.LocalPlayer.alchemyTable)
				{
					// note: hook alchemist bag here
					int num2 = 0;
					for (int j = 0; j < amount; j++)
					{
						if (Main.rand.Next(3) == 0) num2++;
					}

					amount -= num2;
				}

				if (amount > 0)
				{
					Item[] array = Main.LocalPlayer.inventory;
					for (int k = 0; k < array.Length; k++)
					{
						ref Item invItem = ref array[k];
						if (amount <= 0) break;

						if (invItem.IsTheSameAs(item) || self.useWood(invItem.type, item.type) || self.useSand(invItem.type, item.type) || self.useFragment(invItem.type, item.type) || self.useIronBar(invItem.type, item.type) || self.usePressurePlate(invItem.type, item.type) || self.AcceptedByItemGroups(invItem.type, item.type))
						{
							int count = Math.Min(amount, invItem.stack);
							amount -= count;
							invItem.stack -= count;
							if (invItem.stack <= 0) invItem = new Item();
						}
					}

					if (Main.LocalPlayer.chest != -1)
					{
						if (Main.LocalPlayer.chest > -1) array = Main.chest[Main.LocalPlayer.chest].item;
						else if (Main.LocalPlayer.chest == -2) array = Main.LocalPlayer.bank.item;
						else if (Main.LocalPlayer.chest == -3) array = Main.LocalPlayer.bank2.item;
						else if (Main.LocalPlayer.chest == -4) array = Main.LocalPlayer.bank3.item;

						for (int l = 0; l < array.Length; l++)
						{
							ref Item invItem = ref array[l];
							if (amount <= 0) break;

							if (invItem.IsTheSameAs(item) || self.useWood(invItem.type, item.type) || self.useSand(invItem.type, item.type) || self.useIronBar(invItem.type, item.type) || self.usePressurePlate(invItem.type, item.type) || self.useFragment(invItem.type, item.type) || self.AcceptedByItemGroups(invItem.type, item.type))
							{
								int count = Math.Min(amount, invItem.stack);
								amount -= count;
								invItem.stack -= count;
								if (invItem.stack <= 0) invItem = new Item();

								if (Main.netMode == NetmodeID.MultiplayerClient && Main.LocalPlayer.chest >= 0) NetMessage.SendData(MessageID.SyncChestItem, -1, -1, null, Main.LocalPlayer.chest, l);
							}
						}
					}

					foreach (BaseBag bag in Main.LocalPlayer.inventory.OfType<BaseBag>())
					{
						for (int index = 0; index < bag.Handler.stacks.Count; index++)
						{
							if (amount <= 0) break;
							Item invItem = bag.Handler.stacks[index];

							if (invItem.IsTheSameAs(item) || self.useWood(invItem.type, item.type) || self.useSand(invItem.type, item.type) || self.useIronBar(invItem.type, item.type) || self.usePressurePlate(invItem.type, item.type) || self.useFragment(invItem.type, item.type) || self.AcceptedByItemGroups(invItem.type, item.type))
							{
								int count = Math.Min(amount, invItem.stack);
								amount -= count;
								bag.Handler.ExtractItem(index, count);
							}
						}
					}
				}
			}

			AchievementsHelper.NotifyItemCraft(self);
			AchievementsHelper.NotifyItemPickup(Main.LocalPlayer, self.createItem);
			Terraria.Recipe.FindRecipes();
		}

		private static void Recipe_FindRecipes(Recipe.orig_FindRecipes orig)
		{
			int focusIndex = Main.availableRecipe[Main.focusRecipe];
			float focusY = Main.availableRecipeY[Main.focusRecipe];
			for (int i = 0; i < Terraria.Recipe.maxRecipes; i++) Main.availableRecipe[i] = 0;
			Main.numAvailableRecipes = 0;
			bool guideMenu = Main.guideItem.type > 0 && Main.guideItem.stack > 0 && Main.guideItem.Name != "";
			if (guideMenu)
			{
				for (int i = 0; i < Terraria.Recipe.maxRecipes; i++)
				{
					if (Main.recipe[i].createItem.type == 0) break;
					int index = 0;
					while (index < Terraria.Recipe.maxRequirements && Main.recipe[i].requiredItem[index].type != 0)
					{
						if (Main.guideItem.IsTheSameAs(Main.recipe[i].requiredItem[index]) || Main.recipe[i].useWood(Main.guideItem.type, Main.recipe[i].requiredItem[index].type) || Main.recipe[i].useSand(Main.guideItem.type, Main.recipe[i].requiredItem[index].type) || Main.recipe[i].useIronBar(Main.guideItem.type, Main.recipe[i].requiredItem[index].type) || Main.recipe[i].useFragment(Main.guideItem.type, Main.recipe[i].requiredItem[index].type) || Main.recipe[i].AcceptedByItemGroups(Main.guideItem.type, Main.recipe[i].requiredItem[index].type) || Main.recipe[i].usePressurePlate(Main.guideItem.type, Main.recipe[i].requiredItem[index].type))
						{
							Main.availableRecipe[Main.numAvailableRecipes] = i;
							Main.numAvailableRecipes++;
							break;
						}

						index++;
					}
				}
			}
			else
			{
				Dictionary<int, int> availableItems = new Dictionary<int, int>();
				Item item;
				Item[] array = Main.LocalPlayer.inventory;
				for (int i = 0; i < array.Length; i++)
				{
					item = array[i];
					if (item.stack > 0)
					{
						if (availableItems.ContainsKey(item.netID)) availableItems[item.netID] += item.stack;
						else availableItems[item.netID] = item.stack;
					}
				}

				if (Main.player[Main.myPlayer].chest != -1)
				{
					if (Main.player[Main.myPlayer].chest > -1) array = Main.chest[Main.player[Main.myPlayer].chest].item;
					else if (Main.player[Main.myPlayer].chest == -2) array = Main.player[Main.myPlayer].bank.item;
					else if (Main.player[Main.myPlayer].chest == -3) array = Main.player[Main.myPlayer].bank2.item;
					else if (Main.player[Main.myPlayer].chest == -4) array = Main.player[Main.myPlayer].bank3.item;

					for (int i = 0; i < array.Length; i++)
					{
						item = array[i];
						if (item.stack > 0)
						{
							if (availableItems.ContainsKey(item.netID)) availableItems[item.netID] += item.stack;
							else availableItems[item.netID] = item.stack;
						}
					}
				}

				foreach (BaseBag bag in Main.LocalPlayer.inventory.OfType<BaseBag>())
				{
					for (int i = 0; i < bag.Handler.stacks.Count; i++)
					{
						item = bag.Handler.stacks[i];
						if (item.stack > 0)
						{
							if (availableItems.ContainsKey(item.netID)) availableItems[item.netID] += item.stack;
							else availableItems[item.netID] = item.stack;
						}
					}
				}

				int index = 0;
				while (index < Terraria.Recipe.maxRecipes && Main.recipe[index].createItem.type != 0)
				{
					bool hasTile = true;
					int tileIndex = 0;
					while (tileIndex < Terraria.Recipe.maxRequirements && Main.recipe[index].requiredTile[tileIndex] != -1)
					{
						// note: hook alchemist bag here
						if (!Main.player[Main.myPlayer].adjTile[Main.recipe[index].requiredTile[tileIndex]])
						{
							hasTile = false;
							break;
						}

						tileIndex++;
					}

					if (hasTile)
					{
						for (int m = 0; m < Terraria.Recipe.maxRequirements; m++)
						{
							item = Main.recipe[index].requiredItem[m];
							if (item.type == 0) break;

							int stack = item.stack;
							bool recGroup = false;
							foreach (int current in availableItems.Keys)
							{
								if (Main.recipe[index].useWood(current, item.type) || Main.recipe[index].useSand(current, item.type) || Main.recipe[index].useIronBar(current, item.type) || Main.recipe[index].useFragment(current, item.type) || Main.recipe[index].AcceptedByItemGroups(current, item.type) || Main.recipe[index].usePressurePlate(current, item.type))
								{
									stack -= availableItems[current];
									recGroup = true;
								}
							}

							if (!recGroup && availableItems.ContainsKey(item.netID))
							{
								stack -= availableItems[item.netID];
							}

							if (stack > 0)
							{
								hasTile = false;
								break;
							}
						}
					}

					if (hasTile)
					{
						bool flag4 = !Main.recipe[index].needWater || Main.player[Main.myPlayer].adjWater || Main.player[Main.myPlayer].adjTile[172];
						bool flag5 = !Main.recipe[index].needHoney || Main.recipe[index].needHoney == Main.player[Main.myPlayer].adjHoney;
						bool flag6 = !Main.recipe[index].needLava || Main.recipe[index].needLava == Main.player[Main.myPlayer].adjLava;
						bool flag7 = !Main.recipe[index].needSnowBiome || Main.player[Main.myPlayer].ZoneSnow;
						if (!flag4 || !flag5 || !flag6 || !flag7)
						{
							hasTile = false;
						}
					}

					if (hasTile && RecipeHooks.RecipeAvailable(Main.recipe[index]))
					{
						Main.availableRecipe[Main.numAvailableRecipes] = index;
						Main.numAvailableRecipes++;
					}

					index++;
				}
			}

			for (int n = 0; n < Main.numAvailableRecipes; n++)
			{
				if (focusIndex == Main.availableRecipe[n])
				{
					Main.focusRecipe = n;
					break;
				}
			}

			if (Main.focusRecipe >= Main.numAvailableRecipes) Main.focusRecipe = Main.numAvailableRecipes - 1;
			if (Main.focusRecipe < 0) Main.focusRecipe = 0;

			float num7 = Main.availableRecipeY[Main.focusRecipe] - focusY;
			for (int num8 = 0; num8 < Terraria.Recipe.maxRecipes; num8++)
			{
				Main.availableRecipeY[num8] -= num7;
			}
		}
	}
}