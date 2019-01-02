using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lan_Tic_Tac_Toe.GameSprites;
using Lan_Tic_Tac_Toe.Graphics;
using Lan_Tic_Tac_Toe.GameComponents;

namespace Lan_Tic_Tac_Toe
{
   public class Game1 : Game
   {
      GraphicsDeviceManager graphics;

      SpriteBatch spriteBatch;

      RenderSystem CurrentRenderSystem;

      GameBoard MainGameBoard;

      Player MainPlayer;

      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);

         Content.RootDirectory = "Content";

         graphics.PreferredBackBufferHeight = 128;
         graphics.PreferredBackBufferWidth = 96;

         IsMouseVisible = true;

         CurrentRenderSystem = new RenderSystem();
      }

      protected override void Initialize()
      {
         base.Initialize();
      }

      protected override void LoadContent()
      {
         spriteBatch = new SpriteBatch(GraphicsDevice);

         Sprites.BlackCellSprite = Content.Load<Texture2D>("BlankCell");

         Sprites.OCellSprite = Content.Load<Texture2D>("OCell");

         Sprites.XCellSprite = Content.Load<Texture2D>("XCell");

         MainGameBoard = new GameBoard();

         MainPlayer = new Player(Player.PlayerTypes.PlayerO);
      }

      protected override void UnloadContent()
      {

      }

      protected override void Update(GameTime gameTime)
      {
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

         Pointer.Update();

         MainGameBoard.GameBoardUpdate(MainPlayer); 

         base.Update(gameTime);
      }

      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.Black);

         CurrentRenderSystem.Render(spriteBatch);

         base.Draw(gameTime);
      }
   }
}
