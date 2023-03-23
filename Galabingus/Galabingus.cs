using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

// Wabungus Corpsungus Duplicatungus
// 2023, 3, 13
// Galabingus
// Creates a GameObject Instance

namespace Galabingus
{

    public class Galabingus : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // GameObject Dynamic
        private dynamic content;

        // Temporary Background
        private Texture2D tempBackground;

        // UI Object
        private UI userInterface;

        // Player GameObject
        private Player player;

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
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initilize the GameObject Instance and Content Dynamic (Always goes first)
            content = GameObject.Instance.Initialize(Content, GraphicsDevice, _spriteBatch);

            //new UI class and loading its content
            userInterface = new UI(_graphics, Content, _spriteBatch);
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
            
            l_a4_obj_enemyData.Add(new int[] { 1, 1, GameObject.Instance.GraphicsDevice.Viewport.Width - 67, 50 });
            l_a4_obj_enemyData.Add(new int[] { 1, 1, GameObject.Instance.GraphicsDevice.Viewport.Width - 67, GameObject.Instance.GraphicsDevice.Viewport.Height / 4 + 50 });
            l_a4_obj_enemyData.Add(new int[] { 1, 1, GameObject.Instance.GraphicsDevice.Viewport.Width - 67, (GameObject.Instance.GraphicsDevice.Viewport.Height / 4) * 2 + 50 });
            l_a4_obj_enemyData.Add(new int[] { 1, 1, GameObject.Instance.GraphicsDevice.Viewport.Width - 67, (GameObject.Instance.GraphicsDevice.Viewport.Height / 4) * 3 + 50 });

            // Create a player
            player = new Player(new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Height * 0.015f, GameObject.Instance.GraphicsDevice.Viewport.Height * 0.015f), content.player_strip5);

            // Create Bullet Manager
            mng_bullet = BulletManager.Instance;

            // Create Enemy Manager + Load data
            mng_enemy = EnemyManager.Instance.Initialize(l_a4_obj_enemyData);

            // Create Camera
            camera = Camera.Instance;

            // Create Tile Manager
            tileManager = TileManager.Instance;
            tileManager.CreateTile(1);

            // Load the temporary background
            tempBackground = Content.Load<Texture2D>("spacebackground_strip1");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update the player
            player.Update(gameTime);

            // Update the enemies
            mng_enemy.Update(gameTime);

            // Update the bullets
            mng_bullet.Update(gameTime);

            //update the game state
            userInterface.Update();

            // Update the Camera
            camera.Update(gameTime);

            tileManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Change the clear color to transparent and use point rendering for pixel art
            GraphicsDevice.Clear(userInterface.ClearColor);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);

            //draw the screen
            userInterface.Draw();

            if (!(userInterface.GS == GameState.Menu))
            {
                //draw the background using the temporary background texture
                _spriteBatch.Draw(
                    tempBackground,
                    Vector2.Zero,
                    new Rectangle(0, 0, tempBackground.Width, tempBackground.Height),
                    Color.Gray,
                    0,
                    Vector2.Zero,
                    new Vector2(
                        GameObject.Instance.GraphicsDevice.Viewport.Width / (float)tempBackground.Width,
                        GameObject.Instance.GraphicsDevice.Viewport.Height / (float)tempBackground.Height
                    ),
                    SpriteEffects.None,
                    1
                );

                // Draws enemies
                mng_enemy.Draw();

                if (TileManager.Instance.CurrentSpriteNumber == 0)
                {
                    // Draws tiles
                    tileManager.Draw();
                }

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

            // End the SpriteBatch draw
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}