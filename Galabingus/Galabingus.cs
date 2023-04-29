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
// 2023, 3, 13
// Galabingus
// Creates a GameObject Instance

namespace Galabingus
{

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

        private delegate void ResetGameStates();
        private static ResetGameStates ResetState;

        public Galabingus()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 920;
            colliderTimer = 0;
            _graphics.ApplyChanges();
            base.Initialize();
            ResetState = delegate ()
            {
                // Reset the player
                //player = new Player(new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Height * 0.01375f, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.01375f), content.player_strip4);
                ///player.Position = new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Width * 0.5f - Player.PlayerInstance.Transform.Width, GameObject.Instance.GraphicsDevice.Viewport.Height - Player.PlayerInstance.Transform.Height * 10);
                //player.Health = 5;

                _spriteBatch = new SpriteBatch(GraphicsDevice);

                shaders = Content.Load<Effect>("shaders");
                // Initilize the GameObject Instance and Content Dynamic (Always goes first)
                content = GameObject.Instance.Initialize(Content, GraphicsDevice, _spriteBatch, shaders);

                //creates and initializes the UIManager, and then loads its contents
                userInterface = UIManager.Instance;
                userInterface.Initialize(_graphics, Content, _spriteBatch);
                userInterface.LoadContent();

                //gets the list of enemies from the file
                //l_a4_obj_enemyData = userInterface.LevelReader(Type.Enemy);

                // Set tile / enemy data
                l_a4_obj_enemyData = new List<int[]>();

                // ROUGH EXAMPLE FORMAT I USED HERE (we can change it, I just wanted to have something testable
                /* [0] -> Is this an enemy? (1 or 0)
                 * [1] -> What kind of enemy (checks within bounds of EnemyType Enum)
                 * [2] -> X Position
                 * [3] -> Y Position
                 */

                l_a4_obj_enemyData = GameObject.Instance.LoadEnemyLeveFile("GalabingusLevel.level");

                //l_a4_obj_enemyData.Add(new int[] { 1, 6, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 0.5f), (int)(GameObject.Instance.GraphicsDevice.Viewport.Height * 1f), 1 });

                //l_a4_obj_enemyData.Add(new int[] { 1, 1, 0, 0, 0 });

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
                tileManager.CreateTile(1);

                GameObject.Instance.LoadTileLevelFile("GalabingusLevel.level");
                //tileManager.CreateObject(GameObject.Instance.Content.tile_strip26, Vector2.Zero, 25);
                //tileManager.CreateObject();

                // Load the temporary background
                //tempBackground = Content.Load<Texture2D>("spacebackground_strip1");
                tileManager.CreateBackground();
                //ushort asteroid = GameObject.Instance.Content.grayasteroid_strip1;
                //tileManager.CreateObject(asteroid,new Vector2(50,50));
                //tileManager.CreateBackground();
                //tileManager.CreateObject(GameObject.Instance.Content.grayasteroid_strip1, new Vector2(Player.PlayerInstance.Transform.Width * 2 + 100, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.5f - Player.PlayerInstance.Transform.Height + 100));

                // Sound
                //AudioManager.Instance.AddSound("Fire", 3f, "Bullet Fire", Content);
                //AudioManager.Instance.AddSound("Explosion", 0.5f, "Explosion", Content);
            };

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            shaders = Content.Load<Effect>("shaders");
            // Initilize the GameObject Instance and Content Dynamic (Always goes first)
            content = GameObject.Instance.Initialize(Content, GraphicsDevice, _spriteBatch, shaders);

            //creates and initializes the UIManager, and then loads its contents
            userInterface = UIManager.Instance;
            userInterface.Initialize(_graphics, Content, _spriteBatch);
            userInterface.LoadContent();

            //gets the list of enemies from the file
            //l_a4_obj_enemyData = userInterface.LevelReader(Type.Enemy);

            // Set tile / enemy data
            l_a4_obj_enemyData = new List<int[]>();

            // ROUGH EXAMPLE FORMAT I USED HERE (we can change it, I just wanted to have something testable
            /* [0] -> Is this an enemy? (1 or 0)
             * [1] -> What kind of enemy (checks within bounds of EnemyType Enum)
             * [2] -> X Position
             * [3] -> Y Position
             */

            l_a4_obj_enemyData = GameObject.Instance.LoadEnemyLeveFile("GalabingusLevel.level");

