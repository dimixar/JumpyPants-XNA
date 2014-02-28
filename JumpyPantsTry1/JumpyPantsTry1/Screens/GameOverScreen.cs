using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace JumpyPants
{
    class GameOverScreen : GameScreen
    {
        ContentManager content;
        Song gameOverSong;
        SpriteFont gameOverFont;
        int distance;
        int highScore;
        string directory;
        string path;
        float redTime, blueTime;
        bool isHigherScore;
        Color redColor, blueColor;
        Color highScoreColor;
        FileInfo scoreFile;
        Texture2D gameOverBackground;


        #region Inițializare

        public GameOverScreen(int distance)
        {
            this.distance = distance;
            directory = System.Environment.CurrentDirectory;
            path = directory + "\\" + "jphs";

            if (FileExists(path))
            {
                scoreFile = new FileInfo(path);
                using (StreamReader sr = scoreFile.OpenText())
                {
                    highScore = Convert.ToInt32(sr.ReadToEnd().ToString());
                }
            }
            else
            {
                scoreFile = new FileInfo(path);
                using (StreamWriter sw = scoreFile.CreateText())
                {
                    sw.Write(distance);
                }
                highScore = distance;
            }

            isHigherScore = false;

        }

        private bool FileExists(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            else
                return false;
        }

        private void ColorChange(bool isHigher, GameTime gameTime)
        {
            if (isHigher)
            {
                redTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (redTime < 500)
                {
                    redColor = Color.Red;
                    highScoreColor = redColor;
                }
                else if (redTime > 500)
                {
                    blueTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (blueTime < 500)
                    {
                        blueColor = Color.Blue;
                        highScoreColor = blueColor;
                    }
                    else
                    {
                        redTime = 0;
                        blueTime = 0;
                    }
                }
            }
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            gameOverFont = content.Load<SpriteFont>("GameOverFont");
            gameOverSong = content.Load<Song>("Music/GameOverMusic");

            gameOverBackground = content.Load<Texture2D>("GameOverScreen");

            MediaPlayer.Volume = 1f;
            PlaySong(gameOverSong);
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

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                               new MainMenuScreen());
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new RunForeverModeScreen());
            }

            if (distance > highScore)
            {
                isHigherScore = true;
                scoreFile.Delete();
                scoreFile = new FileInfo(path);
                using (StreamWriter sw = scoreFile.CreateText())
                {
                    sw.Write(distance);
                }
                //highScore = distance;
            }

            ColorChange(isHigherScore, gameTime);

        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.Draw(gameOverBackground, Vector2.Zero, Color.White);
            spriteBatch.DrawString(gameOverFont, " " + distance + "m", new Vector2(800, 348), Color.Black);
            spriteBatch.DrawString(gameOverFont, " " + highScore + "m", new Vector2(750, 410), Color.Black);
            if (isHigherScore == true)
            {
                spriteBatch.DrawString(gameOverFont, "You have new High Score!\n           " + distance + "m", new Vector2(325, 235), highScoreColor);
            }

            spriteBatch.End();

        }

        #endregion
    }
}
