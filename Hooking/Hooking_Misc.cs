using ContainerLibrary;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Hooking;

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
					if (pItem.ModItem is BaseBag bag)
					{
						if (bag is Wallet wallet)
						{
							walletCoins += wallet.GetItemStorage().CountCoins();
							continue;
						}

						ItemStorage storage = bag.GetItemStorage();
						foreach (Item item in storage)
						{
							if (item.IsAir) continue;

							if (item.ammo == AmmoID.Bullet || item.useAmmo == AmmoID.Bullet) condArmsDealer = true;
							if (ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[item.type]) condDemolitionist = true;
							if (item.dye > 0 || item.type is >= ItemID.TealMushroom and <= ItemID.DyeVat or >= ItemID.StrangePlant1 and <= ItemID.StrangePlant4) condDyeTrader = true;
						}
					}
				}

				coins = (int)Utils.Clamp(walletCoins + coins, 0, int.MaxValue);
			});
		}
	}
}