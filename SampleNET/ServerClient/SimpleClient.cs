using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerClient
{

   public class SimpleClient
   {

      CommandHandler CommandParser = new CommandHandler();

      public string Command { get; set; }

      public string ClientID;

      public bool IsConnected { get; set; } = false;

      public bool IsDiscconnected { get; set; } = false;

      IPAddress ServerIp = IPAddress.Parse("192.168.16.87");

      private ManualResetEvent connectDone = new ManualResetEvent(false);
      private ManualResetEvent sendDone = new ManualResetEvent(false);
      private ManualResetEvent receiveDone = new ManualResetEvent(false);

      int Port = 5000;

      Socket ClientSocket;

      private static String response = String.Empty;

      public SimpleClient(string ClientName)
      {
         ClientID = ClientName;
      }

      public void StartClientConnection()
      {
         IPEndPoint ClientEndPoint = new IPEndPoint(ServerIp, Port);

         ClientSocket = new Socket(ServerIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

         IAsyncResult result = ClientSocket.BeginConnect(ClientEndPoint,
      new AsyncCallback(ConnectCallback), ClientSocket);

         connectDone.WaitOne();

         // Receive the response from the remote device.  
         Receive(ClientSocket);

         // Write the response to the console.  
         Console.WriteLine("<Client>Connected to server : {0}", isClientConnected().ToString());
      }

      public void Disconnect()
      {
         try
         {
            if (isClientConnected())
            {
               SendToServer("<QUIT>");
               // Release the socket.  
               ClientSocket.Shutdown(SocketShutdown.Both);
               //ClientSocket.Close();
               //  ClientSocket.Dispose();
               IsDiscconnected = true;
               IsConnected = false;
            }
         }
         catch (Exception e)
         {
            Console.WriteLine("<Client> Disconnect Exception: " + e.ToString());
         }
      }

      private void ConnectCallback(IAsyncResult ar)
      {
         try
         {
            if (isClientConnected())
            {
               // Retrieve the socket from the state object.  
               Socket client = (Socket)ar.AsyncState;

               // Complete the connection.  
               client.EndConnect(ar);

               Console.WriteLine("<Client>Socket connected to {0}",
                   client.RemoteEndPoint.ToString());
            }

            // Signal that the connection has been made.  
            connectDone.Set();
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }

      private void Receive(Socket client)
      {
         try
         {
            if (isClientConnected())
            {
               // Create the state object.  
               Client ConnectedClient = new Client();
               ConnectedClient.ClientSocket = client;

               // Begin receiving the data from the remote device.  
               client.BeginReceive(ConnectedClient.Buffer, 0, Client.BufferSize, 0,
                   new AsyncCallback(ReceiveCallback), ConnectedClient);
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }

      private void ReceiveCallback(IAsyncResult ar)
      {
         try
         {
            if (isClientConnected())
            {
               Client ConnectedClient = (Client)ar.AsyncState;
               Socket client = ConnectedClient.ClientSocket;

               // Read data from the remote device.  
               SocketError errorCode;

               int bytesRead = ConnectedClient.ClientSocket.EndReceive(ar, out errorCode);

               if (errorCode != SocketError.Success)
               {
                  bytesRead = 0;
                  client.Shutdown(SocketShutdown.Both);
                  client.Close();

                  IsConnected = false;
               }

               if (bytesRead > 0)
               {
                  // There might be more data, so store the data received so far.  
                  ConnectedClient.StringData.Append(Encoding.ASCII.GetString(ConnectedClient.Buffer, 0, bytesRead));

                     if (ConnectedClient.StringData.ToString().Contains("<FD>"))
                     {
                        Disconnect();
                     }

                  CommandParser.ParseCommand(ConnectedClient.StringData.ToString());

                  ConnectedClient.StringData.Clear();

                  if (isClientConnected())
                  {
                     // Get the rest of the data.  
                     client.BeginReceive(ConnectedClient.Buffer, 0, Client.BufferSize, 0,
                         new AsyncCallback(ReceiveCallback), ConnectedClient);
                  }
               }
               else
               {
                  // All the data has arrived; put it in response.  
                  if (ConnectedClient.StringData.Length > 1)
                  {
                     response = ConnectedClient.StringData.ToString();

                     if (response.Contains("<FD>"))
                     {
                        Disconnect();
                     }
                  }
                  // Signal that all bytes have been received.  
                  receiveDone.Set();
               }
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
      }

      public void SendToServer(string theMessage)
      {
         StringBuilder test = new StringBuilder(theMessage.Length);

         test.Append(' ', test.Capacity);

         Send(ClientSocket, theMessage + "<EOF>");

         sendDone.WaitOne();
      }

      private void Send(Socket client, String data)
      {
         string DataToSend = $"%{ClientID}%: Sent {data}";

         // Convert the string data to byte data using ASCII encoding.  
         byte[] byteData = Encoding.ASCII.GetBytes(DataToSend);

         if (isClientConnected())
         {
            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
         }
         else
         {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("Connection has been terminated");

            Console.ForegroundColor = ConsoleColor.Gray;

            Disconnect();
         }
      }

      private void SendCallback(IAsyncResult ar)
      {
         try
         {
            if (isClientConnected())
            {
               Socket client = (Socket)ar.AsyncState;

               int bytesSent = client.EndSend(ar);

               Console.ForegroundColor = ConsoleColor.Green;

               // Console.WriteLine($"<Client> {ClientID} Sent {bytesSent} bytes to server.");

               Console.ForegroundColor = ConsoleColor.Gray;

               sendDone.Set();
            }
         }
         catch (Exception e)
         {
            Console.WriteLine(e.ToString());
         }
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
      public bool isClientConnected()
      {
         IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

         TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

         foreach (TcpConnectionInformation c in tcpConnections)
         {
            TcpState stateOfConnection = c.State;

            if (c.LocalEndPoint.Equals(ClientSocket.LocalEndPoint) && c.RemoteEndPoint.Equals(ClientSocket.RemoteEndPoint))
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
