#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
#endregion

namespace JumpyPants
{

    ///<summary>
    ///Ajutător pentru citirea input-ului de la tastatură, gamepad, și touch.
    ///Această clasă urmărește amîndouă stări(curent șî precedent) ale device-ului de input,
    ///și implementează metode de interogări pentru acțiune de nivel înalt așa ca "mișcă sus prin menu"
    ///sau "pune jocul pe pauză".
    ///</summary>
    public class InputState
    {
        
        #region variabile

        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        #endregion

        #region Inițializare

        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];

            GamePadWasConnected = new bool[MaxInputs];
        }

        #endregion

        #region Metode Publice

        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);


                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }

            }


            TouchState = TouchPanel.GetState();

            Gestures.Clear();

            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }

        }

        ///<summary>
        ///Ajutător pentru verificarea noului buton apăsat în acest update
        ///</summary>
        ///<param name="key">tasta pe care va apăsa</param>
        ///<param name="controllingPlayer">player-ul care controlează jocul,
        ///daca e NULL atunci va controla primul care va apasa vreo tastă</param>
        ///<param name="playerIndex">acesta va raporta care este player-ul care la moment a apasat butonul</param>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                //citeste input-ul de la player-ul specificat
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key)
                    && LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                //Acceptă input de la orice player
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Face fix aceeași ce face și IsNewKeyPress, numai că aceasta e pentru butoane de la gamepad
        /// </summary>
        /// <param name="button">butonul pe care va apăsa</param>
        /// <param name="controllingPlayer">player-ul care controlează jocul,
        ///daca e NULL atunci va controla primul care va apasa vreun buton</param>
        /// <param name="playerIndex">acesta va raporta care este player-ul care la moment a apasat butonul</param>
        /// <returns></returns>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Verifică acțiunea de input pentru "Selectarea unei opțiuni din menu"
        /// </summary>
        /// <param name="controllingPlayer">player-ul care controleaza la moment jocul, daca e NULL, atunci primul care
        /// va apasa butonul va controla jocul</param>
        /// <param name="playerIndex">raport la cine controleaza la moment player-ul</param>
        /// <returns></returns>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Verifica acțiunea pentru "Iesire din menu"
        /// </summary>
        /// <param name="controllingPlayer">player-ul care controleaza la moment jocul, daca e NULL, atunci primul care
        /// va apasa butonul va controla jocul</param>
        /// <param name="playerIndex">raport la cine controleaza la moment player-ul</param>
        /// <returns></returns>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }


        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }


        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }


        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        public bool IsPlayerCrouching(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.LeftControl, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.RightControl, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.X, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex);
        }

        #endregion

    }
}
