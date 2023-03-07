using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Wabungus Corpsungus Duplicatungus
// 2023, 3, 7
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

        // Player GameObject
        private Player player;

        public Galabingus()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initalize the GameObject Instance and Content Dynamic
            content = GameObject.Instance.Initialize(Content, GraphicsDevice, _spriteBatch);
            player = new Player(new Vector2(16.25f, 16.25f), content.player_strip5);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update the player
            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Change the clear color to transparent and use point rendering for pixel art
            GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);

            // Draw the player
            player.Draw();

            // End the SpriteBatch draw
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}