using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace JumpyPants
{
    class Player
    {
        #region Variabile și Proprietăți

        public Animation PlayerAnimation;

        public Vector2 Position;

        public bool Active;

        public int Health;

        float jumpHeight, time;

        float gravity = 1200f;

        bool jumpState;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        float elapsedTime;
        bool crouchPhase1 = true;
        bool crouchPhase2 = true;
        bool crouchPhase3 = true;
        bool crouchState;
        float crouchTime;

        public float frameTime;

        public Point FrameSize
        {
            get { return PlayerAnimation.FrameSize; }
            set { PlayerAnimation.FrameSize = value; }
        }

        public Viewport ViewPort;

        #endregion

        #region Inițializare

        public void Initialize(Animation animation, Vector2 position)
        {
            jumpState = false;

            crouchState = false;

            PlayerAnimation = animation;

            frameTime = PlayerAnimation.FrameTime;

            Position = position;

            Active = true;

            Health = 40;
        }

        #endregion

        #region Update and Animation

        bool jumping = false;
        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Update(gameTime);



            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            #region jumping
            
            if ((previousKeyboardState.IsKeyDown(Keys.Space) || previousKeyboardState.IsKeyDown(Keys.Up))  && jumpState == false && crouchState == false)
            {
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;


                if (jumpHeight != -700)
                {
                    if (elapsedTime > 1)
                    {
                        jumpHeight += -100;
                        elapsedTime = 0;


                    }
                }
                else
                {
                    jumpState = true;
                }

                PlayerAnimation.TotalFrames = new Point(14, 2);
                PlayerAnimation.CurrentFrame = new Point(3, 1);
                PlayerAnimation.FrameTime = 160f;

                jumping = true;

                
                if (currentKeyboardState.IsKeyUp(Keys.Space))
                    if (currentKeyboardState.IsKeyUp(Keys.Up))
                    {
                        jumpState = true; 
                    }
            }

            if (jumping == true)
            {
                float jump = (jumpHeight * time + gravity * time * time / 2);
                PlayerAnimation.position.Y = jump + 720 - 50 - PlayerAnimation.texture.Height / 3;
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (PlayerAnimation.position.Y > 720 - 50 - PlayerAnimation.texture.Height / 3)
            {
                PlayerAnimation.position.Y = 720 - 50 - PlayerAnimation.texture.Height / 3;
                jumpState = false;
                time = 0;
                jumpHeight = 0;
                jumping = false;

                PlayerAnimation.TotalFrames = new Point(12, 2);
                PlayerAnimation.CurrentFrame = new Point(0, 0);
                PlayerAnimation.FrameTime = frameTime;
            }

            #endregion

            #region Crouching

            if ((previousKeyboardState.IsKeyDown(Keys.Down) || 
                previousKeyboardState.IsKeyDown(Keys.LeftControl) || 
                previousKeyboardState.IsKeyDown(Keys.RightControl)) && jumpState == false)
            {
                crouchTime += (float)gameTime.ElapsedGameTime.Milliseconds;
                if (crouchTime <= 2000)
                {
                    crouchState = true;
                    if (crouchPhase1 == true)
                    {
                        PlayerAnimation.FrameSize = new Point(200, 169);
                        PlayerAnimation.TotalFrames = new Point(8, 2);
                        PlayerAnimation.CurrentFrame = new Point(0, 2);
                        crouchPhase1 = false;
                    }

                    if (PlayerAnimation.CurrentFrame.X == 2)
                    {
                        if (crouchPhase2 == true)
                        {
                            PlayerAnimation.CurrentFrame = new Point(3, 2);
                            PlayerAnimation.FrameTime = 160f;
                            crouchPhase2 = false;
                        }
                        PlayerAnimation.firstFrame = 3;
                        PlayerAnimation.lastFrame = 5;

                    }

                }
                if (crouchState == true)
                {
                    if ((currentKeyboardState.IsKeyUp(Keys.Down) &&
                        currentKeyboardState.IsKeyUp(Keys.LeftControl) &&
                         currentKeyboardState.IsKeyUp(Keys.RightControl)))
                    {
                        if (PlayerAnimation.CurrentFrame.X == 4 || PlayerAnimation.CurrentFrame.X == 3 || PlayerAnimation.CurrentFrame.X == 5)
                        {
                            if (crouchPhase3 == true)
                            {
                                PlayerAnimation.CurrentFrame = new Point(5, 2);
                                crouchPhase3 = false;
                            }
                            PlayerAnimation.firstFrame = 5;
                            PlayerAnimation.lastFrame = 8;
                            PlayerAnimation.FrameTime = frameTime;
                        }
                        
                    }
                    else if ((previousKeyboardState.IsKeyDown(Keys.Down) ||
                     previousKeyboardState.IsKeyDown(Keys.LeftControl) ||
                     previousKeyboardState.IsKeyDown(Keys.RightControl)) && crouchTime > 2000)
                    {
                        if (crouchState)
                        {
                            if (PlayerAnimation.CurrentFrame.X == 4 || PlayerAnimation.CurrentFrame.X == 3 || PlayerAnimation.CurrentFrame.X == 5)
                            {
                                if (crouchPhase3 == true)
                                {
                                    PlayerAnimation.CurrentFrame = new Point(5, 2);
                                    crouchPhase3 = false;
                                }
                                PlayerAnimation.firstFrame = 5;
                                PlayerAnimation.lastFrame = 8;
                                PlayerAnimation.FrameTime = frameTime;
                            }
                            if (PlayerAnimation.CurrentFrame.X <= 8 && PlayerAnimation.CurrentFrame.X >= 5)
                            {
                                crouchState = false;
                            }
                        }
                    }
                }

                if (crouchState == false && PlayerAnimation.CurrentFrame.X == 7 && PlayerAnimation.CurrentFrame.Y == 2)
                {
                    PlayerAnimation.CurrentFrame = new Point(8, 2);
                    PlayerAnimation.FrameSize = new Point(100, 169);
                    PlayerAnimation.TotalFrames = new Point(12, 2);
                    PlayerAnimation.CurrentFrame = new Point(0, 0);
                    crouchPhase1 = true;
                    crouchPhase2 = true;
                    crouchPhase3 = true;
                }

                
            }

            if ((currentKeyboardState.IsKeyUp(Keys.Down) &&
                        currentKeyboardState.IsKeyUp(Keys.LeftControl) &&
                         currentKeyboardState.IsKeyUp(Keys.RightControl)) && PlayerAnimation.CurrentFrame.X == 7 && PlayerAnimation.CurrentFrame.Y == 2)
            {
                PlayerAnimation.CurrentFrame = new Point(8, 2);
                PlayerAnimation.FrameSize = new Point(100, 169);
                PlayerAnimation.TotalFrames = new Point(12, 2);
                PlayerAnimation.CurrentFrame = new Point(0, 0);
                crouchPhase1 = true;
                crouchPhase2 = true;
                crouchPhase3 = true;
                crouchState = false;
            }

            if ((currentKeyboardState.IsKeyUp(Keys.Down) &&
                        currentKeyboardState.IsKeyUp(Keys.LeftControl) &&
                         currentKeyboardState.IsKeyUp(Keys.RightControl)) && (crouchTime < 2000 || crouchTime > 2000))
                crouchTime = 0;

            #endregion


        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
        }

        #endregion

    }
}
