using System;
using System.Collections.Generic;
using System.Text;

namespace Server.ServerUtils
{
   class ServerLogger
   {
      ConsoleColor DefaultColor = ConsoleColor.Gray;

      public enum LogColor
      {
         Warning,
         Error,
         Success,
         Debug,
         Default,
      }

      public void Log(string StringToLog, LogColor Color)
      {
         SetLogColor(Color);

         Console.WriteLine(StringToLog);

         ResetConsoleColor();
      }

      public void Log(string StringToLog)
      {
         Console.WriteLine(StringToLog);
      }

      private void SetLogColor(LogColor Color)
      {
         switch (Color)
         {
            case LogColor.Warning:
               Console.ForegroundColor = ConsoleColor.Yellow;
               break;
            case LogColor.Error:
               Console.ForegroundColor = ConsoleColor.Red;
               break;
            case LogColor.Success:
               Console.ForegroundColor = ConsoleColor.Green;
               break;
            case LogColor.Debug:
               Console.ForegroundColor = ConsoleColor.Blue;
               break;
            case LogColor.Default:
               Console.ForegroundColor = ConsoleColor.Gray;
               break;
            default:
               break;
         }
      }

      private void ResetConsoleColor()
      {
         Console.ForegroundColor = DefaultColor;
      }
   }
}
