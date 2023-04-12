using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace VerticalPong
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;

        //Resolution settings
        private const int WINDOW_HEIGHT = 980; //1080; //500;
        private const int WINDOW_WIDTH = 420; //463; //214;


        private int WindowWidth;
        private int WindowHeight;

        //Paddle settings
        private int PADDLE_SPEED = 180;
        private int PADDLE_LENGTH;
        private int PADDLE_WIDTH;
        private int PADDLE_X;
        private int TOP_GAP;
        private int BOTTOM_GAP;

        //Ball settings
        private int BALL_WIDTH;
        private int BALL_X;
        private int BALL_Y;

        float velo;


        private SpriteBatch _spriteBatch;

        //Textures and fonts
        private Texture2D[] textures = new Texture2D[1];
        private Texture2D whiteTexture;
        private SpriteFont fontLarge;
        private SpriteFont fontMedium;
        private SpriteFont fontSmall;

        private Texture2D testTitle;
        private Texture2D b1Texture;
        private Texture2D b2Texture;
        private Texture2D b3Texture;
        private Texture2D winTitle;
        private Texture2D winnerTextureP1;
        private Texture2D winnerTextureP2;
        private Texture2D playAgain;
        private Rectangle titleLength;

        //Sound effects
        private SoundEffect bounce;
        private SoundEffect hit;
        private SoundEffect score;


        private Ball ball;
        private Paddle player1;
        private Paddle player2;

        private Rectangle centerLine;
        private int lineHeight;

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


        private KeyboardState previousKB, currentKB;
        private GamePadState previousGP, currentGP;
        private Vector2 previousStick1, currentStick1, previousStick2, currentStick2;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);

            //_graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            //_graphics.PreferredBackBufferWidth = WINDOW_WIDTH;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;


        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Devcade.Input.Initialize();


            // Set window size if running debug (in release it will be fullscreen)
            #region
#if DEBUG
            _graphics.PreferredBackBufferWidth = 420;
            _graphics.PreferredBackBufferHeight = 980;
            _graphics.ApplyChanges();
#else
			_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
			_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
			_graphics.ApplyChanges();
