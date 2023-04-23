using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography;

// BULLET CLASS - By Zane Smith
/* The Bullet class manges all spawned items beyond the scope of the Player, Enemies, and Tiles.
 * Bullets will fly out on the screen with certain set gimmicks (most often tied to enemies), and
 * collide with various things, resulting in them taking damage. Bullets can also be used as a
 * way of creating 'effects' such as additional animations for things that may not involve damaging
 * anything. Bullets currently store a Creator value, which is not in use, however it will become
 * relevant when the Boss is added in. */

namespace Galabingus
{
    /// <summary>
    /// Enumeration for all available bullet types to shoot
    /// </summary>
    public enum BulletType
    {
        PlayerNormal,
        EnemyNormal,
        BouncingSide,
        BouncingCenter,
        Wave,
        Splitter,
        SplitOff,
        Seeker,
        Explosion,
        BigExplosion,
        Heart,
        LazerPath,
        LazerStart,
        LazerAttack
    }

    public enum Targets
    {
        Player,
        Enemies,
        Everything,
        Tiles,
        None
    }

    // Zane Smith

    internal class Bullet : GameObject
    {

        #region-------------------[ Fields ]-------------------
        // Is this bullet ready to be destroyed?
        private bool destroy;

        // Position data
        private Vector2 currentPosition;
        private Vector2 oldPosition;

        // Movement data - uses degrees for storage purposes
        private Vector2 velocity;
        private Vector2 direction;

        // State data
        private BulletType ability;
        private int state_timer;

        // Hit Target Storage (Prevents multihits)
        private List<object> hitObjects;

        // Name used to find values from GameObject dynamic
        private ushort contentName;

        // Number into game object index to look for items
        private ushort bulletNumber;

        // Reference to the object that created the bullet
        private object creator;
        private Targets target;

        // Bouncing Bullet Collision
        private bool didBounce;

