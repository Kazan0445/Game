using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Xna.Framework.Media;

namespace Project4
{
    class GameScreen
    {
        public static int Width, Height;
        public static Random rnd = new Random();
        public static SpriteBatch SpriteBatch { get; set; }
        public static Texture2D BackGround { get; set; }
        public static Player Player { get; set; }
        public static SoundEffect GameActionSound { get; set; }
        public static Song BackgroundMusic { get; set; }
        static Color color;
        static List<BulletPlayer> bulletPlayer = new List<BulletPlayer>();
        static List<Zombie> zombies = new List<Zombie>();

        static List<KitHealth> kitHealthDrops = new List<KitHealth>();

        public static List<Explosion> explosions = new List<Explosion>();

        public static TimeSpan playerFireRate { get; set; } = TimeSpan.FromMilliseconds(500);
        public static readonly TimeSpan MinimumFireRate = TimeSpan.FromMilliseconds(50);
        private static TimeSpan playerFireTimer = TimeSpan.Zero;

        static TimeSpan zombieSpawnTimer = TimeSpan.Zero;
        static TimeSpan zombieSpawnInterval = TimeSpan.FromSeconds(1.5);

        public static TimeSpan ElapsedGameTime { get; private set; } = TimeSpan.Zero;

        public static int BulletCount { get; set; } = 1;

        static public int GetIntRandom(int min, int max)
        {
            return rnd.Next(min, max);
        }

        static public void PlayerFire()
        {
            if (playerFireTimer >= playerFireRate)
            {
                Vector2 muzzlePos = Gun.GetMuzzlePosition();
                Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                Vector2 baseDirection = mousePos - muzzlePos;
                if (baseDirection != Vector2.Zero)
                {
                    baseDirection.Normalize();
                }
                else
                {
                    baseDirection = new Vector2(0, -1);
                }

                if (BulletCount == 1)
                {
                    bulletPlayer.Add(new BulletPlayer(muzzlePos, baseDirection, 0));
                }
                else
                {
                    float totalSpreadAngle = 90f;
                    float angleStep = totalSpreadAngle / (BulletCount - 1);
                    float startAngle = -totalSpreadAngle / 2f;

                    for (int i = 0; i < BulletCount; i++)
                    {
                        float currentAngle = startAngle + i * angleStep;
                        bulletPlayer.Add(new BulletPlayer(muzzlePos, baseDirection, currentAngle));
                    }
                }

                playerFireTimer = TimeSpan.Zero;
            }
        }

        public static void PlayBackgroundMusic()
        {
            if (BackgroundMusic != null)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(BackgroundMusic);
            }
        }

