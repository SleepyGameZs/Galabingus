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

    #endregion

    #region Structs

    /// <summary>
    /// holds the basic elements of a UI Object
    /// </summary>
    struct UIElement
    {
        //Fields

        private UIObject element; //the object
        private GameState gs; //its gamestate

        //these represent any more complex events

        //the UIEvent class holds data which it needs to 
        //execute those events and the method to do so
        private UIEvent uiEvent;

        //these are a list of the specfic methods which
        //will be envoked by each function of the object
        //(ex: click, hold, etc) of the element 
        private List<EventType> type;

        //Properties

        /// <summary>
        /// returns the game object (or element)
        /// </summary>
        public UIObject Element
        {
            get { return element; }
        }

        /// <summary>
        /// returns the gameState the element exists in
        /// </summary>
        public GameState GS
        {
            get { return gs;}
        }

        //Constructor

        /// <summary>
        /// instantiates a member of the UIElement structure
        /// </summary>
        /// <param name="element">the game object</param>
        /// <param name="gs">the gameState</param>
        /// <param name="uiEvent">the event data</param>
        /// <param name="type">the envokable events</param>
        public UIElement(UIObject element, GameState gs, 
            UIEvent uiEvent, List<EventType> type) 
        {
            this.element = element;
            this.gs = gs;
            this.uiEvent = uiEvent;
            this.type = type;
        }

        //Methods

        /// <summary>
        /// runs a the UIEvent related to the specified index
        /// </summary>
        /// <param name="index">the index of the event to be invoked</param>
        public void UIEvent(int index)
        {
            uiEvent.Event(element, gs, type[index]);
        }
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
        StreamReader reader;

        //The list of tile values to be
        //returned by the LevelReader
        List<int[]> objectData;

        // Temporary Backgrounds
        private Texture2D tempBackground;
        private Texture2D menuBackground;

        // Event handling
        private EventType currentEvent;
        private List<UIObject> currentMenu;
        private Stack<List<UIObject>> previousMenu;

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

        public List<UIObject> CurrentMenu
        {
            get { return currentMenu; }
            set { currentMenu = value; }
        }

        public List<UIObject> PreviousMenu
        {
            get { return previousMenu.Pop(); }
            set { previousMenu.Push(value); }
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
        }

        /// <summary>
        /// loads all of the necesary content for the game and creates the
        /// data structures needed for the UI to function correctly
        /// </summary>
        public void LoadContent()
        {
            //creates the play button
            AddButton(
                "playbutton_strip1",
                new Vector2(1280 / 2, 720 / 2),
                GameState.Menu, 
                new UIEvent(GameState.Game),
                new List<EventType>() { EventType.StartGame }
            );

            //loads temp background
            tempBackground = cm.Load<Texture2D>("space_only_background_strip1");
            menuBackground = cm.Load<Texture2D>("menubackground_strip1");
        }

        /// <summary>
        /// updates the UI everyframe
        /// </summary>
        /// <param name="gameTime">the game's timer</param>
        public void Update()
        {
            //set the keyboardstate
            currentKBS = Keyboard.GetState();

            //update all of the UIObjects (we do so first because this can change the state)
            UpdateObjects(gs);

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

                   //sb.Draw(
                   //    tempBackground,
                   //    Vector2.Zero,
                   //    new Rectangle(0, 0, tempBackground.Width, tempBackground.Height),
                   //    new Color(Color.White * 0.35f, 1.0f),
                   //    0,
                   //    Vector2.Zero,
                   //    new Vector2(
                   //        GameObject.Instance.GraphicsDevice.Viewport.Width / (float)tempBackground.Width,
                   //        GameObject.Instance.GraphicsDevice.Viewport.Height / (float)tempBackground.Height
                   //    ),
                   //    SpriteEffects.None,
                   //    1
                   //);

                    break;

            }

            switch(currentEvent)
            {
                case (EventType.NoEvent):
                    //then draw the UI elements to the screen (second because they need to be drawn over the other stuff)
                    DrawObjects(gs);
                    break;
                case (EventType.UpMenu):
                    foreach(UIObject uiObject in currentMenu)
                    {
                        uiObject.Draw(sb);
                    }
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
            if (currentKBS.IsKeyDown(key) && previousKBS.IsKeyUp(key))
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
                    //casting the object down to its original form
                    if(element.Element is Button)
                    {
                        Button button = (Button)element.Element;

                        //run the update of the button and store what event it returns
                        int currentEvent = button.Update();

                        //run the event it returns
                        element.UIEvent(currentEvent);
                    }
                    else if (element.Element is Menu)
                    {
                        Menu menu = (Menu)element.Element;

                        //run the update of the menu and store what event it returns
                        int currentEvent = menu.Update();

                        //run the event it returns
                        element.UIEvent(currentEvent);
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
                    if (element.Element is Button)
                    {
                        Button button = (Button)element.Element;

                        //and draw it
                        button.Draw(sb);
                    }
                    else if (element.Element is Menu)
                    {
                        Menu menu = (Menu)element.Element;

                        //and draw it
                        menu.Draw(sb);
                    }
                }
            }
            
        }

        /// <summary>
        /// creates a UIElement and adds it to the list elements
        /// </summary>
        /// <param name="uiObject">the ui object / element</param>
        /// <param name="gs">the gamestate the element exists in</param>
        /// <param name="uiEvent">the data which it needs for its events</param>
        /// <param name="types">the event types it can call</param>
        public void AddButton
            (string filename, Vector2 position, GameState gs, UIEvent uiEvent, List<EventType> types)
        {
            //create the button texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //create the button
            Button button = new Button(texture, position);

            //insert the default event at the from of the list (thus return 0 is always no event)
            types.Insert(0, default(EventType));

            //add the new UIElement to the list
            elements.Add(new UIElement(button, gs, uiEvent, types));
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
            //loads the menus texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //creates the menu with the texture and position
            Menu menu = new Menu(texture, position);

            //insert the default event at the from of the list (thus return 0 is always no event)
            types.Insert(0, default(EventType));

            //add the new UIElement to the list
            elements.Add(new UIElement(menu, gs, uiEvent, types));
        }

        #endregion
    }
}
