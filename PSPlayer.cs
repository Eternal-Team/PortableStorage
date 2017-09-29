using BaseLib;
using BaseLib.Utility;
using Microsoft.Xna.Framework;
using PortableStorage.Items;
using PortableStorage.TileEntities;
using PortableStorage.UI;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage
{
	public class PSPlayer : ModPlayer
	{
		public override void PreUpdate()
		{
			int ID = mod.GetID<TEQEChest>(BaseLib.Utility.Utility.MouseToWorldPoint());
			if (ID == -1) return;

			TEQEChest qeChest = (TEQEChest)TileEntity.ByID[ID];

			if (qeChest != null)
			{
				int i = qeChest.Position.X * 16;
				int j = qeChest.Position.Y * 16;

				Rectangle left = new Rectangle(i + 2, j + 4, 6, 10);
				Rectangle middle = new Rectangle(i + 12, j + 4, 8, 10);
				Rectangle right = new Rectangle(i + 24, j + 4, 6, 10);

				if (Main.mouseRight && Main.mouseRightRelease)
				{
					if (BaseLib.Utility.Utility.HeldItem.type != mod.ItemType<QEBag>())
					{
						Frequency frequency = qeChest.frequency;
						bool handleFrequency = false;

						if (left.Contains(Main.MouseWorld) && qeChest.animState == 0)
						{
							frequency.colorLeft = Utility.ColorFromItem(frequency.colorLeft);
							handleFrequency = true;
						}
						else if (middle.Contains(Main.MouseWorld) && qeChest.animState == 0)
						{
							frequency.colorMiddle = Utility.ColorFromItem(frequency.colorMiddle);
							handleFrequency = true;
						}
						else if (right.Contains(Main.MouseWorld) && qeChest.animState == 0)
						{
							frequency.colorRight = Utility.ColorFromItem(frequency.colorRight);
							handleFrequency = true;
						}
						else
						{
							qeChest.opened = !qeChest.opened;
							PortableStorage.Instance.HandleUI<QEChestUI>(ID);

							Main.PlaySound(SoundID.DD2_EtherianPortalOpen.WithVolume(0.5f));
						}
						if (handleFrequency)
						{
							if (!mod.GetModWorld<PSWorld>().enderItems.ContainsKey(frequency))
							{
								List<Item> items = new List<Item>();
								for (int x = 0; x < 27; x++) items.Add(new Item());
								mod.GetModWorld<PSWorld>().enderItems[frequency] = items;
							}
							qeChest.frequency = frequency;
						}

						qeChest.SendUpdate();
					}
					else
					{
						player.noThrow = 2;
						((QEBag)BaseLib.Utility.Utility.HeldItem.modItem).frequency = qeChest.frequency;
					}
				}
			}
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (PortableStorage.bagKey.JustPressed)
			{
				Item item = BaseLib.Utility.Utility.AccessoryItems.FirstOrDefault(x => x.modItem is BaseBag);

				if (item?.modItem is BaseBag)
				{
					BaseBag bag = (BaseBag)item.modItem;
					bag.HandleUI();
				}
			}
		}
	}
}