            //l_a4_obj_enemyData.Add(new int[] { 1, 6, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 0.5f), (int)(GameObject.Instance.GraphicsDevice.Viewport.Height * -0.4f), 1 });

            //l_a4_obj_enemyData.Add(new int[] { 1, 1, 0, 0, 0 });

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
            tileManager.CreateTile(1);

            GameObject.Instance.LoadTileLevelFile("GalabingusLevel.level");
            //tileManager.CreateObject(GameObject.Instance.Content.tile_strip26, Vector2.Zero, 25);
            //tileManager.CreateObject();

            // Load the temporary background
            //tempBackground = Content.Load<Texture2D>("spacebackground_strip1");
            tileManager.CreateBackground();
            //ushort asteroid = GameObject.Instance.Content.grayasteroid_strip1;
            //tileManager.CreateObject(asteroid,new Vector2(50,50));
            //tileManager.CreateBackground();
            //tileManager.CreateObject(GameObject.Instance.Content.grayasteroid_strip1, new Vector2(Player.PlayerInstance.Transform.Width * 2 + 100, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.5f - Player.PlayerInstance.Transform.Height + 100));

            // Sound
            AudioManager.Instance.AddSound("Fire", 0.1f, "Bullet Fire", Content);
            AudioManager.Instance.AddSound("Enemy Fire", 0.75f, "Enemy Fire", Content);
            AudioManager.Instance.AddSound("Homing", 0.75f, "Homing Shot", Content);
            AudioManager.Instance.AddSound("Scatter", 0.25f, "Scatter Shot", Content);
            AudioManager.Instance.AddSound("Split", 0.75f, "Split Shot", Content);
            AudioManager.Instance.AddSound("Break", 0.1f, "Split Shot Break", Content);
            AudioManager.Instance.AddSound("Wave", 0.75f, "Wave Shot", Content);
            AudioManager.Instance.AddSound("Explosion", 0.1f, "Explosion", Content);
            Song backgroundMuisc = Content.Load<Song>("Background Music");
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundMuisc);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (!UIManager.Instance.IsReset)
            {
                if (GraphicsDevice.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal && colliderTimer == 0)
                {
                    GameObject.Instance.HoldCollider = false;
                    //colliderTimer = 2;
                }

                //Debug.WriteLine(GameObject.Instance.TimeShade);

                //Update the UI
                userInterface.Update();
                GameObject.Instance.PlayBossEffect();
                shaders.Parameters["bossEffect"].SetValue(GameObject.Instance.IsBossEffectActive);
                shaders.Parameters["shadeFadeTime"].SetValue(GameObject.Instance.TimeShade);
                shaders.Parameters["redShade"].SetValue(1);

                colliderTimer--;

                bool shiftBefore = transition;
                transition = (userInterface.GS == GameState.PlayerDead || userInterface.GS == GameState.PlayerWins);
                if (userInterface.GS == GameState.Game)
                {
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

                    tileManager.Update(gameTime);
                }
                else if (userInterface.GS == GameState.PlayerDead || userInterface.GS == GameState.PlayerWins)
                {
                    shaders.Parameters["fadeIn"].SetValue(false);
                    shaders.Parameters["fadeOut"].SetValue(true);
                }

                if (transition != shiftBefore)
                {
                    GameObject.Fade = 1;
                }
            }
            else
            {
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
                
                Reset();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!UIManager.Instance.IsReset)
            {
                //Change the clear color to transparent and use point rendering for pixel art
                //GraphicsDevice.Clear(userInterface.ClearColor);

                GraphicsDevice.Clear(Color.Transparent);

                if (!(userInterface.GS == GameState.Menu) && !(userInterface.GS == GameState.GameOver) && !(userInterface.GS == GameState.Victory))
                {
                    GameObject.Fade = GameObject.Fade * 0.96f;
                    shaders.Parameters["fade"].SetValue(GameObject.Fade);
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, effect: shaders);
                }
                else
                {
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);
                }

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

                GameObject.Instance.DebugDraw(_spriteBatch);

                //draw the screen
                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap);
                userInterface.Draw();

                // End the SpriteBatch draw
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }



        public static void Reset()
        {
            ResetState();
        }

        public void MouseVisibility(bool visibility)
        {
            IsMouseVisible = visibility;
        }

    }
}