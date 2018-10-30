using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ServerClient;
using Network.System;

namespace Client
{
   class Program
   {
      static Random ran = new Random();

      static string UserName;

      static BasicClient Client1;

      static ChatClient ChatUser;

      static void Main(string[] args)
      {
         EnterUserName();

         while (true)
         {
            StartNewClient();
         }
      }

      static void StartClientCommunication()
      {
         try
         {
            Client1 = new BasicClient(UserName);

            Client1.StartClientConnection();

            // Register the event for clicking on the X to close the program
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            if (Client1.isClientConnected())
            {

               // Send name to server
               Client1.SendToServer("THIS IS A TEST");
               Console.WriteLine("------------------------------------------------");

               string aMessage = "";

               while (true)
               {

                  if (Client1.isClientConnected())
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
                  else
                  {
                     break;
                  }
               }

               Client1.Disconnect();

            }
            Console.WriteLine("Try Again");

            Thread.Sleep(1000);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);

            Console.ReadKey();
         }
      }

      static void StartNewClient()
      {
         try
         {
            ChatUser = new ChatClient(UserName, "192.168.16.87", 5000);

            ChatUser.ConnectToServer();

            // Register the event for clicking on the X to close the program
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            Thread.Sleep(500);

            if (ChatUser.isClientConnected() && !ChatUser.IsRoomFull)
            {
               // Send name to server
               ChatUser.SendToServer("THIS IS A TEST");
               Console.WriteLine("------------------------------------------------");

               string aMessage = "";

               while (true)
               {

                  if (ChatUser.isClientConnected())
                  {
                     //aMessage = Console.ReadLine();
                     if ((aMessage = Console.ReadLine()) == "/quit")
                     {
                        ChatUser.SendToServer("/[sys]quit");
                        break;
                     }

                     if (string.IsNullOrEmpty(aMessage.Replace(" ", "")))
                        continue;

                     ChatUser.SendToServer(aMessage);
                  }
                  else
                  {
                     break;
                  }
               }
               ChatUser.Disconnect();
            }

            Console.WriteLine("Try Again");

            Thread.Sleep(1000);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);

            Console.ReadKey();
         }
      }
      static void EnterUserName()
      {
         Console.WriteLine("Enter Username");

         UserName = Console.ReadLine();

         if (string.IsNullOrEmpty(UserName) || string.IsNullOrWhiteSpace(UserName))
         {
            Console.WriteLine("Invalid Username");
            EnterUserName();
         }
      }

      [DllImport("Kernel32")]
      private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
      private delegate bool EventHandler(CtrlType sig);
      static EventHandler _handler;

      enum CtrlType
      {
         CTRL_C_EVENT = 0,
         CTRL_BREAK_EVENT = 1,
         CTRL_CLOSE_EVENT = 2,
         CTRL_LOGOFF_EVENT = 5,
         CTRL_SHUTDOWN_EVENT = 6
      }
      private static bool Handler(CtrlType sig)
      {
         switch (sig)
         {
            case CtrlType.CTRL_C_EVENT:
            case CtrlType.CTRL_LOGOFF_EVENT:
            case CtrlType.CTRL_SHUTDOWN_EVENT:
            case CtrlType.CTRL_CLOSE_EVENT:
            default:
                  ChatUser.Disconnect();
               return false;
         }
      }
   }
}
