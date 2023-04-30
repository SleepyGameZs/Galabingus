using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// ENEMY MANAGER - By Zane Smith
/* The Enemy Manager is what holds the data on all enemies in the game, as well 
 * as what creates / destroys them as needed. In addition to these roles, it also
 * contains the methods for enemy update and draw, plus the data for how enemies
 * are stored in rows that move in unison horizontally back and forth across the
 * screen. Notice: When enemies are created during the update loop, adding them mid
 * loop causes issues, since enemies could be slotted into a position earlier in the
 * loop and not get their update. To make sure everything is even, enemies made mid
 * loop instead have their data stored separately and are placed once the entire
 * update loop has finished. */

namespace Galabingus
{
    public sealed class EnemyManager
    {
        #region-------------------[ Fields ]-------------------

        // actual variable attached to singleton calling property
        private static EnemyManager instance = null;

        // Fileplaced enemy positions (used as base)
        private List<int[]> enemyData;

        // Making Enemy Lines
        private Dictionary<int, List<Enemy>> enemyRows;

        // List existing of enemies
        private List<Enemy> activeEnemies;
        private List<ushort> content;

        // Enemy Created Enemy Storage
        // Needed when an enemy during the Update loop would create another enemy,
        // its data is stored here to be actualized once the loop ends
        private List<EnemyType> storeAbilityEnemies;
        private List<Vector2> storePositionEnemies;
        private List<object> storeCreatorEnemies;
        private List<bool> storeShouldMoveEnemies;

        // Enemy Total
        private ushort enemyTotal;
        private ushort enemiesOnScreen;

        // Boss Detection
        private bool bossOnScreen;
        private Enemy boss;

        #endregion

        #region-------------------[ Properties ]-------------------

        /// <summary>
        /// Reference to the Enemy Manager (use Enemy Manager Constructor method to make a new enemy manager)
        /// </summary>
        public static EnemyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EnemyManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Returns the total enemies on screen
        /// </summary>
        public ushort EnemiesOnScreen
        {
            get { return enemiesOnScreen; }
        }

        /// <summary>
        /// Returns if the boss is on screen currently
        /// </summary>
        public bool BossOnScreen
        {
            get { return bossOnScreen; }
            set { bossOnScreen = value; }
        }

        /// <summary>
        /// Returns the current health of the boss enemy.
        /// </summary>
        public int BossHealth
        {
            get { return boss.Health; }
        }

        #endregion

        #region-------------------[ Constructor ]-------------------

        /// <summary>
        /// The primary constructor,it loads in the base data for the lists and dictionaries, then
        /// everything from the files is loaded in the Initialize method
        /// </summary>
        private EnemyManager()
        {
            // True enemy storage data
            activeEnemies = new List<Enemy>();
            content = new List<ushort>();
            enemyTotal = 0;

            // Row storage data
            enemyRows = new Dictionary<int, List<Enemy>>();

            // Enemy creates enemy data storage
            storeAbilityEnemies = new List<EnemyType>();
            storePositionEnemies = new List<Vector2>();
            storeCreatorEnemies = new List<object>();
            storeShouldMoveEnemies = new List<bool>();

        }

        #endregion

        #region-------------------[ Methods ]-------------------

        /// <summary>
        /// Loads in data for where to place enemies, and parses it
        /// </summary>
        /// <param name="enemyData">A list containing enemy data in the form (FORMAT IS PLACEHOLDER SHAWN)
        ///                         [0] -> Is this an enemy? (1 or 0)
        ///                         [1] -> What kind of enemy (checks within bounds
        ///                                of EnemyType Enum
        ///                         [2] -> X Position
        ///                         [3] -> Y Position
        ///                         [4] -> Should this enemy move (unsupported by level editor)
        ///                         </param>
        public EnemyManager Initialize(List<int[]> enemyData)
        {

            EnemyType tempAbility = EnemyType.Normal;

            // Loop through all enemies from the level file, placing them into the game.
            for (int i = 0; i < enemyData.Count; i++)
            {

                //check if slot contains an enemy
                if (enemyData[i][0] == 1)
                { // Found enemy, set up it stats

                    // Set its ability
                    tempAbility = (EnemyType)enemyData[i][1];

                    // Set its position
                    Vector2 enemyPos = new Vector2(enemyData[i][2], enemyData[i][3]);

                    // Should enemy move?
                    bool shouldMove = (enemyData[i][4] == 1) ? true : false;

                    // Create actual enemy
                    CreateEnemy(tempAbility,    // Ability
                                enemyPos,       // Position
                                null,           // Creator
                                shouldMove,     // Should Enemy move back and forth?
                                false          // Was this Enemy spawned by another enemy?
                                );
                }
            }

            return instance;
        }

