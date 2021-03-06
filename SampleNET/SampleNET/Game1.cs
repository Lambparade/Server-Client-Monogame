﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SimpleNet;
using System;

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

      string Command { get; set; } = "Test";

      string OldCommand = "Test";

      SimpleClient GameClient;

      Random ran = new Random();
      
      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";

         Window.IsBorderless = true;

         graphics.PreferredBackBufferHeight = 512;
         graphics.PreferredBackBufferWidth = 512;

         GameClient = new SimpleClient("Game1" + ran.Next(10, 2000).ToString());
      }

      /// <summary>
      /// Allows the game to perform any initialization it needs to before starting to run.
      /// This is where it can query for any required services and load any non-graphic
      /// related content.  Calling base.Initialize will enumerate through any components
      /// and initialize them as well.
      /// </summary>
      protected override void Initialize()
      {
         // TODO: Add your initialization logic here

         base.Initialize();
      }

      /// <summary>
      /// LoadContent will be called once per game and is the place to load
      /// all of your content.
      /// </summary>
      protected override void LoadContent()
      {
         // Create a new SpriteBatch, which can be used to draw textures.
         spriteBatch = new SpriteBatch(GraphicsDevice);

         DebugFont = Content.Load<SpriteFont>("Font");

         GameClient.StartClientConnection();

         GameClient.SendToServer("Hello from game!");
         // TODO: use this.Content to load your game content here
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

         string NewCommand = CommandHandler.GetCommand();

         if (!string.IsNullOrEmpty(NewCommand))
         {
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

         spriteBatch.DrawString(DebugFont, Command, new Vector2(50, 50), Color.White);

         spriteBatch.End();
         // TODO: Add your drawing code here

         base.Draw(gameTime);
      }
   }
}
