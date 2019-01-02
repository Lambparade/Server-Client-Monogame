using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lan_Tic_Tac_Toe
{
   static class Pointer
   {
      public static Vector2 MousePosition { get; set; }

      public static bool MouseDown { get; set; } = false;

      private static MouseState OldMouseState { get; set; }

      public static void Update()
      {
         MouseState CurrentMouseState = Mouse.GetState();

         MousePosition = new Vector2(CurrentMouseState.X, CurrentMouseState.Y);

         if (CurrentMouseState.LeftButton == ButtonState.Pressed)
         {
            MouseDown = true;
         }

         if (OldMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Released)
         {
            MouseDown = false;
         }

         OldMouseState = CurrentMouseState;
      }

      public static Rectangle GetPointerHitbox()
      {
         return new Rectangle((int)MousePosition.X, (int)MousePosition.Y, 1, 1);
      }
   }
}
