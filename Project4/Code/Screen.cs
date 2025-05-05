using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Project4
{
    public class Screen
    {
        public static Texture2D BackGraund { get; set; }
        public static Texture2D PlayTexture { get; set; }
        public static Texture2D AchievementsTexture { get; set; }
        public static Texture2D UnderName { get; set; }
        public static SpriteFont Font { get; set; }

        private static Song backgroundSong;
        public static Song BackgroundSong
        {
            get => backgroundSong;
            set => backgroundSong = value;
        }

        private List<Button> buttons = new();

        public event EventHandler PlayClicked;
        public event EventHandler AchievementsClicked;

        static int timeCounter = 0;
        static Color color;

        public Screen(int screenWidth, int screenHeight)
        {
            int buttonYStart = screenHeight / 2 - PlayTexture.Height;
            int buttonSpacing = 30;
            float buttonScale = 1.0f;

            Vector2 playButtonPos = new Vector2(screenWidth / 2 - (PlayTexture.Width * buttonScale) / 2, buttonYStart);
            Button playButton = new Button(PlayTexture, playButtonPos, Color.White, buttonScale);
            playButton.Click += OnPlayClicked;
            buttons.Add(playButton);

            Vector2 achieveButtonPos = new Vector2(screenWidth / 2 - (AchievementsTexture.Width * buttonScale) / 2, buttonYStart + (PlayTexture.Height * buttonScale) + buttonSpacing);
            Button achieveButton = new Button(AchievementsTexture, achieveButtonPos, Color.White, buttonScale);
            achieveButton.Click += OnAchievementsClicked;
            buttons.Add(achieveButton);

            if (backgroundSong != null && MediaPlayer.State != MediaState.Playing)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.1f;
                MediaPlayer.Play(backgroundSong);
            }
        }

        private void OnPlayClicked(object sender, EventArgs e)
        {
            MediaPlayer.Stop();
            PlayClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnAchievementsClicked(object sender, EventArgs e)
        {
            AchievementsClicked?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackGraund, new Rectangle(0, 0, 1920, 1080), color);
            spriteBatch.Draw(UnderName, new Rectangle(960 - UnderName.Width / 2, UnderName.Height / 2, UnderName.Width, UnderName.Height), color);
            spriteBatch.DrawString(Font, "Zombie Rush", new Vector2(960 - (int)(UnderName.Width / 2.6), (int)(UnderName.Height / 1.5)), Color.DarkRed);

            foreach (var btn in buttons)
            {
                btn.Draw(spriteBatch);
            }
        }

        public void Update(MouseState mouseState)
        {
            if (timeCounter < 255)
            {
                color = Color.FromNonPremultiplied(255, 255, 255, timeCounter);
                timeCounter += 2;
                if (timeCounter > 255) timeCounter = 255;
            }
            else
            {
                color = Color.White;
            }

            foreach (var btn in buttons)
            {
                btn.Update(mouseState);
            }
        }
    }
}
