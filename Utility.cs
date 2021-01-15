using System.Collections.Generic;
using System.Linq;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;

namespace PortableStorage
{
	// todo: switch this to ItemTags once they get merged in?

	public static class Utility
	{
		public struct AmmoTypeGroup
		{
			public int AmmoType;
			public List<int> AmmoItems;

			public AmmoTypeGroup(int ammoType)
			{
				AmmoType = ammoType;
				AmmoItems = new List<int>();

				// foreach (var (type,item) in ContentSamples.ItemsByType)
				foreach (var pair in ContentSamples.ItemsByType.Where(pair => pair.Value.ammo == ammoType))
				{
					AmmoItems.Add(pair.Key);
				}
			}
		}

		internal static Dictionary<string, List<AmmoTypeGroup>> Ammos;
		internal static List<int> AlchemistBagWhitelist;
		internal static List<int> OreWhitelist;
		internal static List<int> ExplosiveWhitelist;
		internal static List<int> FishingWhitelist;
		internal static List<int> SeedWhitelist;

		internal static RecipeGroup YoYoStrings;

		internal static void AddRecipeGroups()
		{
			YoYoStrings = new RecipeGroup(() => "Any string", ItemID.RedString, ItemID.OrangeString, ItemID.YellowString, ItemID.LimeString, ItemID.GreenString, ItemID.TealString, ItemID.CyanString, ItemID.SkyBlueString, ItemID.BlueString, ItemID.PurpleString, ItemID.VioletString, ItemID.PinkString, ItemID.BrownString, ItemID.WhiteString, ItemID.RainbowString, ItemID.BlackString);
			RecipeGroup.RegisterGroup("PortableStorage:YoYoStrings", YoYoStrings);
		}

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
				ItemID.Hellstone,
				ItemID.Meteorite,

				ItemID.Amber,
				ItemID.Amethyst,
				ItemID.Diamond,
				ItemID.Emerald,
				ItemID.Ruby,
				ItemID.Sapphire,
				ItemID.Topaz
			};

			FishingWhitelist = new List<int>
			{
				ItemID.ArmoredCavefish,
				ItemID.AtlanticCod,
				ItemID.Bass,
				ItemID.BlueJellyfish,
				ItemID.ChaosFish,
				ItemID.CrimsonTigerfish,
				ItemID.Damselfish,
				ItemID.DoubleCod,
				ItemID.Ebonkoi,
				ItemID.FlarefinKoi,
				ItemID.FrostMinnow,
				ItemID.GoldenCarp,
				ItemID.GreenJellyfish,
				ItemID.Hemopiranha,
				ItemID.Honeyfin,
				ItemID.NeonTetra,
				ItemID.Obsidifish,
				ItemID.PinkJellyfish,
				ItemID.PrincessFish,
				ItemID.Prismite,
				ItemID.RedSnapper,
				ItemID.Salmon,
				ItemID.Shrimp,
				ItemID.SpecularFish,
				ItemID.Stinkfish,
				ItemID.Trout,
				ItemID.Tuna,
				ItemID.VariegatedLardfish,
				ItemID.AmanitaFungifin,
				ItemID.Angelfish,
				ItemID.Batfish,
				ItemID.BloodyManowar,
				ItemID.Bonefish,
				ItemID.BumblebeeTuna,
				ItemID.Bunnyfish,
				ItemID.CapnTunabeard,
				ItemID.Catfish,
				ItemID.Cloudfish,
				ItemID.Clownfish,
				ItemID.Cursedfish,
				ItemID.DemonicHellfish,
				ItemID.Derpfish,
				ItemID.Dirtfish,
				ItemID.DynamiteFish,
				ItemID.EaterofPlankton,
				ItemID.FallenStarfish,
				ItemID.TheFishofCthulu,
				ItemID.Fishotron,
				ItemID.Fishron,
				ItemID.GuideVoodooFish,
				ItemID.Harpyfish,
				ItemID.Hungerfish,
				ItemID.Ichorfish,
				ItemID.InfectedScabbardfish,
				ItemID.Jewelfish,
				ItemID.MirageFish,
				ItemID.Mudfish,
				ItemID.MutantFlinxfin,
				ItemID.Pengfish,
				ItemID.Pixiefish,
				ItemID.Slimefish,
				ItemID.Spiderfish,
				ItemID.TropicalBarracuda,
				ItemID.TundraTrout,
				ItemID.UnicornFish,
				ItemID.Wyverntail,
				ItemID.ZombieFish,
				ItemID.WoodenCrate,
				ItemID.IronCrate,
				ItemID.JungleFishingCrate,
				ItemID.FloatingIslandFishingCrate,
				ItemID.CorruptFishingCrate,
				ItemID.CrimsonFishingCrate,
				ItemID.HallowedFishingCrate,
				ItemID.DungeonFishingCrate,
				ItemID.GoldenCrate,
				ItemID.OldShoe,
				ItemID.FishingSeaweed,
				ItemID.TinCan
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
				ItemID.ShiverthornSeeds,

				ItemID.CorruptSeeds,
				ItemID.HallowedSeeds,
				ItemID.GrassSeeds,
				ItemID.JungleGrassSeeds,
				ItemID.MushroomGrassSeeds,
				ItemID.CrimsonSeeds,

				ItemID.Acorn,
				ItemID.PumpkinSeed
			};

			Ammos = new Dictionary<string, List<AmmoTypeGroup>>();

			void Add(string key, int ammoType)
			{
				if (Ammos.ContainsKey(key)) Ammos[key].Add(new AmmoTypeGroup(ammoType));
				else Ammos.Add(key, new List<AmmoTypeGroup> { new AmmoTypeGroup(ammoType) });
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

		// use ModPacket with a cache of all bags?
		internal static void SyncBag(BaseBag bag)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				bag.Item.FindOwner(bag.Item.whoAmI);

				Player player = Main.player[bag.Item.playerIndexTheItemIsReservedFor];

				int index = player.inventory.ToList().FindIndex(x => x == bag.Item);
				if (index < 0) return;

				NetMessage.SendData(MessageID.SyncEquipment, number: bag.Item.playerIndexTheItemIsReservedFor, number2: index);
			}
		}

		// public static class API
		// {
		// 	public enum WhitelistType
		// 	{
		// 		AlchemyIngredient,
		// 		Ore,
		// 		Explosive,
		// 		Fishing,
		// 		Seed
		// 	}
		//
		// 	public static void Register(WhitelistType type, int itemID)
		// 	{
		// 		switch (type)
		// 		{
		// 			case WhitelistType.AlchemyIngredient:
		// 				if (!AlchemistBagWhitelist.Contains(itemID)) AlchemistBagWhitelist.Add(itemID);
		// 				break;
		// 			case WhitelistType.Ore:
		// 				if (!OreWhitelist.Contains(itemID)) OreWhitelist.Add(itemID);
		// 				break;
		// 			case WhitelistType.Explosive:
		// 				if (!ExplosiveWhitelist.Contains(itemID)) ExplosiveWhitelist.Add(itemID);
		// 				break;
		// 			case WhitelistType.Fishing:
		// 				if (!FishingWhitelist.Contains(itemID)) FishingWhitelist.Add(itemID);
		// 				break;
		// 			case WhitelistType.Seed:
		// 				if (!SeedWhitelist.Contains(itemID)) SeedWhitelist.Add(itemID);
		// 				break;
		// 		}
		// 	}
		// }
	}
}