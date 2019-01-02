using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lan_Tic_Tac_Toe.GameSprites;
using Lan_Tic_Tac_Toe.Graphics;

namespace Lan_Tic_Tac_Toe.GameComponents
{
   class GameCell
   {
      public Vector2 Position;

      public CellHitbox Hitbox;

      private Texture2D CellSprite;

      public enum CellState
      {
         Blank,
         X,
         O
      }

      public CellState CurrentCellState;

      public GameCell(Vector2 BoardPosition)
      {
         CurrentCellState = CellState.Blank;

         Position = BoardPosition;

         Hitbox = new CellHitbox(Position,32);

         CellSprite = Sprites.BlackCellSprite;

         RenderSystem.AddCellToRenderQueue(this);
      }

      public void ChangeCellState(Player CurrentPlayer)
      {
         if (CurrentCellState == CellState.Blank)
         {
            if (CurrentPlayer.CurrentPlayerType == Player.PlayerTypes.PlayerO)
            {
               CurrentCellState = CellState.O;
               CellSprite = Sprites.OCellSprite;
            }
            else if (CurrentPlayer.CurrentPlayerType == Player.PlayerTypes.PlayerX)
            {
               CurrentCellState = CellState.X;
               CellSprite = Sprites.XCellSprite;
            }
         }
      }

      public void Draw(SpriteBatch spriteBatch)
      {
         spriteBatch.Draw(CellSprite, Position, Color.White);
      }
   }
}
