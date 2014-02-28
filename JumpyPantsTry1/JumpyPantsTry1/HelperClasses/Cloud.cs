using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpyPants
{
    class Cloud
    {

        #region Variabile și Proprietăți

        public Vector2 Position;

        Texture2D cloudTexture;

        public bool Active;

        float scale;

        public int Width
        {
            get { return cloudTexture.Width; }
        }

        public int Height
        {
            get { return cloudTexture.Height; }
        }

        public float cloudMoveSpeed;

        #endregion

        #region Inițializare

        public void Initialize(Texture2D cloudTexture, Vector2 Position, float scale, float cloudMoveSpeed)
        {
            this.cloudTexture = cloudTexture;
            this.Position = Position;
            this.cloudMoveSpeed = cloudMoveSpeed;
            this.scale = scale;

            Active = true;
        }

        public void Initialize(Texture2D cloudTexture, Vector2 Position, float scale)
        {
            this.cloudTexture = cloudTexture;
            this.Position = Position;
            this.scale = scale;

            Active = true;
        }

        #endregion

        #region Update and Draw

        public void UpdateLeft(GameTime gameTime)
        {
            Position.X -= cloudMoveSpeed;

            if (Position.X < -Width)
                Active = false;
        }

        public void UpdateRight(GameTime gameTime)
        {
            Position.X += cloudMoveSpeed;

            if (Position.X > 1280)
                Active = false;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(cloudTexture, Position, null, Color.White, 0 , Vector2.Zero, scale, spriteEffects, 0);
        }

        #endregion

        public float Scale()
        {
            int flag = new Random().Next(0, 6);

            switch (flag)
            {
                case 0:
                    {
                        cloudMoveSpeed = 1.7f;
                        return 0.6f;
                    }
                case 1:
                    {
                        cloudMoveSpeed = 1.4f;
                        return 0.9f;
                    }
                case 2:
                    {
                        cloudMoveSpeed = 1.2f;
                        return 1f;
                    }
                case 3:
                    {
                        cloudMoveSpeed = 0.7f;
                        return 1.5f;
                    }
                case 4:
                    {
                        cloudMoveSpeed = 1.1f;
                        return 1.2f;
                    }
                default: { cloudMoveSpeed = 1.2f; return 1f; }
            }
        }

    }
}
