using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;

namespace PongCSH
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;

        private int WINDOW_HEIGHT = 720;
        private int WINDOW_WIDTH = 1280;

        private int PADDLE_SPEED = 175;

        private SpriteBatch _spriteBatch;

        private Texture2D texture;

        private Vector2 leftPaddlePos;
        private Rectangle leftPaddleRect;

        private Vector2 ballPos;
        private Rectangle ballRect;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;


            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            leftPaddlePos = new Vector2(20, 310);
            leftPaddleRect = new Rectangle((int)leftPaddlePos.X, (int)leftPaddlePos.Y, 12, 100);

            ballPos = new Vector2((WINDOW_WIDTH / 2) - 7, (WINDOW_HEIGHT / 2) - 7);
            ballRect = new Rectangle((int)ballPos.X, (int)ballPos.Y, 14, 14);
            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = new Texture2D(GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.White });

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            KeyboardState kbState = Keyboard.GetState();

            if (kbState.IsKeyDown(Keys.W))
            {
                leftPaddlePos.Y -= (float)(PADDLE_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                leftPaddlePos.Y += (float)(PADDLE_SPEED * gameTime.ElapsedGameTime.TotalSeconds);
            }

            leftPaddleRect.Y = (int)leftPaddlePos.Y;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            _spriteBatch.Draw(texture, leftPaddleRect, Color.White);
            _spriteBatch.Draw(texture, ballRect, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}