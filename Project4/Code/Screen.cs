using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project4
{
        static public class Screen
    {
        public static Texture2D BackGraund { get; set; }
        static int timeCounter = 0;
        static Color color;
        public static SpriteFont Font { get; set; }
                
        static public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackGraund, new Rectangle(0,0,1920,1080),color);
            spriteBatch.DrawString(Font, "Game withot name ", new Vector2(50,50), Color.Black);
        }

        static public void Update()
        {
            color = Color.FromNonPremultiplied(255, 255, 255, timeCounter);       
            timeCounter++;
        }

    }
}