        /// <summary>
        /// Method for creating an enemy, it takes in data and organizes it into a completed enemy which
        /// is placed in the level. If the enemy would be created during the enemy manager's update loop,
        /// it is instead stored separately until the loop ends.
        /// </summary>
        /// <param name="ability">What kind of enemy this is</param>
        /// <param name="position">Where this enemy will be placed in the level</param>
        /// <param name="creator">Contains a reference to the object that created this
        ///                       enemy. If it was made by the level editor creator
        ///                       is null.</param>
        /// <param name="shouldMove">Whether or not this enemy moves back and forth 
        ///                          horizontally</param>
        /// <param name="isSourceEnemy">Was this enemy created during the enemy manager's update /
        ///                             by another enemy. If true the enemy to be created will have
        ///                             its data stored separately so that it can be properly
        ///                             introduced into the enemy list afterwards</param>
        public void CreateEnemy (EnemyType ability, Vector2 position, object creator, bool shouldMove, bool isSourceEnemy) 
        {

            #region STEP 1: Link Data to GameObject

            // account for if creator is null
            ushort sprite = GameObject.Instance.Content.tile_strip26;

            // Sets the correct sprite to use when creating this enemy type
            switch (ability)
            {
                case EnemyType.Normal:
                    sprite = GameObject.Instance.Content.enemy_red_strip4;
                    break;

                case EnemyType.Bouncing:
                    sprite = GameObject.Instance.Content.enemy_orange_strip4;
                    break;

                case EnemyType.Splitter:
                    sprite = GameObject.Instance.Content.enemy_green_strip4;
                    break;

                case EnemyType.Wave:
                    sprite = GameObject.Instance.Content.enemy_yellow_strip4;
                    break;

                case EnemyType.Seeker:
                    // OLD VERSION, replaced with Shatter after playtest
                    sprite = GameObject.Instance.Content.enemy_purple_strip4;
                    break;

                case EnemyType.Shatter:
                    sprite = GameObject.Instance.Content.enemy_purple_strip4;
                    break;

                case EnemyType.Bomb:
                    sprite = GameObject.Instance.Content.bomb_strip4;
                    break;

                case EnemyType.Boss:
                    sprite = GameObject.Instance.Content.boss_red_strip4;
                    break;

                default:
                    sprite = GameObject.Instance.Content.enemy_purple_strip4;
                    break;
            }

            // Add sprite linker to list
            if (content.Count == 0)
            {
                content.Add(sprite);
            }
            else
            {
                bool foundSprite = false;
                foreach (ushort asset in content)
                {
                    if (asset == sprite)
                    {
                        foundSprite = true;
                    }
                }
                if (!foundSprite)
                {
                    content.Add(sprite);
                }
            }

            #endregion

            #region STEP 2: Enemy Storage Handling

            // Manages enemy placement into the enemy list. If there is a free slot in the list,
            // this new enemy will be put there to improve storage space.
            bool isReplacing = false;
            ushort setNumber = (ushort)Math.Max(0, (Instance.activeEnemies.Count - 1));

            // Loops through the list looking for empty slots
            for (int i = 0; i < Instance.activeEnemies.Count; i++)
            {
                if (Instance.activeEnemies[i] == null)
                { // Empty slot was found, prep to fill it
                    setNumber = (ushort)(i);
                    isReplacing = true;
                    break;
                }
            }

            #endregion

            #region STEP 3: Generate Enemy Proper

            // Reference for the current enemy
            Enemy createdEnemy = null;

            // Stages of checking:
            // 1: Checks to see if this enemy was created during the enemy update loop.
            //    If it was, its data is stored, rather than having them be placed.
            // 2: Enemy should be created here
            //   A: The enemy is added onto the end of the main list
            //   B: The enemy fills a previously used slot in the main list

            if (isSourceEnemy)
            { // Was created by an enemy, store the data
                Instance.storeAbilityEnemies.Add(ability);
                Instance.storePositionEnemies.Add(position);
                Instance.storeCreatorEnemies.Add(creator);
                Instance.storeShouldMoveEnemies.Add(shouldMove);
            }
            else
            { // Add enemy itself to list
                if (isReplacing == false)
                { // The enemy is added onto the end of the main list
                    // Actually create the enemy using given data
                    createdEnemy = new Enemy(ability,    // Ability of the Enemy spawned
                                             position,   // Position of Enemy
                                             creator,    // What created this enemy
                                             shouldMove, // Should enemy move back and forth
                                             sprite,     // Sprite for Enemy
                                             enemyTotal  // Total enemies
                                             );

                    // Add the enemy to the main list at the end
                    Instance.activeEnemies.Add(createdEnemy);

                    // horizontal movement handling - NOTE: the boss will never be included here
                    if (shouldMove && ability != EnemyType.Boss)
                    {
                        if (!Instance.enemyRows.ContainsKey((int)position.Y))
                        {
                            Instance.enemyRows.Add(((int)position.Y), new List<Enemy>());
                        }

                        // Add item to rows list
                        Instance.enemyRows[(int)position.Y].Add(createdEnemy);
                    }

                    // Increment total enemies
                    enemyTotal++;
                }
                else
                { // The enemy fills a previously used slot in the main list
                    // Actually create the enemy using given data
                    createdEnemy = new Enemy(ability,    // Ability of the Enemy spawned
                                             position,   // Position of Enemy
                                             creator,    // What created this enemy
                                             shouldMove, // Should enemy move back and forth
                                             sprite,     // Sprite for Enemy
                                             enemyTotal  // Total enemies
                                             );

                    // Slots the enemy into an open slot found earlier in the main list
                    Instance.activeEnemies[setNumber] = createdEnemy;
                }

                // Check if this enemy is the boss
                if (createdEnemy.Ability == EnemyType.Boss)
                { // Is the boss, set it to be stored for various checks
                    boss = Instance.activeEnemies[setNumber];
                }
            }

            #endregion
        }

