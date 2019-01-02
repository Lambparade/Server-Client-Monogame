using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lan_Tic_Tac_Toe.GameComponents;

namespace Lan_Tic_Tac_Toe.Graphics
{
   class RenderSystem
   {
      private static List<GameCell> CellsToDraw = new List<GameCell>();

      public static void AddCellToRenderQueue(GameCell CellToAdd)
      {
         CellsToDraw.Add(CellToAdd);
      }

      public void Render(SpriteBatch spriteBatch)
      {
         spriteBatch.Begin();

         foreach(GameCell Cell in CellsToDraw)
         {
            Cell.Draw(spriteBatch);
         }

         spriteBatch.End();
      }
   }
}
