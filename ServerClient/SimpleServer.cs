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

         // Create the state object.  
         FoundClient.BeginReceive(ConnectedClient.Buffer, 0, ServerClient.BufferSize, 0,
             new AsyncCallback(ReadCallback), ConnectedClient);

      }

      public void ReadCallback(IAsyncResult ar)
      {
         String content = String.Empty;

         ServerClient ConnectedClient = (ServerClient)ar.AsyncState;
         Socket handler = ConnectedClient.ClientSocket;

         int bytesRead = handler.EndReceive(ar);

         if (bytesRead > 0)
         {
            ConnectedClient.StringData.Append(Encoding.ASCII.GetString(
                ConnectedClient.Buffer, 0, bytesRead));

            content = ConnectedClient.StringData.ToString();

            CheckForNewClient(content, ConnectedClient);

            if (content.IndexOf("<QUIT>") > - 1)
            {
               DisconnectClient(PullClientName(content));

            }
            else if (content.IndexOf("<EOF>") > -1)
            {
               // All the data has been read from the   
               // client. Display it on the console.  
               Console.WriteLine("<SERVER> Read {0} bytes from socket. \n Data : {1}",
                   content.Length, content);

               if (ClientList.Count == 1)
               {
                  Send(ConnectedClient, content);
               }
               else
               {
                  Send(ClientList, content);
               }
            }
            else
            {
               // Not all data received. Get more.  
               handler.BeginReceive(ConnectedClient.Buffer, 0, ServerClient.BufferSize, 0,
               new AsyncCallback(ReadCallback), ConnectedClient);
            }

            ConnectedClient.StringData.Clear();
         }
      }

      private void Send(ServerClient Client, String data)
      {
         string Clientname = PullClientName(data);

         if (Clientname != Client.ClientName)
         {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            Client.ClientSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), Client.ClientSocket);
         }
         else
         {
            Client.ClientSocket.BeginReceive(Client.Buffer, 0, ServerClient.BufferSize, 0, new AsyncCallback(ReadCallback),Client);
         }
      }

      private void Send(List<ServerClient> ClientList, string data)
      {
         foreach (ServerClient ConnectedClient in ClientList)
         {
            Send(ConnectedClient, data);
         }
      }

      private void DisconnectClient(string ClientName)
      {
         Socket SocketToDisconnect;

         foreach(ServerClient client in ClientList.ToArray())
         {
            if(client.ClientName == ClientName)
            {
               SocketToDisconnect = client.ClientSocket;

               SocketToDisconnect.Shutdown(SocketShutdown.Both);
               SocketToDisconnect.Close();

               ClientList.Remove(client);

               return;
            }
         }
      }

      private void SendCallback(IAsyncResult ar)
      {
         try
         {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine($"Sent  {bytesSent} bytes to client.", bytesSent);

            ServerClient ConnectedClient = new ServerClient();
            ConnectedClient.ClientSocket = handler;
            handler.BeginReceive(ConnectedClient.Buffer, 0, ServerClient.BufferSize, 0, new AsyncCallback(ReadCallback), ConnectedClient);
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }

      private void CheckForNewClient(string Content, ServerClient ClientToAdd)
      {
         string ClientName = PullClientName(Content);

         foreach (ServerClient client in ClientList)
         {
            if (client.ClientName == ClientName)
            {
               return;
            }
         }

         if (ClientList.Count < 2)
         {
            ClientToAdd.ClientName = ClientName;

            ClientList.Add(ClientToAdd);
         }
      }

      private int CustomIndexOf(string source, char toFind, int position)
      {
         int index = -1;

         for (int i = 0; i < position; i++)
         {
            index = source.IndexOf(toFind, index + 1);

            if (index == -1)
               break;
         }

         return index + 1;
      }

      private string PullClientName(string Content)
      {
         string ClientName = string.Empty;

         int StartIndex = -1;

         int EndIndex = -1;

         StartIndex = CustomIndexOf(Content, '%', 1);

         EndIndex = CustomIndexOf(Content, '%', 2);

         if (StartIndex != 0 & EndIndex != 0)
         {
            ClientName = Content.Substring(StartIndex, EndIndex - StartIndex - 1);

            return ClientName;
         }

         return ClientName;
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
}