        // Collision layer of the bullet
        private CollisionGroup collisionLayer;

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
                return GetPosition(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetPosition(bulletNumber, value);
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
                return GetSprite(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetSprite(bulletNumber, value);
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
                return GetTransform(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetTransform(bulletNumber, value);
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
                return GetScale(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetScale(bulletNumber, value);
            }
        }

        public Animation Animation
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetAnimation(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetAnimation(bulletNumber, value);
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
                return GetCollider(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetCollider(bulletNumber, value);
            }
        }

        #endregion

        #region BULLET SPECIFIC PROPERTIES

        /// <summary>
        /// Used to see if this bullet should be destroyed now
        /// </summary>
        public bool Destroy
        {
            get { return destroy; }
            set { destroy = value; }
        }

        /// <summary>
        /// The direction of the bullet on being spawned. 
        /// Affects Sprite's visuals and movement for some bullets
        /// X Slot:
        ///     1 = Right
        ///     -1 = Left
        ///     0 = No direction (Most bullets have this for X)
        /// Y Slot:
        ///     1 = Up
        ///     -1 = Down
        ///     0 = No direction (ex: Seeker Bullet)
        /// </summary>
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        /// <summary>
        /// What object owns this bullet?
        /// Collision handling is done by targets
        /// </summary>
        public object Creator
        {
            get { return creator; }
        }

        /// <summary>
        /// Returns the bullet number of this bullet (matches with Bullet Manager).
        /// </summary>
        public ushort BulletNumber
        {
            get { return bulletNumber; }
        }

        /// <summary>
        /// Returns what kind of bullet this is.
        /// </summary>
        public BulletType Ability
        {
            get { return ability; }
        }

        #endregion

        #endregion

        #region-------------------[ Constructor ]-------------------

        /// <summary>
        /// Spawns a bullet with given stats
        /// </summary>
        /// <param name="ability">The ability to give the bullet</param>
        /// <param name="position">The position to spawn the bullet at</param>
        /// <param name="direction">The direction of the bullet horizontally and vertically
        ///                         (as needed) If not relevant will be zero</param>
        /// <param name="creator">Reference to the object who created the bullet</param>
        /// <param name="contentName">Name to use for GameObject storage</param>
        /// <param name="bulletNumber">Number to give bullet in GameObject list</param>
        public Bullet (
            BulletType ability,
            Vector2 position,
            Vector2 direction,
            object creator,
            ushort contentName,
            ushort bulletNumber
        ) : base(contentName, 
            bulletNumber, 
                (creator is Player) ? 
                CollisionGroup.FromPlayer : 
                CollisionGroup.Bullet)
        {
            
            #region GAME OBJECT DATA

            // Sets the content itself for the game object to link going forward for this bullet
            this.contentName = contentName;

            // Number in Bullet Manager
            this.bulletNumber = bulletNumber;

            // Determine the collision layer for the bullet
            collisionLayer = ((creator is Player) ? CollisionGroup.FromPlayer : CollisionGroup.Bullet);

            // Set the animation duration
            this.Animation.AnimationDuration = 0.03f;

            // Set Location Data
            if (ability == BulletType.BigExplosion)
            {
                this.Scale = Player.PlayerInstance.Scale * 1.3f;
                this.Position = new Vector2(position.X + Transform.Width * Scale / 2.5f - 50,
                                        position.Y - Transform.Height * Scale / 2.0f);
            }
            else if (ability == BulletType.Explosion)
            {
                this.Scale = Player.PlayerInstance.Scale * 1.3f;
                this.Position = new Vector2(position.X + Transform.Width * Scale / 2.5f,
                                        position.Y - Transform.Height * Scale / 2.0f);
            } 
            else
            {
                this.Scale = Player.PlayerInstance.Scale;
                this.Position = new Vector2(position.X + Transform.Width * Scale / 1.5f,
                                        position.Y - Transform.Height * Scale / 2.0f);
            }

            #endregion

            #region BULLET SPECIFIC DATA

            // Set the ownering object for this bullet
            // See Property for more details
            this.creator = creator;

            // Sets the horizontal and vertical directions
            // See Property for more details
            this.direction = direction;

            // Set type for this bullet
            this.ability = ability;
            state_timer = 0;

            // Sets current and old position based on Game Object
            // Used for calculations
            currentPosition = this.Position;
            oldPosition = this.Position;

            // Set Velocity
            velocity = Vector2.Normalize(direction);

            // Bouncing on Tile collision
            didBounce = false;

            // Set Empty hit objects list
            hitObjects = new List<object>();

            // Establish all Bullet Type specific data
            switch (ability)
            {
                case BulletType.PlayerNormal:
                    // Player: Moves forward quickly, no special features.
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    
                    // Can target: Enemies
                    target = Targets.Enemies;
                    break;

                case BulletType.EnemyNormal:
                    // Red Enemy: Same movement as player's bullets, except slower.
                    GameObject.Instance.Content = GameObject.Instance.Content.enemy_red_bullet_strip4;

                    // Can target: Player
                    target = Targets.Player;
                    break;

                case BulletType.BouncingSide:
                    // Orange Enemy: Moves to the side, is tiny, and bounces off walls.
                    GameObject.Instance.Content = GameObject.Instance.Content.enemy_orange_bullet_45_strip4;

                    // Can target: Player
                    target = Targets.Player;
                    break;

                case BulletType.BouncingCenter:
                    // Orange Enemy: Moves straight, and is tiny.
                    GameObject.Instance.Content = GameObject.Instance.Content.enemy_orange_bullet_90_strip4;

                    // Can target: Player
                    target = Targets.Player;
                    break;

                case BulletType.Splitter:
                    // Green Enemy: When near Player splits into two that move horizontally.
                    GameObject.Instance.Content = GameObject.Instance.Content.enemy_green_bullet_main_strip4;

                    // Can target: Player
                    target = Targets.Player;
                    break;

                case BulletType.SplitOff:
                    // Orange Enemy: Left moving horizontal bullet.
                    GameObject.Instance.Content = GameObject.Instance.Content.enemy_green_bullet_split_strip4;

                    // Can target: Player
                    target = Targets.Player;
                    break;

                case BulletType.Wave:
                    // Yellow Enemy: Big horizontally covering bullet that moves slower.
                    GameObject.Instance.Content = GameObject.Instance.Content.enemy_yellow_bullet_strip3;

                    // Can target: Player
                    target = Targets.Player;
                    break;

                case BulletType.Seeker:
                    // Purple Enemy: Tracks the player, however it eventually loses focus
                    GameObject.Instance.Content = GameObject.Instance.Content.enemy_purple_bullet_strip4;

                    // Can target: Player
                    target = Targets.Player;
                    break;

                case BulletType.Explosion:
                    // Bomb Enemy: Explosion can damage both the player and other enemies!
                    GameObject.Instance.Content = GameObject.Instance.Content.bomb_explosion_strip5;

                    // Can target: Everything - Players & Enemies (not tiles)
                    target = Targets.Everything;
                    this.Animation.AnimationDuration = 0.07f;
                    break;

                case BulletType.BigExplosion:
                    // Bomb Enemy: Explosion can damage both the player and other enemies!
                    GameObject.Instance.Content = GameObject.Instance.Content.big_explode_strip5;

                    // Can target: Everything - Players & Enemies (not tiles)
                    target = Targets.Everything;
                    this.Animation.AnimationDuration = 0.07f;
                    break;

                case BulletType.Heart:
                    // Healing Heart: Tracks the player, however it eventually loses focus
                    GameObject.Instance.Content = GameObject.Instance.Content.heart_bullet_strip4;

                    // Can target: Player
                    target = Targets.Player;
                    break;

                default:
                    // In case of glass break game
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;
            }

            #endregion

        }

        #endregion

        #region-------------------[ Methods ]-------------------

        public void Update(GameTime gameTime)
        {
            // Get old position
            oldPosition = this.Position;

            #region Unique Ability & Stat Management

            // Ability specific setting
            switch (ability)
            {
                case BulletType.PlayerNormal:
                    currentPosition = SetPosition(gameTime, 14, true);
                    break;

                case BulletType.EnemyNormal:
                    currentPosition = SetPosition(gameTime, 6, false);
                    break;

                case BulletType.BouncingSide:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 3, false);

                    // Check for wall collison
                    bool LeftWallHit = this.Position.X < Sprite.Width - this.Transform.Width * this.Scale;
                    bool RightWallHit = this.Position.X > GameObject.Instance.GraphicsDevice.Viewport.Width;

                    
                    // Hit left wall
                    if (LeftWallHit)
                    { // Flip bullet
                        velocity.X *= -1;
                        Position = new Vector2(Position.X + this.Transform.Width * this.Scale, Position.Y);

                        // Set the new boss sprite
                        ushort newSprite = GameObject.Instance.Content.enemy_orange_bullet_135_strip4;
                        this.Sprite = GetSpriteFrom(newSprite, bulletNumber);
                    }

                    // Hit right wall
                    if (RightWallHit)
                    { // Flip bullet
                        velocity.X *= -1;
                        Position = new Vector2(Position.X - this.Transform.Width * this.Scale, Position.Y);

                        // Set the new boss sprite
                        ushort newSprite = GameObject.Instance.Content.enemy_orange_bullet_135_strip4;
                        this.Sprite = GetSpriteFrom(newSprite, bulletNumber);
                    }

                    break;

                case BulletType.BouncingCenter:
                    currentPosition = SetPosition(gameTime, 3, false);
                    break;

                case BulletType.Splitter:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 4, false);

                    // X Position of the player
                    float PlayerY = Player.PlayerInstance.Position.Y    // Base Position
                                    + Player.PlayerInstance.Velocity.Y; // Player Velocity 

                    // Establish Positions to activate split
                    float rightBound = PlayerY;
                    float leftBound = PlayerY;

                    if (direction.Y == -1)
                    { // Facing Right
                        rightBound = (PlayerY + Player.PlayerInstance.Transform.Height * Player.PlayerInstance.Scale * 0.35f) + 1;
                        leftBound = (PlayerY) - 1;
                    } 
                    else if (direction.Y == 1)
                    { // Facing Left
                        rightBound = (PlayerY + Player.PlayerInstance.Transform.Height * Player.PlayerInstance.Scale) + 1;
                        leftBound = (PlayerY + Player.PlayerInstance.Transform.Height * Player.PlayerInstance.Scale * 0.65f) - 1;
                    }

                    // Split into 2 bullets
                    if (currentPosition.Y < rightBound && currentPosition.Y > leftBound && !destroy)
                    {
                        // Create Bullets
                        BulletManager.Instance.CreateBullet(BulletType.SplitOff, currentPosition, new Vector2(1, 0), creator, true);
                        BulletManager.Instance.CreateBullet(BulletType.SplitOff, currentPosition, new Vector2(-1, 0), creator, true);

                        // Tell Bullet Manager to delete this bullet
                        destroy = true;
                    }
                    break;

                case BulletType.SplitOff:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 5, true);
                    break;

                case BulletType.Wave:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 5, false);
                    break;

