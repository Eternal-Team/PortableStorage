using BaseLibrary.UI;
using Microsoft.Xna.Framework;

namespace PortableStorage.Items;

public class BagUI : UIPanel
{
	public static BagUI Instance = null!;

	private Bag bag;

	private UIText text;
	private UIGrid<UIStorageSlot> gridItems;

	public BagUI()
	{
		Instance = this;

		Size = Dimension.FromPixels(516, 300);
		Position = Dimension.FromPercent(50);
		Display = Display.None;

		text = new UIText("GUID") {
			Size = new Dimension(0, 20, 100, 0),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				TextColor = Color.White,
				BorderColor = Color.Black
			}
		};
		base.Add(text);

		gridItems = new UIGrid<UIStorageSlot>(9) {
			Size = new Dimension(0, -28, 100, 100),
			Position = Dimension.FromPixels(0, 28),
			Settings = {
				ItemMargin = 4
			}
		};
		base.Add(gridItems);
	}

	public void SetBag(Bag bag)
	{
		this.bag = bag;
		text.Text = bag.ID.ToString();

		gridItems.Clear();

		for (int i = 0; i < bag.Storage.Count; i++)
		{
			gridItems.Add(new UIStorageSlot(bag.Storage, i) {
				Size = Dimension.FromPixels(52)
			});
		}
	}
}