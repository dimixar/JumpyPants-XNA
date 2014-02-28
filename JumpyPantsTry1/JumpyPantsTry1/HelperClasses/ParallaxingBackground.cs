using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JumpyPants
{
    class ParallaxingBackground
    {

        #region Variabile

        Texture2D texture;

        Vector2[] positions;

        float speed;

        String texturePath;

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        #endregion

        #region Inițializare

        public void Initialize(ContentManager content, String texturePath, int ScreenWidth, float speed)
        {
            texture = content.Load<Texture2D>(texturePath);

            this.speed = speed;
            this.texturePath = texturePath;

            positions = new Vector2[ScreenWidth / texture.Width + 1000];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(i * texture.Width, 0);
            }

        }

        #endregion

        #region Update and Draw

        public void Update()
        {

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].X -= speed;

                if (texturePath == "BackgroundImages/ground")
                {
                    positions[i].Y = 720 - 50;
                }

                    if (positions[i].X <= -texture.Width)
                    {
                        positions[i].X = texture.Width * (positions.Length - 1);
                    }

            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                spriteBatch.Draw(texture, positions[i], Color.White);
            }
        }

        #endregion

    }
}
