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
        Wave,
        Splitter,
        Shatter,
        Bomb,
        Boss,
        // REMOVED CONTENT
        Seeker
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
        private Color colorHealth;

        // Whether or not this enemy is a boss
        private EnemyType bossPhase;
        private int stateTimer;

        // Whether or not this enemy contains health
        private bool dropHealth;

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
            set
            {
                GameObject.Instance.Content = contentName;
                SetSprite(enemyNumber, value);
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

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

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
        /// Returns the ability this enemy has to figure out what kind of enemy this is
        /// </summary>
        public EnemyType Ability
        {
            get { return ability; }
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
                        this.Position.Y < GameObject.Instance.GraphicsDevice.Viewport.Height
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
        /// <param name="creator">The thing that created this enemy (may be null)</param>
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
            Vector2 scaleGet = GameObject.Instance.PostScaleRatio(true);
            this.Scale = scaleGet.Y * 0.9f;
            //this.Scale = PostScaleRatio();

            // Set type of enemy for its abilities
            this.ability = ability;

            // Set Position
            switch (this.ability)
            {
                case EnemyType.Bomb:
                    this.Scale = scaleGet.Y * 0.7f;
                    this.Position = new Vector2(position.X + 5, // X
                                                position.Y);// Y
                    break;

                case EnemyType.Boss:
                    this.Scale = Player.PlayerInstance.Scale;
                    this.Position = new Vector2(position.X + Transform.Width * Scale / 1.5f,
                                                position.Y - Transform.Height * Scale / 2.0f);
                    break;

                default:
                    this.Scale = scaleGet.Y * 0.9f;
                    this.Position = new Vector2(position.X - 10,    // X
                                                position.Y);        // Y
                    break;
            }

            #endregion

            #region ENEMY SPECIFIC DATA

            // Set creator
            creatorReference = creator;

            // Set the color of the enemy's health bar
            switch (ability)
            {
                case EnemyType.Normal:
                    colorHealth = Color.Red;
                    break;

                case EnemyType.Bouncing:
                    colorHealth = Color.Orange;
                    break;

                case EnemyType.Wave:
                    colorHealth = Color.Yellow;
                    break;

                case EnemyType.Splitter:
                    colorHealth = Color.LimeGreen;
                    break;

                case EnemyType.Seeker:
                    colorHealth = Color.Purple;
                    break;

                case EnemyType.Shatter:
                    colorHealth = Color.Purple;
                    break;

                case EnemyType.Bomb:
                    colorHealth = Color.LightGray;
                    break;

                case EnemyType.Boss:
                    colorHealth = Color.LightGray;
                    break;
            }

            // Set base direction
            direction = new Vector2(1, 1);

            // set randomizer + extra time between next shot
            rng = new Random();
            shotWaitVariance = 9;
            shotWaitTime = rng.Next(shotWaitVariance) - shotWaitVariance / 2;

            // Set shot timer with some randomization
            shotTimer = rng.Next(50);

            // Set if enemy should move
            this.shouldMove = shouldMove;
            velocity = (this.shouldMove) ? new Vector2(2.5f, 0) : Vector2.Zero;

            // Set base position to be stored for dictionary keys
            initialPosition = this.Position;

            // Boss Data + Health
            switch (ability)
            {
                case EnemyType.Bomb:
                    totalHealth = 1;

                    // Bombs cannot drop health
                    dropHealth = false;
                    break;

                case EnemyType.Boss:
                    // Set Health
                    totalHealth = 175;


                    // Boss has special health dropping mechanics in Update
                    dropHealth = true;
                    break;

                default:
                    // Normal health
                    totalHealth = 3;

                    // Does this enemy drop health?
                    dropHealth = (rng.Next(3) == 1) ? true : false;
                    break;
            }
            currentHealth = totalHealth;
            bossPhase = EnemyType.Normal;

            // Sets the state timer to zero
            stateTimer = 0;

            #endregion

        }

        #endregion

        #region-------------------[ Methods ]-------------------

        public void Update (GameTime gameTime)
        {
            // Check if off screen
            bool enemyOnScreen = (this.Position.Y > - this.Transform.Height * this.Scale &&
                                  this.Position.Y < GameObject.Instance.GraphicsDevice.Viewport.Height);

            if (ability == EnemyType.Boss)
            { // Position change for the boss, boss will lock to screen once it appears

                // Check if boss is on screen
                if (this.Position.Y < this.Transform.Height * this.Scale * 0.05f)
                { // Not on screen, keep moving with scroll
                    if (Player.PlayerInstance.CameraLock == true)
                    { // Normal camera movement
                        this.Position = new Vector2(this.Position.X, this.Position.Y - Camera.Instance.OffSet.Y);
                    }
                    else
                    { // In debug mode
                        this.Position = new Vector2(this.Position.X, this.Position.Y - Player.PlayerInstance.Translation.Y);
                    }
                }
                else
                { // On screen, set boss effect and no longer moves
                    GameObject.Instance.StartBossEffect();
                    EnemyManager.Instance.BossOnScreen = true;
                }
            } 
            else
            { // Normal position change, checks if in debug mode
                if (Player.PlayerInstance.CameraLock == true)
                { // Normal camera movement
                    this.Position = new Vector2(this.Position.X, this.Position.Y - Camera.Instance.OffSet.Y);
                }
                else
                { // In debug mode
                    this.Position = new Vector2(this.Position.X, this.Position.Y - Player.PlayerInstance.Translation.Y);
                }
            }
            
            if (enemyOnScreen)
            { // Only does these while on the screen
                if (!destroy)
                { // Actions while Enemy is alive
                    // Draw the healthbar
                    Player.PlayerInstance.CreateHealthBar(currentHealth, totalHealth,
                                                          (int)(this.Position.X + velocity.X), (int)(this.Position.Y + velocity.Y), 
                                                          this.Transform.Width * this.Scale, this.Transform.Height * this.Scale * 0.1f,
                                                          Color.Black, colorHealth);

                    switch (this.ability)
                    {
                        case EnemyType.Normal:
                            // Shooting (3 Bullets)
                            BulletSpawning(130, 
                                           BulletType.EnemyNormal, 
                                           ((Direction.Y == 1) ?    // CHECK DIRECTION
                                               new Vector2(-30, 0) :    // DOWN
                                               new Vector2(-35, -80)),  // UP
                                           0);
                            break;

                        case EnemyType.Bouncing:
                            // Shooting (3 Bullets)
                            BulletSpawning(150,
                                           new BulletType[]
                                           {
                                           BulletType.BouncingSide,
                                           BulletType.BouncingCenter,
                                           BulletType.BouncingSide
                                           },
                                           new Vector2[]
                                           {
                                           ((Direction.Y == 1) ?    // CHECK DIRECTION
                                               new Vector2(-19, 0) :    // DOWN
                                               new Vector2(-24, -80)),  // UP,
                                           ((Direction.Y == 1) ?    // CHECK DIRECTION
                                               new Vector2(-19, 0) :    // DOWN
                                               new Vector2(-24, -80)),  // UP,,
                                           ((Direction.Y == 1) ?    // CHECK DIRECTION
                                               new Vector2(-29, 0) :    // DOWN
                                               new Vector2(-24, -80)),  // UP,
                                           },
                                           new int[] { -1, 0, 1 }
                                           );
                            break;

                        case EnemyType.Splitter:
                            // Shoots
                            BulletSpawning(150, 
                                           BulletType.Splitter,
                                           ((Direction.Y == 1) ?    // CHECK DIRECTION
                                               new Vector2(-47, 0) :    // DOWN
                                               new Vector2(-52, -80)),  // UP,
                                           0);
                            break;

                        case EnemyType.Wave:
                            // Shoots
                            BulletSpawning(160, 
                                           BulletType.Wave,
                                           ((Direction.Y == 1) ?    // CHECK DIRECTION
                                               new Vector2(-145, 0) :    // DOWN
                                               new Vector2(-155, -80)),  // UP
                                           0);
                            break;

                        case EnemyType.Seeker:
                            // Shoots
                            BulletSpawning(170, 
                                           BulletType.Seeker,
                                           ((Direction.Y == 1) ?    // CHECK DIRECTION
                                               new Vector2(-20, 0) :    // DOWN
                                               new Vector2(-25, -80)),  // UP, 
                                           0);
                            break;

                        case EnemyType.Shatter:
                            // Shoots
                            BulletSpawning(190,
                                           BulletType.Shatter,
                                           ((Direction.Y == 1) ?    // CHECK DIRECTION
                                               new Vector2(-20, 0) :    // DOWN
                                               new Vector2(-25, -80)),  // UP, 
                                           0);
                            break;

                        case EnemyType.Boss:
                            if (this.Position.Y > 0 && this.Position.Y < GameObject.Instance.GraphicsDevice.Viewport.Height)
                            {
                                // Base data
                                int phaseTime = 0;
                                ushort newSprite = 0;

                                // Health drops every 30 hits dealt to the boss
                                if (currentHealth % 30 == 0)
                                {
                                    if (dropHealth == true)
                                    { // Spawns a heart
                                        BulletSpawning(0, BulletType.Heart, new Vector2(-70, 0), 0);
                                        dropHealth = false;
                                    }
                                }
                                else
                                {
                                    dropHealth = true;
                                }

                                // Switch for various attacks
                                switch (bossPhase)
                                {
                                    case EnemyType.Normal:
                                        // Set the new boss sprite
                                        newSprite = GameObject.Instance.Content.boss_red_strip4;
                                        this.Sprite = GetSpriteFrom(newSprite, enemyNumber);

                                        // Shooting
                                        bool normalRange = (stateTimer >= 100 && stateTimer < 180) ||
                                                           (stateTimer >= 210 && stateTimer < 290) ||
                                                           (stateTimer >= 320 && stateTimer < 410);

                                        if (stateTimer % 10 == 0 && normalRange)
                                        {
                                            BulletSpawning(0, BulletType.EnemyNormal, new Vector2(-30 + velocity.X, 0), 0);
                                        }

                                        // Horizontal Velocity to use
                                        if (velocity.X > 0)
                                        {
                                            velocity.X = 7f;
                                        }
                                        else
                                        {
                                            velocity.X = -7f;
                                        }

                                        // Time till next phase
                                        phaseTime = 410;
                                        break;

                                    case EnemyType.Bouncing:
                                        // Set the new boss sprite
                                        newSprite = GameObject.Instance.Content.boss_orange_strip4;
                                        this.Sprite = GetSpriteFrom(newSprite, enemyNumber);

                                        // Shooting Bouncy shots
                                        if (stateTimer % 60 == 0 && stateTimer >= 70)
                                        {
                                            BulletSpawning(0,
                                               new BulletType[]
                                               {
                                           BulletType.BossBouncingSide,
                                           BulletType.BossBouncingCenter,
                                           BulletType.BossBouncingSide
                                               },
                                               new Vector2[]
                                               {
                                           new Vector2(-39 + velocity.X, -20),
                                           new Vector2(-34 + velocity.X, -20),
                                           new Vector2(-49 + velocity.X, -20)
                                               },
                                               new int[] { -1, 0, 1 }
                                               );
                                        }

                                        // Horizontal Velocity to use
                                        if (velocity.X > 0)
                                        {
                                            velocity.X = 5.5f;
                                        }
                                        else
                                        {
                                            velocity.X = -5.5f;
                                        }

                                        // Time till next phase
                                        phaseTime = 420;
                                        break;

                                    case EnemyType.Wave:
                                        // Set the new boss sprite
                                        newSprite = GameObject.Instance.Content.boss_yellow_strip4;
                                        this.Sprite = GetSpriteFrom(newSprite, enemyNumber);

                                        // Shooting
                                        if (stateTimer % 40 == 0 && stateTimer >= 80)
                                        {
                                            BulletSpawning(0, BulletType.Wave, new Vector2(-145 + velocity.X, 0), 0);
                                        }

                                        // Horizontal Velocity to use
                                        if (velocity.X > 0)
                                        {
                                            velocity.X = 7f;
                                        }
                                        else
                                        {
                                            velocity.X = -7f;
                                        }

                                        // Time till next phase
                                        phaseTime = 500;
                                        break;

                                    case EnemyType.Splitter:
                                        // Set the new boss sprite
                                        newSprite = GameObject.Instance.Content.boss_green_strip4;
                                        this.Sprite = GetSpriteFrom(newSprite, enemyNumber);

                                        // Shooting
                                        bool splitterRange = (stateTimer >= 80 && stateTimer < 140) ||
                                                             (stateTimer >= 180 && stateTimer < 240) ||
                                                             (stateTimer >= 280 && stateTimer < 340);

                                        if (stateTimer % 20 == 0 && splitterRange)
                                        {
                                            BulletSpawning(0, BulletType.Splitter, new Vector2(-47 + velocity.X, 0), 0);
                                        }

                                        // Horizontal Velocity to use
                                        if (velocity.X > 0)
                                        {
                                            velocity.X = 5.5f;
                                        }
                                        else
                                        {
                                            velocity.X = -5.5f;
                                        }

                                        // Time till next phase
                                        phaseTime = 380;
                                        break;

                                    case EnemyType.Shatter:
                                        // Set the new boss sprite
                                        newSprite = GameObject.Instance.Content.boss_purple_strip4;
                                        this.Sprite = GetSpriteFrom(newSprite, enemyNumber);

                                        // Shooting
                                        if (stateTimer % 80 == 0 && stateTimer >= 100)
                                        {
                                            BulletSpawning(0, BulletType.BossShatter, new Vector2(-20 + velocity.X, 0), 0);
                                        }

                                        // Horizontal Velocity to use
                                        if (velocity.X > 0)
                                        {
                                            velocity.X = 7f;
                                        }
                                        else
                                        {
                                            velocity.X = -7f;
                                        }

                                        // Time till next phase
                                        phaseTime = 500;
                                        break;
                                }

                                // Change to make use of game time
                                if (stateTimer >= phaseTime)
                                {
                                    bossPhase++;
                                    stateTimer = 0;
                                    if (bossPhase == EnemyType.Bomb)
                                    {
                                        bossPhase = EnemyType.Normal;
                                    }
                                }

                                // Increment state timer 
                                stateTimer++;
                            }
                            break;
                    }
                    shotTimer++;

                    // Movement
                    if (ShouldMove)
                    {
                        // Collisions
                        if (ability == EnemyType.Boss)
                        { // Goes beyond borders slightly to allow for shots to hit edge
                            if (this.Position.Y > 0 && this.Position.Y < GameObject.Instance.GraphicsDevice.Viewport.Height)
                            {
                                this.Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

                                // Collide with walls
                                if (this.Position.X + (this.Transform.Width * this.Scale / 2) >=  // Enemy's right side
                                    GameObject.Instance.GraphicsDevice.Viewport.Width &&    // Screen's right side
                                    Velocity.X > 0)                                         // Can only occur when facing Right
                                { // Bounce on right side of screen
                                    this.Position -= velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 120;
                                    Velocity = new Vector2(Velocity.X * -1 * (float)Animation.EllapsedTime, Velocity.Y);

                                }
                                else if (this.Position.X <= this.Transform.Width * this.Scale / -2 && Velocity.X < 0)
                                { // Bounce on left side of screen
                                    this.Position -= velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;
                                    Velocity = new Vector2(Velocity.X * -1 * (float)Animation.EllapsedTime, Velocity.Y);
                                }
                            }
                        } 
                        else
                        { // Bounces at normal borders

                            this.Position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

                            // Collide with walls
                            if (this.Position.X + this.Transform.Width * this.Scale >=  // Enemy's right side
                                GameObject.Instance.GraphicsDevice.Viewport.Width &&    // Screen's right side
                                Velocity.X > 0)                                         // Can only occur when facing Right
                            { // Bounce on right side of screen
                                this.Position -= velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 120;
                                EnemyManager.Instance.FlipEnemies((int)initialPosition.Y, true);
                            }
                            else if (this.Position.X <= 0 && Velocity.X < 0)
                            { // Bounce on left side of screen
                                this.Position -= velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;
                                EnemyManager.Instance.FlipEnemies((int)initialPosition.Y, false);
                            }
                        }
                    }
                }
                else
                { // On kill effects
                    switch (this.ability)
                    {
                        case EnemyType.Bomb:
                            // Creates an explosion
                            BulletSpawning(0, BulletType.BigExplosion, new Vector2(-400, 0), 0);
                            AudioManager.Instance.CallSound("Explosion");
                            break;

                        case EnemyType.Boss:
                            // Creates an explosion
                            BulletSpawning(0, BulletType.BigExplosion, new Vector2(-400, 0), 0);
                            AudioManager.Instance.CallSound("Explosion");
                            GameObject.Instance.StopBossEffect();
                            EnemyManager.Instance.BossOnScreen = false;
                            break;

                        default:
                            // Creates an explosion
                            BulletSpawning(0, BulletType.Explosion, new Vector2(-230, 0), 0);
                            AudioManager.Instance.CallSound("Explosion");

                            // Has a chance to spawn hearts
                            if (dropHealth == true)
                            {
                                BulletSpawning(0, BulletType.Heart, new Vector2(-70, 0), 0);
                            }
                            break;
                    }
                }

                // Manage Animation
                this.Transform = this.Animation.Play(gameTime);
                this.Animation.AnimationDuration = 0.03f;

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
                if (!Player.PlayerInstance.CameraLock && Player.PlayerInstance.CameraLock)
                {
                    Vector2 cameraScroll = new Vector2(0, Camera.Instance.OffSet.Y);

                    // Set direction
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
                        { // Collided with Tile, see if it is active

                            if (((Tile)collision.other).IsActive)
                            { // Tile is currently active
                                if (ability == EnemyType.Boss)
                                { // Boss deletes tiles
                                    ((Tile)collision.other).IsActive = false;
                                }
                                else
                                { // Normal enemies bounce off tiles
                                    if (this.Position.X < ((Tile)collision.other).Position.X && Velocity.X > 0)
                                    { // Bounce right
                                        this.Position -= velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;
                                        EnemyManager.Instance.FlipEnemies((int)initialPosition.Y, true);
                                    }
                                    else if (this.Position.X > ((Tile)collision.other).Position.X && Velocity.X < 0)
                                    { // Bounce left
                                        this.Position -= velocity * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;
                                        EnemyManager.Instance.FlipEnemies((int)initialPosition.Y, false);
                                    }
                                }
                            }
                        } 
                        else if ((collision.other as Player) is Player)
                        {
                            if (ability == EnemyType.Bomb) 
                            { // Bomb blows up
                                destroy = true;
                                BulletSpawning(0, BulletType.Explosion, new Vector2(-400, 0), 0);
                                AudioManager.Instance.CallSound("Explosion");
                            } 
                            else
                            { // Add enemy IFrames then make player take damage on collision
                                Player.PlayerInstance.Health = Player.PlayerInstance.Health - 1f;
                            }
                        }
                    }
                }

            } 
            else
            {
                // Resets enemies when off screen so that they go to their starting
                // positions and unload their colliders
                this.Position = new Vector2(initialPosition.X, Position.Y);

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

            if (shotTimer >= (int)(shootDelay * percentageChange))
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
