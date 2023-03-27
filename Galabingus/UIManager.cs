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
        Pause
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

        //these represent any more complex events,

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
            //a dictionary to store all of the values for each button
            Dictionary<string, Vector2> buttonData = new Dictionary<string, Vector2>();

            //every button being added to the dictionary (side comments are indices)
            buttonData.Add("playbutton_strip1", new Vector2(1280/2, 720/2)); //0

            //a dictionary to store all of the values for each menu
            Dictionary<string, Vector2> menuData = new Dictionary<string, Vector2>();

            //every menu being added to the dictionary (side comments are indices)

            //lists of all of the UIObjects in the program
            List<Button> buttons = CreateButtons(buttonData);
            List<Menu> menus = CreateMenus(menuData);

            //creates the play button
            AddElement(
                buttons[0], 
                GameState.Menu, 
                new UIEvent(GameState.Game),
                new List<EventType>() { EventType.StartGame }
            );

            //loads temp background
            tempBackground = cm.Load<Texture2D>("spacebackground_strip1");
            menuBackground = cm.Load<Texture2D>("menubackground_strip1");
        }

        /// <summary>
        /// updates the UI everyframe
        /// </summary>
        /// <param name="gameTime">the game's timer</param>
        public void Update(GameTime gameTime)
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
                        if (SingleKeyPress(Keys.LeftShift)
                        || SingleKeyPress(Keys.RightShift))
                        {
                            gs = GameState.Game;
                        }
                    }

                    break;

                case GameState.Game:

                    if (ds != DebugState.DebugOff)
                    {
                        //if the shift button is pressed, change the state
                        if (SingleKeyPress(Keys.LeftShift)
                            || SingleKeyPress(Keys.RightShift))
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

                case GameState.Game:

                    sb.Draw(
                        tempBackground,
                        Vector2.Zero,
                        new Rectangle(0, 0, tempBackground.Width, tempBackground.Height),
                        new Color(Color.White * 0.7f, 1.0f),
                        0,
                        Vector2.Zero,
                        new Vector2(
                            GameObject.Instance.GraphicsDevice.Viewport.Width / (float)tempBackground.Width,
                            GameObject.Instance.GraphicsDevice.Viewport.Height / (float)tempBackground.Height
                        ),
                        SpriteEffects.None,
                        1
                    );

                    break;

            }

            //then draw the UI elements to the screen (second because they need to be drawn over the other stuff)
            DrawObjects(gs);
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
        /// create a list of buttons based on file data and a vector2
        /// </summary>
        /// <param name="buttonData">a dict containing a string filename and a vector2 position</param>
        /// <returns>a list of buttons</returns>
        public List<Button> CreateButtons(Dictionary<string, Vector2> buttonData)
        {
            //create a new list of buttons for returning
            List<Button> buttons = new List<Button>();

            //foreach KeyValuePair, load the texture and add the button to the list
            foreach(KeyValuePair<string, Vector2> button in buttonData)
            {
                Texture2D texture = cm.Load<Texture2D>(button.Key);

                buttons.Add(
                    new Button(
                        texture,
                        button.Value
                    )
                );
            }

            return buttons;
        }

        /// <summary>
        /// create a list of menus from a dictionary
        /// </summary>
        /// <param name="menuData">a dictionary containing a string filename and a vextor2 position</param>
        /// <returns>a list of menu</returns>
        public List<Menu> CreateMenus(Dictionary<string, Vector2> menuData)
        {
            //create a new list of menus for returning
            List<Menu> menus = new List<Menu>();

            //for each KeyValuePair load the texture and add the menu to the list
            foreach (KeyValuePair<string, Vector2> menu in menuData)
            {
                Texture2D texture = cm.Load<Texture2D>(menu.Key);

                menus.Add(
                    new Menu(
                        texture,
                        menu.Value
                    )
                );
            }
            return menus;
        }

        /// <summary>
        /// creates a UIElement and adds it to the list elements
        /// </summary>
        /// <param name="uiObject">the ui object / element</param>
        /// <param name="gs">the gamestate the element exists in</param>
        /// <param name="uiEvent">the data which it needs for its events</param>
        /// <param name="types">the event types it can call</param>
        public void AddElement
            (UIObject uiObject, GameState gs, UIEvent uiEvent, List<EventType> types)
        {
            //insert the default event at the from of the list (thus return 0 is always no event)
            types.Insert(0, default(EventType));

            //add the new UIElement to the list
            elements.Add(new UIElement(uiObject, gs, uiEvent, types));
        }

        /// <summary>
        /// changes the UIManagers gameState
        /// </summary>
        /// <param name="gameState">the gameState to change to</param>
        public void ChangeState(GameState gameState)
        {
            gs = gameState;
        }

        #endregion
    }
}
