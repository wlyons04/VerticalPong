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

        //Resolution settings
        private const int WINDOW_HEIGHT = 720;
        private const int WINDOW_WIDTH = 1280;

        //Paddle settings
        private const int PADDLE_SPEED = 175;
        private const int PADDLE_LENGTH = 80;
        private const int PADDLE_WIDTH = 12;
        private const int PADDLE_Y = (WINDOW_HEIGHT / 2) - PADDLE_LENGTH / 2;
        private const int LEFT_GAP = 20;
        private const int RIGHT_GAP = WINDOW_WIDTH - 30;

        //Ball settings
        private const int BALL_X = (WINDOW_WIDTH / 2) - 7;
        private const int BALL_Y = (WINDOW_HEIGHT / 2) - 7;


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

        private State currentState;

        private int scoreP1;
        private int scoreP2;
        private int servingPlayer;

        private bool multiplayer;

        private int hoverButton;

        private string title, button1, button2, button3;

        private KeyboardState previousKB, currentKB;

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

            previousKB = Keyboard.GetState();
            currentKB = Keyboard.GetState();

            //currentState = State.Start;
            currentState = State.Menu;

            scoreP1 = 0;
            scoreP2 = 0;
            servingPlayer = 1;

            multiplayer = false;

            hoverButton = 0;

            title = "Pong";
            button1 = "Single-player";
            button2 = "Multiplayer";
            button3 = "Quit";

            rng = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            whiteTexture.SetData(new Color[] { Color.White });
            font = Content.Load<SpriteFont>("fonts");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            KeyboardState kbState = Keyboard.GetState();

            currentKB = Keyboard.GetState();


            //Menu State
            if (currentState == State.Menu)
            {
                if (currentKB.IsKeyDown(Keys.Enter))
                {
                    switch (hoverButton)
                    { 
                        case 0:
                            currentState = State.Start;
                            break;

                        case 1:
                            currentState = State.Start;
                            multiplayer = true;
                            break;

                        case 2:
                            Exit();
                            break;

                    
                    }

                }


                if (previousKB.IsKeyUp(Keys.Up) && currentKB.IsKeyDown(Keys.Up))
                {
                    hoverButton--;

                    if (hoverButton < 0) hoverButton = 0;
                }

                if (previousKB.IsKeyUp(Keys.Down) && currentKB.IsKeyDown(Keys.Down))
                {
                    hoverButton++;

                    if (hoverButton > 2) hoverButton = 2;
                }

            }

            //Start State
            if (currentState == State.Start)
            {

                if (currentKB.IsKeyDown(Keys.Space))
                {
                    currentState = State.Play;

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

            //Play State
            if (currentState == State.Play)
            {

                //player1 input
                if (currentKB.IsKeyDown(Keys.W))
                {
                    player1.yVelo = -PADDLE_SPEED;
                }
                else if (currentKB.IsKeyDown(Keys.S))
                {
                    player1.yVelo = PADDLE_SPEED;
                }
                else
                {
                    player1.yVelo = 0;
                }

                if (multiplayer)
                {
                    //player2 input
                    if (currentKB.IsKeyDown(Keys.Up))
                    {
                        player2.yVelo = -PADDLE_SPEED;
                    }
                    else if (currentKB.IsKeyDown(Keys.Down))
                    {
                        player2.yVelo = PADDLE_SPEED;
                    }
                    else
                    {
                        player2.yVelo = 0;
                    }
                }
                else
                {
                    //player2 chases ball
                    if (ball.pos.Y > player2.pos.Y)
                    {
                        player2.yVelo = 150;
                    }

                    if (ball.pos.Y < player2.pos.Y)
                    {
                        player2.yVelo = -150;
                    }

                }


                //make sure paddles stay on screen
                CheckPaddles(player1.pos.Y, player2.pos.Y);


                //ball hits paddle
                if (ball.rect.Intersects(player1.rect) || ball.rect.Intersects(player2.rect))
                {
                    ball.xVelo = -ball.xVelo;
                    ball.xVelo = ball.xVelo * 1.05f;

                    if (ball.yVelo < 0)
                    {
                        ball.yVelo = -rng.Next(100, 200);
                    }
                    else
                    {
                        ball.yVelo = rng.Next(100, 200);
                    }


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
                    ResetPlay();
                }

                //right wall
                if (ball.pos.X > 1263)
                {

                    scoreP1++;
                    servingPlayer = 2;
                    ResetPlay();
                }


                player1.Update(deltaTime);
                player2.Update(deltaTime);
                ball.Update(deltaTime);
            }



            previousKB = currentKB;
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();


            if (currentState == State.Menu)
            {
                GraphicsDevice.Clear(Color.Black);

                _spriteBatch.DrawString(font, title, new Vector2(500, 75), Color.White);

                switch (hoverButton)
                {
                    case 0:
                        _spriteBatch.DrawString(font, button1, new Vector2(500, 300), Color.Orange);
                        _spriteBatch.DrawString(font, button2, new Vector2(500, 450), Color.White);
                        _spriteBatch.DrawString(font, button3, new Vector2(500, 600), Color.White);
                        break;

                    case 1:
                        _spriteBatch.DrawString(font, button1, new Vector2(500, 300), Color.White);
                        _spriteBatch.DrawString(font, button2, new Vector2(500, 450), Color.Orange);
                        _spriteBatch.DrawString(font, button3, new Vector2(500, 600), Color.White);
                        break;
                    case 2:
                        _spriteBatch.DrawString(font, button1, new Vector2(500, 300), Color.White);
                        _spriteBatch.DrawString(font, button2, new Vector2(500, 450), Color.White);
                        _spriteBatch.DrawString(font, button3, new Vector2(500, 600), Color.Orange);
                        break;

                }

            }

            if (currentState == State.Start || currentState == State.Play)
            {
                GraphicsDevice.Clear(Color.Black);

                _spriteBatch.DrawString(font, currentState.ToString(), new Vector2(50, 50), Color.White);
                _spriteBatch.DrawString(font, scoreP1.ToString(), new Vector2(550, 50), Color.White);
                _spriteBatch.DrawString(font, scoreP2.ToString(), new Vector2(700, 50), Color.White);

                player1.Draw(_spriteBatch, whiteTexture);
                player2.Draw(_spriteBatch, whiteTexture);
                ball.Draw(_spriteBatch, whiteTexture);

            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }







        private void ResetPlay()
        {
            ball.pos.X = BALL_X;
            ball.pos.Y = BALL_Y;
            ball.xVelo = 0;
            ball.yVelo = 0;

            player1.pos.Y = PADDLE_Y;
            player2.pos.Y = PADDLE_Y;

            currentState = State.Start;
        }

        private void CheckPaddles(float paddleY1, float paddleY2)
        {
            //paddles reaching edge of screens
            if (paddleY1 < 0)
            {
                player1.pos.Y = 0;
            }
            if (paddleY1 > 640)
            {
                player1.pos.Y = 640;
            }
            if (paddleY2 < 0)
            {
                player2.pos.Y = 0;
            }
            if (paddleY2 > 640)
            {
                player2.pos.Y = 640;
            }

        }

    }
}