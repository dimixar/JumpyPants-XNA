using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpyPants
{
    class EvilCatSurprise
    {
        #region Variabile

        public Animation CatAnimation;

        public Vector2 Position;

        public bool Active;

        public float catMoveSpeed;

        #endregion

        #region Inițializare

        public virtual void Initialization(Animation CatAnimation, Vector2 Position, float catMoveSpeed)
        {
            this.CatAnimation = CatAnimation;

            this.Position = Position;

            this.catMoveSpeed = catMoveSpeed;

            Active = true;

        }

        #endregion

        #region Update and Draw

        public virtual void Update(GameTime gameTime)
        {

            Position.X -= catMoveSpeed;

            CatAnimation.Position = Position;

            CatAnimation.Update(gameTime);


            if (Position.X < -CatAnimation.FrameSize.X)
            {
                Active = false;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            CatAnimation.Draw(spriteBatch);
        }

        #endregion
    }
}
