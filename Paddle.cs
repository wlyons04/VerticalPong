using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PongCSH
{
    internal class Paddle
    {
        public Vector2 pos;
        public Rectangle rect;
        public float dy;

        public Paddle(int x, int y, int width, int height)
        {
            dy = 0;
            rect = new Rectangle(x, y, width, height);
            pos = new Vector2(x, y);
        }

        public void Update(double deltaTime)
        {
            pos.Y += (float)(dy * deltaTime);
            rect.Y = (int)pos.Y;
        }

    }
}
