using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project4
{
    internal class Rezalt
    {
        public static Texture2D BackGraund { get; set; }
        public static Texture2D RezaltGame { get; set; }
        public static Texture2D GameOver { get; set; }
        public static Texture2D Back { get; set; }

        private Button backButton;

        public event EventHandler BackToMenuClicked;

        public Rezalt(int screenWidth, int screenHeight)
        {
            Vector2 panelCenter = new Vector2(screenWidth / 2, screenHeight / 2);
            Vector2 panelPos = panelCenter + new Vector2(RezaltGame.Width / 2, RezaltGame.Height / 2);

            float buttonScale = 0.8f;
            Vector2 backButtonPos = new Vector2(
                960 + (Back.Width * buttonScale)*3,
                Back.Height
            );
            
            backButton = new Button(Back, backButtonPos, Color.White, buttonScale);
            backButton.Click += OnBackButtonClicked;
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            BackToMenuClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Update(MouseState mouseState)
        {
            backButton.Update(mouseState);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Draw(BackGraund, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), new Color(0, 0, 0, 150));

            Vector2 screenCenter = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);

            Vector2 gameOverPos = screenCenter - new Vector2(GameOver.Width / 2, GameOver.Height / 2 + RezaltGame.Height / 2 + 50);
            spriteBatch.Draw(GameOver, gameOverPos, Color.White);

            Vector2 rezaltPanelPos = new Vector2(960 - RezaltGame.Width / 2, (int)(gameOverPos.Y * 2));
            spriteBatch.Draw(RezaltGame, rezaltPanelPos, Color.White);

            backButton.Draw(spriteBatch);

            string timeString = $"{GameScreen.ElapsedGameTime:mm\\:ss\\.fff}";

            Vector2 textSize = Screen.Font.MeasureString(timeString);

            Vector2 timePos = new Vector2(
                screenCenter.X - (textSize.X / 3), 
                rezaltPanelPos.Y + 40 
            );

            spriteBatch.DrawString(Screen.Font, timeString, timePos, Color.White, 0f, Vector2.Zero, 0.70f, SpriteEffects.None, 0f);
        }
    }
}
