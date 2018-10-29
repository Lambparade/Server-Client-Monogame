using System;
using System.Collections.Generic;
using System.Text;

namespace ParserSystem
{

   class CommandParserSystem
   {
      public CommandParserSystem()
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
