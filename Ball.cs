using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace VerticalPong
{
    internal class Ball
    {

        // --- Fields ---

        public Vector2 pos;
        private float x;
        private float y;
        public float xVelo;
        public float yVelo;
        public Rectangle rect;


        // --- Constructor(s) ---

        public Ball(int x, int y, int width, int height)
        {

            xVelo = 100;
            yVelo = 100;
            rect = new Rectangle(x, y, width, height);
            pos = new Vector2(x, y);

        }



        // --- Properties ---

        public float X { get { return pos.X; } set { pos.X = value; } }

        public float Y { get { return pos.Y; } set { pos.Y = value; } }

        public float XVelo { get { return xVelo; } set { xVelo = value; } }

        public float YVelo { get { return yVelo; } 

            set { 
            
                if (value < -2200)
                {
                    yVelo = -2200;
                }
                else if (value > 2200)
                {
                    yVelo = 2200;
                }
                else
                {
                    yVelo = value;
                }
            
            }
            
        }



        // --- Methods ---

        public void Update(double deltaTime)
        {
            pos.X += (float)(xVelo * deltaTime);
            rect.X = (int)pos.X;

            pos.Y += (float)(yVelo * deltaTime);
            rect.Y = (int)pos.Y;

        }


        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {

            spriteBatch.Draw(texture, rect, Color.White);

        }


    }
}
