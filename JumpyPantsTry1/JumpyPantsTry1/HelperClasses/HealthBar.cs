using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace JumpyPants
{
    class HealthBar
    {

        Texture2D texture;

        Vector2 position;

        public int healthBarIndex;

        Rectangle sourceRect;

        public void Initialize(Texture2D texture, Vector2 position, int healthBarIndex)
        {
            this.texture = texture;
            this.position = position;
            this.healthBarIndex = healthBarIndex;
        }


        public void Update()
        {
            sourceRect = new Rectangle(healthBarIndex * texture.Width / 4, 0, texture.Width / 4, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, sourceRect, Color.White);
        }
    }
}
