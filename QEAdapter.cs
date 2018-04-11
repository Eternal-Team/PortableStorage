using PortableStorage.TileEntities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TheOneLibrary.Utils;

namespace PortableStorage
{
	internal class QEAdapter
	{
		private Mod mod;

		public QEAdapter(Mod mod)
		{
			this.mod = mod;
		}

		public bool InjectItem(int x, int y, Item item)
		{
			int id = mod.GetID<TEQEChest>(x, y);
			if (id == -1) return false;

			TEQEChest qeChest = (TEQEChest)TileEntity.ByID[id];

			bool injectedPartial = false;

			List<Item> items = qeChest.GetItems();
			if (item.maxStack > 1)
			{
				for (int index = 0; index < items.Count; index++)
				{
					Item i = items[index];
					if (item.IsTheSameAs(i) && i.stack < i.maxStack)
					{
						int spaceLeft = i.maxStack - i.stack;
						if (spaceLeft >= item.stack)
						{
							i.stack += item.stack;
							item.stack = 0;
							HandleItemChange(qeChest.frequency, index);
							return true;
						}

						item.stack -= spaceLeft;
						i.stack = i.maxStack;
						HandleItemChange(qeChest.frequency, index);
						injectedPartial = true;
					}
				}
			}

			for (int index = 0; index < items.Count; index++)
			{
				var i = items[index];
				if (i.IsAir)
				{
					i.SetDefaults(item.type);
					i.prefix = item.prefix;
					i.stack = item.stack;
					item.stack = 0;
					HandleItemChange(qeChest.frequency, index);
					return true;
				}
			}

			return injectedPartial;
		}

		public IEnumerable<Tuple<Item, object>> EnumerateItems(int x, int y)
		{
			int id = mod.GetID<TEQEChest>(x, y);
			if (id == -1) yield break;

			TEQEChest qeChest = (TEQEChest)TileEntity.ByID[id];

			int slot = 0;
			foreach (var item in qeChest.GetItems())
			{
				yield return Tuple.Create(item, (object)slot++);
			}
		}

		public void TakeItem(int x, int y, object slot, int amount)
		{
			int id = mod.GetID<TEQEChest>(x, y);
			if (id == -1) return;

			TEQEChest qeChest = (TEQEChest)TileEntity.ByID[id];

			Item item = qeChest.GetItem((int)slot);
			item.stack -= amount;
			if (item.stack <= 0) item.TurnToAir();
			qeChest.SetItem((int)slot, item);
		}

		private void HandleItemChange(Frequency frequency, int slot) => Net.SendQEItem(frequency, slot);
	}
}