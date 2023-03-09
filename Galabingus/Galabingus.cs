using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Wabungus Corpsungus Duplicatungus
// 2023, 3, 6
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

        private UI userInterface;

        public Galabingus()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //new UI class and loading its content
            userInterface = new UI(_graphics, Content, _spriteBatch);
            userInterface.LoadContent();

            // Initalize the GameObject Instance and Content Dynamic
            content = GameObject.Instance.Initialize(Content, GraphicsDevice, _spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //update the game state
            userInterface.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            _spriteBatch.Begin();

            //draw the screen
            userInterface.Draw();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}