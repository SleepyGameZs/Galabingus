using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Galabingus
{

    #region Enums

    /// <summary>
    /// Represents the overall states of the game
    /// </summary>
    public enum GameState
    {
        Menu,
        Game,
        Pause,
        NoState
    }

    /// <summary>
    /// deterimines whether the program has debug features enabled
    /// </summary>
    public enum DebugState
    {
        DebugOn,
        DebugOff
    }

    /// <summary>
    /// represents the type of level data which should be retrieved
    /// </summary>
    public enum Type
    {
        Enemy,
        Tile
    }

    /// <summary>
    /// the list of all event types which can be triggered
    /// </summary>
    public enum EventType
    {
        NoEvent,
        UpMenu,
        DownMenu,
        StartGame
    }

    #endregion

    sealed class UIManager
    {
        #region Fields

        //the instance of the UIManager
        private static UIManager instance = null;

        //the list of UIObjects it manages
        private List<UIElement> elements;

        //variable containing the current gamestate
        private GameState gs;

        //variable containing the debugstate
        private DebugState ds;

        //create a current and previous keyboardstate variable
        private KeyboardState currentKBS;
        private KeyboardState previousKBS;

        //the Game1 / Galabingus class' managers
        //for updating the game
        private GraphicsDeviceManager gr;
        private ContentManager cm;
        private SpriteBatch sb;

        //File reading
        private StreamReader reader;
        private StreamWriter writer;

        //Screen Dimensions
        int width;
        int height;

        //The list of tile values to be
        //returned by the LevelReader
        List<int[]> objectData;

        // Temporary Backgrounds
        private Texture2D tempBackground;
        private Texture2D menuBackground;

        // Event handling
        private EventType currentEvent;
        private List<UIElement> currentMenu;
        private Stack<List<UIElement>> previousMenu;

        #endregion

        #region Properties

        /// <summary>
        /// returns the instance of the UI manager
        /// </summary>
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UIManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// can return current game state
        /// </summary>
        public GameState GS
        {
            get { return gs; }
            set { gs = value; }
        }

        public DebugState DS
        {
            get { return ds; }
            set { ds = value; }
        }

        public EventType CurrentEvent
        {
            get{ return currentEvent; }
            set { currentEvent = value; }
        }

        public List<UIElement> CurrentMenu
        {
            get { return currentMenu; }
            set { currentMenu = value; }
        }

        public List<UIElement> PreviousMenu
        {
            get { return previousMenu.Pop(); }
            set { previousMenu.Push(value); }
        }

        public int PreviousMenuCount
        {
            get { return previousMenu.Count; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// creates the initial instance of the UIManager 
        /// by instantiating its basic components
        /// </summary>
        private UIManager()
        {
            //initialize the gamestate variable
            gs = new GameState();
            ds = new DebugState();

            //set the base game and debug states
            gs = GameState.Menu;
            ds = DebugState.DebugOn;

            //create the elements list
            elements = new List<UIElement>();

            currentEvent = new EventType();
            currentMenu = new List<UIElement>();
            previousMenu = new Stack<List<UIElement>>();
    }

        #endregion

        #region Methods

        /// <summary>
        /// initializes the UIManager with data from the Game1(Galabingus) class
        /// </summary>
        /// <param name="gr">Game1's Graphics Device Manager (for editing the game window)</param>
        /// <param name="cm">Game1's Content Manager (for loading)</param>
        /// <param name="sb">Game1's Sprite Batch (for drawing)</param>
        public void Initialize(
            GraphicsDeviceManager gr,
            ContentManager cm,
            SpriteBatch sb)
        {
            //initialize the monogame managers in the UI manager
            this.gr = gr;
            this.cm = cm;
            this.sb = sb;

            //set the viewport size to the current screen width and height
            width = gr.GraphicsDevice.Viewport.Width;
            height = gr.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// loads all of the necesary content for the game and creates the
        /// data structures needed for the UI to function correctly
        /// </summary>
        public void LoadContent()
        {
            Button button;
            Menu menu;

            EventDelegate event1;
            EventDelegate event2;

            //Play Button
            event1 = StartGame;

            AddButton("playbutton_strip1",
            new Vector2(width / 2, height / 2),
            GameState.Menu, event1);

            //add the backgrounddddd
            menuBackground = cm.Load<Texture2D>("menubackground_strip1");

        }

        /// <summary>
        /// updates the UI every frame
        /// </summary>
        public void Update()
        {
            //set the keyboardstate
            currentKBS = Keyboard.GetState();

            //update all of the UIObjects (we do so first because this can change the state)
            switch (currentEvent)
            {
                case (EventType.NoEvent):
                    UpdateObjects(gs);
                    break;
                case (EventType.UpMenu):
                    try
                    {
                        foreach (UIElement element in currentMenu)
                        {
                            if (element.Element is Button)
                            {
                                Button button = (Button)element.Element;
                                int currentEvent = button.Update();

                                element.UIEvent(currentEvent);
                            }
                            else if (element.Element is Menu)
                            {
                                Menu menu = (Menu)element.Element;
                                int currentEvent = menu.Update();

                                element.UIEvent(currentEvent);
                            }
                        }
                    }
                    catch (Exception e) { }
                    
                    break;
            }

            //finite state machine for the UI to update the UI based on user input
            switch (gs)
            {
                case GameState.Menu:

                    if (ds != DebugState.DebugOff)
                    {
                        //if the shift button is pressed, change the state
                        if (SingleKeyPress(Keys.L))
                        {
                            gs = GameState.Game;
                        }
                    }

                    break;

                case GameState.Game:

                    if (ds != DebugState.DebugOff)
                    {
                        //if the shift button is pressed, change the state
                        if (SingleKeyPress(Keys.L))
                        {
                            gs = GameState.Menu;
                        }
                    }

                    if (SingleKeyPress(Keys.Tab))
                    {
                        gs = GameState.Pause;
                    }

                    break;

                case GameState.Pause:

                    if (SingleKeyPress(Keys.Tab))
                    {
                        gs = GameState.Game;
                    }

                    break;

            }

            //set the previous KeyboardState to the current one for next frame
            previousKBS = currentKBS;
        }

        /// <summary>
        /// draws UI elements to the screen
        /// </summary>
        public void Draw()
        {
            //finite state machine for determining what to draw this frame
            switch (gs)
            {
                case GameState.Menu:

                    //changes the screen background
                    sb.Draw(
                        menuBackground,
                        new Rectangle(
                            0,
                            0,
                            menuBackground.Width,
                            menuBackground.Height),
                        Color.White);

                    break;

                case GameState.Pause:
                case GameState.Game:

                    break;

            }

            switch(currentEvent)
            {
                case (EventType.NoEvent):
                    //then draw the UI elements to the screen (second because they need to be drawn over the other stuff)
                    DrawObjects(gs);
                    break;
                case (EventType.UpMenu):
                    foreach(UIElement uiObject in currentMenu)
                    {
                        uiObject.Draw(sb);
                    }
                    break;
            }
            
        }

        #endregion

        #region Event Methods

        public void StartGame(object sender)
        {
            gs = GameState.Game;
        }

        #endregion

        #region Element Creation and Updates

        /// <summary>
        /// creates a UIElement and adds it to the list elements
        /// </summary>
        /// <param name="uiObject">the ui object / element</param>
        /// <param name="gs">the gamestate the element exists in</param>
        /// <param name="uiEvent">the data which it needs for its events</param>
        /// <param name="types">the event types it can call</param>
        public void AddButton
            (string filename, Vector2 position, GameState gs, EventDelegate clickEvent)
        {
            //create the button texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //create the button
            Button button = new Button(texture, position, gs);

            button.OnClick += clickEvent;

            elements.Add(button);
        }

        /// <summary>
        /// creates a UIElement and adds it to the list elements
        /// </summary>
        /// <param name="uiObject">the ui object / element</param>
        /// <param name="gs">the gamestate the element exists in</param>
        /// <param name="uiEvent">the data which it needs for its events</param>
        /// <param name="types">the event types it can call</param>
        public void AddMenu
            (string filename, Vector2 position, GameState gs, UIEvent uiEvent, List<EventType> types)
        {
            //create the menus texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //create the button
            Menu menu = new Menu(texture, position, gs);

            elements.Add(menu);
        }

        /// <summary>
        /// updates all of the objects within the list of UIElements
        /// </summary>
        /// <param name="gs">the current gameState</param>
        public void UpdateObjects(GameState gs)
        {
            foreach (UIElement element in elements)
            {
                //if the element is located within the current gameState
                if (element.GS == gs)
                {
                    //TODO: make a seperte method for this part of the UpdateObjects and DrawObjects methods
                    //casting the object down to its original form
                    if(element is Button)
                    {
                        Button button = (Button)element;

                        //run the update of the button and store what event it returns
                        button.Update();
                    }
                    else if (element is Menu)
                    {
                        Menu menu = (Menu)element;

                        //run the update of the menu and store what event it returns
                        menu.Update();
                    }
                }
            }
        }

        /// <summary>
        /// draw every object in the current game state to the screen
        /// </summary>
        /// <param name="gs">the current gameState</param>
        public void DrawObjects(GameState gs)
        {
            foreach (UIElement element in elements)
            {
                //if the current element is in the current gameState
                if (element.GS == gs)
                {
                    //cast it down to its original form
                    if (element is Button)
                    {
                        Button button = (Button)element;

                        //and draw it
                        button.Draw(sb);
                    }
                    else if (element is Menu)
                    {
                        Menu menu = (Menu)element;

                        //and draw it
                        menu.Draw(sb);
                    }
                }
            }
            
        }

        #endregion

        #region Input Helpers

        /// <summary>
        /// determines if a the key was click on the prior frame
        /// </summary>
        /// <param name="key">the key that was pressed</param>
        /// <returns>true if it was pressed last frame and false if it wasn't</returns>
        public bool SingleKeyPress(Keys key)
        {
            if (currentKBS.IsKeyDown(key) && previousKBS.IsKeyUp(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region FileIO Helpers

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
                for (int i = 0; i < s_values.Length; i++)
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