                case BulletType.Seeker:
                    Player seekerPlayer = Player.PlayerInstance;

                    // Tracks the player initially then holds its velocity
                    if (state_timer < 10)
                    {
                        if (this.Position.Y < seekerPlayer.Position.Y)
                        {
                            // Get Player's Center relative to bullet
                            Vector2 seekerPlayerCenter = new Vector2(seekerPlayer.Position.X + (seekerPlayer.Transform.Width * seekerPlayer.Scale) / 2,
                                                               seekerPlayer.Position.Y + (seekerPlayer.Transform.Height * seekerPlayer.Scale) / 2);

                            // Get Bullet's Center
                            Vector2 seekerBulletCenter = new Vector2(oldPosition.X, oldPosition.Y);

                            // Find vector distance between player and bullet
                            Vector2 seekerPlayerBulletDistance = seekerPlayerCenter - seekerBulletCenter;

                            // Check which way to shift angle
                            if (seekerPlayerBulletDistance.X > 0)
                            {
                                velocity.X = Math.Max(velocity.X + 0.03f, 0.1f);
                            }
                            else
                            {
                                velocity.X = Math.Min(velocity.X - 0.03f, -0.1f);
                            }

                            // Normalize the new velocity
                            Vector2.Normalize(velocity);
                        }
                    }

                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 5, true);
                    break;

