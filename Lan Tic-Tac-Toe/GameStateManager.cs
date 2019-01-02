using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lan_Tic_Tac_Toe
{
   class GameStateManager
   {
      public enum GameStates
      {
         WaitingForTurn,
         TurnInProgress,
         FindingAnOpponent,
         WaitingForOpponent,
         Victory,
      }

      public static GameStates GameState { get; private set; }

      public void ChangeGameState(GameStates SetGameState)
      {
         GameState = SetGameState;
      }
   }
}
