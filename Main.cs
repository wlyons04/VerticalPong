using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PongCSH
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;

        //Resolution settings
        private const int WINDOW_HEIGHT = 980;
        private const int WINDOW_WIDTH = 420;

        //Paddle settings
        private const int PADDLE_SPEED = 175;
        private const int PADDLE_LENGTH = 60;
        private const int PADDLE_WIDTH = 10;
        private const int PADDLE_X = (WINDOW_WIDTH / 2) - PADDLE_LENGTH / 2;
        private const int TOP_GAP = 20;
        private const int BOTTOM_GAP = WINDOW_HEIGHT - 30;

        //Ball settings
        private const int BALL_WIDTH = 14;
        private const int BALL_X = (WINDOW_WIDTH / 2) - 7;
        private const int BALL_Y = (WINDOW_HEIGHT / 2) - 7;


        private SpriteBatch _spriteBatch;

        //Textures and fonts
        private Texture2D[] textures = new Texture2D[1];
        private Texture2D whiteTexture;
        private SpriteFont fontLarge;
        private SpriteFont fontMedium;
        private SpriteFont fontSmall;

        //Sound effects
        private SoundEffect bounce;
        private SoundEffect hit;
        private SoundEffect score;


        private Ball ball;
        private Paddle player1;
        private Paddle player2;

        private Random rng;

        private enum State
        { 
            Start,
            Play,
            Menu,
            Win
        }

        private State currentState;

        private int scoreP1;
        private int scoreP2;
        private int servingPlayer;
        private int winner;

        private int winAmount;

        //if playing in one or two player mode
        private bool multiplayer;

        //which button is hovered; Ex: top button hovered, hoverButton = 0
        private int hoverButtonMenu;
        private int hoverButtonWin;


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

            ball = new Ball(BALL_X, BALL_Y, BALL_WIDTH, BALL_WIDTH);
            player1 = new Paddle(PADDLE_X, TOP_GAP, PADDLE_LENGTH, PADDLE_WIDTH);
            player2 = new Paddle(PADDLE_X, BOTTOM_GAP, PADDLE_LENGTH, PADDLE_WIDTH);


            textures[0] = whiteTexture;

            previousKB = Keyboard.GetState();
            currentKB = Keyboard.GetState();

            //currentState = State.Start;
            currentState = State.Menu;

            scoreP1 = 0;
            scoreP2 = 0;
            servingPlayer = 1;
            winner = 0;
            winAmount = 3;

            multiplayer = false;

            hoverButtonMenu = 0;
            hoverButtonWin = 0;

            title = "Pong";
            button1 = "Singleplayer";
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
            fontLarge = Content.Load<SpriteFont>("font60");
            fontMedium = Content.Load<SpriteFont>("font40");
            fontSmall = Content.Load<SpriteFont>("font20");

            bounce = Content.Load<SoundEffect>("pong_bounce");
            hit = Content.Load<SoundEffect>("pong_hit");
            score = Content.Load<SoundEffect>("pong_score");


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

            //Win State
            if (currentState == State.Win)
            {
                if (previousKB.IsKeyUp(Keys.Enter) && currentKB.IsKeyDown(Keys.Enter))
                {
                    switch (hoverButtonWin)
                    {
                        case 0:

                            currentState = State.Menu;
                            break;

                        case 1:
                            Exit();
                            break;
                    }

                }


                if (previousKB.IsKeyUp(Keys.Up) && currentKB.IsKeyDown(Keys.Up))
                {
                    hoverButtonWin--;

                    if (hoverButtonWin < 0) hoverButtonWin = 0;
                }

                if (previousKB.IsKeyUp(Keys.Down) && currentKB.IsKeyDown(Keys.Down))
                {
                    hoverButtonWin++;

                    if (hoverButtonWin > 1) hoverButtonWin = 1;
                }

            }

            //Menu State
            if (currentState == State.Menu)
            {

                winner = 0;
                scoreP1 = 0;
                scoreP2 = 0;

                if (previousKB.IsKeyUp(Keys.Enter) && currentKB.IsKeyDown(Keys.Enter))
                {
                    switch (hoverButtonMenu)
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
                    hoverButtonMenu--;

                    if (hoverButtonMenu < 0) hoverButtonMenu = 0;
                }

                if (previousKB.IsKeyUp(Keys.Down) && currentKB.IsKeyDown(Keys.Down))
                {
                    hoverButtonMenu++;

                    if (hoverButtonMenu > 2) hoverButtonMenu = 2;
                }

            }

            //Start State
            if (currentState == State.Start)
            {

                if (currentKB.IsKeyDown(Keys.Space))
                {
                    currentState = State.Play;

                    ball.xVelo = rng.Next(-175, 176);

                    if (servingPlayer == 1)
                    {
                        ball.yVelo = rng.Next(200, 301);
                    }
                    else
                    {
                        ball.yVelo = -rng.Next(200, 301);
                    }


                }
            }

            //Play State
            if (currentState == State.Play)
            {

                //player1 input
                if (currentKB.IsKeyDown(Keys.A))
                {
                    player1.xVelo = -PADDLE_SPEED;
                }
                else if (currentKB.IsKeyDown(Keys.D))
                {
                    player1.xVelo = PADDLE_SPEED;
                }
                else
                {
                    player1.xVelo = 0;
                }

                if (multiplayer)
                {
                    //player2 input
                    if (currentKB.IsKeyDown(Keys.Left))
                    {
                        player2.xVelo = -PADDLE_SPEED;
                    }
                    else if (currentKB.IsKeyDown(Keys.Right))
                    {
                        player2.xVelo = PADDLE_SPEED;
                    }
                    else
                    {
                        player2.xVelo = 0;
                    }
                }
                else
                {
                    //player2 chases ball
                    if (ball.pos.X > player2.pos.X)
                    {
                        player2.xVelo = PADDLE_SPEED;
                    }

                    if (ball.pos.X < player2.pos.X)
                    {
                        player2.xVelo = -PADDLE_SPEED;
                    }

                }


                //make sure paddles stay on screen
                CheckPaddles(player1.pos.X, player2.pos.X);


                //ball hits paddle
                if (ball.rect.Intersects(player1.rect))
                {
                    ball.pos.Y = player1.pos.Y + (BALL_WIDTH + 1);
                    onCollision();
                    hit.Play();
                }

                if (ball.rect.Intersects(player2.rect))
                {
                    ball.pos.Y = player2.pos.Y - BALL_WIDTH;
                    onCollision();
                    hit.Play();
                }

                //top wall
                if (ball.pos.Y < 0)
                {
                    scoreP2++;
                    servingPlayer = 1;
                    ResetPlay();
                    score.Play();
                }

                //bottom wall
                if (ball.pos.Y > WINDOW_HEIGHT - BALL_WIDTH)
                {  
                    scoreP1++;
                    servingPlayer = 2;
                    ResetPlay();
                    score.Play();
                }

                //left wall
                if (ball.pos.X < 0)
                {
                    ball.xVelo = -ball.xVelo;
                    bounce.Play();
                }

                //right wall
                if (ball.pos.X > WINDOW_WIDTH - (BALL_WIDTH + 4))
                {
                    ball.xVelo = -ball.xVelo;
                    bounce.Play();
                }


                if (scoreP1 == winAmount)
                {
                    winner = 1;
                    currentState = State.Win;
                }

                if (scoreP2 == winAmount)
                {
                    winner = 2;
                    currentState = State.Win;
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

                _spriteBatch.DrawString(fontLarge, title, new Vector2(110, 90), Color.White);

                switch (hoverButtonMenu)
                {
                    case 0:
                        _spriteBatch.DrawString(fontMedium, button1, new Vector2(20, 330), Color.Gold);
                        _spriteBatch.DrawString(fontMedium, button2, new Vector2(35, 480), Color.White);
                        _spriteBatch.DrawString(fontSmall, button3, new Vector2(160, 630), Color.White);
                        break;

                    case 1:
                        _spriteBatch.DrawString(fontMedium, button1, new Vector2(20, 330), Color.White);
                        _spriteBatch.DrawString(fontMedium, button2, new Vector2(35, 480), Color.Gold);
                        _spriteBatch.DrawString(fontSmall, button3, new Vector2(160, 630), Color.White);
                        break;
                    case 2:
                        _spriteBatch.DrawString(fontMedium, button1, new Vector2(20, 330), Color.White);
                        _spriteBatch.DrawString(fontMedium, button2, new Vector2(35, 480), Color.White);
                        _spriteBatch.DrawString(fontSmall, button3, new Vector2(160, 630), Color.Gold);
                        break;

                }

            }


            if (currentState == State.Start || currentState == State.Play)
            {
                GraphicsDevice.Clear(Color.Black);


                _spriteBatch.DrawString(fontMedium, scoreP1.ToString(), new Vector2(20, 455), Color.LightSkyBlue);
                _spriteBatch.DrawString(fontMedium, scoreP2.ToString(), new Vector2(365, 455), Color.Crimson);

                player1.Draw(_spriteBatch, whiteTexture);
                player2.Draw(_spriteBatch, whiteTexture);
                ball.Draw(_spriteBatch, whiteTexture);


            }

            if (currentState == State.Win)
            {

                GraphicsDevice.Clear(Color.Black);
                _spriteBatch.DrawString(fontLarge, "Winner!", new Vector2(50, 110), Color.White);

                if (winner == 1)
                {
                    _spriteBatch.DrawString(fontMedium, "Player " + winner, new Vector2(80, 260), Color.LightSkyBlue);
                }
                else
                {
                    _spriteBatch.DrawString(fontMedium, "Player " + winner, new Vector2(80, 260), Color.Crimson);
                }

                switch (hoverButtonWin)
                {
                    case 0:
                        _spriteBatch.DrawString(fontSmall, "Play Again", new Vector2(90, 500), Color.Gold);
                        _spriteBatch.DrawString(fontSmall, "Quit", new Vector2(160, 600), Color.White);
                        break;

                    case 1:
                        _spriteBatch.DrawString(fontSmall, "Play Again", new Vector2(90, 500), Color.White);
                        _spriteBatch.DrawString(fontSmall, "Quit", new Vector2(160, 600), Color.Gold);
                        break;


                }


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

            player1.pos.X = PADDLE_X;
            player2.pos.X = PADDLE_X;

            currentState = State.Start;
        }

        private void CheckPaddles(float paddleX1, float paddleX2)
        {

            int right = WINDOW_WIDTH - PADDLE_LENGTH;

            //paddles reaching edge of screens
            if (paddleX1 < 0)
            {
                player1.pos.X = 0;
            }
            if (paddleX1 > right)
            {
                player1.pos.X = right;
            }
            if (paddleX2 < 0)
            {
                player2.pos.X = 0;
            }
            if (paddleX2 > right)
            {
                player2.pos.X = right;
            }

        }

        //Inverses ball velo on collison
        private void onCollision()
        {
            ball.yVelo = -ball.yVelo;
            ball.yVelo = ball.yVelo * 1.05f;

            if (ball.xVelo < 0)
            {
                ball.xVelo = -rng.Next(100, 200);
            }
            else
            {
                ball.xVelo = rng.Next(100, 200);
            }
        }



    }
}