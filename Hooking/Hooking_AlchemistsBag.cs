using System;
using System.Linq;
using System.Reflection;
using BaseLibrary.Utility;
using ContainerLibrary;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Hooking;

public static partial class Hooking
{
	private static void QuickBuff_Del(Player player, ref SoundStyle? sound)
	{
		if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickBuff) return;

		if (player.CountBuffs() == Player.MaxBuffs) return;

		foreach (AlchemistBag bag in player.inventory.OfModItemType<AlchemistBag>())
		{
			ItemStorage storage = bag.GetItemStorage();

			for (int i = 0; i < storage.Count; i++)
			{
				Item item = storage[i];

				if (item.IsAir || item.buffType <= 0 || item.DamageType == DamageClass.Summon) continue;

				int buffType = item.buffType;
				bool canUse = CombinedHooks.CanUseItem(player, item) && QuickBuff_ShouldBotherUsingThisBuff.Invoke<bool>(player, buffType);
				if (item.mana > 0 && canUse)
				{
					if (player.CheckMana(item, -1, true, true)) player.manaRegenDelay = (int)player.maxRegenDelay;
				}

				if (player.whoAmI == Main.myPlayer && item.type == ItemID.Carrot && !Main.runningCollectorsEdition) canUse = false;

				if (buffType == 27)
				{
					buffType = Main.rand.Next(3);
					if (buffType == 0) buffType = 27;
					if (buffType == 1) buffType = 101;
					if (buffType == 2) buffType = 102;
				}

				if (!canUse) continue;

				ItemLoader.UseItem(item, player);
				sound = item.UseSound;
				int buffTime = item.buffTime;
				if (buffTime == 0) buffTime = 3600;

				player.AddBuff(buffType, buffTime);
				if (item.consumable && ItemLoader.ConsumeItem(item, player)) storage.ModifyStackSize(player, i, -1);

				if (player.CountBuffs() == Player.MaxBuffs) return;
			}
		}
	}

	private static MethodInfo QuickBuff_ShouldBotherUsingThisBuff = typeof(Player).GetMethod("QuickBuff_ShouldBotherUsingThisBuff", ReflectionUtility.DefaultFlags);

	private static void QuickBuff(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloca(0), i => i.MatchCall(typeof(SoundStyle?).GetMethod("get_HasValue", ReflectionUtility.DefaultFlags)), i => i.MatchBrfalse(out _)))
		{
			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.Emit(OpCodes.Ldloca, 0);

			cursor.EmitDelegate(QuickBuff_Del);
		}
	}

	private static void QuickHeal_Del(Player player, int lostHealth, ref int healthGain, ref Item result)
	{
		if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickHeal) return;

		foreach (AlchemistBag bag in player.inventory.OfModItemType<AlchemistBag>())
		{
			ItemStorage storage = bag.GetItemStorage();

			for (int i = 0; i < storage.Count; i++)
			{
				Item item = storage[i];
				if (item.IsAir || !item.potion || item.healLife <= 0 || !CombinedHooks.CanUseItem(player, item)) continue;

				int healWaste = player.GetHealLife(item, true) - lostHealth;
				if (item.type == ItemID.RestorationPotion && healWaste < 0)
				{
					healWaste += 30;
					if (healWaste > 0)
						healWaste = 0;
				}

				if (healthGain < 0)
				{
					if (healWaste > healthGain)
					{
						result = item;
						healthGain = healWaste;
					}
				}
				else if (healWaste < healthGain && healWaste >= 0)
				{
					result = item;
					healthGain = healWaste;
				}
			}
		}
	}

	private static void QuickHeal(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(0), i => i.MatchStloc(3), i => i.MatchBr(out _)))
		{
			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.Emit(OpCodes.Ldloc, 0);
			cursor.Emit(OpCodes.Ldloca, 2);
			cursor.Emit(OpCodes.Ldloca, 1);

			cursor.EmitDelegate(QuickHeal_Del);
		}
	}

	private static void QuickMana(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);
		ILLabel label = cursor.DefineLabel();

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(0), i => i.MatchStloc(0)))
		{
			cursor.Emit(OpCodes.Ldarg, 0);

			cursor.EmitDelegate<Func<Player, bool>>(player =>
			{
				if (!ModContent.GetInstance<PortableStorageConfig>().AlchemistBagQuickMana) return false;

				foreach (AlchemistBag bag in player.inventory.OfModItemType<AlchemistBag>())
				{
					ItemStorage storage = bag.GetItemStorage();

					for (int i = 0; i < storage.Count; i++)
					{
						Item item = storage[i];
						if (item.IsAir || item.healMana <= 0 || (player.potionDelay > 0 && item.potion) || !CombinedHooks.CanUseItem(player, item)) continue;

						if (item.UseSound != null) SoundEngine.PlaySound(item.UseSound.Value, player.position);
						if (item.potion)
						{
							if (item.type == ItemID.RestorationPotion)
							{
								player.potionDelay = player.restorationDelayTime;
								player.AddBuff(21, player.potionDelay);
							}
							else if (item.type == ItemID.Mushroom)
							{
								player.potionDelay = player.mushroomDelayTime;
								player.AddBuff(21, player.potionDelay);
							}
							else
							{
								player.potionDelay = player.potionDelayTime;
								player.AddBuff(21, player.potionDelay);
							}
						}

						ItemLoader.UseItem(item, player);
						int healLife = player.GetHealLife(item, true);
						int healMana = player.GetHealMana(item, true);
						player.statLife += healLife;
						player.statMana += healMana;

						if (player.statLife > player.statLifeMax2) player.statLife = player.statLifeMax2;
						if (player.statMana > player.statManaMax2) player.statMana = player.statManaMax2;

						if (healLife > 0 && Main.myPlayer == player.whoAmI) player.HealEffect(healLife);

						if (healMana > 0)
						{
							player.AddBuff(94, Player.manaSickTime);
							if (Main.myPlayer == player.whoAmI) player.ManaEffect(healMana);
						}

						if (ItemLoader.ConsumeItem(item, player)) storage.ModifyStackSize(player, i, -1);

						Recipe.FindRecipes();

						return true;
					}
				}

				return false;
			});

			cursor.Emit(OpCodes.Brfalse, label);
			cursor.Emit(OpCodes.Ret);
			cursor.MarkLabel(label);
		}
	}

	private static void AdjTiles(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(i => i.MatchLdsfld<Main>("playerInventory")))
		{
			cursor.Emit(OpCodes.Ldarg, 0);

			cursor.EmitDelegate<Action<Player>>(player =>
			{
				if (player.inventory.OfModItemType<AlchemistBag>().Any())
				{
					player.adjTile[TileID.Bottles] = true;
					player.alchemyTable = true;
				}
			});
		}
	}

	private static void PickBestFoodItem_Del(Player player, ref Item foodItem, ref int num)
	{
		foreach (AlchemistBag bag in player.inventory.OfModItemType<AlchemistBag>())
		{
			ItemStorage storage = bag.GetItemStorage();

			for (int i = 0; i < storage.Count; i++)
			{
				Item item = storage[i];
				if (item.IsAir) continue;

				int priority = QuickBuff_FindFoodPriority.Invoke<int>(player, item.buffType);
				if (priority >= num && (foodItem == null || foodItem.buffTime < item.buffTime || priority > num))
				{
					foodItem = item;
					num = priority;
				}
			}
		}
	}

	private static MethodInfo QuickBuff_FindFoodPriority = typeof(Player).GetMethod("QuickBuff_FindFoodPriority", ReflectionUtility.DefaultFlags);

	private static void PickBestFoodItem(ILContext il)
	{
		ILCursor cursor = new ILCursor(il);

		if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(1), i => i.MatchRet()))
		{
			cursor.Emit(OpCodes.Ldarg, 0);
			cursor.Emit(OpCodes.Ldloca, 1);
			cursor.Emit(OpCodes.Ldloca, 0);

			cursor.EmitDelegate(PickBestFoodItem_Del);
		}
	}
}