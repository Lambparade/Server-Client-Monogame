using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.ServerUtils;
using static Server.ServerUtils.ServerLogger;

namespace Server
{
   class ImprovedServer
   {
      public static ManualResetEvent allDone = new ManualResetEvent(false);

      readonly IPAddress ServerIP = IPAddress.Parse("192.168.16.87");

      readonly int Port = 5000;

      ServerLogger ServerLog = new ServerLogger();

      Socket MainServerSocket;

      public enum Tag
      {
         EOM,
      }

      public void StartServer()
      {
         if (IntalizeServer())
         {
            ServerLog.Log($"Server has started on {ServerIP} Port: {Port}", LogColor.Success);
            ServerLog.Log("Ready to receive connections",LogColor.Success);
            ServerLog.Log("----------------------------------------");

            while (true)
            {
               allDone.Reset();

               MainServerSocket.BeginAccept(
                    new AsyncCallback(AcceptConnections),
                    MainServerSocket);

               allDone.WaitOne();
            }
         }
      }

      private void AcceptConnections(IAsyncResult AsycSocket)
      {
         allDone.Set();

         Client ConnectedClient = new Client();

         Socket AcceptedSocket = (Socket)AsycSocket.AsyncState;

         ConnectedClient.ClientSocket = AcceptedSocket.EndAccept(AsycSocket);

         ServerLog.Log($"A connection has been made from {ConnectedClient.ClientSocket.LocalEndPoint} ",LogColor.Success);

         ConnectedClient.ClientSocket.BeginReceive(ConnectedClient.Buffer, 0, Client.BufferSize, 0,
             new AsyncCallback(Receive), ConnectedClient);
      }

      private void Receive(IAsyncResult ar)
      {
         string DataRead = string.Empty;
         int bytesRead;

         Client ConnectedClient = (Client)ar.AsyncState;

         if (isClientConnected(ConnectedClient.ClientSocket))
         {
            SocketError errorCode;

            bytesRead = ConnectedClient.ClientSocket.EndReceive(ar, out errorCode);

            if (errorCode == SocketError.Success)
            {
               if (bytesRead > 0)
               {
                  ConnectedClient.StringData.Append(Encoding.ASCII.GetString(
                      ConnectedClient.Buffer, 0, bytesRead));

                  DataRead = ConnectedClient.StringData.ToString();

                  ServerLog.Log(DataRead,LogColor.Debug);
               }
            }
         }
         ConnectedClient.StringData.Clear();
   }

   private bool IntalizeServer()
   {
      bool ServerConfiguredCorrectly = false;

      try
      {
         IPEndPoint ServerEndPoint = new IPEndPoint(ServerIP, Port);

         MainServerSocket = new Socket(ServerIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

         MainServerSocket.Bind(ServerEndPoint);

         MainServerSocket.Listen(100);

         ServerConfiguredCorrectly = true;
      }
      catch (Exception ex)
      {

      }

      return ServerConfiguredCorrectly;
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
}
class Client
{
   public string ClientName;

   public Socket ClientSocket = null;

   public const int BufferSize = 1024;

   public byte[] Buffer = new byte[BufferSize];

   public StringBuilder StringData = new StringBuilder();
}
}
