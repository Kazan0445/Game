using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project4
{
    public class Button
    {
        private Texture2D texture;
        private Vector2 position;
        private Color defaultColor;
        private Color hoverColor;
        private Color currentColor;
        private bool isHovering;
        private MouseState previousMouseState;
        private float scale;

        public Rectangle Bounds { get; private set; }
        public event EventHandler Click;

        public Button(Texture2D texture, Vector2 position, Color color, float scale = 1.0f)
        {
            this.texture = texture;
            this.position = position;
            this.defaultColor = color;
            this.hoverColor = new Color(Math.Max(0, color.R - 30), Math.Max(0, color.G - 30), Math.Max(0, color.B - 30), color.A);
            this.currentColor = this.defaultColor;
            this.scale = scale;
            Bounds = new Rectangle((int)position.X, (int)position.Y, (int)(texture.Width), (int)(texture.Height));
        }

        public void Update(MouseState currentMouseState)
        {
            isHovering = Bounds.Contains(currentMouseState.Position);
            currentColor = isHovering ? hoverColor : defaultColor;

            if (isHovering && currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released)
            {
                Click?.Invoke(this, EventArgs.Empty);
            }

            previousMouseState = currentMouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, currentColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}
