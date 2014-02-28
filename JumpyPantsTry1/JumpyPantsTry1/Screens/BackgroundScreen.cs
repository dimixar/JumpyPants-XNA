#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
#endregion

namespace JumpyPants
{
    class BackgroundScreen : GameScreen
    {
        #region variabile

        ContentManager content;
        Texture2D backgroundTextureSky;
        Texture2D backgroundTextureGround;
        Texture2D backgroundTextureDealuri;

        Song MainMenuSong;

        Texture2D sunTexture;
        Animation sunAnimation;
        Vector2 sunPosition;
#if WINDOWS
        Texture2D controlSchemeTexture;
        Vector2 controlSchemePosition;
#endif
        Texture2D playerTexture;
        Vector2 playerPosition;
        Animation playerAnimation;

        Texture2D cloudTexture;
        List<Cloud> cloudsLeft;
        List<Cloud> cloudsRight;
        Random random;
        TimeSpan cloudSpawnTimeLeft;
        TimeSpan cloudPreviousSpawnTimeLeft;
        float cloudMoveSpeed;
        TimeSpan cloudSpawnTimeRight;
        TimeSpan cloudPreviousSpawnTimeRight;


        Texture2D jumpyTexture;
        Vector2 jumpyPosition;


        #endregion

        #region Initializarea

        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            sunAnimation = new Animation();
            playerAnimation = new Animation();

            cloudsLeft = new List<Cloud>();
            cloudsRight = new List<Cloud>();


            random = new Random();
            cloudSpawnTimeLeft = TimeSpan.FromSeconds(1.5f);
            cloudPreviousSpawnTimeLeft = TimeSpan.Zero;

            cloudSpawnTimeRight = TimeSpan.FromSeconds(3.5f);
            cloudPreviousSpawnTimeRight = TimeSpan.Zero;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            MainMenuSong = content.Load<Song>("Music/MainMenuSong");
            backgroundTextureSky = content.Load<Texture2D>("BackgroundImages/Sky_Background");
            backgroundTextureGround = content.Load<Texture2D>("BackgroundImages/Ground_Background");
            backgroundTextureDealuri = content.Load<Texture2D>("BackgroundImages/Dealuri_Background");

            MediaPlayer.Volume = 1f;
            PlaySong(MainMenuSong);
#if WINDOWS
            controlSchemeTexture = content.Load<Texture2D>("MenuImages/ControlScheme");
            controlSchemePosition = new Vector2(
                ScreenManager.Game.GraphicsDevice.Viewport.Width - controlSchemeTexture.Width - 60,
                ScreenManager.Game.GraphicsDevice.Viewport.Height - backgroundTextureGround.Height - controlSchemeTexture.Height);
#endif
            playerTexture = content.Load<Texture2D>("MenuImages/Player_Awesome_Animation");
            playerPosition = new Vector2(40,
                ScreenManager.Game.GraphicsDevice.Viewport.Height - backgroundTextureGround.Height - playerTexture.Height + 10);
            playerAnimation.Initialize(playerTexture, playerPosition, new Point(100, 169), new Point(9, 0), 75, Color.White, true);

            sunPosition = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width - 150, -70);
            sunTexture = content.Load<Texture2D>("BgElements/SunAnimation_Background");
            sunAnimation.Initialize(sunTexture, sunPosition, new Point(200, 200), new Point(8, 0), 80, Color.White, true);

