using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleNet;
using System;
using System.Collections.Generic;

namespace SampleNET
{
   /// <summary>
   /// This is the main type for your game.
   /// </summary>
   public class Game1 : Game
   {
      GraphicsDeviceManager graphics;

      SpriteBatch spriteBatch;

      SpriteFont DebugFont;

      Texture2D PlayerTexture;

      string Command { get; set; } = "Test";

      string OldCommand = "Test";

      string ConnectionStatus = string.Empty;

      GameClient Test;

      Random ran = new Random();

      List<OtherPlayer> OtherPlayerList = new List<OtherPlayer>();

      Player MainChar;

      double WaitTimeForServer = 0;

      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";

         Window.IsBorderless = false ;

         graphics.PreferredBackBufferHeight = 512;
         graphics.PreferredBackBufferWidth = 512;

         //GameClient = new SimpleClient("Game1" + ran.Next(10, 2000).ToString());

         Test = new GameClient("Player", "192.168.16.87", 5000);
      }

      protected override void Initialize()
      {
         // TODO: Add your initialization logic here

         base.Initialize();
      }

      protected override void LoadContent()
      {
         // Create a new SpriteBatch, which can be used to draw textures.
         spriteBatch = new SpriteBatch(GraphicsDevice);

         DebugFont = Content.Load<SpriteFont>("Font");
         PlayerTexture = Content.Load<Texture2D>("Player");

         Test.ConnectToServer();
      }

      /// <summary>
      /// UnloadContent will be called once per game and is the place to unload
      /// game-specific content.
      /// </summary>
      protected override void UnloadContent()
      {
         // TODO: Unload any non ContentManager content here
      }

      /// <summary>
      /// Allows the game to run logic such as updating the world,
      /// checking for collisions, gathering input, and playing audio.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Update(GameTime gameTime)
      {
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

         MakeFirstContactWithServer(gameTime);

         ConnectionStatus = Test.isClientConnected().ToString();

         string NewCommand = CommandHandler.GetCommand();

         if (!string.IsNullOrEmpty(NewCommand))
         {
            if (NewCommand.IndexOf("ACK") != -1 || NewCommand.IndexOf("ACC") != -1)
            {
               CreatePlayers(NewCommand);
            }

            Command = string.Empty;

            Command += NewCommand;

            OldCommand = Command;
         }
         else
         {
            Command = OldCommand;
         }

         // TODO: Add your update logic here

         base.Update(gameTime);
      }

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.Black);

         spriteBatch.Begin();

         spriteBatch.DrawString(DebugFont, $"CurrentCommand: {Command}", new Vector2(200, 0), Color.White);

         spriteBatch.DrawString(DebugFont, $"Connected to server: {ConnectionStatus}", new Vector2(200, 32), Color.Turquoise);

         foreach (Player P in PlayerList)
         {
            P.Draw(spriteBatch);
         }

         spriteBatch.End();
         // TODO: Add your drawing code here

         base.Draw(gameTime);
      }

      public void CreatePlayers(string PlayerCommand)
      {
         int EndIndex = PlayerCommand.IndexOf("<");

         string Pname = PlayerCommand.Substring(0, EndIndex);

         if (!CheckForPlayer(Pname))
         {
            PlayerList.Add(new Player(PlayerTexture, new Vector2(10 + 10 * PlayerList.Count, 10), Pname));

            BroadCastCurrentPlayers();
         }

      }

      public void UpdatePlayers(GameTime gameTime, string Command)
      {
         foreach (Player P in PlayerList)
         {
            P.Update(gameTime, Command);
         }
      }

      public bool CheckForPlayer(string PlayerName)
      {
         bool ret = false;

         foreach (Player P in PlayerList)
         {
            if (P.PlayerName == PlayerName)
            {
               ret = true;
            }
         }

         return ret;
      }

      public void BroadCastCurrentPlayers()
      {
         foreach (Player P in PlayerList)
         {
            Test.SendToServer($"{P.PlayerName}<ACC><X{P.X}><Y{P.Y}>");
         }
      }

      public void MakeFirstContactWithServer(GameTime gameTime)
      {
         if(WaitTimeForServer < 1201)
         {
            WaitTimeForServer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if(WaitTimeForServer >= 1200)
            {
               if (Test.isClientConnected())
               {
                  Test.SendToServer("Connect");
               }
            }
         }
      }
   }
}
