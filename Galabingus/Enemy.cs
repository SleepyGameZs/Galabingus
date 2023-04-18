using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// ENEMY CLASS - By Zane Smith
/* The Enemy Class manages all spawned enemies, most of which are placed at the 
 * start of the game, via file loading. Enemies have various types which in turn
 * link in with the bullets they can shoot. Some special enemies have added abilities,
 * such as exploding when killed, damaging everything around them! Enemies may also be
 * set to be placed in rows or remain still, with moving enemies in the same row turning
 * as a group, rather than constantly bouncing off eachother. */

namespace Galabingus
{
    /// <summary>
    /// Enumeration for all available normal enemy types
    /// </summary>
    public enum EnemyType
    {
        Normal,
        Bouncing,
        Splitter,
        Wave,
        Seeker,
        Bomb
    }

    internal class Enemy : GameObject
    {
        #region-------------------[ Fields ]-------------------
        // Is this enemy ready to be destroyed?
        private bool destroy;

        // Movement data
        private Vector2 velocity;

        // State data
        private EnemyType ability;
        private Vector2 direction;

        // Name used to find values from GameObject dynamic
        private ushort contentName;

        // Number into game object index to look for items
        private ushort enemyNumber;

        // Time between each shot
        private int shotTimer;

        // Whether or not the enemy can Move
        private bool shouldMove;
        private Vector2 initialPosition;

        // Randomizer for time between shots
        private Random rng;
        private int shotWaitTime;
        private int shotWaitVariance;

        // Health value of this enemy
        private int currentHealth;
        private int totalHealth;

        // Reference to what thing created this enemy (can be null)
        private object creatorReference;

        #endregion

        #region-------------------[ Properties ]-------------------

