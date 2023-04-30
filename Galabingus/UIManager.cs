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
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Media;
using System.Threading;

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
        PlayerDead,
        PlayerWins,
        GameOver,
        Victory,
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
    /// (todo) represents the ways the player can interact with the UI
    /// </summary>
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

    /// <summary>
    /// represents the different levels of the games UI
    /// </summary>
    public enum UIState
    {
        BaseMenu,
        BaseGame,
        BasePause,
        BaseGameOver
    }

    #endregion

    sealed class UIManager
    {
        #region Fields

        //the instance of the UIManager
        private static UIManager instance = null;

        //variable containing the current gamestate
        private GameState gs;
        private GameState pgs;

        //variable containing the debugstate
        private DebugState ds;

        //controls whether the user naviagtes with mouse or keyboard
        private UIControlState cs;

        //keyboard control state
        private bool keyboardIsActive;
        private bool keyboardTakeOver;

        //selected button identifier
        private float selectedButton;

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
        List<UIElement> menu;
        List<UIElement> game;
        List<UIElement> pause;
        List<UIElement> gameOver;
        List<UIElement> victory;

        //the stack which holds the current set of menus
        Stack<List<UIElement>> currentMenu;

        //the dictionary containing the base set of ui states
        Dictionary<GameState, List<UIElement>> gameStates;

        //the current action being taken by the UI
        //(0 - nothing, 1 - pop a layer, 2 - push a layer)
        int menuState;

        //for unique menus
        bool displayMenu;
        List<UIElement> menuToDisplay;

        //represents of the game is being reset
        private bool reset;

        //represents is the boss was previously on screen
        private bool prevBossOnScreen;

        //a float between 0 and 1 representing the current volume
        float masterVolume;

        //the time (in frames) it takes for the game to transition to the death state
        const int changeState = 80;

        //the current amount of frames passed
        int timedPassed;

        #endregion

        #region Properties

        /// <summary>
        /// returns and sets a bool which says if the game is being reset
        /// </summary>
        public bool IsReset
        {
            get
            {
                return reset;
            }
            set
            {
                reset = value;
            }
        }

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

        /// <summary>
        /// returns and sets the current debugState
        /// </summary>
        public DebugState DS
        {
            get { return ds; }
            set { ds = value; }
        }

        /// <summary>
        ///  The keyboard code running?
        /// </summary>
        public bool KeyboardTakeOver
        {
            get
            {
                return keyboardIsActive;
            }
        }

        /// <summary>
        ///  The keyboard is in control?
        /// </summary>
        public bool IsKeyboardActive
        {
            get
            {
                return keyboardTakeOver;
            }
        }

        /// <summary>
        ///  Y coordinate of the button
        /// </summary>
        public float ButtonSelection
        {
            get
            {
                return selectedButton;
            }
            set
            {
                selectedButton = value;
            }
        }

        /// <summary>
        /// returns a float between 0 and 1 to represent the current volume chosen
        /// </summary>
        public float MasterVolume
        {
            get
            {
                return masterVolume;
            }
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
            ds = DebugState.DebugOff;
            cs = UIControlState.Mouse;

            //set the previous gameState to the current
            pgs = gs;

            //list of menu levels
            currentMenu = new Stack<List<UIElement>>();
            gameStates = new Dictionary<GameState, List<UIElement>>();
        }

        #endregion

        #region Initialize

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

            //set the boss being on screen to false to start
            prevBossOnScreen = false;

            //set the menustate to base
            menuState = 0;
        }

        #endregion

        #region Load Content

        /// <summary>
        /// loads all of the necesary content for the game and creates the
        /// data structures needed for the UI to function correctly
        /// </summary>
        public void LoadContent()
        {
            //the sets of menus in the game
            menu = new List<UIElement>();
            game = new List<UIElement>();
            pause = new List<UIElement>();
            gameOver = new List<UIElement>();
            victory = new List<UIElement>();

            //dummy variables
            Button button;
            Background background;
            Texture2D texture;
            Slider slider;

            //sub menus
            List<UIElement> howToPlayMenu = new List<UIElement>();
            List<UIElement> optionsMenu = new List<UIElement>();
            List<UIElement> creditsMenu = new List<UIElement>();

            #region Add stuffs

            //----------------------------Create the play button----------------------------//

            //call the add button method which loads the buttons texture, creates the button
            //and subscribes its events and adds it to the appropriate list, and then returns it
            button = AddButton("buttonPlay_base_strip1", 0.6f,
            new Vector2(width / 2 + 30, height / 2),
            StartGame, HoverTexture, menu);

            //uses the returned button to set its hover texture
            button.HoverTexture = cm.Load<Texture2D>("buttonPlay_hover_strip1");

            //----------------------------Create the How To Play Button----------------------------//

            button = AddButton("buttonHowToPlay_base_strip1", 0.6f,
            new Vector2(width / 2 + 30, height / 2 + 100),
            DisplayMenu, HoverTexture, menu);

            button.HoverTexture = cm.Load<Texture2D>("buttonHowToPlay_hover_strip1");

            //----------------------------Create the How To Play Background----------------------------//

            AddBackground(
                "HowToPlayMenu_strip1", 0.4f,
                new Vector2(width / 2, height / 2 - 50),
                howToPlayMenu);

            //adds how to play button to the field holding the How to Play buttons menu to display field
            button.DisplayMenu = howToPlayMenu;

            //----------------------------Create the How To Play Back Button----------------------------//

            button = AddButton("buttonBack_base_strip1", 0.6f,
                new Vector2(width / 2 + 30, height / 2 + 350),
                HideMenu, HoverTexture, howToPlayMenu);

            button.HoverTexture = cm.Load<Texture2D>("buttonBack_hover_strip1");

            //----------------------------Create the Options Button----------------------------//

            button = AddButton("buttonOptions_base_strip1", 0.6f,
            new Vector2(width / 2 + 30, height / 2 + 200),
            DisplayMenu, HoverTexture, menu);

            button.DisplayMenu = optionsMenu;
            button.HoverTexture = cm.Load<Texture2D>("buttonOptions_hover_strip1");

            //----------------------------Create the Options Background----------------------------//

            AddBackground("OptionsMenu_strip1", 0.4f,
                new Vector2(width / 2, height / 2 - 50),
                optionsMenu);

            //----------------------------Create the Volume Slider----------------------------//

            AddSlider("SliderBack_strip1", "SliderKnob_strip1", 0.9f,
                new Vector2(width / 2 + 80, height / 2 - 195),
                AdjustVolume, optionsMenu);

            //----------------------------Create the GodMode and Colliders Toggle----------------------------//

            AddToggle("ToggleOn_strip1", "ToggleOff_strip1", 0.9f,
                new Vector2(width / 2 + 150, height / 2 - 70),
                EnableGodMode, DisableGodMode, optionsMenu);

            AddToggle("ToggleOn_strip1", "ToggleOff_strip1", 0.9f,
                new Vector2(width / 2 + 150, height / 2 + 30),
                EnableColliders, DisableColliders, optionsMenu);

            //----------------------------Create the Options Back Button----------------------------//

            button = AddButton("buttonBack_base_strip1", 0.6f,
                new Vector2(width / 2 + 30, height / 2 + 350),
                HideMenu, HoverTexture, optionsMenu);

            button.HoverTexture = cm.Load<Texture2D>("buttonBack_hover_strip1");

            //----------------------------Create the Credits Button----------------------------//

            button = AddButton("buttonCredits_base_strip1", 0.6f,
            new Vector2(width / 2 + 30, height / 2 + 300),
            DisplayMenu, HoverTexture, menu);

            button.DisplayMenu = creditsMenu;
            button.HoverTexture = cm.Load<Texture2D>("buttonCredits_hover_strip1");

            //----------------------------Create the Credits Background----------------------------//

            AddBackground("CreditsMenu_strip1", 0.4f,
                new Vector2(width / 2, height / 2 - 50),
                creditsMenu);

            //----------------------------Create the Credits Text----------------------------//

            string font = "arial_12";
            int leftAdjust = 195;
            int lineCapacity = 48;
            int spacing = 20;
            Color color = Color.LightGray;

            AddText(
                "arial_18", "Art",
                new Vector2(width / 2 - leftAdjust, height / 2 - 230),
                color, null, creditsMenu);

            AddText(
                font, "Game Art - Zane Smith",
                new Vector2(width / 2 - leftAdjust, height / 2 - 200),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font, "UI Art - Shawn Roller",
                new Vector2(width / 2 - leftAdjust, height / 2 - 175),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font,
                "Pixel Keys - https://joshuajennerdev.itch.io/pixel-keys-x16\r\n",
                new Vector2(width / 2 - leftAdjust, height / 2 - 150),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                "arial_18","Audio",
                new Vector2(width / 2 - leftAdjust, height / 2 - 105),
                color, null, creditsMenu);

            AddText(
                font,"Splitter - Undertale",
                new Vector2(width / 2 - leftAdjust, height / 2 - 75),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font, "All Other Bullets - 1001 Sounds ",
                new Vector2(width / 2 - leftAdjust, height / 2 - 50),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font, "Player Hit -  https://pixabay.com/users/edr-1177074/",
                new Vector2(width / 2 - leftAdjust, height / 2 - 25),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font,
                "Charging - https://pixabay.com/?utm_source=link-attribution&amp;utm_medium=referral&amp;utm_campaign=music&amp;utm_content=37395%22%3EPixabay",
                new Vector2(width / 2 - leftAdjust, height / 2),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font, "Game Over and Pause GFX Sounds -  https://gfxsounds.com/free-sound-effects/",
                new Vector2(width / 2 - leftAdjust, height / 2 + 65),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font,
                "Victory - https://pixabay.com/?utm_source=link-attribution&amp;utm_medium=referral&amp;utm_campaign=music&amp;utm_content=6993%22%3EPixabay",
                new Vector2(width / 2 - leftAdjust, height / 2 + 110),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font,
                "Menu Confirm - https://pixabay.com/sound-effects/?utm_source=link-attribution&amp;utm_medium=referral&amp;utm_campaign=music&amp;utm_content=6104%22%3EPixabay",
                new Vector2(width / 2 - leftAdjust, height / 2 + 175),
                lineCapacity, spacing, color, creditsMenu);

            AddText(
                font,"Menu Selection - Mixkit.co no author",
                new Vector2(width / 2 - leftAdjust, height / 2 + 253),
                lineCapacity, spacing, color, creditsMenu);

            //----------------------------Create the Credits Back Button----------------------------//

            button = AddButton("buttonBack_base_strip1", 0.6f,
                new Vector2(width / 2 + 30, height / 2 + 350),
                HideMenu, HoverTexture, creditsMenu);

            button.HoverTexture = cm.Load<Texture2D>("buttonBack_hover_strip1");

            //----------------------------Create the Logo----------------------------//

            AddBackground("galabinguslogo_strip1", 5,
                new Vector2(width / 2, height / 4),
                menu);

            //----------------------------Create the Pause Background----------------------------//

            AddBackground("pauseMenu_strip1", 2f,
                new Vector2(width / 2, height / 2), pause);

            //----------------------------Create the Resume Button----------------------------//

            button = AddButton("resumeButton_base_strip1", 2.3f,
                new Vector2(width / 2 + 35, height / 2 - 50),
                StartGame, HoverTexture, pause);

            button.HoverTexture = cm.Load<Texture2D>("resumeButton_hover_strip1");

            //----------------------------Create the Pause Options Button----------------------------//
            button = AddButton("buttonOptions_base_strip1", 0.6f,
            new Vector2(width / 2 + 40, height / 2 + 50),
            DisplayMenu, HoverTexture, pause);

            button.HoverTexture = cm.Load<Texture2D>("buttonOptions_hover_strip1");
            button.DisplayMenu = optionsMenu;

            //----------------------------Create the Pause Quit Background----------------------------//

            button = AddButton("quitButton_base_strip1", 2.3f,
            new Vector2(width / 2 + 40, height / 2 + 150),
            ReturnMenu, HoverTexture, pause);

            button.HoverTexture = cm.Load<Texture2D>("quitButton_hover_strip1");

            //----------------------------Create the GameOver Text----------------------------//

            AddBackground("deathTitle_strip1", 1f,
                new Vector2(width / 2, height / 4),
                gameOver);

            //----------------------------Create the GameOver Menu Button----------------------------//

            button = AddButton("buttonMenu_base_strip1", 0.6f,
            new Vector2(width / 2 + 30, height / 2 + 150),
            ReturnMenu, HoverTexture, gameOver);

            button.HoverTexture = cm.Load<Texture2D>("buttonMenu_hover_strip1");

            //----------------------------Create the Victory Text----------------------------//

            AddBackground("victoryTitle_strip1", 1f,
                new Vector2(width / 2, height / 4),
                victory);

            //----------------------------Create the Victory Menu Button----------------------------//

            button = AddButton("buttonMenu_base_strip1", 0.6f,
            new Vector2(width / 2 + 30, height / 2 + 150),
            ReturnMenu, HoverTexture, victory);

            button.HoverTexture = cm.Load<Texture2D>("buttonMenu_hover_strip1");

            #endregion

            //add the background
            menuBackground = cm.Load<Texture2D>("menubackground_strip1");

            //add each of dictionary entries containing the base menus
            gameStates.Add(GameState.Menu, menu);
            gameStates.Add(GameState.Game, game);
            gameStates.Add(GameState.Pause, pause);
            gameStates.Add(GameState.GameOver, gameOver);
            gameStates.Add(GameState.Victory, victory);

            //push the first gameState to the stack (which is the menu)
            currentMenu.Push(gameStates[gs]);

            //set the current selected button really low (so it can become the lowest in Update)
            selectedButton = 10000;
        }

        #endregion

        #region Update

        /// <summary>
        /// updates the UI every frame
        /// </summary>
        public void Update()
        {
            //set the keyboardstate
            currentKBS = Keyboard.GetState();

            //if the stack isn't at its base menu, the back button goes back a state
            if (!(currentMenu.Count <= 1))
            {
                if (SingleKeyPress(Keys.Back))
                {
                    menuState = 1;
                }
            }

            //based on the current menuState, the Stack will be altered
            switch (menuState)
            {
                case 1:

                    //case one pops the current menu
                    ResetButtons();
                    currentMenu.Pop();
                    
                    break;
                case 2:

                    //case 2 pushs whatever menu is in menuToDisplay
                    ResetButtons();
                    currentMenu.Push(menuToDisplay);
                    
                    break;
                default:

                    //any other case (I use 0) causes nothing to change

                    break;
            }

            //set the menuState back to its base
            menuState = 0;

            //check the current keyboard actions to see which UIObject in the current menu is selected
            KeyboardSelection(currentMenu.Peek());

            //update all of the objects in the current menuState
            UpdateObjects(currentMenu.Peek());

            //finite state machine for the UI to update the UI based on user input
            //debug for UI can be set manually in the constructor, for release it is off
            switch (gs)
            {
                //when the player is on the menu
                case GameState.Menu:

                    if (ds == DebugState.DebugOn)
                    {
                        //if the shift button is pressed, change the state
                        if (SingleKeyPress(Keys.L))
                        {
                            gs = GameState.Game;
                        }
                    }

                    break;

                //when the game is actively running
                case GameState.Game:

                    if (ds == DebugState.DebugOn)
                    {
                        //if the shift button is pressed, change the state
                        if (SingleKeyPress(Keys.L))
                        {
                            gs = GameState.Menu;
                        }
                    }

                    //pause the game when tab is pressed
                    if (SingleKeyPress(Keys.Tab))
                    {
                        gs = GameState.Pause;
                        MediaPlayer.Pause();
                        AudioManager.Instance.CallSound("Pause");
                    }

                    //if boss health = 0, go to player wins
                    if (!EnemyManager.Instance.BossOnScreen && prevBossOnScreen)
                    {
                        gs = GameState.PlayerWins;
                        Player.RemovePlayerText();
                        timedPassed = 0;
                        MediaPlayer.Stop();
                        AudioManager.Instance.CallSound("Victory");
                    }

                    prevBossOnScreen = EnemyManager.Instance.BossOnScreen;

                    //if player health = 0, go to player dead
                    if (Player.PlayerInstance.Health == 0)
                    {
                        gs = GameState.PlayerDead;
                        Player.RemovePlayerText();
                        timedPassed = 0;
                        MediaPlayer.Stop();
                        AudioManager.Instance.CallSound("Game Over");
                    }

                    break;

                //when the game is not actively running (being updated) but is being drawn
                case GameState.Pause:

                    //if the game is paused, unpause when tab is hit
                    if (SingleKeyPress(Keys.Tab))
                    {
                        gs = GameState.Game;
                        MediaPlayer.Resume();
                    }

                    break;

                //when the game is still running but the player is dead
                case GameState.PlayerDead:

                    timedPassed++;

                    if (timedPassed > changeState)
                    {
                        gs = GameState.GameOver;
                    }

                    break;

                //when the game is still running but the player has beat the boss
                case GameState.PlayerWins:

                    timedPassed++;

                    if (timedPassed > changeState)
                    {
                        gs = GameState.Victory;

                    }

                    break;

                //after the game fades out and the death text appears
                case GameState.GameOver:

                    //System.Diagnostics.Debug.WriteLine("hello");

                    if (SingleKeyPress(Keys.Enter))
                    {
                        gs = GameState.Menu;
                        reset = true;
                    }

                    break;

                //after the game fades out and the victory text appears
                case GameState.Victory:

                    if (SingleKeyPress(Keys.Enter))
                    {
                        gs = GameState.Menu;
                        reset = true;
                    }

                    break;
            }

            //if the gameState has changed
            if (pgs != gs)
            {
                //and the state isn't the PlayerDead or PLayerWins, which are supposed to continue running
                //the previous UI menu and thus have no dict entries
                if (gs != GameState.PlayerDead && gs != GameState.PlayerWins)
                {
                    currentMenu.Clear();
                    currentMenu.Push(gameStates[gs]);
                }
            }

            //set the previous KeyboardState and GameState
            previousKBS = currentKBS;
            pgs = gs;
        }

        #endregion
                 
        #region Draw

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
                            (int)(menuBackground.Width / 1.1),
                            (int)(menuBackground.Height / 1.1)),
                        Color.White);

                    break;
            }

            //draw all of the objects in the current menu on top of the stack
            DrawObjects(currentMenu.Peek());
        }

        /// <summary>
        /// resets the UI
        /// </summary>
        public void Reset()
        {
            //sets the UI instance to null
            instance = null;
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// starts the game
        /// </summary>
        /// <param name="sender">the button which triggered the method</param>
        private void StartGame(object sender)
        {
            //set the gameState to game
            gs = GameState.Game;

            //starts the game's background music
            AudioManager.Instance.CallMusic("Background Music");
        }

        /// <summary>
        /// returns the player back to the menu
        /// </summary>
        /// <param name="sender">the button which triggered the method</param>
        private void ReturnMenu(object sender)
        {
            //sets the gameState to menu
            gs = GameState.Menu;

            //resets the game
            reset = true;

        }

        /// <summary>
        /// displays a new menu
        /// </summary>
        /// <param name="sender">the button which trigger the event</param>
        private void DisplayMenu(object sender)
        {
            //casts the UIElement sent down to a button
            Button button = (Button)sender;

            //sets the menuState to 2 (pushes menuToDisplay)
            menuState = 2;

            //sets menuToDisplay to the buttons displayMenu field
            menuToDisplay = button.DisplayMenu;

        }

        /// <summary>
        /// hides the current menu and goes back to the previous
        /// </summary>
        /// <param name="sender"></param>
        private void HideMenu(object sender)
        {
            //sets the menuState to 1 (which Pops the currentMenu)
            menuState = 1;
        }

        /// <summary>
        /// changes the texture of a button on hover
        /// </summary>
        /// <param name="sender">the button which is being hovered over</param>
        private void HoverTexture(object sender)
        {
            //casts the UIElement sent down to a button
            Button button = (Button)sender;

            //set the buttons texture to be drawn to its denoted hover texture
            button.UITexture = button.HoverTexture;
        }

        /// <summary>
        /// enables the GodMode
        /// </summary>
        /// <param name="sender">the UIEelement which triggered the method</param>
        private void EnableGodMode(object sender)
        {
            //calls the enable god mode method
            Player.EnableGodMode();
        }

        /// <summary>
        /// disbales the GodMode
        /// </summary>
        /// <param name="sender">the UIEelement which triggered the method</param>
        private void DisableGodMode(object sender)
        {
            //calls the disable god mode method
            Player.DisableGodMode();
        }

        /// <summary>
        /// enables the visualization of colliders
        /// </summary>
        /// <param name="sender">the UIEelement which triggered the method</param>
        private void EnableColliders(object sender)
        {
            //calls the enable collision method
            Collider.EnableCollisionDebug();
        }

        /// <summary>
        /// disables the visualization of colliders
        /// </summary>
        /// <param name="sender">the UIElement which triggered the method</param>
        private void DisableColliders(object sender)
        {
            //calls the disable collision method
            Collider.DisableCollisionDebug();
        }

        /// <summary>
        /// adjusts the volume loudness
        /// </summary>
        /// <param name="sender">the slider which triggered the method</param>
        private void AdjustVolume(object sender)
        {
            //casts the UIElement to a slider
            Slider slider = (Slider)sender;

            //set the returned percentage to the percentage of the master volume
            masterVolume = slider.ReturnPercentage;
        }

        #endregion

        #region Element Creation

        /// <summary>
        /// creates a button with the click event
        /// </summary>
        /// <param name="filename">the filename containing its texture</param>
        /// <param name="scale">the scale to be applied to the button</param>
        /// <param name="position">the x and y position of the button</param>
        /// <param name="clickEvent">the method to be triggered on click</param>
        /// <param name="listToAdd">the list the button belongs to</param>
        /// <returns>the instance of the button object</returns>
        private Button AddButton
            (string filename, float scale, Vector2 position, EventDelegate clickEvent, List<UIElement> listToAdd)
        {
            //create the button texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //create the button
            Button button = new Button(texture, position, scale);

            //subscribe its OnClick method
            button.OnClick += clickEvent;

            //add it to the give list
            listToAdd.Add(button);

            //return the button
            return button;
        }

        /// <summary>
        /// create a button with a hover and click event
        /// </summary>
        /// <param name="filename">the filename containing the texture</param>
        /// <param name="scale">the scale to applied</param>
        /// <param name="position">the x y position of the button</param>
        /// <param name="clickEvent">the method to be called when the button is clicked</param>
        /// <param name="hoverEvent">the method to be called when the button is hovered over</param>
        /// <param name="listToAdd">the menu the button belongs to</param>
        /// <returns>the instance of the button</returns>
        private Button AddButton
            (string filename, float scale, Vector2 position, EventDelegate clickEvent, EventDelegate hoverEvent, List<UIElement> listToAdd)
        {
            //create the button texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //create the button
            Button button = new Button(texture, position, scale);

            //subscribe the on click and hover buttons
            button.OnClick += clickEvent;
            button.OnHover += hoverEvent;

            //add the the button to the given list
            listToAdd.Add(button);

            //return the button
            return button;
        }

        /// <summary>
        /// creates a toggle with enabled and disabled events
        /// </summary>
        /// <param name="filenameOn">the filename of the texture to be shown when its on</param>
        /// <param name="filenameOff">the filename of the texture to be shown when its off</param>
        /// <param name="scale">the scale to be applied to the button (without hover)</param>
        /// <param name="position">the x y position of the button</param>
        /// <param name="enabled">the event to occur when the slider is enabled</param>
        /// <param name="disabled">the event to occur when the slider is disabled</param>
        /// <param name="listToAdd">the menu which the slider belongs to</param>
        /// <returns>an instance of the toggle object</returns>
        private Toggle AddToggle
            (string filenameOn, string filenameOff, float scale, Vector2 position, EventDelegate enabled, EventDelegate disabled, List<UIElement> listToAdd)
        {
            //create the on and off textures
            Texture2D textureOn = cm.Load<Texture2D>(filenameOn);
            Texture2D textureOff = cm.Load<Texture2D>(filenameOff);

            //create the toggle
            Toggle toggle = new Toggle(textureOff, textureOn, position, scale);

            //subscribe its toggle on and off buttons
            toggle.OnEnable += enabled;
            toggle.OnDisable += disabled;

            //add the toggle to the given menu list
            listToAdd.Add(toggle);

            //return the toggle
            return toggle;
        }

        /// <summary>
        /// creates a background object
        /// </summary>
        /// <param name="filename">the filename of the texture</param>
        /// <param name="scale">the scale to be applied</param>
        /// <param name="position">its x y position</param>
        /// <param name="listToAdd">the menu which it belongs to</param>
        /// <returns>an instance of the background object</returns>
        private Background AddBackground
            (string filename, float scale, Vector2 position, List<UIElement> listToAdd)
        {
            //create the backgrounds texture
            Texture2D texture = cm.Load<Texture2D>(filename);

            //create the background
            Background background = new Background(texture, position, scale);

            //add it to the given list
            listToAdd.Add(background);

            //return the instance of the background
            return background;
        }

        /// <summary>
        /// creates a text object (public, for other classes)
        /// </summary>
        /// <param name="content">the content of the text string</param>
        /// <param name="position">its x y position</param>
        /// <param name="scale">the size of the text</param>
        /// <param name="tint">the color to tint it</param>
        /// <param name="uIState">the menu it should be added to</param>
        /// <returns>an instance of the text object</returns>
        public Text AddText(string content, Vector2 position, float scale, Color tint, UIState uIState)
        {

            //determine which scale the given value best fits into
            if (scale < 14 && scale > 0)
            {
                scale = 12;
            }
            else if (scale < 24 && scale > 15)
            {
                scale = 18;
            }
            else if (scale > 25)
            {
                scale = 36;
            }
            else
            {
                scale = 12;
            }

            //based on the given UIState, add the text to the correct menu list
            switch (uIState)
            {
                case UIState.BaseMenu:
                    return AddText($"arial_{scale}", content, position, tint, null, menu);
                    break;
                case UIState.BaseGame:
                    return AddText($"arial_{scale}", content, position, tint, null, game);
                    break;
                case UIState.BasePause:
                    return AddText($"arial_{scale}", content, position, tint, null, pause);
                    break;
                case UIState.BaseGameOver:
                    return AddText($"arial_{scale}", content, position, tint, null, gameOver);
                    break;
                default:
                    return null;
                    break;
            }
        }

        /// <summary>
        /// creates a text and has text which can be updated
        /// </summary>
        /// <param name="filename">the filename containing the font and size</param>
        /// <param name="content">the content of the string</param>
        /// <param name="position">the x y position</param>
        /// <param name="changeText">method which changes the text of the string</param>
        /// <param name="listToAdd">the menu which the text should appear in</param>
        /// <returns>an instance of the text object</returns>
        private Text AddText(string filename, string content, Vector2 position, TextEvent changeText, List<UIElement> listToAdd)
        {
            //creates the spriteFont
            SpriteFont font = cm.Load<SpriteFont>(filename);

            //creates the text object
            Text text = new Text(font, content, position);

            //subscribes the method to the update text method
            text.UpdateText += changeText;

            //adds the text to the given list
            listToAdd.Add(text);

            return text;
        }

        /// <summary>
        /// creates text which can be updated and has specified tint
        /// </summary>
        /// <param name="filename">the filename of the font</param>
        /// <param name="content">the text to be displayed</param>
        /// <param name="position">the x y position</param>
        /// <param name="tint">the color of the text</param>
        /// <param name="changeText">the method which changes the text</param>
        /// <param name="listToAdd">the menu to which the text belongs</param>
        /// <returns></returns>
        private Text AddText(string filename, string content, Vector2 position, Color tint, TextEvent changeText, List<UIElement> listToAdd)
        {
            //creates the sprite font
            SpriteFont font = cm.Load<SpriteFont>(filename);

            //creates the text
            Text text = new Text(font, content, position, tint);

            //subscribes the method to the update text method
            text.UpdateText += changeText;

            //adds the text to the given list
            listToAdd.Add(text);

            //returns the instance of the text
            return text;
        }

        /// <summary>
        /// creates an list of text objects which are each shorter than a give character limit
        /// </summary>
        /// <param name="filename">the filename of the spriteFont</param>
        /// <param name="content">the text to be displayed</param>
        /// <param name="position">the x y position</param>
        /// <param name="lineCapacity">the maximum character capacity of each text object</param>
        /// <param name="spacing">the spacing between text objects</param>
        /// <param name="listToAdd">the menu which the text objects belong to</param>
        /// <returns>a list of text objects</returns>
        private List<Text> AddText(string filename, string content, Vector2 position, int lineCapacity, int spacing, List<UIElement> listToAdd)
        {
            //creates the sprite font
            SpriteFont font = cm.Load<SpriteFont>(filename);

            //list of strings
            List<string> contentDivided = new List<string>();

            //loops through the string until it has been completely divided up
            while (content.Length != 0)
            {
                //the final space before we exceed the character limit
                int finalPoint = 0;

                //loop until you hit the line capacity
                for (int i = 0; i < lineCapacity; i++)
                {
                    //if you hit the end, make the final character the final point
                    if (i + 1 == content.Length)
                    {
                        finalPoint = i;
                        break;
                    }
                    //if you hit a space, make that the final point
                    if (content[i] == ' ')
                    {
                        finalPoint = i;
                    }

                }

                //create a substring up to the final point and add it to the list of strings
                string dividedPortion = content.Substring(0, finalPoint + 1);
                contentDivided.Add(dividedPortion);

                //if we haven't hit the end, remove that substring from the full string
                if (content.Length != 0)
                {
                    content = content.Substring(finalPoint + 1, content.Length - finalPoint - 1);
                }
            }

            //create a new list of text objects
            List<Text> textList = new List<Text>();

            //for each of the list of strings
            for (int i = 0; i < contentDivided.Count; i++)
            {
                //create a new text object with that string and a little lower down
                Text text = new Text(font, contentDivided[i],
                    new Vector2(position.X, position.Y + spacing));

                //add the text to the menu it belongs to
                listToAdd.Add(text);

                //add it also to the list of text objects
                textList.Add(text);
            }

            //return the list of text objects
            return textList;
        }

        /// <summary>
        /// creates an list of text objects which are each shorter than a give character limit
        /// </summary>
        /// <param name="filename">the filename of the spriteFont</param>
        /// <param name="content">the text to be displayed</param>
        /// <param name="position">the x y position</param>
        /// <param name="lineCapacity">the maximum character capacity of each text object</param>
        /// <param name="spacing">the spacing between text objects</param>
        /// <param name="tint">the color of the text objects</param>
        /// <param name="listToAdd">the menu which the text objects belong to</param>
        /// <returns>a list of text objects</returns>
        private List<Text> AddText(string filename, string content, Vector2 position, int lineCapacity, int spacing, Color tint, List<UIElement> listToAdd)
            {
            //creates the sprite font
            SpriteFont font = cm.Load<SpriteFont>(filename);

            //list of strings
            List<string> contentDivided = new List<string>();

            //loops through the string until it has been completely divided up
            while (content.Length != 0)
            {
                //the final space before we exceed the character limit
                int finalPoint = 0;

                //loop until you hit the line capacity
                for (int i = 0; i < lineCapacity; i++)
                {
                    //if you hit the end, make the final character the final point
                    if (i + 1 == content.Length)
                    {
                        finalPoint = i;
                        break;
                    }
                    //if you hit a space, make that the final point
                    if (i + 1 == lineCapacity)
                    {
                        finalPoint = i;
                    }
                }

                //create a substring up to the final point and add it to the list of strings
                string dividedPortion = content.Substring(0, finalPoint + 1);
                contentDivided.Add(dividedPortion);

                //if we haven't hit the end, remove that substring from the full string
                if (content.Length != 0)
                {
                    content = content.Substring(finalPoint + 1, content.Length - finalPoint - 1);
                }
            }

            //create a new list of text objects
            List<Text> textList = new List<Text>();

            //for each of the list of strings
            for (int i = 0; i < contentDivided.Count; i++)
            {
                //create a new text object with that string and a little lower down
                Text text = new Text(font, contentDivided[i],
                    new Vector2(position.X, position.Y + (spacing * i)), tint);

                //add the text to its menu
                listToAdd.Add(text);

                //add the text to the list of text objects
                textList.Add(text);
            }

            //return the list of text objecgts
            return textList;
        }

        /// <summary>
        /// creates a new slider
        /// </summary>
        /// <param name="back">the filename of the texture of the back of the slider</param>
        /// <param name="knotch">the filename of the texture of the knotch of the slider</param>
        /// <param name="scale">the scale to be applied</param>
        /// <param name="position">the x y position</param>
        /// <param name="slideEvent">the event to occur when the slider is adjust</param>
        /// <param name="listToAdd">the menu which the slider belongs</param>
        /// <returns></returns>
        public Slider AddSlider(string back, string knotch, float scale, Vector2 position, EventDelegate slideEvent, List<UIElement> listToAdd)
        {
            //create the texture of the back of the slider and of the knotch
            Texture2D backTexture = cm.Load<Texture2D>(back);
            Texture2D knotchTexture = cm.Load<Texture2D>(knotch);

            //create the slider
            Slider slider = new Slider(backTexture, knotchTexture, position, scale);

            //subscribe the slideEvent method to the OnSlide event
            slider.OnSlide += slideEvent;

            //add the slider to the given list
            listToAdd.Add(slider);

            //set the starting values of the slider
            slider.SetSliderStart();

            //return the slider
            return slider;
        }

        #endregion

        #region Element Updates

        /// <summary>
        /// updates all of the objects in the specified list of UIElements
        /// </summary>
        /// <param name="elementList">list of UIElements to be update</param>
        private void UpdateObjects(List<UIElement> elementList)
        {
            foreach (UIElement element in elementList)
            {
                if (element is Button)
                {
                    //cast the UIElement back to a button
                    Button button = (Button)element;

                    //run the update of the button
                    button.Update();
                }
                else if (element is Background)
                {
                    //cast the UIElement back to a background
                    Background background = (Background)element;

                    //run the update of the bakcground
                    background.Update();
                }
                else if (element is Text)
                {
                    //cast the UIElement back to text
                    Text text = (Text)element;

                    //run an update of the text
                    text.Update();
                }
                else if (element is Slider)
                {
                    //cast the UIELement back to a slider
                    Slider slider = (Slider)element;

                    //run an update of the slider
                    slider.Update();
                }
                else if (element is Toggle)
                {
                    //cast the UIElement back to a toggle
                    Toggle toggle = (Toggle)element;

                    //run an update of the toggle
                    toggle.Update();
                }
            }
        }

        /// <summary>
        /// draws every UIElement in the specified list of UIElements
        /// </summary>
        /// <param name="elementList">the list of UIElements to be update</param>
        private void DrawObjects(List<UIElement> elementList)
        {
            foreach (UIElement element in elementList)
            {
                
                if (element is Button)
                {
                    //cast the UIElement back to a button
                    Button button = (Button)element;

                    //and draw it
                    button.Draw(sb);
                }
                else if (element is Background)
                {
                    //cast the UIElement back to a background
                    Background background = (Background)element;

                    //and draw it
                    background.Draw(sb);
                }
                else if (element is Text)
                {
                    //cast the UIElement back to text
                    Text text = (Text)element;

                    //and draw it
                    text.Draw(sb);
                }
                else if (element is Slider)
                {
                    //cast the UIElement back to a slider
                    Slider slider = (Slider)element;

                    //and draw it
                    slider.Draw(sb);
                }
                else if (element is Toggle)
                {
                    //cast the UIElement back to a toggle
                    Toggle toggle = (Toggle)element;

                    //and draw it
                    toggle.Draw(sb);
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

        public void KeyboardSelection(List<UIElement> current)
        {
            //if there is no selected button (default is 10000)
            if (selectedButton == 10000)
            {
                //loop through each of the UIElements in the current
                foreach (UIElement element in current)
                {
                    //determine what objects are selectable, and find the one with the
                    //lowest Y (highest on screen) to be the default
                    if (element is Button || element is Toggle || element is Slider)
                    {
                        if (selectedButton > element.UIPosition.Y)
                        {
                            selectedButton = element.UIPosition.Y;
                        }
                    }
                }
            }
            
            //set the button being switched to false
            bool switchedButton = false;

            //set the closest button to its default
            float closeButton = 10000000;

            //loop through every UIElement is the given list
            foreach (UIElement element in current)
            {
                //determine which ULElements are selectable
                if (element is Button || element is Toggle || element is Slider)
                {
                    //if the down key has been selected and the button is below the current selectedButton
                    if ((SingleKeyPress(Keys.Down) || SingleKeyPress(Keys.S)) && element.UIPosition.Y > selectedButton)
                    {
                        //find out if it is currently the closest button and select it if so set it to be the closest button
                        if (Math.Abs(selectedButton - element.UIPosition.Y) < Math.Abs(selectedButton - closeButton))
                        {
                            closeButton = element.UIPosition.Y;
                            AudioManager.Instance.CallSound("Menu Select");
                        }

                        //set switched button to true
                        switchedButton = true;
                    }

                    //if the up key has been selected and the button is above the current selectedButton
                    if ((SingleKeyPress(Keys.Up) || SingleKeyPress(Keys.W)) && element.UIPosition.Y < selectedButton)
                    {
                        //find out if it is currently the closest button and select it if so set it to be the closest button
                        if (Math.Abs(selectedButton - element.UIPosition.Y) < Math.Abs(selectedButton - closeButton))
                        {
                            closeButton = element.UIPosition.Y;
                            AudioManager.Instance.CallSound("Menu Select");
                        }

                        //set switched button to be true
                        switchedButton = true;
                    }
                }

            }

            //if the button was switched, then set the selected button to the closeButton value
            if (switchedButton)
            {
                selectedButton = closeButton;
            }
        }

        /// <summary>
        /// resets the selected button value to its default
        /// </summary>
        public void ResetButtons()
        {
            selectedButton = 10000;
        }
           

        #endregion

    }
}
