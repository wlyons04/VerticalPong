using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace PongCSH
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;

        private const int WINDOW_HEIGHT = 720;
        private const int WINDOW_WIDTH = 1280;
        private const int PADDLE_SPEED = 175;

        private const int PADDLE_LENGTH = 80;
        private const int PADDLE_WIDTH = 12;

        private const int BALL_X = (WINDOW_WIDTH / 2) - 7;
        private const int BALL_Y = (WINDOW_HEIGHT / 2) - 7;
        private const int PADDLE_Y = (WINDOW_HEIGHT / 2) - PADDLE_LENGTH/2;
        private const int LEFT_GAP = 20;
        private const int RIGHT_GAP = WINDOW_WIDTH - 30;

        private SpriteBatch _spriteBatch;

        private Texture2D[] textures = new Texture2D[1];
        private Texture2D whiteTexture;
        private SpriteFont font;

        private Ball ball;
        private Paddle player1;
        private Paddle player2;

        private Random rng;

        private enum State
        { 
            Start,
            Play,
            Menu
        
        }

        State myState;

        int scoreP1;
        int scoreP2;
        int servingPlayer;



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

            ball = new Ball(BALL_X, BALL_Y, 14, 14);
            player1 = new Paddle(LEFT_GAP, PADDLE_Y, PADDLE_WIDTH, PADDLE_LENGTH);
            player2 = new Paddle(RIGHT_GAP, PADDLE_Y, PADDLE_WIDTH, PADDLE_LENGTH);


            textures[0] = whiteTexture;
            font = Content.Load<SpriteFont>("fonts");

            myState = State.Start;

            scoreP1 = 0;
            scoreP2 = 0;
            servingPlayer = 1;

            rng = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            whiteTexture.SetData(new Color[] { Color.White });

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            KeyboardState kbState = Keyboard.GetState();


            if (myState == State.Start)
            {

                if (kbState.IsKeyDown(Keys.Space))
                {
                    myState = State.Play;

                    ball.yVelo = rng.Next(-175, 176);

                    if (servingPlayer == 1)
                    {
                        ball.xVelo = rng.Next(200, 301);
                    }
                    else
                    {
                        ball.xVelo = -rng.Next(200, 301);
                    }


                }
            }


            if (myState == State.Play)
            {

                //player1 input
                if (kbState.IsKeyDown(Keys.W))
                {
                    player1.yVelo = -PADDLE_SPEED;
                }
                else if (kbState.IsKeyDown(Keys.S))
                {
                    player1.yVelo = PADDLE_SPEED;
                }
                else
                {
                    player1.yVelo = 0;
                }

                //paddles reaching edge of screens
                if (player1.pos.Y < 0)
                {
                    player1.pos.Y = 0;
                }
                if (player1.pos.Y > 640)
                {
                    player1.pos.Y = 640;
                }
                if (player2.pos.Y < 0)
                {
                    player2.pos.Y = 0;
                }
                if (player2.pos.Y > 640)
                {
                    player2.pos.Y = 640;
                }


                //player2 chases ball
                if (ball.pos.Y > player2.pos.Y)
                {
                    player2.yVelo = 100;
                }

                if (ball.pos.Y < player2.pos.Y)
                { 
                    player2.yVelo = -100;
                }


                //ball hits paddle
                if (ball.rect.Intersects(player1.rect) || ball.rect.Intersects(player2.rect))
                {
                    ball.xVelo = -ball.xVelo;
                    ball.xVelo = ball.xVelo * 1.1f;
                    ball.yVelo = ball.yVelo * 1.1f;

                }

                //top wall
                if (ball.pos.Y < 0)
                {
                    ball.yVelo = -ball.yVelo;
                }

                //bottom wall
                if (ball.pos.Y > 706)
                {
                    ball.yVelo = -ball.yVelo;
                }

                //left wall
                if (ball.pos.X < 0)
                {

                    scoreP2++;
                    servingPlayer = 1;
                    Reset();
                }

                //right wall
                if (ball.pos.X > 1263)
                {

                    scoreP1++;
                    servingPlayer = 2;
                    Reset();
                }


                player1.Update(deltaTime);
                player2.Update(deltaTime);
                ball.Update(deltaTime);
            }






            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();


            player1.Draw(_spriteBatch, whiteTexture);
            player2.Draw(_spriteBatch, whiteTexture);
            ball.Draw(_spriteBatch, whiteTexture);

            _spriteBatch.DrawString(font, myState.ToString(), new Vector2(50, 50), Color.White);
            _spriteBatch.DrawString(font, scoreP1.ToString(), new Vector2(550, 50), Color.White);
            _spriteBatch.DrawString(font, scoreP2.ToString(), new Vector2(700, 50), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }


        private void Reset()
        {
            ball.pos.X = BALL_X;
            ball.pos.Y = BALL_Y;
            ball.xVelo = 0;
            ball.yVelo = 0;

            player1.pos.Y = PADDLE_Y;
            player2.pos.Y = PADDLE_Y;

            myState = State.Start;
        }

    }
}