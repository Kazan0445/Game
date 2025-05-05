using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Project4
{
    public class MenuPause
    {
        public static Texture2D BackGraund { get; set; }
        public static Texture2D Monitor { get; set; }
        public static Texture2D BackToGame { get; set; }
        public static Texture2D BackToMenu { get; set; }

        private Button backToGameButton;
        private Button backToMenuButton;

        public event EventHandler ResumeGameClicked;
        public event EventHandler BackToMenuClicked;

        public MenuPause(int screenWidth, int screenHeight)
        {
            float pauseButtonScale = 1.0f;
            int pauseButtonSpacing = 20;
            Vector2 monitorCenter = new Vector2(screenWidth / 2, screenHeight / 2);

            Vector2 backToGamePos = monitorCenter - new Vector2(BackToGame.Width * pauseButtonScale / 2, (BackToGame.Height * pauseButtonScale / 2 + pauseButtonSpacing) / 2);
            backToGameButton = new Button(BackToGame, backToGamePos, Color.White, pauseButtonScale);
            backToGameButton.Click += OnResumeGameClicked;

            Vector2 backToMenuPos = monitorCenter - new Vector2(BackToMenu.Width * pauseButtonScale / 2, (-BackToMenu.Height * pauseButtonScale /2 - pauseButtonSpacing) * 2);
            backToMenuButton = new Button(BackToMenu, backToMenuPos, Color.White, pauseButtonScale);
            backToMenuButton.Click += OnBackToMenuClicked;
        }

        private void OnResumeGameClicked(object sender, EventArgs e)
        {
            ResumeGameClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnBackToMenuClicked(object sender, EventArgs e)
        {
            BackToMenuClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Update(MouseState mouseState)
        {
            backToGameButton.Update(mouseState);
            backToMenuButton.Update(mouseState);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            spriteBatch.Draw(BackGraund, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), new Color(0, 0, 0, 150));

            Vector2 monitorCenter = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            Vector2 monitorPos = monitorCenter - new Vector2(Monitor.Width / 2, Monitor.Height);
            spriteBatch.Draw(Monitor, monitorPos, Color.White);

            backToGameButton.Draw(spriteBatch);
            backToMenuButton.Draw(spriteBatch);
        }
    }
}
