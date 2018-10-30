﻿using System;
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
   class EchoServer
   {
      public static ManualResetEvent allDone = new ManualResetEvent(false);

      readonly IPAddress ServerIP = IPAddress.Parse("192.168.16.87");

      readonly int Port = 5000;

      ServerLogger ServerLog = new ServerLogger();

      Socket MainServerSocket;

      List<Client> ClientsList = new List<Client>();

      int ClientCap = 1;

      public EchoServer(string EchoIP,int EchoPort)
      {
         ServerIP = IPAddress.Parse(EchoIP);
         Port = EchoPort;
      }

      public EchoServer()
      {

      }

      public void StartServer()
      {
         if (IntalizeServer())
         {
            ServerLog.Log($"Server has started on {ServerIP} Port: {Port}", LogColor.Success);
            ServerLog.Log("Ready to receive connections", LogColor.Success);
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

      private void Receive(Client Client)
      {
         if (isClientConnected(Client))
         {
            Client.ClientSocket.BeginReceive(Client.Buffer, 0, Client.BufferSize, 0,
                new AsyncCallback(StartReceiving), Client);
         }
      }

      private void SendDataToClient(Client Client, String Data)
      {
         string Clientname = Data;

         byte[] byteData = Encoding.ASCII.GetBytes(Data);

         if (isClientConnected(Client))
         {
            Client.ClientSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), Client.ClientSocket);
         }
      }

      private void EchoToAllClients(string Data,Client ClientToIgnore)
      {
         foreach (Client ConnectedClient in ClientsList.ToArray())
         {
            if (ConnectedClient != ClientToIgnore)
            {
               SendDataToClient(ConnectedClient, Data);
            }
         }
      }

      private void StartReceiving(IAsyncResult AsycSocket)
      {
         string DataRead = string.Empty;

         int bytesRead;

         Client ConnectedClient = (Client)AsycSocket.AsyncState;

         if (isClientConnected(ConnectedClient))
         {
            SocketError errorCode;

            bytesRead = ConnectedClient.ClientSocket.EndReceive(AsycSocket, out errorCode);

            if (errorCode == SocketError.Success)
            {
               if (bytesRead > 0)
               {
                  ConnectedClient.StringData.Append(Encoding.ASCII.GetString(
                      ConnectedClient.Buffer, 0, bytesRead));

                  DataRead = ConnectedClient.StringData.ToString();

                  EchoToAllClients(DataRead,ConnectedClient);

                  ServerLog.Log($"String sent to server {DataRead}", LogColor.Debug);

                  
                  Receive(ConnectedClient);
               }
            }
         }
         ConnectedClient.StringData.Clear();
      }

      private void SendCallback(IAsyncResult AsycSocket)
      {
         try
         {
            Client ConnectedClient = new Client();

            Socket AcceptedSocket = (Socket)AsycSocket.AsyncState;

            ConnectedClient.ClientSocket = AcceptedSocket;

            int bytesSent = ConnectedClient.ClientSocket.EndSend(AsycSocket);

            Receive(ConnectedClient);
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }

      private void AcceptConnections(IAsyncResult AsycSocket)
      {
         allDone.Set();

         Client ConnectedClient = new Client();

         Socket AcceptedSocket = (Socket)AsycSocket.AsyncState;

         ConnectedClient.ClientSocket = AcceptedSocket.EndAccept(AsycSocket);

         if (ClientsList.Count < ClientCap)
         {
            ClientsList.Add(ConnectedClient);

            Receive(ConnectedClient);
         }
         else
         {
            SendDataToClient(ConnectedClient, "Room is full...<FD>");
         }
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

      private bool isClientConnected(Client CurrentClient)
      {
         IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

         TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

         try
         {
            foreach (TcpConnectionInformation c in tcpConnections)
            {
               TcpState stateOfConnection = c.State;

               if (c.LocalEndPoint.Equals(CurrentClient.ClientSocket.LocalEndPoint) && c.RemoteEndPoint.Equals(CurrentClient.ClientSocket.RemoteEndPoint))
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
         }
         catch
         {

         }

         RemoveDisconnectedClient(CurrentClient);

         return false;
      }

      private void CleanUpConnections()
      {
         foreach(Client client in ClientsList.ToArray())
         {
            isClientConnected(client);
         }
      }

      private void RemoveDisconnectedClient(Client ClientToRemove)
      {
         ClientsList.Remove(ClientToRemove);
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
