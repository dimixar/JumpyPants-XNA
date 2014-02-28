using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JumpyPants
{
    class Animation
    {
        #region Variabile

        public Texture2D texture;

        Point frameSize, totalFrames, currentFrame;

        public int firstFrame, lastFrame;

        float frameTime;

        float elapsedTime;

        Color color;

        public bool Active;

        public bool Looping;

        public Vector2 position;

        Rectangle sourceRect;

        public SpriteEffects spriteEffect;

        #endregion

        #region Proprietăți

        public Point FrameSize
        {
            get { return frameSize; }
            set { frameSize = value; }
        }

        public Point TotalFrames
        {
            get { return totalFrames; }
            set 
            { 
                totalFrames = value;
                firstFrame = 0;
                lastFrame = totalFrames.X;
            }
        }

        public Point CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion

        #region Inițializare

        public void Initialize(Texture2D texture, Vector2 position,
            Point frameSize, Point totalFrames, float frameTime,
            Color color, bool looping)
        {
            this.color = color;
            this.frameSize = frameSize;
            this.totalFrames = totalFrames;
            this.frameTime = frameTime;
            firstFrame = 0;
            lastFrame = totalFrames.X;

            spriteEffect = SpriteEffects.None;

            this.texture = texture;
            this.position = position;
            Looping = looping;

            currentFrame = new Point(0, 0);
            elapsedTime = 0;

            Active = true;
        }

        #endregion

        #region Update and Draw

        public void Update(GameTime gameTime)
        {
            if (Active == false)
                return;

            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > frameTime)
            {
                if (currentFrame.X >= firstFrame && currentFrame.X <= lastFrame)
                currentFrame.X++;

                if (currentFrame.X >= lastFrame)
                {
                    currentFrame.X = firstFrame;

                    if (Looping == false)
                        Active = false;
                }

                elapsedTime = 0;
            }

            sourceRect = new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(texture, position, sourceRect, color, 0, Vector2.Zero, 1f, spriteEffect, 0f);
            }

        }

        #endregion

    }
}