        public static void StopBackgroundMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }
        }

        static public void Init(SpriteBatch SpriteBatch, int Width, int Height)
        {
            GameScreen.Width = Width;
            GameScreen.Height = Height;
            GameScreen.SpriteBatch = SpriteBatch;
            Player = new Player(new Vector2(960 - Player.NormalPlayerTexture.Width / 2, 960 - Player.NormalPlayerTexture.Height / 2));
            playerFireTimer = TimeSpan.Zero;
            zombies.Clear();
            bulletPlayer.Clear();
            kitHealthDrops.Clear();
            explosions.Clear();
            ElapsedGameTime = TimeSpan.Zero;
            zombieSpawnInterval = TimeSpan.FromSeconds(1.5);
            zombieSpawnTimer = TimeSpan.Zero;

            BulletCount = 1;
            playerFireRate = TimeSpan.FromMilliseconds(500);
            BulletPlayer.SpeedMultiplier = 1.2f;
            Upgrade.SlowModeActive = false;

            Level.ResetLevel();

            PlayBackgroundMusic();
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BackGround, new Rectangle(0, 0, 1920, 1080), color);
            Player.Draw();
            Gun.Draw();
            foreach (BulletPlayer bullet in bulletPlayer)
            {
                bullet.Draw();
            }
            foreach (var zombie in zombies)
            {
                zombie.Draw();
            }
            foreach (var explosion in explosions)
            {
                explosion.Draw(SpriteBatch);
            }
            foreach (var kit in kitHealthDrops)
            {
                kit.Draw(SpriteBatch);
            }
            HealthPlayer.Draw();
            Level.Draw();

            if (Upgrade.SlowModeActive)
            {
                spriteBatch.Draw(BackGround, new Rectangle(0, 0, Width, Height), new Color(0, 0, 0, 150));
                Upgrade.Draw(spriteBatch);
            }
        }

        public static void Update(GameTime gameTime)
        {
            TimeSpan effectiveElapsed = Upgrade.SlowModeActive ?
                TimeSpan.FromTicks(gameTime.ElapsedGameTime.Ticks / 10) :
                gameTime.ElapsedGameTime;

            color = Color.FromNonPremultiplied(255, 255, 255, 255);
            playerFireTimer += effectiveElapsed;
            ElapsedGameTime += effectiveElapsed;

            foreach (var zombie in zombies)
            {
                zombie.Update(gameTime);
            }

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
                    Rectangle zombieRect = zombie.GetBounds();
                    if (bulletRect.Intersects(zombieRect))
                    {
                        zombie.TakeDamage(34);
                        bulletPlayer.RemoveAt(i);
                        if (zombie.Health <= 0)
                        {
                            zombie.OnDeath(gameTime);
                            int expReward = 50;
                            if (zombie is BigZombie)
                                expReward = 50 * 4;
                            else if (zombie is BombZombie)
                                expReward = 50 * 6;
                            Level.AddExperience(expReward);
                            TryDropKit(zombie);
                            zombies.RemoveAt(j);
                        }
                        break;
                    }
                }
            }

            Rectangle playerRect = Player.GetBounds();
            for (int j = zombies.Count - 1; j >= 0; j--)
            {
                var zombie = zombies[j];
                Rectangle zombieRect = zombie.GetBounds();
                if (playerRect.Intersects(zombieRect))
                {
                    bool removeZombie = zombie.OnPlayerCollision(gameTime);
                    if (removeZombie)
                    {
                        if (zombie.Health <= 0)
                        {
                            zombie.OnDeath(gameTime);
                            int expReward = 50;
                            if (zombie is BigZombie)
                                expReward = 50 * 4;
                            else if (zombie is BombZombie)
                                expReward = 50 * 6;
                            Level.AddExperience(expReward);
                            TryDropKit(zombie);
                        }
                        zombies.RemoveAt(j);
                    }
                }
            }

            for (int i = 0; i < zombies.Count; i++)
            {
                for (int j = i + 1; j < zombies.Count; j++)
                {
                    var zombieA = zombies[i];
                    var zombieB = zombies[j];
                    Rectangle boundsA = zombieA.GetBounds();
                    Rectangle boundsB = zombieB.GetBounds();

                    if (boundsA.Intersects(boundsB))
                    {
                        Rectangle intersection = Rectangle.Intersect(boundsA, boundsB);
                        Vector2 centerA = new Vector2(boundsA.Center.X, boundsA.Center.Y);
                        Vector2 centerB = new Vector2(boundsB.Center.X, boundsB.Center.Y);
                        Vector2 direction = centerB - centerA;
                        float overlapX = intersection.Width;
                        float overlapY = intersection.Height;
                        Vector2 mtvDirection = (overlapX < overlapY) ? new Vector2(direction.X < 0 ? -1 : 1, 0) : new Vector2(0, direction.Y < 0 ? -1 : 1);
                        float mtvMagnitude = (overlapX < overlapY) ? overlapX / 2.0f : overlapY / 2.0f;
                        Vector2 mtv = mtvDirection * mtvMagnitude;
                        zombieA.AdjustPosition(-mtv);
                        zombieB.AdjustPosition(mtv);
                    }
                }
            }

            zombieSpawnTimer += effectiveElapsed;
            if (zombieSpawnTimer >= zombieSpawnInterval)
            {
                zombieSpawnTimer = TimeSpan.Zero;
                Zombie newZombie = null;
                if (ElapsedGameTime.TotalMinutes < 1)
                {
                    newZombie = new NormalZombie();
                }
                else if (ElapsedGameTime.TotalMinutes < 2)
                {
                    if (rnd.NextDouble() < 0.7)
                        newZombie = new NormalZombie();
                    else
                        newZombie = new BigZombie();
                }
                else
                {
                    double chance = rnd.NextDouble();
                    if (chance < 0.5)
                        newZombie = new NormalZombie();
                    else if (chance < 0.8)
                        newZombie = new BigZombie();
                    else
                        newZombie = new BombZombie();
                }

                if (newZombie != null)
                {
                    zombies.Add(newZombie);
                }

                double newIntervalMs = Math.Max(500, zombieSpawnInterval.TotalMilliseconds - 100);
                zombieSpawnInterval = TimeSpan.FromMilliseconds(newIntervalMs);
            }

            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                Explosion explosion = explosions[i];
                explosion.Update(gameTime);

                if (!explosion.DamageApplied)
                {
                    if (Player.GetBounds().Intersects(explosion.BoundingBox))
                    {
                        Player.TakeDamage(explosion.Damage);
                    }
                    for (int j = zombies.Count - 1; j >= 0; j--)
                    {
                        if (zombies[j].GetBounds().Intersects(explosion.BoundingBox))
                        {
                            zombies[j].TakeDamage(explosion.Damage);
                            if (zombies[j].Health <= 0)
                            {
                                zombies[j].OnDeath(gameTime);
                                int expReward = 50;
                                if (zombies[j] is BigZombie)
                                    expReward = 50 * 4;
                                else if (zombies[j] is BombZombie)
                                    expReward = 50 * 6;
                                Level.AddExperience(expReward);
                                zombies.RemoveAt(j);
                            }
                        }
                    }
                    explosion.DamageApplied = true;
                }

                if (explosion.IsFinished)
                    explosions.RemoveAt(i);
            }

            for (int i = kitHealthDrops.Count - 1; i >= 0; i--)
            {
                KitHealth kit = kitHealthDrops[i];
                if (Player.GetBounds().Intersects(kit.GetBounds()))
                {
                    Player.RestoreHealth(30);
                    kitHealthDrops.RemoveAt(i);
                }
            }

            if (Upgrade.SlowModeActive)
            {
                Upgrade.ProcessSelection();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                GameActionSound?.Play();
            }
        }

        static void TryDropKit(Zombie zombie)
        {
            double chance = 0;
            if (zombie is NormalZombie)
                chance = 0.10;
            else if (zombie is BigZombie)
                chance = 0.30;
            else
                return;

            if (rnd.NextDouble() < chance)
            {
                kitHealthDrops.Add(new KitHealth(zombie.Pos));
            }
        }

        public class Explosion
        {
            public Vector2 Position { get; }
            public Texture2D Texture { get; }
            public int Damage { get; }
            public float Duration { get; }
            private float elapsedTime;
            public bool DamageApplied { get; set; } = false;

            public Explosion(Vector2 position, Texture2D texture, int damage, float duration)
            {
                Position = position;
                Texture = texture;
                Damage = damage;
                Duration = duration;
                elapsedTime = 0;
            }

            public Rectangle BoundingBox =>
                new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            public void Update(GameTime gameTime)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            public bool IsFinished => elapsedTime >= Duration;

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(Texture, Position, Color.White);
            }
        }
    }

    class Player
    {
        public static Random RandomSpawn = new Random();
        public static Texture2D NormalPlayerTexture { get; set; }
        public static Texture2D DamagePlayerTexture { get; set; }
        Vector2 Pos;
        Color color = Color.White;
        public int HealthCount = 100;
        public int Health { get; private set; } = 100;

        public Player(Vector2 Pos)
        {
            this.Pos = Pos;
        }

        public Vector2 GetPosition() => Pos;

        public Vector2 getPosForPlayerBullet() => new Vector2(Pos.X + NormalPlayerTexture.Width / 7, (int)(Pos.Y + NormalPlayerTexture.Height / 3.25));

        public void Draw()
        {
            float centerX = Pos.X + NormalPlayerTexture.Height * 0.5f * 0.5f;
            int mouseX = Mouse.GetState().X;
            SpriteEffects spriteEffect = mouseX < centerX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            GameScreen.SpriteBatch.Draw(NormalPlayerTexture, Pos, null, color, 0, Vector2.Zero, 0.5f, spriteEffect, 0);
        }

        public void TakeDamage(int amount)
        {
            Health = Math.Max(Health - amount, 0);
        }

        public void RestoreHealth(int amount)
        {
            Health = Math.Min(Health + amount, HealthCount);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Pos.X, (int)Pos.Y, (int)(NormalPlayerTexture.Width * 0.5f), (int)(NormalPlayerTexture.Height * 0.5f));
        }

        public void Right()
        {
            if (this.Pos.X < GameScreen.Width - NormalPlayerTexture.Width * 0.4)
                this.Pos.X += 5;
        }

        public void Left()
        {
            if (this.Pos.X > 0)
                this.Pos.X -= 5;
        }

        public void Up()
        {
            if (this.Pos.Y > 0)
                this.Pos.Y -= 5;
        }

        public void Down()
        {
            if (this.Pos.Y < GameScreen.Height - NormalPlayerTexture.Width * 0.9)
                this.Pos.Y += 5;
        }
    }

    class Gun
    {
        public static Texture2D GunNormal { get; set; }
        public static Texture2D GunDamage { get; set; }

        public static void Draw()
        {
            Vector2 gunPos = GameScreen.Player.getPosForPlayerBullet();
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 direction = mousePos - gunPos;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            Vector2 origin = new Vector2(0, GunNormal.Height / 2);
            Color color = Color.White;
            SpriteEffects spriteEffect = mousePos.X < gunPos.X ? SpriteEffects.FlipVertically : SpriteEffects.None;
            if (mousePos.X < gunPos.X)
            {
                gunPos.X += Player.NormalPlayerTexture.Width / 4;
            }
            GameScreen.SpriteBatch.Draw(GunNormal, gunPos, null, color, rotation, origin, 0.5f, spriteEffect, 0);
        }

        public static Vector2 GetMuzzlePosition()
        {
            Vector2 gunPos = GameScreen.Player.getPosForPlayerBullet();
            Vector2 mousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Vector2 direction = mousePos - gunPos;
            if (direction != Vector2.Zero)
                direction.Normalize();
            return gunPos + direction * (GunNormal.Width * 0.5f);
        }
    }

    class BulletPlayer
    {
        public static Random RandomSpawn = new Random();
        private static float baseSpeed = 10f;
        public static float SpeedMultiplier { get; set; } = 1.0f;
        public static Texture2D Texture2D { get; set; }
        private Vector2 Pos;
        private Vector2 Dir;
        Color color = Color.Yellow;

        public BulletPlayer(Vector2 startPos, Vector2 targetPos)
        {
            this.Pos = startPos;
            this.Dir = targetPos - startPos;
            if (Dir != Vector2.Zero)
            {
                Dir.Normalize();
                Dir *= (baseSpeed * SpeedMultiplier);
            }
        }

        public BulletPlayer(Vector2 startPos, Vector2 baseDir, float angleOffsetDegrees)
        {
            this.Pos = startPos;
            float angleOffsetRadians = MathHelper.ToRadians(angleOffsetDegrees);
            float cos = (float)Math.Cos(angleOffsetRadians);
            float sin = (float)Math.Sin(angleOffsetRadians);
            this.Dir = new Vector2(
                baseDir.X * cos - baseDir.Y * sin,
                baseDir.X * sin + baseDir.Y * cos);

            if (Dir != Vector2.Zero)
            {
                Dir.Normalize();
                Dir *= (baseSpeed * SpeedMultiplier);
            }
            else
            {
                Dir = new Vector2(0, -1) * (baseSpeed * SpeedMultiplier);
            }
        }

        public Vector2 Position => Pos;

        public bool Hidden => (Pos.X > GameScreen.Width || Pos.Y > GameScreen.Height || Pos.X < 0 || Pos.Y < 0);

        public void Update()
        {
            Pos += Dir;
        }

        public void Draw()
        {
            GameScreen.SpriteBatch.Draw(Texture2D, Pos, color);
        }
    }

    abstract class Zombie
    {
        public static Random RandomSpawn = new Random();
        public static Texture2D BaseTexture { get; set; }
        public static Texture2D BigTexture { get; set; }
        public static Texture2D BombZombieTexture { get; set; }
        public static Texture2D BombTexture { get; set; }

        public Vector2 Pos { get; protected set; }
        public abstract int MaxHealth { get; }
        public int Health { get; protected set; }
        public abstract float BaseSpeed { get; }
        public virtual float CurrentSpeed => BaseSpeed;
        public abstract int Damage { get; }
        public abstract float Scale { get; }
        public abstract Texture2D Texture { get; }

        public TimeSpan LastDamageTime { get; set; } = TimeSpan.Zero;

        protected Zombie()
        {
            Health = MaxHealth;
            RandomSet();
        }

        protected virtual void RandomSet()
        {
            int spawnEdge = RandomSpawn.Next(0, 4);
            float x = 0, y = 0;
            int textureWidth = Texture.Width;
            int textureHeight = Texture.Height;
            float scaledWidth = textureWidth * Scale;
            float scaledHeight = textureHeight * Scale;

            switch (spawnEdge)
            {
                case 0:
                    x = RandomSpawn.Next(0, GameScreen.Width - (int)scaledWidth);
                    y = -scaledHeight;
                    break;
                case 1:
                    x = GameScreen.Width;
                    y = RandomSpawn.Next(0, GameScreen.Height - (int)scaledHeight);
                    break;
                case 2:
                    x = RandomSpawn.Next(0, GameScreen.Width - (int)scaledWidth);
                    y = GameScreen.Height;
                    break;
                case 3:
                    x = -scaledWidth;
                    y = RandomSpawn.Next(0, GameScreen.Height - (int)scaledHeight);
                    break;
            }
            Pos = new Vector2(x, y);
        }

        public virtual void Update(GameTime gameTime)
        {
            Vector2 playerPos = GameScreen.Player.GetPosition();
            Vector2 vectorToPlayer = playerPos - Pos;
            if (vectorToPlayer != Vector2.Zero)
            {
                vectorToPlayer.Normalize();
                Pos += vectorToPlayer * CurrentSpeed;
            }
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            Health = Math.Max(Health, 0);
        }

        public virtual void Draw()
        {
            SpriteEffects spriteEffect = (GameScreen.Player.GetPosition().X < Pos.X) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            GameScreen.SpriteBatch.Draw(Texture, Pos, null, Color.White, 0, Vector2.Zero, Scale, spriteEffect, 0);
        }

        public virtual Rectangle GetBounds()
        {
            return new Rectangle((int)Pos.X, (int)Pos.Y, (int)(Texture.Width * Scale), (int)(Texture.Height * Scale));
        }

        public void AdjustPosition(Vector2 adjustment)
        {
            Pos += adjustment;
        }

        public virtual void OnDeath(GameTime gameTime)
        {
        }

        public virtual bool OnPlayerCollision(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - LastDamageTime >= TimeSpan.FromSeconds(0.5))
            {
                GameScreen.Player.TakeDamage(this.Damage);
                LastDamageTime = gameTime.TotalGameTime;
            }
            return false;
        }
    }

    class NormalZombie : Zombie
    {
        public override int MaxHealth => 100;
        public override float BaseSpeed => 1f;
        public override int Damage => 10;
        public override float Scale => 0.5f;
        public override Texture2D Texture => BaseTexture;

        public NormalZombie() : base() { }
    }

    class BigZombie : Zombie
    {
        public override int MaxHealth => 300;
        public override float BaseSpeed => 0.5f;
        public override int Damage => 50;
        public override float Scale => 1f;
        public override Texture2D Texture => BigTexture;

        public BigZombie() : base() { }
    }

    class BombZombie : Zombie
    {
        public override int MaxHealth => 70;
        public override float BaseSpeed => 3f;
        public override int Damage => 100;
        public override float Scale => 0.8f;
        public override Texture2D Texture => BombZombieTexture;

        private bool exploded = false;

        public BombZombie() : base() { }

        private void CreateExplosion(GameTime gameTime)
        {
            if (exploded) return;
            exploded = true;
            GameScreen.explosions.Add(new GameScreen.Explosion(Pos, BombTexture, 100, 0.8f));
        }

        public override void OnDeath(GameTime gameTime)
        {
            CreateExplosion(gameTime);
            base.OnDeath(gameTime);
        }

        public override bool OnPlayerCollision(GameTime gameTime)
        {
            CreateExplosion(gameTime);
            return true;
        }

        public override void Draw()
        {
            if (exploded) return;
            base.Draw();
        }

        public override void Update(GameTime gameTime)
        {
            if (exploded) return;
            base.Update(gameTime);
        }
    }

    class KitHealth
    {
        public static Texture2D Texture2D { get; set; }
        public Vector2 Position { get; set; }

        public KitHealth(Vector2 position)
        {
            Position = position;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, Texture2D.Width / 5, Texture2D.Height / 5);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            GameScreen.SpriteBatch.Draw(Texture2D, Position, null, Color.White, 0, Vector2.Zero, 0.2f, SpriteEffects.None, 0);
        }
    }

    class HealthPlayer
    {
        public static Texture2D Texture2D_1 { get; set; }
        public static Texture2D Texture2D_2 { get; set; }

        public static void Draw()
        {
            Vector2 pos = new Vector2(50, 50);
            int barWidth = 480;
            int barHeight = 60;
            int maxHealth = GameScreen.Player.HealthCount;
            int currentHealth = GameScreen.Player.Health;
            int healthWidth = (int)(currentHealth / (float)maxHealth * barWidth);
            GameScreen.SpriteBatch.Draw(Texture2D_1, new Rectangle((int)pos.X, (int)pos.Y, barWidth, barHeight), Color.Brown);
            GameScreen.SpriteBatch.Draw(Texture2D_2, new Rectangle((int)pos.X + 25, (int)pos.Y + 15, healthWidth - 43, (int)(barHeight * 0.4f) + 3), Color.DarkRed);
        }
    }

    class Level
    {
        public static Texture2D Texture2D_1 { get; set; }
        public static Texture2D Texture2D_2 { get; set; }
        public static Texture2D Texture2D_3 { get; set; }

        private static int currentExperience = 0;
        private static int experienceToLevelUp = 300;
        private static int currentLevel = 1;

        public static void ResetLevel()
        {
            currentExperience = 0;
            experienceToLevelUp = 300;
            currentLevel = 1;
        }

        public static void AddExperience(int exp)
        {
            currentExperience += exp;
            while (currentExperience >= experienceToLevelUp)
            {
                currentExperience -= experienceToLevelUp;
                currentLevel++;
                experienceToLevelUp = (int)(experienceToLevelUp * 2);
                Upgrade.SlowModeActive = true;
                Upgrade.Initialize(GameScreen.Width, GameScreen.Height);
                GameScreen.Player.HealthCount += 10;
            }
        }

        public static void Draw()
        {
            Vector2 expBarPos = new Vector2(50, 50);
            int barWidth = 480;
            int barHeight = 60;
            GameScreen.SpriteBatch.Draw(Texture2D_1, new Rectangle((int)expBarPos.X, (int)(expBarPos.Y * 2.5), barWidth, barHeight), Color.Gray);
            int filledWidth = (int)((float)currentExperience / experienceToLevelUp * barWidth);
            filledWidth = Math.Min(filledWidth, barWidth - 20);
            GameScreen.SpriteBatch.Draw(Texture2D_2, new Rectangle((int)expBarPos.X + 10, (int)(expBarPos.Y * 2.5 + 10), filledWidth, (int)(barHeight / 1.5) - 5), Color.Blue);

            Vector2 levelIconPos = new Vector2(expBarPos.X + barWidth + 10, expBarPos.Y * 1.5f + barHeight / 2f - Texture2D_3.Height / 2f);
            GameScreen.SpriteBatch.Draw(Texture2D_3, levelIconPos, Color.White);

            string levelText = currentLevel.ToString();
            Vector2 textSize = Screen.Font.MeasureString(levelText);
            Vector2 textPosition = new Vector2(
                levelIconPos.X + (Texture2D_3.Width / 2f) - (textSize.X / 2f),
                levelIconPos.Y + (Texture2D_3.Height / 2.5f) - (textSize.Y / 2f)
            );
            GameScreen.SpriteBatch.DrawString(Screen.Font, levelText, textPosition, Color.Black);
        }
    }

    class Upgrade
    {
        public static Texture2D RatePlus { get; set; }
        public static Texture2D CountPlus { get; set; }

        public static Rectangle RateRect { get; set; }
        public static Rectangle CountRect { get; set; }

        public static bool SlowModeActive { get; set; } = false;

        public static void Initialize(int screenWidth, int screenHeight)
        {
            RateRect = new Rectangle(screenWidth / 2 - 250, screenHeight / 2 - 50, 200, 100);
            CountRect = new Rectangle(screenWidth / 2 + 50, screenHeight / 2 - 50, 200, 100);
        }

        public static void ProcessSelection()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Point mousePoint = new Point(mouseState.X, mouseState.Y);
                if (RateRect.Contains(mousePoint))
                {
                    BulletPlayer.SpeedMultiplier *= 1.5f;
                    TimeSpan reduction = TimeSpan.FromMilliseconds(120);
                    TimeSpan newRate = GameScreen.playerFireRate - reduction;
                    GameScreen.playerFireRate = newRate < GameScreen.MinimumFireRate ? GameScreen.MinimumFireRate : newRate;
                    SlowModeActive = false;
                }
                else if (CountRect.Contains(mousePoint))
                {
                    GameScreen.BulletCount++;
                    SlowModeActive = false;
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(RatePlus, RateRect, Color.White);
            spriteBatch.Draw(CountPlus, CountRect, Color.White);
        }
    }
}
