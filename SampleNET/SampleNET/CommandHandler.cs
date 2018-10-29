using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleNet;

namespace SimpleNet
{
   static class CommandHandler
   {
      static public string CurrentCommand { get; set; }
   
      static public void ParseCommand(string Command)
      {
         string DataToReceive = Command.Replace("<EOF>", "").Replace("Sent", "").Replace("%", "").ToString();

         Console.ForegroundColor = ConsoleColor.Cyan;

         Console.WriteLine(DataToReceive);

         CurrentCommand = DataToReceive;

         Console.ForegroundColor = ConsoleColor.Gray;
      }

      static public string GetCommand()
      {
         string CommandToReturn = string.Empty;

         CommandToReturn = CurrentCommand;

         CurrentCommand = string.Empty;

         return CommandToReturn;
      }
   }
}
