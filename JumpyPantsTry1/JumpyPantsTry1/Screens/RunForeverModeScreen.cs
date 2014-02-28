#region using statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace JumpyPants
{
    class RunForeverModeScreen : GameScreen
    {
        #region Variabile

        ContentManager content;
        SpriteFont gameFont;
        SpriteFont notifyFont;
        SpriteFont slowDownNotify;
        float speedNotifyCount;
        float notifyCountColorBlue;
        float notifyCountColorGray;
        Color notifySlowdownColor;

        Random random;

        //Pentru Muzică și sunet
        string musicPath;
        Song gameplayMusic;
        SoundEffect crouchingSound;
        SoundEffect runningSound;
        SoundEffect jumpingSound;
        SoundEffect windSound;
        SoundEffect bufSound;
        SoundEffect powerUpGainSound;
        SoundEffect slowdownSound;
        SoundEffect gameOverSound;
        SoundEffectInstance bufSoundInstance;
        SoundEffectInstance runningSoundInstance;
        SoundEffectInstance jumpingSoundInstance;
        SoundEffectInstance windSoundInstance;
        SoundEffectInstance crouchingSoundInstance;
        bool runningState, crouchingState;


        float pauseAlpha;
        bool playMusic;

        //Pentru Background
        ParallaxingBackground dealuriBackground;
        ParallaxingBackground groundBackground;
        ParallaxingBackground SkyBackground;
        float dealuriMoveSpeed;
        float groundMoveSpeed;
        float skyMoveSpeed;

        //Pentru Soare
        Texture2D sunTexture;
        Vector2 sunPosition;
        Animation sunAnimation;
        float sunMoveSpeed;

        //Pentru Player
        Texture2D playerTexture;
        Vector2 playerPosition;
        Animation playerAnimation;
        Player player;
        float playerFrameTime;

        //Pentru Nori
        Texture2D cloudTexture;
        List<Cloud> cloudsLeft;
        TimeSpan cloudSpawnTimeLeft;
        TimeSpan cloudPreviousSpawnTimeLeft;
        float cloudMoveSpeed;

        Texture2D evilCatTexture;
        List<EvilCatSurprise> evilCats;
        TimeSpan evilCatPreviousSpawnTime;
        TimeSpan evilCatSpawnTime;
        bool isEvil;
        float evilElapsedTime;
        Color evilColor;

        //Pentru Inamici
        Texture2D enemyTexture;
        List<Enemy> enemies;
        TimeSpan enemySpawnTime;
        TimeSpan enemyPreviousSpawnTime;
        Point enemyFrameSize;
        Point enemyCurrentFrame;
        Point enemyTotalFrames;
        float enemyMoveSpeed;
        float enemyFrameTime;

        //Pentru explozii
        Texture2D explosionTexture;
        List<Animation> explosions;

        //Pentru powerUp-uri
        Texture2D powerUpTexture;
        List<PowerUp> powerUpsHealth;
        List<PowerUp> powerUpsSpeed;
        TimeSpan powerUpSpawnTime;
        TimeSpan powerUpPreviousSpawnTime;
        float powerUpMoveSpeed;
        bool healthLower, healing;
        bool speedALot, speeding;
        int speedPress; // 0 = did not press Shift Key ;  1 = already pressed Shift Key;
        bool showNoSpeedPU;
        
        //Pentru HealthBar
        HealthBar healthBar;
        Texture2D healthTexture;
        int healthBarIndex;
        Texture2D slowdownTexture;
        Color slowdownColor;
        bool isSlowDown;
        

        //Pentru ridicarea vitezei jocului si aflarea distantei parcurse
        float elapsedTime;
        float distanceCounter;
        int distanceRan;
        int metersPerMilliseconds;
        float distanceMultiplier;

        bool isGameOver;

        KeyboardState currentKeyboardState, previousKeyboardState;

        #endregion

        #region Inițializare

        public RunForeverModeScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            dealuriBackground = new ParallaxingBackground();
            groundBackground = new ParallaxingBackground();
            SkyBackground = new ParallaxingBackground();

            sunAnimation = new Animation();

            cloudsLeft = new List<Cloud>();
            random = new Random();
            cloudSpawnTimeLeft = TimeSpan.FromMilliseconds(4500f);
            cloudPreviousSpawnTimeLeft = TimeSpan.Zero;

            enemies = new List<Enemy>();
            enemySpawnTime = TimeSpan.FromMilliseconds(2000f);
            enemyPreviousSpawnTime = TimeSpan.Zero;
            enemyMoveSpeed = 7f;

            evilCats = new List<EvilCatSurprise>();
            evilCatSpawnTime = TimeSpan.FromMilliseconds(250f);
            evilCatPreviousSpawnTime = TimeSpan.Zero;
            isEvil = false;
            evilColor = new Color(255, 255, 255);

            playerAnimation = new Animation();
            player = new Player();
            playerFrameTime = 30f;

            explosions = new List<Animation>();

            powerUpsHealth = new List<PowerUp>();
            powerUpSpawnTime = TimeSpan.FromMilliseconds(1000f);
            powerUpPreviousSpawnTime = TimeSpan.Zero;
            powerUpsSpeed = new List<PowerUp>();
            healthLower = false;
            healing = false;
            speedALot = false;
            speeding = false;
            speedPress = 0;
            showNoSpeedPU = false;

            healthBar = new HealthBar();
            healthBarIndex = 0;
            slowdownColor = Color.Gray;
            isSlowDown = false;

            runningState = true;
            crouchingState = false;

            playMusic = true;

            metersPerMilliseconds = 160;
            elapsedTime = 0f;
            powerUpMoveSpeed = 5f;
            distanceCounter = 0f;
            dealuriMoveSpeed = 1f;
            groundMoveSpeed = 5f;
            skyMoveSpeed = 0.1f;
            distanceRan = 0;
            sunMoveSpeed = 0.1f;
            cloudMoveSpeed = 0f;
            distanceMultiplier = 10f;

            isGameOver = false;
        }

      

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            musicPath = MusicRandomizer();
            gameFont = content.Load<SpriteFont>("gameFont");
            notifyFont = content.Load<SpriteFont>("notificationFont");
            slowDownNotify = content.Load<SpriteFont>("slowdownNotify");
            gameplayMusic = content.Load<Song>(musicPath);

            dealuriBackground.Initialize(content, "BackgroundImages/Dealuri_Background", ScreenManager.GraphicsDevice.Viewport.Width, dealuriMoveSpeed);
            groundBackground.Initialize(content, "BackgroundImages/ground", ScreenManager.GraphicsDevice.Viewport.Width, groundMoveSpeed);
            SkyBackground.Initialize(content, "BackgroundImages/Sky_Background", ScreenManager.GraphicsDevice.Viewport.Width, skyMoveSpeed);

            sunTexture = content.Load<Texture2D>("BgElements/SunAnimation_Background");
            sunPosition = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width - 150, -70);
            sunAnimation.Initialize(sunTexture, sunPosition, new Point(200, 200), new Point(8, 1), 80, Color.White, true);

            cloudTexture = content.Load<Texture2D>("BgElements/Cloud_Background1");

            playerTexture = content.Load<Texture2D>("Player_Animation_Full");
            playerPosition = new Vector2(120, ScreenManager.Game.GraphicsDevice.Viewport.Height - 50 - playerTexture.Height/3);
            playerAnimation.Initialize(playerTexture, playerPosition, new Point(100, 169), new Point(12, 2), playerFrameTime, Color.White, true);
            player.Initialize(playerAnimation, playerPosition);
            player.ViewPort = ScreenManager.Game.GraphicsDevice.Viewport;

            enemyTexture = content.Load<Texture2D>("Enemies_Animation_Full");
            evilCatTexture = content.Load<Texture2D>("surprise_Anim");

            powerUpTexture = content.Load<Texture2D>("PowerUps/powerUps");

            explosionTexture = content.Load<Texture2D>("buf_Animation");

            healthTexture = content.Load<Texture2D>("Health_Bar_Full");
            healthBar.Initialize(healthTexture, new Vector2(5, 5), healthBarIndex);
            slowdownTexture = content.Load<Texture2D>("PowerUps/Slow_Down");

            jumpingSound = content.Load<SoundEffect>("SoundEffects/JumpingSound");
            crouchingSound = content.Load<SoundEffect>("SoundEffects/SlidingSound");
            windSound = content.Load<SoundEffect>("SoundEffects/WindSound");
            runningSound = content.Load<SoundEffect>("SoundEffects/RunningSound");
            bufSound = content.Load<SoundEffect>("SoundEffects/buf");
            powerUpGainSound = content.Load<SoundEffect>("SoundEffects/HealthGainSound");
            slowdownSound = content.Load<SoundEffect>("SoundEffects/slowdownSound");
            gameOverSound = content.Load<SoundEffect>("SoundEffects/GameOverSound");

            bufSoundInstance = bufSound.CreateInstance();
            runningSoundInstance = runningSound.CreateInstance();
            jumpingSoundInstance = jumpingSound.CreateInstance();
            crouchingSoundInstance = crouchingSound.CreateInstance();
            windSoundInstance = windSound.CreateInstance();
            windSoundInstance.IsLooped = true;
            crouchingSoundInstance.IsLooped = true;
            bufSoundInstance.Pitch = 1.0f;

            ScreenManager.Game.ResetElapsedTime();
            MediaPlayer.Volume = 0.7f;

        }

    

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Handle Input


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

                if (playMusic)
                {
                    PlaySong(gameplayMusic);
                    playMusic = false;
                }

                if (otherScreenHasFocus)
                {
                    SoundOffMusicLow();
                }
                else
                {

                    if (MediaPlayer.Volume != 0.7f)
                    MediaPlayer.Volume += 0.1f;

                    dealuriBackground.Update();
                    groundBackground.Update();
                    SkyBackground.Update();

                    player.Update(gameTime);

                    SunMoonUpdate();

                    UpdatePlayer();

                    sunAnimation.Update(gameTime);

                    UpdateCloudsLeft(gameTime);

                    UpdateEnemy(gameTime);

                    UpdatePowerUps(gameTime);

                    UpdateCollision();

                    UpdateExplosions(gameTime);

                    UpdateHealth();

                    UpdateSpeed(gameTime);

                    ChangeMusic();

                    UpdateSurprises(gameTime);
                }

                

            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

        }

        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState currentKeyboardState = input.CurrentKeyboardStates[playerIndex];

            if (input.IsPauseGame(ControllingPlayer))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }

        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime)
        {
            DayAndNight(gameTime);

            SlowDownNotifyColor(gameTime);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            DrawBackgroundElements(spriteBatch);

            DrawEnemiesAndPowerUps(spriteBatch);

            player.Draw(spriteBatch);

            DrawHudAndExplosions(spriteBatch);

            NotifyStrings(gameTime, spriteBatch);

            spriteBatch.End();


            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }

        #endregion

        #region Helper Methods

        #region Misc Methods

        void SoundOffMusicLow()
        {
            if (isGameOver == false)
            {
                if (MediaPlayer.Volume >= 0.1f)
                    MediaPlayer.Volume -= 0.1f;
            }

            if (runningSoundInstance.IsDisposed == false)
                runningSoundInstance.Stop();

            if (jumpingSoundInstance.IsDisposed == false)
                jumpingSoundInstance.Stop();

            if (crouchingSoundInstance.IsDisposed == false)
                crouchingSoundInstance.Stop();

            if (crouchingSoundInstance.IsDisposed == false)
                windSoundInstance.Stop();
        }

        void SunMoonUpdate()
        {
            sunAnimation.position.X -= sunMoveSpeed;

            if (sunAnimation.Position.X < -sunTexture.Width / 8)
            {
                sunAnimation.Position = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width, -70);
            }

            if (gameplayMusic.IsDisposed == false)
            {
                if (gameplayMusic.Name.EndsWith("Track2") || gameplayMusic.Name.EndsWith("Track4"))
                {
                    isEvil = true;
                    sunAnimation.CurrentFrame = new Point(sunAnimation.CurrentFrame.X, 1);
                }
                else
                {
                    isEvil = false;
                    sunAnimation.CurrentFrame = new Point(sunAnimation.CurrentFrame.X, 0);
                }
            }
        }

        #endregion

        #region Clouds

        void AddCloudLeft()
        {
            Cloud cloud = new Cloud();

            Vector2 position = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width + cloudTexture.Width * 4,
                random.Next(20, ScreenManager.Game.GraphicsDevice.Viewport.Height / 2 - cloudTexture.Height));

            cloud.Initialize(cloudTexture, position, cloud.Scale());

            cloudsLeft.Add(cloud);

        }

        void UpdateCloudsLeft(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - cloudPreviousSpawnTimeLeft > cloudSpawnTimeLeft)
            {
                cloudPreviousSpawnTimeLeft = gameTime.TotalGameTime;


                AddCloudLeft();
            }

            for (int i = cloudsLeft.Count - 1; i >= 0; i--)
            {
                cloudsLeft[i].UpdateLeft(gameTime);

                if (cloudsLeft[i].Active == false)
                {
                    cloudsLeft.RemoveAt(i);
                }
            }

        }

        #endregion

        #region Enemies

        void AddEnemy()
        {

            Animation enemyAnimation = new Animation();

            EnemyRandomizer();

            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, enemyFrameSize, enemyTotalFrames, enemyFrameTime, Color.White, true);
            enemyAnimation.CurrentFrame = enemyCurrentFrame;

            Vector2 Position = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width + enemyTexture.Width,
                random.Next(ScreenManager.Game.GraphicsDevice.Viewport.Height / 2, ScreenManager.Game.GraphicsDevice.Viewport.Height - 50 - enemyFrameSize.Y));

            Enemy enemy = new Enemy();

            enemy.Initialization(enemyAnimation, Position, enemyMoveSpeed);

            enemies.Add(enemy);

        }


        void UpdateEnemy(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - enemyPreviousSpawnTime > enemySpawnTime)
            {
                enemyPreviousSpawnTime = gameTime.TotalGameTime;

                AddEnemy();
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                enemies[i].enemyMoveSpeed = enemyMoveSpeed;

                if (enemies[i].Active == false)
                {
                    if (enemies[i].Health <= 0)
                    {
                        bufSoundInstance.Play();
                    }
                    enemies.RemoveAt(i);
                }
            }

        }

        void EnemyRandomizer()
        {
            int flag = random.Next(0, 3);

            switch (flag)
            {
                case 1: { enemyFrameSize = new Point(125, 125); enemyCurrentFrame = new Point(0, 0); enemyTotalFrames = new Point(3, 1); enemyFrameTime = 70f; break; }
                case 2: { enemyFrameSize = new Point(300, 125); enemyCurrentFrame = new Point(0, 1); enemyTotalFrames = new Point(2, 1); enemyFrameTime = 45f; break; }
                default: { enemyFrameSize = new Point(125, 125); enemyCurrentFrame = new Point(0, 0); enemyTotalFrames = new Point(3, 1); enemyFrameTime = 70f; break; }
            }

        }
        #endregion

        #region Collisions

        void UpdateCollision()
        {



            Rectangle playerRectangle = new Rectangle((int)player.PlayerAnimation.Position.X,
                                                      (int)player.PlayerAnimation.Position.Y,
                                                           player.PlayerAnimation.FrameSize.X,
                                                           player.PlayerAnimation.FrameSize.Y);

            for (int i = 0; i < enemies.Count - 1; i++)
            {



                Rectangle enemyRectangleFull = new Rectangle((int)enemies[i].EnemyAnimation.Position.X,
                                                         (int)enemies[i].EnemyAnimation.Position.Y,
                                                         enemies[i].EnemyAnimation.FrameSize.X,
                                                         enemies[i].EnemyAnimation.FrameSize.Y);


                if (playerRectangle.Intersects(enemyRectangleFull))
                {

                    Rectangle enemyPreciseRect = new Rectangle();

                    if (enemies[i].EnemyAnimation.CurrentFrame.Y == 0)
                    {
                        enemyPreciseRect = new Rectangle((int)enemies[i].EnemyAnimation.Position.X + 27, (int)enemies[i].EnemyAnimation.Position.Y + 20, enemies[i].EnemyAnimation.FrameSize.X - 25, enemies[i].EnemyAnimation.FrameSize.Y - 15);
                    }
                    else if (enemies[i].EnemyAnimation.CurrentFrame.Y == 1)
                    {
                        enemyPreciseRect = new Rectangle((int)enemies[i].EnemyAnimation.Position.X + 15, (int)enemies[i].EnemyAnimation.Position.Y + 90, enemies[i].EnemyAnimation.FrameSize.X - 15, enemies[i].EnemyAnimation.FrameSize.Y - 100);
                    }

                    if (IsCollision(enemyPreciseRect))
                    {
                        AddExplosion(enemies[i].Position);
                        player.Health -= 10;
                        enemies[i].Health = 0;
                        enemies[i].Active = false;
                    }

                }

            }

            for (int i = powerUpsHealth.Count - 1; i >= 0; i--)
            {
                Rectangle powerUpRectangle = new Rectangle((int)powerUpsHealth[i].position.X, (int)powerUpsHealth[i].position.Y, 75, 75);

                if (playerRectangle.Intersects(powerUpRectangle))
                {
                    Rectangle powerUpRectPrecise = new Rectangle((int)powerUpsHealth[i].position.X + 10, (int)powerUpsHealth[i].position.Y + 10, 65, 65);

                    if (IsCollision(powerUpRectPrecise))
                    {
                        if (powerUpsHealth[i].WhichPowerUp(0))
                        {
                            if (player.Health < 40)
                            {
                                powerUpGainSound.Play();
                                player.Health += 10;
                            }
                            powerUpsHealth[i].Active = false;

                            if (player.Health == 40)
                            {
                                healing = false;
                                powerUpsHealth.RemoveAt(i);
                            }
                        }

                    }

                }

            }

            for (int i = powerUpsSpeed.Count - 1; i >= 0; i--)
            {
                Rectangle powerUpRectangle = new Rectangle((int)powerUpsSpeed[i].position.X, (int)powerUpsSpeed[i].position.Y, 75, 75);

                if (playerRectangle.Intersects(powerUpRectangle))
                {
                    Rectangle powerUpRectPrecise = new Rectangle((int)powerUpsSpeed[i].position.X + 10, (int)powerUpsSpeed[i].position.Y + 10, 65, 65);

                    if (IsCollision(powerUpRectPrecise))
                    {
                        if (powerUpsSpeed[i].WhichPowerUp(1))
                        {
                            powerUpGainSound.Play();
                            isSlowDown = true;
                            powerUpsSpeed.RemoveAt(i);
                        }

                    }

                }

            }

        }

        bool IsCollision(Rectangle rectB)
        {
            if (player.PlayerAnimation.CurrentFrame.Y == 0)
            {
                Rectangle headRect = new Rectangle((int)player.PlayerAnimation.Position.X + 15, (int)player.PlayerAnimation.Position.Y + 45, player.PlayerAnimation.FrameSize.X - 30, player.PlayerAnimation.FrameSize.Y - 107);
                Rectangle corpseRect = new Rectangle((int)player.PlayerAnimation.Position.X + 42, (int)player.PlayerAnimation.Position.Y + 62, player.PlayerAnimation.FrameSize.X - 80, player.PlayerAnimation.FrameSize.Y - 69);
                Rectangle legsRect = new Rectangle((int)player.PlayerAnimation.Position.X + 10, (int)player.PlayerAnimation.Position.Y + 107, player.PlayerAnimation.FrameSize.X - 50, player.PlayerAnimation.FrameSize.Y);

                if (headRect.Intersects(rectB) || corpseRect.Intersects(rectB) || legsRect.Intersects(rectB))
                {
                    return true;

                }
            }
            else if (player.PlayerAnimation.CurrentFrame.Y == 1)
            {
                Rectangle headRect = new Rectangle((int)player.PlayerAnimation.Position.X + 30, (int)player.PlayerAnimation.Position.Y + 50, player.PlayerAnimation.FrameSize.X - 10, player.PlayerAnimation.FrameSize.Y - 118);
                Rectangle corpseRect = new Rectangle((int)player.PlayerAnimation.Position.X + 52, (int)player.PlayerAnimation.Position.Y + 62, player.PlayerAnimation.FrameSize.X - 95, player.PlayerAnimation.FrameSize.Y - 69);
                Rectangle legsRect = new Rectangle((int)player.PlayerAnimation.Position.X + 72, (int)player.PlayerAnimation.Position.Y + 120, player.PlayerAnimation.FrameSize.X - 105, player.PlayerAnimation.FrameSize.Y - 60);

                if (headRect.Intersects(rectB) || corpseRect.Intersects(rectB) || legsRect.Intersects(rectB))
                {
                    return true;
                }

            }
            else if (player.PlayerAnimation.CurrentFrame.Y == 2)
            {
                Rectangle headRect = new Rectangle((int)player.PlayerAnimation.Position.X + 25, (int)player.PlayerAnimation.Position.Y + 105, player.PlayerAnimation.FrameSize.X - 146, player.PlayerAnimation.FrameSize.Y - 32);
                Rectangle corpseRect = new Rectangle((int)player.PlayerAnimation.Position.X + 58, (int)player.PlayerAnimation.Position.Y + 122, player.PlayerAnimation.FrameSize.X - 130, player.PlayerAnimation.FrameSize.Y - 13);
                Rectangle legsRect = new Rectangle((int)player.PlayerAnimation.Position.X + 96, (int)player.PlayerAnimation.Position.Y + 160, player.PlayerAnimation.FrameSize.X - 95, player.PlayerAnimation.FrameSize.Y);

                if (headRect.Intersects(rectB) || corpseRect.Intersects(rectB) || legsRect.Intersects(rectB))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Player

        void UpdatePlayer()
        {
            windSoundInstance.Play();
            windSoundInstance.Volume = 0.8f;

            if (runningState == true)
            {
                if (player.PlayerAnimation.CurrentFrame.X >= 8 && player.PlayerAnimation.CurrentFrame.X <= 12 && player.PlayerAnimation.CurrentFrame.Y == 0)
                    runningSoundInstance.Play();
                else
                    runningSoundInstance.Stop();
            }
            else
            {
                runningSoundInstance.Stop();
            }

            if (previousKeyboardState.IsKeyDown(Keys.Space) || previousKeyboardState.IsKeyDown(Keys.Up) && player.PlayerAnimation.CurrentFrame.Y == 1 && player.PlayerAnimation.CurrentFrame.X == 3)
            {
                    runningState = false;
                    crouchingState = false;
                    jumpingSoundInstance.Play();
            }


            if ((previousKeyboardState.IsKeyDown(Keys.Down) ||
                previousKeyboardState.IsKeyDown(Keys.LeftControl) ||
                previousKeyboardState.IsKeyDown(Keys.RightControl)) &&
                player.PlayerAnimation.CurrentFrame.Y == 2 &&
                player.PlayerAnimation.CurrentFrame.X >= 3 &&
                player.PlayerAnimation.CurrentFrame.X <= 5)
            {
                runningState = false;
                crouchingState = true;
            }

            if (player.PlayerAnimation.CurrentFrame.Y != 2)
            {
                runningState = true;
                crouchingState = false;
            }

            if (crouchingState == true)
            {
                crouchingSoundInstance.Play();
            }
            else
            {
                crouchingSoundInstance.Stop();
            }

            if (previousKeyboardState.IsKeyDown(Keys.RightShift))
            {
                if (currentKeyboardState.IsKeyUp(Keys.RightShift))
                {
                    if (isSlowDown)
                    {
                        slowdownSound.Play();
                        groundMoveSpeed -= 4;
                        dealuriMoveSpeed -= 3;
                        enemyMoveSpeed -= 4;
                        enemySpawnTime += TimeSpan.FromSeconds(0.5f);
                        powerUpMoveSpeed -= 3;
                        isSlowDown = false;
                        speedPress = 1;
                    }
                    else
                    {
                        showNoSpeedPU = true;
                    }
                }
            }
        }

        #endregion

        #region Explosion

        void AddExplosion(Vector2 position)
        {
            Animation explosion = new Animation();
            explosion.Initialize(explosionTexture, position, new Point(100, 100), new Point(13, 0), 50f, Color.White, false);
            explosions.Add(explosion);
        }

        void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);

                if (explosions[i].Active == false)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        #endregion

        #region PowerUps

        void AddPowerUpHealth()
        {
            if (healing == false)
            {
                PowerUp powerUp = new PowerUp();

                powerUp.Initialize(powerUpTexture,
                    new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + powerUpTexture.Width * 6,
                        random.Next(ScreenManager.Game.GraphicsDevice.Viewport.Height / 2, ScreenManager.Game.GraphicsDevice.Viewport.Height - 50 - powerUpTexture.Height)),
                        0, powerUpMoveSpeed);

                powerUpsHealth.Add(powerUp);
                healing = true;
            }
        }

        void AddPowerUpSpeed()
        {
            if (speeding == false)
            {
                PowerUp powerUp = new PowerUp();

                powerUp.Initialize(powerUpTexture,
                    new Vector2(ScreenManager.GraphicsDevice.Viewport.Width + powerUpTexture.Width * 6,
                        random.Next(ScreenManager.Game.GraphicsDevice.Viewport.Height / 2,
                        ScreenManager.Game.GraphicsDevice.Viewport.Height - 50 - powerUpTexture.Height)),
                        1, powerUpMoveSpeed);

                powerUpsSpeed.Add(powerUp);
                speeding = true;
            }
        }

        void UpdatePowerUps(GameTime gameTime)
        {
            if (healthLower)
            {
                if (gameTime.TotalGameTime - powerUpPreviousSpawnTime > powerUpSpawnTime)
                {
                    powerUpPreviousSpawnTime = gameTime.TotalGameTime;

                    AddPowerUpHealth();
                }

                for (int i = powerUpsHealth.Count - 1; i >= 0; i--)
                {
                    powerUpsHealth[i].Update(gameTime);
                    powerUpsHealth[i].powerUpMoveSpeed = powerUpMoveSpeed;

                    if (powerUpsHealth[i].Active == false)
                    {
                        powerUpsHealth.RemoveAt(i);
                        healing = false;
                    }
                }
            }
            else
            {
                for (int i = powerUpsHealth.Count - 1; i >= 0; i--)
                {
                    powerUpsHealth[i].Update(gameTime);
                    powerUpsHealth[i].powerUpMoveSpeed = powerUpMoveSpeed;

                    if (powerUpsHealth[i].Active == false)
                    {
                        powerUpsHealth.RemoveAt(i);
                        healing = false;
                    }
                }
            }


            if (speedPress == 0)
            {
                if (speedALot)
                {
                    if (isSlowDown == false)
                    {
                        if (gameTime.TotalGameTime - powerUpPreviousSpawnTime > powerUpSpawnTime)
                        {
                            powerUpPreviousSpawnTime = gameTime.TotalGameTime;

                            AddPowerUpSpeed();

                        }

                        for (int i = powerUpsSpeed.Count - 1; i >= 0; i--)
                        {
                            powerUpsSpeed[i].Update(gameTime);
                            powerUpsSpeed[i].powerUpMoveSpeed = powerUpMoveSpeed;

                            if (powerUpsSpeed[i].Active == false)
                            {
                                powerUpsSpeed.RemoveAt(i);
                                speeding = false;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = powerUpsSpeed.Count - 1; i >= 0; i--)
                {
                    powerUpsSpeed[i].Update(gameTime);
                    powerUpsSpeed[i].powerUpMoveSpeed = powerUpMoveSpeed;

                    if (powerUpsSpeed[i].Active == false)
                    {
                        powerUpsSpeed.RemoveAt(i);
                        speeding = false;
                    }
                }
            }

        }

        #endregion

        #region Health

        void UpdateHealth()
        {
            switch (player.Health)
            {
                case 40:
                    {
                        healthBarIndex = 0;
                        break;
                    }
                case 30:
                    {
                        healthBarIndex = 1;
                        break;
                    }
                case 20:
                    {
                        healthBarIndex = 2;
                        break;
                    }
                case 10:
                    {
                        healthBarIndex = 3;
                        break;
                    }
                case 0:
                    {
                        isGameOver = true;
                        MediaPlayer.Stop();
                        gameOverSound.Play();
                        ScreenManager.AddScreen(new GameOverScreen(distanceRan), PlayerIndex.One);
                        break;
                    }
            }

            healthBar.healthBarIndex = healthBarIndex;

            healthBar.Update();

            if (player.Health < 40)
                healthLower = true;
            else
                healthLower = false;

        }

        #endregion

        #region Speed Up Algorythm

        void UpdateSpeed(GameTime gameTime)
        {

            elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

            
            if (elapsedTime > metersPerMilliseconds)
            {
                distanceCounter += 1;
                distanceRan += 1;
                elapsedTime = 0;
            }

            if (distanceMultiplier > 10 && distanceMultiplier < 160)
            {
                speedALot = true;
            }
            else
            {
                speedALot = false;
            }

            if (distanceCounter > distanceMultiplier)
            {
                metersPerMilliseconds -= 1;
                dealuriMoveSpeed += 0.1f;
                enemyMoveSpeed += 0.1f;
                groundMoveSpeed += 0.1f;
                powerUpMoveSpeed += 0.1f;

                for (int i = cloudsLeft.Count - 1; i >= 0; i--)
                {
                    cloudsLeft[i].cloudMoveSpeed += 0.5f;
                }

                cloudMoveSpeed += 0.1f;
                cloudSpawnTimeLeft -= TimeSpan.FromMilliseconds(15);

                if (enemySpawnTime >= TimeSpan.FromSeconds(1))
                enemySpawnTime -= TimeSpan.FromMilliseconds(15);

                powerUpSpawnTime -= TimeSpan.FromMilliseconds(15);
                distanceCounter = 0;

                if (enemyMoveSpeed > 15)
                    enemySpawnTime -= TimeSpan.FromSeconds(0.01f);
            }

            dealuriBackground.Speed = dealuriMoveSpeed;
            groundBackground.Speed = groundMoveSpeed;
            SkyBackground.Speed = skyMoveSpeed;

            switch (metersPerMilliseconds)
            {
                case 160:
                    {
                        distanceMultiplier = 5;
                        break;
                    }
                case 150:
                    {
                        distanceMultiplier = 10;
                        break;
                    }
                case 130:
                    {
                        distanceMultiplier = 20;
                        break;
                    }
                case 110:
                    {
                        distanceMultiplier = 20;
                        break;
                    }
                case 80:
                    {
                        distanceMultiplier = 20;
                        break;
                    }
                case 0:
                    {
                        distanceMultiplier = 20;
                        break;
                    }
            }

        }

        #endregion

        #region Draw Methods

        //Schimbarea culorilor textului si imaginii slowDown PowerUp
        void SlowDownNotifyColor(GameTime gameTime)
        {

            if (isSlowDown == false)
            {
                slowdownColor = new Color(0, 0, 0, 125);
            }
            else
            {
                slowdownColor = Color.White;
            }

            if (isSlowDown)
            {
                notifyCountColorGray += gameTime.ElapsedGameTime.Milliseconds;
                if (notifyCountColorGray < 500)
                {
                    notifySlowdownColor = Color.Gray;
                }
                else if (notifyCountColorGray > 500)
                {
                    notifyCountColorBlue += gameTime.ElapsedGameTime.Milliseconds;

                    if (notifyCountColorBlue < 500)
                    {
                        notifySlowdownColor = Color.Blue;
                    }
                    else
                    {
                        notifyCountColorGray = 0;
                        notifyCountColorBlue = 0;
                    }
                }
            }
            else
            {
                notifySlowdownColor = Color.Gray;
            }
        }

        // Notificarea Playerului in privinta slowDown powerUp
        void NotifyStrings(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (isSlowDown)
            {
                spriteBatch.DrawString(slowDownNotify, " Active(Press Right Shift)", new Vector2(42, 38), notifySlowdownColor);
            }
            else
            {
                spriteBatch.DrawString(slowDownNotify, " Not Active", new Vector2(42, 38), notifySlowdownColor);
            }

            if (showNoSpeedPU == true)
            {
                speedNotifyCount += gameTime.ElapsedGameTime.Milliseconds;
                if (speedNotifyCount < 2000)
                {
                    if (speedPress == 0)
                        spriteBatch.DrawString(notifyFont, "SlowDown is not active", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 300, 100), Color.Red);
                    else if (speedPress == 1)
                        spriteBatch.DrawString(notifyFont, "You already used SlowDown", new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - 300, 100), Color.Red);
                }
                else if (speedNotifyCount > 2000)
                {
                    showNoSpeedPU = false;
                    speedNotifyCount = 0;
                }
            }
        }

        void DayAndNight(GameTime gameTime)
        {
            if (isEvil)
            {
                if (evilColor.B == 125 && evilColor.G == 0 && evilColor.R == 0)
                {
                }
                else
                {
                    evilElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (evilElapsedTime > 5)
                    {
                        if (evilColor.R != 0)
                            evilColor.R -= 5;
                        if (evilColor.G != 0)
                            evilColor.G -= 5;
                        if (evilColor.B != 125)
                            evilColor.B -= 5;
                        evilElapsedTime = 0;
                    }

                }
                ScreenManager.GraphicsDevice.Clear(evilColor);
            }
            else
            {
                if (evilColor.B == 255 && evilColor.G == 255 && evilColor.R == 255)
                {

                }
                else
                {
                    evilElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (evilElapsedTime > 5)
                    {
                        if (evilColor.R != 255)
                            evilColor.R += 5;
                        if (evilColor.G != 255)
                            evilColor.G += 5;
                        if (evilColor.B != 255)
                            evilColor.B += 5;
                        evilElapsedTime = 0;
                    }
                }
                ScreenManager.GraphicsDevice.Clear(evilColor);
            }
        }

        void DrawBackgroundElements(SpriteBatch spriteBatch)
        {
            SkyBackground.Draw(spriteBatch);
            sunAnimation.Draw(spriteBatch);

            for (int i = 0; i < cloudsLeft.Count - 1; i++)
            {
                cloudsLeft[i].Draw(spriteBatch, SpriteEffects.None);
            }

            for (int i = evilCats.Count - 1; i >= 0; i--)
            {
                evilCats[i].Draw(spriteBatch);
            }

            dealuriBackground.Draw(spriteBatch);
            groundBackground.Draw(spriteBatch);
        }

        void DrawEnemiesAndPowerUps(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < enemies.Count - 1; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            for (int i = powerUpsHealth.Count - 1; i >= 0; i--)
            {
                powerUpsHealth[i].Draw(spriteBatch);
            }

            for (int i = powerUpsSpeed.Count - 1; i >= 0; i--)
            {
                powerUpsSpeed[i].Draw(spriteBatch);
            }
        }

        void DrawHudAndExplosions(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Draw(spriteBatch);
            }

            healthBar.Draw(spriteBatch);

            if (isEvil)
            {
                spriteBatch.DrawString(gameFont, "Distance: " + distanceRan, new Vector2(160, 5), Color.White);
            }
            else
            {
                spriteBatch.DrawString(gameFont, "Distance: " + distanceRan, new Vector2(160, 5), Color.Black);
            }

            spriteBatch.Draw(slowdownTexture, new Rectangle(5, 30, 37, 37), slowdownColor);
        }

        #endregion

        #region Music Methods

        void ChangeMusic()
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                musicPath = MusicRandomizer();
                if (gameplayMusic.IsDisposed == false)
                gameplayMusic.Dispose();

                gameplayMusic = content.Load<Song>(musicPath);
                PlaySong(gameplayMusic);
            }
        }

        void PlaySong(Song song)
        {
            try
            {
                MediaPlayer.Play(song);

                MediaPlayer.IsRepeating = false;
            }
            catch { }
        }

        string MusicRandomizer()
        {
            int flag = random.Next(1, 5);

            switch (flag)
            {
                case 1: { return "Music/ChilloutMode/Track1"; }
                case 2: { return "Music/ChilloutMode/Track2"; }
                case 3: { return "Music/ChilloutMode/Track3"; }
                default: { return "Music/ChilloutMode/Track4"; }
            }
        }

        #endregion

        #region Surprise Animation

        void AddSurprise()
        {
            Animation catAnimation = new Animation();

            catAnimation.Initialize(evilCatTexture, Vector2.Zero, new Point(125, 50), new Point(11, 0), 30f, Color.White, true);

            Vector2 Position = new Vector2(ScreenManager.Game.GraphicsDevice.Viewport.Width + evilCatTexture.Width,
                random.Next(0, ScreenManager.Game.GraphicsDevice.Viewport.Height / 2 - catAnimation.FrameSize.Y));

            EvilCatSurprise evilCat = new EvilCatSurprise();

            evilCat.Initialization(catAnimation, Position, enemyMoveSpeed);

            evilCats.Add(evilCat);

        }

        void UpdateSurprises(GameTime gameTime)
        {
            if (isEvil == true)
            {
                if (gameTime.TotalGameTime - evilCatPreviousSpawnTime > evilCatSpawnTime)
                {
                    evilCatPreviousSpawnTime = gameTime.TotalGameTime;

                    AddSurprise();
                }
            }
            for (int i = evilCats.Count - 1; i >= 0; i--)
            {
                evilCats[i].Update(gameTime);
                evilCats[i].catMoveSpeed = enemyMoveSpeed;

                if (evilCats[i].Active == false)
                {
                    evilCats.RemoveAt(i);
                }
            }
            

        }

        #endregion

        #endregion

    }
}
