#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
#endregion

namespace JumpyPants
{
    class MainMenuScreen : MenuScreen
    {

        string directory;
        string path;
        FileInfo scoreFile;

        #region Inițializare

        public MainMenuScreen()
            : base("Main Menu")
        {
            directory = System.Environment.CurrentDirectory;
            path = directory + "\\" + "jphs";

            if (FileExists(path) == false)
            {
                scoreFile = new FileInfo(path);
                
                using (StreamWriter sw = scoreFile.CreateText())
                {
                    sw.Write("0");
                }
            }

            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit Game");

            playGameMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(playGameMenuEntry_Selected);
            optionsMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(OptionsMenuEntry_Selected);
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
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

        #endregion

        #region Manipularea Input-ului

        void playGameMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ChooseModeScreen(), e.PlayerIndex);
        }

        void OptionsMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
#if WINDOWS
            const string message = "Are you sure you want to quit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += new EventHandler<PlayerIndexEventArgs>(confirmExitMessageBox_Accepted);

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
#endif

#if WINDOWS_PHONE
            ScreenManager.Game.Exit();
#endif
        }

        void confirmExitMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        #endregion

    }
}
