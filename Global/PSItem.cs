using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace PortableStorage.Global
{
	public class PSItem : GlobalItem
	{
		public override bool InstancePerEntity => true;
		public override bool CloneNewInstances => true;

		#region Constant fields
		private const float angleDecrement = 0.06981317f;
		private const float scaleDecrement = 0.015f;
		private static readonly Vector2 origin = new Vector2(30);
		#endregion

		#region Instance fields
		public bool markedForSuction;
		public float angle;
		public float scale = 1f;
		#endregion

		public override bool OnPickup(Item item, Player player)
		{
			markedForSuction = false;
			scale = 1f;
			angle = 0f;

			return base.OnPickup(item, player);
		}

		public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			if (markedForSuction)
			{
				markedForSuction = false;

				if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;
				if (this.scale > 0f) this.scale -= scaleDecrement;

				scale *= this.scale;
				spriteBatch.Draw(Main.extraTexture[50], Main.itemAnimationsRegistered.Contains(item.type) ? new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f / Main.itemAnimations[item.type].FrameCount + 2f) : new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f + 2f), null, Color.White, angle + rotation, origin, this.scale, SpriteEffects.None, 0f);
			}

			#region old
			//if (item.IsAir || item.modItem is TheBlackHole || !Main.player.Any(x => x.HasItem(mod.ItemType<TheBlackHole>()) && Vector2.Distance(x.position, item.Center) < 160))
			//{
			// this.scale = 1f;
			// return true;
			//}

			//if (this.scale > 0f) this.scale -= 0.015f;
			//else
			//{
			// this.scale = 1f;

			// Player player =Main.player.FirstOrDefault(x => x.HasItem(mod.ItemType<TheBlackHole>()) && Vector2.Distance(x.position, item.Center) < 160);
			//    if (player?.inventory.FirstOrDefault(x => x.modItem is TheBlackHole)?.modItem is TheBlackHole hole)
			//    {
			//        for (int i = 0; i < hole.handler.Slots; i++)
			//        {
			//            item = hole.handler.InsertItem(i, item);
			//            if (item.IsAir) return true;
			//        }
			//    }
			//}
			///*   else
			//   {
			//       int j = Main.item.ToList().FindIndex(x => x == item);
			//       if (j == -1) return true;

			//       if ((Main.LocalPlayer.HeldItem.type != 0 || Main.LocalPlayer.itemAnimation <= 0))
			//       {
			//           if (!ItemLoader.OnPickup(item, Main.LocalPlayer))
			//           {
			//               item = new Item();
			//               if (Main.netMode == 1)
			//               {
			//                   NetMessage.SendData(MessageID.SyncItem, -1, -1, null, j);
			//               }
			//           }
			//           if (ItemID.Sets.NebulaPickup[Main.item[j].type])
			//           {
			//               Main.PlaySound(7, (int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y);
			//               int num2 = Main.item[j].buffType;
			//               Main.item[j] = new Item();
			//               if (Main.netMode == 1)
			//               {
			//                   NetMessage.SendData(102, -1, -1, null, Main.myPlayer, num2, Main.LocalPlayer.Center.X, Main.LocalPlayer.Center.Y);
			//                   NetMessage.SendData(21, -1, -1, null, j);
			//               }
			//               else
			//               {
			//                   Main.LocalPlayer.NebulaLevelup(num2);
			//               }
			//           }
			//           if (Main.item[j].type == 58 || Main.item[j].type == 1734 || Main.item[j].type == 1867)
			//           {
			//               Main.PlaySound(7, (int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y);
			//               Main.LocalPlayer.statLife += 20;
			//               if (Main.myPlayer == Main.LocalPlayer.whoAmI)
			//               {
			//                   Main.LocalPlayer.HealEffect(20, true);
			//               }
			//               if (Main.LocalPlayer.statLife > Main.LocalPlayer.statLifeMax2)
			//               {
			//                   Main.LocalPlayer.statLife = Main.LocalPlayer.statLifeMax2;
			//               }
			//               Main.item[j] = new Item();
			//               if (Main.netMode == 1)
			//               {
			//                   NetMessage.SendData(21, -1, -1, null, j);
			//               }
			//           }
			//           else if (Main.item[j].type == 184 || Main.item[j].type == 1735 || Main.item[j].type == 1868)
			//           {
			//               Main.PlaySound(7, (int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y);
			//               Main.LocalPlayer.statMana += 100;
			//               if (Main.myPlayer == Main.LocalPlayer.whoAmI)
			//               {
			//                   Main.LocalPlayer.ManaEffect(100);
			//               }
			//               if (Main.LocalPlayer.statMana > Main.LocalPlayer.statManaMax2)
			//               {
			//                   Main.LocalPlayer.statMana = Main.LocalPlayer.statManaMax2;
			//               }
			//               Main.item[j] = new Item();
			//               if (Main.netMode == 1)
			//               {
			//                   NetMessage.SendData(21, -1, -1, null, j);
			//               }
			//           }
			//           else
			//           {
			//               Main.item[j] = Main.LocalPlayer.GetItem(Main.myPlayer, Main.item[j]);
			//               if (Main.netMode == 1)
			//               {
			//                   NetMessage.SendData(21, -1, -1, null, j);
			//               }
			//           }
			//       }
			//   }*/
			//scale *= this.scale;

			//if ((angle -= angleDecrement) < 0f) angle = MathHelper.TwoPi;
			//spriteBatch.Draw(Main.extraTexture[50], new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - Main.itemTexture[item.type].Height * 0.5f + 2f), null, Color.White, angle + rotation, origin,this.scale, SpriteEffects.None, 0f);
			#endregion

			return true;
		}
	}
}