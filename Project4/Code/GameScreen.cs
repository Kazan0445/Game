using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Project4
{
    class GameScreen
    {
        public static int Width, Height;
        public static Random rnd = new Random();
        public static SpriteBatch SpriteBatch { get; set; }
        public static Texture2D BackGround { get; set; }
        public static Player Player { get; set; }
        static Color color;
        static List<BulletPlayer> bulletPlayer = new List<BulletPlayer>();
        static List<Zombie> zombies = new List<Zombie>();
        private static TimeSpan playerFireRate = TimeSpan.FromMilliseconds(200);
        private static TimeSpan playerFireTimer = TimeSpan.Zero;

        static public int GetIntRandom(int min, int max)
        {
            return rnd.Next(min, max);
        }

        static public void PlayerFire()
        {
            if (playerFireTimer >= playerFireRate)
            {
                bulletPlayer.Add(new BulletPlayer(Player.getPosForPlayerBullet()));
                playerFireTimer = TimeSpan.Zero;
            }
        }

        static public void Init(SpriteBatch SpriteBatch, int Width, int Height)
        {
            GameScreen.Width = Width;
            GameScreen.Height = Height;
            GameScreen.SpriteBatch = SpriteBatch;
            Player = new Player(new Vector2(960 - Player.Texture2D.Width / 2, 960 - Player.Texture2D.Height / 2));
            playerFireTimer = TimeSpan.Zero;
            for (int i = 0; i < 20; i++)
            {
                zombies.Add(new Zombie());
            }
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackGround, new Rectangle(0, 0, 1920, 1080), color);
            Player.Draw();
            foreach (BulletPlayer bullet in bulletPlayer)
            {
                bullet.Draw();
            }
            foreach (var zombie in zombies)
            {
                zombie.Draw();
            }
        }

        public static void Update(GameTime gameTime)
        {
            color = Color.FromNonPremultiplied(255, 255, 255, 255);
            playerFireTimer += gameTime.ElapsedGameTime;
            for (int i = bulletPlayer.Count - 1; i >= 0; i--)
            {
                bulletPlayer[i].Update();
                if (bulletPlayer[i].Hidden)
                {
                    bulletPlayer.RemoveAt(i);
                }
            }

            for (int i = bulletPlayer.Count - 1; i >= 0; i--)
            {
                var bullet = bulletPlayer[i];
                Rectangle bulletRect = new Rectangle((int)bullet.Position.X, (int)bullet.Position.Y, BulletPlayer.Texture2D.Width, BulletPlayer.Texture2D.Height);
                for (int j = zombies.Count - 1; j >= 0; j--)
                {
                    var zombie = zombies[j];
                    Rectangle zombieRect = new Rectangle((int)zombie.Pos.X, (int)zombie.Pos.Y, Zombie.Texture2D.Width, Zombie.Texture2D.Height);
                    if (bulletRect.Intersects(zombieRect))
                    {
                        zombie.TakeDamage(10);
                        bulletPlayer.RemoveAt(i);
                        break; 
                    }
                }
            }

            for (int j = zombies.Count - 1; j >= 0; j--)
            {
                if (zombies[j].Health <= 0)
                {
                    zombies.RemoveAt(j);
                }
            }

            foreach (var zombie in zombies)
            {
                zombie.Update();
            }
        }
    }

    class Player
    {
        public static Random RandomSpawn = new Random();
        public static Texture2D Texture2D { get; set; }
        Vector2 Pos;
        Color color = Color.White;

        public Player(Vector2 Pos)
        {
            this.Pos = Pos;
        }

        public Vector2 GetPosition() => Pos;

        public Vector2 getPosForPlayerBullet() => new Vector2(Pos.X + Texture2D.Width / 2 - 10, Pos.Y + Texture2D.Height / 4);

        public void Draw()
        {
            GameScreen.SpriteBatch.Draw(Texture2D, Pos, null, color, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
        }

        public void Right()
        {
            if (this.Pos.X < GameScreen.Width - Texture2D.Width * 0.4) this.Pos.X += 5;
        }

        public void Left()
        {
            if (this.Pos.X > 0) this.Pos.X -= 5;
        }

        public void Up()
        {
            if (this.Pos.Y > 0) this.Pos.Y -= 5;
        }

        public void Down()
        {
            if (this.Pos.Y < GameScreen.Height - Texture2D.Width * 0.9) this.Pos.Y += 5;
        }
    }

    class BulletPlayer
    {
        public static Random RandomSpawn = new Random();
        private float speed = 10f;
        public static Texture2D Texture2D { get; set; }
        private Vector2 Pos;
        private Vector2 Dir;
        Color color = Color.Yellow;
        private Vector2 mousePos;

        public BulletPlayer(Vector2 Pos)
        {
            this.Pos = Pos;
            this.mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            this.Dir = mousePos - Pos;
            if (Dir != Vector2.Zero)
            {
                Dir.Normalize();
                Dir *= speed;
            }
        }

        public Vector2 Position => Pos;

        public bool Hidden => (Pos.X > GameScreen.Width || Pos.Y > GameScreen.Height ||
                             Pos.X < 0 || Pos.Y < 0);

        public void Update()
        {
            Pos += Dir;
        }

        public void Draw()
        {
            GameScreen.SpriteBatch.Draw(Texture2D, Pos, color);
        }
    }

    class Zombie
    {
        public static Random RandomSpawn = new Random();
        public static Texture2D Texture2D { get; set; }
        public float Scale { get; set; }
        public Vector2 Pos { get; internal set; }
        private Vector2 Dir;
        private float speed = 15f;
        Color color;

        public int Health { get; private set; } = 100;

        public Zombie()
        {
            RandomSet();
        }

        public void Update()
        {
            Vector2 playerPos = GameScreen.Player.GetPosition();
            if ((playerPos - Pos).Length() > 0)
            {
                Dir = (playerPos - Pos) * speed;
            }
            else
            {
                Dir = Vector2.Zero;
            }

            Dir.Normalize();
            Pos += Dir;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        public void RandomSet()
        {
            int zombieSpawnX = RandomSpawn.Next(-500, GameScreen.Width - 500);
            int zombieSpawnY = RandomSpawn.Next(-500, GameScreen.Height - 500);
            Pos = new Vector2(zombieSpawnX, zombieSpawnY);
            color = Color.White;
        }

        public void Draw()
        {
            GameScreen.SpriteBatch.Draw(Texture2D, Pos, color);
        }
    }
}