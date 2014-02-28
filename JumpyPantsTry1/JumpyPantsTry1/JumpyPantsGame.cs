#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace JumpyPants
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class JumpyPantsGame : Microsoft.Xna.Framework.Game
    {
        #region Variabile

        

        ScreenManager screenManager;

        static readonly string[] preloadAssets =
        {
            "trans",
        };


        #endregion



        public JumpyPantsGame()
        {
            
            Content.RootDirectory = "Content";

            
            

            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);

           

        }


        protected override void LoadContent()
        {
            
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }
            
        }

        protected override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            

            base.Draw(gameTime);
        }
    }

    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (JumpyPantsGame game = new JumpyPantsGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
