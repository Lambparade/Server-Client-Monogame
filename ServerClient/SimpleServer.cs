using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPServer
{

   class SimpleServer
   {
      public static ManualResetEvent allDone = new ManualResetEvent(false);

      List<ServerClient> ClientList = new List<ServerClient>();

      IPAddress ServerIp = IPAddress.Parse("192.168.16.87");

      int Port = 5000;

      Socket ServerSocket;

      public SimpleServer()
      {

      }

      public void StartServer()
      {
         IPEndPoint ServerEndPoint = new IPEndPoint(ServerIp, Port);

         ServerSocket = new Socket(ServerIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

         ServerSocket.Bind(ServerEndPoint);

         ServerSocket.Listen(100);

         Console.WriteLine($"Server has Started listening on {ServerIp} on port:{Port}");

         while (true)
         {
            allDone.Reset();

            Console.WriteLine("Server is waiting for connections");

            ServerSocket.BeginAccept(
                 new AsyncCallback(AcceptCallback),
                 ServerSocket);

            allDone.WaitOne();

            Console.WriteLine("A connection has been made...");
         }
      }

      public void AcceptCallback(IAsyncResult ar)
      {
         // Signal the main thread to continue.  
         allDone.Set();

         // Get the socket that handles the client request.  
         Socket listener = (Socket)ar.AsyncState;
         Socket FoundClient = listener.EndAccept(ar);

         ServerClient ConnectedClient = new ServerClient();

         ConnectedClient.ClientSocket = FoundClient;
         if (ClientList.Count < 4)
         {
            ClientList.Add(ConnectedClient);
            // Create the state object.  
            FoundClient.BeginReceive(ConnectedClient.Buffer, 0, ServerClient.BufferSize, 0,
                new AsyncCallback(ReadCallback), ConnectedClient);
         }
      }

      public void ReadCallback(IAsyncResult ar)
      {
         String content = String.Empty;

         // Retrieve the state object and the handler socket  
         // from the asynchronous state object.  
         ServerClient ConnectedClient = (ServerClient)ar.AsyncState;
         Socket handler = ConnectedClient.ClientSocket;

         // Read data from the client socket.   
         int bytesRead = handler.EndReceive(ar);

         if (bytesRead > 0)
         {
            // There  might be more data, so store the data received so far.  
            ConnectedClient.StringData.Append(Encoding.ASCII.GetString(
                ConnectedClient.Buffer, 0, bytesRead));

            // Check for end-of-file tag. If it is not there, read   
            // more data.  
            content = ConnectedClient.StringData.ToString();
            if (content.IndexOf("<EOF>") > -1)
            {
               // All the data has been read from the   
               // client. Display it on the console.  
               Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                   content.Length, content);
               // Echo the data back to the client.  
               Send(handler, content);
            }
            else
            {
               // Not all data received. Get more.  
               handler.BeginReceive(ConnectedClient.Buffer, 0, ServerClient.BufferSize, 0,
               new AsyncCallback(ReadCallback), ConnectedClient);
            }
         }
      }

      private void Send(Socket ClientSocket, String data)
      {
         // Convert the string data to byte data using ASCII encoding.  
         byte[] byteData = Encoding.ASCII.GetBytes(data);

         // Begin sending the data to the remote device.  
         ClientSocket.BeginSend(byteData, 0, byteData.Length, 0,
             new AsyncCallback(SendCallback), ClientSocket);
      }

      private void SendCallback(IAsyncResult ar)
      {
         try
         {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            //handler.Shutdown(SocketShutdown.Both);
            //handler.Close();
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }
   }

   class ServerClient
   {
      public string ClientName;

      public Socket ClientSocket = null;

      public const int BufferSize = 1024;

      public byte[] Buffer = new byte[BufferSize];

      public StringBuilder StringData = new StringBuilder();
   }
}