        #region Normal Monogame methods

        /// <summary>
        /// Runs the update methods for all active enemies, and manages deletion
        /// </summary>
        /// <param name="gameTime">Used to get the correct pace</param>
        public void Update (GameTime gameTime)
        {
            // Resets enemies on screen value back to zero for update check
            enemiesOnScreen = 0;

            // Run the Update method for all stored enemies
            for (int i = 0; i < Instance.activeEnemies.Count; i++)
            { // Checks if the enemy slot is null
                if (Instance.activeEnemies[i] != null)
                { 
                    // Runs the enemy's update.
                    Instance.activeEnemies[i].Update(gameTime);

                    // Adds this enemy to the on screen int if it is actually on the screen
                    if (Instance.activeEnemies[i].OnScreen)
                    {
                        enemiesOnScreen++;
                    }

                    // Checks if enemy is set to be destroyed
                    if (Instance.activeEnemies[i].Destroy)
                    {
                        // Remove from Row List (for enemies that move horizontally only)
                        if (Instance.activeEnemies[i].ShouldMove && Instance.activeEnemies[i].Ability != EnemyType.Boss)
                        {
                             Instance.enemyRows[(int)(Instance.activeEnemies[i].InitialPosition.Y)].Remove(Instance.activeEnemies[i]);
                        }

                        // Remove from main list and GameObject
                        Instance.activeEnemies[i].Collider.Unload();
                        Instance.activeEnemies[i].Delete((ushort)i);
                        Instance.activeEnemies[i] = null;
                    }
                }
            }

            // Slot in stored enemies created by other enemies mid update into main list
            for (int i = 0; i < Instance.storeAbilityEnemies.Count; i++)
            {// Create all the stored enemies
                Instance.CreateEnemy(Instance.storeAbilityEnemies[i],    // Ability of the Enemy spawned
                                     Instance.storePositionEnemies[i],   // Position of Enemy
                                     Instance.storeCreatorEnemies[i],    // Enemy's creator
                                     Instance.storeShouldMoveEnemies[i], // Enemy should move?
                                     false
                                     );
            }

            // Clear all stored enemy data built up during this update run
            Instance.storeAbilityEnemies.Clear();
            Instance.storePositionEnemies.Clear();
            Instance.storeCreatorEnemies.Clear();
            Instance.storeShouldMoveEnemies.Clear();
        }

