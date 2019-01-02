using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lan_Tic_Tac_Toe.GameComponents
{
   class Player
   {
      public enum PlayerTypes
      {
        Spectator,
        PlayerX,
        PlayerO,
      }

      public PlayerTypes CurrentPlayerType;

      public Player(PlayerTypes TypeOfPlayer)
      {
         CurrentPlayerType = TypeOfPlayer;
      }
   }
}
