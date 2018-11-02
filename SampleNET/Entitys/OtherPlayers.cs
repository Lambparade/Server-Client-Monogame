using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleNET
{
   class OtherPlayer
   {
      Texture2D Graphic;

      public int X;

      public int Y;

      public string PlayerName;

      public OtherPlayer(Texture2D Sprite, Vector2 StartPosition, string Name)
      {
         Graphic = Sprite;

         X = (int)StartPosition.X;
         Y = (int)StartPosition.Y;

         PlayerName = Name;
      }

      public void Update(GameTime gameTime, string Command)
      {
         ParseCommand(Command);
      }

      public void Draw(SpriteBatch spriteBatch)
      {
         spriteBatch.Draw(Graphic, new Vector2(X, Y), Color.White);
      }

      public void ParseCommand(string Command)
      {

      }
   }
}
