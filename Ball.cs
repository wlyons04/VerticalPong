using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace PongCSH
{
    internal class Ball
    {

        public Vector2 pos;
        public float xVelo;
        public float yVelo;
        public Rectangle rect;


        public Ball(int x, int y, int width, int height)
        {
            xVelo = 100;
            yVelo = 100;
            rect = new Rectangle(x, y, width, height);
            pos = new Vector2(x, y);

        }

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
