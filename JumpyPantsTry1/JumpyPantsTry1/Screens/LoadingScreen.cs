using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace JumpyPants
{
    class LoadingScreen : GameScreen
    {

        #region Variabile

        bool loadingIsSlow;
        bool otherScreensAreGone;

        GameScreen[] screensToLoad;

        ContentManager content;
        Texture2D loadTexture;
        Vector2 loadPosition;
        Animation loadAnimation;


        int elapsedTime;
        int frameTime;

        #endregion


        #region Inițializare

        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow,
                              GameScreen[] screensToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;
            loadAnimation = new Animation();


            frameTime = 30;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);

        }

        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
            PlayerIndex? controllingPlayer, params GameScreen[] screensToLoad)
        {

            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            LoadingScreen loadingScreen = new LoadingScreen(screenManager, loadingIsSlow, screensToLoad);

            screenManager.AddScreen(loadingScreen, controllingPlayer);

        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            loadTexture = content.Load<Texture2D>("MenuImages/LoadingAnimation");
            loadPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - loadTexture.Width / 3 + 130,
                ScreenManager.GraphicsDevice.Viewport.Height / 2 - loadTexture.Height / 2 + 70);


            loadAnimation.Initialize(loadTexture, loadPosition, new Point(300, 50), new Point(3, 0), 70, Color.White, true);
            loadAnimation.CurrentFrame = new Point(0, 0);

        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion


        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            elapsedTime += (int)gameTime.TotalGameTime.Milliseconds;

            if (elapsedTime > frameTime)
            {
                MediaPlayer.Volume -= 0.05f;
                if (MediaPlayer.Volume == 0)
                    MediaPlayer.Stop();

                elapsedTime = 0;
            }

            loadAnimation.Update(gameTime);

            if (otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                        ScreenManager.AddScreen(screen, ControllingPlayer);
                }

                //ScreenManager.Game.ResetElapsedTime();

            }

            
        }


        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == JumpyPants.ScreenState.Active)
                && (ScreenManager.GetScreens().Length == 1))
            { otherScreensAreGone = true; }

            if (loadingIsSlow)
            {

                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font = ScreenManager.Font;

                const string message = "Loading...";


                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                Color color = Color.White * TransitionAlpha;


                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                ScreenManager.GraphicsDevice.Clear(Color.Black);
                spriteBatch.DrawString(font, message, textPosition, color);

                loadAnimation.Draw(spriteBatch);

                spriteBatch.End();
            }
        }

        #endregion

    }
}
