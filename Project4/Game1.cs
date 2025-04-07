using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project4
{
    enum StatOfScreen
    {
        ThrstMenuScreen,
        SecondMenuScreen,
        Game,
        EndOfGame,
        Pause
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        StatOfScreen StatOfScreen = StatOfScreen.ThrstMenuScreen;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Screen.BackGraund = Content.Load<Texture2D>("BackGroundMenu");
            GameScreen.BackGround = Content.Load<Texture2D>("BackGroundGame");
            BulletPlayer.Texture2D = Content.Load<Texture2D>("BulletTexture");
            Player.Texture2D = Content.Load<Texture2D>("Player");
            Screen.Font = Content.Load<SpriteFont>("SpriteFont1");
            GameScreen.Init(_spriteBatch, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        }

        protected override void Update(GameTime gameTime)
        {

            var keyState = Keyboard.GetState();

            switch (StatOfScreen)
            {
                case StatOfScreen.ThrstMenuScreen:
                    Screen.Update();
                    if (keyState.IsKeyDown(Keys.Space) && Keyboard.GetState().IsKeyUp(Keys.Enter)) StatOfScreen = StatOfScreen.Game;
                    break;
                case StatOfScreen.Game:
                    Screen.Update();
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed) GameScreen.PlayerFire();
                    if (keyState.IsKeyDown(Keys.Escape)) StatOfScreen = StatOfScreen.ThrstMenuScreen;
                    if (keyState.IsKeyDown(Keys.W)) GameScreen.Player.Up();
                    if (keyState.IsKeyDown(Keys.S)) GameScreen.Player.Down();
                    if (keyState.IsKeyDown(Keys.A)) GameScreen.Player.Left();
                    if (keyState.IsKeyDown(Keys.D)) GameScreen.Player.Right();
                        break;
                //case StatOfScreen.SecondScreenMenu:
                //    Screen.Update();
                //    break;
                //case StatOfScreen.Pause:
                //    Screen.Update();
                //    if (Keyboard.GetState().IsKeyUp(Keys.Escape)) StatOfScreen = StatOfScreen.Game;
                //    break;
                //case StatOfScreen.EndOfGame:
                //    if (Keyboard.GetState().IsKeyUp(Keys.Escape)) StatOfScreen = StatOfScreen.ThirstScreenMenu;
                //    Screen.Update();
                //    break;

            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyState.IsKeyDown(Keys.Escape))
                Exit();
            Screen.Update();
            //Bullet.Update();
            GameScreen.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            switch (StatOfScreen)
            {
                case StatOfScreen.ThrstMenuScreen:
                    Screen.Draw(_spriteBatch);
                    break;
                    case StatOfScreen.Game:
                    //Bullet.Draw(_spriteBatch);
                    GameScreen.Draw(_spriteBatch);
                    break;
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Draw(Texture2D backGround)
        {
            throw new NotImplementedException();
        }
    }
}
