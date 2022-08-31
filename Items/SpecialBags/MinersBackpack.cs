// modded ores courtesy of Vein Miner (will be different in 1.4, use as a reference)
/*
ThoriumMod,SmoothCoal,ThoriumOre,LifeQuartz,MagmaOre,Aquaite,LodeStone,ValadiumChunk,IllumiteChunk,SynthGold,SynthPlatinum,Opal,PearlStone,Onyx,PlacedGem
SpiritMod,FloranOreTile,MarbleOre,GraniteOre,CryoliteOreTile,SpiritOreTile,ThermiteOre
CalamityMod,AerialiteOre,CryonicOre,PerennialOre,CharredOre,AstralOre,ChaoticOre,UelibloomOre,AuricOre,SeaPrism
Tremor,IceOre,AngeliteOreTile,ArgiteOre,CollapsiumOreTile,CometiteOreTile,FrostoneOreTile,HardCometiteOreTile,NightmareOreTile
CheezeMod,StarlightOre
Antiaris,EnchantedStone
SacredTools,LapisOre,OblivionOreBlock,Flarium,BismuthOre,VenomiteOre
Bluemagic,PuriumOre
CrystiliumMod,RadiantOre
AAMod,AbyssiumOre,IncineriteOre,EventideAbyssiumOre,DaybreakIncineriteOre,LuminiteOre,Apocalyptite,DynaskullOre,YtriumOre,UraniumOre,TechneciumOre,RadiumOre,PrismOre,HallowedOre,DoomitePlate
UniverseOfSwordsMod,DamascusOreTile
*/

using Terraria;
using Terraria.ID;

namespace PortableStorage.Items;

public class MinersBackpack : BaseBag
{
	private class MinersBackpackItemStorage : BagStorage
	{
		public MinersBackpackItemStorage(int slots, BaseBag bag) : base(bag, slots)
		{
		}

		public override bool IsItemValid(int slot, Item item)
		{
			return Utility.OreWhitelist.Contains(item.type) || Utility.ExplosiveWhitelist.Contains(item.type);
		}
	}

	public override string Texture => PortableStorage.AssetPath + "Textures/Items/MinersBackpack";

	public MinersBackpack()
	{
		Storage = new MinersBackpackItemStorage(18, this);
	}

	public override void SetDefaults()
	{
		base.SetDefaults();

		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(gold: 1);
	}
}