            cloudTexture = content.Load<Texture2D>("BgElements/Cloud_Background1");

#if WINDOWS
            jumpyTexture = content.Load<Texture2D>("MenuImages/JumpyPants");
            jumpyPosition = new Vector2(
                ScreenManager.Game.GraphicsDevice.Viewport.Width / 2 - jumpyTexture.Width / 2 + 100,
                ScreenManager.Game.GraphicsDevice.Viewport.Height / 2 - jumpyTexture.Height - 50);
#endif

#if WINDOWS_PHONE
            jumpyTexture = content.Load<Texture2D>("MenuImages/JumpyPants");
            jumpyPosition = new Vector2(
                ScreenManager.Game.GraphicsDevice.Viewport.Width / 2 - jumpyTexture.Width / 2 + 90,
                ScreenManager.Game.GraphicsDevice.Viewport.Height / 2 - jumpyTexture.Height + 110);
#endif

        }


        public override void UnloadContent()
        {
            content.Unload();
        }

        void PlaySong(Song song)
        {
            try
            {
                MediaPlayer.Play(song);

                MediaPlayer.IsRepeating = true;

            }
            catch { }
        }

        void StopSong()
        {
            try
            {
                MediaPlayer.Stop();
            }
            catch { }
        }

        #endregion

        #region Update and Draw

        #region Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            sunAnimation.Update(gameTime);
            playerAnimation.Update(gameTime);
            UpdateCloudsLeft(gameTime);
            UpdateCloudsRight(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                MediaPlayer.IsMuted = true;
            else if (Keyboard.GetState().IsKeyDown(Keys.RightShift))
                MediaPlayer.IsMuted = false;

            base.Update(gameTime, otherScreenHasFocus, false);
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);


            spriteBatch.Draw(backgroundTextureSky, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            sunAnimation.Draw(spriteBatch);

            for (int i = 0; i < cloudsRight.Count - 1; i++)
            {
                cloudsRight[i].Draw(spriteBatch, SpriteEffects.FlipHorizontally);
            }

            for (int i = 0; i < cloudsLeft.Count - 1; i++)
            {
                cloudsLeft[i].Draw(spriteBatch, SpriteEffects.None);
            }

            spriteBatch.Draw(backgroundTextureDealuri, fullscreen, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.Draw(backgroundTextureGround, new Vector2(0, viewport.Height - 50), new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.Draw(jumpyTexture, jumpyPosition, null, Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

#if WINDOWS
            spriteBatch.Draw(controlSchemeTexture, controlSchemePosition, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
#endif
            

            playerAnimation.Draw(spriteBatch);

            spriteBatch.End();

        }

        #endregion

        #region Helper Methods

        #region Clouds From Left Margin of the screen

        void AddCloudLeft()
        {
            Cloud cloud = new Cloud();

            Vector2 position = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width + cloudTexture.Width * 4,
                random.Next(20, ScreenManager.Game.GraphicsDevice.Viewport.Height / 2 - cloudTexture.Height));

            cloud.Initialize(cloudTexture, position, Scale(), cloudMoveSpeed);

            cloudsLeft.Add(cloud);

        }

        void UpdateCloudsLeft(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - cloudPreviousSpawnTimeLeft > cloudSpawnTimeLeft)
            {
                cloudPreviousSpawnTimeLeft = gameTime.TotalGameTime;


                AddCloudLeft();
            }

            for (int i = cloudsLeft.Count - 1; i >= 0; i--)
            {
                cloudsLeft[i].UpdateLeft(gameTime);

                if (cloudsLeft[i].Active == false)
                {
                    cloudsLeft.RemoveAt(i);
                }
            }

        }

        #endregion

        #region Clouds From Right Margin of the screen

        void AddCloudRight()
        {
            Cloud cloud = new Cloud();

            Vector2 position = new Vector2(-cloudTexture.Width * 4,
                random.Next(20, ScreenManager.Game.GraphicsDevice.Viewport.Height / 2 - cloudTexture.Height));

            cloud.Initialize(cloudTexture, position, Scale(), cloudMoveSpeed);

            cloudsRight.Add(cloud);

        }


        void UpdateCloudsRight(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - cloudPreviousSpawnTimeRight > cloudSpawnTimeRight)
            {
                cloudPreviousSpawnTimeRight = gameTime.TotalGameTime;

                AddCloudRight();
            }

            for (int i = cloudsRight.Count - 1; i >= 0; i--)
            {
                cloudsRight[i].UpdateRight(gameTime);

                if (cloudsRight[i].Active == false)
                {
                    cloudsRight.RemoveAt(i);
                }
            }

        }

        #endregion

        /// <summary>
        /// Metodă pentru alegerea aleatoare a scalării nourilor împreună cu viteza lor de mișcare
        /// </summary>
        /// <returns>float</returns>
        float Scale()
        {
            int flag = random.Next(0, 6);

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
                case 5:
                    {
                        cloudMoveSpeed = 0.5f;
                        return 1.7f;
                    }
                case 6:
                    {
                        cloudMoveSpeed = 0.9f;
                        return 1.4f;
                    }
                default: { cloudMoveSpeed = 1.2f; return 1f; }
            }
        }

        #endregion

        #endregion

    }
}
