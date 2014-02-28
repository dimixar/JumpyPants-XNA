using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace JumpyPants
{
    class PauseMenuScreen : MenuScreen
    {

        #region Inițializare

        public PauseMenuScreen()
            : base("Paused")
        {
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry restartGameMenuEntry = new MenuEntry("Restart Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit to Main Menu");

            resumeGameMenuEntry.Selected += OnCancel;
            restartGameMenuEntry.Selected += RestartGameMenuEntrySelected;
            optionsMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(OptionsMenuEntry_Selected);
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(restartGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        #endregion


        #region Manipularea Input-ului

        void OptionsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsScreen(), e.PlayerIndex);
        }

        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Do you really want to quit? \nIf you quit now, your score will not count.";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += new EventHandler<PlayerIndexEventArgs>(confirmQuitMessageBox_Accepted);

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        void RestartGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new RunForeverModeScreen());
        }

        void confirmQuitMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }

        #endregion

    }
}
