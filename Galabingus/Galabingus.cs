using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;

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
        //private UI userInterface;
        private UIManager userInterface;

        // Player GameObject
        private Player player;

        // Shaders
        private Effect shaders;

        private BulletManager mng_bullet;
        private EnemyManager mng_enemy;

        // Camera
        private Camera camera;

        // Tiles 
        private TileManager tileManager;

        // Tile / Enemy Position & Data
        private List<int[]> l_a4_obj_enemyData;

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
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            shaders = Content.Load<Effect>("shaders");
            // Initilize the GameObject Instance and Content Dynamic (Always goes first)
            content = GameObject.Instance.Initialize(Content, GraphicsDevice, _spriteBatch, shaders);

            //new UI class and loading its content
            //userInterface = new UI(_graphics, Content, _spriteBatch);
            //userInterface.LoadContent();

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


            l_a4_obj_enemyData.Add(new int[] { 1, 1, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 1 - 172), (GameObject.Instance.GraphicsDevice.Viewport.Height * -0) + 10 });
            l_a4_obj_enemyData.Add(new int[] { 1, 2, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 0 + 100), (GameObject.Instance.GraphicsDevice.Viewport.Height * -0) + 10 });

            l_a4_obj_enemyData.Add(new int[] { 1, 2, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 1 - 172), (GameObject.Instance.GraphicsDevice.Viewport.Height * -1) + 10 });
            l_a4_obj_enemyData.Add(new int[] { 1, 4, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 0 + 100), (GameObject.Instance.GraphicsDevice.Viewport.Height * -1) + 10 });

            l_a4_obj_enemyData.Add(new int[] { 1, 4, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 1 - 172), (GameObject.Instance.GraphicsDevice.Viewport.Height * -2) + 10 });
            l_a4_obj_enemyData.Add(new int[] { 1, 5, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 0 + 100), (GameObject.Instance.GraphicsDevice.Viewport.Height * -2) + 10 });

            l_a4_obj_enemyData.Add(new int[] { 1, 4, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 1 - 172), (GameObject.Instance.GraphicsDevice.Viewport.Height * -3) + 10 });
            l_a4_obj_enemyData.Add(new int[] { 1, 5, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 0 + 100), (GameObject.Instance.GraphicsDevice.Viewport.Height * -3) + 10 });

            l_a4_obj_enemyData.Add(new int[] { 1, 1, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 1 - 172), (GameObject.Instance.GraphicsDevice.Viewport.Height * -4) + 10 });
            l_a4_obj_enemyData.Add(new int[] { 1, 2, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 0 + 240), (GameObject.Instance.GraphicsDevice.Viewport.Height * -4) + 10 });
            l_a4_obj_enemyData.Add(new int[] { 1, 4, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 1 - 172), (GameObject.Instance.GraphicsDevice.Viewport.Height * -4) + 10 });
            l_a4_obj_enemyData.Add(new int[] { 1, 5, (int)(GameObject.Instance.GraphicsDevice.Viewport.Width * 1 - 240), (GameObject.Instance.GraphicsDevice.Viewport.Height * -4) + 10 });


            // Create a player
            player = new Player(new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Height * 0.00875f, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.00875f), content.player_strip5);
            player.Position = new Vector2(Player.PlayerInstance.Transform.Width * 2, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.5f - Player.PlayerInstance.Transform.Height);
            player.Health = 5;

            // Create Bullet Manager
            mng_bullet = BulletManager.Instance;

            // Create Enemy Manager + Load data
            mng_enemy = EnemyManager.Instance.Initialize(l_a4_obj_enemyData);

            // Create Camera
            camera = Camera.Instance;
            camera.InitalCameraScroll = -2;

            // Create Tile Manager
            tileManager = TileManager.Instance;
            tileManager.CreateTile(1);

            // Load the temporary background
            //tempBackground = Content.Load<Texture2D>("spacebackground_strip1");
            tileManager.CreateBackground();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Update the UI
            userInterface.Update();

            if (!(userInterface.GS == GameState.Pause) && !(userInterface.GS == GameState.Menu))
            {
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Change the clear color to transparent and use point rendering for pixel art
            //GraphicsDevice.Clear(userInterface.ClearColor);

            GraphicsDevice.Clear(Color.Transparent);

            if (!(userInterface.GS == GameState.Menu))
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, effect: shaders);
            }
            else
            {
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);
            }
            //draw the screen
            userInterface.Draw();

            if (!(userInterface.GS == GameState.Menu))
            {

                //draw the background using the temporary background texture
                /*_spriteBatch.Draw(
                    tempBackground,
                    Vector2.Zero,
                    new Rectangle(0, 0, tempBackground.Width, tempBackground.Height),
                    new Color(Color.White * 0.7f,1.0f),
                    0,
                    Vector2.Zero,
                    new Vector2(
                        GameObject.Instance.GraphicsDevice.Viewport.Width / (float)tempBackground.Width,
                        GameObject.Instance.GraphicsDevice.Viewport.Height / (float)tempBackground.Height
                    ),
                    SpriteEffects.None,
                    1
                );
                */

                if (TileManager.Instance.CurrentSpriteNumber == 0)
                {
                    // Draws tiles
                    tileManager.Draw();
                }

                // Draws enemies
                mng_enemy.Draw();

                // Draw the player
                player.Draw();

                // Draws bullets
                mng_bullet.Draw();

                if (TileManager.Instance.CurrentSpriteNumber == 1)
                {
                    // Draws tiles
                    tileManager.Draw();
                }
            }

            GameObject.Instance.DebugDraw(_spriteBatch);

            // End the SpriteBatch draw
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}