                case BulletType.Explosion:
                    // Set Current Position
                    this.Position -= Camera.Instance.OffSet;

                    if (state_timer > 14)
                    {
                        destroy = true;
                        velocity = Vector2.Zero;
                    }
                    break;

                case BulletType.BigExplosion:
                    // Set Current Position
                    this.Position -= Camera.Instance.OffSet;
                    
                    if (state_timer > 14)
                    {
                        destroy = true;
                        velocity = Vector2.Zero;
                    }

                    break;

                case BulletType.Heart:
                    
                    // Change angle over time
                    Player heartPlayer = Player.PlayerInstance;
                    // Get Player's Center relative to bullet
                    Vector2 heartPlayerCenter = new Vector2(heartPlayer.Position.X + (heartPlayer.Transform.Width * heartPlayer.Scale) / 2,
                                                       heartPlayer.Position.Y + (heartPlayer.Transform.Height * heartPlayer.Scale) / 2);
                    // Get Bullet's  Center
                    Vector2 heartBulletCenter = new Vector2(oldPosition.X, oldPosition.Y);
                    // Find vector distance between player and bullet
                    Vector2 heartPlayerBulletDistance = heartPlayerCenter - heartBulletCenter;
                    // Find angle distance between player and bullet
                    double heartPlayerBulletAngle = Math.Atan2(heartPlayerBulletDistance.X, 
                                                          heartPlayerBulletDistance.Y);

