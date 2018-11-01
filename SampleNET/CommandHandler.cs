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

      enum Commands
      {
         EOF,
         MOVE,
         ACK,
         FD,
      }
   
      static public void ParseCommand(string Command)
      {
         Console.ForegroundColor = ConsoleColor.Cyan;

         Console.WriteLine(Command);

         CurrentCommand = Command;

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
