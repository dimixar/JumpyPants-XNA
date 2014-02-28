using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JumpyPants
{
    class ChooseModeScreen : MenuScreen
    {
        SpriteFont font;
        ContentManager content;

        #region Inițializare
        MenuEntry RunForeverModeMenuEntry;
        MenuEntry SurviveForeverModeMenuEntry;
        MenuEntry PracticeForeverModeMenuEntry;
        MenuEntry BackMenuEntry;

        public ChooseModeScreen()
            : base("Game Mode")
        {
            RunForeverModeMenuEntry = new MenuEntry("Run Forever");
            SurviveForeverModeMenuEntry = new MenuEntry("Survive Forever (Coming Soon)");
            PracticeForeverModeMenuEntry = new MenuEntry("Practice Forever (Coming Soon)");
            BackMenuEntry = new MenuEntry("Back");

            RunForeverModeMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(chilloutModeMenuEntry_Selected);
            SurviveForeverModeMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(HeavyMetalModeMenuEntry_Selected);
            PracticeForeverModeMenuEntry.Selected += new EventHandler<PlayerIndexEventArgs>(PracticeModeMenuEntry_Selected);
            BackMenuEntry.Selected += OnCancel;

            MenuEntries.Add(RunForeverModeMenuEntry);
            MenuEntries.Add(SurviveForeverModeMenuEntry);
            MenuEntries.Add(PracticeForeverModeMenuEntry);
            MenuEntries.Add(BackMenuEntry);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            font = content.Load<SpriteFont>("gameFont");
        }

        #endregion

        #region Manipularea Input-ului

        void PracticeModeMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            //TODO: Loading Screen from Practice Mode Gameplay
        }

        void HeavyMetalModeMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            //TODO: Loading Screen from Heavy Metal Mode Gameplay
        }

        void chilloutModeMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new RunForeverModeScreen());
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            if (RunForeverModeMenuEntry.IsSelected)
            {
                ScreenManager.SpriteBatch.DrawString(font, "Run as much as you can and beat the high score. \n      Your Distance will count as you score", new Vector2(20, 300), Color.Black);
            }
            else if (SurviveForeverModeMenuEntry.IsSelected)
            {
                ScreenManager.SpriteBatch.DrawString(font, "This mode will be included at the next update.", new Vector2(20, 300), Color.Black);
            }
            else if (PracticeForeverModeMenuEntry.IsSelected)
            {
                ScreenManager.SpriteBatch.DrawString(font, "This mode will be included at the next update.", new Vector2(20, 300), Color.Black);
            }

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
