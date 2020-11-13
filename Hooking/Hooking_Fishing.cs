namespace PortableStorage.Hooking
{
	public static partial class Hooking
	{
		// private delegate void ItemCheckDelegate(Player player, int index, ref bool foundBait, ref int baitType);
		//
		// private static void ItemCheck(Player player, int index, ref bool foundBait, ref int baitType)
		// {
		// 	foreach (Item item in player.inventory.OfType<FishingBelt>().SelectMany(belt => belt.Handler.Items))
		// 	{
		// 		if (item.stack > 0 && item.bait > 0)
		// 		{
		// 			bool consumeBait = false;
		// 			int baitPower = 1 + item.bait / 5;
		//
		// 			if (baitPower < 1) baitPower = 1;
		// 			if (player.accTackleBox) baitPower++;
		// 			if (Main.rand.Next(baitPower) == 0) consumeBait = true;
		// 			if (Main.projectile[index].localAI[1] < 0f) consumeBait = true;
		//
		// 			if (Main.projectile[index].localAI[1] > 0f)
		// 			{
		// 				Item fish = new Item();
		// 				fish.SetDefaults((int)Main.projectile[index].localAI[1]);
		// 				if (fish.rare < 0) consumeBait = false;
		// 			}
		//
		// 			if (consumeBait)
		// 			{
		// 				baitType = item.type;
		// 				if (ItemLoader.ConsumeItem(item, player)) item.stack--;
		// 				if (item.stack <= 0) item.SetDefaults();
		// 			}
		//
		// 			foundBait = true;
		// 			break;
		// 		}
		// 	}
		// }
		//
		// private static void Player_ItemCheck(ILContext il)
		// {
		// 	ILCursor cursor = new ILCursor(il);
		// 	ILLabel label = cursor.DefineLabel();
		//
		// 	if (cursor.TryGotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(29)))
		// 	{
		// 		cursor.Index += 2;
		//
		// 		cursor.Emit(OpCodes.Ldarg, 0);
		// 		cursor.Emit(OpCodes.Ldloc, 26);
		// 		cursor.Emit(OpCodes.Ldloca, 28);
		// 		cursor.Emit(OpCodes.Ldloca, 29);
		//
		// 		cursor.EmitDelegate<ItemCheckDelegate>(ItemCheck);
		//
		// 		cursor.Emit(OpCodes.Ldloc, 28);
		// 		cursor.Emit(OpCodes.Brtrue, label);
		// 	}
		//
		// 	if (cursor.TryGotoNext(i => i.MatchLdloc(28), i => i.MatchBrfalse(out _))) cursor.MarkLabel(label);
		// }
		//
		// private static int Player_FishingLevel(On.Terraria.Player.orig_FishingLevel orig, Player self)
		// {
		// 	Item fishingPole = self.inventory[self.selectedItem];
		//
		// 	if (fishingPole.fishingPole == 0)
		// 	{
		// 		for (int i = 0; i < 58; i++)
		// 		{
		// 			if (self.inventory[i].fishingPole > fishingPole.fishingPole) fishingPole = self.inventory[i];
		// 		}
		//
		// 		foreach (Item item in self.inventory.OfType<FishingBelt>().SelectMany(belt => belt.Handler.Items))
		// 		{
		// 			if (item.fishingPole > fishingPole.fishingPole) fishingPole = item;
		// 		}
		// 	}
		//
		// 	Item bait = new Item();
		// 	for (int i = 0; i < 58; i++)
		// 	{
		// 		if (self.inventory[i].stack > 0 && self.inventory[i].bait > 0)
		// 		{
		// 			if (self.inventory[i].type == 2673) return -1;
		//
		// 			bait = self.inventory[i];
		// 			break;
		// 		}
		// 	}
		//
		// 	if (bait.IsAir)
		// 	{
		// 		foreach (Item item in self.inventory.OfType<FishingBelt>().SelectMany(belt => belt.Handler.Items))
		// 		{
		// 			if (item.stack > 0 && item.bait > 0)
		// 			{
		// 				if (item.type == 2673) return -1;
		//
		// 				bait = item;
		// 				break;
		// 			}
		// 		}
		// 	}
		//
		// 	if (bait.IsAir || fishingPole.fishingPole == 0) return 0;
		//
		// 	int fishingLevel = bait.bait + fishingPole.fishingPole + self.fishingSkill;
		// 	if (Main.raining) fishingLevel = (int)(fishingLevel * 1.2f);
		// 	if (Main.cloudBGAlpha > 0f) fishingLevel = (int)(fishingLevel * 1.1f);
		// 	if (Main.dayTime && (Main.time < 5400.0 || Main.time > 48600.0)) fishingLevel = (int)(fishingLevel * 1.3f);
		// 	if (Main.dayTime && Main.time > 16200.0 && Main.time < 37800.0) fishingLevel = (int)(fishingLevel * 0.8f);
		// 	if (!Main.dayTime && Main.time > 6480.0 && Main.time < 25920.0) fishingLevel = (int)(fishingLevel * 0.8f);
		// 	if (Main.moonPhase == 0) fishingLevel = (int)(fishingLevel * 1.1f);
		// 	if (Main.moonPhase == 1 || Main.moonPhase == 7) fishingLevel = (int)(fishingLevel * 1.05f);
		// 	if (Main.moonPhase == 3 || Main.moonPhase == 5) fishingLevel = (int)(fishingLevel * 0.95f);
		// 	if (Main.moonPhase == 4) fishingLevel = (int)(fishingLevel * 0.9f);
		//
		// 	PlayerHooks.GetFishingLevel(self, fishingPole, bait, ref fishingLevel);
		// 	return fishingLevel;
		// }
		//
		// private static void Player_GetItem(ILContext il)
		// {
		// 	ILCursor cursor = new ILCursor(il);
		// 	ILLabel label = cursor.DefineLabel();
		//
		// 	if (cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdloc(3), i => i.MatchStloc(4)))
		// 	{
		// 		cursor.Emit(OpCodes.Ldarg, 0);
		// 		cursor.Emit(OpCodes.Ldarg, 2);
		//
		// 		cursor.EmitDelegate<Func<Player, Item, Item>>((player, item) =>
		// 		{
		// 			if (ItemUtility.BlockGetItem) return item;
		//
		// 			if (item.bait > 0 || Utility.FishingWhitelist.Contains(item.type))
		// 			{
		// 				FishingBelt belt = player.inventory.OfType<FishingBelt>().FirstOrDefault(bag => bag.Handler.HasSpace(item));
		//
		// 				if (belt != null)
		// 				{
		// 					Main.PlaySound(SoundID.Grab);
		//
		// 					Item temp = item.Clone();
		//
		// 					belt.Handler.InsertItem(ref item);
		//
		// 					BagItemText(belt.item, temp, temp.stack - item.stack, false, false);
		//
		// 					if (item.IsAir || !item.active) return item;
		// 				}
		// 			}
		//
		// 			if (item.stack <= 0) item.active = false;
		// 			return item;
		// 		});
		//
		// 		cursor.Emit(OpCodes.Starg, 2);
		//
		// 		cursor.Emit(OpCodes.Ldarg, 2);
		// 		cursor.EmitDelegate<Func<Item, bool>>(item => item.IsAir);
		// 		cursor.Emit(OpCodes.Brfalse, label);
		// 		cursor.Emit(OpCodes.Ldarg, 2);
		// 		cursor.Emit(OpCodes.Ret);
		// 		cursor.MarkLabel(label);
		// 	}
		// }
	}
}