using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;

namespace JumpyPants
{
    /// <summary>
    /// Enum-ul descrie stările de tranziție a unui screen
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// Colecția de proprietăți și metode, care vor fi folosite pentru controlul mai ușor a stării jocului
    /// </summary>
    public abstract class GameScreen
    {

        #region Proprietati

        /// <summary>
        /// Indica daca screen-ul care trebuie să se deschidă e de tip popup sau nu.
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        bool isPopup = false;

        /// <summary>
        /// Determină cât timp are nevoie ca să facă o tranziție screen-ul activat.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;

        /// <summary>
        /// Determină cât timp are nevoie ca să facă o tranziție screen-ul dezactivat.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;

        /// <summary>
        /// Determină poziția curentă a tranziției, începînd de la zero
        /// (activat) pînă la unu (tranziționat pînă la sfârșit).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;

        /// <summary>
        /// Primește alpha curentă de la tranziția screen-ului, începînd de la zero
        /// (activat) pînă la unu (tranziționat pînă la sfârșit).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        /// <summary>
        /// Primește și determină starea curentă a tranziției screen-ului.
        /// </summary>
        public ScreenState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenState screenState = ScreenState.TransitionOn;

        /// <summary>
        /// Determină dacă screen-ul trebuie să iese din memorie total sau pur și simplu
        /// să se ducă în spatele screen-ului activ.
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        bool isExiting = false;

        /// <summary>
        /// Verifică dacă screen-ul este activ și este capabil la răspuns de la utilizator
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                    (screenState == ScreenState.TransitionOn ||
                     screenState == ScreenState.Active);
            }
        }

        bool otherScreenHasFocus;


        /// <summary>
        /// Primește manager-ul screen-ului corespunzător
        /// </summary>
        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            internal set { screenManager = value; }
        }

        ScreenManager screenManager;

        /// <summary>
        /// Primește index-ul jucătorului care controlează la moment screen-ul jocului
        /// </summary>
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }

        PlayerIndex? controllingPlayer;

        public GestureType EnabledGestures
        {
            get { return enabledGestures; }
            protected set 
            {
                enabledGestures = value;

                if (ScreenState == ScreenState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }

        GestureType enabledGestures = GestureType.None;


        #endregion

        #region Inițializare

        /// <summary>
        /// Încărcăm contentul grafic pentru screen
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// Eliberăm memoria de contentul, care nu este necesar
        /// </summary>
        public virtual void UnloadContent() { }

        #endregion

        #region Update and Draw

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                      bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                screenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    //Încă e ocupat cu tranziția
                    screenState = ScreenState.TransitionOff;
                }
                else
                {
                    //Tranziția a avut loc cu succes
                    screenState = JumpyPants.ScreenState.Hidden;
                }
            }
            else
            {
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    screenState = JumpyPants.ScreenState.TransitionOn;
                }
                else
                {
                    screenState = JumpyPants.ScreenState.Active;
                }
            }
        }

        /// <summary>
        /// Ajutător pentru actualizarea poziției tranziției screen-ului
        /// </summary>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            //cât de mult trebuie să ne mișcăm?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
            {
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
            }

            //Actualizăm poziția tranziției.
            transitionPosition += transitionDelta * direction;

            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            //Dacă încă e în tranziție, atunci este TRUE.
            return true;
        }

        /// <summary>
        /// Permite screen-ul să aibă posibilitatea de a face o acțiune de la tastieră sau touch
        /// </summary>
        public virtual void HandleInput(InputState input) { }

        /// <summary>
        /// Aceasta trebuie apelată atunci când screen-ul trebuie să se desene singur pe dânsul
        /// </summary>
        public virtual void Draw(GameTime gameTime) { }

        #endregion

        #region Metode Publice

        /// <summary>
        /// Spune screen-ului să plece de pe ecran. Această metodă așteaptă pînă ce screen-ul
        /// a tranziționat complet de pe ecran șî după aceea îl distruge.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                isExiting = true;
            }
        }

        #endregion

    }
}
