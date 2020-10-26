using Terraria;

namespace PortableStorage
{
	public interface IItemHandler
	{
		bool InsertItem(int slot, ref Item item, bool user);

		bool ExtractItem(int slot, out Item item, bool user);

		ItemHandler GetItemHandler();
	}
}