        #region GAME OBJECT PROPERTIES

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored position value
        /// </summary>
        public Vector2 Position
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetPosition(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetPosition(enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored sprite for this bullet
        /// </summary>
        public Texture2D Sprite
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetSprite(enemyNumber);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's transform off total spritesheet (most likely
        /// relates to which frame of the animation is to be shown)
        /// </summary>
        public Rectangle Transform
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetTransform(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetTransform(enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's sprite scale, so that it can be easily resized
        /// </summary>
        public float Scale
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetScale(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetScale(enemyNumber, value);
            }
        }

        public Animation Animation
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetAnimation(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetAnimation(enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored collider for collision checking
        /// </summary>
        public Collider Collider
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetCollider(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetCollider(enemyNumber, value);
            }
        }

        #endregion

        #region ENEMY SPECIFIC PROPERTIES

        /// <summary>
        /// Used to see if this bullet should be destroyed now.
        /// </summary>
        public bool Destroy
        {
            get { return destroy; }
            set { destroy = value; }
        }

        /// <summary>
        /// Allows setting and returning of this enemy's health.
        /// </summary>
        public int Health
        {
            get
            {
                return currentHealth;
            }
            set
            {
                currentHealth = value;
            }
        }

        /// <summary>
        /// The direction of the bullet on being spawned. 
        /// Affects Sprite's visuals and movement for some bullets
        /// X Slot:
        ///     1 = Right
        ///     -1 = Left
        /// Y Slot:
        ///     1 = Up
        ///     -1 = Down
        /// </summary>
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// What object owns this enemy?
        /// If enemy was spawned from file then this will be null.
        /// </summary>
        public object Creator
        {
            get
            {
                return creatorReference;
            }
        }

        /// <summary>
        /// Does this enemy move back and forth?
        /// </summary>
        public bool ShouldMove
        {
            get
            {
                return shouldMove;
            }
        }

        /// <summary>
        /// Returns the initial Y position of the enemy, for the dictionary
        /// in EnemyManager's keys
        /// </summary>
        public Vector2 InitialPosition
        {
            get { return initialPosition; }
        }

        /// <summary>
        /// Returns the enemy number of this enemy (matches with Enemy Manager).
        /// </summary>
        public ushort EnemyNumber
        {
            get { return enemyNumber; }
        }

        /// <summary>
        /// Returns if the enemy is on screen
        /// </summary>
        public bool OnScreen
        {
            get { 
                return (this.Position.Y > 0 &&
                        this.Position.Y < BulletManager.Instance.ScreenDimensions.Y
                        - this.Transform.Height * this.Scale);
            }
        }

        #endregion

        #endregion

        #region-------------------[ Constructor ]-------------------

        /// <summary>
        /// Spawns an enemy with given stats
        /// </summary>
        /// <param name="ability">The ability of the enemy</param>
        /// <param name="position">The position of the enemy</param>
        /// <param name="position">The thing that created this enemy (may be null)</param>
        /// <param name="contentName">Name to use for GameObject storage</param>
        /// <param name="enemyNumber">Number to give bullet in GameObject list</param>
        public Enemy (
            EnemyType ability,
            Vector2 position,
            object creator,
            bool shouldMove,
            ushort contentName,
            ushort enemyNumber
        ) : base(contentName, 
            enemyNumber, 
            CollisionGroup.Enemy)
        {
           
            #region GAME OBJECT DATA

            // Set to GameObject
            this.thisGameObject = this;

            // Set Sprite from given
            this.contentName = contentName;
            this.enemyNumber = enemyNumber;

            // Sets animations speed
            Animation.AnimationDuration = 0.01f;

            // Set Scale
            this.Scale = Player.PlayerInstance.Scale;

            // Set Position
            this.Position = new Vector2(position.X + this.Transform.Width * this.Scale * 0.5f - 10,  // X
                                        position.Y + this.Transform.Height * this.Scale * 0.5f);// Y

            #endregion

            #region ENEMY SPECIFIC DATA

            // Set creator
            creatorReference = creator;

            // Set type of enemy for its abilities
            this.ability = ability;

            // Set base direction
            direction = new Vector2(1, 1);

            // Set velocity to zero at start
            velocity = Vector2.Zero;

            // set randomizer + extra time between next shot
            rng = new Random();
            shotWaitVariance = 8;
            shotWaitTime = rng.Next(shotWaitVariance) - shotWaitVariance / 2;

            // Set shot timer with some randomization
            shotTimer = rng.Next(50);

            // Set Health
            totalHealth = 3;
            currentHealth = totalHealth;

            // Set if enemy should move
            this.shouldMove = shouldMove;

            // Set base position to be stored for dictionary keys
            initialPosition = position;

            #endregion

        }

        #endregion

        #region-------------------[ Methods ]-------------------

        public void Update (GameTime gameTime)
        {
            // Check if off screen
            bool enemyOnScreen = (this.Position.Y > - this.Transform.Height * this.Scale &&
                                  this.Position.Y < BulletManager.Instance.ScreenDimensions.Y);

            // Move enemy with Y camera scrolling
            Vector2 cameraScroll = new Vector2(0, Camera.Instance.OffSet.Y);
            Position -= cameraScroll;

            if (enemyOnScreen)
            { // Only does these while on the screen
                if (!destroy)
                { // Actions while Enemy is alive
                    switch (this.ability)
                    {
                        case EnemyType.Normal:
                            // Shooting (3 Bullets)
                            BulletSpawning(100, BulletType.EnemyNormal, new Vector2(-25, 0), 0);
                            break;

                        case EnemyType.Bouncing:
                            // Shooting (3 Bullets)
                            BulletSpawning(140,
                                           new BulletType[]
                                           {
                                           BulletType.BouncingSide,
                                           BulletType.BouncingCenter,
                                           BulletType.BouncingSide
                                           },
                                           new Vector2[]
                                           {
                                           new Vector2(-14, 0),
                                           new Vector2(-14, 0),
                                           new Vector2(-24, 0)
                                           },
                                           new int[] { -1, 0, 1 }
                                           );
                            break;

                        case EnemyType.Splitter:
                            // Shoots
                            BulletSpawning(140, BulletType.Splitter, new Vector2(-42, 0), 0);
                            break;

                        case EnemyType.Wave:
                            // Shoots
                            BulletSpawning(150, BulletType.Wave, new Vector2(-115, 0), 0);
                            break;

                        case EnemyType.Seeker:
                            // Shoots
                            BulletSpawning(190, BulletType.Seeker, new Vector2(-15, 0), 0);
                            break;
                    }
                    shotTimer++;

                    // Movement
                    if (ShouldMove)
                    {
                        this.Position += new Vector2(3 * direction.X, 0);
                    }
                }
                else
                { // On kill effects
                    switch (this.ability)
                    {
                        case EnemyType.Bomb:
                            // Creates an explosion
                            BulletSpawning(0, BulletType.BigExplosion, new Vector2(-360, 0), 0);
                            AudioManager.Instance.CallSound("Explosion");
                            break;

                        default:
                            // Creates an explosion
                            BulletSpawning(0, BulletType.Explosion, new Vector2(-180, 0), 0);
                            AudioManager.Instance.CallSound("Explosion");
                            break;
                    }
                }

                // Creates currect collider for Enemy
                this.Transform = this.Animation.Play(gameTime);

                this.Collider.Resolved = true;

                SpriteEffects flipper = (Direction.Y < 0) ? SpriteEffects.None : SpriteEffects.FlipVertically;

                // Main Enemy Collider
                List<Collision> intercepts = this.Collider.UpdateTransform(
                    this.Sprite,                            // Enemy Sprite itself
                    this.Position,                          // Position
                    this.Transform,                         // Enemy transform for sprite selection
                    GameObject.Instance.GraphicsDevice,     // Graphics Device Info
                    GameObject.Instance.SpriteBatch,        // Sprite Batcher (carries through)
                    1,                                      // Removed old variant of direction (bully Matt to remove this)
                    new Vector2(this.Scale, this.Scale),    // Scale
                    flipper,                              // Sprite Effects
                    (ushort)CollisionGroup.Enemy,           // Collision Layer
                    enemyNumber                             // Enemy Number (tied to Manager)
                );

                // Get camera's movement direction
                float cameraScrollY = Camera.Instance.OffSet.Y;
                if (Player.PlayerInstance.CameraLock == true)
                {
                    if (cameraScroll.Y > 0)
                    {
                        direction.Y = -1;
                    }
                    else if (cameraScroll.Y < 0)
                    {
                        direction.Y = 1;
                    }
                }
                
                // Checks what kind of things can be collided with
                foreach (Collision collision in intercepts)
                {
                    if (collision.other != null && !destroy)
                    {
                        if ((collision.other as Tile) is Tile)
                        { // Collided with Tile
                            Vector2 overlapZone = ((Tile)collision.other).ScaleVector;

                            //System.Diagnostics.Debug.WriteLine(overlapZone.X);
                            //System.Diagnostics.Debug.WriteLine(overlapZone.Y);
                            if (overlapZone.X < overlapZone.Y)
                            {
                                // Check if collision on left or right
                                if (this.Position.X < ((Tile)collision.other).Position.X)
                                {
                                    EnemyManager.Instance.FlipEnemies((int)initialPosition.Y, true);
                                } else
                                {
                                    EnemyManager.Instance.FlipEnemies((int)initialPosition.Y, false);
                                }
                            }
                        }
                    }
                }

                // Manage Animation
                this.Animation.AnimationDuration = 0.03f;
                this.Transform = this.Animation.Play(gameTime);

            } 
            else
            {
                Position = new Vector2(initialPosition.X, Position.Y);


                this.Collider.Unload();
            }

        }

        #region Bullet Creation Methods

        /// <summary>
        /// Handles the delay between spawning a bullet for the enemy.
        /// </summary>
        /// <param name="shootDelay">Time between each shot</param>
        /// <param name="ability">The ability to give this Bullet</param>
        /// <param name="shootOffset">Spawning offset from Enemy's position</param>
        /// <param name="horizontalDirection">Which way does the bullet face Horizontally?
        ///                                   -1: Left
        ///                                   0: No Direction
        ///                                   1: Right
        ///                                   </param>
        private void BulletSpawning (int shootDelay,
                                    BulletType ability,
                                    Vector2 shootOffset,
                                    int horizontalDirection)
        {
            double percentageChange = 1 + (0.1 * shotWaitTime);

            if (shotTimer > (int)(shootDelay * percentageChange))
            {
                CreateBullet(ability, shootOffset, horizontalDirection);

                // Reset Shooting time
                shotWaitTime = rng.Next(shotWaitVariance) - shotWaitVariance / 2;
                shotTimer = 0;
            }

            
        }

        /// <summary>
        /// Handles the delay between spawning bullets for the enemy. Allows for spawning
        /// of multiple bullets at once
        /// </summary>
        /// <param name="shootDelay">Time between the shots</param>
        /// <param name="ability">The ability to give these Bullets</param>
        /// <param name="shootOffset">Spawning offset from Enemy's position</param>
        /// <param name="horizontalDirection">Which way do the bullets face Horizontally?
        ///                                   -1: Left
        ///                                   0: No Direction
        ///                                   1: Right
        ///                                   </param>
        private void BulletSpawning (int shootDelay,
                                    BulletType[] ability,
                                    Vector2[] shootOffset,
                                    int[] horizontalDirection)
        {
            double percentageChange = 1 + (0.1 * shotWaitTime);

            if (shotTimer > (int)(shootDelay * percentageChange))
            {
                // Makes sure it will never run extra times if given unequal count of values in Lists
                if (ability.Length == shootOffset.Length && ability.Length == horizontalDirection.Length)
                {
                    for (int i = 0; i < ability.Length; i++)
                    { // Create all bullets
                        CreateBullet(ability[i], shootOffset[i], horizontalDirection[i]);
                    }
                }

                // Reset Shooting time
                shotWaitTime = rng.Next(shotWaitVariance) - shotWaitVariance / 2;
                shotTimer = 0;
            }
        }

        /// <summary>
        /// More consice version of creating bullets, which manages
        /// positions and camera data.
        /// </summary>
        /// <param name="ability">The ability to give this Bullet</param>
        /// <param name="shootOffset">Spawning offset from Enemy's position</param>
        /// <param name="horizontalDirection">Which way does the bullet face Horizontally?
        ///                                   -1: Left
        ///                                   0: No Direction
        ///                                   1: Right
        ///                                   </param>
        private void CreateBullet (BulletType ability, Vector2 shootOffset, int horizontalDirection)
        {
            // Check which direction the enemy is facing

            // Get shooting position
            float enemyShootX = (Transform.Width * this.Scale) / 2;
            float enemyShootY = (direction.Y > 0) ? Transform.Height * this.Scale : 0f;

            Vector2 shootPos = new Vector2(Position.X + // Base player X position
                                           enemyShootX, // Center horizontally
                                           Position.Y + // Base player Y position
                                           enemyShootY  // Center vertically
                                           );

            // Check if offset is to be added to Bullet
            shootPos += (direction.Y > 0) ? shootOffset : Vector2.Zero;
            
            // Create the Bullet
            BulletManager.Instance.CreateBullet(ability,                        // Bullet's Ability
                                                shootPos,                       // Spawn Position
                                                new Vector2(horizontalDirection,// Horizontal Direction
                                                            direction.Y),       // Vertical Direction
                                                this,                           // Creator Reference
                                                false);                         // Screen Scrolling
        }

        #endregion

        #endregion
    }
}
