using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items.SpecialBags;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private static IEnumerable<AlchemistBag> GetAlchemistBags(Player player)
		{
			foreach (Item item in player.inventory)
			{
				if (item.IsAir) continue;

				if (item.modItem is AlchemistBag bag) yield return bag;
			}
		}

		// private delegate bool QuickBuffDelegate(Player player, ref LegacySoundStyle sound);
		//
		// private static bool QuickBuff(Player player, ref LegacySoundStyle sound)
		// {
		// 	if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickBuff) return false;
		//
		// 	foreach (Item item in player.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items))
		// 	{
		// 		if (player.CountBuffs() == player.buffType.Length) return true;
		//
		// 		if (item.stack > 0 && item.type > 0 && item.buffType > 0 && !item.summon && item.buffType != 90)
		// 		{
		// 			int buffType = item.buffType;
		// 			bool useItem = ItemLoader.CanUseItem(item, player);
		// 			for (int i = 0; i < player.buffType.Length; i++)
		// 			{
		// 				if (buffType == 27 && (player.buffType[i] == buffType || player.buffType[i] == 101 || player.buffType[i] == 102))
		// 				{
		// 					useItem = false;
		// 					break;
		// 				}
		//
		// 				if (player.buffType[i] == buffType)
		// 				{
		// 					useItem = false;
		// 					break;
		// 				}
		//
		// 				if (Main.meleeBuff[buffType] && Main.meleeBuff[player.buffType[i]])
		// 				{
		// 					useItem = false;
		// 					break;
		// 				}
		// 			}
		//
		// 			if (Main.lightPet[item.buffType] || Main.vanityPet[item.buffType])
		// 			{
		// 				for (int buffIndex = 0; buffIndex < player.buffType.Length; buffIndex++)
		// 				{
		// 					if (Main.lightPet[player.buffType[buffIndex]] && Main.lightPet[item.buffType]) useItem = false;
		// 					if (Main.vanityPet[player.buffType[buffIndex]] && Main.vanityPet[item.buffType]) useItem = false;
		// 				}
		// 			}
		//
		// 			if (item.mana > 0 && useItem)
		// 			{
		// 				if (player.statMana >= (int)(item.mana * player.manaCost))
		// 				{
		// 					player.manaRegenDelay = (int)player.maxRegenDelay;
		// 					player.statMana -= (int)(item.mana * player.manaCost);
		// 				}
		// 				else useItem = false;
		// 			}
		//
		// 			if (player.whoAmI == Main.myPlayer && item.type == 603 && !Main.cEd) useItem = false;
		//
		// 			if (buffType == 27)
		// 			{
		// 				buffType = Main.rand.Next(3);
		// 				if (buffType == 0) buffType = 27;
		// 				if (buffType == 1) buffType = 101;
		// 				if (buffType == 2) buffType = 102;
		// 			}
		//
		// 			if (useItem)
		// 			{
		// 				ItemLoader.UseItem(item, player);
		// 				sound = item.UseSound;
		// 				int buffTime = item.buffTime;
		// 				if (buffTime == 0) buffTime = 3600;
		//
		// 				player.AddBuff(buffType, buffTime);
		// 				if (item.consumable)
		// 				{
		// 					if (ItemLoader.ConsumeItem(item, player)) item.stack--;
		// 					if (item.stack <= 0) item.TurnToAir();
		// 				}
		// 			}
		// 		}
		// 	}
		//
		// 	return false;
		// }
		//
		// private static void Player_QuickBuff(ILContext il)
		// {
		// 	ILCursor cursor = new ILCursor(il);
		// 	ILLabel label = cursor.DefineLabel();
		//
		// 	if (cursor.TryGotoNext(i => i.MatchLdnull(), i => i.MatchStloc(0)))
		// 	{
		// 		cursor.Index += 2;
		//
		// 		cursor.Emit(OpCodes.Ldarg_0);
		// 		cursor.Emit(OpCodes.Ldloca, 0);
		//
		// 		cursor.EmitDelegate<QuickBuffDelegate>(QuickBuff);
		//
		// 		cursor.Emit(OpCodes.Brfalse, label);
		// 		cursor.Emit(OpCodes.Ret);
		// 		cursor.MarkLabel(label);
		// 	}
		// }
		//
		// private delegate void QuickHealDelegate(Player player, int lostHealth, ref int healthGain, ref Item result);
		//
		// private static void QuickHeal(Player player, int lostHealth, ref int healthGain, ref Item result)
		// {
		// 	if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickHeal) return;
		//
		// 	foreach (Item item in player.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items))
		// 	{
		// 		if (item.stack > 0 && item.type > 0 && item.potion && item.healLife > 0 && ItemLoader.CanUseItem(item, player))
		// 		{
		// 			int healWaste = player.GetHealLife(item, true) - lostHealth;
		// 			if (healthGain < 0)
		// 			{
		// 				if (healWaste > healthGain)
		// 				{
		// 					result = item;
		// 					healthGain = healWaste;
		// 				}
		// 			}
		// 			else if (healWaste < healthGain && healWaste >= 0)
		// 			{
		// 				result = item;
		// 				healthGain = healWaste;
		// 			}
		// 		}
		// 	}
		// }
		//
		// private static void Player_QuickHeal_GetItemToUse(ILContext il)
		// {
		// 	ILCursor cursor = new ILCursor(il);
		//
		// 	if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(3), i => i.MatchBr(out _)))
		// 	{
		// 		cursor.Emit(OpCodes.Ldarg, 0);
		// 		cursor.Emit(OpCodes.Ldloc, 0);
		// 		cursor.Emit(OpCodes.Ldloca, 2);
		// 		cursor.Emit(OpCodes.Ldloca, 1);
		//
		// 		cursor.EmitDelegate<QuickHealDelegate>(QuickHeal);
		// 	}
		// }
		//
		// private static void Player_QuickMana(ILContext il)
		// {
		// 	ILCursor cursor = new ILCursor(il);
		// 	ILLabel label = cursor.DefineLabel();
		//
		// 	if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(0), i => i.MatchStloc(0)))
		// 	{
		// 		cursor.Emit(OpCodes.Ldarg, 0);
		//
		// 		cursor.EmitDelegate<Func<Player, bool>>(player =>
		// 		{
		// 			if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickMana) return false;
		//
		// 			foreach (Item item in player.inventory.OfType<AlchemistBag>().SelectMany(x => x.Handler.Items))
		// 			{
		// 				if (item.stack > 0 && item.type > 0 && item.healMana > 0 && (player.potionDelay == 0 || !item.potion) && ItemLoader.CanUseItem(item, player))
		// 				{
		// 					Main.PlaySound(item.UseSound, player.position);
		// 					if (item.potion)
		// 					{
		// 						if (item.type == ItemID.RestorationPotion)
		// 						{
		// 							player.potionDelay = player.restorationDelayTime;
		// 							player.AddBuff(BuffID.PotionSickness, player.potionDelay);
		// 						}
		// 						else
		// 						{
		// 							player.potionDelay = player.potionDelayTime;
		// 							player.AddBuff(BuffID.PotionSickness, player.potionDelay);
		// 						}
		// 					}
		//
		// 					ItemLoader.UseItem(item, player);
		// 					int healLife = player.GetHealLife(item, true);
		// 					int healMana = player.GetHealMana(item, true);
		// 					player.statLife += healLife;
		// 					player.statMana += healMana;
		// 					if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;
		// 					if (player.statMana > player.statManaMax2) player.statMana = player.statManaMax2;
		// 					if (healLife > 0 && Main.myPlayer == player.whoAmI) player.HealEffect(healLife);
		// 					if (healMana > 0)
		// 					{
		// 						player.AddBuff(BuffID.ManaSickness, Player.manaSickTime);
		// 						if (Main.myPlayer == player.whoAmI) player.ManaEffect(healMana);
		// 					}
		//
		// 					if (ItemLoader.ConsumeItem(item, player)) item.stack--;
		// 					if (item.stack <= 0) item.TurnToAir();
		//
		// 					Recipe.FindRecipes();
		// 					return true;
		// 				}
		// 			}
		//
		// 			return false;
		// 		});
		//
		// 		cursor.Emit(OpCodes.Brfalse, label);
		// 		cursor.Emit(OpCodes.Ret);
		// 		cursor.MarkLabel(label);
		// 	}
		// }
		
		private static void AdjTiles(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(i => i.MatchLdsfld<Main>("playerInventory"), i => i.MatchLdcI4(0)))
			{
				cursor.Emit(OpCodes.Ldarg, 0);

				cursor.EmitDelegate<Action<Player>>(player =>
				{
					if (GetAlchemistBags(player).Any())
					{
						player.adjTile[TileID.Bottles] = true;
						player.alchemyTable = true;
					}
				});
			}
		}
	}
}