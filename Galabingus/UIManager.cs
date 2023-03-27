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
        NoState,
        Menu,
        Game,
        Pause
    }

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

    struct UIElement
    {
        //Fields

        private UIObject element;
        private GameState gs;
        private UIEvent uiEvent;
        private List<EventType> type;

        //Properties

        public UIObject Element
        {
            get { return element; }
        }

        public GameState GS
        {
            get { return gs;}
        }

        //Constructor

        public UIElement(UIObject element, GameState gs, 
            UIEvent uiEvent, List<EventType> type) 
        {
            this.element = element;
            this.gs = gs;
            this.uiEvent = uiEvent;
            this.type = type;
        }

        //Methods

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

        //the current state of the mouse
        private MouseState mouseState;

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
        /// 
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

        public void Update(GameTime gameTime)
        {
            //set the keyboardstate
            currentKBS = Keyboard.GetState();

            UpdateObjects(gs);

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

            previousKBS = currentKBS;
        }

        public void Draw()
        {
            
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

        public void UpdateObjects(GameState gs)
        {
            foreach (UIElement element in elements)
            {
                if (element.GS == gs)
                {
                    if(element.Element is Button)
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
        }

        public void DrawObjects(GameState gs)
        {
            foreach (UIElement element in elements)
            {
                if (element.GS == gs)
                {
                    if (element.Element is Button)
                    {
                        Button button = (Button)element.Element;
                        button.Draw(sb);
                    }
                    else if (element.Element is Menu)
                    {
                        Menu menu = (Menu)element.Element;
                        menu.Draw(sb);
                    }
                }
            }
            
        }

        public List<Button> CreateButtons(Dictionary<string, Vector2> buttonData)
        {
            List<Button> buttons = new List<Button>();

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

        public List<Menu> CreateMenus(Dictionary<string, Vector2> menuData)
        {
            List<Menu> menus = new List<Menu>();

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

        public void AddElement
            (UIObject uiObject, GameState gs, UIEvent uiEvent, List<EventType> types)
        {
            types.Insert(0, default(EventType));

            elements.Add(new UIElement(uiObject, gs, uiEvent, types));
        }

        public void ChangeState(GameState gameState)
        {
            gs = gameState;
        }

        #endregion
    }
}
