using BaseLibrary;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace PortableStorage
{
	public static class Utility
	{
		public static class API
		{
			public enum WhitelistType
			{
				AlchemyIngredient,
				Ore,
				Explosive,
				Fishing,
				Seed
			}

			public static void Register(WhitelistType type, int itemID)
			{
				switch (type)
				{
					case WhitelistType.AlchemyIngredient:
						if (!AlchemistBagWhitelist.Contains(itemID)) AlchemistBagWhitelist.Add(itemID);
						break;
					case WhitelistType.Ore:
						if (!OreWhitelist.Contains(itemID)) OreWhitelist.Add(itemID);
						break;
					case WhitelistType.Explosive:
						if (!ExplosiveWhitelist.Contains(itemID)) ExplosiveWhitelist.Add(itemID);
						break;
					case WhitelistType.Fishing:
						if (!FishingWhitelist.Contains(itemID)) FishingWhitelist.Add(itemID);
						break;
					case WhitelistType.Seed:
						if (!SeedWhitelist.Contains(itemID)) SeedWhitelist.Add(itemID);
						break;
				}
			}
		}

		internal static readonly Dictionary<string, MultiValueDictionary<int, int>> Ammos = new Dictionary<string, MultiValueDictionary<int, int>>();

		internal static List<int> AlchemistBagWhitelist;

		internal static List<int> OreWhitelist;

		internal static List<int> ExplosiveWhitelist;

		internal static List<int> FishingWhitelist;

		internal static List<int> SeedWhitelist;

		internal static void PostSetupContent()
		{
			AlchemistBagWhitelist = new List<int>
			{
				ItemID.Bottle,
				ItemID.BottledWater,
				ItemID.LesserHealingPotion,
				ItemID.LesserManaPotion,
				ItemID.Gel,
				ItemID.Mushroom,
				ItemID.PinkGel,
				ItemID.GlowingMushroom,
				ItemID.FallenStar,
				ItemID.PixieDust,
				ItemID.CrystalShard,
				ItemID.UnicornHorn,
				ItemID.Daybloom,
				ItemID.Moonglow,
				ItemID.Blinkroot,
				ItemID.Deathweed,
				ItemID.Waterleaf,
				ItemID.Fireblossom,
				ItemID.Shiverthorn,
				ItemID.SpecularFish,
				ItemID.ChaosFish,
				ItemID.Lens,
				ItemID.RottenChunk,
				ItemID.Vertebrae,
				ItemID.DoubleCod,
				ItemID.Damselfish,
				ItemID.Amber,
				ItemID.Cobweb,
				ItemID.ArmoredCavefish,
				ItemID.Feather,
				ItemID.CrispyHoneyBlock,
				ItemID.Coral,
				ItemID.CrimsonTigerfish,
				ItemID.SharkFin,
				ItemID.Obsidifish,
				ItemID.FlarefinKoi,
				ItemID.Prismite,
				ItemID.IronOre,
				ItemID.GoldOre,
				ItemID.LeadOre,
				ItemID.PlatinumOre,
				ItemID.PrincessFish,
				ItemID.Obsidian,
				ItemID.AntlionMandible,
				ItemID.Hemopiranha,
				ItemID.Stinkfish,
				ItemID.VariegatedLardfish,
				ItemID.Cactus,
				ItemID.WormTooth,
				ItemID.Stinger,
				ItemID.Bone,
				ItemID.FrostMinnow,
				ItemID.Ebonkoi,
				ItemID.GreaterHealingPotion,
				ItemID.GreaterManaPotion,
				ItemID.FragmentNebula,
				ItemID.FragmentSolar,
				ItemID.FragmentStardust,
				ItemID.FragmentVortex
			};

			OreWhitelist = new List<int>
			{
				ItemID.LunarOre,
				ItemID.FossilOre,
				ItemID.TinOre,
				ItemID.CrimtaneOre,
				ItemID.LeadOre,
				ItemID.TungstenOre,
				ItemID.PlatinumOre,
				ItemID.IronOre,
				ItemID.CopperOre,
				ItemID.GoldOre,
				ItemID.SilverOre,
				ItemID.DemoniteOre,
				ItemID.CobaltOre,
				ItemID.MythrilOre,
				ItemID.AdamantiteOre,
				ItemID.ChlorophyteOre,
				ItemID.OrichalcumOre,
				ItemID.TitaniumOre,
				ItemID.PalladiumOre,
				ItemID.Hellstone
			};

			FishingWhitelist = new List<int>
			{
				2303,
				2299,
				2290,
				2436,
				2317,
				2305,
				2304,
				2313,
				2318,
				2312,
				2306,
				2308,
				2437,
				2319,
				2314,
				2302,
				2315,
				2438,
				2307,
				2310,
				2301,
				2298,
				2316,
				2309,
				2321,
				2297,
				2300,
				2311,
				2475,
				2476,
				2450,
				2477,
				2478,
				2451,
				2479,
				2480,
				2452,
				2453,
				2481,
				2454,
				2482,
				2483,
				2455,
				2456,
				2457,
				2458,
				2459,
				2460,
				2484,
				2472,
				2461,
				2462,
				2463,
				2485,
				2464,
				2465,
				2486,
				2466,
				2467,
				2468,
				2487,
				2469,
				2488,
				2470,
				2471,
				2473,
				2474,
				2334,
				2335,
				3208,
				3206,
				3203,
				3204,
				3207,
				3205,
				2336,
				2337,
				2338,
				2339
			};

			ExplosiveWhitelist = new List<int>
			{
				ItemID.Bomb,
				ItemID.StickyBomb,
				ItemID.BouncyBomb,
				ItemID.Dynamite,
				ItemID.StickyDynamite,
				ItemID.BouncyDynamite
			};

			SeedWhitelist = new List<int>
			{
				ItemID.DaybloomSeeds,
				ItemID.MoonglowSeeds,
				ItemID.BlinkrootSeeds,
				ItemID.DeathweedSeeds,
				ItemID.WaterleafSeeds,
				ItemID.FireblossomSeeds,
				ItemID.ShiverthornSeeds
			};

			void Add(string key, int ammoType)
			{
				BaseLibrary.Utility.Cache.ItemCache.Where(item => item?.ammo == ammoType).Select(item => item.type).ForEach(itemType =>
				{
					if (!Ammos.ContainsKey(key)) Ammos.Add(key, new MultiValueDictionary<int, int>());

					Ammos[key].Add(ammoType, itemType);
				});
			}

			Add("Misc", AmmoID.FallenStar);
			Add("Misc", AmmoID.Sand);
			Add("Misc", AmmoID.Snowball);
			Add("Misc", AmmoID.CandyCorn);
			Add("Misc", AmmoID.Stake);

			Add("Flammable", AmmoID.Rocket);
			Add("Flammable", AmmoID.Gel);
			Add("Flammable", AmmoID.Flare);
			Add("Flammable", AmmoID.StyngerBolt);
			Add("Flammable", AmmoID.JackOLantern);

			Add("Bullet", AmmoID.Bullet);
			Add("Arrow", AmmoID.Arrow);
			Add("Dart", AmmoID.Dart);
			Add("Dart", AmmoID.NailFriendly);
			Add("Solution", AmmoID.Solution);
			Add("Coin", AmmoID.Coin);
		}

		internal static RecipeGroup yoyoStringGroup;

		internal static void AddRecipeGroups()
		{
			yoyoStringGroup = new RecipeGroup(() => "Any string", ItemID.RedString, ItemID.OrangeString, ItemID.YellowString, ItemID.LimeString, ItemID.GreenString, ItemID.TealString, ItemID.CyanString, ItemID.SkyBlueString, ItemID.BlueString, ItemID.PurpleString, ItemID.VioletString, ItemID.PinkString, ItemID.BrownString, ItemID.WhiteString, ItemID.RainbowString, ItemID.BlackString);
			RecipeGroup.RegisterGroup("PortableStorage:YoYoStrings", yoyoStringGroup);
		}

		internal static void SyncBag(this Item item)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				Player player = Main.player[item.owner];

				int index = player.inventory.ToList().FindIndex(x => x == item);
				if (index < 0) return;

				NetMessage.SendData(MessageID.SyncEquipment, number: item.owner, number2: index);
			}
		}
	}
}