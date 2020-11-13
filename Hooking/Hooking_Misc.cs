using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.Container;

namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		private delegate void SpawnTownNPCs_Del(Player player, ref int coins, ref bool f, ref bool f1, ref bool f3);

		private static void SpawnTownNPCs(ILContext il)
		{
			ILCursor cursor = new ILCursor(il);

			if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdsfld<Main>("player"), i => i.MatchLdloc(85), i => i.MatchLdelemRef()))
			{
				cursor.Emit<Main>(OpCodes.Ldsfld, "player");
				cursor.Emit(OpCodes.Ldloc, 85);
				cursor.Emit(OpCodes.Ldelem_Ref);

				cursor.Emit(OpCodes.Ldloca, 33);
				cursor.Emit(OpCodes.Ldloca, 36);
				cursor.Emit(OpCodes.Ldloca, 37);
				cursor.Emit(OpCodes.Ldloca, 38);

				cursor.EmitDelegate<SpawnTownNPCs_Del>((Player player, ref int coins, ref bool condArmsDealer, ref bool condDemolitionist, ref bool condDyeTrader) =>
				{
					long walletCoins = 0;

					foreach (Item pItem in player.inventory)
					{
						if (pItem.modItem is BaseBag bag)
						{
							if (bag is Wallet wallet)
							{
								walletCoins += wallet.Handler.CountCoins();
								continue;
							}

							ItemHandler handler = bag.GetItemHandler();
							foreach (Item item in handler.Items)
							{
								if (item.IsAir) continue;

								if (item.ammo == AmmoID.Bullet || item.useAmmo == AmmoID.Bullet) condArmsDealer = true;
								if (ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[item.type]) condDemolitionist = true;
								if (item.dye > 0 || item.type >= ItemID.TealMushroom && item.type <= ItemID.DyeVat || item.type >= ItemID.StrangePlant1 && item.type <= ItemID.StrangePlant4) condDyeTrader = true;
							}
						}
					}

					coins = (int)Utils.Clamp(walletCoins + coins, 0, int.MaxValue);
				});
			}
		}
	}
}