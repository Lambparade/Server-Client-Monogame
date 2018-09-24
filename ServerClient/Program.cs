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
         
         Thread.Sleep(500);               // Give server time to start.

         SimpleClient Client1 = new SimpleClient("Client1");

         Client1.StartClientConnection();
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