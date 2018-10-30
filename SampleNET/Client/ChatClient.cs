using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
   class ChatClient
   {
      string ClientName = string.Empty;

      IPAddress ServerIp = IPAddress.Parse("192.168.16.87");

      int ServerPort = 5000;

      Socket ClientSocket;

      string ServerResponse = string.Empty;

      private ManualResetEvent connectDone = new ManualResetEvent(false);
      private ManualResetEvent sendDone = new ManualResetEvent(false);
      private ManualResetEvent receiveDone = new ManualResetEvent(false);

      public bool IsRoomFull = false;

      public ChatClient(string Username, string ServerIP, int Port)
      {
         ClientName = Username;

         ServerIp = IPAddress.Parse(ServerIP);
         ServerPort = Port;
      }

      public void ConnectToServer()
      {
         IPEndPoint ClientEndPoint = new IPEndPoint(ServerIp, ServerPort);

         ClientSocket = new Socket(ServerIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

         IAsyncResult result = ClientSocket.BeginConnect(ClientEndPoint,
      new AsyncCallback(ConnectCallback), ClientSocket);

         connectDone.WaitOne();

         if (isClientConnected(ClientSocket))
         {
            Receive(ClientSocket);
         }
      }

      public void SendToServer(string theMessage)
      {
         StringBuilder DataToSend = new StringBuilder(theMessage.Length);

         DataToSend.Append(' ', DataToSend.Capacity);

         Send(theMessage + "<EOF>");

         sendDone.WaitOne();
      }

      private void ConnectCallback(IAsyncResult ar)
      {
         try
         {
            Socket client = (Socket)ar.AsyncState;

            if (isClientConnected(ClientSocket))
            {
               client.EndConnect(ar);

               Console.WriteLine("<Client>Socket connected to {0}",
                   client.RemoteEndPoint.ToString());
            }
            connectDone.Set();
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }

      private void Receive(Socket ServerSocket)
      {
         try
         {
            if (isClientConnected(ClientSocket))
            {
               Server ConnectedServer = new Server();

               ConnectedServer.ServerSocket = ServerSocket;

               ConnectedServer.ServerSocket.BeginReceive(ConnectedServer.Buffer, 0, Server.BufferSize, 0,
                   new AsyncCallback(StartReceiving), ConnectedServer);
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }

      private void StartReceiving(IAsyncResult AsycSocket)
      {
         string DataRead = string.Empty;

         int bytesRead;

         Server ConnectedServer = (Server)AsycSocket.AsyncState;

         if (isClientConnected(ConnectedServer.ServerSocket))
         {
            SocketError errorCode;

            bytesRead = ConnectedServer.ServerSocket.EndReceive(AsycSocket, out errorCode);

            if (errorCode == SocketError.Success)
            {
               if (bytesRead > 0)
               {
                  ConnectedServer.StringData.Append(Encoding.ASCII.GetString(
                      ConnectedServer.Buffer, 0, bytesRead));

                  DataRead = ConnectedServer.StringData.ToString();

                  Console.WriteLine(DataRead);

                  if (DataRead.IndexOf("<FD>") != -1)
                  {
                     ForceDisconnect();
                     IsRoomFull = true;
                     return;
                  }
                  else
                  {
                     IsRoomFull = false;
                  }

                  //Pass Data To parser**************************************************************************************

                  Receive(ClientSocket);
               }
            }
         }
         ConnectedServer.StringData.Clear();
      }

      public void Disconnect()
      {
         SendToServer($"{ClientName} has disconnected");
      }

      private void Send(String data)
      {
         string DataToSend = $"%{ClientName}%: Sent {data}";

         byte[] byteData = Encoding.ASCII.GetBytes(DataToSend);

         if (isClientConnected(ClientSocket))
         {
            ClientSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), ClientSocket);
         }
      }

      private void SendCallback(IAsyncResult ar)
      {
         try
         {
            if (isClientConnected(ClientSocket))
            {
               Socket ConnectedServerSocket = (Socket)ar.AsyncState;

               int bytesSent = ConnectedServerSocket.EndSend(ar);

               sendDone.Set();
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }

      private bool isClientConnected(Socket Client)
      {
         IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

         TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

         foreach (TcpConnectionInformation c in tcpConnections)
         {
            TcpState stateOfConnection = c.State;

            if (c.LocalEndPoint.Equals(Client.LocalEndPoint) && c.RemoteEndPoint.Equals(Client.RemoteEndPoint))
            {
               if (stateOfConnection == TcpState.Established)
               {
                  return true;
               }
               else
               {
                  return false;
               }
            }
         }
         return false;
      }

      public bool isClientConnected()
      {
         return isClientConnected(ClientSocket);
      }

      private void ForceDisconnect()
      {
         ClientSocket.Shutdown(SocketShutdown.Both);
      }

      class Server
      {
         public string ServerName;

         public Socket ServerSocket = null;

         public const int BufferSize = 1024;

         public byte[] Buffer = new byte[BufferSize];

         public StringBuilder StringData = new StringBuilder();
      }
   }
}

