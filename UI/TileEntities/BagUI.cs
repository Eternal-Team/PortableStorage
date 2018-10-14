using System;
using PortableStorage.TileEntities;
using Terraria;
using TheOneLibrary.Base.UI;

namespace PortableStorage.UI.TileEntities
{
	public class TEUI : BaseUI
	{
		public override void OnInitialize()
		{
		}

		public void HandleTE(BasePSTE te)
		{
			if (te.UI != null) CloseTE(te);
			else OpenTE(te);
		}

		public void CloseTE(BasePSTE te)
		{
			te.UIPosition = te.UI.Position;
			Elements.Remove(te.UI);
			Main.PlaySound(te.CloseSound);
		}

		public void OpenTE(BasePSTE te)
		{
			BaseTEPanel teUI = (BaseTEPanel)Activator.CreateInstance(te.UIType);
			teUI.te = te;
			teUI.Activate();
			if (te.UIPosition != null)
			{
				teUI.HAlign = teUI.VAlign = 0f;
				teUI.Position = te.UIPosition.Value;
			}

			Append(teUI);
			Main.PlaySound(te.OpenSound);
		}
	}
}