using System.IO;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;

namespace PortableStorage.Items.Bags
{
	public class ThePerfectSolution : BaseAmmoBag
	{
		public override string Texture => "PortableStorage/Textures/Items/ThePerfectSolution";

		[PathOverride("PortableStorage/Textures/Items/ThePerfectSolution_Infill")]
		public static Texture2D Texture_Infill { get; set; }

		public override string AmmoType => "Solution";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Perfect Solution");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of solutions");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 18;
			item.height = 32;
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			spriteBatch.Draw(Texture_Infill, position + new Vector2(9, 23) * scale, null, Main.DiscoColor, 0f, Texture_Infill.Size() * 0.5f, scale, SpriteEffects.None, 0f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 position = item.position - Main.screenPosition;
			Vector2 origin = new Vector2(9, 18);
			spriteBatch.Draw(Texture_Infill, position + origin, null, Main.DiscoColor, rotation, origin - new Vector2(4, 18), scale, SpriteEffects.None, 0f);
		}

		public override TagCompound Save() => new TagCompound
		{
			["Items"] = Handler.Save()
		};

		public override void Load(TagCompound tag)
		{
			Handler.Load(tag.GetCompound("Items"));
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));
	}
}