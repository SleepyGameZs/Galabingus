using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

// Wabungus Corpsungus Duplicatungus
// 2023, 4, 30
// Galabingus

namespace Galabingus
{
    /// <summary>
    ///  Seperator for what can colloide with what,
    ///  Anything that is not on the same collision layer can collide
    /// </summary>
    enum CollisionGroup
    {
        None,
        Player,
        FromPlayer,
        Destroyable,
        Tile,
        Bullet,
        Enemy
    }

    public class Galabingus : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // GameObject Dynamic
        private dynamic content;

        // Temporary Background
        private Texture2D tempBackground;

        // UI Object
        private UIManager userInterface;

        // Player GameObject
        private Player player;

        // Shaders
        private Effect shaders;
        private byte colliderTimer;
        private bool transition;

        // Enemies and Bullets
        private BulletManager mng_bullet;
        private EnemyManager mng_enemy;

        // Camera
        private Camera camera;

        // Tiles 
        private TileManager tileManager;

        // Tile / Enemy Position & Data
        private List<int[]> l_a4_obj_enemyData;

        // Reset
        private delegate void ResetGameStates();
        private static ResetGameStates ResetState;

        // Music
        private Song backgroundMusic;

        // Intilizes the mouse state, graphics device and ContentManager root directory
        public Galabingus()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // Initlizes the size of the window, colliders update timer, and Reset delegate
        protected override void Initialize()
        {
            // Apply the size of the windwo
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 920;
            _graphics.ApplyChanges();
            
            // Set the collider update timer
            colliderTimer = 0;

            // Initlize the base
            base.Initialize();

            // Reset Delatgate
            ResetState = delegate ()
            {
                // Create a sprite batch
                _spriteBatch = new SpriteBatch(GraphicsDevice);
                
                // Load in the universal shaders
                shaders = Content.Load<Effect>("shaders");
                
                // Initilize the GameObject Instance and Content Dynamic (Always goes first)
                content = GameObject.Instance.Initialize(Content, GraphicsDevice, _spriteBatch, shaders);

                //creates and initializes the UIManager, and then loads its contents
                userInterface = UIManager.Instance;
                userInterface.Initialize(_graphics, Content, _spriteBatch);
                userInterface.LoadContent();

                // Set tile / enemy data
                l_a4_obj_enemyData = new List<int[]>();

                // ROUGH EXAMPLE FORMAT I USED HERE (we can change it, I just wanted to have something testable
                /* [0] -> Is this an enemy? (1 or 0)
                 * [1] -> What kind of enemy (checks within bounds of EnemyType Enum)
                 * [2] -> X Position
                 * [3] -> Y Position
                 */
                l_a4_obj_enemyData = GameObject.Instance.LoadEnemyLeveFile("GalabingusLevel.level");

                // Create a player
                player = new Player(new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Height * 0.01375f, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.01375f), content.player_strip4);
                player.Position = new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Width * 0.5f - Player.PlayerInstance.Transform.Width, GameObject.Instance.GraphicsDevice.Viewport.Height - Player.PlayerInstance.Transform.Height * 10);

                // Create Bullet Manager
                mng_bullet = BulletManager.Instance;

                // Create Enemy Manager + Load data
                mng_enemy = EnemyManager.Instance.Initialize(l_a4_obj_enemyData);

                // Create Camera
                camera = Camera.Instance;
                camera.InitalCameraScroll = -2f;

