using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lan_Tic_Tac_Toe.GameComponents
{
   class CellHitbox
   {
      private Rectangle Hitbox;

      public CellHitbox(Vector2 Position, int Size)
      {
         Hitbox = new Rectangle((int)Position.X, (int)Position.Y, Size, Size);
      }

      public bool WasCellClicked(Rectangle HitboxToCheck)
      {
         bool ret = false;

         if (Hitbox.Contains(HitboxToCheck))
         {
            ret = true;
         }

         return ret;
      }
   }
}
