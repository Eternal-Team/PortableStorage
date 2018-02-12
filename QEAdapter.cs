using PortableStorage.TileEntities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TheOneLibrary.Utility;

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

			if (item.maxStack > 1)
			{
				foreach (var i in qeChest.GetItems())
				{
					if (item.IsTheSameAs(i) && i.stack < i.maxStack)
					{
						int spaceLeft = i.maxStack - i.stack;
						if (spaceLeft >= item.stack)
						{
							i.stack += item.stack;
							item.stack = 0;
							HandleItemChange();
							return true;
						}

						item.stack -= spaceLeft;
						i.stack = i.maxStack;
						HandleItemChange();
						injectedPartial = true;
					}
				}
			}

			foreach (var i in qeChest.GetItems())
			{
				if (i.IsAir)
				{
					i.SetDefaults(item.type);
					i.prefix = item.prefix;
					i.stack = item.stack;
					item.stack = 0;
					HandleItemChange();
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
			qeChest.SetItem((int)slot, item);
		}

		private void HandleItemChange()
		{
			Net.SyncQEItems();
		}
	}
}