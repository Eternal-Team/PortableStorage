using System;
using System.IO;
using BaseLibrary.Utility;
using PortableStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage;

public class PortableStorage : Mod
{
	public const string AssetPath = "PortableStorage/Assets/";

	public static PortableStorage Instance => ModContent.GetInstance<PortableStorage>();

	public override void Load()
	{
		Hooking.Hooking.Load();
		BagPopupText.Load();
	}

	public override void PostSetupContent()
	{
		Utility.PostSetupContent();
	}

	public override void HandlePacket(BinaryReader reader, int whoAmI)
	{
		PacketID msgType = (PacketID)reader.ReadByte();

		if (msgType == PacketID.SyncBag)
		{
			var playernumber = reader.ReadByte();

			Guid id = reader.ReadGuid();

			if (!ModContent.GetInstance<BagSyncSystem>().AllBags.ContainsKey(id)) return;
			
			BaseBag bag = ModContent.GetInstance<BagSyncSystem>().AllBags[id];
			var itemStorage = bag.GetItemStorage();
			itemStorage.Read(reader);

			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = GetPacket();
				packet.Write((byte)PacketID.SyncBag);
				packet.Write(playernumber);
				packet.Write(id);
				itemStorage.Write(packet);
				packet.Send(-1, playernumber);
			}
		}
	}
}

public class PortableStorageSystem : ModSystem
{
	public override void AddRecipeGroups()
	{
		Utility.AddRecipeGroups();
	}
}