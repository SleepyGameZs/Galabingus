using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System;

namespace Galabingus
{

    //TODO:
        // Learn Singleton and Do Singleton
        // Implement A Debug Mode

    #region Enums

    /// <summary>
    /// Represents the overall states of the game
    /// </summary>
    public enum GameState
    {
        Menu,
        Game
    }

    /// <summary>
    /// represents the type of level data which should be retrieved
    /// </summary>
    public enum Type
    {
        Enemy,
        Tile
    }

    #endregion

    internal class UI
    {
        #region Fields

        //The current gamestate enum
        GameState gs;

        //the current and previous keyboard states
        KeyboardState currentKBS;
        KeyboardState prevKBS;

        //the current state of the mouse
        MouseState mouseState;

        //image for the play button
        Texture2D playButtonTexture;

        //the play buttons location and size
        Rectangle playButtonRect;

        //holds the current screen color
        Color clearColor;

        //the scale size for the playButtonRect
        int playButtonScale = 6;

        //the screen width and height
        int screenWidth;
        int screenHeight;

        //the Game1 / Galabingus class' managers
        //for updating the game
        GraphicsDeviceManager gr;
        ContentManager cm;
        SpriteBatch sb;

        //File reading
        StreamReader reader;

        //The list of tile values to be
        //returned by the LevelReader
        List<int[]> objectData;

        #endregion

        #region Properties

        /// <summary>
        /// can return current game state
        /// </summary>
        public GameState GS
        {
            get { return gs; }
        }

        /// <summary>
        /// can return and set the current 
        /// game's background color
        /// </summary>
        public Color ClearColor
        {
            get
            {
                return clearColor;
            }
            set
            {
                clearColor = value;
            }
        }

        #endregion

        #region Constructor & Intialize

        /// <summary>
        /// the contructor for the UI class (which acts as initialize too)
        /// </summary>
        /// <param name="gr"> the galabingus' GraphicsDeviceManager </param>
        /// <param name="cm"> the galabingus' ContentManager </param>
        /// <param name="sb"> the galabingus' SpriteBatch </param>
        public UI
            (GraphicsDeviceManager gr, 
            ContentManager cm, 
            SpriteBatch sb)
        {
            //sets this classes managers to the
            //Galabingus classes managers
            this.gr = gr;
            this.cm = cm;
            this.sb = sb;

            // --- Intializations --- //

            gs = GameState.Menu;
            currentKBS = Keyboard.GetState();
            prevKBS = currentKBS;

            playButtonScale = 6;

            //sets the current screens size to the current screen size
            screenWidth = gr.PreferredBackBufferWidth;
            screenHeight = gr.PreferredBackBufferHeight;

            //creates the list which will hold the tiles locations
            objectData = new List<int[]>();
        }

        #endregion

        #region LoadContent

        /// <summary>
        /// loads the content which the UI will need and initializes it
        /// </summary>
        public void LoadContent()
        {
            //load the image for the play button
            playButtonTexture = cm.Load<Texture2D>("playbutton_strip1");
            
            playButtonRect =
                new Rectangle(
                    (screenWidth - (playButtonTexture.Width / playButtonScale)) / 2,
                    (screenHeight - (playButtonTexture.Height / playButtonScale)) / 2,
                    (playButtonTexture.Width / playButtonScale),
                    (playButtonTexture.Height / playButtonScale)
                );
        }

        #endregion

        #region Update

        /// <summary>
        /// updates the elements of the UI based on the current game state
        /// </summary>
        public void Update()
        {
            //get the current keyboard and mouse states
            currentKBS = Keyboard.GetState();
            mouseState = Mouse.GetState();
            
            //TODO: Implement a debug state

            //update the game based on the current game state and player inputs
            switch (gs)
            {
                case GameState.Menu:
                    
                    //if the button in pressed, change state to the game state
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        if (playButtonRect.Contains(mouseState.Position))
                        {
                            gs = GameState.Game;
                        }
                    }

                    //DEBUG: if the shift button is pressed, change the state
                    if (SingleKeyPress(Keys.LeftShift)
                        || SingleKeyPress(Keys.RightShift))
                    {
                        gs = GameState.Game;
                    }

                    break;

                case GameState.Game:

                    //DEBUG: if the shift button is pressed, change the state
                    if (SingleKeyPress(Keys.LeftShift)
                        || SingleKeyPress(Keys.RightShift))
                    {
                        gs = GameState.Menu;
                    }

                    break;

            }

            //assign the current frame to the previous one for next frame
            prevKBS = currentKBS;
        }

        #endregion

        #region Draw

        /// <summary>
        /// draws the updates to the UI to the screen
        /// </summary>
        public void Draw()
        {
            //draws a different screen based on the current game state
            switch (gs)
            {
                case GameState.Menu:

                    //changes the screen color to dark violet
                    ClearColor = Color.DarkViolet;

                    //draws the button
                    sb.Draw(
                        playButtonTexture,
                        playButtonRect,
                        Color.White);

                    break;

                case GameState.Game:

                    //changes the screen color to blue
                    ClearColor = Color.DarkSlateGray;

                    break;

            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// determines if a the key was click on the prior frame
        /// </summary>
        /// <param name="key">the key that was pressed</param>
        /// <returns>true if it was pressed last frame and false if it wasn't</returns>
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

        /// <summary>
        /// reads in a level file and creates a list of a certain element within it
        /// </summary>
        /// <param name="type">the type of data you want returned</param>
        /// <returns>a list of a certain type of data</returns>
        public List<int[]> LevelReader(Type type)
        {
            //a new stream reader from a level file
            reader = new StreamReader("Content/level1.level");

            if (!reader.EndOfStream)
            {
                //read the line, split it to a list of string
                string line = reader.ReadLine();
                string[] s_values = line.Split('|');
                int[] i_values = new int[s_values.Length];

                //parse the list of strings to a list of ints
                for(int i = 0; i < s_values.Length; i++)
                {
                    i_values[i] = int.Parse(s_values[i]);
                }

                //add integer list to the overall list
                objectData.Add(i_values);
            }

            //a list to return the desired values
            List<int[]> returnList = new List<int[]>();

            //determines what values of object data should be returned
            foreach (int[] value in objectData)
            {
                if (value[0] == (int)type)
                {
                    returnList.Add(value);
                }
            }

            //clears object data for the next time
            objectData.Clear();

            //returns the list of desired values
            return returnList;
        }

        #endregion
    }
}
