using System;
using System.Linq;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PortableStorage.Items;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.UI;

namespace PortableStorage.UI;

public class BuilderReservePanel : BaseBagPanel<BuilderReserve>
{
	private UIGrid<UIContainerSlot> gridItems;

	public BuilderReservePanel(BuilderReserve bag) : base(bag)
	{
		Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
		Height.Pixels = 40 + (SlotSize + SlotMargin) * Container.GetItemStorage().Count / 9;

		gridItems = new UIGrid<UIContainerSlot>(9)
		{
			Width = { Percent = 100 },
			Height = { Pixels = -28, Percent = 100 },
			Y = { Pixels = 28 },
			Settings = { ItemMargin = SlotMargin }
		};
		Add(gridItems);

		for (int i = 0; i < Container.GetItemStorage().Count; i++)
		{
			UISelectableBagSlot slot = new UISelectableBagSlot(Container, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridItems.Add(slot);
		}
	}

	protected override void Activate()
	{
		gridItems.Clear();

		ItemStorage storage = BagSyncSystem.Instance.AllBags[Container.GetID()].GetItemStorage();
		for (int i = 0; i < storage.Count; i++)
		{
			UISelectableBagSlot slot = new UISelectableBagSlot(Container, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridItems.Add(slot);
		}
	}
}

public class GardenerSatchelPanel : BaseBagPanel<GardenerSatchel>
{
	private UIGrid<UIContainerSlot> gridItems;

	public GardenerSatchelPanel(GardenerSatchel bag) : base(bag)
	{
		Width.Pixels = 12 + (SlotSize + SlotMargin) * 9;
		Height.Pixels = 40 + (SlotSize + SlotMargin) * Container.GetItemStorage().Count / 9;

		gridItems = new UIGrid<UIContainerSlot>(9)
		{
			Width = { Percent = 100 },
			Height = { Pixels = -28, Percent = 100 },
			Y = { Pixels = 28 },
			Settings = { ItemMargin = SlotMargin }
		};
		Add(gridItems);

		for (int i = 0; i < Container.GetItemStorage().Count; i++)
		{
			UISelectableBagSlot slot = new UISelectableBagSlot(Container, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridItems.Add(slot);
		}
	}

	protected override void Activate()
	{
		gridItems.Clear();

		ItemStorage storage = BagSyncSystem.Instance.AllBags[Container.GetID()].GetItemStorage();
		for (int i = 0; i < storage.Count; i++)
		{
			UISelectableBagSlot slot = new UISelectableBagSlot(Container, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			gridItems.Add(slot);
		}
	}
}

internal class UISelectableBagSlot : UIContainerSlot
{
	private BaseSelectableBag bag;

	private new ItemStorage storage => bag.GetItemStorage();
	public new Item Item => storage[slot];

	public UISelectableBagSlot(BaseSelectableBag bag, int slot) : base(bag.GetItemStorage(), slot)
	{
		Width.Pixels = 44;
		Height.Pixels = 44;

		this.bag = bag;
	}

	protected override void MouseDown(MouseButtonEventArgs args)
	{
		if (args.Button != MouseButton.Left) return;

		args.Handled = true;

		UISelectableBagSlot otherSlot = (UISelectableBagSlot)Parent.Children.FirstOrDefault(x => x is UISelectableBagSlot s && s.Item.type == Main.mouseItem.type);
		if (otherSlot != null && otherSlot != this && !otherSlot.Item.IsAir)
		{
			otherSlot.MouseDown(args);
			return;
		}

		if (Main.keyState.IsKeyDown(Keys.LeftAlt) && !Item.IsAir)
		{
			if (bag.SelectedIndex == slot) bag.SelectedIndex = -1;
			else bag.SelectedIndex = slot;

			BagSyncSystem.Instance.Sync(bag.GetID(), PacketID.SelectedIndex);
		}
		else
		{
			if (storage.IsItemValid(slot, Main.mouseItem) || Main.mouseItem.IsAir)
			{
				Item.newAndShiny = false;
				Player player = Main.LocalPlayer;

				if (ItemSlot.ShiftInUse)
				{
					Main.LocalPlayer.Loot(storage, slot);
					return;
				}

				if (Main.mouseItem.IsAir) storage.RemoveItem(Main.LocalPlayer, slot, out Main.mouseItem, Item.maxStack);
				else
				{
					if (Item.type == Main.mouseItem.type) storage.InsertItem(Main.LocalPlayer, slot, ref Main.mouseItem);
					else
					{
						if (Item.stack <= Item.maxStack)
						{
							storage.SwapStacks(Main.LocalPlayer, slot, ref Main.mouseItem);
						}
					}
				}

				if (Item.stack > 0) AchievementsHelper.NotifyItemPickup(player, Item);

				if (Main.mouseItem.type > ItemID.None || Item.type > ItemID.None)
				{
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.Grab);
				}
			}
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		var texture = bag.SelectedIndex == slot ? TextureAssets.InventoryBack15.Value : Settings.SlotTexture;
		DrawingUtility.DrawSlot(spriteBatch, Dimensions, texture, Color.White);

		float scale = Math.Min(InnerDimensions.Width / (float)texture.Width, InnerDimensions.Height / (float)texture.Height);

		if (!Item.IsAir) DrawItem(spriteBatch, Item, scale);
	}
}