#endif
            #endregion



            int windowHeight = GraphicsDevice.Viewport.Height;
            int windowWidth = GraphicsDevice.Viewport.Width;

            WindowWidth = GraphicsDevice.Viewport.Width;
            WindowHeight = GraphicsDevice.Viewport.Height;

            PADDLE_SPEED = (WindowHeight * 180) / 980;
            

            BALL_WIDTH = windowHeight / 70;
            BALL_X = (windowWidth / 2) - (BALL_WIDTH / 2);
            BALL_Y = (windowHeight / 2) - (BALL_WIDTH / 2);

            PADDLE_LENGTH = windowWidth / 6;
            PADDLE_WIDTH = PADDLE_LENGTH / 6;
            PADDLE_X = (windowWidth / 2) - (PADDLE_LENGTH / 2);


            TOP_GAP = PADDLE_WIDTH * 2;
            BOTTOM_GAP = windowHeight - (int)(TOP_GAP * 1.45);


            ball = new Ball(BALL_X, BALL_Y, BALL_WIDTH, BALL_WIDTH);
            player1 = new Paddle(PADDLE_X, BOTTOM_GAP, PADDLE_LENGTH, PADDLE_WIDTH);
            player2 = new Paddle(PADDLE_X, TOP_GAP, PADDLE_LENGTH, PADDLE_WIDTH);

            lineHeight = windowHeight / 216;
            centerLine = new Rectangle(0, (windowHeight / 2) - (lineHeight/2), windowWidth, lineHeight);


            textures[0] = whiteTexture;

            previousKB = Keyboard.GetState();
            currentKB = Keyboard.GetState();

            previousGP = GamePad.GetState(PlayerIndex.One);
            currentGP = GamePad.GetState(PlayerIndex.One);

            previousStick1 = Devcade.Input.GetStick(1);
            currentStick1 = Devcade.Input.GetStick(1);

            previousStick2 = Devcade.Input.GetStick(2);
            currentStick2 = Devcade.Input.GetStick(2);

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

            testTitle = Content.Load<Texture2D>("JetTitle");
            titleLength = new Rectangle(100, 50, 10, 5);

            b1Texture = Content.Load<Texture2D>("singleP");
            b2Texture = Content.Load<Texture2D>("multiP");
            b3Texture = Content.Load<Texture2D>("quit");

            winTitle = Content.Load<Texture2D>("winTitle");
            winnerTextureP1 = Content.Load<Texture2D>("player1");
            winnerTextureP2 = Content.Load<Texture2D>("player2");
            playAgain = Content.Load<Texture2D>("playAgain");

            bounce = Content.Load<SoundEffect>("pong_bounce");
            hit = Content.Load<SoundEffect>("pong_hit");
            score = Content.Load<SoundEffect>("pong_score");


            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState kbState = Keyboard.GetState();

            currentKB = Keyboard.GetState();
            currentGP = GamePad.GetState(PlayerIndex.One);
            currentStick1 = Devcade.Input.GetStick(1);
            currentStick2 = Devcade.Input.GetStick(2);


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || (currentGP.IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.Menu)))
                Exit();

            // TODO: Add your update logic here


            //Win State
            if (currentState == State.Win)
            {
                if ((previousKB.IsKeyUp(Keys.Enter) && currentKB.IsKeyDown(Keys.Enter)) || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.A1) || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.A1))
                {
                    switch (hoverButtonWin)
                    {
                        case 0:

                            hoverButtonMenu = -1;
                            multiplayer = false;
                            currentState = State.Menu;
                            break;

                        case 1:
                            Exit();
                            break;
                    }

                }


                if ((previousKB.IsKeyUp(Keys.Up) && currentKB.IsKeyDown(Keys.Up)) || ((previousStick2.Y < 0.8) && (currentStick2.Y > 0.8)))
                {
                    hoverButtonWin--;

                    if (hoverButtonWin < 0) hoverButtonWin = 0;
                }

                if ((previousKB.IsKeyUp(Keys.Down) && currentKB.IsKeyDown(Keys.Down)) || ((previousStick2.Y > -0.8) && (currentStick2.Y < -0.8)))
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

                if (((previousKB.IsKeyUp(Keys.Enter) && currentKB.IsKeyDown(Keys.Enter))) || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.A1) || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.A1)) //((previousGP.IsButtonUp((Buttons)Devcade.Input.ArcadeButtons.A1) && (currentGP.IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A1)))))
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


                if ((previousKB.IsKeyUp(Keys.Up) && currentKB.IsKeyDown(Keys.Up)) || ((previousStick2.Y < 0.8) && (currentStick2.Y > 0.8)))
                {
                    hoverButtonMenu--;

                    if (hoverButtonMenu < 0) hoverButtonMenu = 0;
                }

                if ((previousKB.IsKeyUp(Keys.Down) && currentKB.IsKeyDown(Keys.Down)) || ((previousStick2.Y > -0.8) && (currentStick2.Y < -0.8)))
                {
                    hoverButtonMenu++;

                    if (hoverButtonMenu > 2) hoverButtonMenu = 2;
                }

            }

            //Start State
            if (currentState == State.Start)
            {

                if ( (previousKB.IsKeyUp(Keys.Space) && currentKB.IsKeyDown(Keys.Space)) || Devcade.Input.GetButtonDown(1, Devcade.Input.ArcadeButtons.A1) || Devcade.Input.GetButtonDown(2, Devcade.Input.ArcadeButtons.A1)) //currentGP.IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.A1) || currentGP.IsButtonDown((Buttons)Devcade.Input.ArcadeButtons.B1))
                {
                    currentState = State.Play;

                    ball.XVelo = rng.Next(-375, 376);

                    if (servingPlayer == 1)
                    {
                        ball.YVelo = -rng.Next(700, 801);
                    }
                    else
                    {
                        ball.YVelo = rng.Next(700, 801);
                    }


                }
            }

            //Play State
            if (currentState == State.Play)
            {

                //player1 input
                if ((currentKB.IsKeyDown(Keys.A)) || (currentStick2.X < 0))
                {
                    player1.xVelo = -PADDLE_SPEED;
                }
                else if ((currentKB.IsKeyDown(Keys.D)) || (currentStick2.X > 0))
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
                    if ((currentKB.IsKeyDown(Keys.Left)) || (currentStick1.X < 0))
                    {
                        player2.xVelo = -PADDLE_SPEED;
                    }
                    else if ((currentKB.IsKeyDown(Keys.Right)) || (currentStick1.X > 0))
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

                    ball.pos.Y = player1.pos.Y - BALL_WIDTH;
                    onCollision();
                    hit.Play();

                }

                if (ball.rect.Intersects(player2.rect))
                {

                    ball.pos.Y = player2.pos.Y + (BALL_WIDTH + 1);
                    onCollision();
                    hit.Play();
                }

                //top wall
                if (ball.Y < 0)
                {
                    scoreP1++;
                    servingPlayer = 2;
                    ResetPlay();
                    score.Play();
                }

                //bottom wall
                if (ball.Y > WindowHeight - BALL_WIDTH)
                {  
                    scoreP2++;
                    servingPlayer = 1;
                    ResetPlay();
                    score.Play();
                }

                //left wall
                if (ball.X < 0)
                {
                    ball.xVelo = -ball.xVelo;
                    bounce.Play();
                }

                //right wall
                if (ball.X > WindowWidth - (BALL_WIDTH + 4))
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

                velo = ball.yVelo;

            }





            previousKB = currentKB;
            previousGP = currentGP;
            previousStick1 = currentStick1;
            previousStick2 = currentStick2;

            Devcade.Input.Update();
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


                int titleSpace = (WindowWidth - 375) / 2;
                int b1Space = (WindowWidth - 380) / 2;
                int b2Space = (WindowWidth - 350) / 2;
                int b3Space = (WindowWidth - 92) / 2;

                _spriteBatch.Draw(testTitle, new Vector2(titleSpace-70, 0), Color.White);

                switch (hoverButtonMenu)
                {
                    case 0:

                        _spriteBatch.Draw(b1Texture, new Vector2(b1Space, 350), Color.Gold);
                        _spriteBatch.Draw(b2Texture, new Vector2(b2Space, 490), Color.White);
                        _spriteBatch.Draw(b3Texture, new Vector2(b3Space, 640), Color.White);

                        break;

                    case 1:

                        _spriteBatch.Draw(b1Texture, new Vector2(b1Space, 350), Color.White);
                        _spriteBatch.Draw(b2Texture, new Vector2(b2Space, 490), Color.Gold);
                        _spriteBatch.Draw(b3Texture, new Vector2(b3Space, 640), Color.White);

                        break;
                    case 2:

                        _spriteBatch.Draw(b1Texture, new Vector2(b1Space, 350), Color.White);
                        _spriteBatch.Draw(b2Texture, new Vector2(b2Space, 490), Color.White);
                        _spriteBatch.Draw(b3Texture, new Vector2(b3Space, 640), Color.Gold);

                        break;

                    default:

                        _spriteBatch.Draw(b1Texture, new Vector2(b1Space, 350), Color.White);
                        _spriteBatch.Draw(b2Texture, new Vector2(b2Space, 490), Color.White);
                        _spriteBatch.Draw(b3Texture, new Vector2(b3Space, 640), Color.White);

                        break;


                }

            }


            if (currentState == State.Start || currentState == State.Play)
            {
                GraphicsDevice.Clear(Color.Black);

                _spriteBatch.Draw(whiteTexture, centerLine, Color.White);

                _spriteBatch.DrawString(fontMedium, scoreP1.ToString(), new Vector2(20, WindowHeight/2 + 15 ), Color.LightSkyBlue);
                _spriteBatch.DrawString(fontMedium, scoreP2.ToString(), new Vector2(20, WindowHeight/2 - 85), Color.Crimson); //455
                //_spriteBatch.DrawString(fontSmall, velo.ToString(), new Vector2(10, 10), Color.White);

                player1.Draw(_spriteBatch, whiteTexture);
                player2.Draw(_spriteBatch, whiteTexture);
                ball.Draw(_spriteBatch, whiteTexture);

                


            }

            if (currentState == State.Win)
            {

                GraphicsDevice.Clear(Color.Black);

                titleLength.X = 240;

                int winSpace = (WindowWidth - 345) / 2;
                int p1Space = (WindowWidth - 252) / 2;
                int playAgainSpace = (WindowWidth - 238) / 2;
                int quitSpace = (WindowWidth - 92) / 2;

                _spriteBatch.Draw(winTitle, new Vector2(winSpace, 130), Color.White);


                if (winner == 1)
                {
                    _spriteBatch.Draw(winnerTextureP1, new Vector2(p1Space, 280), Color.LightSkyBlue);

                }
                else
                {
                    _spriteBatch.Draw(winnerTextureP2, new Vector2(p1Space, 280), Color.Crimson);

                }

                switch (hoverButtonWin)
                {
                    case 0:

                        _spriteBatch.Draw(playAgain, new Vector2(playAgainSpace, 520), Color.Gold);
                        _spriteBatch.Draw(b3Texture, new Vector2(quitSpace, 620), Color.White);

                        break;

                    case 1:
                        _spriteBatch.Draw(playAgain, new Vector2(playAgainSpace, 520), Color.White);
                        _spriteBatch.Draw(b3Texture, new Vector2(quitSpace, 620), Color.Gold);

                        break;


                }


            }


            _spriteBatch.End();

            base.Draw(gameTime);
        }







        private void ResetPlay()
        {
            ball.X = BALL_X;
            ball.Y = BALL_Y;
            ball.XVelo = 0;
            ball.YVelo = 0;

            player1.pos.X = PADDLE_X;
            player2.pos.X = PADDLE_X;

            currentState = State.Start;
        }

        private void CheckPaddles(float paddleX1, float paddleX2)
        {

            int right = WindowWidth - PADDLE_LENGTH;

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

            ball.YVelo = -ball.YVelo * 1.05f;

            if (ball.XVelo < 0)
            {
                ball.XVelo = -rng.Next(300, 400);
            }
            else
            {
                ball.XVelo = rng.Next(300, 400);
            }

        }



    }
}