                // Create Tile Manager
                tileManager = TileManager.Instance;
                GameObject.Instance.LoadTileLevelFile("GalabingusLevel.level");
                tileManager.CreateBackground();
            };

        }

        /// <summary>
        ///  Loads in the level, player, enmies, ui and sound
        /// </summary>
        protected override void LoadContent()
        {
            // Create the sprite batch
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load the universal shaders
            shaders = Content.Load<Effect>("shaders");

            // Initilize the GameObject Instance and Content Dynamic (Always goes first)
            content = GameObject.Instance.Initialize(Content, GraphicsDevice, _spriteBatch, shaders);

            //creates and initializes the UIManager, and then loads its contents
            userInterface = UIManager.Instance;
            userInterface.Initialize(_graphics, Content, _spriteBatch);
            userInterface.LoadContent();

            // Set tile / enemy data
            l_a4_obj_enemyData = new List<int[]>();

            // ROUGH EXAMPLE FORMAT I USED HERE (we can change it, I just wanted to have something testable
            /* [0] -> Is this an enemy? (1 or 0)
             * [1] -> What kind of enemy (checks within bounds of EnemyType Enum)
             * [2] -> X Position
             * [3] -> Y Position
             */
            l_a4_obj_enemyData = GameObject.Instance.LoadEnemyLeveFile("GalabingusLevel.level");

            // Create a player
            player = new Player(new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Height * 0.01375f, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.01375f), content.player_strip4);
            player.Position = new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Width * 0.5f - Player.PlayerInstance.Transform.Width, GameObject.Instance.GraphicsDevice.Viewport.Height - Player.PlayerInstance.Transform.Height * 10);
            player.Health = 5;

            // Create Bullet Manager
            mng_bullet = BulletManager.Instance;

            // Create Enemy Manager + Load data
            mng_enemy = EnemyManager.Instance.Initialize(l_a4_obj_enemyData);

            // Create Camera
            camera = Camera.Instance;
            camera.InitalCameraScroll = -2f;

            // Create Tile Manager
            tileManager = TileManager.Instance;
            GameObject.Instance.LoadTileLevelFile("GalabingusLevel.level");
            tileManager.CreateBackground();

            #region Sound Loading
            #region Bullets

            AudioManager.Instance.AddSound("Fire", 0.1f, "Bullet Fire", Content);
            AudioManager.Instance.AddSound("Big Shot", 0.25f, "Big Shot", Content);
            AudioManager.Instance.AddSound("Enemy Fire", 0.75f, "Enemy Fire", Content);
            AudioManager.Instance.AddSound("Homing", 0.75f, "Homing Shot", Content);
            AudioManager.Instance.AddSound("Scatter", 0.25f, "Scatter Shot", Content);
            AudioManager.Instance.AddSound("Split", 0.75f, "Split Shot", Content);
            AudioManager.Instance.AddSound("Break", 0.1f, "Split Shot Break", Content);
            AudioManager.Instance.AddSound("Wave", 0.75f, "Wave Shot", Content);
            AudioManager.Instance.AddSound("Purple Scatter", 1f, "Purple Scatter Shot1", Content);
            AudioManager.Instance.AddSound("Purple Break", 0.25f, "Purple Break", Content);
            AudioManager.Instance.AddSound("Explosion", 0.1f, "Explosion", Content);

            #endregion

            #region Player Sounds

            AudioManager.Instance.AddSound("Charge", 1f, "Charge", Content);
            AudioManager.Instance.AddSound("Hit", 1f, "Player Hit", Content);

            #endregion

            #region Menu Sounds

            AudioManager.Instance.AddSound("Menu Select", 0.25f, "Menu Select", Content);
            AudioManager.Instance.AddSound("Menu Confirm", 0.75f, "Menu Confirm", Content);
            AudioManager.Instance.AddSound("Victory", 1f, "Victory", Content);
            AudioManager.Instance.AddSound("Game Over", 1f, "Game Over", Content);

            #endregion

            #region Music

            backgroundMusic = Content.Load<Song>("Background Music");
            AudioManager.Instance.SongCollection.Add(backgroundMusic);
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;

            #endregion
            #endregion
        }

        /// <summary>
        ///  Updates all of Galabingus
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update the game when not reseting
            if (!UIManager.Instance.IsReset)
            {

                // Determine the status of the Graphics Device and allow for colliders to create render targets
                if (GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal && colliderTimer == 0)
                {
                    GameObject.Instance.HoldCollider = false;
                }

                //Update the UI
                userInterface.Update();
                GameObject.Instance.PlayBossEffect();
                shaders.Parameters["bossEffect"].SetValue(GameObject.Instance.IsBossEffectActive);
                shaders.Parameters["shadeFadeTime"].SetValue(GameObject.Instance.TimeShade);
                shaders.Parameters["redShade"].SetValue(1);

                // Update the collider timer
                colliderTimer--;

                // Shift before is the transiotion state to trigger the fade
                bool shiftBefore = transition;
                transition = (userInterface.GS == GameState.PlayerDead || userInterface.GS == GameState.PlayerWins);
                
                // The UI is in the game state so update everything
                if (userInterface.GS == GameState.Game)
                {
                    // Fade in
                    shaders.Parameters["fadeIn"].SetValue(true);
                    shaders.Parameters["fadeOut"].SetValue(false);

                    // Update the player
                    player.Update(gameTime);

                    // Update the enemies
                    mng_enemy.Update(gameTime);

                    // Update the bullets
                    mng_bullet.Update(gameTime);

                    // Update the Camera
                    camera.Update(gameTime);

                    // Update the Tile Manager
                    tileManager.Update(gameTime);
                }
                else if (userInterface.GS == GameState.PlayerDead || userInterface.GS == GameState.PlayerWins)
                {
                    // Fade out
                    shaders.Parameters["fadeIn"].SetValue(false);
                    shaders.Parameters["fadeOut"].SetValue(true);
                }

                // There is a change in state for fade, so fade
                if (transition != shiftBefore)
                {
                    GameObject.Fade = 1;
                }
            }
            else
            {
                // Reset everything
                UIManager.Instance.IsReset = false;
                GameObject.Instance.Reset();
                player.Reset();
                tileManager.Reset();
                mng_bullet.Reset();
                userInterface.Reset();
                mng_enemy.Reset();
                camera.Reset();
                content = null;
                player = null;
                tileManager = null;
                mng_bullet = null;
                userInterface = null;
                l_a4_obj_enemyData = new List<int[]>();
                
                // Call the reset method to re initlize
                Reset();
            }

            // Update teh base
            base.Update(gameTime);
        }

        /// <summary>
        ///  Draw all of Galabingus
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            // Draw everyhing unless we are reseting
            if (!UIManager.Instance.IsReset)
            {
                // Clear the screen to draw the next frame
                GraphicsDevice.Clear(Color.Transparent);

                // If we are in a game state that should draw with the universal shader, draw with the universal shader
                if (!(userInterface.GS == GameState.Menu) && !(userInterface.GS == GameState.GameOver) && !(userInterface.GS == GameState.Victory))
                {
                    // ADjust fade time and begin the sprite batch
                    GameObject.Fade = GameObject.Fade * 0.96f;
                    shaders.Parameters["fade"].SetValue(GameObject.Fade);
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, effect: shaders);
                }
                else
                {
                    // Draw regularly
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);
                }

                // AS long as the game state is not in the menu, draw everything
                if (!(userInterface.GS == GameState.Menu))
                {
                    if (TileManager.Instance.CurrentSpriteNumber == 0)
                    {
                        // Draws tiles
                        tileManager.Draw();
                    }

                    if (!(userInterface.GS == GameState.GameOver) && !(userInterface.GS == GameState.Victory))
                    {
                        // Draws enemies
                        mng_enemy.Draw();

                        // Draw the player
                        player.Draw();

                        // Draws bullets
                        mng_bullet.Draw();
                    }

                    if (TileManager.Instance.CurrentSpriteNumber == 1)
                    {
                        // Draws tiles
                        tileManager.Draw();
                    }
                }

                // Draw debug stuff on top of everything
                GameObject.Instance.DebugDraw(_spriteBatch);

                //draw the screen
                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);
                userInterface.Draw();

                // End the SpriteBatch draw
                _spriteBatch.End();
            }

            // Call base draw
            base.Draw(gameTime);
        }

        /// <summary>
        ///  Triggers the Reset Delegate
        /// </summary>
        public static void Reset()
        {
            ResetState();
        }

        /// <summary>
        ///  Determines if the mosue is visible
        /// </summary>
        public void MouseVisibility(bool visibility)
        {
            IsMouseVisible = visibility;
        }
    }
}