                    // Find midpoint between current velocity and player line velocity
                    velocity = Vector2.Normalize(new Vector2((float)(10 * Math.Sin(heartPlayerBulletAngle)), // X
                                                             (float)(10 * Math.Cos(heartPlayerBulletAngle))  // Y
                                                             ));

                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 4, false);

                    break;

                default:
                    // Doesn't move lol
                    break;
            }
            state_timer++;
            #endregion

            // Creates currect collider for Enemy
            this.Transform = this.Animation.Play(gameTime);

            // If the bullet is off the screen, destroy it
            bool bulletOffScreen = this.Position.Y < 0 ||
                                   this.Position.Y > GameObject.Instance.GraphicsDevice.Viewport.Height;

            if (bulletOffScreen && ability != BulletType.Explosion && ability != BulletType.BigExplosion)
            {
                destroy = true;
            }

            // Collisons handled separately bellow
            ColliderHandling();
        }

        /// <summary>
        /// Handles all Collider related matters, from its establishment
        /// to what should be done in the case of various kinds of collisions
        /// </summary>
        private void ColliderHandling()
        {
            #region Create Collider

            // Get position for collider
            Vector2 bulletCenterPosition = this.Position -                              // Base Position
                                           new Vector2(Transform.Width * Scale * 0.5f,  // X Midpoint
                                                       Transform.Height * Scale * 0.5f  // Y Midpoint
                                                       );

            // Tells Collider to use proper visual direction
            SpriteEffects flipping = SpriteEffects.None;

            if (this.Direction.X == -1)
            { // Flip Horizontally
                flipping = SpriteEffects.FlipHorizontally;
            }

            if (this.Direction.Y == 1)
            { // Flip Vertically
                flipping = flipping | SpriteEffects.FlipVertically;
            }

            List<Collision> intercepts = this.Collider.UpdateTransform(
                this.Sprite,                            // Bullet Sprite itself
                this.Position,                          // Position
                this.Transform,                         // Bullet transform for sprite selection
                GameObject.Instance.GraphicsDevice,     // Graphics Device Info
                GameObject.Instance.SpriteBatch,        // Sprite Batcher (carries through)
                0,                                      // Removed old variant of direction (bully Matt to remove this)
                new Vector2(this.Scale, this.Scale),    // Scale
                flipping,                               // Sprite Effects
                (ushort)collisionLayer,                 // Collision Layer
                bulletNumber                            // Bullet Number (tied to Manager)
            );

            // Tells Collider info has been given
            this.Collider.Resolved = true;

            #endregion

            #region Check for Collisions

            // Checks what kind of things can be collided with
            foreach (Collision collision in intercepts)
            {
                if (collision.other != null && !destroy)
                {
                    // Tile specific collision
                    if ((collision.other as Tile) is Tile)
                    {
                        // Check if tile is currently active
                        if (((Tile)collision.other).IsActive)
                        {
                            // Run tile effects on active tile
                            switch (ability)
                            {
                                case BulletType.BouncingSide:
                                    Vector2 overlapZone = collision.overlap;

                                    // Will destroy this bullet if it is lodged in a tile
                                    if (didBounce == false)
                                    {
                                        didBounce = true;

                                        // Pove position over from tile
                                        if (velocity.X > 1)
                                        { // Left
                                            Position = new Vector2(Position.X - this.Transform.Width * this.Scale, Position.Y);
                                            ushort newSprite = GameObject.Instance.Content.enemy_orange_bullet_135_strip4;
                                            this.Sprite = GetSpriteFrom(newSprite, bulletNumber);
                                        }
                                        else if (velocity.X < 1)
                                        { // Right
                                            ushort newSprite = GameObject.Instance.Content.enemy_orange_bullet_135_strip4;
                                            this.Sprite = GetSpriteFrom(newSprite, bulletNumber);
                                            //Position = new Vector2(Position.X + this.Transform.Width * this.Scale, Position.Y);
                                        }

                                        // Flip velocity
                                        velocity.X *= -1;
                                    } 
                                    else
                                    { // Head on collision, destroy bullet
                                        destroy = true;
                                        velocity = Vector2.Zero;
                                    }
                                    break;

                                case BulletType.Explosion:
                                    // Not destroyed nor does it affect tiles
                                    break;

                                case BulletType.Wave:
                                case BulletType.BigExplosion:
                                    // Destroy the touched tiles
                                    ((Tile)collision.other).IsActive = false;
                                    break;

                                case BulletType.Heart:
                                    // Ignorews tiles
                                    break;

                                default:
                                    // Destroy the bullet
                                    destroy = true;
                                    velocity = Vector2.Zero;
                                    break;
                            }
                        } 
                        else
                        {
                            didBounce = false;
                        }
                    }

                    // Collision with player / enemies
                    switch (target)
                    {
                        case Targets.Player:
                            if ((collision.other as Player) is Player)
                            { // Collided object is a player!
                                if ((Player.PlayerInstance.Health - 0.5) >= 0)
                                {
                                    switch (ability)
                                    {
                                        case BulletType.EnemyNormal:
                                            Player.PlayerInstance.Health = Player.PlayerInstance.Health - 1f;
                                            break;

                                        case BulletType.Wave:
                                            Player.PlayerInstance.Health = Player.PlayerInstance.Health - 2f;
                                            break;

                                        case BulletType.Heart:
                                            Player.PlayerInstance.Health = Math.Min(Player.PlayerInstance.Health + 1f, 5);
                                            break;

                                        default:
                                            Player.PlayerInstance.Health = Player.PlayerInstance.Health - 0.5f;
                                            break;
                                    }
                                }

                                // Destroy the bullet
                                destroy = true;
                                velocity = Vector2.Zero;
                            }
                            break;

                        case Targets.Enemies:
                            if ((collision.other as Enemy) is Enemy)
                            { // Collided object is an Enemy
                                ((Enemy)collision.other).Health -= 1;

                                // Kill the enemy if its health is below zero
                                if (((Enemy)collision.other).Health <= 0)
                                {
                                    ((Enemy)collision.other).Destroy = true;
                                }

                                // Destroy the bullet
                                destroy = true;
                                velocity = Vector2.Zero;
                            }
                            break;

                        case Targets.Everything:
                            // Can hit both Enemies and the Player (not tiles).
                            bool hasHit = false;

                            for (int i = 0; i < hitObjects.Count; i++)
                            {
                                if (hitObjects[i] == collision.other)
                                {
                                    hasHit = true;
                                }
                            }

                            if (!hasHit)
                            { // object hasn't yet been hit
                                if ((collision.other as Player) is Player)
                                { // Collided object is a player!
                                    if ((Player.PlayerInstance.Health - 0.5) >= 0)
                                    {
                                        Player.PlayerInstance.Health = Player.PlayerInstance.Health - 1f;
                                    }

                                    hitObjects.Add(collision.other);
                                }
                                else if ((collision.other as Enemy) is Enemy)
                                { // Collided object is an Enemy
                                    switch (ability)
                                    {
                                        case BulletType.BigExplosion:
                                            ((Enemy)collision.other).Health -= 2;
                                            break;

                                        default:
                                            ((Enemy)collision.other).Health -= 1;
                                            break;
                                    }

                                    // Kill the enemy if its health is below zero
                                    if (((Enemy)collision.other).Health <= 0)
                                    {
                                        ((Enemy)collision.other).Destroy = true;
                                    }

                                    hitObjects.Add(collision.other);
                                }
                            }
                            break;
                    }
                }
                
            }

            #endregion
        }

        /// <summary>
        /// Handles changing the player's position normally, then returns the player's current position
        /// </summary>
        /// <param name="gameTime">Game Time Data</param>
        /// <returns></returns>
        private Vector2 SetPosition(GameTime gameTime, int abilitySpeed, bool ignoreCamera)
        {
            int multiplier = 2;

            // Sets position
            Vector2 finalVelocity = (velocity *                                 // Actual velocity
                                     multiplier *
                                     abilitySpeed *                              // Ability specific speed changes
                                     Player.PlayerInstance.TranslationRatio *    
                                     (float)this.Animation.EllapsedTime          // Animation data
                                     );

            // Final position change, and whether or not to include camera movement
            this.Position += finalVelocity;

            // Include camera change?
            if (ignoreCamera)
            {
                if (!Player.PlayerInstance.CameraLock)
                { // In debug mode
                    this.Position = new Vector2(this.Position.X, this.Position.Y - Player.PlayerInstance.Translation.Y);
                }
                else
                { // Normal camera movement
                    this.Position = new Vector2(this.Position.X, this.Position.Y - Camera.Instance.OffSet.Y);
                }
            }

            // Returns position
            return this.Position;
        }

        #endregion

    }
}
