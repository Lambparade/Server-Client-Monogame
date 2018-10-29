using System;
using System.Collections.Generic;
using System.Text;

namespace ServerClient
{
   class CommandHandler
   {
      public CommandHandler()
      {

      }

      public void ParseCommand(string Command)
      {
         string DataToReceive = Command.Replace("<EOF>", "").Replace("Sent", "").Replace("%", "").ToString();

         Console.ForegroundColor = ConsoleColor.Cyan;

         Console.WriteLine(DataToReceive);

         Console.ForegroundColor = ConsoleColor.Gray;
      }
   }
}
