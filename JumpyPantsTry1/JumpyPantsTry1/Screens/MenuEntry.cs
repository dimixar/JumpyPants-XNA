#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace JumpyPants
{
    /// <summary>
    /// Aceasta clasă ne va permite să creăm opțiuni de acțiuni în meniul jocului nostru
    /// </summary>
    class MenuEntry
    {

        #region Variabile

        /// <summary>
        /// Textul care va fi prezent in opțiunea din menu
        /// </summary>
        string text;

        /// <summary>
        /// Aceasta vom folosi atunci cand vom face tranzitii intre screen-uri din menu,
        /// pentru a face meniul putin mai interesant
        /// </summary>
        float selectionFade;

        /// <summary>
        /// Aceasta vom folosi pentru a aranja opțiunea noastră pe ecranul jocului
        /// </summary>
        Vector2 position;

        #endregion

        #region Proprietăți

        /// <summary>
        /// Setează sau Primește textul de la opțiunea din menu
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Setează sau Primește poziția opțiunii din menu
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        //Verifică dacă textul este Selectat
        public bool IsSelected { get; set; }

        #endregion

        #region Evenimente

        public event EventHandler<PlayerIndexEventArgs> Selected;

        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        #endregion

        #region Inițializare

        public MenuEntry(string text)
        {
            this.text = text;
        }

        #endregion

        #region Update and Draw

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
#if WINDOWS_PHONE
            isSelected = false;
#endif

            IsSelected = isSelected;

            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);

        }

        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {

#if WINDOWS_PHONE
            isSelected = false;
#endif

            Color color = isSelected ? Color.Red : Color.DarkSlateBlue;

            double time = gameTime.ElapsedGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 3) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            color *= screen.TransitionAlpha;

            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0, origin, scale, SpriteEffects.None, 0);

        }


        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }


        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }


        #endregion

    }
}
