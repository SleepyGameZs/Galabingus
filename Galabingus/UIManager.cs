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
        GameOver,
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

    public enum UIControlState
    {
        Keys,
        Mouse
    }

    /// <summary>
    /// represents the type of level data which should be retrieved
    /// </summary>
    public enum TileType
    {
        Enemy,
        Tile
    }

    public enum UIState
    {
        BaseMenu,
        BaseGame,
        BasePause,
        BaseGameOver
    }


    #endregion

    #region Struct

    struct UILevel
    {
        //Fields

        private List<UIElement> menu;
        private GameState gs;
        private int level;

        //Properties

        public List<UIElement> Menu
        {
            get { return menu; }
        }

        public GameState GS
        {
            get { return gs; }
        }

        public int Level
        {
            get { return level; }
        }

        //CTOR

        public UILevel(List<UIElement> menu, GameState gs, int level)
        {
            this.menu = menu;
            this.gs = gs;
            this.level = level;
        }

    }

    #endregion

    sealed class UIManager
    {
        #region Fields

        //the instance of the UIManager
        private static UIManager instance = null;

        //the list of UIObjects it manages
        private List<UILevel> gameLevels;
        private List<UILevel> debugLevels;
        int currentLevel;

        //variable containing the current gamestate
        private GameState gs;

        //variable containing the debugstate
        private DebugState ds;

        //controls whether the user naviagtes with mouse or keyboard
        private UIControlState cs;

        //create a current and previous keyboardstate variable
        private KeyboardState currentKBS;
        private KeyboardState previousKBS;

        //the current mouseState
        private MouseState currentMS;
        private MouseState previousMS;

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
        private Texture2D menuBackground;

        //the sets of menus in the game
        List<UIElement> menu1;
        List<UIElement> menu2;
        List<UIElement> game1;
        List<UIElement> pause1;
        List<UIElement> gameOver1;

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

        public UIControlState CS
        {
            get { return cs; }
            set { cs = value;  }
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
            cs = new UIControlState();

            //set the base game and debug states
            gs = GameState.Menu;
            ds = DebugState.DebugOn;
            cs = UIControlState.Mouse;

            //list of menu levels
            gameLevels = new List<UILevel>();
            debugLevels = new List<UILevel>();
            currentLevel = 1;
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
            //the sets of menus in the game
            menu1 = new List<UIElement>();
            menu2 = new List<UIElement>();
            game1 = new List<UIElement>();
            pause1 = new List<UIElement>();
            gameOver1 = new List<UIElement>();

            //dummy variables
            Button button;
            Background background;

            //more dummy variables
            EventDelegate event1;
            EventDelegate event2;
            TextEvent textEvent1;

            #region Normal Game UI

            //Create the Play Button
            event1 = StartGame;
            textEvent1 = null;

            AddButton("buttonPlay_strip1", 1,
            new Vector2(width / 2, height / 2),
            event1, menu1);

            AddText("arial_36", "Hello Welcome to Game",
                new Vector2(width / 2 - 200,
                height / 2 - 200), textEvent1, menu1);

            //Create the Options Button
            event1 = null;
            event2 = UpMenu;
            textEvent1 = null;
            
            //create buttons to go in the menu it displays and add them to the list
            AddButton("buttonHowToPlay_strip1", 1,
                new Vector2(width / 2, height / 2 - 100),
                event1, menu2);

            AddButton("buttonCredits_strip1", 1,
                new Vector2(width / 2, height / 2 + 100),
                event1, menu2);

            AddText("arial_18", "use backspace to go back",
                new Vector2(width / 2 - 200,
                height / 2 - 200), textEvent1, menu2);

            //create the options button in the main list
            AddButton("buttonOptions_strip1", 1,
                new Vector2(width / 2, height / 2 + 200),
                event2, menu1);

            /*
            //Pause

            AddText("arial_36", "hello there you are now paused",
                new Vector2(width / 2 - 200,
                height / 2 - 200), textEvent1, pause1);

            //GameOver

            AddText("arial_36", "LOLLLLL :skull_emoji", 
                new Vector2(width / 2 - 100,
                height / 2 - 150), textEvent1, gameOver1);

            */

            //add the background
            menuBackground = cm.Load<Texture2D>("menubackground_strip1");

            //dd all of the levels to the level list
            gameLevels.Add(new UILevel(menu1, GameState.Menu, 1));
            gameLevels.Add(new UILevel(menu2, GameState.Menu, 2));
            gameLevels.Add(new UILevel(game1, GameState.Game, 1));
            gameLevels.Add(new UILevel(pause1, GameState.Pause, 1));
            gameLevels.Add(new UILevel(gameOver1, GameState.GameOver, 1));

            #endregion



            #region Debug UI

            List<UIElement> debugMenu1 = new List<UIElement>();
            List<UIElement> debugMenu2 = new List<UIElement>();
            List<UIElement> debugGame1 = new List<UIElement>();
            List<UIElement> debugPause1 = new List<UIElement>();
            List<UIElement> debugGameOver1 = new List<UIElement>();

            #endregion

        }

        /// <summary>
        /// updates the UI every frame
        /// </summary>
        public void Update()
        {
            //set the keyboardstate
            currentKBS = Keyboard.GetState();
            currentMS = Mouse.GetState();

            /*
            if (currentMS != previousMS)
            {
                cs = UIControlState.Mouse;
            }
            else if (
                currentKBS.IsKeyDown(Keys.W) && currentKBS.IsKeyDown(Keys.A) &&
                currentKBS.IsKeyDown(Keys.S) && currentKBS.IsKeyDown(Keys.D) &&
                currentKBS.IsKeyDown(Keys.Up) && currentKBS.IsKeyDown(Keys.Down) &&
                currentKBS.IsKeyDown(Keys.Left) && currentKBS.IsKeyDown(Keys.Right))
            {
                cs = UIControlState.Keys;
            }
            */


            foreach (UILevel level in gameLevels)
            {
                if(level.Level == currentLevel && level.GS == gs)
                {
                    UpdateObjects(level.Menu);
                }
            }

            if(ds != DebugState.DebugOff)
            {
                foreach (UILevel level in debugLevels)
                {
                    if (level.Level == currentLevel && level.GS == gs)
                    {
                        UpdateObjects(level.Menu);
                    }
                }
            }

            if (SingleKeyPress(Keys.Back) && currentLevel > 1)
            {
                currentLevel--;
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

                case GameState.GameOver:

                    if (SingleKeyPress(Keys.Enter))
                    {
                        gs = GameState.Menu;
                    }

                    break;

            }

            //set the previous KeyboardState to the current one for next frame
            previousKBS = currentKBS;
            previousMS = currentMS;
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
                            gr.GraphicsDevice.Viewport.Width,
                            gr.GraphicsDevice.Viewport.Height),
                        Color.White);

                    break;

                case GameState.Pause:
                case GameState.Game:

                    break;

            }

            foreach (UILevel level in gameLevels)
            {
                if (level.Level == currentLevel && level.GS == gs)
                {
                    DrawObjects(level.Menu);
                }
            }

            if (ds != DebugState.DebugOff)
            {
                foreach (UILevel level in debugLevels)
                {
                    if (level.Level == currentLevel && level.GS == gs)
                    {
                        UpdateObjects(level.Menu);
                    }
                }
            }

        }

        #endregion

        #region Event Methods

        private void StartGame(object sender)
        {
            gs = GameState.Game;
        }

        private void UpMenu(object sender)
        {
            currentLevel++;
        }

        private void DownMenu(object sender)
        {
            currentLevel--;
        }

        private void HoverLightGray(object sender)
        {
            Button button = (Button)sender;

            button.ClearColor = Color.LightGray;
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
        private void AddButton
            (string filename, int scale, Vector2 position, EventDelegate clickEvent, List<UIElement> listToAdd)
        {
            //create the button texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //create the button
            Button button = new Button(texture, position, scale);

            button.OnClick += clickEvent;

            listToAdd.Add(button);
        }

        /// <summary>
        /// creates a UIElement and adds it to the list elements
        /// </summary>
        /// <param name="uiObject">the ui object / element</param>
        /// <param name="gs">the gamestate the element exists in</param>
        /// <param name="uiEvent">the data which it needs for its events</param>
        /// <param name="types">the event types it can call</param>
        private void AddBackground
            (string filename, int scale, Vector2 position,  List<UIElement> listToAdd)
        {
            //create the menus texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //create the button
            Background background = new Background(texture, position, gs);

            listToAdd.Add(background);
        }

        public Text AddText(string content, Vector2 position, int scale, Color tint, UIState uIState)
        {

            if(scale < 14 && scale > 0)
            {
                scale = 12;
            }
            else if(scale < 24 && scale > 15)
            {
                scale = 18;
            }
            else if(scale > 25)
            {
                scale = 36;
            }
            else
            {
                scale = 12;
            }

            switch(uIState)
            {
                case UIState.BaseMenu:
                    return AddText($"arial_{scale}", content, position, tint, null, menu1);
                    break;
                case UIState.BaseGame:
                    return AddText($"arial_{scale}", content, position, tint, null, game1);
                    break;
                case UIState.BasePause:
                    return AddText($"arial_{scale}", content, position, tint, null, pause1);
                    break;
                case UIState.BaseGameOver:
                    return AddText($"arial_{scale}", content, position, tint, null, gameOver1);
                    break;
                default:
                    return null;
                    break;
            }
        }

        private void AddText(string filename, string content, Vector2 position, TextEvent changeText, List<UIElement> listToAdd)
        {
            SpriteFont font = cm.Load<SpriteFont>(filename);

            Text text = new Text(font, content, position);

            text.UpdateText += changeText;

            listToAdd.Add(text);
        }

        private Text AddText(string filename, string content, Vector2 position, Color tint, TextEvent changeText, List<UIElement> listToAdd)
        {
            SpriteFont font = cm.Load<SpriteFont>(filename);

            Text text = new Text(font, content, position, tint);

            text.UpdateText += changeText;

            listToAdd.Add(text);

            return text;
        }

        private void AddText(string filename, string content, Vector2 position, int lineCapacity, int spacing,  List<UIElement> listToAdd)
        {
            SpriteFont font = cm.Load<SpriteFont>(filename);

            List<string> contentDivided = new List<string>();

            while (content.Length != 0)
            {
                int finalPoint = 0;

                for (int i = 0; i < lineCapacity; i++)
                {
                    if (i + 1 == content.Length || content[i] == ' ')
                    {
                        finalPoint = i;
                    }
                    
                }

                string dividedPortion = content.Substring(0, finalPoint + 1);
                contentDivided.Add(dividedPortion);

                if(content.Length != 0)
                {
                    content = content.Substring(finalPoint + 1, content.Length - finalPoint - 1);
                }
            }

            for(int i = 0; i < contentDivided.Count; i++)
            {
                Text text = new Text(font, contentDivided[i], 
                    new Vector2 (position.X, position.Y + spacing));

                listToAdd.Add(text);
            }
        }

        private void AddText(string filename, string content, Vector2 position, int lineCapacity, int spacing, Color tint, List<UIElement> listToAdd)
        {
            SpriteFont font = cm.Load<SpriteFont>(filename);

            List<string> contentDivided = new List<string>();

            while (content != null)
            {
                int finalPoint = 0;

                for (int i = 0; i < lineCapacity; i++)
                {
                    if (content[i] == ' ' || i - 1 == content.Length)
                    {
                        finalPoint = i;
                    }
                }

                string dividedPortion = content.Substring(0, finalPoint + 1);
                contentDivided.Add(dividedPortion);

                if (content.Length != 0)
                {
                    content = content.Substring(finalPoint + 2, content.Length - 1);
                }
            }

            for (int i = 0; i < contentDivided.Count; i++)
            {
                Text text = new Text(font, contentDivided[i],
                    new Vector2(position.X, position.Y + spacing), tint);

                listToAdd.Add(text);
            }
        }

        /// <summary>
        /// updates all of the objects within the list of UIElements
        /// </summary>
        /// <param name="gs">the current gameState</param>
        private void UpdateObjects(List<UIElement> elementList)
        {
            foreach (UIElement element in elementList)
            {
                //casting the object down to its original form
                if(element is Button)
                {
                    Button button = (Button)element;

                    //run the update of the button and store what event it returns
                    button.Update();
                }
                else if (element is Background)
                {
                    Background background = (Background)element;

                    //run the update of the menu and store what event it returns
                    background.Update();
                }
                else if (element is Text)
                {
                    Text text = (Text)element;

                    text.Update();
                }
            }
        }

        /// <summary>
        /// draw every object in the current game state to the screen
        /// </summary>
        /// <param name="gs">the current gameState</param>
        private void DrawObjects(List<UIElement> elementList)
        {
            foreach (UIElement element in elementList)
            {
                //cast it down to its original form
                if (element is Button)
                {
                    Button button = (Button)element;

                    //and draw it
                    button.Draw(sb);
                }
                else if (element is Background)
                {
                    Background background = (Background)element;

                    //and draw it
                    background.Draw(sb);
                }
                else if (element is Text)
                {
                    Text text = (Text)element;

                    text.Draw(sb);
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
        public List<int[]> LevelReader(TileType type)
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
