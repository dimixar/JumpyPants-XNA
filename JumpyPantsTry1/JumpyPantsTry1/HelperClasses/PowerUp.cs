using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace JumpyPants
{
    class PowerUp
    {
        #region Variabile și Proprietăți

        Texture2D powerUpTexture;

        public Vector2 position;

        int powerUpIndex;

        public float powerUpMoveSpeed;

        bool goUp;

        public bool Active;

        float floatingPosition;

        Rectangle sourceRect;

        #endregion

        #region Inițializare

        /// <summary>
        /// Inițializăm textura, poziția și powerUp-ul necesar
        /// </summary>
        /// <param name="texture">Textura de powerUp-uri</param>
        /// <param name="position">poziția powerUp-ului</param>
        /// <param name="powerUpIndex">0 = Health Gain; 1 = Slow Down</param>
        public void Initialize(Texture2D texture, Vector2 position, int powerUpIndex, float powerUpMoveSpeed)
        {
            powerUpTexture = texture;
            this.position = position;
            this.powerUpIndex = powerUpIndex;
            this.powerUpMoveSpeed = powerUpMoveSpeed;
            goUp = true;
            Active = true;
            floatingPosition = position.Y;
        }

        #endregion

        #region Update and Draw

        public void Update(GameTime gameTime)
        {
            position.X -= powerUpMoveSpeed;

            if (goUp)
            {
                if (position.Y > floatingPosition - 100)
                {
                    position.Y -= 1f;
                }
                else
                {
                    goUp = false;
                }
            }
            else
            {
                if (position.Y <= floatingPosition + 100)
                {
                    if (position.Y + 75 < 720 - 50)
                        position.Y += 1f;
                    else
                        goUp = true;
                }
                else
                {
                    goUp = true;
                }
            }

            if (position.X < -powerUpTexture.Width)
            {
                Active = false;
            }

            sourceRect = new Rectangle(powerUpIndex * powerUpTexture.Width / 2, 0, powerUpTexture.Width / 2, powerUpTexture.Height);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(powerUpTexture, position, sourceRect, Color.White);
        }

        #endregion

        public bool WhichPowerUp(int index)
        {
            if (index == powerUpIndex)
            {
                return true;
            }

            return false;
        }

    }
}
