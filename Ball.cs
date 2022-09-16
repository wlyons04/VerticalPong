using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace PongCSH
{
    internal class Ball
    {

        public Vector2 pos;
        public float dx;
        public float dy;
        public Rectangle rect;
        public Ball(int x, int y, int width, int height)
        {
            dx = 100;
            dy = 100;
            rect = new Rectangle(x, y, width, height);
            pos = new Vector2(x, y);

        }

        public void Update(double deltaTime)
        {
            pos.X += (float)(dx * deltaTime);
            rect.X = (int)pos.X;

            pos.Y += (float)(dy * deltaTime);
            rect.Y = (int)pos.Y;

            //Collision walls
            if (pos.Y < 0)
            {
                dy = -dy;
            }
            if (pos.Y > 706)
            {
                dy = -dy;
            }
            if (pos.X < 0)
            {
                dx = -dx;
            }
            if (pos.X > 1263)
            {
                dx = -dx;
            }

        }

    }
}
