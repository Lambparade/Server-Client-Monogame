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
   class GameBoard
   {
      GameCell[,] CurrentGameBoard = new GameCell[3, 3];

      public GameBoard()
      {
         CreateGameBoard();
      }

      public void GameBoardUpdate(Player CurrentPlayer)
      {
         if (Pointer.MouseDown)
         {
            foreach (GameCell cell in CurrentGameBoard)
            {
               if (cell.Hitbox.WasCellClicked(Pointer.GetPointerHitbox()))
               {
                  cell.ChangeCellState(CurrentPlayer);
               }
            }
         }
      }

      private void CreateGameBoard()
      {
         for (int x = 0; x < 3; x++)
         {
            for (int y = 0; y < 3; y++)
            {
               CurrentGameBoard[x, y] = new GameCell(new Vector2(32 * x, 32 + (32 * y)));
            }
         }
      }
   }
}
