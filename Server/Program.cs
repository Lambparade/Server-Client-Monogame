using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Network.System;

namespace Server
{
   class Program
   {
      public static int Main(String[] args)
      {
         ImprovedServer Server = new ImprovedServer();

         new Thread(Server.StartServer).Start();

         return 0;
      }
   }
}