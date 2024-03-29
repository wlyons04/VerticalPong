﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VerticalPong
{
    internal class Paddle
    {
        public Vector2 pos;
        public Rectangle rect;
        public float xVelo;

        public Paddle(int x, int y, int width, int height)
        {
            xVelo = 0;
            rect = new Rectangle(x, y, width, height);
            pos = new Vector2(x, y);
        }

        public void Update(double deltaTime)
        {

            pos.X += (float)(xVelo * deltaTime);
            rect.X = (int)pos.X;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, rect, Color.White);
        }

    }
}
