using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using Terraria;
using Terraria.ID;

namespace PortableStorage.Global
{
	public static class Utility
	{
		public static Dictionary<string, MultiValueDictionary<int, int>> Ammos { get; internal set; } = new Dictionary<string, MultiValueDictionary<int, int>>();

		public static List<short> AlchemistBagWhitelist { get; internal set; } = new List<short>
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

		/*
		["Arrow"] = itemCache.Where(item => item.ammo == AmmoID.Arrow).Select(item => item.type).ToList(),
		["Dart"] = itemCache.Where(item => item.ammo == AmmoID.Dart).Select(item => item.type).ToList(),
		["Bullet"] = itemCache.Where(item => item.ammo == AmmoID.Bullet).Select(x => x.type).ToList(),
		["Solution"] = itemCache.Where(item => item.ammo == AmmoID.Solution).Select(item => item.type).ToList(),
		["Coin"] = itemCache.Where(item => item.ammo == AmmoID.Coin).Select(item => item.type).ToList(),
		["All"] = itemCache.Where(item => item.ammo > 0).Select(item => item.type).ToList()
		 */

		internal static void Load()
		{
			void Add(string key, int ammoType)
			{
				BaseLibrary.Utility.Cache.ItemCache.Where(item => item.ammo == ammoType).Select(item => item.type).ForEach(itemType =>
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

			Add("Flameable", AmmoID.Rocket);
			Add("Flameable", AmmoID.Gel);
			Add("Flameable", AmmoID.Flare);
			Add("Flameable", AmmoID.StyngerBolt);
			Add("Flameable", AmmoID.JackOLantern);
		}

		public static Colors ColorFromItem(this Item item, Colors existing)
		{
			Main.LocalPlayer.noThrow = 2;
			switch (item.type)
			{
				case ItemID.Diamond: return Colors.White;
				case ItemID.Ruby: return Colors.Red;
				case ItemID.Emerald: return Colors.Green;
				case ItemID.Topaz: return Colors.Yellow;
				case ItemID.Amethyst: return Colors.Purple;
				case ItemID.Sapphire: return Colors.Blue;
				case ItemID.Amber: return Colors.Orange;
				default: return existing;
			}
		}
	}
}