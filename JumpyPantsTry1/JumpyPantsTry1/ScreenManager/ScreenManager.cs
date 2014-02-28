﻿#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace JumpyPants
{
    public class ScreenManager : DrawableGameComponent
    {

        #region Variabile

        List<GameScreen> screens = new List<GameScreen>();
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        InputState input = new InputState();

        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D blankTexture;

        public GraphicsDeviceManager graphics;

        bool isInitialized;

        bool traceEnabled;

        #endregion

        #region Proprietăți

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public SpriteFont Font
        {
            get { return font; }
        }

        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }

        #endregion

        #region Inițializarea

        public ScreenManager(Game game)
            : base(game)
        {
            TouchPanel.EnabledGestures = GestureType.Tap;
            graphics = new GraphicsDeviceManager(Game);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();
        }


        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("menufont");
            blankTexture = content.Load<Texture2D>("blank");

            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }


        #endregion

        #region Update and Draw


        public override void Update(GameTime gameTime)
        {
            input.Update();

            screensToUpdate.Clear();

            foreach (GameScreen screen in screens)
            {
                screensToUpdate.Add(screen);
            }

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            while (screensToUpdate.Count > 0)
            {
                GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;

                }
            }

            if (traceEnabled)
                TraceScreens();
        }


        private void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
            {
                screenNames.Add(screen.GetType().Name);
            }

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }


        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }



        #endregion

        #region Metode Publice

        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);

            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }

        public void RemoveScreen(GameScreen screen)
        {
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);

            if (screens.Count > 0)
            {
                TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
            }
        }


        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;
            spriteBatch.Begin();

            spriteBatch.Draw(blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.Black * alpha);

            spriteBatch.End();

        }


        #endregion

    }
}
