﻿#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace JumpyPants
{
    class MessageBoxScreen : GameScreen
    {

        #region Valiabile

        string message;
        Texture2D gradientTexture;

        #endregion

        #region Evenimente

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Inițializare

        public MessageBoxScreen(string message)
            : this(message, true)
        {
        }

        public MessageBoxScreen(string message, bool includeUsageText)
        {
            const string usageText = "\nPress Space or Enter = Quit" +
                                     "\nPress Esc = Cancel";

            if (includeUsageText)
                this.message = message + usageText;
            else
                this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            gradientTexture = content.Load<Texture2D>("trans");
        }


        #endregion

        #region Manipularea Input-ului

        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;


            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                if (Accepted != null)
                {
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));
                }

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            //Background-ul boxei trebuie să includă 
            //o ramă pentru margini mai largă ca textul din mesaj
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);

            spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();
        }

        #endregion

    }
}
