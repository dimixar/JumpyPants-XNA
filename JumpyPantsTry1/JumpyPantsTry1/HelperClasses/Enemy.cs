using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpyPants
{
    class Enemy
    {
        #region Variabile

        public Animation EnemyAnimation;

        public Vector2 Position;

        public bool Active;

        public int Health;

        public float enemyMoveSpeed;

        #endregion

        #region Inițializare

        public virtual void Initialization(Animation EnemyAnimation, Vector2 Position, float enemyMoveSpeed)
        {
            this.EnemyAnimation = EnemyAnimation;

            this.Position = Position;

            this.enemyMoveSpeed = enemyMoveSpeed;

            Active = true;

            Health = 10;

        }

        #endregion

        #region Update and Draw

        public virtual void Update(GameTime gameTime)
        {

            Position.X -= enemyMoveSpeed;

            EnemyAnimation.Position = Position;

            EnemyAnimation.Update(gameTime);


            if (Position.X < -EnemyAnimation.FrameSize.X || Health <= 0)
            {
                Active = false;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            EnemyAnimation.Draw(spriteBatch);
        }

        #endregion
    }
}
