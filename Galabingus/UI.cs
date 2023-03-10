using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Collections.Generic;
using System;

namespace Galabingus
{

    public enum GameState
    {
        Menu,
        Game
    }

    public enum Type
    {
        Enemy,
        Tile
    }

    internal class UI
    {
        #region Fields

        GameState gameState; //Gamestate variable

        KeyboardState currentKBS;
        KeyboardState prevKBS;

        MouseState mouseState;

        Texture2D playButtonTexture;

        Rectangle playButtonRect;

        int playButtonScale = 6;

        int screenWidth;
        int screenHeight;

        GraphicsDeviceManager gr;
        ContentManager cm;
        SpriteBatch sb;

        //File reading
        StreamReader reader;

        //Data values
        List<int[]> objectData;

        #endregion

        #region Properties

        public GameState GS
        {
            get { return gameState; }
        }

        #endregion

        #region Constructor & Intialize

        public UI(GraphicsDeviceManager gr, ContentManager cm, SpriteBatch sb)
        {
            this.gr = gr;
            this.cm = cm;
            this.sb = sb;

            //Intializations
            gameState = GameState.Menu;
            currentKBS = Keyboard.GetState();
            prevKBS = currentKBS;

            playButtonScale = 6;

            screenWidth = gr.PreferredBackBufferWidth;
            screenHeight = gr.PreferredBackBufferHeight;

            objectData = new List<int[]>();
        }

        #endregion

        #region LoadContent

        public void LoadContent()
        {
            playButtonTexture = cm.Load<Texture2D>("PlayButton");
            playButtonRect =
                new Rectangle(
                    (screenWidth - (playButtonTexture.Width / playButtonScale)) / 2,
                    (screenHeight - (playButtonTexture.Height / playButtonScale)) / 2,
                    (playButtonTexture.Width / playButtonScale),
                    (playButtonTexture.Height / playButtonScale)
                );

            //TODO: File Loading


            
        }

        #endregion

        #region Update

        public void Update()
        {
            currentKBS = Keyboard.GetState();
            mouseState = Mouse.GetState();

            switch (gameState)
            {
                case GameState.Menu:

                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (playButtonRect.Contains(mouseState.Position))
                        {
                            gameState = GameState.Game;
                        }
                    }

                    if (SingleKeyPress(Keys.LeftShift)
                        || SingleKeyPress(Keys.RightShift))
                    {
                        gameState = GameState.Game;
                    }

                    break;

                case GameState.Game:

                    if (SingleKeyPress(Keys.LeftShift)
                        || SingleKeyPress(Keys.RightShift))
                    {
                        gameState = GameState.Menu;
                    }

                    break;

            }

            prevKBS = currentKBS;
        }

        #endregion

        #region Draw

        public void Draw()
        {
            switch (gameState)
            {
                case GameState.Menu:

                   gr.GraphicsDevice.Clear(Color.DarkViolet);

                    sb.Draw(
                        playButtonTexture,
                        playButtonRect,
                        Color.White);

                    break;

                case GameState.Game:

                    gr.GraphicsDevice.Clear(Color.DarkBlue);

                    break;

            }
        }

        #endregion

        #region Helpers

        public bool SingleKeyPress(Keys key)
        {
            if (currentKBS.IsKeyDown(key) && prevKBS.IsKeyUp(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<int[]> LevelReader(Type type)
        {
            reader = new StreamReader("Content/level1.txt");

            if (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] s_values = line.Split('|');
                int[] i_values = new int[s_values.Length];

                for(int i = 0; i < s_values.Length; i++)
                {
                    i_values[i] = int.Parse(s_values[i]);
                }

                objectData.Add(i_values);
            }

            List<int[]> returnList = new List<int[]>();

            foreach (int[] value in objectData)
            {
                if (value[0] == (int)type)
                {
                    returnList.Add(value);
                }
            }

            return returnList;
        }

        #endregion
    }
}
