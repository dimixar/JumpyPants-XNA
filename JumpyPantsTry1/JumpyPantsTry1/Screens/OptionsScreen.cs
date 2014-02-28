using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System;
using Microsoft.Xna.Framework.Audio;

namespace JumpyPants
{
    class OptionsScreen : MenuScreen
    {
        #region fields

        MenuEntry musicMenuEntry;
        MenuEntry soundMenuEntry;
        MenuEntry fullscreenMenuEntry;
        MenuEntry highScoreResetMenuEntry;

        bool isMusic;
        bool isSound;
        bool isFullscreen;

        string directory;
        string path;
        FileInfo scoreFile;
        string highScore;

        #endregion

        #region Initializare

        public OptionsScreen()
            : base("Options")
        {

            directory = System.Environment.CurrentDirectory;
            path = directory + "\\" + "jphs";
            
            if (FileExists(path))
            {
                scoreFile = new FileInfo(path);
                using (StreamReader sr = scoreFile.OpenText())
                {
                    highScore = sr.ReadToEnd().ToString();
                }
            }
            else
            {
                highScore = "0";
                
            }

            fullscreenMenuEntry = new MenuEntry(string.Empty);
            musicMenuEntry = new MenuEntry(string.Empty);
            soundMenuEntry = new MenuEntry(string.Empty);
            highScoreResetMenuEntry = new MenuEntry(string.Empty);

            isFullscreen = false;
            VerifyMusic();
            VerifySound();

            SetMenuEntryTextMusic();
            SetMenuEntryTextSound();
            SetMenuEntryTextFullscreen();
            SetMenuEntryTextHighScore();

            MenuEntry backMenuEntry = new MenuEntry("back");

            fullscreenMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(fullscreenMenuEntry_Selected);
            musicMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(musicMenuEntry_Selected);
            soundMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(soundMenuEntry_Selected);
            highScoreResetMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(highScoreResetMenuEntry_Selected);
            backMenuEntry.Selected += OnCancel;

            MenuEntries.Add(fullscreenMenuEntry);
            MenuEntries.Add(musicMenuEntry);
            MenuEntries.Add(soundMenuEntry);
            MenuEntries.Add(highScoreResetMenuEntry);
            MenuEntries.Add(backMenuEntry);  

        }

        

        void SetMenuEntryTextHighScore()
        {
            highScoreResetMenuEntry.Text = "Reset High Score " + "(" + highScore + "m)";
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

        private void SetMenuEntryTextMusic()
        {
            musicMenuEntry.Text = "Music : " + (isMusic ? "ON" : "OFF");
            
        }

        private void SetMenuEntryTextSound()
        {
            soundMenuEntry.Text = "Sound : " + (isSound ? "ON" : "OFF");
        }

        private void SetMenuEntryTextFullscreen()
        {
            fullscreenMenuEntry.Text = "Change Fullscreen/Windowed";
        }

        void VerifyMusic()
        {
            if (MediaPlayer.IsMuted)
            { isMusic = false; }
            else
            { isMusic = true; }
        }

        void VerifySound()
        {
            if (SoundEffect.MasterVolume == 1f)
            { isSound = true; }
            else if (SoundEffect.MasterVolume == 0f)
            { isSound = false; }
        }

        #endregion

        #region Handle Input

        void highScoreResetMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (FileExists(path))
            {
                scoreFile = new FileInfo(path);
                scoreFile.Delete();
                using (StreamWriter sw = scoreFile.CreateText())
                {
                    sw.Write("0");
                }
            }
            else
            {
                scoreFile = new FileInfo(path);
                using (StreamWriter sw = scoreFile.CreateText())
                {
                    sw.Write("0");
                }
            }
            highScore = "0";
            SetMenuEntryTextHighScore();
        }

        void musicMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            isMusic = !isMusic;

            if (isMusic)
            {
                MediaPlayer.IsMuted = false;
            }
            else
            {
                MediaPlayer.IsMuted = true;
            }

            SetMenuEntryTextMusic();
        }

        void soundMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            isSound = !isSound;

            if (isSound)
            {
                SoundEffect.MasterVolume = 1f;
            }
            else
            {
                SoundEffect.MasterVolume = 0f;
            }

            SetMenuEntryTextSound();
        }

        void fullscreenMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            
            isFullscreen = !isFullscreen;

            if (isFullscreen)
            {
                ScreenManager.graphics.ToggleFullScreen();
            }
            else
            {
                ScreenManager.graphics.ToggleFullScreen();
            }

            SetMenuEntryTextFullscreen();
        }

        #endregion

    }
}
