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

        static public int GetIntRandom(int min, int max)
        {
            return rnd.Next(min, max);
        }

        static public void PlayerFire()
        {
            bulletPlayer.Add(new BulletPlayer(Player.getPosForPlayerBullet()));
        }

        static public void Init(SpriteBatch SpriteBatch, int Width, int Height)
        {
            GameScreen.Width = Width;
            GameScreen.Height = Height;
            GameScreen.SpriteBatch = SpriteBatch;
            //bullets = new Bullet[100];
            //for (int i = 0; i < bullets.Length; i++)
            //{
            //    bullets[i] = new Bullet(new Vector2(-rnd.Next(1,10),0));
            //}
            Player = new Player(new Vector2(960 - Player.Texture2D.Width, 960 - Player.Texture2D.Height));
        }
        
        static public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackGround, new Rectangle(0, 0, 1920, 1080), color);
            Player.Draw();
            foreach (BulletPlayer bullet in bulletPlayer)
            {
                bullet.Draw();
            }
        }
        public static void Update ()
        {
            color = Color.FromNonPremultiplied(255, 255, 255, 255);
            for (int i = 0; i < bulletPlayer.Count; i++)
            {
                bulletPlayer[i].Update();
                if (bulletPlayer[i].Hidden)
                {
                    bulletPlayer.RemoveAt(i);
                    i--;
                }
            }
            //foreach (Bullet bullet in bullets)
            //{
            //    bullet.Update();
            //}
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

        public Vector2 getPosForPlayerBullet() => new Vector2(Pos.X + Texture2D.Width/2 -10, Pos.Y + Texture2D.Height/4);
        public void Draw()
        {
            GameScreen.SpriteBatch.Draw(Texture2D, Pos, null, color,0,Vector2.Zero,0.5f, SpriteEffects.None,0);
        }


        public void Right()
        {
            if (this.Pos.X < GameScreen.Width - Texture2D.Width*0.4) this.Pos.X += 15;
        }

        public void Left()
        {
            if (this.Pos.X > 0) this.Pos.X -= 15;
        }

        public void Up()
        {
            if (this.Pos.Y > 0) this.Pos.Y -= 15;
        }

        public void Down()
        {
            if (this.Pos.Y < GameScreen.Height - Texture2D.Width*0.9 )  this.Pos.Y += 15;
        }

       
    }
    class BulletPlayer
    {
        public static Random RandomSpawn = new Random();
        private float speed = 10f;
        public static Texture2D Texture2D { get; set; }
        Vector2 Pos;
        Vector2 Dir;
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
    //class Bullet
    //{
    //    public static Random RandomSpawn = new Random();
    //    public static Texture2D Texture2D { get; set; }
    //    Vector2 Pos;
    //    Vector2 Dir;
    //    Color color;

    //    public Bullet(Vector2 Pos, Vector2 Dir)
    //    {
    //        this.Pos = Pos;
    //        this.Dir = Dir;
    //    }

    //    public Bullet(Vector2 Dir)
    //    {
    //        this.Dir = Dir;
    //        RandomSet();
    //    }

    //    public void Update()
    //    {
    //        Pos += Dir;
    //        if (Pos.X < 0 || Pos.X > GameScreen.Width || Pos.Y < 0 || Pos.Y > GameScreen.Height) RandomSet();
    //        color = Color.FromNonPremultiplied(255, 255, 255, 255);
    //    }

    //    public void RandomSet()
    //    {
    //        Pos = new Vector2(GameScreen.rnd.Next(GameScreen.Width, GameScreen.Width + 300), GameScreen.rnd.Next(GameScreen.Height));
    //        color = Color.FromNonPremultiplied(GameScreen.GetIntRandom(0, 255), GameScreen.GetIntRandom(0, 255), GameScreen.GetIntRandom(0, 255), 255);
    //    }

    //    public void Draw()
    //    {
    //        GameScreen.SpriteBatch.Draw(Texture2D, Pos, color);
    //    }

    //}

}

