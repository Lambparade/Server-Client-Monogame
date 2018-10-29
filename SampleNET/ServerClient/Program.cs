using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TCPServer;

namespace ServerClient
{
   class TcpDemo
   {
      static void Main()
      {
         SimpleServer Server = new SimpleServer();

         new Thread(Server.StartServer).Start();       // Run server method concurrently.

         Thread.Sleep(500);               // Give server time to start...//

         StartClientCommunication();

      }

      static void StartClientCommunication()
      {

         SimpleClient Client1 = new SimpleClient("Client1");
         Client1.StartClientConnection();

         // Register the event for clicking on the X to close the program
         //_handler += new EventHandler(Handler);
         //SetConsoleCtrlHandler(_handler, true);
         if (Client1.isClientConnected())
         {

            // Send name to server
            Client1.SendToServer($"{Client1.ClientID} has Connected!"); ;
            Console.WriteLine("------------------------------------------------");

            string aMessage = "";
            if (Client1.isClientConnected())
            {
               while (true)
               {
                  //aMessage = Console.ReadLine();
                  if ((aMessage = Console.ReadLine()) == "/quit")
                  {
                     Client1.SendToServer("/[sys]quit");
                     break;
                  }

                  if (string.IsNullOrEmpty(aMessage.Replace(" ", "")))
                     continue;

                  Client1.SendToServer(aMessage);
               }

               Client1.Disconnect();
            }
         }
         Console.WriteLine("Attempting to reconnect");
         Console.ReadKey();
      }
      static void Client()
      {
         using (TcpClient client = new TcpClient("192.168.16.87", 5000))

         using (NetworkStream n = client.GetStream())
         {
            BinaryWriter w = new BinaryWriter(n);
            w.Write("Message from Client 1<EOF>");
            w.Flush();
         }

         Console.WriteLine("Client 1 Dissconnected");
      }
      static void Client2()
      {
         using (TcpClient client = new TcpClient("192.168.16.87", 5000))

         using (NetworkStream n = client.GetStream())
         {
            BinaryWriter w = new BinaryWriter(n);
            w.Write("Message from Client 2<EOF>");
            w.Flush();
         }

         Console.WriteLine("Client 2 Disconnected");
      }
   }
}