        /// <summary>
        /// Draws all currently active enemies to the screen
        /// </summary>
        public void Draw ()
        {
            foreach (Enemy enemy in Instance.activeEnemies)
            {
                // ignores slots which are currently null from the main list
                if (enemy != null)
                {
                    // Checks if this enemy should be drawn up or down
                    SpriteEffects flipper = (enemy.Direction.Y < 0) ? SpriteEffects.None: SpriteEffects.FlipVertically;

                    // Draws the enemy proper
                    GameObject.Instance.SpriteBatch.Draw(
                        enemy.Sprite,       // The sprite-sheet for the player
                        enemy.Position,     // The position for the player
                        enemy.Transform,    // The scale and bounding box for the animation
                        Color.White,        // The color for the palyer (RED IS TEMP UNTIL WE GET ENEMY SPRITES IN)
                        0.0f,               // There cannot be any rotation of the player
                        Vector2.Zero,       // Starting render position
                        enemy.Scale,        // The scale of the sprite
                        flipper,            // Which direction the sprite faces
                        0.0f                // Layer depth of the player is 0.0
                    );
                }
            }
        }

        #endregion

        #region Movement Row Handling

        /// <summary>
        /// Flips all enemies' horizontal velocity in a given row
        /// </summary>
        /// <param name="positionY">The initial Y position of the enemy when placed, it is used as
        ///                         a key to access the list of all enemies which are meant to move
        ///                         in that specific row</param>
        /// <param name="collideOnRight">Whether this collision was on the right side or the left</param>
        public void FlipEnemies(int positionY, bool collideOnRight)
        {
            if (Instance.enemyRows.ContainsKey(positionY))
            { // There are enemies in the line

                List<Enemy> enemyList = Instance.enemyRows[positionY];
                for (int i = 0; i < enemyList.Count; i++)
                {
                    enemyList[i].Velocity = new Vector2(enemyList[i].Velocity.X * -1 * (float)enemyList[i].Animation.EllapsedTime, enemyList[i].Velocity.Y);
                    if (!collideOnRight || enemyList.Count == 1)
                    { // Fixes slight offset on first ship in row with each bonce
                        enemyList[i].Position += enemyList[i].Velocity;
                    }
                }
            }
        }
        
        /// <summary>
        /// Checks if the given enemy is in the same row slot as the given key
        /// </summary>
        /// <param name="selfY">The initial y position of the enemy to check from
        ///                     for the row (acts as the key for dictionary)</param>
        /// <param name="otherEnemyNumber">The enemy to check if in row</param>
        /// <returns>True if the enemy is in the same row as the other enemy given</returns>
        public bool InSameRow(int selfY, ushort otherEnemyNumber)
        {
            if (Instance.enemyRows.ContainsKey(selfY))
            { // There are enemies in the line
                foreach (Enemy enemy in Instance.enemyRows[selfY])
                {
                    if (enemy.EnemyNumber == otherEnemyNumber)
                    { // Enemy was found to match up correctly
                        return true;
                    }
                }
            }

            // No matches were found
            return false;
        }

        #endregion

        /// <summary>
        /// Resets the enemy manager
        /// </summary>
        public void Reset ()
        {
            instance = null;
        }

        #endregion

    }
}
