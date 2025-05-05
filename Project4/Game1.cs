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
        public GameWindow Window { get; }
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        StatOfScreen StatOfScreen = StatOfScreen.ThrstMenuScreen;

        private Screen menuScreen;
        
        private MenuPause menuPause;
        
        private Rezalt rezaltScreen;
        
        private KeyboardState previousKeyState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();
            base.Initialize();
        }

                protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Screen.BackGraund = Content.Load<Texture2D>("BackGroundMenu");
            Screen.PlayTexture = Content.Load<Texture2D>("Play1");
            Screen.UnderName = Content.Load<Texture2D>("UnderName");
            Screen.AchievementsTexture = Content.Load<Texture2D>("Achievements1");
            KitHealth.Texture2D = Content.Load<Texture2D>("Kit");
            GameScreen.BackGround = Content.Load<Texture2D>("BackGroundGame");
            BulletPlayer.Texture2D = Content.Load<Texture2D>("BulletTexture");
            Player.NormalPlayerTexture = Content.Load<Texture2D>("PlayerNormal");
            Zombie.BaseTexture = Content.Load<Texture2D>("ZombieDefault");
            Zombie.BigTexture = Content.Load<Texture2D>("BigZombie1");
            Zombie.BombZombieTexture = Content.Load<Texture2D>("BombZombie");
            Zombie.BombTexture = Content.Load<Texture2D>("Boom");
            Gun.GunNormal = Content.Load<Texture2D>("GunNormal");
            HealthPlayer.Texture2D_1 = Content.Load<Texture2D>("HP_Bar_1");
            HealthPlayer.Texture2D_2 = Content.Load<Texture2D>("HP_Bar_2");
            Level.Texture2D_1 = Content.Load<Texture2D>("Background");
            Level.Texture2D_2 = Content.Load<Texture2D>("Line");
            Level.Texture2D_3 = Content.Load<Texture2D>("Hover@2x-1");
            Upgrade.CountPlus = Content.Load<Texture2D>("CountPlus");
            Upgrade.RatePlus = Content.Load<Texture2D>("RatePlus");
            Screen.Font = Content.Load<SpriteFont>("SpriteFont1");
            Rezalt.BackGraund = Content.Load<Texture2D>("WhiteFlash");
            Rezalt.Back = Content.Load<Texture2D>("Back");
            Rezalt.GameOver = Content.Load<Texture2D>("GameOver");
            Rezalt.RezaltGame = Content.Load<Texture2D>("Rezalt");
            MenuPause.BackGraund = Content.Load<Texture2D>("WhiteFlash"); 
            MenuPause.Monitor = Content.Load<Texture2D>("Monitor Pause");       
            MenuPause.BackToGame = Content.Load<Texture2D>("Continue");     
            MenuPause.BackToMenu = Content.Load<Texture2D>("BackToMenu");     

            GameScreen.Init(spriteBatch, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            int screenWidth = graphics.PreferredBackBufferWidth;
            int screenHeight = graphics.PreferredBackBufferHeight;

            menuScreen = new Screen(screenWidth, screenHeight);
            menuScreen.PlayClicked += MenuScreen_PlayClicked;
            menuScreen.AchievementsClicked += MenuScreen_AchievementsClicked;
            menuPause = new MenuPause(screenWidth, screenHeight);

            menuPause.ResumeGameClicked += MenuPause_ResumeGameClicked;
            menuPause.BackToMenuClicked += MenuPause_BackToMenuClicked;

            rezaltScreen = new Rezalt(screenWidth, screenHeight);
            rezaltScreen.BackToMenuClicked += RezaltScreen_BackToMenuClicked;
        }

        private void MenuScreen_PlayClicked(object sender, EventArgs e)
        {
            StatOfScreen = StatOfScreen.Game; 
        }

        private void MenuScreen_AchievementsClicked(object sender, EventArgs e)
        {
            
            Console.WriteLine("Achievements button clicked! (Handled in Game1)"); 
        }

        private void MenuPause_ResumeGameClicked(object sender, EventArgs e)
        {
            StatOfScreen = StatOfScreen.Game; 
        }

        private void MenuPause_BackToMenuClicked(object sender, EventArgs e)
        {
            StatOfScreen = StatOfScreen.ThrstMenuScreen; 
            
            GameScreen.Init(spriteBatch, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight); 
        }

        private void RezaltScreen_BackToMenuClicked(object sender, EventArgs e)
        {
            StatOfScreen = StatOfScreen.ThrstMenuScreen; 
            
            GameScreen.Init(spriteBatch, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight); 
        }

        protected override void Update(GameTime gameTime)
        {
            var keyState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) 
                 Exit();
            switch (StatOfScreen)
            {
                case StatOfScreen.ThrstMenuScreen:
                    menuScreen.Update(mouseState);
                    if (keyState.IsKeyDown(Keys.Escape))
                         Exit();
                    break;
                case StatOfScreen.Game:
                    
                     if (keyState.IsKeyDown(Keys.Escape) && !previousKeyState.IsKeyDown(Keys.Escape)) 
                    {
                        StatOfScreen = StatOfScreen.Pause;
                    }
                    else 
                    {
                        GameScreen.Update(gameTime);
                        if (mouseState.LeftButton == ButtonState.Pressed) GameScreen.PlayerFire();
                        if (keyState.IsKeyDown(Keys.W)) GameScreen.Player.Up();
                        if (keyState.IsKeyDown(Keys.S)) GameScreen.Player.Down();
                        if (keyState.IsKeyDown(Keys.A)) GameScreen.Player.Left();
                        if (keyState.IsKeyDown(Keys.D)) GameScreen.Player.Right();
                        if (GameScreen.Player.Health <= 0)
                        {
                            StatOfScreen = StatOfScreen.EndOfGame;
                        }
                    }
                    break;
                case StatOfScreen.Pause: 
                      if (keyState.IsKeyDown(Keys.Escape) && !previousKeyState.IsKeyDown(Keys.Escape)) 
                     {
                         StatOfScreen = StatOfScreen.Game; 
                     }
                     else 
                     {
                         menuPause.Update(mouseState); 
                     }
                     break;
                case StatOfScreen.EndOfGame: 
                    rezaltScreen.Update(mouseState);
                    break;
            }
            previousKeyState = keyState; 
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

             spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend); 
            switch (StatOfScreen)
            {
                case StatOfScreen.ThrstMenuScreen:
                    GraphicsDevice.Clear(Color.CornflowerBlue); 
                    menuScreen.Draw(spriteBatch);
                    break;
                case StatOfScreen.Game:
                    GraphicsDevice.Clear(Color.Black); 
                    GameScreen.Draw(spriteBatch);
                    break;
                case StatOfScreen.Pause: 
                     GameScreen.Draw(spriteBatch);
                     menuPause.Draw(spriteBatch, GraphicsDevice);
                     break;
                case StatOfScreen.EndOfGame: 
                    GameScreen.Draw(spriteBatch); 
                    rezaltScreen.Draw(spriteBatch, GraphicsDevice);
                    break;
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
